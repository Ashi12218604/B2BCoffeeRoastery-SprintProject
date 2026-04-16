using B2B.Contracts.Events.Order;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Consumers;

// Updates order status in DB when saga rejects
public class OrderRejectedConsumer : IConsumer<IOrderRejectedEvent>
{
    private readonly OrderDbContext _db;

    public OrderRejectedConsumer(OrderDbContext db) => _db = db;

    public async Task Consume(ConsumeContext<IOrderRejectedEvent> context)
    {
        var msg = context.Message;
        var order = await _db.Orders
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == msg.OrderId);

        if (order is null) return;

        order.Status = OrderStatus.Rejected;
        order.RejectionReason = msg.Reason;
        order.UpdatedAt = DateTime.UtcNow;
        order.StatusHistory.Add(new OrderStatusHistory
        {
            Status = OrderStatus.Rejected,
            Note = msg.Reason
        });

        await _db.SaveChangesAsync();
        Console.WriteLine(
            $"[OrderService] Order {msg.OrderId} rejected: {msg.Reason}");
    }
}