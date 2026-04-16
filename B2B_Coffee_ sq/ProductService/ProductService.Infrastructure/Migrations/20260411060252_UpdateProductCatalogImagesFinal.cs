using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductCatalogImagesFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000004"));

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "DiscountedPrice", "ImageUrl", "IsActive", "IsFeatured", "MinimumOrderQuantity", "Name", "Origin", "Price", "RoastLevel", "SKU", "UpdatedAt", "WeightInGrams" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-1111-4aaa-bbbb-000000000001"), 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000001"), "Bright, floral, and fruity with a light body. Notes of jasmine, lemon, and blueberry.", null, "/assets/images/products/ethiopian-yirgacheffe.png", true, true, 5, "Ethiopian Yirgacheffe", "Ethiopia", 1850.00m, 1, "ETH-YIR-01", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000.0 },
                    { new Guid("a1b2c3d4-2222-4aaa-bbbb-000000000002"), 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000001"), "Well-balanced with mild acidity. Notes of caramel, chocolate, and walnut.", null, "/assets/images/products/colombian-supremo.png", true, true, 5, "Colombian Supremo", "Colombia", 1650.00m, 2, "COL-SUP-01", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000.0 },
                    { new Guid("a1b2c3d4-3333-4aaa-bbbb-000000000003"), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000001"), "Authentic dark roast espresso blend with a thick crema and hints of spice and cocoa.", null, "/assets/images/products/italian-espresso.png", true, true, 10, "Italian Espresso Classico", "Italy/Brazil", 1550.00m, 4, "ESP-ITA-01", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000.0 },
                    { new Guid("a1b2c3d4-5555-4aaa-bbbb-000000000005"), 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000001"), "Famous for its complex flavor profile. Spicy, smokey, and chocolaty with floral hints.", null, "/assets/images/products/guatemala-antigua.png", true, false, 5, "Guatemala Antigua", "Guatemala", 1950.00m, 2, "GUA-ANT-01", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000.0 },
                    { new Guid("a1b2c3d4-7777-4aaa-bbbb-000000000007"), 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000001"), "Bright acidity and intense fruitiness. Notes of blackcurrant, wine, and citrus.", null, "/assets/images/products/kenya-aa.png", true, true, 5, "Kenya AA Peaberry", "Kenya", 2250.00m, 1, "KEN-PEA-01", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-1111-4aaa-bbbb-000000000001"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-2222-4aaa-bbbb-000000000002"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-3333-4aaa-bbbb-000000000003"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-5555-4aaa-bbbb-000000000005"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-7777-4aaa-bbbb-000000000007"));

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "DiscountedPrice", "ImageUrl", "IsActive", "IsFeatured", "MinimumOrderQuantity", "Name", "Origin", "Price", "RoastLevel", "SKU", "UpdatedAt", "WeightInGrams" },
                values: new object[,]
                {
                    { new Guid("11111111-0000-0000-0000-000000000001"), 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000001"), "Bright, floral, and fruity with a light body. Notes of jasmine, lemon, and blueberry.", null, "https://placehold.co/400x300?text=Ethiopian+Yirgacheffe", true, true, 5, "Ethiopian Yirgacheffe", "Ethiopia", 1850.00m, 1, "COFFEE-ETH-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000.0 },
                    { new Guid("11111111-0000-0000-0000-000000000002"), 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000001"), "Well-balanced with mild acidity. Notes of caramel, chocolate, and walnut.", null, "https://placehold.co/400x300?text=Colombian+Supremo", true, true, 5, "Colombian Supremo", "Colombia", 1650.00m, 2, "COFFEE-COL-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000.0 },
                    { new Guid("11111111-0000-0000-0000-000000000003"), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000001"), "Bold, rich espresso blend with low acidity. Perfect for cafes and restaurants.", null, "https://placehold.co/400x300?text=Espresso+Blend", true, false, 10, "B2B House Espresso Blend", "Multi-Origin", 1450.00m, 4, "COFFEE-ESP-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000.0 },
                    { new Guid("11111111-0000-0000-0000-000000000004"), 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000001"), "99.9% caffeine-free using Swiss Water Process. Full flavor without the buzz.", null, "https://placehold.co/400x300?text=Swiss+Decaf", true, false, 5, "Swiss Water Decaf", "Brazil", 2100.00m, 3, "COFFEE-DEC-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000.0 }
                });
        }
    }
}
