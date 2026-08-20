using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tagr.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSellerRequestedToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSellerRequested",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSellerRequested",
                table: "AspNetUsers");
        }
    }
}
