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
        const string matchCheck = "JoinMultiplicityExplosion.JoinPattern.ProductOrderLine.1";
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
                CREATE TABLE dbo.Product (ProductId int NOT NULL);
                CREATE TABLE dbo.OrderLine (ProductId int NOT NULL);
                INSERT dbo.Customer VALUES (1, 1), (2, 2);
                INSERT dbo.[Order] VALUES (1), (3);
                INSERT dbo.CustomerRegion VALUES (1), (3);
                INSERT dbo.Product VALUES (1), (1);
                INSERT dbo.OrderLine VALUES (1);
                """);

            var model = DataQualityToSqlWeaveWitnessTests.CreateWitnessModel();
            foreach (var candidate in model.DataQualityCandidateList)
                if (candidate.Id is not (runtimeCheck or analysisCheck or matchCheck))
                    candidate.Status = CandidateStatuses.Discovered;
            var workspace = Path.Combine(root, "Source");
            TypedWorkspaceModelMapper.Create(model, MetaDataQualityModels.LoadContract(), new MetaDataQualityWorkspaceBinding(), workspace, "xml");
            var deploymentPath = Path.Combine(root, "Deploy.sql");
            var artifact = new DataQualityToSqlConverter().Convert(workspace, deploymentPath);
            Assert.Equal(3, artifact.CandidateViewCount);
            var deployment = (await File.ReadAllTextAsync(deploymentPath))
                .Replace("[MetaDQ]", $"[{logDatabase}]", StringComparison.Ordinal)
                .Replace("N'MetaDQ'", $"N'{logDatabase}'", StringComparison.Ordinal);
            foreach (var batch in Regex.Split(deployment, @"(?m)^GO\s*$"))
                if (!string.IsNullOrWhiteSpace(batch)) await Execute(source, batch);

            // Use a connection to a different database to exercise qualified
            // dashboard queries as well as queries from the deployment database.
            var dashboard = $"[{sourceDatabase}].[dq].[v_DataQualityReview]";
            var before = await Read(master, dashboard);
            Assert.Equal(3, before.Count);
            Assert.All(before, row =>
            {
                Assert.Equal("Not checked", row.Outcome);
                Assert.Null(row.Investigate);
            });

            var firstRun = await RunReview(source, "EXEC dq.Run;");
            var first = await Read(master, dashboard);
            Assert.Equal(firstRun.OrderBy(x => x.Investigate), first.OrderBy(x => x.Investigate));
            var finding = Assert.Single(first, x => x.Outcome == "Review affected rows" && x.Check != "Join may repeat rows");
            var pattern = Assert.Single(first, x => x.Outcome == "Review transform pattern");
            var matchFinding = Assert.Single(first, x => x.Check == "Join may repeat rows");

            var explanation = await Explain(master, finding);
            Assert.Contains("2 rows on the checked side", explanation.Notes["What the run measured"], StringComparison.Ordinal);
            Assert.Contains("checked table", explanation.Notes["What to do"], StringComparison.Ordinal);
            Assert.Equal(new[] { "Relationship", "Checked table", "Join key", "Rows counted" }, explanation.Columns);
            Assert.Equal(new long[] { 1, 1 }, explanation.Evidence.Select(row => (long)row[3]));
            Assert.Contains(explanation.Evidence, row => (string)row[1] == "dbo.Order" && (string)row[2] == "CustomerId=3");

            var patternExplanation = await Explain(master, pattern);
            Assert.Equal(new[] { "Relationship", "Transforms", "Common condition", "Different condition" }, patternExplanation.Columns);
            Assert.NotEmpty(patternExplanation.Evidence);
            Assert.All(patternExplanation.Evidence, row => Assert.NotEqual(row[2], row[3]));

            var reused = await Explain(master, matchFinding);
            Assert.Contains("1 join-key groups", reused.Notes["What the run measured"], StringComparison.Ordinal);
            Assert.Contains("2 input rows matched; each had exactly one match", reused.Notes["What the data shows now"], StringComparison.Ordinal);
            Assert.Empty(reused.Evidence);
            await Execute(source, "INSERT dbo.OrderLine VALUES (1);");
            var multiplied = await Explain(master, matchFinding);
            Assert.Equal(reused.Notes["What the run measured"], multiplied.Notes["What the run measured"]);
            Assert.Contains("2 input rows matched more than one row", multiplied.Notes["What the data shows now"], StringComparison.Ordinal);
            Assert.Equal(new[] { "ProductId", "MatchesPerInputRow", "AffectedInputRows" }, multiplied.Columns);
            Assert.Equal(new object[] { 1, 2L, 2L }, Assert.Single(multiplied.Evidence));

            await Execute(source, "INSERT dbo.Customer VALUES (3, 3); INSERT dbo.[Order] VALUES (2); INSERT dbo.CustomerRegion VALUES (2); DELETE dbo.OrderLine;");
            Assert.Contains(finding, await Read(master, dashboard));
            var secondRun = await RunReview(source, "EXEC dq.Run;");
            var latestPattern = Assert.Single(secondRun);
            Assert.Equal(pattern.Check, latestPattern.Check);
            Assert.Equal(pattern.Outcome, latestPattern.Outcome);
            Assert.NotEqual(pattern.Investigate, latestPattern.Investigate);
            var second = await Read(master, dashboard);
            var clean = Assert.Single(second, x => x.Check == finding.Check);
            Assert.Equal("No rows found", clean.Outcome);
            var historical = await Explain(master, finding);
            Assert.Equal(explanation.Notes["Saved result"], historical.Notes["Saved result"]);
            Assert.Contains("returns zero evidence rows on current data", historical.Notes["What the data shows now"], StringComparison.Ordinal);
            Assert.Empty(historical.Evidence);

            var includeClean = $"""
                DECLARE @RunId bigint;
                EXEC [{sourceDatabase}].dq.Run @IncludeClean = 1, @RunId = @RunId OUTPUT;
                SELECT @RunId;
                """;
            var allResults = await RunReview(master, includeClean, verifyOutputId: true);
            Assert.Equal(3, allResults.Count);
            Assert.Contains(allResults, row => row.Check == clean.Check && row.Outcome == clean.Outcome);

            var savedClean = (await Read(master, dashboard)).Single(x => x.Check == finding.Check);
            await Execute(source, $"DROP VIEW [dq].[r_{runtimeCheck}];");
            Assert.Contains(savedClean, await Read(master, dashboard));
            var failureRun = await RunReview(source, "EXEC dq.Run;");
            var third = await Read(master, dashboard);
            var failure = Assert.Single(failureRun, x => x.Outcome == "Check failed");
            Assert.Contains(failure, third);
            Assert.Contains(third, row => row.Check == pattern.Check && row.Outcome == pattern.Outcome);
            var failureExplanation = await Explain(master, failure);
            Assert.Contains($"r_{runtimeCheck}", failureExplanation.Notes["SQL error"], StringComparison.Ordinal);
            Assert.Empty(failureExplanation.Evidence);
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

    private static async Task<List<ReviewRow>> RunReview(
        SqlConnection connection, string sql, bool verifyOutputId = false)
    {
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 60 };
        await using var reader = await command.ExecuteReaderAsync();
        var review = await ReadRows(reader);
        Assert.Equal(review.Select(row => row.Transform).Order(StringComparer.OrdinalIgnoreCase),
            review.Select(row => row.Transform));
        if (verifyOutputId)
        {
            Assert.True(await reader.NextResultAsync());
            Assert.True(await reader.ReadAsync());
            var runId = reader.GetInt64(0);
            Assert.True(runId > 0);
            Assert.All(review, row => Assert.EndsWith($"@RunId = {runId};", row.Investigate, StringComparison.Ordinal));
            Assert.False(await reader.ReadAsync());
        }
        Assert.False(await reader.NextResultAsync());
        return review;
    }

    private static async Task<List<ReviewRow>> Read(SqlConnection connection, string qualifiedView)
    {
        await using var command = new SqlCommand(
            $"SELECT * FROM {qualifiedView};",
            connection) { CommandTimeout = 30 };
        await using var reader = await command.ExecuteReaderAsync();
        return await ReadRows(reader);
    }

    private static async Task<List<ReviewRow>> ReadRows(SqlDataReader reader)
    {
        string[] columns = ["Transform", "Check", "Outcome", "Investigate"];
        Assert.Equal(columns, Enumerable.Range(0, reader.FieldCount).Select(reader.GetName));
        var results = new List<ReviewRow>();
        while (await reader.ReadAsync())
            results.Add(new ReviewRow(
                reader.IsDBNull(0) ? null : reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3)));
        return results;
    }

    private static async Task<(Dictionary<string, string> Notes, string[] Columns, List<object[]> Evidence)> Explain(
        SqlConnection connection, ReviewRow row)
    {
        Assert.NotNull(row.Investigate);
        await using var command = new SqlCommand(row.Investigate, connection);
        await using var reader = await command.ExecuteReaderAsync();
        var notes = new Dictionary<string, string>(StringComparer.Ordinal);
        while (await reader.ReadAsync()) notes.Add(reader.GetString(0), reader.GetString(1));
        Assert.Equal(row.Check, notes["Check"]);
        Assert.Equal(row.Outcome, notes["Saved result"]);
        string[] columns = [];
        var evidence = new List<object[]>();
        if (await reader.NextResultAsync())
        {
            columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToArray();
            while (await reader.ReadAsync())
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                evidence.Add(values);
            }
            Assert.False(await reader.NextResultAsync());
        }
        return (notes, columns, evidence);
    }

    private sealed record ReviewRow(string? Transform, string Check, string Outcome, string? Investigate);
}
