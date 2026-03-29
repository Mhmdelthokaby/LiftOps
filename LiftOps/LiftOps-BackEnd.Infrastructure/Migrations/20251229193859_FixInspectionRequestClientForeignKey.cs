using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixInspectionRequestClientForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionRequests_Customers_ClientId",
                table: "InspectionRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionRequests_Customers_ClientId",
                table: "InspectionRequests",
                column: "ClientId",
                principalTable: "Customers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionRequests_Customers_ClientId",
                table: "InspectionRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionRequests_Customers_ClientId",
                table: "InspectionRequests",
                column: "ClientId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
