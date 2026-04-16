using B2B.Contracts.Events.Inventory;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Application.Commands.DeductInventory;

public class DeductInventoryCommandHandler
    : IRequestHandler<DeductInventoryCommand, DeductInventoryResult>
{
    private readonly IInventoryDbContext _db;
    private readonly IPublishEndpoint _publish;

    public DeductInventoryCommandHandler(
        IInventoryDbContext db, IPublishEndpoint publish)
    {
        _db = db;
        _publish = publish;
    }

    public async Task<DeductInventoryResult> Handle(
        DeductInventoryCommand request, CancellationToken ct)
    {
        // Load all items involved in this order
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var inventoryItems = await _db.InventoryItems
            .Where(i => productIds.Contains(i.ProductId))
            .ToListAsync(ct);

        // Validate all items have enough stock
        foreach (var orderItem in request.Items)
        {
            var inv = inventoryItems
                .FirstOrDefault(i => i.ProductId == orderItem.ProductId);

            if (inv is null)
            {
                await PublishFailed(request,
                    $"Product {orderItem.ProductId} not found in inventory.",
                    ct);
                return new DeductInventoryResult(false,
                    $"Product {orderItem.ProductId} not found.");
            }

            var available = inv.QuantityAvailable - inv.ReservedQuantity;
            if (available < orderItem.Quantity)
            {
                await PublishFailed(request,
                    $"Insufficient stock for {inv.ProductName}. " +
                    $"Available: {available}, Requested: {orderItem.Quantity}",
                    ct);
                return new DeductInventoryResult(false,
                    $"Insufficient stock for '{inv.ProductName}'.");
            }
        }

        // All checks passed — deduct
        foreach (var orderItem in request.Items)
        {
            var inv = inventoryItems
                .First(i => i.ProductId == orderItem.ProductId);
            var before = inv.QuantityAvailable;

            inv.QuantityAvailable -= orderItem.Quantity;
            inv.UpdatedAt = DateTime.UtcNow;

            _db.InventoryTransactions.Add(new InventoryTransaction
            {
                InventoryItemId = inv.Id,
                OrderId = request.OrderId,
                Type = "Deduct",
                Quantity = orderItem.Quantity,
                QuantityBefore = before,
                QuantityAfter = inv.QuantityAvailable,
                Reason = $"Order {request.OrderId}"
            });
        }

        await _db.SaveChangesAsync(ct);

        // Publish success event → Saga continues
        await _publish.Publish<IInventoryDeductedEvent>(new
        {
            request.OrderId,
            CorrelationId = request.CorrelationId,
            DeductedAt = DateTime.UtcNow
        }, ct);

        return new DeductInventoryResult(true);
    }

    private async Task PublishFailed(
        DeductInventoryCommand request, string reason,
        CancellationToken ct) =>
        await _publish.Publish<IInventoryDeductionFailedEvent>(new
        {
            request.OrderId,
            CorrelationId = request.CorrelationId,
            Reason = reason,
            FailedAt = DateTime.UtcNow
        }, ct);
}