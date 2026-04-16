using System;

namespace B2B.Contracts.Events.Delivery;

public interface IDeliveryStatusChangedEvent
{
    Guid DeliveryId { get; }
    Guid OrderId { get; }
    string ClientEmail { get; }
    string ClientName { get; }
    string Status { get; }   // "Dispatched" | "Delivered"
    string? TrackingNumber { get; }
    DateTime ChangedAt { get; }
}