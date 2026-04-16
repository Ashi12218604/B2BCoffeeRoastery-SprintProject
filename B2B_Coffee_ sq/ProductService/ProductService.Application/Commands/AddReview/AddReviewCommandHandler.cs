using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using ProductService.Application.Mappings;

namespace ProductService.Application.Commands.AddReview;

public class AddReviewCommandHandler
    : IRequestHandler<AddReviewCommand, ProductReviewDto?>
{
    private readonly IProductDbContext _db;

    public AddReviewCommandHandler(IProductDbContext db) => _db = db;

    public async Task<ProductReviewDto?> Handle(
        AddReviewCommand request, CancellationToken ct)
    {
        var exists = await _db.Products
            .AnyAsync(p => p.Id == request.ProductId && p.IsActive, ct);

        if (!exists) return null;

        if (request.Rating < 1 || request.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        var review = new ProductReview
        {
            ProductId = request.ProductId,
            ClientId = request.ClientId,
            ClientName = request.ClientName,
            Rating = request.Rating,
            Comment = request.Comment
        };

        _db.ProductReviews.Add(review);
        await _db.SaveChangesAsync(ct);

        return new ProductReviewDto(
            review.Id, review.ClientId, review.ClientName,
            review.Rating, review.Comment, review.CreatedAt);
    }
}