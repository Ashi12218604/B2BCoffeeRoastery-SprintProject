using System;
using System.Collections.Generic;

namespace B2B.Contracts.Events.Order;

public interface IOrderCancelledEvent
{
    Guid OrderId { get; }
    Guid ClientId { get; }
    string ClientEmail { get; }
    string ClientName { get; }
    DateTime CancelledAt { get; }
    string Reason { get; }
    List<OrderCancelledItemDto> Items { get; }
}

public class OrderCancelledItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
