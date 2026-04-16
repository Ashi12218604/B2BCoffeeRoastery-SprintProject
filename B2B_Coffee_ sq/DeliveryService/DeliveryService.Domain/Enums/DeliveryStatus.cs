namespace DeliveryService.Domain.Enums;

public enum DeliveryStatus
{
    Pending = 0,
    Assigned = 1,
    InProcess = 2,
    Dispatched = 3,
    Delivered = 4,
    Failed = 5
}