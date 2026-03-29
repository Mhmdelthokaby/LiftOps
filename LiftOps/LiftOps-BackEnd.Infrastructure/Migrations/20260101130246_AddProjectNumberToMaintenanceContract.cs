using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectNumberToMaintenanceContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFromInstallation",
                table: "MaintenanceContracts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProjectNumber",
                table: "MaintenanceContracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFromInstallation",
                table: "MaintenanceContracts");

            migrationBuilder.DropColumn(
                name: "ProjectNumber",
                table: "MaintenanceContracts");
        }
    }
}
