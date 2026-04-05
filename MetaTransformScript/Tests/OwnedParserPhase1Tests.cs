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
    [InlineData("009_grouping_sets.sql")]
    [InlineData("010_rollup_cube.sql")]
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
    [InlineData("021_inline_values.sql")]
    [InlineData("023_table_sample.sql")]
    [InlineData("025_distinct_predicate.sql")]
    [InlineData("027_fulltext.sql")]
    [InlineData("029_literals_and_special_calls.sql")]
    [InlineData("030_time_zone_extract.sql")]
    [InlineData("031_join_parentheses.sql")]
    [InlineData("036_sequence_and_globals.sql")]
    [InlineData("040_view_column_list.sql")]
    [InlineData("024_query_parentheses.sql")]
    [InlineData("041_xml_namespaces_default.sql")]
    [InlineData("042_cte_column_list.sql")]
    [InlineData("043_recursive_cte_column_list.sql")]
    [InlineData("044_window_frame_offsets.sql")]
    [InlineData("045_nested_subqueries.sql")]
    [InlineData("046_aggregate_distinct.sql")]
    [InlineData("047_parenthesized_scalar_expressions.sql")]
    public void ParserRoundTripsSupportedCorpus(string fileName)
    {
        var sql = LoadCorpus(fileName);
        var parser = new MetaTransformScriptSqlParser();
        var firstModel = parser.ParseSqlCode(sql, bareSelectName: "dbo.v_test");
        firstModel = RoundTripWorkspace(firstModel, "first");

        var service = new MetaTransformScriptSqlService();
        var firstEmission = service.ExportToSqlCode(firstModel);
        var secondModel = parser.ParseSqlCode(firstEmission, bareSelectName: "dbo.v_test");
        secondModel = RoundTripWorkspace(secondModel, "second");
        var secondEmission = service.ExportToSqlCode(secondModel);

        Assert.Equal(firstEmission, secondEmission);
    }

    [Fact]
    public void ParserRejectsUnsupportedGroupByAll()
    {
        const string sql = """
SELECT
    c.CustomerId
FROM sales.Customer AS c
GROUP BY ALL c.CustomerId
""";
        var exception = Assert.Throws<MetaTransformScriptSqlParserException>(
            () => new MetaTransformScriptSqlParser().ParseSqlCode(sql, bareSelectName: "dbo.v_group_by_all"));

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
    private static MetaTransformScriptModel RoundTripWorkspace(MetaTransformScriptModel model, string label)
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "meta-bi", "metatransformscript-tests", Guid.NewGuid().ToString("N"), label);
        Directory.CreateDirectory(workspacePath);
        MetaTransformScriptInstance.SaveToWorkspace(model, workspacePath);
        return MetaTransformScriptInstance.LoadFromWorkspace(workspacePath, searchUpward: false);
    }
}
