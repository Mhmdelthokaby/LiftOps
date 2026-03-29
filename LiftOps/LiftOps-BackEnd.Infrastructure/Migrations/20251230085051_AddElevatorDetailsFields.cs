using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddElevatorDetailsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add temporary column for enum conversion
            migrationBuilder.AddColumn<int>(
                name: "ElevatorTypeTemp",
                table: "Elevators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Step 2: Convert existing string values to enum values
            // Map common string values to enum: WithMachineRoom=0, MachineRoomLess=1, Hydraulic=2
            migrationBuilder.Sql(@"
                UPDATE [Elevators] 
                SET [ElevatorTypeTemp] = CASE 
                    WHEN [ElevatorType] IN ('WithMachineRoom', 'With Machine Room', 'مع غرفة') THEN 0
                    WHEN [ElevatorType] IN ('MachineRoomLess', 'Machine Room Less', 'MRL', 'بدون غرفة') THEN 1
                    WHEN [ElevatorType] IN ('Hydraulic', 'هيدروليك') THEN 2
                    ELSE 0  -- Default to WithMachineRoom for unknown values
                END
            ");

            // Step 3: Drop old column and rename temp column
            migrationBuilder.DropColumn(
                name: "ElevatorType",
                table: "Elevators");

            migrationBuilder.RenameColumn(
                name: "ElevatorTypeTemp",
                table: "Elevators",
                newName: "ElevatorType");

            migrationBuilder.AddColumn<int>(
                name: "FloorsCount",
                table: "Elevators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "HoleDepth",
                table: "Elevators",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LastFloorHeight",
                table: "Elevators",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Elevators",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PitDepth",
                table: "Elevators",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PitType",
                table: "Elevators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PitWidth",
                table: "Elevators",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Elevators",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StopsCount",
                table: "Elevators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TravelLength",
                table: "Elevators",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FloorsCount",
                table: "Elevators");

            migrationBuilder.DropColumn(
                name: "HoleDepth",
                table: "Elevators");

            migrationBuilder.DropColumn(
                name: "LastFloorHeight",
                table: "Elevators");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Elevators");

            migrationBuilder.DropColumn(
                name: "PitDepth",
                table: "Elevators");

            migrationBuilder.DropColumn(
                name: "PitType",
                table: "Elevators");

            migrationBuilder.DropColumn(
                name: "PitWidth",
                table: "Elevators");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Elevators");

            migrationBuilder.DropColumn(
                name: "StopsCount",
                table: "Elevators");

            migrationBuilder.DropColumn(
                name: "TravelLength",
                table: "Elevators");

            // Convert enum back to string for rollback
            migrationBuilder.AddColumn<string>(
                name: "ElevatorTypeTemp",
                table: "Elevators",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE [Elevators] 
                SET [ElevatorTypeTemp] = CASE 
                    WHEN [ElevatorType] = 0 THEN 'WithMachineRoom'
                    WHEN [ElevatorType] = 1 THEN 'MachineRoomLess'
                    WHEN [ElevatorType] = 2 THEN 'Hydraulic'
                    ELSE 'WithMachineRoom'
                END
            ");

            migrationBuilder.DropColumn(
                name: "ElevatorType",
                table: "Elevators");

            migrationBuilder.RenameColumn(
                name: "ElevatorTypeTemp",
                table: "Elevators",
                newName: "ElevatorType");
        }
    }
}
