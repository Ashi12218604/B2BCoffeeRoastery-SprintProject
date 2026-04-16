using DeliveryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryService.Application.Interfaces;

public interface IDeliveryDbContext
{
    DbSet<Delivery> Deliveries { get; }
    DbSet<DeliveryStatusHistory> DeliveryStatusHistories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}