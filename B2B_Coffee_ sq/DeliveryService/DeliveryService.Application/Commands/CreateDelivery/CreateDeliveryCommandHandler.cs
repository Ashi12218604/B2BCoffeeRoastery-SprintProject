using DeliveryService.Application.DTOs;
using DeliveryService.Application.Interfaces;
using DeliveryService.Application.Mappings;
using DeliveryService.Domain.Entities;
using DeliveryService.Domain.Enums;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryService.Application.Commands.CreateDelivery;

public class CreateDeliveryCommandHandler
    : IRequestHandler<CreateDeliveryCommand, DeliveryDto>
{
    private readonly IDeliveryDbContext _db;

    public CreateDeliveryCommandHandler(IDeliveryDbContext db)
        => _db = db;

    public async Task<DeliveryDto> Handle(
        CreateDeliveryCommand request, CancellationToken ct)
    {
        var delivery = new Delivery
        {
            OrderId = request.OrderId,
            ClientId = request.ClientId,
            ClientEmail = request.ClientEmail,
            ClientName = request.ClientName,
            DeliveryAddress = request.DeliveryAddress,
            City = request.City,
            State = request.State,
            PinCode = request.PinCode,
            ApprovedByAdminName = request.ApprovedByAdminName,
            ProductNames = request.ProductNames,
            Status = DeliveryStatus.Pending,
            EstimatedDeliveryDate = request.EstimatedDeliveryDate,
            Notes = request.Notes,
            TrackingNumber = GenerateTrackingNumber()
        };

        delivery.StatusHistory.Add(new DeliveryStatusHistory
        {
            Status = DeliveryStatus.Pending,
            Note = "Delivery created, awaiting assignment."
        });

        _db.Deliveries.Add(delivery);
        await _db.SaveChangesAsync(ct);
        return delivery.ToDto();
    }

    private static string GenerateTrackingNumber() =>
        $"B2B{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(10000, 99999)}";
}