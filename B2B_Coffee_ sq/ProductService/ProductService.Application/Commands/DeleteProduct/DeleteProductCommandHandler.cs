using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using ProductService.Application.Mappings;

namespace ProductService.Application.Commands.DeleteProduct;

public class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductDbContext _db;

    public DeleteProductCommandHandler(IProductDbContext db) => _db = db;

    public async Task<bool> Handle(
        DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _db.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (product is null) return false;

        // Soft delete
        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}