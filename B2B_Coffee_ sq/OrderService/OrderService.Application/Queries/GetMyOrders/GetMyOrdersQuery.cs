using MediatR;
using OrderService.Application.DTOs;
using System;
using System.Collections.Generic;

namespace OrderService.Application.Queries.GetMyOrders;

public record GetMyOrdersQuery(Guid ClientId)
    : IRequest<List<OrderDto>>;