using ProductService.Application.DTOs;
using ProductService.Domain.Entities;

namespace ProductService.Application.Mappings;

public static class ProductMappings
{
    public static ProductDto ToDto(this Product p) => new(
        p.Id,
        p.Name,
        p.Description,
        p.SKU,
        p.Price,
        p.DiscountedPrice,
        p.Origin,
        p.Category.ToString(),
        p.RoastLevel.ToString(),
        p.WeightInGrams,
        p.ImageUrl,
        p.IsActive,
        p.IsFeatured,
        p.MinimumOrderQuantity,
        p.Reviews.Any()
            ? p.Reviews.Average(r => r.Rating)
            : 0,
        p.Reviews.Count,
        p.CreatedAt
    );
}