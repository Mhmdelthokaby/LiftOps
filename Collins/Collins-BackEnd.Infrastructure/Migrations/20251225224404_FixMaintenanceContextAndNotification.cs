using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Collins_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMaintenanceContextAndNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaultSparePartUsage_FaultTicket_FaultTicketId",
                table: "FaultSparePartUsage");

            migrationBuilder.DropForeignKey(
                name: "FK_FaultSparePartUsage_InventoryItems_InventoryItemId",
                table: "FaultSparePartUsage");

            migrationBuilder.DropForeignKey(
                name: "FK_FaultTicket_Technicians_AssignedTechnicianId",
                table: "FaultTicket");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceContract_Customers_CustomerId",
                table: "MaintenanceContract");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceElevator_MaintenanceContract_ContractId",
                table: "MaintenanceElevator");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceSparePartUsage_InventoryItems_InventoryItemId",
                table: "MaintenanceSparePartUsage");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceSparePartUsage_MaintenanceVisit_MaintenanceVisitId",
                table: "MaintenanceSparePartUsage");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceVisit_MaintenanceElevator_MaintenanceElevatorId",
                table: "MaintenanceVisit");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceVisit_Technicians_TechnicianId",
                table: "MaintenanceVisit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceVisit",
                table: "MaintenanceVisit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceSparePartUsage",
                table: "MaintenanceSparePartUsage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceElevator",
                table: "MaintenanceElevator");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceContract",
                table: "MaintenanceContract");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FaultTicket",
                table: "FaultTicket");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FaultSparePartUsage",
                table: "FaultSparePartUsage");

            migrationBuilder.RenameTable(
                name: "MaintenanceVisit",
                newName: "MaintenanceVisits");

            migrationBuilder.RenameTable(
                name: "MaintenanceSparePartUsage",
                newName: "MaintenanceSparePartUsages");

            migrationBuilder.RenameTable(
                name: "MaintenanceElevator",
                newName: "MaintenanceElevators");

            migrationBuilder.RenameTable(
                name: "MaintenanceContract",
                newName: "MaintenanceContracts");

            migrationBuilder.RenameTable(
                name: "FaultTicket",
                newName: "FaultTickets");

            migrationBuilder.RenameTable(
                name: "FaultSparePartUsage",
                newName: "FaultSparePartUsages");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceVisit_TechnicianId",
                table: "MaintenanceVisits",
                newName: "IX_MaintenanceVisits_TechnicianId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceVisit_MaintenanceElevatorId",
                table: "MaintenanceVisits",
                newName: "IX_MaintenanceVisits_MaintenanceElevatorId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceSparePartUsage_MaintenanceVisitId",
                table: "MaintenanceSparePartUsages",
                newName: "IX_MaintenanceSparePartUsages_MaintenanceVisitId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceSparePartUsage_InventoryItemId",
                table: "MaintenanceSparePartUsages",
                newName: "IX_MaintenanceSparePartUsages_InventoryItemId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceElevator_ContractId",
                table: "MaintenanceElevators",
                newName: "IX_MaintenanceElevators_ContractId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceContract_CustomerId",
                table: "MaintenanceContracts",
                newName: "IX_MaintenanceContracts_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_FaultTicket_AssignedTechnicianId",
                table: "FaultTickets",
                newName: "IX_FaultTickets_AssignedTechnicianId");

            migrationBuilder.RenameIndex(
                name: "IX_FaultSparePartUsage_InventoryItemId",
                table: "FaultSparePartUsages",
                newName: "IX_FaultSparePartUsages_InventoryItemId");

            migrationBuilder.RenameIndex(
                name: "IX_FaultSparePartUsage_FaultTicketId",
                table: "FaultSparePartUsages",
                newName: "IX_FaultSparePartUsages_FaultTicketId");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceVisits",
                table: "MaintenanceVisits",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceSparePartUsages",
                table: "MaintenanceSparePartUsages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceElevators",
                table: "MaintenanceElevators",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceContracts",
                table: "MaintenanceContracts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FaultTickets",
                table: "FaultTickets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FaultSparePartUsages",
                table: "FaultSparePartUsages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FaultSparePartUsages_FaultTickets_FaultTicketId",
                table: "FaultSparePartUsages",
                column: "FaultTicketId",
                principalTable: "FaultTickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaultSparePartUsages_InventoryItems_InventoryItemId",
                table: "FaultSparePartUsages",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaultTickets_Technicians_AssignedTechnicianId",
                table: "FaultTickets",
                column: "AssignedTechnicianId",
                principalTable: "Technicians",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceContracts_Customers_CustomerId",
                table: "MaintenanceContracts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceElevators_MaintenanceContracts_ContractId",
                table: "MaintenanceElevators",
                column: "ContractId",
                principalTable: "MaintenanceContracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceSparePartUsages_InventoryItems_InventoryItemId",
                table: "MaintenanceSparePartUsages",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceSparePartUsages_MaintenanceVisits_MaintenanceVisitId",
                table: "MaintenanceSparePartUsages",
                column: "MaintenanceVisitId",
                principalTable: "MaintenanceVisits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceVisits_MaintenanceElevators_MaintenanceElevatorId",
                table: "MaintenanceVisits",
                column: "MaintenanceElevatorId",
                principalTable: "MaintenanceElevators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceVisits_Technicians_TechnicianId",
                table: "MaintenanceVisits",
                column: "TechnicianId",
                principalTable: "Technicians",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaultSparePartUsages_FaultTickets_FaultTicketId",
                table: "FaultSparePartUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_FaultSparePartUsages_InventoryItems_InventoryItemId",
                table: "FaultSparePartUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_FaultTickets_Technicians_AssignedTechnicianId",
                table: "FaultTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceContracts_Customers_CustomerId",
                table: "MaintenanceContracts");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceElevators_MaintenanceContracts_ContractId",
                table: "MaintenanceElevators");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceSparePartUsages_InventoryItems_InventoryItemId",
                table: "MaintenanceSparePartUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceSparePartUsages_MaintenanceVisits_MaintenanceVisitId",
                table: "MaintenanceSparePartUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceVisits_MaintenanceElevators_MaintenanceElevatorId",
                table: "MaintenanceVisits");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceVisits_Technicians_TechnicianId",
                table: "MaintenanceVisits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceVisits",
                table: "MaintenanceVisits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceSparePartUsages",
                table: "MaintenanceSparePartUsages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceElevators",
                table: "MaintenanceElevators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceContracts",
                table: "MaintenanceContracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FaultTickets",
                table: "FaultTickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FaultSparePartUsages",
                table: "FaultSparePartUsages");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");

            migrationBuilder.RenameTable(
                name: "MaintenanceVisits",
                newName: "MaintenanceVisit");

            migrationBuilder.RenameTable(
                name: "MaintenanceSparePartUsages",
                newName: "MaintenanceSparePartUsage");

            migrationBuilder.RenameTable(
                name: "MaintenanceElevators",
                newName: "MaintenanceElevator");

            migrationBuilder.RenameTable(
                name: "MaintenanceContracts",
                newName: "MaintenanceContract");

            migrationBuilder.RenameTable(
                name: "FaultTickets",
                newName: "FaultTicket");

            migrationBuilder.RenameTable(
                name: "FaultSparePartUsages",
                newName: "FaultSparePartUsage");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceVisits_TechnicianId",
                table: "MaintenanceVisit",
                newName: "IX_MaintenanceVisit_TechnicianId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceVisits_MaintenanceElevatorId",
                table: "MaintenanceVisit",
                newName: "IX_MaintenanceVisit_MaintenanceElevatorId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceSparePartUsages_MaintenanceVisitId",
                table: "MaintenanceSparePartUsage",
                newName: "IX_MaintenanceSparePartUsage_MaintenanceVisitId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceSparePartUsages_InventoryItemId",
                table: "MaintenanceSparePartUsage",
                newName: "IX_MaintenanceSparePartUsage_InventoryItemId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceElevators_ContractId",
                table: "MaintenanceElevator",
                newName: "IX_MaintenanceElevator_ContractId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceContracts_CustomerId",
                table: "MaintenanceContract",
                newName: "IX_MaintenanceContract_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_FaultTickets_AssignedTechnicianId",
                table: "FaultTicket",
                newName: "IX_FaultTicket_AssignedTechnicianId");

            migrationBuilder.RenameIndex(
                name: "IX_FaultSparePartUsages_InventoryItemId",
                table: "FaultSparePartUsage",
                newName: "IX_FaultSparePartUsage_InventoryItemId");

            migrationBuilder.RenameIndex(
                name: "IX_FaultSparePartUsages_FaultTicketId",
                table: "FaultSparePartUsage",
                newName: "IX_FaultSparePartUsage_FaultTicketId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceVisit",
                table: "MaintenanceVisit",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceSparePartUsage",
                table: "MaintenanceSparePartUsage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceElevator",
                table: "MaintenanceElevator",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceContract",
                table: "MaintenanceContract",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FaultTicket",
                table: "FaultTicket",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FaultSparePartUsage",
                table: "FaultSparePartUsage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FaultSparePartUsage_FaultTicket_FaultTicketId",
                table: "FaultSparePartUsage",
                column: "FaultTicketId",
                principalTable: "FaultTicket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaultSparePartUsage_InventoryItems_InventoryItemId",
                table: "FaultSparePartUsage",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaultTicket_Technicians_AssignedTechnicianId",
                table: "FaultTicket",
                column: "AssignedTechnicianId",
                principalTable: "Technicians",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceContract_Customers_CustomerId",
                table: "MaintenanceContract",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceElevator_MaintenanceContract_ContractId",
                table: "MaintenanceElevator",
                column: "ContractId",
                principalTable: "MaintenanceContract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceSparePartUsage_InventoryItems_InventoryItemId",
                table: "MaintenanceSparePartUsage",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceSparePartUsage_MaintenanceVisit_MaintenanceVisitId",
                table: "MaintenanceSparePartUsage",
                column: "MaintenanceVisitId",
                principalTable: "MaintenanceVisit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceVisit_MaintenanceElevator_MaintenanceElevatorId",
                table: "MaintenanceVisit",
                column: "MaintenanceElevatorId",
                principalTable: "MaintenanceElevator",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceVisit_Technicians_TechnicianId",
                table: "MaintenanceVisit",
                column: "TechnicianId",
                principalTable: "Technicians",
                principalColumn: "Id");
        }
    }
}
