using System;
using System.Collections.Generic;

namespace B2B.Contracts.Events.Order;

public interface IOrderPlacedEvent
{
    Guid OrderId { get; }
    Guid ClientId { get; }
    string ClientEmail { get; }
    string ClientName { get; }
    List<IOrderItemContract> Items { get; }
    decimal TotalAmount { get; }
    DateTime PlacedAt { get; }
    string? DeliveryAddress { get; }
    string? City { get; }
    string? State { get; }
    string? PinCode { get; }
}

public interface IOrderItemContract
{
    Guid ProductId { get; }
    string ProductName { get; }
    int Quantity { get; }
    decimal UnitPrice { get; }
}