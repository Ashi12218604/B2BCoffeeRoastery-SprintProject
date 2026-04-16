using OrderService.Domain.Enums;
using System;
using System.Collections.Generic;

namespace OrderService.Application.DTOs;

public record PlaceOrderDto(
    List<OrderItemRequestDto> Items,
    string? Notes,
    string? DeliveryAddress,
    string? City,
    string? State,
    string? PinCode
);

public record OrderItemRequestDto(
    Guid ProductId,
    string ProductName,
    string SKU,
    int Quantity,
    decimal UnitPrice
);

public record OrderDto(
    Guid Id,
    Guid ClientId,
    string ClientEmail,
    string ClientName,
    string CompanyName,
    string Status,
    decimal TotalAmount,
    string? RejectionReason,
    string? Notes,
    string PaymentStatus,
    string? RazorpayOrderId,
    string? RazorpayPaymentId,
    DateTime? PaidAt,
    List<OrderItemDto> Items,
    List<OrderStatusHistoryDto> StatusHistory,
    DateTime PlacedAt,
    DateTime UpdatedAt
);

public record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string SKU,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal
);

public record OrderStatusHistoryDto(
    string Status,
    string? Note,
    DateTime ChangedAt
);

public record UpdateOrderStatusDto(
    OrderStatus NewStatus,
    string? Note,
    string? TrackingNumber
);

// Payment Flow DTOs
public record PaymentResponseDto(
    bool Success,
    string Message,
    string? RazorpayOrderId = null,
    string? DemoPaymentIdToUseForTest = null,
    string? DemoSignatureToUseForTest = null
);

public record PaymentVerificationDto(
    string RazorpayPaymentId,
    string RazorpaySignature
);

