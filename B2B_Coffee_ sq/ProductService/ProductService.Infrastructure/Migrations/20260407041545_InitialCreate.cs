using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    RoastLevel = table.Column<int>(type: "int", nullable: false),
                    WeightInGrams = table.Column<double>(type: "float", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    MinimumOrderQuantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductReviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductId",
                table: "ProductReviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductReviews");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
