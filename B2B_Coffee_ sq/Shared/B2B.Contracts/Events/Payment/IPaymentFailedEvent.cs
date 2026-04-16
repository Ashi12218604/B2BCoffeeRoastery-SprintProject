using System;

namespace B2B.Contracts.Events.Payment;

public interface IPaymentFailedEvent
{
    Guid OrderId { get; }
    Guid CorrelationId { get; }
    string Reason { get; }
    DateTime FailedAt { get; }
}