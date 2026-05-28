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

    [Fact]
    public async Task BuildBlockersAsync_BlocksLiveOnlyColumnDropWhenDefaultConstraintDependsOnColumn()
    {
        await AssertDropColumnDependencyBlockerAsync(
            databaseNamePrefix: "MetaSqlFeasibilityDropDefault",
            createDependencySql: """
                CREATE TABLE dbo.Customer (
                    Id int NOT NULL,
                    LegacyCode nvarchar(50) NULL CONSTRAINT DF_Customer_LegacyCode DEFAULT (N'LEG')
                );
                """,
            expectedDependencyKind: "DEFAULT");
    }

    [Fact]
    public async Task BuildBlockersAsync_BlocksLiveOnlyColumnDropWhenCheckConstraintDependsOnColumn()
    {
        await AssertDropColumnDependencyBlockerAsync(
            databaseNamePrefix: "MetaSqlFeasibilityDropCheck",
            createDependencySql: """
                CREATE TABLE dbo.Customer (
                    Id int NOT NULL,
                    LegacyCode nvarchar(50) NULL,
                    CONSTRAINT CK_Customer_LegacyCode CHECK (LegacyCode IS NULL OR LEN(LegacyCode) > 0)
                );
                """,
            expectedDependencyKind: "CHECK");
    }

    [Fact]
    public async Task BuildBlockersAsync_BlocksLiveOnlyColumnDropWhenUniqueConstraintDependsOnColumn()
    {
        await AssertDropColumnDependencyBlockerAsync(
            databaseNamePrefix: "MetaSqlFeasibilityDropUnique",
            createDependencySql: """
                CREATE TABLE dbo.Customer (
                    Id int NOT NULL,
                    LegacyCode nvarchar(50) NULL,
                    CONSTRAINT UQ_Customer_LegacyCode UNIQUE (LegacyCode)
                );
                """,
            expectedDependencyKind: "UNIQUE");
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

    private static Workspace CreateDropColumnWorkspace(string workspacePath, bool includeLegacyCode)
    {
        var columns = new List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.ColumnRow>
        {
            new("dbo", "Customer", "Id", 1, false, "int", null, null, null),
        };
        if (includeLegacyCode)
        {
            columns.Add(new("dbo", "Customer", "LegacyCode", 2, true, "nvarchar", 50, null, null));
        }

        return MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.Project(
            newWorkspacePath: workspacePath,
            databaseName: "SalesDb",
            tableRows:
            [
                new MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.TableRow("dbo", "Customer")
            ],
            columnsByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Customer"] = columns,
            },
            primaryKeysByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase),
            primaryKeyColumnsByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeysByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeyColumnsByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            indexesByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
            indexColumnsByTableKey: new Dictionary<string, List<MetaSql.Extractors.SqlServer.SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));
    }

    private static async Task AssertDropColumnDependencyBlockerAsync(
        string databaseNamePrefix,
        string createDependencySql,
        string expectedDependencyKind)
    {
        var databaseName = $"{databaseNamePrefix}_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var sourcePath = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), "source");
        var livePath = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), "live");

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, createDependencySql);

            var sourceWorkspace = CreateDropColumnWorkspace(sourcePath, includeLegacyCode: false);
            var liveWorkspace = CreateDropColumnWorkspace(livePath, includeLegacyCode: true);
            var differences = new MetaSqlDifferenceService().BuildDifferences(sourceWorkspace, liveWorkspace);

            var blockers = await new MetaSqlDifferenceFeasibilityService()
                .BuildBlockersAsync(differences, sourceWorkspace, liveWorkspace, databaseConnectionString);

            var blocker = Assert.Single(blockers);
            Assert.Equal(MetaSqlDifferenceKind.ExtraInLive, blocker.Difference.DifferenceKind);
            Assert.Contains("DROP COLUMN is blocked by live", blocker.Reason, StringComparison.Ordinal);
            Assert.Contains(expectedDependencyKind, blocker.Reason, StringComparison.Ordinal);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
        }
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
