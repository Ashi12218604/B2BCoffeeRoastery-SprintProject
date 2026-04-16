using InventoryService.Application.DTOs;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.Mappings;

public static class InventoryMappings
{
    public static InventoryItemDto ToDto(this InventoryItem i) => new(
        i.Id,
        i.ProductId,
        i.ProductName,
        i.SKU,
        i.QuantityAvailable,
        i.ReservedQuantity,
        i.LowStockThreshold,
        i.QuantityAvailable <= i.LowStockThreshold,
        i.UpdatedAt
    );
}