using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaillingLoc.Migrations
{
    /// <inheritdoc />
    public partial class AddLatitudeLongitudeToBoats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_BoatOwnerId",
                schema: "dbo",
                table: "Reservations");

            migrationBuilder.AlterColumn<string>(
                name: "BoatOwnerId",
                schema: "dbo",
                table: "Reservations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Boats",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Boats",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_BoatOwnerId",
                schema: "dbo",
                table: "Reservations",
                column: "BoatOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_BoatOwnerId",
                schema: "dbo",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Boats");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Boats");

            migrationBuilder.AlterColumn<string>(
                name: "BoatOwnerId",
                schema: "dbo",
                table: "Reservations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_BoatOwnerId",
                schema: "dbo",
                table: "Reservations",
                column: "BoatOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
