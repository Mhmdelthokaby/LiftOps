using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Collins_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianIdToMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TechnicianId",
                table: "MaintenanceContracts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceContracts_TechnicianId",
                table: "MaintenanceContracts",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceContracts_Technicians_TechnicianId",
                table: "MaintenanceContracts",
                column: "TechnicianId",
                principalTable: "Technicians",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceContracts_Technicians_TechnicianId",
                table: "MaintenanceContracts");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceContracts_TechnicianId",
                table: "MaintenanceContracts");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "MaintenanceContracts");
        }
    }
}
