using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace LiftOps_BackEnd.Infrastructure.Persistence;

/// <summary>
/// Creates the target database on the server if it is missing so startup (baseline + migrate) can connect.
/// Connects to <c>master</c>; requires the login to have <c>CREATE DATABASE</c> rights (e.g. dbcreator/sysadmin on local dev).
/// </summary>
public static class SqlServerDatabaseEnsurer
{
    /// <summary>
    /// No-op if the string is not a usable SQL Server connection string or the catalog name is unsafe.
    /// </summary>
    public static async Task EnsureExistsAsync(
        string? connectionString,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        SqlConnectionStringBuilder builder;
        try
        {
            builder = new SqlConnectionStringBuilder(connectionString);
        }
        catch
        {
            return;
        }

        var databaseName = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            return;
        }

        if (!IsSafeSqlIdentifier(databaseName))
        {
            logger.LogWarning(
                "Skipping automatic database creation: database name must be letters, digits, or underscore only ({Name}).",
                databaseName);
            return;
        }

        builder.InitialCatalog = "master";

        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText =
            """
            IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = @name)
            BEGIN
                DECLARE @sql nvarchar(max) = N'CREATE DATABASE ' + QUOTENAME(@name);
                EXEC sp_executesql @sql;
            END
            """;
        cmd.Parameters.AddWithValue("@name", databaseName);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
        logger.LogInformation("Verified SQL Server database {Database} exists.", databaseName);
    }

    private static bool IsSafeSqlIdentifier(string name) =>
        name.Length is > 0 and <= 128
        && name.All(c => char.IsAsciiLetterOrDigit(c) || c == '_');
}
