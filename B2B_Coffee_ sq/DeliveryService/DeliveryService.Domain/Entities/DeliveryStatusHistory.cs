using DeliveryService.Domain.Enums;
using System;

namespace DeliveryService.Domain.Entities;

public class DeliveryStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeliveryId { get; set; }
    public DeliveryStatus Status { get; set; }
    public string? Note { get; set; }
    public string? Location { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public Delivery Delivery { get; set; } = null!;
}