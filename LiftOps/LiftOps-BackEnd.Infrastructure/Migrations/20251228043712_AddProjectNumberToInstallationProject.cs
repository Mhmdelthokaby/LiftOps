using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftOps_BackEnd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectNumberToInstallationProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectNumber",
                table: "InstallationProjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // Migrate existing ProjectNumber from Customer to InstallationProject
            // For each project, copy the ProjectNumber from its Customer
            migrationBuilder.Sql(@"
                UPDATE InstallationProjects
                SET ProjectNumber = (
                    SELECT ProjectNumber 
                    FROM Customers 
                    WHERE Customers.Id = InstallationProjects.CustomerId
                )
                WHERE ProjectNumber = '' OR ProjectNumber IS NULL
            ");

            // For projects that still don't have a ProjectNumber, generate one
            migrationBuilder.Sql(@"
                DECLARE @Counter INT = 1;
                DECLARE @ProjectId UNIQUEIDENTIFIER;
                DECLARE @NewProjectNumber NVARCHAR(50);
                
                DECLARE project_cursor CURSOR FOR
                SELECT Id FROM InstallationProjects WHERE ProjectNumber = '' OR ProjectNumber IS NULL
                ORDER BY CreatedAt;
                
                OPEN project_cursor;
                FETCH NEXT FROM project_cursor INTO @ProjectId;
                
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    SET @NewProjectNumber = 'PR-' + RIGHT('00' + CAST(@Counter AS VARCHAR), 2);
                    
                    -- Check if this number already exists
                    WHILE EXISTS (SELECT 1 FROM InstallationProjects WHERE ProjectNumber = @NewProjectNumber)
                    BEGIN
                        SET @Counter = @Counter + 1;
                        SET @NewProjectNumber = 'PR-' + RIGHT('00' + CAST(@Counter AS VARCHAR), 2);
                    END
                    
                    UPDATE InstallationProjects
                    SET ProjectNumber = @NewProjectNumber
                    WHERE Id = @ProjectId;
                    
                    SET @Counter = @Counter + 1;
                    FETCH NEXT FROM project_cursor INTO @ProjectId;
                END
                
                CLOSE project_cursor;
                DEALLOCATE project_cursor;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectNumber",
                table: "InstallationProjects");
        }
    }
}
