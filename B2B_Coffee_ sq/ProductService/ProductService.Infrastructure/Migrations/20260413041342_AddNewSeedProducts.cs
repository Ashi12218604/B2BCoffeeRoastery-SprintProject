using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewSeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use conditional inserts to avoid PK violations when products already exist in DB
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Products WHERE Id = 'a1b2c3d4-4444-4aaa-bbbb-000000000004')
                INSERT INTO Products (Id, Category, CreatedAt, CreatedBy, [Description], DiscountedPrice, ImageUrl, IsActive, IsFeatured, MinimumOrderQuantity, Name, Origin, Price, RoastLevel, SKU, UpdatedAt, WeightInGrams)
                VALUES ('a1b2c3d4-4444-4aaa-bbbb-000000000004', 2, '2024-01-01', '00000000-0000-0000-0000-000000000001', 'Full-bodied Indonesian coffee with earthy, herbal complexity. Low acidity with a lingering dark chocolate and cedar finish.', 1880.00, '/assets/images/products/sumatra-mandheling.png', 1, 0, 5, 'Sumatra Mandheling', 'Indonesia', 1920.00, 4, 'SUM-MAN-01', '2024-01-01', 1000.0);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE Id = 'a1b2c3d4-6666-4aaa-bbbb-000000000006')
                INSERT INTO Products (Id, Category, CreatedAt, CreatedBy, [Description], DiscountedPrice, ImageUrl, IsActive, IsFeatured, MinimumOrderQuantity, Name, Origin, Price, RoastLevel, SKU, UpdatedAt, WeightInGrams)
                VALUES ('a1b2c3d4-6666-4aaa-bbbb-000000000006', 3, '2024-01-01', '00000000-0000-0000-0000-000000000001', 'Chemically-free Swiss Water Process decaf with all the flavor, none of the caffeine. Smooth, mellow, and perfect for evening cups.', NULL, '/assets/images/products/swiss-decaf.png', 1, 0, 10, 'Swiss Water Decaf Blend', 'Brazil/Peru', 1650.00, 2, 'DEC-SWS-01', '2024-01-01', 1000.0);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE Id = 'a1b2c3d4-8888-4aaa-bbbb-000000000008')
                INSERT INTO Products (Id, Category, CreatedAt, CreatedBy, [Description], DiscountedPrice, ImageUrl, IsActive, IsFeatured, MinimumOrderQuantity, Name, Origin, Price, RoastLevel, SKU, UpdatedAt, WeightInGrams)
                VALUES ('a1b2c3d4-8888-4aaa-bbbb-000000000008', 4, '2024-01-01', '00000000-0000-0000-0000-000000000001', 'Pre-ground coarse blend optimized for 12-24hr cold extraction. Yields a smooth, low-acid concentrate with natural sweetness and chocolate notes.', 1499.00, '/assets/images/products/cold-brew.png', 1, 0, 10, 'Cold Brew Concentrate', 'Brazil', 1550.00, 3, 'CLD-BRW-01', '2024-01-01', 1000.0);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE Id = 'a1b2c3d4-9999-4aaa-bbbb-000000000009')
                INSERT INTO Products (Id, Category, CreatedAt, CreatedBy, [Description], DiscountedPrice, ImageUrl, IsActive, IsFeatured, MinimumOrderQuantity, Name, Origin, Price, RoastLevel, SKU, UpdatedAt, WeightInGrams)
                VALUES ('a1b2c3d4-9999-4aaa-bbbb-000000000009', 2, '2024-01-01', '00000000-0000-0000-0000-000000000001', 'Bright and clean Costa Rican beans from the famed Tarrazu highlands. Honey-processed with notes of apricot, brown sugar, and a silky mouthfeel.', NULL, '/assets/images/products/costa-rica-tarrazu.png', 1, 0, 5, 'Costa Rica Tarrazu', 'Costa Rica', 1890.00, 2, 'CRC-TAR-01', '2024-01-01', 1000.0);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE Id = 'a1b2c3d4-aaaa-4aaa-bbbb-000000000010')
                INSERT INTO Products (Id, Category, CreatedAt, CreatedBy, [Description], DiscountedPrice, ImageUrl, IsActive, IsFeatured, MinimumOrderQuantity, Name, Origin, Price, RoastLevel, SKU, UpdatedAt, WeightInGrams)
                VALUES ('a1b2c3d4-aaaa-4aaa-bbbb-000000000010', 1, '2024-01-01', '00000000-0000-0000-0000-000000000001', 'A luxurious house blend of Ethiopian and Brazilian beans with rich cocoa, toasted almond, and a whisper of vanilla. Perfect for lattes.', 1650.00, '/assets/images/products/midnight-mocha.png', 1, 1, 5, 'Midnight Mocha Blend', 'Ethiopia/Brazil', 1690.00, 3, 'BLD-MOC-01', '2024-01-01', 1000.0);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-4444-4aaa-bbbb-000000000004"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-6666-4aaa-bbbb-000000000006"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-8888-4aaa-bbbb-000000000008"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-9999-4aaa-bbbb-000000000009"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-aaaa-4aaa-bbbb-000000000010"));
        }
    }
}
