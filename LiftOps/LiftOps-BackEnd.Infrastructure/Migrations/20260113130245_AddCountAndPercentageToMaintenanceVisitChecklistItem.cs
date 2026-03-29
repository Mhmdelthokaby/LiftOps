using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCountAndPercentageToMaintenanceVisitChecklistItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "MaintenanceVisitChecklistItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Percentage",
                table: "MaintenanceVisitChecklistItems",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "MaintenanceVisitChecklistItems");

            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "MaintenanceVisitChecklistItems");
        }
    }
}
