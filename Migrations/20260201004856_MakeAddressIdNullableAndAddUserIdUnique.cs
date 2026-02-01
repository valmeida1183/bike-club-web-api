using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bike_club_api.Migrations
{
    /// <inheritdoc />
    public partial class MakeAddressIdNullableAndAddUserIdUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopCarts_Addresses_AddressId",
                table: "ShopCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShopCarts_UserId",
                table: "ShopCarts");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "ShopCarts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Electric");

            migrationBuilder.CreateIndex(
                name: "IX_ShopCarts_UserId",
                table: "ShopCarts",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopCarts_Addresses_AddressId",
                table: "ShopCarts",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopCarts_Addresses_AddressId",
                table: "ShopCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShopCarts_UserId",
                table: "ShopCarts");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "ShopCarts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Eletric");

            migrationBuilder.CreateIndex(
                name: "IX_ShopCarts_UserId",
                table: "ShopCarts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopCarts_Addresses_AddressId",
                table: "ShopCarts",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
