using System;
using System.Collections.Generic;

namespace InventoryService.Domain.Entities;

public class InventoryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }          // Same ID as ProductService
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int QuantityAvailable { get; set; }
    public int ReservedQuantity { get; set; }    // Held during saga
    public int LowStockThreshold { get; set; } = 20;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<InventoryTransaction> Transactions { get; set; }
        = new List<InventoryTransaction>();
}