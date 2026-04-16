using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSuperAdminSeedHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 11, 4, 46, 39, 321, DateTimeKind.Utc).AddTicks(524), "$2a$11$hhLH.mbgOD75YxVkUbIVwumhfMXxLKzB5PPragDAaekWNolt.Ufx6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 10, 5, 11, 35, 548, DateTimeKind.Utc).AddTicks(7857), "$2a$11$RVp67XyVvH7J3y3f9.z6O4.L7f9.X7f9.X7f9.RVp67XyVvH7J3y3f" });
        }
    }
}
