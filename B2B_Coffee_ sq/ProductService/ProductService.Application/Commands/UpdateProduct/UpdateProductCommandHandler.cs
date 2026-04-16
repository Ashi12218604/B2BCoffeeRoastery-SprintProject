using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Application.Mappings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProductService.Application.Commands.UpdateProduct;

public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, ProductDto?>
{
    private readonly IProductDbContext _db;

    public UpdateProductCommandHandler(IProductDbContext db) => _db = db;

    public async Task<ProductDto?> Handle(
        UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _db.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (product is null) return null;

        product.Name = request.Name;
        product.Description = request.Description;
        product.SKU = request.SKU;
        product.Price = request.Price;
        product.DiscountedPrice = request.DiscountedPrice;
        product.Origin = request.Origin;
        product.Category = request.Category;
        product.RoastLevel = request.RoastLevel;
        product.WeightInGrams = request.WeightInGrams;
        product.ImageUrl = request.ImageUrl;
        product.IsFeatured = request.IsFeatured;
        product.IsActive = request.IsActive;
        product.MinimumOrderQuantity = request.MinimumOrderQuantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return product.ToDto();
    }
}
