using System;

namespace B2B.Contracts.Events.Order;

public interface IOrderRejectedEvent
{
    Guid OrderId { get; }
    Guid ClientId { get; }
    string ClientEmail { get; }
    string ClientName { get; }
    string Reason { get; }
    DateTime RejectedAt { get; }
}