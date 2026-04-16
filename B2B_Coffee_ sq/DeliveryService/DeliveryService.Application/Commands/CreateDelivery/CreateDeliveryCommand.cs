using DeliveryService.Application.DTOs;
using MediatR;
using System;

namespace DeliveryService.Application.Commands.CreateDelivery;

public record CreateDeliveryCommand(
    Guid OrderId,
    Guid ClientId,
    string ClientEmail,
    string ClientName,
    string DeliveryAddress,
    string City,
    string State,
    string PinCode,
    string? ApprovedByAdminName,
    string? ProductNames,
    DateTime? EstimatedDeliveryDate,
    string? Notes
) : IRequest<DeliveryDto>;