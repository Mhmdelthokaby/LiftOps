using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LiftOps_BackEnd.Infrastructure.Persistence;

/// <summary>
/// When a database was created or restored without <c>__EFMigrationsHistory</c> rows, EF tries to re-apply
/// the first migration and fails if Identity tables already exist. This helper inserts history rows to match reality.
/// </summary>
public static class EfMigrationHistoryBaseline
{
    public const string ProductVersion = "10.0.1";

    /// <summary>All migrations in chronological order (must match Migrations folder).</summary>
    public static readonly IReadOnlyList<string> MigrationIds = new[]
    {
        "20251224182112_AddInventoryItemTable",
        "20251224184151_AddCategoryEntity",
        "20251225211623_AddInstallationModule",
        "20251225220848_AddTechnicianModule",
        "20251225223758_AddMaintenanceAndFaultsModules",
        "20251225224404_FixMaintenanceContextAndNotification",
        "20251227133019_AddItemNumberToInventoryItem",
        "20251227204705_AddStagePriceAndCollectionFields",
        "20251228043712_AddProjectNumberToInstallationProject",
        "20251228050313_AddUniqueIndexToProjectNumber",
        "20251228190348_AddProjectAddressToInstallationProject",
        "20251229183341_AddInspectionRequestsAndOffersTables",
        "20251229193512_AddClientIdToInspectionRequest",
        "20251229193859_FixInspectionRequestClientForeignKey",
        "20251230073300_AddInspectionQuotationFlow",
        "20251230085051_AddElevatorDetailsFields",
        "20251230085514_UpdateExistingElevatorData",
        "20251231085159_AddLeaderToTechnician",
        "20251231114755_AddStageTechnicianTable",
        "20251231120042_AddRatingToStageTechnician",
        "20260101124906_AddMaintenanceChecklistItems",
        "20260101130246_AddProjectNumberToMaintenanceContract",
        "20260106120555_AddEmergencyModule",
        "20260110155623_AddTechnicianIdToMaintenance",
        "20260111134825_AddCityToProjectsAndCustomers",
        "20260112121737_AddProjectAddressToMaintenanceContract",
        "20260113130245_AddCountAndPercentageToMaintenanceVisitChecklistItem",
        "20260113133115_AddStatusToMaintenanceVisitChecklistItem",
        "20260113190516_AddPaymentNotesToMaintenanceVisit",
        "20260115100641_AddGoogleMapsLinkToMaintenanceContract",
        "20260116165620_AddUserIdToTechnician",
        "20260118000000_AddDisplayOrderToMaintenanceVisit",
        "20260121073319_AddGoogleMapsLinkToEmergencyTickt",
        "20260330122213_MT001_AddCompanyTenantEntity",
        "20260331085236_MT002_AddCompanyIdToBusinessTables",
        "20260331085846_MT003_AddCompanyIdToAppUser",
        "20260331090425_MT004_ConfigureCompanyRelationshipsAndFks",
        "20260331093010_MT005_BackfillDefaultCompanyAndRequireCompanyId",
        "20260331135338_DB001_TenantScopedUniqueConstraints",
        "20260331135759_DB002_TenantCompositeIndexes",
        "20260331144307_SUB001_AddSubscriptionEntities",
        "20260403200704_PLAT001_PlatformAdminFields"
    };

    public static async Task TryBaselineAsync(
        ApplicationDbContext db,
        bool enabled,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        if (!enabled)
        {
            return;
        }

        var connection = db.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);
        try
        {
            if (!await SqlServerObjectExistsAsync(connection,
                    "SELECT 1 FROM sys.tables WHERE name = N'AspNetRoles'", cancellationToken))
            {
                return;
            }

            await db.Database.ExecuteSqlRawAsync(
                """
                IF OBJECT_ID(N'[__EFMigrationsHistory]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[__EFMigrationsHistory] (
                        [MigrationId] nvarchar(150) NOT NULL,
                        [ProductVersion] nvarchar(32) NOT NULL,
                        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId])
                    );
                END
                """,
                cancellationToken: cancellationToken);

            var historyCount = await ScalarIntAsync(connection,
                "SELECT COUNT(*) FROM [__EFMigrationsHistory]", cancellationToken);
            if (historyCount > 0)
            {
                return;
            }

            var hasPlat001Schema = await SqlServerObjectExistsAsync(connection,
                """
                SELECT 1 FROM sys.columns c
                INNER JOIN sys.tables t ON c.object_id = t.object_id
                WHERE t.name = N'Companies' AND c.name = N'TenantStatus'
                """,
                cancellationToken);

            var ids = hasPlat001Schema
                ? MigrationIds
                : MigrationIds.Where(id => !id.Contains("PLAT001", StringComparison.Ordinal)).ToList();

            foreach (var id in ids)
            {
                await db.Database.ExecuteSqlRawAsync(
                    """
                    IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = {0})
                    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ({0}, {1})
                    """,
                    new object[] { id, ProductVersion },
                    cancellationToken);
            }

            logger.LogWarning(
                "Database:AutoBaselineMigrationHistory is enabled: inserted {Count} rows into __EFMigrationsHistory " +
                "because AspNetRoles existed but history was empty. PLAT001 marked applied: {PlatIncluded}.",
                ids.Count,
                hasPlat001Schema);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private static async Task<bool> SqlServerObjectExistsAsync(
        System.Data.Common.DbConnection connection,
        string sql,
        CancellationToken cancellationToken)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return result != null && result != DBNull.Value;
    }

    private static async Task<int> ScalarIntAsync(
        System.Data.Common.DbConnection connection,
        string sql,
        CancellationToken cancellationToken)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return result is int i ? i : Convert.ToInt32(result);
    }
}
