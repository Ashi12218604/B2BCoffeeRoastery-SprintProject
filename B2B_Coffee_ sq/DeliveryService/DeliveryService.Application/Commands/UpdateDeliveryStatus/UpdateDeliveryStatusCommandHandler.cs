using B2B.Contracts.Events.Delivery;
using DeliveryService.Application.DTOs;
using DeliveryService.Application.Interfaces;
using DeliveryService.Application.Mappings;
using DeliveryService.Domain.Entities;
using DeliveryService.Domain.Enums;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryService.Application.Commands.UpdateDeliveryStatus;

public class UpdateDeliveryStatusCommandHandler
    : IRequestHandler<UpdateDeliveryStatusCommand, DeliveryDto?>
{
    private readonly IDeliveryDbContext _db;
    private readonly IPublishEndpoint _publish;

    public UpdateDeliveryStatusCommandHandler(
        IDeliveryDbContext db, IPublishEndpoint publish)
    {
        _db = db;
        _publish = publish;
    }

    public async Task<DeliveryDto?> Handle(
        UpdateDeliveryStatusCommand request, CancellationToken ct)
    {
        var delivery = await _db.Deliveries
            .Include(d => d.StatusHistory)
            .FirstOrDefaultAsync(d => d.Id == request.DeliveryId, ct);

        if (delivery is null) return null;

        delivery.Status = request.NewStatus;
        delivery.UpdatedAt = DateTime.UtcNow;

        if (request.TrackingNumber is not null)
            delivery.TrackingNumber = request.TrackingNumber;

        if (request.AssignedAgent is not null)
            delivery.AssignedAgent = request.AssignedAgent;

        if (request.AgentPhone is not null)
            delivery.AgentPhone = request.AgentPhone;

        if (request.NewStatus == DeliveryStatus.Delivered)
            delivery.ActualDeliveryDate = DateTime.UtcNow;

        _db.DeliveryStatusHistories.Add(new DeliveryStatusHistory
        {
            DeliveryId = delivery.Id,
            Status = request.NewStatus,
            Note = request.Note,
            Location = request.Location
        });

        await _db.SaveChangesAsync(ct);

        // Publish to RabbitMQ → NotificationService sends email
        // Only for statuses that trigger client notifications
        if (request.NewStatus is DeliveryStatus.Dispatched
                              or DeliveryStatus.Delivered
                              or DeliveryStatus.InProcess)
        {
            var statusLabel = request.NewStatus switch
            {
                DeliveryStatus.InProcess => "In-Process",
                DeliveryStatus.Dispatched => "Dispatched",
                DeliveryStatus.Delivered => "Delivered",
                _ => request.NewStatus.ToString()
            };

            await _publish.Publish<IDeliveryStatusChangedEvent>(new
            {
                DeliveryId = delivery.Id,
                OrderId = delivery.OrderId,
                ClientEmail = delivery.ClientEmail,
                ClientName = delivery.ClientName,
                Status = statusLabel,
                TrackingNumber = delivery.TrackingNumber,
                ChangedAt = DateTime.UtcNow
            }, ct);
        }

        return delivery.ToDto();
    }
}