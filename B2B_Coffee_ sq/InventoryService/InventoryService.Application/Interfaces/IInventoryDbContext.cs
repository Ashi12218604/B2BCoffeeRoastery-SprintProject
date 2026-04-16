using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Application.Interfaces;

public interface IInventoryDbContext
{
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<InventoryTransaction> InventoryTransactions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}