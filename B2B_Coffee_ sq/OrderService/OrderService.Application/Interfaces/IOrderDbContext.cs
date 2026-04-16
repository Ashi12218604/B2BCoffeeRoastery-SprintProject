using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IOrderDbContext
{
    DbSet<Order> Orders { get; set; }
    DbSet<OrderItem> OrderItems { get; set; }
    DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    DbSet<OrderSagaState> OrderSagaStates { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
