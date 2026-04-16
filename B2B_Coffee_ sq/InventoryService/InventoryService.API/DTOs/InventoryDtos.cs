using System;

namespace InventoryService.API.DTOs;

public record UpsertInventoryDto(
    Guid ProductId,
    string ProductName,
    string SKU,
    int QuantityAvailable,
    int LowStockThreshold
);

public record RestockDto(
    int Quantity,
    string Reason
);
