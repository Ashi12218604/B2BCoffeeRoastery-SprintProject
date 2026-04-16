using System;

namespace InventoryService.Domain.Entities;

public class InventoryTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InventoryItemId { get; set; }
    public Guid? OrderId { get; set; }
    public string Type { get; set; } = string.Empty;
    // "Deduct" | "Restore" | "Restock" | "Reserve" | "Release"
    public int Quantity { get; set; }
    public int QuantityBefore { get; set; }
    public int QuantityAfter { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public InventoryItem InventoryItem { get; set; } = null!;
}