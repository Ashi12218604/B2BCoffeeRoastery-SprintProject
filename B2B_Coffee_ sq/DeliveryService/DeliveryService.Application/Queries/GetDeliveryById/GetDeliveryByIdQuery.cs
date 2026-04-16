using DeliveryService.Application.DTOs;
using MediatR;
using System;

namespace DeliveryService.Application.Queries.GetDeliveryById;

public record GetDeliveryByIdQuery(Guid Id) : IRequest<DeliveryDto?>;