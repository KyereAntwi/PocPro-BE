using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DevSync.PocPro.Accounts.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Account_Migrations_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "PermissionType", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("01967cdf-76ba-73bd-a768-547dd237f588"), new DateTimeOffset(new DateTime(2025, 4, 28, 14, 49, 1, 882, DateTimeKind.Unspecified).AddTicks(3537), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_PERMISSIONS", null, null },
                    { new Guid("01967cdf-76bc-7045-a591-559006d37fc8"), new DateTimeOffset(new DateTime(2025, 4, 28, 14, 49, 1, 884, DateTimeKind.Unspecified).AddTicks(7030), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_USERS", null, null },
                    { new Guid("01967cdf-76bc-7144-a6bf-3a40f93ae81b"), new DateTimeOffset(new DateTime(2025, 4, 28, 14, 49, 1, 884, DateTimeKind.Unspecified).AddTicks(7052), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_PRODUCTS", null, null },
                    { new Guid("01967cdf-76bc-73f8-a949-88397f726e20"), new DateTimeOffset(new DateTime(2025, 4, 28, 14, 49, 1, 884, DateTimeKind.Unspecified).AddTicks(7016), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_USERS", null, null },
                    { new Guid("01967cdf-76bc-75d2-b0fe-c7fb302a10c5"), new DateTimeOffset(new DateTime(2025, 4, 28, 14, 49, 1, 884, DateTimeKind.Unspecified).AddTicks(7049), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_TENANTS", null, null },
                    { new Guid("01967cdf-76bc-75e5-8fb6-767a45feb346"), new DateTimeOffset(new DateTime(2025, 4, 28, 14, 49, 1, 884, DateTimeKind.Unspecified).AddTicks(7034), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_TENANTS", null, null },
                    { new Guid("01967cdf-76bc-7f40-a739-a3fff151e426"), new DateTimeOffset(new DateTime(2025, 4, 28, 14, 49, 1, 884, DateTimeKind.Unspecified).AddTicks(7054), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_PRODUCTS", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_TenantId",
                table: "ApplicationUsers",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_TenantId",
                table: "ApplicationUsers");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01967cdf-76ba-73bd-a768-547dd237f588"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01967cdf-76bc-7045-a591-559006d37fc8"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01967cdf-76bc-7144-a6bf-3a40f93ae81b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01967cdf-76bc-73f8-a949-88397f726e20"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01967cdf-76bc-75d2-b0fe-c7fb302a10c5"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01967cdf-76bc-75e5-8fb6-767a45feb346"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01967cdf-76bc-7f40-a739-a3fff151e426"));
        }
    }
}
