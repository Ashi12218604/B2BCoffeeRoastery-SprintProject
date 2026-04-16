using B2B.Contracts.Events.Order;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Application.Mappings;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Application.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderDto?>
{
    private readonly IOrderDbContext _db;
    private readonly IPublishEndpoint _publish;

    public CancelOrderCommandHandler(IOrderDbContext db, IPublishEndpoint publish)
    {
        _db = db;
        _publish = publish;
    }

    public async Task<OrderDto?> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var query = _db.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory);

        var order = request.AdminRole == "SuperAdmin" 
            ? await query.FirstOrDefaultAsync(o => o.Id == request.OrderId, ct)
            : await query.FirstOrDefaultAsync(o => o.Id == request.OrderId && o.ClientId == request.ClientId, ct);

        if (order is null) return null;

        // Ensure order isn't already dispatched, delivered, rejected, or cancelled
        if (order.Status == OrderStatus.Dispatched ||
            order.Status == OrderStatus.Delivered ||
            order.Status == OrderStatus.Cancelled ||
            order.Status == OrderStatus.Rejected)
        {
            // Business rule: Cannot cancel
            return null;
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        string cancelNote = request.AdminRole == "SuperAdmin"
            ? "Order cancelled by SuperAdmin."
            : "Order cancelled by client.";

        // If paid, mock refund process
        if (order.PaymentStatus == PaymentStatus.Paid)
        {
            order.PaymentStatus = PaymentStatus.Refunded;
            cancelNote += " Refund initiated.";
        }

        order.StatusHistory.Add(new OrderStatusHistory
        {
            Status = OrderStatus.Cancelled,
            Note = cancelNote
        });

        await _db.SaveChangesAsync(ct);

        // Publish status change
        await _publish.Publish<IOrderStatusChangedEvent>(new
        {
            order.Id,
            OrderId = order.Id,
            order.ClientId,
            ClientEmail = order.ClientEmail,
            ClientName = order.ClientName,
            NewStatus = "Cancelled",
            ChangedAt = DateTime.UtcNow,
            TrackingNumber = (string?)null
        }, ct);

        // Publish cancellation so InventoryService restores stock
        await _publish.Publish<IOrderCancelledEvent>(new
        {
            OrderId = order.Id,
            ClientId = order.ClientId,
            ClientEmail = order.ClientEmail,
            ClientName = order.ClientName,
            CancelledAt = DateTime.UtcNow,
            Reason = cancelNote,
            Items = order.Items.Select(i => new OrderCancelledItemDto 
            { 
                ProductId = i.ProductId, 
                Quantity = i.Quantity 
            }).ToList()
        }, ct);

        return order.ToDto();
    }
}
