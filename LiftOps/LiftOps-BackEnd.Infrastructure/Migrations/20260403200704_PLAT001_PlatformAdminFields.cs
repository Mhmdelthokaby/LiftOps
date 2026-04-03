using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PLAT001_PlatformAdminFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillingCycle",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentPeriodStart",
                table: "Subscriptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<bool>(
                name: "AllowApiAccess",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowEmergencyModule",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowFaultsModule",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowFinanceModule",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowInventoryModule",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxElevators",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.AddColumn<int>(
                name: "MaxInstallationProjects",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.AddColumn<int>(
                name: "MaxMaintenanceContracts",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.AddColumn<int>(
                name: "MaxUsers",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.AddColumn<int>(
                name: "TrialDays",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 14);

            migrationBuilder.AddColumn<decimal>(
                name: "YearlyPrice",
                table: "SubscriptionPlans",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedAt",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspensionReason",
                table: "Companies",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantStatus",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                """
                UPDATE [Subscriptions] SET [CurrentPeriodStart] = [CreatedAt]
                WHERE [CurrentPeriodStart] < '2000-01-02'
                """);

            migrationBuilder.Sql(
                """
                UPDATE [SubscriptionPlans] SET [YearlyPrice] = [MonthlyPrice] * 12
                WHERE [YearlyPrice] = 0
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingCycle",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "CurrentPeriodStart",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "AllowApiAccess",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "AllowEmergencyModule",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "AllowFaultsModule",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "AllowFinanceModule",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "AllowInventoryModule",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "MaxElevators",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "MaxInstallationProjects",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "MaxMaintenanceContracts",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "MaxUsers",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "TrialDays",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "YearlyPrice",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "SuspendedAt",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "SuspensionReason",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "TenantStatus",
                table: "Companies");
        }
    }
}
