using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using ProductService.Application.Mappings;

namespace ProductService.Application.Queries.GetProductById;

public class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductDbContext _db;

    public GetProductByIdQueryHandler(IProductDbContext db) => _db = db;

    public async Task<ProductDto?> Handle(
        GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await _db.Products
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(
                p => p.Id == request.Id && p.IsActive, ct);

        return product?.ToDto();
    }
}