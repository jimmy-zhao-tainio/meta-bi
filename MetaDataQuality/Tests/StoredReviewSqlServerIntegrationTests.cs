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
        const string outerCheck = "OuterJoinNullExpansion.JoinPattern.OrderShipment.1";
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
                CREATE TABLE dbo.[Order] (CustomerId int NOT NULL, OrderId binary(4) NOT NULL);
                CREATE TABLE dbo.Shipment (OrderId binary(4) NOT NULL);
                CREATE TABLE dbo.CustomerRegion (RegionId int NOT NULL);
                CREATE TABLE dbo.Product (ProductId int NOT NULL);
                CREATE TABLE dbo.OrderLine (ProductId int NOT NULL);
                INSERT dbo.Customer VALUES (1, 1), (2, 2);
                INSERT dbo.[Order] VALUES (1, 0xA1B2C3D4), (3, 0x0102FDFE);
                INSERT dbo.Shipment VALUES (0xA1B2C3D4);
                INSERT dbo.CustomerRegion VALUES (1), (3);
                INSERT dbo.Product VALUES (1), (1);
                INSERT dbo.OrderLine VALUES (1);
                """);

            var model = DataQualityToSqlWeaveWitnessTests.CreateWitnessModel();
            var shipmentJoin = model.JoinPatternList.Single(row => row.Id == "JoinPattern.OrderShipment");
            shipmentJoin.EqualityPredicateCount = "2";
            model.JoinPatternKeyPartList.Add(new global::MetaDataQuality.JoinPatternKeyPart
            {
                Id = "JoinPattern.OrderShipment.RankFilter",
                JoinPattern = shipmentJoin,
                Ordinal = "1",
                BooleanComparisonExpressionId = "comparison.ShipmentRank",
                FirstExpressionId = "scalar.ShipmentRank",
                FirstExpressionDisplay = "shipment.VersionRank",
                SecondExpressionId = "scalar.One",
            });
            foreach (var candidate in model.DataQualityCandidateList)
                if (candidate.Id is not (runtimeCheck or analysisCheck or matchCheck or outerCheck))
                    candidate.Status = CandidateStatuses.Discovered;
            var workspace = Path.Combine(root, "Source");
            TypedWorkspaceModelMapper.Create(model, MetaDataQualityModels.LoadContract(), new MetaDataQualityWorkspaceBinding(), workspace, "xml");
            var deploymentPath = Path.Combine(root, "Deploy.sql");
            var artifact = new DataQualityToSqlConverter().Convert(workspace, deploymentPath);
            Assert.Equal(4, artifact.CandidateViewCount);
            var deployment = (await File.ReadAllTextAsync(deploymentPath))
                .Replace("[MetaDQ]", $"[{logDatabase}]", StringComparison.Ordinal)
                .Replace("N'MetaDQ'", $"N'{logDatabase}'", StringComparison.Ordinal);
            foreach (var batch in Regex.Split(deployment, @"(?m)^GO\s*$"))
                if (!string.IsNullOrWhiteSpace(batch)) await Execute(source, batch);

            // Use a connection to a different database to exercise qualified
            // dashboard queries as well as queries from the deployment database.
            var dashboard = $"[{sourceDatabase}].[dq].[v_DataQualityReview]";
            var before = await Read(master, dashboard);
            Assert.Equal(4, before.Select(row => row.Check).Distinct().Count());
            Assert.All(before, row =>
            {
                Assert.Equal("Not checked", row.Outcome);
                Assert.Null(row.Investigate);
            });

            string customerCommand;
            await using (var picker = new SqlCommand($"EXEC [{sourceDatabase}].dq.Run;", master))
            await using (var reader = await picker.ExecuteReaderAsync())
            {
                Assert.Equal(new[] { "Transform", "Run checks" }, Enumerable.Range(0, reader.FieldCount).Select(reader.GetName));
                var choices = new Dictionary<string, string>();
                while (await reader.ReadAsync()) choices.Add(reader.GetString(0), reader.GetString(1));
                Assert.Equal(4, choices.Count);
                customerCommand = choices["dbo.v_customer_orders"];
                Assert.False(await reader.NextResultAsync());
            }
            await using (var runs = new SqlCommand($"SELECT COUNT(*) FROM [{logDatabase}].dbo.RunLog", master))
                Assert.Equal(0, await runs.ExecuteScalarAsync());
            await Assert.ThrowsAsync<SqlException>(() => Execute(source, "EXEC dq.Run @Transform = N'unknown test transform';"));

            var firstRun = await RunReview(source, "EXEC dq.Run @All = 1;");
            var first = await Read(master, dashboard);
            Assert.Equal(firstRun.Select(x => x.Investigate).Order(), first.Select(x => x.Investigate).Distinct().Order());
            var finding = Assert.Single(firstRun, x => x.Check == "Missing referenced rows");
            var pattern = Assert.Single(firstRun, x => x.Outcome == "Review transform pattern");
            var matchFinding = Assert.Single(firstRun, x => x.Check == "Join may repeat rows");
            var outerFinding = Assert.Single(firstRun, x => x.Check == "Missing matches in outer joins");

            var explanation = await Explain(master, finding);
            Assert.Contains("2 rows on the checked side", explanation.Notes["Saved result"], StringComparison.Ordinal);
            Assert.Equal(2, explanation.Grids.Count);
            var orders = Assert.Single(explanation.Grids, grid => grid.Columns.Contains("Order.CustomerId"));
            Assert.Equal(new[] { "Customer.CustomerId", "Order.CustomerId" }, orders.Columns);
            Assert.Equal(new object[] { DBNull.Value, 3 }, Assert.Single(orders.Rows));

            var outer = await Explain(master, outerFinding);
            Assert.Contains("1 preserved rows had no match", outer.Notes["Saved result"], StringComparison.Ordinal);
            var shipments = Assert.Single(outer.Grids);
            Assert.Equal(new[] { "Order.OrderId", "Shipment.OrderId" }, shipments.Columns);
            var gap = Assert.Single(shipments.Rows);
            Assert.Equal(new byte[] { 0x01, 0x02, 0xFD, 0xFE }, Assert.IsType<byte[]>(gap[0]));
            Assert.Equal(DBNull.Value, gap[1]);

            var scoped = Assert.Single(await RunReview(master, customerCommand));
            Assert.Equal("dbo.v_customer_orders", scoped.Transform);
            Assert.Equal(finding.Check, scoped.Check);
            var scopedExplanation = await Explain(master, scoped);
            var mark = scopedExplanation.Notes["Mark no issue"].TrimEnd(';');
            await Execute(master, mark + ", @Reason = N'Kopplingen saknas avsiktligt.';");
            await Execute(master, mark + ", @Reason = N'Retry should preserve the first reason';");
            var reviewed = await Explain(master, scoped);
            Assert.StartsWith("No issue (", reviewed.Notes["Review"], StringComparison.Ordinal);
            Assert.Equal("Kopplingen saknas avsiktligt.", reviewed.Notes["Review reason"]);
            Assert.Equal(scopedExplanation.Notes["Saved result"], reviewed.Notes["Saved result"]);
            var saved = await Read(master, dashboard);
            Assert.Equal("No issue", Assert.Single(saved, row => row.Investigate == scoped.Investigate).Outcome);
            Assert.Equal("Review affected rows", Assert.Single(saved, row =>
                row.Transform == "dbo.v_customer_regions" && row.Investigate == finding.Investigate).Outcome);
            Assert.Contains(saved, row => row.Investigate == matchFinding.Investigate);
            await using (var count = new SqlCommand($"""
                SELECT COUNT(*) FROM [{logDatabase}].dbo.RunCheck c
                JOIN [{logDatabase}].dbo.RunLog r ON r.RunId=c.RunId
                WHERE r.RunId=(SELECT MAX(RunId) FROM [{logDatabase}].dbo.RunLog);
                """, master))
                Assert.Equal(1, await count.ExecuteScalarAsync());
            var rerun = Assert.Single(await RunReview(source, "EXEC dq.Run @Transform = N'dbo.v_customer_orders';"));
            Assert.Equal("Review affected rows", rerun.Outcome);
            Assert.NotEqual(scoped.Investigate, rerun.Investigate);
            Assert.True((await Explain(master, rerun)).Notes.ContainsKey("Mark no issue"));
            Assert.Equal(reviewed.Notes["Review"], (await Explain(master, scoped)).Notes["Review"]);

            var patternExplanation = await Explain(master, pattern);
            var conditions = Assert.Single(patternExplanation.Grids);
            Assert.Equal(new[] { "Relationship", "Transforms", "Common condition", "Different condition" }, conditions.Columns);
            Assert.NotEmpty(conditions.Rows);
            Assert.All(conditions.Rows, row => Assert.NotEqual(row[2], row[3]));

            var reused = await Explain(master, matchFinding);
            Assert.Contains("1 join-key groups", reused.Notes["Saved result"], StringComparison.Ordinal);
            Assert.Contains("2 input rows matched; each had exactly one match", reused.Notes["What the data shows now"], StringComparison.Ordinal);
            Assert.Empty(reused.Grids);
            Assert.False(reused.Notes.ContainsKey("Evidence query"));
            await Execute(source, "INSERT dbo.OrderLine VALUES (1);");
            var multiplied = await Explain(master, matchFinding);
            Assert.Equal(reused.Notes["Saved result"], multiplied.Notes["Saved result"]);
            Assert.Contains("2 input rows matched more than one row", multiplied.Notes["What the data shows now"], StringComparison.Ordinal);
            var repeated = Assert.Single(multiplied.Grids);
            Assert.Equal(new[] { "Product.ProductId", "OrderLine.ProductId", "MatchesPerInputRow" }, repeated.Columns);
            Assert.Equal(new object[] { 1, 1, 2L }, Assert.Single(repeated.Rows));

            await Execute(source, "INSERT dbo.Customer VALUES (3, 3); INSERT dbo.[Order] VALUES (2, 0xA1B2C3D4); INSERT dbo.CustomerRegion VALUES (2); INSERT dbo.Shipment VALUES (0x0102FDFE); DELETE dbo.OrderLine;");
            Assert.Contains(await Read(master, dashboard), row => row.Investigate == finding.Investigate);
            var secondRun = await RunReview(source, "EXEC dq.Run @All = 1;");
            var latestPattern = Assert.Single(secondRun);
            Assert.Equal(pattern.Check, latestPattern.Check);
            Assert.Equal(pattern.Outcome, latestPattern.Outcome);
            Assert.NotEqual(pattern.Investigate, latestPattern.Investigate);
            var second = await Read(master, dashboard);
            var clean = Assert.Single(second, x => x.Check == finding.Check && x.Transform == "dbo.v_customer_orders");
            Assert.Equal("No rows found", clean.Outcome);
            var historical = await Explain(master, finding);
            Assert.Equal(explanation.Notes["Saved result"], historical.Notes["Saved result"]);
            Assert.Contains("empty grid", historical.Notes["What the data shows now"], StringComparison.Ordinal);
            Assert.All(historical.Grids, grid => Assert.Empty(grid.Rows));

            var includeClean = $"""
                DECLARE @RunId bigint;
                EXEC [{sourceDatabase}].dq.Run @All = 1, @IncludeClean = 1, @RunId = @RunId OUTPUT;
                SELECT @RunId;
                """;
            var allResults = await RunReview(master, includeClean, verifyOutputId: true);
            Assert.Equal(4, allResults.Count);
            Assert.Contains(allResults, row => row.Check == clean.Check && row.Outcome == clean.Outcome);

            var savedClean = (await Read(master, dashboard)).Single(x => x.Check == finding.Check && x.Transform == "dbo.v_customer_orders");
            await Execute(source, $"DROP VIEW [dq].[r_{runtimeCheck}];");
            Assert.Contains(savedClean, await Read(master, dashboard));
            var failureRun = await RunReview(source, "EXEC dq.Run @All = 1;");
            var third = await Read(master, dashboard);
            var failure = Assert.Single(failureRun, x => x.Outcome == "Check failed");
            Assert.Contains(third, row => row.Investigate == failure.Investigate && row.Outcome == failure.Outcome);
            Assert.Contains(third, row => row.Check == pattern.Check && row.Outcome == pattern.Outcome);
            var failureExplanation = await Explain(master, failure);
            Assert.Contains($"r_{runtimeCheck}", failureExplanation.Notes["SQL error"], StringComparison.Ordinal);
            Assert.Equal("-", failureExplanation.Notes["Saved result"]);
            Assert.Empty(failureExplanation.Grids);
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

    private static async Task<(Dictionary<string, string> Notes, List<EvidenceGrid> Grids)> Explain(
        SqlConnection connection, ReviewRow row)
    {
        Assert.NotNull(row.Investigate);
        await using var command = new SqlCommand(row.Investigate, connection);
        await using var reader = await command.ExecuteReaderAsync();
        var notes = new Dictionary<string, string>(StringComparer.Ordinal);
        while (await reader.ReadAsync()) notes.Add(reader.GetString(0), reader.GetString(1));
        Assert.Equal(row.Check, notes["Check"]);
        var evidence = new List<EvidenceGrid>();
        while (await reader.NextResultAsync())
            evidence.Add(await ReadEvidenceGrid(reader));

        if (notes.TryGetValue("Evidence query", out var query))
        {
            // The query shown in the first grid can be copied and run as-is from master.
            await using var replay = new SqlConnection(connection.ConnectionString);
            await replay.OpenAsync();
            await using var replayCommand = new SqlCommand(query, replay);
            await using var replayReader = await replayCommand.ExecuteReaderAsync();
            foreach (var grid in evidence)
            {
                var reproduced = await ReadEvidenceGrid(replayReader);
                Assert.Equal(grid.Columns, reproduced.Columns);
                Assert.Equal(grid.Rows, reproduced.Rows);
                if (grid != evidence[^1]) Assert.True(await replayReader.NextResultAsync());
            }
            Assert.False(await replayReader.NextResultAsync());
        }
        return (notes, evidence);
    }

    private static async Task<EvidenceGrid> ReadEvidenceGrid(SqlDataReader reader)
    {
        var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToArray();
        var rows = new List<object[]>();
        while (await reader.ReadAsync())
        {
            var values = new object[reader.FieldCount];
            reader.GetValues(values);
            rows.Add(values);
        }
        return new EvidenceGrid(columns, rows);
    }

    private sealed record EvidenceGrid(string[] Columns, List<object[]> Rows);
    private sealed record ReviewRow(string? Transform, string Check, string Outcome, string? Investigate);
}
