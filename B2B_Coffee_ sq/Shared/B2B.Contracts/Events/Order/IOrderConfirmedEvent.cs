using System;

namespace B2B.Contracts.Events.Order;

public interface IOrderConfirmedEvent
{
    Guid OrderId { get; }
    Guid ClientId { get; }
    string ClientEmail { get; }
    string ClientName { get; }
    decimal TotalAmount { get; }
    string? AdminName { get; }
    List<string> ProductNames { get; }
    string? DeliveryAddress { get; }
    string? City { get; }
    string? State { get; }
    string? PinCode { get; }
    DateTime ConfirmedAt { get; }
}