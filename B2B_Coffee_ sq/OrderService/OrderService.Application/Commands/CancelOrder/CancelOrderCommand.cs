using MediatR;
using OrderService.Application.DTOs;
using System;

namespace OrderService.Application.Commands.CancelOrder;

public record CancelOrderCommand(
    Guid OrderId,
    Guid? ClientId,
    string? AdminRole = null
) : IRequest<OrderDto?>;
