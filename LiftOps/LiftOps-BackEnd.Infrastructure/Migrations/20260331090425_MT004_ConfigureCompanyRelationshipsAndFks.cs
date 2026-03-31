using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MT004_ConfigureCompanyRelationshipsAndFks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Technicians_CompanyId",
                table: "Technicians",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianAssignments_CompanyId",
                table: "TechnicianAssignments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StageTechnicians_CompanyId",
                table: "StageTechnicians",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StageRequiredParts_CompanyId",
                table: "StageRequiredParts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_CompanyId",
                table: "Quotations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationAttachments_CompanyId",
                table: "QuotationAttachments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CompanyId",
                table: "Offers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CompanyId",
                table: "Notifications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceVisits_CompanyId",
                table: "MaintenanceVisits",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceVisitChecklistItems_CompanyId",
                table: "MaintenanceVisitChecklistItems",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSparePartUsages_CompanyId",
                table: "MaintenanceSparePartUsages",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceElevators_CompanyId",
                table: "MaintenanceElevators",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceContracts_CompanyId",
                table: "MaintenanceContracts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceChecklistItems_CompanyId",
                table: "MaintenanceChecklistItems",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_CompanyId",
                table: "InventoryItems",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallationStages_CompanyId",
                table: "InstallationStages",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallationProjects_CompanyId",
                table: "InstallationProjects",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionRequests_CompanyId",
                table: "InspectionRequests",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultTickets_CompanyId",
                table: "FaultTickets",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultSparePartUsages_CompanyId",
                table: "FaultSparePartUsages",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyTickets_CompanyId",
                table: "EmergencyTickets",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Elevators_CompanyId",
                table: "Elevators",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CompanyId",
                table: "Customers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CompanyId",
                table: "Categories",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Companies_CompanyId",
                table: "Categories",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Companies_CompanyId",
                table: "Customers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Elevators_Companies_CompanyId",
                table: "Elevators",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyTickets_Companies_CompanyId",
                table: "EmergencyTickets",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaultSparePartUsages_Companies_CompanyId",
                table: "FaultSparePartUsages",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaultTickets_Companies_CompanyId",
                table: "FaultTickets",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionRequests_Companies_CompanyId",
                table: "InspectionRequests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallationProjects_Companies_CompanyId",
                table: "InstallationProjects",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallationStages_Companies_CompanyId",
                table: "InstallationStages",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Companies_CompanyId",
                table: "InventoryItems",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceChecklistItems_Companies_CompanyId",
                table: "MaintenanceChecklistItems",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceContracts_Companies_CompanyId",
                table: "MaintenanceContracts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceElevators_Companies_CompanyId",
                table: "MaintenanceElevators",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceSparePartUsages_Companies_CompanyId",
                table: "MaintenanceSparePartUsages",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceVisitChecklistItems_Companies_CompanyId",
                table: "MaintenanceVisitChecklistItems",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceVisits_Companies_CompanyId",
                table: "MaintenanceVisits",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Companies_CompanyId",
                table: "Notifications",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Companies_CompanyId",
                table: "Offers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationAttachments_Companies_CompanyId",
                table: "QuotationAttachments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Companies_CompanyId",
                table: "Quotations",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StageRequiredParts_Companies_CompanyId",
                table: "StageRequiredParts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StageTechnicians_Companies_CompanyId",
                table: "StageTechnicians",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicianAssignments_Companies_CompanyId",
                table: "TechnicianAssignments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Technicians_Companies_CompanyId",
                table: "Technicians",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Companies_CompanyId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Companies_CompanyId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Elevators_Companies_CompanyId",
                table: "Elevators");

            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyTickets_Companies_CompanyId",
                table: "EmergencyTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_FaultSparePartUsages_Companies_CompanyId",
                table: "FaultSparePartUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_FaultTickets_Companies_CompanyId",
                table: "FaultTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionRequests_Companies_CompanyId",
                table: "InspectionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_InstallationProjects_Companies_CompanyId",
                table: "InstallationProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_InstallationStages_Companies_CompanyId",
                table: "InstallationStages");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Companies_CompanyId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceChecklistItems_Companies_CompanyId",
                table: "MaintenanceChecklistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceContracts_Companies_CompanyId",
                table: "MaintenanceContracts");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceElevators_Companies_CompanyId",
                table: "MaintenanceElevators");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceSparePartUsages_Companies_CompanyId",
                table: "MaintenanceSparePartUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceVisitChecklistItems_Companies_CompanyId",
                table: "MaintenanceVisitChecklistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceVisits_Companies_CompanyId",
                table: "MaintenanceVisits");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Companies_CompanyId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Companies_CompanyId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationAttachments_Companies_CompanyId",
                table: "QuotationAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Companies_CompanyId",
                table: "Quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_StageRequiredParts_Companies_CompanyId",
                table: "StageRequiredParts");

            migrationBuilder.DropForeignKey(
                name: "FK_StageTechnicians_Companies_CompanyId",
                table: "StageTechnicians");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnicianAssignments_Companies_CompanyId",
                table: "TechnicianAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Technicians_Companies_CompanyId",
                table: "Technicians");

            migrationBuilder.DropIndex(
                name: "IX_Technicians_CompanyId",
                table: "Technicians");

            migrationBuilder.DropIndex(
                name: "IX_TechnicianAssignments_CompanyId",
                table: "TechnicianAssignments");

            migrationBuilder.DropIndex(
                name: "IX_StageTechnicians_CompanyId",
                table: "StageTechnicians");

            migrationBuilder.DropIndex(
                name: "IX_StageRequiredParts_CompanyId",
                table: "StageRequiredParts");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_CompanyId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_QuotationAttachments_CompanyId",
                table: "QuotationAttachments");

            migrationBuilder.DropIndex(
                name: "IX_Offers_CompanyId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CompanyId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceVisits_CompanyId",
                table: "MaintenanceVisits");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceVisitChecklistItems_CompanyId",
                table: "MaintenanceVisitChecklistItems");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceSparePartUsages_CompanyId",
                table: "MaintenanceSparePartUsages");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceElevators_CompanyId",
                table: "MaintenanceElevators");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceContracts_CompanyId",
                table: "MaintenanceContracts");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceChecklistItems_CompanyId",
                table: "MaintenanceChecklistItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_CompanyId",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InstallationStages_CompanyId",
                table: "InstallationStages");

            migrationBuilder.DropIndex(
                name: "IX_InstallationProjects_CompanyId",
                table: "InstallationProjects");

            migrationBuilder.DropIndex(
                name: "IX_InspectionRequests_CompanyId",
                table: "InspectionRequests");

            migrationBuilder.DropIndex(
                name: "IX_FaultTickets_CompanyId",
                table: "FaultTickets");

            migrationBuilder.DropIndex(
                name: "IX_FaultSparePartUsages_CompanyId",
                table: "FaultSparePartUsages");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyTickets_CompanyId",
                table: "EmergencyTickets");

            migrationBuilder.DropIndex(
                name: "IX_Elevators_CompanyId",
                table: "Elevators");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CompanyId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CompanyId",
                table: "Categories");
        }
    }
}
