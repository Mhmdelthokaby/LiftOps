using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Collins_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToTechnician : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Technicians",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Technicians");
        }
    }
}
