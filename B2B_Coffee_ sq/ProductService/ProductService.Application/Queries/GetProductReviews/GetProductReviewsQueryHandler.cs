using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProductService.Application.Mappings;

namespace ProductService.Application.Queries.GetProductReviews;

public class GetProductReviewsQueryHandler
    : IRequestHandler<GetProductReviewsQuery, List<ProductReviewDto>>
{
    private readonly IProductDbContext _db;

    public GetProductReviewsQueryHandler(IProductDbContext db) => _db = db;

    public async Task<List<ProductReviewDto>> Handle(
        GetProductReviewsQuery request, CancellationToken ct) =>
        await _db.ProductReviews
            .Where(r => r.ProductId == request.ProductId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ProductReviewDto(
                r.Id, r.ClientId, r.ClientName,
                r.Rating, r.Comment, r.CreatedAt))
            .ToListAsync(ct);
}