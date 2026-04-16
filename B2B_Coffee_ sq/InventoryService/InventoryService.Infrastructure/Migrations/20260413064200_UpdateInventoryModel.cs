using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InventoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInventoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000001"),
                columns: new[] { "LowStockThreshold", "ProductId", "QuantityAvailable", "SKU" },
                values: new object[] { 20, new Guid("a1b2c3d4-1111-4aaa-bbbb-000000000001"), 150, "ETH-YIR-01" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000002"),
                columns: new[] { "LowStockThreshold", "ProductId", "QuantityAvailable", "SKU" },
                values: new object[] { 25, new Guid("a1b2c3d4-2222-4aaa-bbbb-000000000002"), 200, "COL-SUP-01" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000003"),
                columns: new[] { "LowStockThreshold", "ProductId", "ProductName", "QuantityAvailable", "SKU" },
                values: new object[] { 30, new Guid("a1b2c3d4-3333-4aaa-bbbb-000000000003"), "Italian Espresso Classico", 300, "ESP-ITA-01" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000004"),
                columns: new[] { "LowStockThreshold", "ProductId", "ProductName", "QuantityAvailable", "SKU" },
                values: new object[] { 15, new Guid("a1b2c3d4-4444-4aaa-bbbb-000000000004"), "Sumatra Mandheling", 100, "SUM-MAN-01" });

            migrationBuilder.InsertData(
                table: "InventoryItems",
                columns: new[] { "Id", "CreatedAt", "LowStockThreshold", "ProductId", "ProductName", "QuantityAvailable", "ReservedQuantity", "SKU", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("22222222-0000-0000-0000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 20, new Guid("a1b2c3d4-5555-4aaa-bbbb-000000000005"), "Guatemala Antigua", 180, 0, "GUA-ANT-01", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("22222222-0000-0000-0000-000000000006"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 15, new Guid("a1b2c3d4-6666-4aaa-bbbb-000000000006"), "Swiss Water Decaf Blend", 80, 0, "DEC-SWS-01", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("22222222-0000-0000-0000-000000000007"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, new Guid("a1b2c3d4-7777-4aaa-bbbb-000000000007"), "Kenya AA Peaberry", 60, 0, "KEN-PEA-01", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("22222222-0000-0000-0000-000000000008"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, new Guid("a1b2c3d4-8888-4aaa-bbbb-000000000008"), "Cold Brew Concentrate", 250, 0, "CLD-BRW-01", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("22222222-0000-0000-0000-000000000009"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 15, new Guid("a1b2c3d4-9999-4aaa-bbbb-000000000009"), "Costa Rica Tarrazu", 120, 0, "CRC-TAR-01", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("22222222-0000-0000-0000-000000000010"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 25, new Guid("a1b2c3d4-aaaa-4aaa-bbbb-000000000010"), "Midnight Mocha Blend", 220, 0, "BLD-MOC-01", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000010"));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000001"),
                columns: new[] { "LowStockThreshold", "ProductId", "QuantityAvailable", "SKU" },
                values: new object[] { 50, new Guid("11111111-0000-0000-0000-000000000001"), 500, "COFFEE-ETH-001" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000002"),
                columns: new[] { "LowStockThreshold", "ProductId", "QuantityAvailable", "SKU" },
                values: new object[] { 50, new Guid("11111111-0000-0000-0000-000000000002"), 350, "COFFEE-COL-001" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000003"),
                columns: new[] { "LowStockThreshold", "ProductId", "ProductName", "QuantityAvailable", "SKU" },
                values: new object[] { 100, new Guid("11111111-0000-0000-0000-000000000003"), "B2B House Espresso Blend", 800, "COFFEE-ESP-001" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000004"),
                columns: new[] { "LowStockThreshold", "ProductId", "ProductName", "QuantityAvailable", "SKU" },
                values: new object[] { 30, new Guid("11111111-0000-0000-0000-000000000004"), "Swiss Water Decaf", 200, "COFFEE-DEC-001" });
        }
    }
}
