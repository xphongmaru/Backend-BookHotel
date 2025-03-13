using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookHotel.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    Amenities_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.Amenities_id);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Discount_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discount_percentage = table.Column<float>(type: "real", nullable: false),
                    Price_applies = table.Column<float>(type: "real", nullable: false),
                    Max_discount = table.Column<float>(type: "real", nullable: false),
                    Start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Discount_id);
                });

            migrationBuilder.CreateTable(
                name: "Guess",
                columns: table => new
                {
                    Guess_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bod = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<bool>(type: "bit", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guess", x => x.Guess_id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Permission_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Permission_id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Role_id);
                });

            migrationBuilder.CreateTable(
                name: "TypeRooms",
                columns: table => new
                {
                    TypeRoom_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeRooms", x => x.TypeRoom_id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Booking_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Check_in = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Check_out = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Request = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Guess_id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Booking_id);
                    table.ForeignKey(
                        name: "FK_Bookings_Guess_Guess_id",
                        column: x => x.Guess_id,
                        principalTable: "Guess",
                        principalColumn: "Guess_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Role_Permissions",
                columns: table => new
                {
                    Role_id = table.Column<int>(type: "int", nullable: false),
                    Permission_id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role_Permissions", x => new { x.Role_id, x.Permission_id });
                    table.ForeignKey(
                        name: "FK_Role_Permissions_Permissions_Permission_id",
                        column: x => x.Permission_id,
                        principalTable: "Permissions",
                        principalColumn: "Permission_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Role_Permissions_Roles_Role_id",
                        column: x => x.Role_id,
                        principalTable: "Roles",
                        principalColumn: "Role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role_id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_Role_id",
                        column: x => x.Role_id,
                        principalTable: "Roles",
                        principalColumn: "Role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Room_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Max_occupancy = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeRoom_id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Room_id);
                    table.ForeignKey(
                        name: "FK_Rooms_TypeRooms_TypeRoom_id",
                        column: x => x.TypeRoom_id,
                        principalTable: "TypeRooms",
                        principalColumn: "TypeRoom_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Booking_Discounts",
                columns: table => new
                {
                    Booking_id = table.Column<int>(type: "int", nullable: false),
                    Discount_id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking_Discounts", x => new { x.Booking_id, x.Discount_id });
                    table.ForeignKey(
                        name: "FK_Booking_Discounts_Bookings_Booking_id",
                        column: x => x.Booking_id,
                        principalTable: "Bookings",
                        principalColumn: "Booking_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Booking_Discounts_Discounts_Discount_id",
                        column: x => x.Discount_id,
                        principalTable: "Discounts",
                        principalColumn: "Discount_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Booking_Rooms",
                columns: table => new
                {
                    Booking_id = table.Column<int>(type: "int", nullable: false),
                    Room_id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Name_Guess = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone_Guess = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking_Rooms", x => new { x.Booking_id, x.Room_id });
                    table.ForeignKey(
                        name: "FK_Booking_Rooms_Bookings_Booking_id",
                        column: x => x.Booking_id,
                        principalTable: "Bookings",
                        principalColumn: "Booking_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Booking_Rooms_Rooms_Room_id",
                        column: x => x.Room_id,
                        principalTable: "Rooms",
                        principalColumn: "Room_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Room_id = table.Column<int>(type: "int", nullable: false),
                    Guess_id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Review_id);
                    table.ForeignKey(
                        name: "FK_Reviews_Guess_Guess_id",
                        column: x => x.Guess_id,
                        principalTable: "Guess",
                        principalColumn: "Guess_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Rooms_Room_id",
                        column: x => x.Room_id,
                        principalTable: "Rooms",
                        principalColumn: "Room_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Room_Amenities",
                columns: table => new
                {
                    Room_id = table.Column<int>(type: "int", nullable: false),
                    Amenities_id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room_Amenities", x => new { x.Room_id, x.Amenities_id });
                    table.ForeignKey(
                        name: "FK_Room_Amenities_Amenities_Amenities_id",
                        column: x => x.Amenities_id,
                        principalTable: "Amenities",
                        principalColumn: "Amenities_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Room_Amenities_Rooms_Room_id",
                        column: x => x.Room_id,
                        principalTable: "Rooms",
                        principalColumn: "Room_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomPhotos",
                columns: table => new
                {
                    RoomPhoto_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Room_id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomPhotos", x => x.RoomPhoto_id);
                    table.ForeignKey(
                        name: "FK_RoomPhotos_Rooms_Room_id",
                        column: x => x.Room_id,
                        principalTable: "Rooms",
                        principalColumn: "Room_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Discounts_Discount_id",
                table: "Booking_Discounts",
                column: "Discount_id");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Rooms_Room_id",
                table: "Booking_Rooms",
                column: "Room_id");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Guess_id",
                table: "Bookings",
                column: "Guess_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Guess_id",
                table: "Reviews",
                column: "Guess_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Room_id",
                table: "Reviews",
                column: "Room_id");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Permissions_Permission_id",
                table: "Role_Permissions",
                column: "Permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_Room_Amenities_Amenities_id",
                table: "Room_Amenities",
                column: "Amenities_id");

            migrationBuilder.CreateIndex(
                name: "IX_RoomPhotos_Room_id",
                table: "RoomPhotos",
                column: "Room_id");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_TypeRoom_id",
                table: "Rooms",
                column: "TypeRoom_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role_id",
                table: "Users",
                column: "Role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Booking_Discounts");

            migrationBuilder.DropTable(
                name: "Booking_Rooms");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Role_Permissions");

            migrationBuilder.DropTable(
                name: "Room_Amenities");

            migrationBuilder.DropTable(
                name: "RoomPhotos");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Guess");

            migrationBuilder.DropTable(
                name: "TypeRooms");
        }
    }
}
