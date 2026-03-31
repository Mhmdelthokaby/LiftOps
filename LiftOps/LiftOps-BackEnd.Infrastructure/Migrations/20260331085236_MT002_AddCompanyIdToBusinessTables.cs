using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MT002_AddCompanyIdToBusinessTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Technicians",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "TechnicianAssignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "StageTechnicians",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "StageRequiredParts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Quotations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "QuotationAttachments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Offers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "MaintenanceVisits",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "MaintenanceVisitChecklistItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "MaintenanceSparePartUsages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "MaintenanceElevators",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "MaintenanceContracts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "MaintenanceChecklistItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "InventoryItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "InstallationStages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "InstallationProjects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "InspectionRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "FaultTickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "FaultSparePartUsages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "EmergencyTickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Elevators",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "TechnicianAssignments");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "StageTechnicians");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "StageRequiredParts");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "QuotationAttachments");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "MaintenanceVisits");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "MaintenanceVisitChecklistItems");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "MaintenanceSparePartUsages");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "MaintenanceElevators");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "MaintenanceContracts");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "MaintenanceChecklistItems");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "InstallationStages");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "InstallationProjects");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "InspectionRequests");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "FaultTickets");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "FaultSparePartUsages");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "EmergencyTickets");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Elevators");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Categories");
        }
    }
}
