namespace OrderService.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    InProcess = 2,
    Dispatched = 3,
    Delivered = 4,
    Rejected = 5,
    Cancelled = 6
}