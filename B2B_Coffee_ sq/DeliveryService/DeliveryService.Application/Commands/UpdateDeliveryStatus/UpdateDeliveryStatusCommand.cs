using DeliveryService.Application.DTOs;
using DeliveryService.Domain.Enums;
using MediatR;
using System;

namespace DeliveryService.Application.Commands.UpdateDeliveryStatus;

public record UpdateDeliveryStatusCommand(
    Guid DeliveryId,
    DeliveryStatus NewStatus,
    string? Note,
    string? Location,
    string? TrackingNumber,
    string? AssignedAgent,
    string? AgentPhone
) : IRequest<DeliveryDto?>;