using B2B.Contracts.Events.Order;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Consumers;

// Updates order status in DB when saga confirms
public class OrderConfirmedConsumer : IConsumer<IOrderConfirmedEvent>
{
    private readonly OrderDbContext _db;

    public OrderConfirmedConsumer(OrderDbContext db) => _db = db;

    public async Task Consume(ConsumeContext<IOrderConfirmedEvent> context)
    {
        var msg = context.Message;
        
        // Use direct update to avoid concurrency tracking issues (ignores current version)
        await _db.Orders
            .Where(o => o.Id == msg.OrderId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Status, OrderStatus.Confirmed)
                .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));

        // Add history record separately with a safety check
        try 
        {
            _db.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = msg.OrderId,
                Status = OrderStatus.Confirmed,
                Note = "Payment confirmed. Order is being processed. (Saga Background)"
            });
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException) 
        {
            // Likely already exists or concurrent update to history table (safe to ignore)
        }

        Console.WriteLine($"[OrderService] Order {msg.OrderId} status confirmed via background consumer.");
    }
}