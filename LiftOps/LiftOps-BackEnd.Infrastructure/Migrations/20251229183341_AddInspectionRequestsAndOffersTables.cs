using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionRequestsAndOffersTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConvertedFromInspectionId",
                table: "InstallationProjects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleMapsLink",
                table: "InstallationProjects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LastFloorHeight",
                table: "InstallationProjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PitDepth",
                table: "InstallationProjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShaftDepth",
                table: "InstallationProjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShaftType",
                table: "InstallationProjects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShaftWidth",
                table: "InstallationProjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechnicalNotes",
                table: "InstallationProjects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TravelHeight",
                table: "InstallationProjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InspectionRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GoogleMapsLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfElevatorsRequired = table.Column<int>(type: "int", nullable: false),
                    ElevatorType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShaftType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShaftWidth = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ShaftDepth = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LastFloorHeight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PitDepth = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TravelHeight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TechnicalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConvertedToProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionRequests_InstallationProjects_ConvertedToProjectId",
                        column: x => x.ConvertedToProjectId,
                        principalTable: "InstallationProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectionRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstallationPricePerUnit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalInstallationPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstimatedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfferPdfPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offers_InspectionRequests_InspectionRequestId",
                        column: x => x.InspectionRequestId,
                        principalTable: "InspectionRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionRequests_ConvertedToProjectId",
                table: "InspectionRequests",
                column: "ConvertedToProjectId",
                unique: true,
                filter: "[ConvertedToProjectId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_InspectionRequestId",
                table: "Offers",
                column: "InspectionRequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DropTable(
                name: "InspectionRequests");

            migrationBuilder.DropColumn(
                name: "ConvertedFromInspectionId",
                table: "InstallationProjects");

            migrationBuilder.DropColumn(
                name: "GoogleMapsLink",
                table: "InstallationProjects");

            migrationBuilder.DropColumn(
                name: "LastFloorHeight",
                table: "InstallationProjects");

            migrationBuilder.DropColumn(
                name: "PitDepth",
                table: "InstallationProjects");

            migrationBuilder.DropColumn(
                name: "ShaftDepth",
                table: "InstallationProjects");

            migrationBuilder.DropColumn(
                name: "ShaftType",
                table: "InstallationProjects");

            migrationBuilder.DropColumn(
                name: "ShaftWidth",
                table: "InstallationProjects");

            migrationBuilder.DropColumn(
                name: "TechnicalNotes",
                table: "InstallationProjects");

            migrationBuilder.DropColumn(
                name: "TravelHeight",
                table: "InstallationProjects");
        }
    }
}
