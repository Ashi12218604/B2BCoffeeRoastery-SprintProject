using DeliveryService.Application.DTOs;
using MediatR;
using System;

namespace DeliveryService.Application.Queries.GetDeliveryByOrder;

public record GetDeliveryByOrderQuery(Guid OrderId)
    : IRequest<DeliveryDto?>;