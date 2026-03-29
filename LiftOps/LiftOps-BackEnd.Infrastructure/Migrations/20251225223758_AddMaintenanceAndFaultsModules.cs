using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceAndFaultsModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnicianAssignments",
                table: "TechnicianAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TechnicianAssignments_TechnicianId",
                table: "TechnicianAssignments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnicianAssignments",
                table: "TechnicianAssignments",
                columns: new[] { "TechnicianId", "ElevatorId" });

            migrationBuilder.CreateTable(
                name: "FaultTicket",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleMapsLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ElevatorType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FaultDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    FaultDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedTechnicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolvedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaultTicket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaultTicket_Technicians_AssignedTechnicianId",
                        column: x => x.AssignedTechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceContract",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PricePerMonth = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FreeMonths = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FrozenReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FreezeEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceContract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceContract_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaultSparePartUsage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FaultTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PriceAtTimeOfUsage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaultSparePartUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaultSparePartUsage_FaultTicket_FaultTicketId",
                        column: x => x.FaultTicketId,
                        principalTable: "FaultTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaultSparePartUsage_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceElevator",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfStops = table.Column<int>(type: "int", nullable: false),
                    NumberOfFloors = table.Column<int>(type: "int", nullable: false),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InstallationElevatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceElevator", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceElevator_MaintenanceContract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "MaintenanceContract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceVisit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenanceElevatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TechnicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceVisit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceVisit_MaintenanceElevator_MaintenanceElevatorId",
                        column: x => x.MaintenanceElevatorId,
                        principalTable: "MaintenanceElevator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceVisit_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceSparePartUsage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenanceVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PriceAtTimeOfUsage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceSparePartUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceSparePartUsage_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceSparePartUsage_MaintenanceVisit_MaintenanceVisitId",
                        column: x => x.MaintenanceVisitId,
                        principalTable: "MaintenanceVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FaultSparePartUsage_FaultTicketId",
                table: "FaultSparePartUsage",
                column: "FaultTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultSparePartUsage_InventoryItemId",
                table: "FaultSparePartUsage",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultTicket_AssignedTechnicianId",
                table: "FaultTicket",
                column: "AssignedTechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceContract_CustomerId",
                table: "MaintenanceContract",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceElevator_ContractId",
                table: "MaintenanceElevator",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSparePartUsage_InventoryItemId",
                table: "MaintenanceSparePartUsage",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSparePartUsage_MaintenanceVisitId",
                table: "MaintenanceSparePartUsage",
                column: "MaintenanceVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceVisit_MaintenanceElevatorId",
                table: "MaintenanceVisit",
                column: "MaintenanceElevatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceVisit_TechnicianId",
                table: "MaintenanceVisit",
                column: "TechnicianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaultSparePartUsage");

            migrationBuilder.DropTable(
                name: "MaintenanceSparePartUsage");

            migrationBuilder.DropTable(
                name: "FaultTicket");

            migrationBuilder.DropTable(
                name: "MaintenanceVisit");

            migrationBuilder.DropTable(
                name: "MaintenanceElevator");

            migrationBuilder.DropTable(
                name: "MaintenanceContract");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnicianAssignments",
                table: "TechnicianAssignments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnicianAssignments",
                table: "TechnicianAssignments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianAssignments_TechnicianId",
                table: "TechnicianAssignments",
                column: "TechnicianId");
        }
    }
}
