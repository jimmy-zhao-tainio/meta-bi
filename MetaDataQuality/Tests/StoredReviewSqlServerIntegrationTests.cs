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
            Assert.All(before, row => Assert.Equal("Not checked", row.Outcome));

            var firstRun = await RunReview(source, "EXEC dq.Run;", expectedFailures: 0);
            var first = await Read(master, dashboard);
            Assert.Equal(firstRun.Rows.OrderBy(x => x.CheckNo), first.OrderBy(x => x.CheckNo));
            var finding = Assert.Single(first, x => x.Outcome == "Review affected rows");
            var pattern = Assert.Single(first, x => x.Outcome == "Review transform pattern");
            Assert.Equal(2L, finding.Groups);
            Assert.Null(pattern.Groups);

            var explanation = await Explain(source, finding.CheckNo!.Value, firstRun.RunId);
            Assert.Contains("customer 42", explanation.Notes["Example"], StringComparison.Ordinal);
            Assert.Contains("which side", explanation.Notes["How to decide"], StringComparison.Ordinal);
            var inspect = explanation.Queries["Inspect up to 20 rows from this check"];
            Assert.Contains($"[{sourceDatabase}].[dq].[v_{runtimeCheck}]", inspect, StringComparison.Ordinal);
            Assert.Contains("NOT EXISTS", explanation.Queries["Confirm the issue (replace example tables and keys)"], StringComparison.Ordinal);
            await using (var detail = new SqlCommand(inspect, master))
            await using (var reader = await detail.ExecuteReaderAsync())
            {
                var counts = new List<long>();
                while (await reader.ReadAsync()) counts.Add(reader.GetInt64(reader.GetOrdinal("SuspectCount")));
                Assert.Equal(new long[] { 1, 1 }, counts);
            }
            var patternExplanation = await Explain(source, pattern.CheckNo!.Value, firstRun.RunId);
            var patternQuery = patternExplanation.Queries["Inspect up to 20 rows from this check"];
            await using (var detail = new SqlCommand(patternQuery, master))
            await using (var reader = await detail.ExecuteReaderAsync())
            {
                Assert.True(await reader.ReadAsync());
                Assert.NotEqual(reader.GetString(1), reader.GetString(2));
            }

            await Execute(source, "INSERT dbo.Customer VALUES (3, 3); INSERT dbo.[Order] VALUES (2); INSERT dbo.CustomerRegion VALUES (2);");
            Assert.Contains(finding, await Read(master, dashboard));
            var secondRun = await RunReview(source, "EXEC dq.Run;", expectedFailures: 0);
            Assert.Equal(pattern, Assert.Single(secondRun.Rows));
            var second = await Read(master, dashboard);
            var clean = Assert.Single(second, x => x.Outcome == "No rows found");
            Assert.Equal(0L, clean.Groups);
            Assert.NotEqual(firstRun.RunId, secondRun.RunId);
            var historical = await Explain(source, finding.CheckNo.Value, firstRun.RunId);
            Assert.Equal(explanation.Notes["Saved result"], historical.Notes["Saved result"]);

            var includeClean = $"""
                DECLARE @RunId bigint;
                EXEC [{sourceDatabase}].dq.Run @IncludeClean = 1, @RunId = @RunId OUTPUT;
                SELECT @RunId;
                """;
            var allResults = await RunReview(master, includeClean, expectedFailures: 0, verifyOutputId: true);
            Assert.Equal(2, allResults.Rows.Count);
            Assert.Contains(clean, allResults.Rows);

            await Execute(source, $"DROP VIEW [dq].[r_{runtimeCheck}];");
            Assert.Contains(clean, await Read(master, dashboard));
            var failureRun = await RunReview(source, "EXEC dq.Run;", expectedFailures: 1);
            var third = await Read(master, dashboard);
            var failure = Assert.Single(failureRun.Rows, x => x.Outcome == "Check failed");
            Assert.Contains(failure, third);
            Assert.Contains(pattern, third);
            var failureExplanation = await Explain(source, failure.CheckNo!.Value);
            Assert.Contains($"r_{runtimeCheck}", failureExplanation.Notes["SQL error"], StringComparison.Ordinal);
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

    private static async Task<(long RunId, List<ReviewRow> Rows)> RunReview(
        SqlConnection connection, string sql, long expectedFailures, bool verifyOutputId = false)
    {
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 60 };
        await using var reader = await command.ExecuteReaderAsync();
        Assert.Equal(new[] { "RunId", "Status", "Checks", "ChecksFailed", "Seconds" },
            Enumerable.Range(0, reader.FieldCount).Select(reader.GetName));
        Assert.True(await reader.ReadAsync());
        var runId = reader.GetInt64(reader.GetOrdinal("RunId"));
        Assert.Equal(expectedFailures == 0 ? "Completed" : "CompletedWithErrors", reader.GetString(reader.GetOrdinal("Status")));
        Assert.Equal(2L, reader.GetInt64(reader.GetOrdinal("Checks")));
        Assert.Equal(expectedFailures, reader.GetInt64(reader.GetOrdinal("ChecksFailed")));
        Assert.False(await reader.ReadAsync());

        Assert.True(await reader.NextResultAsync());
        var review = await ReadRows(reader);
        if (verifyOutputId)
        {
            Assert.True(await reader.NextResultAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(runId, reader.GetInt64(0));
        }
        Assert.False(await reader.NextResultAsync());
        return (runId, review);
    }

    private static async Task<List<ReviewRow>> Read(SqlConnection connection, string qualifiedView)
    {
        await using var command = new SqlCommand(
            $"SELECT * FROM {qualifiedView};",
            connection) { CommandTimeout = 30 };
        await using var reader = await command.ExecuteReaderAsync();
        return await ReadRows(reader, includesRunId: true);
    }

    private static async Task<List<ReviewRow>> ReadRows(SqlDataReader reader, bool includesRunId = false)
    {
        string[] columns = includesRunId
            ? ["CheckNo", "Check", "Transform", "Groups", "Outcome", "RunId"]
            : ["CheckNo", "Check", "Transform", "Groups", "Outcome"];
        Assert.Equal(columns, Enumerable.Range(0, reader.FieldCount).Select(reader.GetName));
        var results = new List<ReviewRow>();
        while (await reader.ReadAsync())
            results.Add(new ReviewRow(
                reader.IsDBNull(0) ? null : reader.GetInt64(0),
                reader.IsDBNull(3) ? null : reader.GetInt64(3),
                reader.GetString(4)));
        return results;
    }

    private static async Task<(Dictionary<string, string> Notes, Dictionary<string, string> Queries)> Explain(
        SqlConnection connection, long checkNo, long? runId = null)
    {
        await using var command = new SqlCommand("dq.Explain", connection) { CommandType = System.Data.CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@CheckNo", checkNo);
        if (runId.HasValue) command.Parameters.AddWithValue("@RunId", runId.Value);
        await using var reader = await command.ExecuteReaderAsync();
        var notes = new Dictionary<string, string>(StringComparer.Ordinal);
        while (await reader.ReadAsync()) notes.Add(reader.GetString(0), reader.GetString(1));
        Assert.True(await reader.NextResultAsync());
        var queries = new Dictionary<string, string>(StringComparer.Ordinal);
        while (await reader.ReadAsync()) queries.Add(reader.GetString(0), reader.GetString(1));
        Assert.False(await reader.NextResultAsync());
        return (notes, queries);
    }

    private sealed record ReviewRow(long? CheckNo, long? Groups, string Outcome);
}
