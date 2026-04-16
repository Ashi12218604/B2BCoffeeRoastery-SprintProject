using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSmtpCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SmtpConfigs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "SenderEmail", "SenderName" },
                values: new object[] { "labu hrjv njfs zsxs", "ashigupta2809@gmail.com", "Ember&Bean" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 4, 9, 39, 12, 215, DateTimeKind.Utc).AddTicks(3497));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SmtpConfigs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "SenderEmail", "SenderName" },
                values: new object[] { "your-app-password", "your-gmail@gmail.com", "B2B Coffee Roastery" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 4, 9, 32, 36, 516, DateTimeKind.Utc).AddTicks(5345));
        }
    }
}
