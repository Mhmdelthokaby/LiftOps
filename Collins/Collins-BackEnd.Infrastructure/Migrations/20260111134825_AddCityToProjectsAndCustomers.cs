using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Collins_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCityToProjectsAndCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "MaintenanceContracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "القاهرة الجديدة");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "InstallationProjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "القاهرة الجديدة");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "القاهرة الجديدة");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "MaintenanceContracts");

            migrationBuilder.DropColumn(
                name: "City",
                table: "InstallationProjects");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Customers");
        }
    }
}
