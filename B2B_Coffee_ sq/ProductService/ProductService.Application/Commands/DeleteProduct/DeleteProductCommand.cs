using MediatR;
using System;

namespace ProductService.Application.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest<bool>;