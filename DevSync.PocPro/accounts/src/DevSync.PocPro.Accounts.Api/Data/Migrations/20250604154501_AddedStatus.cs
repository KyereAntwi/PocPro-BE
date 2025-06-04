using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DevSync.PocPro.Accounts.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2ed-7290-bf40-e85e289efc40"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-73ea-ba9e-ad09416ca88a"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7609-b2a5-9f1cfa593fa5"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7795-910c-9fa91a652152"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-785d-bba4-7c9d9a745246"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-78d0-9c85-022ca9bf86da"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-794d-90c5-9e446b97091c"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7956-b31f-c5dce0a4304f"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-796e-8865-46b3069c79b6"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7a5e-9ea9-bdaca2f87d9b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7b09-9167-ecca27d95f33"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7cc9-98f8-64bd8235d996"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7cf8-92f0-47af22e33a75"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7dd8-b54f-3448ea9b5775"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7df7-ab6c-656c1d9443d2"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7dfc-ae00-dbae0a1bd953"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7e08-892d-4136bbc77b9a"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7e3d-9b78-7c35d855c765"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7e8c-8223-c4f06fe4826f"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0196c3ec-c2f7-7f9b-a22c-0e15ab14b3cc"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Permissions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ApplicationUsers",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "PermissionType", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("01973b9e-06cd-73f8-92f7-c988e26ce9a9"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 645, DateTimeKind.Unspecified).AddTicks(2690), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_PERMISSIONS", "Active", null, null },
                    { new Guid("01973b9e-06d7-7050-9f91-fb19b3cd3d01"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6760), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_PRODUCTS", "Active", null, null },
                    { new Guid("01973b9e-06d7-7115-92e2-4c1d4fbf944a"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6760), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_CATEGORIES", "Active", null, null },
                    { new Guid("01973b9e-06d7-719e-894e-f9854a36bbbb"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6730), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_USERS", "Active", null, null },
                    { new Guid("01973b9e-06d7-7225-b878-6bfce2302b17"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6730), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_TENANTS", "Active", null, null },
                    { new Guid("01973b9e-06d7-72cd-a3eb-e3deccc0e139"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6820), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_PURCHASES", "Active", null, null },
                    { new Guid("01973b9e-06d7-7324-a0e7-a537ddfddc66"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6810), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_SALES", "Active", null, null },
                    { new Guid("01973b9e-06d7-7345-86fa-9236c56aa1f2"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6740), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_TENANTS", "Active", null, null },
                    { new Guid("01973b9e-06d7-739c-ab58-cb413a91189a"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6750), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_PRODUCTS", "Active", null, null },
                    { new Guid("01973b9e-06d7-73f1-ae83-82196ee711bf"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6790), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_POS", "Active", null, null },
                    { new Guid("01973b9e-06d7-74f0-83fa-6b533ee20314"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6840), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_CUSTOMERS", "Active", null, null },
                    { new Guid("01973b9e-06d7-761d-b2fb-e1279cbf2fc0"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6830), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_CUSTOMERS", "Active", null, null },
                    { new Guid("01973b9e-06d7-765b-8982-85a3235a3258"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6810), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_SALES", "Active", null, null },
                    { new Guid("01973b9e-06d7-7691-ba07-b54fbc0797a4"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6710), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_USERS", "Active", null, null },
                    { new Guid("01973b9e-06d7-7907-80de-1cdc2d9a2ec6"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6850), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_ORDERS", "Active", null, null },
                    { new Guid("01973b9e-06d7-7bbb-b63f-868550e714e2"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6770), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "MANAGE_SUPPLIERS", "Active", null, null },
                    { new Guid("01973b9e-06d7-7cd6-b48c-3babb81c170d"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6780), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_SUPPLIERS", "Active", null, null },
                    { new Guid("01973b9e-06d7-7dbe-b49d-ec880a0d71f4"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6830), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_PURCHASES", "Active", null, null },
                    { new Guid("01973b9e-06d7-7eb1-92f4-989c082289a2"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6800), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "GET_POS", "Active", null, null },
                    { new Guid("01973b9e-06d7-7f3f-8043-a14aa13481bb"), new DateTimeOffset(new DateTime(2025, 6, 4, 15, 45, 1, 655, DateTimeKind.Unspecified).AddTicks(6780), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "VIEW_STOCKS", "Active", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06cd-73f8-92f7-c988e26ce9a9"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-7050-9f91-fb19b3cd3d01"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-7115-92e2-4c1d4fbf944a"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-719e-894e-f9854a36bbbb"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-7225-b878-6bfce2302b17"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-72cd-a3eb-e3deccc0e139"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-7324-a0e7-a537ddfddc66"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-7345-86fa-9236c56aa1f2"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-739c-ab58-cb413a91189a"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-73f1-ae83-82196ee711bf"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-74f0-83fa-6b533ee20314"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-761d-b2fb-e1279cbf2fc0"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-765b-8982-85a3235a3258"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-7691-ba07-b54fbc0797a4"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-7907-80de-1cdc2d9a2ec6"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-7bbb-b63f-868550e714e2"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-7cd6-b48c-3babb81c170d"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-7dbe-b49d-ec880a0d71f4"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-7eb1-92f4-989c082289a2"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01973b9e-06d7-7f3f-8043-a14aa13481bb"));

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ApplicationUsers");

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
        }
    }
}
