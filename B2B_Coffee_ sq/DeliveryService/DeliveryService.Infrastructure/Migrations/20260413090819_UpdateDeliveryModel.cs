using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeliveryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedByAdminName",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductNames",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedByAdminName",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "ProductNames",
                table: "Deliveries");
        }
    }
}
