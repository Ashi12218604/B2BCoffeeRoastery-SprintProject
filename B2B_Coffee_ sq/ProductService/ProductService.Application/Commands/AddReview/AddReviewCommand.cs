using MediatR;
using ProductService.Application.DTOs;
using System;

namespace ProductService.Application.Commands.AddReview;

public record AddReviewCommand(
	Guid ProductId,
	Guid ClientId,
	string ClientName,
	int Rating,
	string Comment
) : IRequest<ProductReviewDto?>;