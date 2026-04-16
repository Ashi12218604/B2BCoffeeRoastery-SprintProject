using System;

namespace InventoryService.Application.DTOs;

public record InventoryItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string SKU,
    int QuantityAvailable,
    int ReservedQuantity,
    int LowStockThreshold,
    bool IsLowStock,
    DateTime UpdatedAt
);

public record InventoryTransactionDto(
    Guid Id,
    Guid? OrderId,
    string Type,
    int Quantity,
    int QuantityBefore,
    int QuantityAfter,
    string Reason,
    DateTime CreatedAt
);
