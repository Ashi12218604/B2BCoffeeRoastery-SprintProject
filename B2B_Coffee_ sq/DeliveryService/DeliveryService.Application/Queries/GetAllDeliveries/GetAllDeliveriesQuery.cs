using DeliveryService.Application.DTOs;
using DeliveryService.Domain.Enums;
using MediatR;
using System.Collections.Generic;

namespace DeliveryService.Application.Queries.GetAllDeliveries;

public record GetAllDeliveriesQuery(
    DeliveryStatus? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<List<DeliveryDto>>;