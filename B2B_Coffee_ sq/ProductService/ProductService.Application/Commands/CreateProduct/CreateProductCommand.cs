using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Enums;
using System;

namespace ProductService.Application.Commands.CreateProduct;

public record CreateProductCommand(
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
    int MinimumOrderQuantity,
    Guid CreatedBy
) : IRequest<ProductDto>;
