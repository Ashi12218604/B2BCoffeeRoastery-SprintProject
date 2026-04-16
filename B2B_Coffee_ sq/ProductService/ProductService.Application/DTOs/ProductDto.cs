using ProductService.Domain.Enums;
using System;

namespace ProductService.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string SKU,
    decimal Price,
    decimal? DiscountedPrice,
    string Origin,
    string Category,
    string RoastLevel,
    double WeightInGrams,
    string ImageUrl,
    bool IsActive,
    bool IsFeatured,
    int MinimumOrderQuantity,
    double AverageRating,
    int ReviewCount,
    DateTime CreatedAt
);

public record CreateProductDto(
    string Name,
    string Description,
    string SKU,
    decimal Price,
    decimal? DiscountedPrice,
    string Origin,
    ProductCategory Category,
    RoastLevel RoastLevel,
    double WeightInGrams,
    string ImageUrl,
    bool IsFeatured,
    int MinimumOrderQuantity
);

public record UpdateProductDto(
    string Name,
    string Description,
    string SKU,
    decimal Price,
    decimal? DiscountedPrice,
    string Origin,
    ProductCategory Category,
    RoastLevel RoastLevel,
    double WeightInGrams,
    string ImageUrl,
    bool IsFeatured,
    bool IsActive,
    int MinimumOrderQuantity
);

public record ProductReviewDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    int Rating,
    string Comment,
    DateTime CreatedAt
);

public record AddReviewDto(
    int Rating,
    string Comment
);