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

            var firstRun = await RunReview(source, "EXEC dq.Run;", expectedFailures: 0);
            var first = await Read(master, dashboard);
            Assert.Equal(firstRun[runtimeCheck], first[runtimeCheck]);
            Assert.Equal(firstRun[analysisCheck], first[analysisCheck]);
            Assert.Equal("Review affected rows", first[runtimeCheck].Outcome);
            Assert.Equal(2L, first[runtimeCheck].Groups);
            Assert.Equal(2L, first[runtimeCheck].Rows);
            Assert.Equal($"SELECT * FROM [{sourceDatabase}].[dq].[v_{runtimeCheck}];", first[runtimeCheck].Inspect);
            Assert.Equal("Review transform pattern", first[analysisCheck].Outcome);
            Assert.Null(first[analysisCheck].Rows);
            Assert.Null(first[analysisCheck].Groups);

            await Execute(source, "INSERT dbo.Customer VALUES (3, 3); INSERT dbo.[Order] VALUES (2); INSERT dbo.CustomerRegion VALUES (2);");
            Assert.Equal(first[runtimeCheck], (await Read(master, dashboard))[runtimeCheck]);
            var secondRun = await RunReview(source, "EXEC dq.Run;", expectedFailures: 0);
            Assert.Equal(analysisCheck, Assert.Single(secondRun).Key);
            var second = await Read(master, dashboard);
            Assert.Equal(secondRun[analysisCheck], second[analysisCheck]);
            Assert.Equal("No rows found", second[runtimeCheck].Outcome);
            Assert.Equal(0L, second[runtimeCheck].Groups);
            Assert.Equal(0L, second[runtimeCheck].Rows);
            Assert.NotEqual(first[runtimeCheck].RunId, second[runtimeCheck].RunId);

            var includeClean = $"""
                DECLARE @RunId bigint;
                EXEC [{sourceDatabase}].dq.Run @IncludeClean = 1, @RunId = @RunId OUTPUT;
                SELECT @RunId;
                """;
            var allResults = await RunReview(master, includeClean, expectedFailures: 0, verifyOutputId: true);
            Assert.Equal(2, allResults.Count);
            Assert.Equal("No rows found", allResults[runtimeCheck].Outcome);

            await Execute(source, $"DROP VIEW [dq].[r_{runtimeCheck}];");
            Assert.Equal(allResults[runtimeCheck], (await Read(master, dashboard))[runtimeCheck]);
            var failureRun = await RunReview(source, "EXEC dq.Run;", expectedFailures: 1);
            var third = await Read(master, dashboard);
            Assert.Equal(failureRun[runtimeCheck], third[runtimeCheck]);
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

    private static async Task<Dictionary<string, ReviewRow>> RunReview(
        SqlConnection connection, string sql, long expectedFailures, bool verifyOutputId = false)
    {
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 60 };
        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        var runId = reader.GetInt64(reader.GetOrdinal("RunId"));
        Assert.Equal(expectedFailures == 0 ? "Completed" : "CompletedWithErrors", reader.GetString(reader.GetOrdinal("Status")));
        Assert.Equal(2L, reader.GetInt64(reader.GetOrdinal("Checks")));
        Assert.Equal(expectedFailures, reader.GetInt64(reader.GetOrdinal("ChecksFailed")));
        Assert.False(await reader.ReadAsync());

        Assert.True(await reader.NextResultAsync());
        Assert.Equal("Check", reader.GetName(0));
        var review = await ReadRows(reader);
        Assert.All(review.Values, row => Assert.Equal(runId, row.RunId));
        if (verifyOutputId)
        {
            Assert.True(await reader.NextResultAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(runId, reader.GetInt64(0));
        }
        Assert.False(await reader.NextResultAsync());
        return review;
    }

    private static async Task<Dictionary<string, ReviewRow>> Read(SqlConnection connection, string qualifiedView)
    {
        await using var command = new SqlCommand(
            $"SELECT CheckId, Outcome, FindingGroups, SuspectRows, RunId, RunStatus, Explanation, InspectCurrentRows FROM {qualifiedView};",
            connection) { CommandTimeout = 30 };
        await using var reader = await command.ExecuteReaderAsync();
        return await ReadRows(reader);
    }

    private static async Task<Dictionary<string, ReviewRow>> ReadRows(SqlDataReader reader)
    {
        var checkId = reader.GetOrdinal("CheckId");
        var outcome = reader.GetOrdinal("Outcome");
        var groups = reader.GetOrdinal("FindingGroups");
        var rows = reader.GetOrdinal("SuspectRows");
        var runId = reader.GetOrdinal("RunId");
        var runStatus = reader.GetOrdinal("RunStatus");
        var explanation = reader.GetOrdinal("Explanation");
        var inspect = reader.GetOrdinal("InspectCurrentRows");
        var results = new Dictionary<string, ReviewRow>(StringComparer.Ordinal);
        while (await reader.ReadAsync())
            results.Add(reader.GetString(checkId), new ReviewRow(
                reader.GetString(outcome),
                reader.IsDBNull(groups) ? null : reader.GetInt64(groups),
                reader.IsDBNull(rows) ? null : reader.GetInt64(rows),
                reader.IsDBNull(runId) ? null : reader.GetInt64(runId),
                reader.IsDBNull(runStatus) ? null : reader.GetString(runStatus),
                reader.IsDBNull(explanation) ? null : reader.GetString(explanation),
                reader.GetString(inspect)));
        return results;
    }

    private sealed record ReviewRow(
        string Outcome, long? Groups, long? Rows, long? RunId, string? RunStatus, string? Explanation, string Inspect);
}
