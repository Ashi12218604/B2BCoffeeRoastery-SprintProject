using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSuperAdminHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 10, 5, 11, 35, 548, DateTimeKind.Utc).AddTicks(7857), "$2a$11$Nqh71nFqvsjg31JIY.TmxO0AIxHNBjrTlBIBfpF.kZqO.pfqxjxIC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 4, 9, 54, 24, 631, DateTimeKind.Utc).AddTicks(7213), "$2a$11$mC.7jYm2ghz9.6A/mD8Pue9VvH7J3y3f9.z6O4.L7f9.X7f9.X7f9" });
        }
    }
}
