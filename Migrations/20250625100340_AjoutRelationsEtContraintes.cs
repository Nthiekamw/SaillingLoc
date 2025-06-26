using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaillingLoc.Migrations
{
    /// <inheritdoc />
    public partial class AjoutRelationsEtContraintes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boats_BoatTypes_BoatTypeId",
                table: "Boats");

            migrationBuilder.DropForeignKey(
                name: "FK_Boats_Ports_PortId",
                table: "Boats");

            migrationBuilder.DropForeignKey(
                name: "FK_Boats_Users_UserId",
                table: "Boats");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Boats_BoatId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations");

            migrationBuilder.AddForeignKey(
                name: "FK_Boats_BoatTypes_BoatTypeId",
                table: "Boats",
                column: "BoatTypeId",
                principalTable: "BoatTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Boats_Ports_PortId",
                table: "Boats",
                column: "PortId",
                principalTable: "Ports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Boats_Users_UserId",
                table: "Boats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Boats_BoatId",
                table: "Reservations",
                column: "BoatId",
                principalTable: "Boats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boats_BoatTypes_BoatTypeId",
                table: "Boats");

            migrationBuilder.DropForeignKey(
                name: "FK_Boats_Ports_PortId",
                table: "Boats");

            migrationBuilder.DropForeignKey(
                name: "FK_Boats_Users_UserId",
                table: "Boats");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Boats_BoatId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations");

            migrationBuilder.AddForeignKey(
                name: "FK_Boats_BoatTypes_BoatTypeId",
                table: "Boats",
                column: "BoatTypeId",
                principalTable: "BoatTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Boats_Ports_PortId",
                table: "Boats",
                column: "PortId",
                principalTable: "Ports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Boats_Users_UserId",
                table: "Boats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Boats_BoatId",
                table: "Reservations",
                column: "BoatId",
                principalTable: "Boats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
