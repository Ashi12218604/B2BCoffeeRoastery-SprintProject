using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace InventoryService.Infrastructure.Data;

public class InventoryDbContext : DbContext, IInventoryDbContext
{
    public InventoryDbContext(
        DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<InventoryItem> InventoryItems { get; set; } = null!;
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<InventoryItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ProductId).IsUnique();
            e.HasIndex(x => x.SKU).IsUnique();
            e.HasMany(x => x.Transactions)
             .WithOne(x => x.InventoryItem)
             .HasForeignKey(x => x.InventoryItemId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InventoryTransaction>(e =>
        {
            e.HasKey(x => x.Id);
        });

        // Seed inventory matching ProductService seed products
        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000001"),
                ProductId = Guid.Parse("A1B2C3D4-1111-4AAA-BBBB-000000000001"),
                ProductName = "Ethiopian Yirgacheffe",
                SKU = "ETH-YIR-01",
                QuantityAvailable = 150,
                ReservedQuantity = 0,
                LowStockThreshold = 20,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new InventoryItem
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000002"),
                ProductId = Guid.Parse("A1B2C3D4-2222-4AAA-BBBB-000000000002"),
                ProductName = "Colombian Supremo",
                SKU = "COL-SUP-01",
                QuantityAvailable = 200,
                ReservedQuantity = 0,
                LowStockThreshold = 25,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new InventoryItem
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000003"),
                ProductId = Guid.Parse("A1B2C3D4-3333-4AAA-BBBB-000000000003"),
                ProductName = "Italian Espresso Classico",
                SKU = "ESP-ITA-01",
                QuantityAvailable = 300,
                ReservedQuantity = 0,
                LowStockThreshold = 30,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new InventoryItem
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000004"),
                ProductId = Guid.Parse("A1B2C3D4-4444-4AAA-BBBB-000000000004"),
                ProductName = "Sumatra Mandheling",
                SKU = "SUM-MAN-01",
                QuantityAvailable = 100,
                ReservedQuantity = 0,
                LowStockThreshold = 15,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new InventoryItem
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000005"),
                ProductId = Guid.Parse("A1B2C3D4-5555-4AAA-BBBB-000000000005"),
                ProductName = "Guatemala Antigua",
                SKU = "GUA-ANT-01",
                QuantityAvailable = 180,
                ReservedQuantity = 0,
                LowStockThreshold = 20,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new InventoryItem
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000006"),
                ProductId = Guid.Parse("A1B2C3D4-6666-4AAA-BBBB-000000000006"),
                ProductName = "Swiss Water Decaf Blend",
                SKU = "DEC-SWS-01",
                QuantityAvailable = 80,
                ReservedQuantity = 0,
                LowStockThreshold = 15,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new InventoryItem
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000007"),
                ProductId = Guid.Parse("A1B2C3D4-7777-4AAA-BBBB-000000000007"),
                ProductName = "Kenya AA Peaberry",
                SKU = "KEN-PEA-01",
                QuantityAvailable = 60,
                ReservedQuantity = 0,
                LowStockThreshold = 10,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new InventoryItem
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000008"),
                ProductId = Guid.Parse("A1B2C3D4-8888-4AAA-BBBB-000000000008"),
                ProductName = "Cold Brew Concentrate",
                SKU = "CLD-BRW-01",
                QuantityAvailable = 250,
                ReservedQuantity = 0,
                LowStockThreshold = 30,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new InventoryItem
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000009"),
                ProductId = Guid.Parse("A1B2C3D4-9999-4AAA-BBBB-000000000009"),
                ProductName = "Costa Rica Tarrazu",
                SKU = "CRC-TAR-01",
                QuantityAvailable = 120,
                ReservedQuantity = 0,
                LowStockThreshold = 15,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new InventoryItem
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000010"),
                ProductId = Guid.Parse("A1B2C3D4-AAAA-4AAA-BBBB-000000000010"),
                ProductName = "Midnight Mocha Blend",
                SKU = "BLD-MOC-01",
                QuantityAvailable = 220,
                ReservedQuantity = 0,
                LowStockThreshold = 25,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            }
        );
    }
}