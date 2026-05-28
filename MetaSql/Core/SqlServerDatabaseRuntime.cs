using Microsoft.Data.SqlClient;

namespace MetaSql;

public enum MetaSqlLiveDatabasePresence
{
    Present = 0,
    Missing = 1,
}

public static class SqlServerDatabaseRuntime
{
    public static string RequireDatabaseName(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog?.Trim();
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException(
                "connection string must include Database or Initial Catalog for MetaSql deploy/deploy-plan.");
        }

        return databaseName;
    }

    public static async Task<MetaSqlLiveDatabasePresence> GetPresenceAsync(
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        var databaseName = RequireDatabaseName(connectionString);
        var masterConnectionString = BuildMasterConnectionString(connectionString);

        await using var connection = new SqlConnection(masterConnectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT CASE WHEN DB_ID(@databaseName) IS NULL THEN 0 ELSE 1 END;";
        command.Parameters.Add(new SqlParameter("@databaseName", databaseName));
        var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result is int value && value == 1
            ? MetaSqlLiveDatabasePresence.Present
            : MetaSqlLiveDatabasePresence.Missing;
    }

    public static async Task CreateDatabaseAsync(
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        var databaseName = RequireDatabaseName(connectionString);
        var masterConnectionString = BuildMasterConnectionString(connectionString);

        await using var connection = new SqlConnection(masterConnectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE {QuoteIdentifier(databaseName)};";
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static string BuildMasterConnectionString(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = "master",
        };
        return builder.ConnectionString;
    }

    private static string QuoteIdentifier(string value)
    {
        return "[" + value.Replace("]", "]]", StringComparison.Ordinal) + "]";
    }
}
