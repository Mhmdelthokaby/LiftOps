using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Company_PlanId_Fk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionPlan",
                table: "Companies");

            migrationBuilder.AddColumn<Guid>(
                name: "PlanId",
                table: "Companies",
                type: "uniqueidentifier",
                nullable: true);

            // Copy latest subscription plan per company onto Companies.PlanId (SubscriptionPlans seed unchanged).
            migrationBuilder.Sql(
                """
                UPDATE c
                SET c.[PlanId] = x.[PlanId]
                FROM [Companies] AS c
                OUTER APPLY (
                    SELECT TOP 1 s.[PlanId]
                    FROM [Subscriptions] AS s
                    WHERE s.[CompanyId] = c.[Id]
                    ORDER BY s.[CreatedAt] DESC
                ) AS x
                WHERE x.[PlanId] IS NOT NULL;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_PlanId",
                table: "Companies",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_SubscriptionPlans_PlanId",
                table: "Companies",
                column: "PlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_SubscriptionPlans_PlanId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_PlanId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionPlan",
                table: "Companies",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }
    }
}
