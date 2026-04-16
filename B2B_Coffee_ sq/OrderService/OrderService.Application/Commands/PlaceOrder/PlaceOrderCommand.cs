using MediatR;
using OrderService.Application.DTOs;
using System;
using System.Collections.Generic;

namespace OrderService.Application.Commands.PlaceOrder;

public record PlaceOrderCommand(
    Guid ClientId,
    string ClientEmail,
    string ClientName,
    string CompanyName,
    List<OrderItemRequestDto> Items,
    string? Notes,
    string? DeliveryAddress,
    string? City,
    string? State,
    string? PinCode
) : IRequest<OrderDto>;