using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DB001_TenantScopedUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InstallationProjects_ProjectNumber",
                table: "InstallationProjects");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectNumber",
                table: "MaintenanceContracts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ItemNumber",
                table: "InventoryItems",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TicketNumber",
                table: "FaultTickets",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceContracts_CompanyId_ProjectNumber",
                table: "MaintenanceContracts",
                columns: new[] { "CompanyId", "ProjectNumber" },
                unique: true,
                filter: "[ProjectNumber] IS NOT NULL AND [ProjectNumber] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_CompanyId_ItemNumber",
                table: "InventoryItems",
                columns: new[] { "CompanyId", "ItemNumber" },
                unique: true,
                filter: "[ItemNumber] IS NOT NULL AND [ItemNumber] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_InstallationProjects_CompanyId_ProjectNumber",
                table: "InstallationProjects",
                columns: new[] { "CompanyId", "ProjectNumber" },
                unique: true,
                filter: "[ProjectNumber] IS NOT NULL AND [ProjectNumber] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_FaultTickets_CompanyId_TicketNumber",
                table: "FaultTickets",
                columns: new[] { "CompanyId", "TicketNumber" },
                unique: true,
                filter: "[TicketNumber] IS NOT NULL AND [TicketNumber] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyTickets_CompanyId_TicketNumber",
                table: "EmergencyTickets",
                columns: new[] { "CompanyId", "TicketNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaintenanceContracts_CompanyId_ProjectNumber",
                table: "MaintenanceContracts");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_CompanyId_ItemNumber",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InstallationProjects_CompanyId_ProjectNumber",
                table: "InstallationProjects");

            migrationBuilder.DropIndex(
                name: "IX_FaultTickets_CompanyId_TicketNumber",
                table: "FaultTickets");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyTickets_CompanyId_TicketNumber",
                table: "EmergencyTickets");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectNumber",
                table: "MaintenanceContracts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ItemNumber",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "TicketNumber",
                table: "FaultTickets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_InstallationProjects_ProjectNumber",
                table: "InstallationProjects",
                column: "ProjectNumber",
                unique: true,
                filter: "[ProjectNumber] IS NOT NULL AND [ProjectNumber] <> ''");
        }
    }
}
