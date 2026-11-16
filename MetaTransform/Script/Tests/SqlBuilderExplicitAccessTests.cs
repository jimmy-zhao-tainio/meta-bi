using MetaTransformScript;
using MetaTransformScript.Sql;

public sealed class SqlBuilderExplicitAccessTests
{
    [Fact]
    public async Task FileImportRestoresIdsAndCanonicalReferencesBeforeContinuingAfterFailure()
    {
        var directory = Path.Combine(Path.GetTempPath(), "sql-builder-access", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var first = Path.Combine(directory, "first.sql");
            var discarded = Path.Combine(directory, "discarded.sql");
            var third = Path.Combine(directory, "third.sql");
            await File.WriteAllTextAsync(first, "CREATE VIEW dbo.First AS SELECT CAST(N'first' AS nvarchar(10)) AS Value;");
            await File.WriteAllTextAsync(discarded, "CREATE VIEW dbo.Discarded AS SELECT CAST(N'discarded' AS nvarchar(10)) AS Value;");
            await File.WriteAllTextAsync(third, "CREATE VIEW dbo.Third AS SELECT CAST(N'Åsa' AS nvarchar(20)) AS Value;");
            var service = new MetaTransformScriptSqlService();

            // Target rejection happens after the complete discarded graph has been built and indexed.
            var result = await service.ImportSqlFilesAsync(
                [new(first, null), new(discarded, "too.many.parts.in.target"), new(third, null)],
                MetaTransformScriptModel.CreateEmpty(), null, CancellationToken.None);

            Assert.Equal(2, result.Successes.Count);
            Assert.Single(result.Failures);
            Assert.Equal(2, result.ScriptCount);
            Assert.DoesNotContain(result.Model.StringLiteralList, row => row.Literal.Value == "discarded");
            var emitted = service.ExportToSqlCode(result.Model, result.Successes[1].ScriptName);
            Assert.Contains("CAST(N'Åsa' AS nvarchar(20)) AS Value", emitted, StringComparison.Ordinal);
            AssertCanonicalGraph(result.Model);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void SequentialImportsRemapAllRowIdsWithoutBreakingReferences()
    {
        var service = new MetaTransformScriptSqlService();
        var model = MetaTransformScriptModel.CreateEmpty();
        service.ImportSqlCode(model, "CREATE VIEW dbo.First AS SELECT dbo.JoinText(N'left', N'right') AS Value;", null);
        service.ImportSqlCode(model, "CREATE VIEW dbo.Second AS SELECT dbo.JoinText(N'up', N'down') AS Value;", null);

        var definitions = service.ExportModuleDefinitions(model);

        Assert.Equal(2, definitions.Count);
        Assert.Contains(definitions, row => row.DefinitionSql.Contains("dbo.JoinText(N'left', N'right') AS Value", StringComparison.Ordinal));
        Assert.Contains(definitions, row => row.DefinitionSql.Contains("dbo.JoinText(N'up', N'down') AS Value", StringComparison.Ordinal));
        AssertCanonicalGraph(model);
    }

    [Fact]
    public void ScalarReplacementFollowsNestedEdgesAndPreservesOtherOccurrences()
    {
        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(
            "SELECT src.Raw + 1 AS Adjusted FROM dbo.Input AS src WHERE src.Raw > 0;", "dbo.Rewritten");
        var addition = Assert.Single(model.BinaryExpressionList);
        var selected = model.BinaryExpressionFirstExpressionLinkList.Single(row => ReferenceEquals(row.BinaryExpression, addition)).ScalarExpression;
        var reference = model.ColumnReferenceExpressionList.Single(row => ReferenceEquals(row.PrimaryExpression.ScalarExpression, selected));

        service.ReplaceColumnReference(model, reference, "CONVERT(int, REVERSE(src.Raw))", "Adjusted");

        var emitted = service.ExportToSqlCode(model);
        Assert.Contains("CONVERT(int, REVERSE(src.Raw)) + 1 AS Adjusted", emitted, StringComparison.Ordinal);
        Assert.Contains("WHERE src.Raw > 0", emitted, StringComparison.Ordinal);
        AssertCanonicalGraph(model);
    }

    [Fact]
    public void ScalarReplacementAddsMissingAliasAndStarExpansionPreservesProjectionOrder()
    {
        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode("SELECT src.Raw, src.*, 9 AS Tail FROM dbo.Input AS src;", "dbo.Expanded");
        var original = Assert.Single(model.ColumnReferenceExpressionList);
        service.ReplaceColumnReference(model, original, "REVERSE(src.Raw)", "Converted Text");
        var star = Assert.Single(model.SelectStarExpressionList);

        service.ExpandSelectStar(model, star.Id, [new[] { "src", "B" }, new[] { "src", "A" }]);

        var emitted = service.ExportToSqlCode(model);
        var expected = service.ExportToSqlCode(service.ImportFromSqlCode(
            "SELECT REVERSE(src.Raw) AS [Converted Text], [src].[B] AS [B], [src].[A] AS [A], 9 AS Tail FROM dbo.Input AS src;", "dbo.Expanded"));
        Assert.Equal(expected, emitted);
        Assert.Empty(model.SelectStarExpressionList);
        AssertCanonicalGraph(model);
    }

    private static void AssertCanonicalGraph(MetaTransformScriptModel model)
    {
        // Write rejects duplicate IDs, missing targets and references to objects outside the owning collections.
        var binding = new MetaTransformScriptWorkspaceBinding();
        var state = binding.Write(model, MetaTransformScriptModels.LoadContract());
        var restored = binding.Read(state);
        var sql = new MetaTransformScriptSqlService();
        Assert.Equal(sql.ExportModuleDefinitions(model), sql.ExportModuleDefinitions(restored));
    }
}
