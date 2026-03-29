using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Collins_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaderToTechnician : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LeaderId",
                table: "Technicians",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Technicians_LeaderId",
                table: "Technicians",
                column: "LeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Technicians_Technicians_LeaderId",
                table: "Technicians",
                column: "LeaderId",
                principalTable: "Technicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Technicians_Technicians_LeaderId",
                table: "Technicians");

            migrationBuilder.DropIndex(
                name: "IX_Technicians_LeaderId",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "LeaderId",
                table: "Technicians");
        }
    }
}
