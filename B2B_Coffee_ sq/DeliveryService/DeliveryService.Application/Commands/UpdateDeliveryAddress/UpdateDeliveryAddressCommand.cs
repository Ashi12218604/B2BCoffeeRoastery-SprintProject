using DeliveryService.Application.DTOs;
using MediatR;
using System;

namespace DeliveryService.Application.Commands.UpdateDeliveryAddress;

public record UpdateDeliveryAddressCommand(
    Guid DeliveryId,
    string DeliveryAddress,
    string City,
    string State,
    string PinCode
) : IRequest<DeliveryDto?>;
