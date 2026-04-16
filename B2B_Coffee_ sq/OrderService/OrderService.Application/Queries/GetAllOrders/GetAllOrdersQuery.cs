using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Enums;
using System.Collections.Generic;

namespace OrderService.Application.Queries.GetAllOrders;

public record GetAllOrdersQuery(
    OrderStatus? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<List<OrderDto>>;