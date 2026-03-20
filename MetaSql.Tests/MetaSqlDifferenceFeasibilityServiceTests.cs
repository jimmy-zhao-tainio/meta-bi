using Microsoft.Data.SqlClient;
using Meta.Core.Domain;

namespace MetaSql.Tests;

public sealed class MetaSqlDifferenceFeasibilityServiceTests
{
    [Fact]
    public async Task BuildBlockersAsync_BlocksLengthNarrowingWhenLiveDataExceedsNewLength()
    {
        var databaseName = $"MetaSqlFeasibilityLen_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var sourcePath = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), "source");
        var livePath = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), "live");

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                CREATE TABLE dbo.Customer (
                    Name nvarchar(200) NOT NULL
                );
                INSERT INTO dbo.Customer(Name) VALUES (REPLICATE(N'X', 150));
                """);

            var sourceWorkspace = CreateColumnWorkspace(sourcePath, "nvarchar", 100, sourceNullable: false);
            var liveWorkspace = CreateColumnWorkspace(livePath, "nvarchar", 200, sourceNullable: false);
            var differences = new MetaSqlDifferenceService().BuildDifferences(sourceWorkspace, liveWorkspace);

            var blockers = await new MetaSqlDifferenceFeasibilityService()
                .BuildBlockersAsync(differences, sourceWorkspace, liveWorkspace, databaseConnectionString);

            var blocker = Assert.Single(blockers);
            Assert.Contains("smaller than live data", blocker.Reason, StringComparison.Ordinal);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
        }
    }

    [Fact]
    public async Task BuildBlockersAsync_BlocksNullableToNotNullWhenLiveContainsNulls()
    {
        var databaseName = $"MetaSqlFeasibilityNull_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var sourcePath = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), "source");
        var livePath = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), "live");

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                CREATE TABLE dbo.Customer (
                    Name nvarchar(100) NULL
                );
                INSERT INTO dbo.Customer(Name) VALUES (NULL);
                """);

            var sourceWorkspace = CreateColumnWorkspace(sourcePath, "nvarchar", 100, sourceNullable: false);
            var liveWorkspace = CreateColumnWorkspace(livePath, "nvarchar", 100, sourceNullable: true);
            var differences = new MetaSqlDifferenceService().BuildDifferences(sourceWorkspace, liveWorkspace);

            var blockers = await new MetaSqlDifferenceFeasibilityService()
                .BuildBlockersAsync(differences, sourceWorkspace, liveWorkspace, databaseConnectionString);

            var blocker = Assert.Single(blockers);
            Assert.Contains("contains NULL values", blocker.Reason, StringComparison.Ordinal);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
        }
    }

    private static Workspace CreateColumnWorkspace(string workspacePath, string typeName, int length, bool sourceNullable)
    {
        return MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.Project(
            newWorkspacePath: workspacePath,
            databaseName: "SalesDb",
            tableRows:
            [
                new MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.TableRow("dbo", "Customer")
            ],
            columnsByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Customer"] =
                [
                    new("dbo", "Customer", "Name", 1, sourceNullable, typeName, length, null, null),
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase),
            primaryKeyColumnsByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeysByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeyColumnsByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            indexesByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
            indexColumnsByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));
    }

    private static void CreateDatabase(string masterConnectionString, string databaseName)
    {
        ExecuteSql(masterConnectionString, $"""
            IF DB_ID('{databaseName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{databaseName}];
            END;
            CREATE DATABASE [{databaseName}];
            """);
    }

    private static void DropDatabase(string masterConnectionString, string databaseName)
    {
        try
        {
            ExecuteSql(masterConnectionString, $"""
                IF DB_ID('{databaseName}') IS NOT NULL
                BEGIN
                    ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{databaseName}];
                END;
                """);
        }
        catch
        {
        }
    }

    private static void ExecuteSql(string connectionString, string sql)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}
