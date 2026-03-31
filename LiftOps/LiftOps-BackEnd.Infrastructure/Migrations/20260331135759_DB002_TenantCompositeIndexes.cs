using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DB002_TenantCompositeIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "MaintenanceVisitChecklistItems",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Technicians_CompanyId_CreatedAt",
                table: "Technicians",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianAssignments_CompanyId_CreatedAt",
                table: "TechnicianAssignments",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StageTechnicians_CompanyId_CreatedAt",
                table: "StageTechnicians",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StageRequiredParts_CompanyId_CreatedAt",
                table: "StageRequiredParts",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_CompanyId_CreatedAt",
                table: "Quotations",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_CompanyId_Status",
                table: "Quotations",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_QuotationAttachments_CompanyId_CreatedAt",
                table: "QuotationAttachments",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CompanyId_CreatedAt",
                table: "Offers",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CompanyId_Status",
                table: "Offers",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CompanyId_CreatedAt",
                table: "Notifications",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceVisits_CompanyId_CreatedAt",
                table: "MaintenanceVisits",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceVisits_CompanyId_Status",
                table: "MaintenanceVisits",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceVisitChecklistItems_CompanyId_CreatedAt",
                table: "MaintenanceVisitChecklistItems",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceVisitChecklistItems_CompanyId_Status",
                table: "MaintenanceVisitChecklistItems",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSparePartUsages_CompanyId_CreatedAt",
                table: "MaintenanceSparePartUsages",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceElevators_CompanyId_CreatedAt",
                table: "MaintenanceElevators",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceElevators_CompanyId_Status",
                table: "MaintenanceElevators",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceContracts_CompanyId_CreatedAt",
                table: "MaintenanceContracts",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceContracts_CompanyId_Status",
                table: "MaintenanceContracts",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceChecklistItems_CompanyId_CreatedAt",
                table: "MaintenanceChecklistItems",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_CompanyId_CreatedAt",
                table: "InventoryItems",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InstallationStages_CompanyId_CreatedAt",
                table: "InstallationStages",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InstallationStages_CompanyId_Status",
                table: "InstallationStages",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_InstallationProjects_CompanyId_CreatedAt",
                table: "InstallationProjects",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionRequests_CompanyId_CreatedAt",
                table: "InspectionRequests",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionRequests_CompanyId_Status",
                table: "InspectionRequests",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FaultTickets_CompanyId_CreatedAt",
                table: "FaultTickets",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FaultTickets_CompanyId_Status",
                table: "FaultTickets",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FaultSparePartUsages_CompanyId_CreatedAt",
                table: "FaultSparePartUsages",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyTickets_CompanyId_CreatedAt",
                table: "EmergencyTickets",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyTickets_CompanyId_Status",
                table: "EmergencyTickets",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Elevators_CompanyId_CreatedAt",
                table: "Elevators",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CompanyId_CreatedAt",
                table: "Customers",
                columns: new[] { "CompanyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CompanyId_Status",
                table: "Customers",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CompanyId_CreatedAt",
                table: "Categories",
                columns: new[] { "CompanyId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Technicians_CompanyId_CreatedAt",
                table: "Technicians");

            migrationBuilder.DropIndex(
                name: "IX_TechnicianAssignments_CompanyId_CreatedAt",
                table: "TechnicianAssignments");

            migrationBuilder.DropIndex(
                name: "IX_StageTechnicians_CompanyId_CreatedAt",
                table: "StageTechnicians");

            migrationBuilder.DropIndex(
                name: "IX_StageRequiredParts_CompanyId_CreatedAt",
                table: "StageRequiredParts");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_CompanyId_CreatedAt",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_CompanyId_Status",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_QuotationAttachments_CompanyId_CreatedAt",
                table: "QuotationAttachments");

            migrationBuilder.DropIndex(
                name: "IX_Offers_CompanyId_CreatedAt",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_CompanyId_Status",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CompanyId_CreatedAt",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceVisits_CompanyId_CreatedAt",
                table: "MaintenanceVisits");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceVisits_CompanyId_Status",
                table: "MaintenanceVisits");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceVisitChecklistItems_CompanyId_CreatedAt",
                table: "MaintenanceVisitChecklistItems");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceVisitChecklistItems_CompanyId_Status",
                table: "MaintenanceVisitChecklistItems");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceSparePartUsages_CompanyId_CreatedAt",
                table: "MaintenanceSparePartUsages");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceElevators_CompanyId_CreatedAt",
                table: "MaintenanceElevators");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceElevators_CompanyId_Status",
                table: "MaintenanceElevators");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceContracts_CompanyId_CreatedAt",
                table: "MaintenanceContracts");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceContracts_CompanyId_Status",
                table: "MaintenanceContracts");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceChecklistItems_CompanyId_CreatedAt",
                table: "MaintenanceChecklistItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_CompanyId_CreatedAt",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InstallationStages_CompanyId_CreatedAt",
                table: "InstallationStages");

            migrationBuilder.DropIndex(
                name: "IX_InstallationStages_CompanyId_Status",
                table: "InstallationStages");

            migrationBuilder.DropIndex(
                name: "IX_InstallationProjects_CompanyId_CreatedAt",
                table: "InstallationProjects");

            migrationBuilder.DropIndex(
                name: "IX_InspectionRequests_CompanyId_CreatedAt",
                table: "InspectionRequests");

            migrationBuilder.DropIndex(
                name: "IX_InspectionRequests_CompanyId_Status",
                table: "InspectionRequests");

            migrationBuilder.DropIndex(
                name: "IX_FaultTickets_CompanyId_CreatedAt",
                table: "FaultTickets");

            migrationBuilder.DropIndex(
                name: "IX_FaultTickets_CompanyId_Status",
                table: "FaultTickets");

            migrationBuilder.DropIndex(
                name: "IX_FaultSparePartUsages_CompanyId_CreatedAt",
                table: "FaultSparePartUsages");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyTickets_CompanyId_CreatedAt",
                table: "EmergencyTickets");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyTickets_CompanyId_Status",
                table: "EmergencyTickets");

            migrationBuilder.DropIndex(
                name: "IX_Elevators_CompanyId_CreatedAt",
                table: "Elevators");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CompanyId_CreatedAt",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CompanyId_Status",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CompanyId_CreatedAt",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "MaintenanceVisitChecklistItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
