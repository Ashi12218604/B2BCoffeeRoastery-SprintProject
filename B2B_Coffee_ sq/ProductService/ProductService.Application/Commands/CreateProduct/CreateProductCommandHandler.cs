using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using System.Threading;
using ProductService.Application.Mappings;
using System.Threading.Tasks;

namespace ProductService.Application.Commands.CreateProduct;

public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductDbContext _db;

    public CreateProductCommandHandler(IProductDbContext db) => _db = db;

    public async Task<ProductDto> Handle(
        CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            SKU = request.SKU,
            Price = request.Price,
            DiscountedPrice = request.DiscountedPrice,
            Origin = request.Origin,
            Category = request.Category,
            RoastLevel = request.RoastLevel,
            WeightInGrams = request.WeightInGrams,
            ImageUrl = request.ImageUrl,
            IsFeatured = request.IsFeatured,
            MinimumOrderQuantity = request.MinimumOrderQuantity,
            CreatedBy = request.CreatedBy,
            IsActive = true
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);

        return product.ToDto();
    }
}