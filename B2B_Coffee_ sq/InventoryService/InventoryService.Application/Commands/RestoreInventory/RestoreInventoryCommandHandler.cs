using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Application.Commands.RestoreInventory;

// Compensating transaction — called by Saga on order failure
public class RestoreInventoryCommandHandler
    : IRequestHandler<RestoreInventoryCommand, bool>
{
    private readonly IInventoryDbContext _db;

    public RestoreInventoryCommandHandler(IInventoryDbContext db)
        => _db = db;

    public async Task<bool> Handle(
        RestoreInventoryCommand request, CancellationToken ct)
    {
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var items = await _db.InventoryItems
            .Where(i => productIds.Contains(i.ProductId))
            .ToListAsync(ct);

        foreach (var orderItem in request.Items)
        {
            var inv = items.FirstOrDefault(
                i => i.ProductId == orderItem.ProductId);
            if (inv is null) continue;

            var before = inv.QuantityAvailable;
            inv.QuantityAvailable += orderItem.Quantity;
            inv.UpdatedAt = DateTime.UtcNow;

            _db.InventoryTransactions.Add(new InventoryTransaction
            {
                InventoryItemId = inv.Id,
                OrderId = request.OrderId,
                Type = "Restore",
                Quantity = orderItem.Quantity,
                QuantityBefore = before,
                QuantityAfter = inv.QuantityAvailable,
                Reason = request.Reason
            });
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }
}