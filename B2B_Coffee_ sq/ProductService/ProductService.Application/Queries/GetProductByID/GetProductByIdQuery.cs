using MediatR;
using ProductService.Application.DTOs;
using System;

namespace ProductService.Application.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;