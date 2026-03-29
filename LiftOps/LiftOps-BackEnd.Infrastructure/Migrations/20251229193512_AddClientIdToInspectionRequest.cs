using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientIdToInspectionRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "InspectionRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionRequests_ClientId",
                table: "InspectionRequests",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionRequests_Customers_ClientId",
                table: "InspectionRequests",
                column: "ClientId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionRequests_Customers_ClientId",
                table: "InspectionRequests");

            migrationBuilder.DropIndex(
                name: "IX_InspectionRequests_ClientId",
                table: "InspectionRequests");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "InspectionRequests");
        }
    }
}
