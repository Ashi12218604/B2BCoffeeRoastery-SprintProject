using DeliveryService.Application.DTOs;
using MediatR;
using System;

namespace DeliveryService.Application.Commands.AssignAgent;

public record AssignAgentCommand(
    Guid DeliveryId,
    string AgentName,
    string AgentPhone
) : IRequest<DeliveryDto?>;