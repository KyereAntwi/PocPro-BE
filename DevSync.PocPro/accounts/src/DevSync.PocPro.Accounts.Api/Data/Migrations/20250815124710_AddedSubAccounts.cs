using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DevSync.PocPro.Accounts.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "SubAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessName = table.Column<string>(type: "text", nullable: false),
                    SettlementBank = table.Column<string>(type: "text", nullable: false),
                    AccountNumber = table.Column<string>(type: "text", nullable: false),
                    PercentageCharge = table.Column<float>(type: "real", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubAccounts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "PermissionType", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("0198adc5-0ec0-7697-9123-ec6f41fa3993"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 504, DateTimeKind.Unspecified).AddTicks(6530), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_PERMISSIONS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-709f-bc16-6ea9663cf024"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(1970), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_PRODUCTS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-71fd-b39c-326e15bdcb72"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2090), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_SETTINGS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-723c-82db-518df87a760d"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2100), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_SUBACCOUNTS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7293-94c0-d20e458846f1"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2050), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_CUSTOMERS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-72ac-9d2b-feeecce53a1e"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(1960), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_TENANTS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7436-b0e6-a0a29801d7e7"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2010), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_POS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-745c-aaa3-a18cc0b660c1"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(1980), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_CATEGORIES", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7507-9f4c-f49535d5eded"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(1990), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "VIEW_SUPPLIERS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-750c-b77c-91cf0ef21bcd"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2080), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "VIEW_REPORTS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-755e-8cad-34792b3b875b"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2000), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "VIEW_STOCKS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7633-bd25-8dfc45aa5ce1"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2080), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_BRANDS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7688-a9d2-822edbb6c1b2"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2040), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "VIEW_PURCHASES", "Active", null, null },
                    { new Guid("0198adc5-0ec6-769c-a562-aed0ab245436"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2070), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "VIEW_SESSIONS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-77f3-8eda-099831000ff4"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2060), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "VIEW_CUSTOMERS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7835-805e-3f85acf10011"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2030), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "VIEW_SALES", "Active", null, null },
                    { new Guid("0198adc5-0ec6-78ae-b1ce-feb136996f1a"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(1970), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "VIEW_PRODUCTS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-797f-bb30-b007dd24c15e"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2010), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "GET_POS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-79c7-b170-3f8d112b8059"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2040), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_PURCHASES", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7aea-84a4-06c7c582ec20"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(1930), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "VIEW_USERS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7b60-98da-de3d761552c1"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2060), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "VIEW_ORDERS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7dec-b627-66d19a3d096d"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(1950), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "VIEW_TENANTS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7e3d-ad95-cbb06d3363c9"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(1990), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_SUPPLIERS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7e8f-9f01-95760b3629e4"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(1940), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_USERS", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7ed9-8951-ceb35f322d5d"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2020), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_SALES", "Active", null, null },
                    { new Guid("0198adc5-0ec6-7fb0-be60-bd58ef9ed984"), new DateTimeOffset(new DateTime(2025, 8, 15, 15, 47, 9, 510, DateTimeKind.Unspecified).AddTicks(2100), new TimeSpan(0, 3, 0, 0, 0)), "System", null, null, "MANAGE_PROMO_CODES", "Active", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubAccounts_TenantId",
                table: "SubAccounts",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubAccounts");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec0-7697-9123-ec6f41fa3993"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-709f-bc16-6ea9663cf024"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-71fd-b39c-326e15bdcb72"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-723c-82db-518df87a760d"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7293-94c0-d20e458846f1"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-72ac-9d2b-feeecce53a1e"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7436-b0e6-a0a29801d7e7"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-745c-aaa3-a18cc0b660c1"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7507-9f4c-f49535d5eded"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-750c-b77c-91cf0ef21bcd"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-755e-8cad-34792b3b875b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7633-bd25-8dfc45aa5ce1"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7688-a9d2-822edbb6c1b2"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-769c-a562-aed0ab245436"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-77f3-8eda-099831000ff4"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7835-805e-3f85acf10011"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-78ae-b1ce-feb136996f1a"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-797f-bb30-b007dd24c15e"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-79c7-b170-3f8d112b8059"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7aea-84a4-06c7c582ec20"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7b60-98da-de3d761552c1"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7dec-b627-66d19a3d096d"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7e3d-ad95-cbb06d3363c9"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7e8f-9f01-95760b3629e4"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7ed9-8951-ceb35f322d5d"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0198adc5-0ec6-7fb0-be60-bd58ef9ed984"));

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
    }
}
