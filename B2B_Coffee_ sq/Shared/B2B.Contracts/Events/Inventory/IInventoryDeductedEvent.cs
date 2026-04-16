using System;

namespace B2B.Contracts.Events.Inventory;

public interface IInventoryDeductedEvent
{
	Guid OrderId { get; }
	Guid CorrelationId { get; }
	DateTime DeductedAt { get; }
}