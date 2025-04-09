﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookHotel.Migrations
{
    /// <inheritdoc />
    public partial class AddAnonymousReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Anonymous",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Anonymous",
                table: "Reviews");
        }
    }
}
