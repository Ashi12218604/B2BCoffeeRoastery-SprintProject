using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using ProductService.Application.Mappings;

namespace ProductService.Application.Queries.GetAllProducts;

public class GetAllProductsQueryHandler
    : IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductDbContext _db;

    public GetAllProductsQueryHandler(IProductDbContext db) => _db = db;

    public async Task<PagedResult<ProductDto>> Handle(
        GetAllProductsQuery request, CancellationToken ct)
    {
        var query = _db.Products
            .Include(p => p.Reviews)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(p =>
                p.Name.Contains(request.Search) ||
                p.Description.Contains(request.Search) ||
                p.Origin.Contains(request.Search));

        if (request.Category.HasValue)
            query = query.Where(p => p.Category == request.Category);

        if (request.RoastLevel.HasValue)
            query = query.Where(p => p.RoastLevel == request.RoastLevel);

        if (request.IsFeatured.HasValue)
            query = query.Where(p => p.IsFeatured == request.IsFeatured);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.IsFeatured)
            .ThenByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => p.ToDto())
            .ToListAsync(ct);

        return new PagedResult<ProductDto>(
            items, total, request.Page, request.PageSize,
            (int)Math.Ceiling(total / (double)request.PageSize));
    }
}