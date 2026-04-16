using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Enums;
using System;

namespace ProductService.Application.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
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
) : IRequest<ProductDto?>;