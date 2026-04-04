using System.Collections;
using MetaTransformScript;
using MetaTransformScript.Instance;
using MetaTransformScript.Sql;
using MetaTransformScript.Sql.Parsing;

public sealed class OwnedParserPhase1Tests
{
    [Theory]
    [InlineData("001_basic_select.sql")]
    [InlineData("002_select_star.sql")]
    [InlineData("003_join_variants.sql")]
    [InlineData("004_apply_sources.sql")]
    [InlineData("005_pivot.sql")]
    [InlineData("006_unpivot.sql")]
    [InlineData("007_where_predicates.sql")]
    [InlineData("008_group_by_having.sql")]
    [InlineData("011_subqueries_and_correlation.sql")]
    [InlineData("012_subquery_predicates.sql")]
    [InlineData("013_set_operations.sql")]
    [InlineData("014_value_expressions.sql")]
    [InlineData("015_window_functions.sql")]
    [InlineData("016_named_window.sql")]
    [InlineData("017_cte.sql")]
    [InlineData("018_ordering_and_top.sql")]
    [InlineData("019_offset_fetch.sql")]
    [InlineData("020_xml_namespaces_and_methods.sql")]
    [InlineData("024_query_parentheses.sql")]
    [InlineData("041_xml_namespaces_default.sql")]
    [InlineData("042_cte_column_list.sql")]
    [InlineData("043_recursive_cte_column_list.sql")]
    [InlineData("044_window_frame_offsets.sql")]
    [InlineData("045_nested_subqueries.sql")]
    public void OwnedParser_MatchesCurrentImporter_OnPhase1Corpus(string fileName)
    {
        var sql = LoadCorpus(fileName);
        var ownedModel = new MetaTransformScriptSqlParser().ParseSqlCode(sql);
        var oracleModel = new MetaTransformScriptSqlService().ImportFromSqlCode(sql);

        AssertModelListCountsEqual(oracleModel, ownedModel);

        ownedModel = RoundTripWorkspace(ownedModel, "owned");
        oracleModel = RoundTripWorkspace(oracleModel, "oracle");

        var service = new MetaTransformScriptSqlService();
        Assert.Equal(service.ExportToSqlCode(oracleModel), service.ExportToSqlCode(ownedModel));
        Assert.Equal(oracleModel.TransformScriptList.Single().Name, ownedModel.TransformScriptList.Single().Name);
    }

    [Fact]
    public void OwnedParser_RejectsViewColumnLists_InCurrentSlice()
    {
        var sql = LoadCorpus("040_view_column_list.sql");
        var exception = Assert.Throws<MetaTransformScriptSqlParserException>(
            () => new MetaTransformScriptSqlParser().ParseSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlParserFailureKind.UnsupportedSyntax, exception.FailureKind);
    }

    private static string LoadCorpus(string fileName)
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "Reference",
            "Corpus",
            fileName));
        return File.ReadAllText(path);
    }

    private static void AssertModelListCountsEqual(
        MetaTransformScriptModel expected,
        MetaTransformScriptModel actual)
    {
        var listProperties = typeof(MetaTransformScriptModel)
            .GetProperties()
            .Where(static property => typeof(ICollection).IsAssignableFrom(property.PropertyType))
            .OrderBy(static property => property.Name, StringComparer.Ordinal);

        foreach (var property in listProperties)
        {
            var expectedCount = ((ICollection?)property.GetValue(expected))?.Count ?? 0;
            var actualCount = ((ICollection?)property.GetValue(actual))?.Count ?? 0;
            Assert.True(
                expectedCount == actualCount,
                $"{property.Name}: expected {expectedCount}, actual {actualCount}");
        }
    }

    private static MetaTransformScriptModel RoundTripWorkspace(MetaTransformScriptModel model, string label)
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "meta-bi", "metatransformscript-tests", Guid.NewGuid().ToString("N"), label);
        Directory.CreateDirectory(workspacePath);
        MetaTransformScriptInstance.SaveToWorkspace(model, workspacePath);
        return MetaTransformScriptInstance.LoadFromWorkspace(workspacePath, searchUpward: false);
    }
}
