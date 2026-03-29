using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Collins_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStageTechnicianTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StageTechnicians",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TechnicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageTechnicians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StageTechnicians_InstallationStages_StageId",
                        column: x => x.StageId,
                        principalTable: "InstallationStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StageTechnicians_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StageTechnicians_StageId",
                table: "StageTechnicians",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_StageTechnicians_TechnicianId",
                table: "StageTechnicians",
                column: "TechnicianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StageTechnicians");
        }
    }
}
