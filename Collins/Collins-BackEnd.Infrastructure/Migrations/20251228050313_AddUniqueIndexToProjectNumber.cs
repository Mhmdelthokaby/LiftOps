using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Collins_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToProjectNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, fix any duplicate project numbers by generating unique ones
            migrationBuilder.Sql(@"
                DECLARE @Counter INT = 1;
                DECLARE @ProjectId UNIQUEIDENTIFIER;
                DECLARE @NewProjectNumber NVARCHAR(50);
                DECLARE @CurrentProjectNumber NVARCHAR(50);
                DECLARE @MaxNumber INT;
                
                -- Find the highest existing number
                SELECT @MaxNumber = ISNULL(MAX(CASE 
                    WHEN ProjectNumber LIKE 'PR-[0-9]%' OR ProjectNumber LIKE 'PR-[0-9][0-9]%'
                    THEN CAST(SUBSTRING(ProjectNumber, 4, LEN(ProjectNumber)) AS INT)
                    ELSE 0
                END), 0)
                FROM InstallationProjects
                WHERE ProjectNumber IS NOT NULL AND ProjectNumber <> '';
                
                SET @Counter = @MaxNumber + 1;
                
                -- Find all duplicate project numbers
                DECLARE duplicate_cursor CURSOR FOR
                SELECT p1.Id, p1.ProjectNumber
                FROM InstallationProjects p1
                WHERE p1.ProjectNumber IS NOT NULL AND p1.ProjectNumber <> ''
                AND EXISTS (
                    SELECT 1 FROM InstallationProjects p2 
                    WHERE p2.Id <> p1.Id 
                    AND p2.ProjectNumber = p1.ProjectNumber
                )
                ORDER BY p1.CreatedAt;
                
                OPEN duplicate_cursor;
                FETCH NEXT FROM duplicate_cursor INTO @ProjectId, @CurrentProjectNumber;
                
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    SET @NewProjectNumber = 'PR-' + RIGHT('00' + CAST(@Counter AS VARCHAR), 2);
                    
                    -- Check if this number already exists
                    WHILE EXISTS (SELECT 1 FROM InstallationProjects WHERE ProjectNumber = @NewProjectNumber)
                    BEGIN
                        SET @Counter = @Counter + 1;
                        SET @NewProjectNumber = 'PR-' + RIGHT('00' + CAST(@Counter AS VARCHAR), 2);
                    END
                    
                    -- Update the duplicate project number
                    UPDATE InstallationProjects
                    SET ProjectNumber = @NewProjectNumber
                    WHERE Id = @ProjectId;
                    
                    SET @Counter = @Counter + 1;
                    FETCH NEXT FROM duplicate_cursor INTO @ProjectId, @CurrentProjectNumber;
                END
                
                CLOSE duplicate_cursor;
                DEALLOCATE duplicate_cursor;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectNumber",
                table: "InstallationProjects",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_InstallationProjects_ProjectNumber",
                table: "InstallationProjects",
                column: "ProjectNumber",
                unique: true,
                filter: "[ProjectNumber] IS NOT NULL AND [ProjectNumber] <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InstallationProjects_ProjectNumber",
                table: "InstallationProjects");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectNumber",
                table: "InstallationProjects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
