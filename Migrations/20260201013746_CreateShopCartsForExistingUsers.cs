using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bike_club_api.Migrations
{
    /// <inheritdoc />
    public partial class CreateShopCartsForExistingUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert a new ShopCart for every existing user
            migrationBuilder.Sql(@"
                INSERT INTO ShopCarts (UserId, PurchaseDate, TotalAmount)
                SELECT Id, GETUTCDATE(), 0
                FROM Users
                WHERE Id NOT IN (SELECT UserId FROM ShopCarts)
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove ShopCarts that were created by this migration
            migrationBuilder.Sql(@"
                DELETE FROM ShopCarts
                WHERE UserId IN (SELECT Id FROM Users)
                AND PurchaseDate = (SELECT MIN(PurchaseDate) FROM ShopCarts)
            ");
        }
    }
}
