using DeliveryService.Domain.Enums;
using System;
using System.Collections.Generic;

namespace DeliveryService.Application.DTOs;

public record DeliveryDto(
    Guid Id,
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

    string Status,
    string? TrackingNumber,
    string? AssignedAgent,
    string? AgentPhone,
    string? Notes,
    DateTime? EstimatedDeliveryDate,
    DateTime? ActualDeliveryDate,
    List<DeliveryStatusHistoryDto> StatusHistory,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record DeliveryStatusHistoryDto(
    string Status,
    string? Note,
    string? Location,
    DateTime ChangedAt
);

public record CreateDeliveryDto(
    Guid OrderId,
    Guid ClientId,
    string ClientEmail,
    string ClientName,
    string DeliveryAddress,
    string City,
    string State,
    string PinCode,
    DateTime? EstimatedDeliveryDate,
    string? Notes
);

public record UpdateDeliveryStatusDto(
    DeliveryStatus NewStatus,
    string? Note,
    string? Location,
    string? TrackingNumber,
    string? AssignedAgent,
    string? AgentPhone
);

public record AssignAgentDto(
    string AgentName,
    string AgentPhone
);

public record UpdateDeliveryAddressDto(
    string DeliveryAddress,
    string City,
    string State,
    string PinCode
);