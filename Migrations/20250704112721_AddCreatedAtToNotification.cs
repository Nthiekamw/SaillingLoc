using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaillingLoc.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Notifications",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Notifications",
                newName: "DateCreated");
        }
    }
}
