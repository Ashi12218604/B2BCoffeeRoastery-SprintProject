using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Enums;
using System.Collections.Generic;

namespace ProductService.Application.Queries.GetAllProducts;

public record GetAllProductsQuery(
    string? Search = null,
    ProductCategory? Category = null,
    RoastLevel? RoastLevel = null,
    bool? IsFeatured = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedResult<ProductDto>>;

public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);