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

namespace OrderService.Application.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler
    : IRequestHandler<UpdateOrderStatusCommand, OrderDto?>
{
    private readonly IOrderDbContext _db;
    private readonly IPublishEndpoint _publish;

    public UpdateOrderStatusCommandHandler(
        IOrderDbContext db, IPublishEndpoint publish)
    {
        _db = db;
        _publish = publish;
    }

    public async Task<OrderDto?> Handle(
        UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null) return null;

        order.Status = request.NewStatus;
        order.UpdatedAt = DateTime.UtcNow;
        if (request.NewStatus == OrderStatus.Rejected)
        {
            order.RejectionReason = request.Note;
        }

        _db.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = request.NewStatus,
            Note = request.Note
        });

        await _db.SaveChangesAsync(ct);

        // Publish status change → NotificationService sends email
        var statusLabel = request.NewStatus.ToString() switch
        {
            "InProcess" => "In-Process",
            "Dispatched" => "Dispatched",
            "Delivered" => "Delivered",
            _ => request.NewStatus.ToString()
        };

        await _publish.Publish<IOrderStatusChangedEvent>(new
        {
            order.Id,
            OrderId = order.Id,
            order.ClientId,
            ClientEmail = order.ClientEmail,
            ClientName = order.ClientName,
            NewStatus = statusLabel,
            ChangedAt = DateTime.UtcNow,
            TrackingNumber = request.TrackingNumber
        }, ct);

        return order.ToDto();
    }
}