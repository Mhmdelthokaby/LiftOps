using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExistingElevatorData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing records to populate FloorsCount and StopsCount from legacy fields
            migrationBuilder.Sql(@"
                UPDATE [Elevators] 
                SET [FloorsCount] = CASE 
                    WHEN [FloorsCount] = 0 AND [NumberOfFloors] > 0 THEN [NumberOfFloors]
                    ELSE [FloorsCount]
                END,
                [StopsCount] = CASE 
                    WHEN [StopsCount] = 0 AND [NumberOfStops] > 0 THEN [NumberOfStops]
                    ELSE [StopsCount]
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
