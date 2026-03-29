using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Collins_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleMapsLinkToEmergencyTickt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleMapsLink",
                table: "EmergencyTickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleMapsLink",
                table: "EmergencyTickets");
        }
    }
}
