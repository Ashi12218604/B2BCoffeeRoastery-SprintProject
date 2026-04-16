using System;

namespace B2B.Contracts.Events.Inventory;

public interface IInventoryDeductionFailedEvent
{
    Guid OrderId { get; }
    Guid CorrelationId { get; }
    string Reason { get; }
    DateTime FailedAt { get; }
}