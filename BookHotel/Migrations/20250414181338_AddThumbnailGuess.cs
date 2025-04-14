using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookHotel.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailGuess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "thumbnail",
                table: "Guess",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thumbnail",
                table: "Guess");
        }
    }
}
