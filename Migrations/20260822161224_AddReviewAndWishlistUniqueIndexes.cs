using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tagr.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewAndWishlistUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wishlists_CustomerId",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_CustomerId_ProductId",
                table: "Wishlists",
                columns: new[] { "CustomerId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId_ProductId",
                table: "Reviews",
                columns: new[] { "CustomerId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wishlists_CustomerId_ProductId",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_CustomerId_ProductId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_CustomerId",
                table: "Wishlists",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                column: "CustomerId");
        }
    }
}
