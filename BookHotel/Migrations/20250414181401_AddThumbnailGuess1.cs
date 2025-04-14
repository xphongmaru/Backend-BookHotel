using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookHotel.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailGuess1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "thumbnail",
                table: "Guess",
                newName: "Thumbnail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Thumbnail",
                table: "Guess",
                newName: "thumbnail");
        }
    }
}
