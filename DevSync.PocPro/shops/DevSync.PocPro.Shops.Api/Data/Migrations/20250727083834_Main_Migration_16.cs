using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevSync.PocPro.Shops.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Main_Migration_16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WishListItems");

            migrationBuilder.AddColumn<Guid>(
                name: "PointOfSaleId",
                table: "WishListItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointOfSaleId",
                table: "WishListItems");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "WishListItems",
                type: "character varying(225)",
                maxLength: 225,
                nullable: false,
                defaultValue: "");
        }
    }
}
