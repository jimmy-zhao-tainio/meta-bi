using System.Text.RegularExpressions;
using Meta.Integration;
using MetaConvert.DataQualityToSql;
using MetaDataQuality.Core;
using Microsoft.Data.SqlClient;

namespace MetaDataQuality.Tests;

[Trait("Category", "Integration")]
public sealed class StoredReviewSqlServerIntegrationTests
{
    [Fact]
    public async Task DeployedReviewReadsSavedOutcomesAndIsolatesACheckFailure()
    {
        const string runtimeCheck = "JoinOrphan.JoinPattern.CustomerOrder.1";
        const string analysisCheck = "MinorityJoinPattern.CustomerOrder";
        var suffix = Guid.NewGuid().ToString("N");
        var root = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", suffix);
        var sourceDatabase = "DqSource_" + suffix;
        var logDatabase = "DqLog_" + suffix;
        const string server = "Server=.;Integrated Security=true;TrustServerCertificate=true;Encrypt=false;Pooling=false";
        await using var master = new SqlConnection($"{server};Database=master");
        await master.OpenAsync();
        await Execute(master, $"CREATE DATABASE [{sourceDatabase}];");
        try
        {
            await using var source = new SqlConnection($"{server};Database={sourceDatabase}");
            await source.OpenAsync();
            await Execute(source, """
                CREATE TABLE dbo.Customer (CustomerId int NOT NULL, RegionId int NOT NULL);
                CREATE TABLE dbo.[Order] (CustomerId int NOT NULL);
                CREATE TABLE dbo.CustomerRegion (RegionId int NOT NULL);
                INSERT dbo.Customer VALUES (1, 1), (2, 2);
                INSERT dbo.[Order] VALUES (1), (3);
                INSERT dbo.CustomerRegion VALUES (1), (3);
                """);

            var model = DataQualityToSqlWeaveWitnessTests.CreateWitnessModel();
            foreach (var candidate in model.DataQualityCandidateList)
                if (candidate.Id is not (runtimeCheck or analysisCheck))
                    candidate.Status = CandidateStatuses.Discovered;
            var workspace = Path.Combine(root, "Source");
            TypedWorkspaceModelMapper.Create(model, MetaDataQualityModels.LoadContract(), new MetaDataQualityWorkspaceBinding(), workspace, "xml");
            var deploymentPath = Path.Combine(root, "Deploy.sql");
            var artifact = new DataQualityToSqlConverter().Convert(workspace, deploymentPath);
            Assert.Equal(2, artifact.CandidateViewCount);
            var deployment = (await File.ReadAllTextAsync(deploymentPath))
                .Replace("[MetaDQ]", $"[{logDatabase}]", StringComparison.Ordinal)
                .Replace("N'MetaDQ'", $"N'{logDatabase}'", StringComparison.Ordinal);
            foreach (var batch in Regex.Split(deployment, @"(?m)^GO\s*$"))
                if (!string.IsNullOrWhiteSpace(batch)) await Execute(source, batch);

            // Use a connection to a different database to exercise qualified
            // dashboard queries as well as queries from the deployment database.
            var dashboard = $"[{sourceDatabase}].[dq].[v_DataQualityReview]";
            var before = await Read(master, dashboard);
            Assert.Equal(2, before.Count);
            Assert.All(before.Values, row => Assert.Equal("Not checked", row.Outcome));

            var run = $"""
                DECLARE @RunId bigint;
                EXEC [{logDatabase}].dbo.Run @SourceDatabaseName = N'{sourceDatabase}', @RunId = @RunId OUTPUT;
                """;
            await Execute(master, run);
            var first = await Read(master, dashboard);
            Assert.Equal("Review affected rows", first[runtimeCheck].Outcome);
            Assert.Equal(2L, first[runtimeCheck].Groups);
            Assert.Equal(2L, first[runtimeCheck].Rows);
            Assert.Equal($"SELECT * FROM [{sourceDatabase}].[dq].[v_{runtimeCheck}];", first[runtimeCheck].Inspect);
            Assert.Equal("Review transform pattern", first[analysisCheck].Outcome);
            Assert.Null(first[analysisCheck].Rows);
            Assert.Null(first[analysisCheck].Groups);

            await Execute(source, "INSERT dbo.Customer VALUES (3, 3); INSERT dbo.[Order] VALUES (2); INSERT dbo.CustomerRegion VALUES (2);");
            Assert.Equal(first[runtimeCheck], (await Read(master, dashboard))[runtimeCheck]);
            await Execute(master, run);
            var second = await Read(master, dashboard);
            Assert.Equal("No rows found", second[runtimeCheck].Outcome);
            Assert.Equal(0L, second[runtimeCheck].Groups);
            Assert.Equal(0L, second[runtimeCheck].Rows);
            Assert.NotEqual(first[runtimeCheck].RunId, second[runtimeCheck].RunId);

            await Execute(source, $"DROP VIEW [dq].[r_{runtimeCheck}];");
            Assert.Equal(second[runtimeCheck], (await Read(master, dashboard))[runtimeCheck]);
            await Execute(master, run);
            var third = await Read(master, dashboard);
            Assert.Equal("CompletedWithErrors", third[runtimeCheck].RunStatus);
            Assert.Equal("Check failed", third[runtimeCheck].Outcome);
            Assert.Contains($"r_{runtimeCheck}", third[runtimeCheck].Explanation, StringComparison.Ordinal);
            Assert.Equal("Review transform pattern", third[analysisCheck].Outcome);
            Assert.Equal(third[runtimeCheck].RunId, third[analysisCheck].RunId);
        }
        finally
        {
            await Execute(master, $"ALTER DATABASE [{sourceDatabase}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{sourceDatabase}];");
            await Execute(master, $"IF DB_ID(N'{logDatabase}') IS NOT NULL BEGIN ALTER DATABASE [{logDatabase}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{logDatabase}]; END;");
            if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
        }
    }

    private static async Task Execute(SqlConnection connection, string sql)
    {
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 60 };
        await command.ExecuteNonQueryAsync();
    }

    private static async Task<Dictionary<string, ReviewRow>> Read(SqlConnection connection, string qualifiedView)
    {
        await using var command = new SqlCommand(
            $"SELECT CheckId, Outcome, FindingGroups, SuspectRows, RunId, RunStatus, Explanation, InspectCurrentRows FROM {qualifiedView};",
            connection) { CommandTimeout = 30 };
        await using var reader = await command.ExecuteReaderAsync();
        var results = new Dictionary<string, ReviewRow>(StringComparer.Ordinal);
        while (await reader.ReadAsync())
            results.Add(reader.GetString(0), new ReviewRow(
                reader.GetString(1),
                reader.IsDBNull(2) ? null : reader.GetInt64(2),
                reader.IsDBNull(3) ? null : reader.GetInt64(3),
                reader.IsDBNull(4) ? null : reader.GetInt64(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                reader.IsDBNull(6) ? null : reader.GetString(6),
                reader.GetString(7)));
        return results;
    }

    private sealed record ReviewRow(
        string Outcome, long? Groups, long? Rows, long? RunId, string? RunStatus, string? Explanation, string Inspect);
}
