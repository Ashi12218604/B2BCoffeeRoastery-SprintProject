using System;

namespace B2B.Contracts.Events.Order;

public interface IOrderStatusChangedEvent
{
    Guid OrderId { get; }
    Guid ClientId { get; }
    string ClientEmail { get; }
    string ClientName { get; }
    string NewStatus { get; }   // "In-Process" | "Dispatched" | "Delivered"
    DateTime ChangedAt { get; }
    string? TrackingNumber { get; }
}