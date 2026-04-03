/*
  Use this when: the database already has the LiftOps schema (tables like AspNetRoles exist),
  but __EFMigrationsHistory is empty or incomplete, so `Database.Migrate()` fails with
  "There is already an object named 'AspNetRoles'".

  Steps:
  1. Connect to the target database in SSMS / Azure Data Studio.
  2. If your schema is up to date through SUB001 (subscriptions) but NOT yet PLAT001:
     - Run every INSERT block below EXCEPT the last one (PLAT001).
     - Then run: dotnet ef database update --project LiftOps-BackEnd.Infrastructure --startup-project LiftOps-BackEnd.API
  3. If your schema already includes PLAT001 columns (Companies.TenantStatus, etc.):
     - Run all INSERT blocks including PLAT001.
  4. If this is a throwaway dev database: prefer dropping the database and running Migrate() on a clean DB.

  ProductVersion must match your EF Core runtime (10.0.1 for this solution).
*/

SET NOCOUNT ON;

IF OBJECT_ID(N'[__EFMigrationsHistory]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId])
    );
END

DECLARE @pv nvarchar(32) = N'10.0.1';

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251224182112_AddInventoryItemTable')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251224182112_AddInventoryItemTable', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251224184151_AddCategoryEntity')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251224184151_AddCategoryEntity', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251225211623_AddInstallationModule')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251225211623_AddInstallationModule', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251225220848_AddTechnicianModule')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251225220848_AddTechnicianModule', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251225223758_AddMaintenanceAndFaultsModules')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251225223758_AddMaintenanceAndFaultsModules', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251225224404_FixMaintenanceContextAndNotification')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251225224404_FixMaintenanceContextAndNotification', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251227133019_AddItemNumberToInventoryItem')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251227133019_AddItemNumberToInventoryItem', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251227204705_AddStagePriceAndCollectionFields')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251227204705_AddStagePriceAndCollectionFields', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251228043712_AddProjectNumberToInstallationProject')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251228043712_AddProjectNumberToInstallationProject', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251228050313_AddUniqueIndexToProjectNumber')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251228050313_AddUniqueIndexToProjectNumber', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251228190348_AddProjectAddressToInstallationProject')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251228190348_AddProjectAddressToInstallationProject', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251229183341_AddInspectionRequestsAndOffersTables')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251229183341_AddInspectionRequestsAndOffersTables', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251229193512_AddClientIdToInspectionRequest')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251229193512_AddClientIdToInspectionRequest', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251229193859_FixInspectionRequestClientForeignKey')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251229193859_FixInspectionRequestClientForeignKey', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251230073300_AddInspectionQuotationFlow')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251230073300_AddInspectionQuotationFlow', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251230085051_AddElevatorDetailsFields')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251230085051_AddElevatorDetailsFields', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251230085514_UpdateExistingElevatorData')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251230085514_UpdateExistingElevatorData', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251231085159_AddLeaderToTechnician')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251231085159_AddLeaderToTechnician', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251231114755_AddStageTechnicianTable')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251231114755_AddStageTechnicianTable', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251231120042_AddRatingToStageTechnician')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251231120042_AddRatingToStageTechnician', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260101124906_AddMaintenanceChecklistItems')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260101124906_AddMaintenanceChecklistItems', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260101130246_AddProjectNumberToMaintenanceContract')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260101130246_AddProjectNumberToMaintenanceContract', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260106120555_AddEmergencyModule')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260106120555_AddEmergencyModule', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260110155623_AddTechnicianIdToMaintenance')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260110155623_AddTechnicianIdToMaintenance', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260111134825_AddCityToProjectsAndCustomers')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260111134825_AddCityToProjectsAndCustomers', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260112121737_AddProjectAddressToMaintenanceContract')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260112121737_AddProjectAddressToMaintenanceContract', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260113130245_AddCountAndPercentageToMaintenanceVisitChecklistItem')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260113130245_AddCountAndPercentageToMaintenanceVisitChecklistItem', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260113133115_AddStatusToMaintenanceVisitChecklistItem')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260113133115_AddStatusToMaintenanceVisitChecklistItem', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260113190516_AddPaymentNotesToMaintenanceVisit')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260113190516_AddPaymentNotesToMaintenanceVisit', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260115100641_AddGoogleMapsLinkToMaintenanceContract')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260115100641_AddGoogleMapsLinkToMaintenanceContract', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260116165620_AddUserIdToTechnician')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260116165620_AddUserIdToTechnician', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260118000000_AddDisplayOrderToMaintenanceVisit')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260118000000_AddDisplayOrderToMaintenanceVisit', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260121073319_AddGoogleMapsLinkToEmergencyTickt')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260121073319_AddGoogleMapsLinkToEmergencyTickt', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260330122213_MT001_AddCompanyTenantEntity')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260330122213_MT001_AddCompanyTenantEntity', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260331085236_MT002_AddCompanyIdToBusinessTables')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260331085236_MT002_AddCompanyIdToBusinessTables', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260331085846_MT003_AddCompanyIdToAppUser')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260331085846_MT003_AddCompanyIdToAppUser', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260331090425_MT004_ConfigureCompanyRelationshipsAndFks')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260331090425_MT004_ConfigureCompanyRelationshipsAndFks', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260331093010_MT005_BackfillDefaultCompanyAndRequireCompanyId')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260331093010_MT005_BackfillDefaultCompanyAndRequireCompanyId', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260331135338_DB001_TenantScopedUniqueConstraints')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260331135338_DB001_TenantScopedUniqueConstraints', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260331135759_DB002_TenantCompositeIndexes')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260331135759_DB002_TenantCompositeIndexes', @pv);
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260331144307_SUB001_AddSubscriptionEntities')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260331144307_SUB001_AddSubscriptionEntities', @pv);

/* Comment out the next block if PLAT001 is not yet applied to the schema; then run `dotnet ef database update`. */
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260403200704_PLAT001_PlatformAdminFields')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260403200704_PLAT001_PlatformAdminFields', @pv);

PRINT N'Baseline complete. Verify rows: SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;';
