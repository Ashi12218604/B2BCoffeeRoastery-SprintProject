using MediatR;
using ProductService.Application.DTOs;
using System;
using System.Collections.Generic;

namespace ProductService.Application.Queries.GetProductReviews;

public record GetProductReviewsQuery(Guid ProductId)
    : IRequest<List<ProductReviewDto>>;