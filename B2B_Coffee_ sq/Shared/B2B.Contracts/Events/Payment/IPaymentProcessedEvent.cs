using System;

namespace B2B.Contracts.Events.Payment;

public interface IPaymentProcessedEvent
{
    Guid OrderId { get; }
    Guid ClientId { get; }
    string ClientEmail { get; }
    string ClientName { get; }
    decimal Amount { get; }
    string TransactionId { get; }
    DateTime ProcessedAt { get; }
}