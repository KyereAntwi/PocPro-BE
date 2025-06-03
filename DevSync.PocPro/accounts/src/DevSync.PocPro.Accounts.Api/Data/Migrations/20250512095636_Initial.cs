using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DevSync.PocPro.Accounts.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    OtherNames = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    PhotoUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueIdentifier = table.Column<string>(type: "text", nullable: false),
                    ConnectionString = table.Column<string>(type: "text", nullable: false),
                    SubscriptionType = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserPermission",
                columns: table => new
                {
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserPermission", x => new { x.ApplicationUserId, x.PermissionsId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserPermission_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserPermission_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "PermissionType", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("0196c3ec-c2ed-7290-bf40-e85e289efc40"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 693, DateTimeKind.Unspecified).AddTicks(4470), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_PERMISSIONS", null, null },
                    { new Guid("0196c3ec-c2f7-73ea-ba9e-ad09416ca88a"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9320), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "GET_POS", null, null },
                    { new Guid("0196c3ec-c2f7-7609-b2a5-9f1cfa593fa5"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9360), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_CUSTOMERS", null, null },
                    { new Guid("0196c3ec-c2f7-7795-910c-9fa91a652152"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9290), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_CATEGORIES", null, null },
                    { new Guid("0196c3ec-c2f7-785d-bba4-7c9d9a745246"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9350), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_PURCHASES", null, null },
                    { new Guid("0196c3ec-c2f7-78d0-9c85-022ca9bf86da"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9340), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_PURCHASES", null, null },
                    { new Guid("0196c3ec-c2f7-794d-90c5-9e446b97091c"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9300), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_SUPPLIERS", null, null },
                    { new Guid("0196c3ec-c2f7-7956-b31f-c5dce0a4304f"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9310), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_STOCKS", null, null },
                    { new Guid("0196c3ec-c2f7-796e-8865-46b3069c79b6"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9260), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_TENANTS", null, null },
                    { new Guid("0196c3ec-c2f7-7a5e-9ea9-bdaca2f87d9b"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9290), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_SUPPLIERS", null, null },
                    { new Guid("0196c3ec-c2f7-7b09-9167-ecca27d95f33"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9280), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_PRODUCTS", null, null },
                    { new Guid("0196c3ec-c2f7-7cc9-98f8-64bd8235d996"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9270), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_PRODUCTS", null, null },
                    { new Guid("0196c3ec-c2f7-7cf8-92f0-47af22e33a75"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9370), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_ORDERS", null, null },
                    { new Guid("0196c3ec-c2f7-7dd8-b54f-3448ea9b5775"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9270), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_TENANTS", null, null },
                    { new Guid("0196c3ec-c2f7-7df7-ab6c-656c1d9443d2"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9330), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_SALES", null, null },
                    { new Guid("0196c3ec-c2f7-7dfc-ae00-dbae0a1bd953"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9240), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_USERS", null, null },
                    { new Guid("0196c3ec-c2f7-7e08-892d-4136bbc77b9a"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9250), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_USERS", null, null },
                    { new Guid("0196c3ec-c2f7-7e3d-9b78-7c35d855c765"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9330), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_SALES", null, null },
                    { new Guid("0196c3ec-c2f7-7e8c-8223-c4f06fe4826f"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9310), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_POS", null, null },
                    { new Guid("0196c3ec-c2f7-7f9b-a22c-0e15ab14b3cc"), new DateTimeOffset(new DateTime(2025, 5, 12, 9, 56, 35, 703, DateTimeKind.Unspecified).AddTicks(9360), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_CUSTOMERS", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserPermission_PermissionsId",
                table: "ApplicationUserPermission",
                column: "PermissionsId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_TenantId",
                table: "ApplicationUsers",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserPermission");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "Permissions");
        }
    }
}
