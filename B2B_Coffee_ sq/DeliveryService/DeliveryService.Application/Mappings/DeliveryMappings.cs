using DeliveryService.Application.DTOs;
using DeliveryService.Domain.Entities;

namespace DeliveryService.Application.Mappings;

public static class DeliveryMappings
{
    public static DeliveryDto ToDto(this Delivery d) => new(
        d.Id,
        d.OrderId,
        d.ClientId,
        d.ClientEmail,
        d.ClientName,
        d.DeliveryAddress,
        d.City,
        d.State,
        d.PinCode,
        d.ApprovedByAdminName,
        d.ProductNames,

        d.Status.ToString(),
        d.TrackingNumber,
        d.AssignedAgent,
        d.AgentPhone,
        d.Notes,
        d.EstimatedDeliveryDate,
        d.ActualDeliveryDate,
        d.StatusHistory
            .OrderBy(h => h.ChangedAt)
            .Select(h => new DeliveryStatusHistoryDto(
                h.Status.ToString(), h.Note,
                h.Location, h.ChangedAt))
            .ToList(),
        d.CreatedAt,
        d.UpdatedAt
    );
}