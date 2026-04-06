using System.Collections;
using MetaTransformScript;
using MetaTransformScript.Instance;
using MetaTransformScript.Sql;
using MetaTransformScript.Sql.Parsing;

public sealed class SqlServiceOwnedImportTests
{
    [Theory]
    [InlineData("001_basic_select.sql")]
    [InlineData("017_cte.sql")]
    [InlineData("020_xml_namespaces_and_methods.sql")]
    [InlineData("021_inline_values.sql")]
    [InlineData("023_table_sample.sql")]
    [InlineData("025_distinct_predicate.sql")]
    [InlineData("027_fulltext.sql")]
    [InlineData("029_literals_and_special_calls.sql")]
    [InlineData("030_time_zone_extract.sql")]
    [InlineData("031_join_parentheses.sql")]
    [InlineData("036_sequence_and_globals.sql")]
    [InlineData("009_grouping_sets.sql")]
    [InlineData("010_rollup_cube.sql")]
    [InlineData("040_view_column_list.sql")]
    [InlineData("044_window_frame_offsets.sql")]
    [InlineData("045_nested_subqueries.sql")]
    [InlineData("046_aggregate_distinct.sql")]
    [InlineData("047_parenthesized_scalar_expressions.sql")]
    [InlineData("048_group_by_all.sql")]
    [InlineData("049_data_type_variants.sql")]
    [InlineData("050_remaining_sanctioned_sqlserver_types.sql")]
    [InlineData("051_cross_database_names.sql")]
    [InlineData("052_arithmetic_operators.sql")]
    [InlineData("053_negated_predicates.sql")]
    [InlineData("054_like_escape.sql")]
    public void ImportFromSqlCode_MatchesDirectParser_OnSupportedInputs(string fileName)
    {
        var sql = LoadCorpus(fileName);
        const string bareSelectName = "dbo.v_test";

        var serviceModel = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, bareSelectName);
        var parserModel = new MetaTransformScriptSqlParser().ParseSqlCode(sql, bareSelectName: bareSelectName);

        AssertModelListCountsEqual(parserModel, serviceModel);

        serviceModel = RoundTripWorkspace(serviceModel, "service");
        parserModel = RoundTripWorkspace(parserModel, "parser");

        var service = new MetaTransformScriptSqlService();
        Assert.Equal(service.ExportToSqlCode(parserModel), service.ExportToSqlCode(serviceModel));
        Assert.Equal(parserModel.TransformScriptList.Single().Name, serviceModel.TransformScriptList.Single().Name);
    }

    [Fact]
    public void ImportFromSqlCode_RequiresName_ForBareSelectInput()
    {
        const string sql = """
SELECT
    c.CustomerId
FROM sales.Customer AS c
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
    }

    [Fact]
    public void ImportFromSqlCode_MapsParseErrors_ToParseFailed()
    {
        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode("SELECT * FROM", "dbo.v_parse_fail"));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.ParseFailed, exception.Kind);
    }

    public static IEnumerable<object[]> SingleFileOwnedSqlCases()
    {
        yield return
        [
            "cte.sql",
            """
CREATE VIEW dbo.v_cte AS
WITH base_cte AS
(
    SELECT
        s.Id
    FROM dbo.Source AS s
)
SELECT
    b.Id
FROM base_cte AS b
"""
        ];

        yield return
        [
            "xml.sql",
            """
CREATE VIEW dbo.v_xml AS
WITH XMLNAMESPACES ('urn:test' AS ns)
SELECT
    s.XmlPayload.value('(/ns:Root/ns:Id/text())[1]', 'int') AS XmlId
FROM dbo.XmlSource AS s
"""
        ];
    }

    [Theory]
    [MemberData(nameof(SingleFileOwnedSqlCases))]
    public void ImportFromSqlPath_UsesOwnedParser_OnSingleFileNoGoSupportedInputs(string fileName, string sql)
    {
        var tempFilePath = WriteTempSqlFile(fileName, sql);

        var serviceModel = new MetaTransformScriptSqlService().ImportFromSqlPath(tempFilePath);
        var parserModel = new MetaTransformScriptSqlParser().ParseSqlCode(
            sql,
            Path.GetFileName(tempFilePath));

        AssertModelListCountsEqual(parserModel, serviceModel);

        serviceModel = RoundTripWorkspace(serviceModel, "service-path");
        parserModel = RoundTripWorkspace(parserModel, "parser-path");

        var service = new MetaTransformScriptSqlService();
        Assert.Equal(service.ExportToSqlCode(parserModel), service.ExportToSqlCode(serviceModel));
        Assert.Equal(parserModel.TransformScriptList.Single().Name, serviceModel.TransformScriptList.Single().Name);
    }

    [Fact]
    public void ImportFromSqlPath_ParsesCreateViewColumnLists_OnSingleFileInputs()
    {
        var sql = LoadCorpus("040_view_column_list.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlPath(
            WriteTempSqlFile("wrapper-heavy.sql", sql));

        var script = Assert.Single(model.TransformScriptList);
        Assert.Equal("dbo.v_view_column_list", script.Name);
        Assert.Equal(2, model.TransformScriptViewColumnsItemList.Count);
    }

    [Fact]
    public void ImportFromSqlPath_ParsesCrossDatabaseSchemaObjectNames_OnSingleFileInputs()
    {
        var sql = LoadCorpus("051_cross_database_names.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlPath(
            WriteTempSqlFile("cross-database.sql", sql));
        model = RoundTripWorkspace(model, "cross-database");

        var script = Assert.Single(model.TransformScriptList);
        Assert.Equal("dbo.v_cross_database_names", script.Name);

        var emittedSql = new MetaTransformScriptSqlService().ExportToSqlCode(model);
        Assert.Contains("FROM SalesDb.sales.Customer AS src", emittedSql);
        Assert.Contains("NEXT VALUE FOR UtilityDb.dbo.CustomerSequence", emittedSql);
        Assert.Contains("CROSS APPLY UtilityDb.dbo.fnSplit(src.TagList) AS splitItem", emittedSql);
        Assert.Contains("FROM ArchiveDb.sales.CustomerArchive AS arc", emittedSql);
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesLeftAndRightFunctionCalls_AsDedicatedModelShapes()
    {
        var sql = LoadCorpus("029_literals_and_special_calls.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        Assert.Single(model.LeftFunctionCallList);
        Assert.Single(model.RightFunctionCallList);

        var leftOrRightFunctionNames = model.FunctionCallFunctionNameLinkList
            .Select(link => model.IdentifierList.Single(identifier => string.Equals(identifier.Id, link.ValueId, StringComparison.Ordinal)).Value)
            .Where(static name => string.Equals(name, "LEFT", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "RIGHT", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        Assert.Empty(leftOrRightFunctionNames);
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesLikeEscape_AsDedicatedEscapeLink()
    {
        var sql = LoadCorpus("054_like_escape.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        Assert.Equal(2, model.LikePredicateList.Count);
        Assert.Equal(2, model.LikePredicateEscapeExpressionLinkList.Count);
        Assert.All(model.LikePredicateList, predicate => Assert.False(string.Equals(predicate.OdbcEscape, "true", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void ImportFromSqlPath_FailsExplicitly_ForBareSelectSingleFileInputs()
    {
        const string sql = """
SELECT
    s.CustomerId
FROM dbo.Source AS s
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlPath(
                WriteTempSqlFile("bare-select.sql", sql)));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
    }

    [Fact]
    public void ImportFromSqlPath_ParsesSetAndGoWrappedSingleFileViewScripts()
    {
        const string sql = """
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW dbo.v_set_go
(
    OutputCustomerId
)
AS
SELECT
    s.CustomerId
FROM dbo.Source AS s
GO
""";

        var model = new MetaTransformScriptSqlService().ImportFromSqlPath(
            WriteTempSqlFile("set-go.sql", sql));

        var script = Assert.Single(model.TransformScriptList);
        Assert.Equal("dbo.v_set_go", script.Name);
        Assert.Single(model.TransformScriptViewColumnsItemList);
    }

    [Fact]
    public void ImportFromSqlPath_FailsExplicitly_ForUnsupportedCreateViewOptions()
    {
        const string sql = """
CREATE VIEW dbo.v_schema_bound
WITH SCHEMABINDING
AS
SELECT
    s.CustomerId
FROM dbo.Source AS s
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlPath(
                WriteTempSqlFile("schemabinding.sql", sql)));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
        Assert.Contains("WITH SCHEMABINDING", exception.Message);
    }

    [Fact]
    public void ImportFromSqlPath_FailsExplicitly_ForWithCheckOption()
    {
        const string sql = """
CREATE VIEW dbo.v_check_option
AS
SELECT
    s.CustomerId
FROM dbo.Source AS s
WITH CHECK OPTION
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlPath(
                WriteTempSqlFile("check-option.sql", sql)));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
        Assert.Contains("WITH CHECK OPTION", exception.Message);
    }

    [Fact]
    public void ImportFromSqlPath_FailsExplicitly_ForUnsupportedAuxiliaryBatches()
    {
        const string sql = """
USE ReportingDb
GO
CREATE VIEW dbo.v_use_batch AS
SELECT
    s.CustomerId
FROM dbo.Source AS s
GO
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlPath(
                WriteTempSqlFile("use-batch.sql", sql)));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
        Assert.Contains("Auxiliary batch 'USE' is not supported", exception.Message);
    }

    [Fact]
    public void ImportFromSqlPath_FolderImport_TreatsFolderAsOneLogicalViewSource()
    {
        const string goSplitSql = """
SET ANSI_NULLS ON
GO
CREATE VIEW dbo.v_go_one AS
SELECT
    1 AS One
GO
CREATE VIEW dbo.v_go_two AS
SELECT
    2 AS Two
GO
""";
        const string plainWrappedViewSql = """
CREATE VIEW sales.v_plain AS
SELECT
    s.CustomerId,
    s.CustomerName
FROM dbo.Source AS s
WHERE s.CustomerId = 1
""";

        var folderPath = WriteTempSqlFolder(
            ("plain.sql", plainWrappedViewSql),
            ("wrapper.sql", LoadCorpus("040_view_column_list.sql")),
            ("batches.sql", goSplitSql));

        var model = new MetaTransformScriptSqlService().ImportFromSqlPath(folderPath);

        Assert.Collection(
            model.TransformScriptList.OrderBy(static script => script.Name, StringComparer.Ordinal).ToArray(),
            script => Assert.Equal("dbo.v_go_one", script.Name),
            script => Assert.Equal("dbo.v_go_two", script.Name),
            script => Assert.Equal("dbo.v_view_column_list", script.Name),
            script => Assert.Equal("sales.v_plain", script.Name));
        Assert.Equal(2, model.TransformScriptViewColumnsItemList.Count);
    }

    [Fact]
    public void ImportFromSqlPath_FolderImport_FailsExplicitly_WhenFolderMixesBareSelectAndCreateView()
    {
        const string bareSelectSql = """
SELECT
    c.CustomerId
FROM sales.Customer AS c
""";
        const string wrappedViewSql = """
CREATE VIEW dbo.v_folder_wrapped AS
SELECT
    s.CustomerId
FROM dbo.Source AS s
""";

        var folderPath = WriteTempSqlFolder(
            ("bare-select.sql", bareSelectSql),
            ("wrapped.sql", wrappedViewSql));

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlPath(folderPath));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
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

    private static string WriteTempSqlFile(string fileName, string sql)
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), "meta-bi", "metatransformscript-tests", Guid.NewGuid().ToString("N"), "sql-path");
        Directory.CreateDirectory(directoryPath);
        var filePath = Path.Combine(directoryPath, fileName);
        File.WriteAllText(filePath, sql);
        return filePath;
    }

    private static string WriteTempSqlFolder(params (string FileName, string Sql)[] files)
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), "meta-bi", "metatransformscript-tests", Guid.NewGuid().ToString("N"), "sql-folder");
        Directory.CreateDirectory(directoryPath);

        foreach (var (fileName, sql) in files)
        {
            File.WriteAllText(Path.Combine(directoryPath, fileName), sql);
        }

        return directoryPath;
    }
}
