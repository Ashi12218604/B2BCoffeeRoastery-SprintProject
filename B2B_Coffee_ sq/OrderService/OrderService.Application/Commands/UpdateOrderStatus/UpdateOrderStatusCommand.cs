using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Enums;
using System;

namespace OrderService.Application.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus,
    string? Note,
    string? TrackingNumber
) : IRequest<OrderDto?>;