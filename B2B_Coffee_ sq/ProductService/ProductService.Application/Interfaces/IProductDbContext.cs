using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ProductService.Application.Interfaces;

public interface IProductDbContext
{
    DbSet<Product> Products { get; }
    DbSet<ProductReview> ProductReviews { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}