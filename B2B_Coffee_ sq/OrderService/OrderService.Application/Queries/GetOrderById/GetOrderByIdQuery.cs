using MediatR;
using OrderService.Application.DTOs;
using System;

namespace OrderService.Application.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId, Guid? ClientId = null)
    : IRequest<OrderDto?>;