using MetaTransformScript;
using MetaTransformScript.Sql;
using MetaTransformScript.Sql.Parsing;

public sealed class SqlServiceImportExportTests
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
    [InlineData("024_query_parentheses.sql")]
    [InlineData("026_builtin_table_functions.sql")]
    [InlineData("023_table_sample.sql")]
    [InlineData("025_distinct_predicate.sql")]
    [InlineData("027_fulltext.sql")]
    [InlineData("061_freetext.sql")]
    [InlineData("029_literals_and_special_calls.sql")]
    [InlineData("030_time_zone_extract.sql")]
    [InlineData("031_join_parentheses.sql")]
    [InlineData("036_sequence_and_globals.sql")]
    [InlineData("009_grouping_sets.sql")]
    [InlineData("010_rollup_cube.sql")]
    [InlineData("040_view_column_list.sql")]
    [InlineData("042_cte_column_list.sql")]
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
    [InlineData("055_xml_nodes.sql")]
    [InlineData("056_analytic_window_functions.sql")]
    [InlineData("057_percentile_within_group.sql")]
    [InlineData("058_remaining_aggregate_functions.sql")]
    [InlineData("059_range_window_frames.sql")]
    [InlineData("060_remaining_analytic_functions.sql")]
    [InlineData("028_fulltext_table.sql")]
    [InlineData("062_freetext_table.sql")]
    [InlineData("063_four_part_names.sql")]
    [InlineData("064_remaining_data_types.sql")]
    [InlineData("065_select_star_plain.sql")]
    [InlineData("066_inline_tvf.sql")]
    [InlineData("067_backtick_identifiers.sql")]
    [InlineData("068_parenthesized_set_derived_table.sql")]
    public void ImportFromSqlCode_MatchesDirectParser_OnAuditedCorpus(string fileName)
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus(fileName);
        const string bareSelectName = "dbo.v_test";

        var serviceModel = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, bareSelectName);
        var parserModel = new MetaTransformScriptSqlParser().ParseSqlCode(sql, bareSelectName: bareSelectName);

        MetaTransformScriptTestHelper.AssertModelListCountsEqual(parserModel, serviceModel);

        serviceModel = MetaTransformScriptTestHelper.RoundTripWorkspace(serviceModel, "service");
        parserModel = MetaTransformScriptTestHelper.RoundTripWorkspace(parserModel, "parser");

        var service = new MetaTransformScriptSqlService();
        Assert.Equal(service.ExportToSqlCode(parserModel), service.ExportToSqlCode(serviceModel));
        Assert.Equal(parserModel.TransformScriptList.Single().Name, serviceModel.TransformScriptList.Single().Name);
        Assert.Equal(parserModel.TransformScriptList.Single().TargetSqlIdentifier, serviceModel.TransformScriptList.Single().TargetSqlIdentifier);
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
    public void ImportFromSqlCode_BareSelect_UsesScriptNameAsTargetSqlIdentifier()
    {
        const string sql = """
SELECT
    c.CustomerId
FROM sales.Customer AS c
""";

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_inline_target");
        var script = Assert.Single(model.TransformScriptList);
        Assert.Equal("dbo.v_inline_target", script.Name);
        Assert.Equal("dbo.v_inline_target", script.TargetSqlIdentifier);
    }

    [Fact]
    public void ImportFromSqlCode_MapsParseErrors_ToParseFailed()
    {
        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode("SELECT * FROM", "dbo.v_parse_fail"));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.ParseFailed, exception.Kind);
    }

    public static IEnumerable<object[]> SingleFileSqlImportCases()
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
    [MemberData(nameof(SingleFileSqlImportCases))]
    public void ImportFromSqlFile_MatchesDirectParser_OnSingleFileInputs(string fileName, string sql)
    {
        var tempFilePath = MetaTransformScriptTestHelper.WriteTempSqlFile(fileName, sql);

        var serviceModel = new MetaTransformScriptSqlService().ImportFromSqlFile(tempFilePath);
        var parserModel = new MetaTransformScriptSqlParser().ParseSqlCode(
            sql,
            Path.GetFileName(tempFilePath));

        MetaTransformScriptTestHelper.AssertModelListCountsEqual(parserModel, serviceModel);

        serviceModel = MetaTransformScriptTestHelper.RoundTripWorkspace(serviceModel, "service-path");
        parserModel = MetaTransformScriptTestHelper.RoundTripWorkspace(parserModel, "parser-path");

        var service = new MetaTransformScriptSqlService();
        Assert.Equal(service.ExportToSqlCode(parserModel), service.ExportToSqlCode(serviceModel));
        Assert.Equal(parserModel.TransformScriptList.Single().Name, serviceModel.TransformScriptList.Single().Name);
    }

    [Fact]
    public void ImportFromSqlFile_ParsesCreateViewColumnLists_OnSingleFileInputs()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("040_view_column_list.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlFile(
            MetaTransformScriptTestHelper.WriteTempSqlFile("wrapper-heavy.sql", sql));

        var script = Assert.Single(model.TransformScriptList);
        Assert.Equal("dbo.v_view_column_list", script.Name);
        Assert.Equal("dbo.v_view_column_list", script.TargetSqlIdentifier);
        Assert.Equal(2, model.TransformScriptViewColumnsItemList.Count);
    }

    [Fact]
    public void ImportFromSqlFile_ParsesInlineTableValuedFunctionWrappers()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("066_inline_tvf.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlFile(
            MetaTransformScriptTestHelper.WriteTempSqlFile("inline-tvf.sql", sql));

        var script = Assert.Single(model.TransformScriptList);
        Assert.Equal("dbo.fn_customer_orders", script.Name);
        Assert.Equal("InlineTableValuedFunction", script.ScriptObjectKind);
        Assert.Equal("dbo.fn_customer_orders", script.TargetSqlIdentifier);
        Assert.Empty(model.TransformScriptViewColumnsItemList);
        Assert.Equal(2, model.TransformScriptFunctionParametersItemList.Count);

        var parameterNames = model.TransformScriptFunctionParametersItemList
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => model.IdentifierList.Single(identifier => string.Equals(identifier.Id, item.IdentifierId, StringComparison.Ordinal)).Value)
            .ToArray();
        Assert.Equal(["@CustomerId", "@FromDate"], parameterNames);
    }

    [Fact]
    public void ImportFromSqlFile_ParsesCrossDatabaseSchemaObjectNames_OnSingleFileInputs()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("051_cross_database_names.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlFile(
            MetaTransformScriptTestHelper.WriteTempSqlFile("cross-database.sql", sql));
        model = MetaTransformScriptTestHelper.RoundTripWorkspace(model, "cross-database");

        var script = Assert.Single(model.TransformScriptList);
        Assert.Equal("dbo.v_cross_database_names", script.Name);

        var emittedSql = new MetaTransformScriptSqlService().ExportToSqlCode(model);
        Assert.Contains("FROM SalesDb.sales.Customer AS src", emittedSql);
        Assert.Contains("NEXT VALUE FOR UtilityDb.dbo.CustomerSequence", emittedSql);
        Assert.Contains("CROSS APPLY UtilityDb.dbo.fnSplit(src.TagList) AS splitItem", emittedSql);
        Assert.Contains("FROM ArchiveDb.sales.CustomerArchive AS arc", emittedSql);
    }

    [Fact]
    public void ImportFromSqlFile_ParsesFourPartSchemaObjectNames_OnSingleFileInputs()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("063_four_part_names.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlFile(
            MetaTransformScriptTestHelper.WriteTempSqlFile("four-part.sql", sql));
        model = MetaTransformScriptTestHelper.RoundTripWorkspace(model, "four-part");

        var script = Assert.Single(model.TransformScriptList);
        Assert.Equal("dbo.v_four_part_names", script.Name);

        var emittedSql = new MetaTransformScriptSqlService().ExportToSqlCode(model);
        Assert.Contains("FROM ReportingSrv.SalesDb.sales.Customer AS src", emittedSql);
        Assert.Contains("NEXT VALUE FOR UtilitySrv.UtilityDb.dbo.CustomerSequence", emittedSql);
        Assert.Contains("FROM ArchiveSrv.ArchiveDb.sales.CustomerArchive AS arc", emittedSql);
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesLeftAndRightFunctionCalls_AsDedicatedModelShapes()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("029_literals_and_special_calls.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        Assert.Single(model.LeftFunctionCallList);
        Assert.Single(model.RightFunctionCallList);

        var leftOrRightFunctionNames = model.FunctionCallFunctionNameLinkList
            .Select(link => model.IdentifierList.Single(identifier => string.Equals(identifier.Id, link.IdentifierId, StringComparison.Ordinal)).Value)
            .Where(static name => string.Equals(name, "LEFT", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "RIGHT", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        Assert.Empty(leftOrRightFunctionNames);
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesLikeEscape_AsDedicatedEscapeLink()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("054_like_escape.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        Assert.Equal(2, model.LikePredicateList.Count);
        Assert.Equal(2, model.LikePredicateEscapeExpressionLinkList.Count);
        Assert.All(model.LikePredicateList, predicate => Assert.False(string.Equals(predicate.OdbcEscape, "true", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesXmlNodesTableReference_AsDedicatedModelShape()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("055_xml_nodes.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        Assert.Single(model.XmlNodesTableReferenceList);
        Assert.Single(model.XmlNodesTableReferenceTargetExpressionLinkList);
        Assert.Single(model.XmlNodesTableReferenceXQueryStringLinkList);
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesBuiltInTableFunctions_AsGlobalFunctionTableReferences()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("026_builtin_table_functions.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        Assert.Equal(2, model.GlobalFunctionTableReferenceList.Count);
        Assert.Empty(model.SchemaObjectFunctionTableReferenceList);

        var functionNames = model.GlobalFunctionTableReferenceNameLinkList
            .Select(link => model.IdentifierList.Single(identifier => string.Equals(identifier.Id, link.IdentifierId, StringComparison.Ordinal)).Value)
            .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.Equal(["GENERATE_SERIES", "STRING_SPLIT"], functionNames, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesWithinGroupOrderBy_OnFunctionCalls()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("057_percentile_within_group.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        Assert.Equal(2, model.FunctionCallWithinGroupOrderByClauseLinkList.Count);
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesRemainingAggregateFunctions_AsGenericFunctionCalls()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("058_remaining_aggregate_functions.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        var functionNames = model.FunctionCallFunctionNameLinkList
            .Select(link => model.IdentifierList.Single(identifier => string.Equals(identifier.Id, link.IdentifierId, StringComparison.Ordinal)).Value)
            .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.Equal(
            [
                "APPROX_COUNT_DISTINCT",
                "CHECKSUM_AGG",
                "COUNT_BIG",
                "MIN",
                "STDEV",
                "STDEVP",
                "STRING_AGG",
                "VAR",
                "VARP"
            ],
            functionNames,
            StringComparer.OrdinalIgnoreCase);
        Assert.Single(model.FunctionCallWithinGroupOrderByClauseLinkList);
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesRangeWindowFrames_AsDedicatedWindowFrameClauses()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("059_range_window_frames.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        Assert.Equal(2, model.WindowFrameClauseList.Count);
        Assert.All(
            model.WindowFrameClauseList,
            windowFrameClause => Assert.Equal("Range", windowFrameClause.WindowFrameType));

        var topDelimiterTypes = model.WindowFrameClauseTopLinkList
            .Select(link => model.WindowDelimiterList.Single(windowDelimiter => string.Equals(windowDelimiter.Id, link.WindowDelimiterId, StringComparison.Ordinal)).WindowDelimiterType)
            .OrderBy(static value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(["CurrentRow", "UnboundedPreceding"], topDelimiterTypes);

        var bottomDelimiterTypes = model.WindowFrameClauseBottomLinkList
            .Select(link => model.WindowDelimiterList.Single(windowDelimiter => string.Equals(windowDelimiter.Id, link.WindowDelimiterId, StringComparison.Ordinal)).WindowDelimiterType)
            .ToArray();

        Assert.Equal(["CurrentRow"], bottomDelimiterTypes);
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesFreeTextPredicate_AsDedicatedModelShape()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("061_freetext.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        Assert.Single(model.FullTextPredicateList);
        Assert.Equal("FreeText", model.FullTextPredicateList.Single().FullTextFunctionType);
        Assert.Single(model.FullTextPredicateColumnsItemList);
        Assert.Single(model.FullTextPredicateValueLinkList);
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesFullTextTableReferences_AsDedicatedModelShape()
    {
        var containsSql = MetaTransformScriptTestHelper.LoadCorpus("028_fulltext_table.sql");
        var freeTextSql = MetaTransformScriptTestHelper.LoadCorpus("062_freetext_table.sql");

        var containsModel = new MetaTransformScriptSqlService().ImportFromSqlCode(containsSql, "dbo.v_test");
        var freeTextModel = new MetaTransformScriptSqlService().ImportFromSqlCode(freeTextSql, "dbo.v_test");

        Assert.Single(containsModel.FullTextTableReferenceList);
        Assert.Equal("Contains", containsModel.FullTextTableReferenceList.Single().FullTextFunctionType);
        Assert.Single(containsModel.FullTextTableReferenceTableNameLinkList);
        Assert.Single(containsModel.FullTextTableReferenceSearchConditionLinkList);

        Assert.Single(freeTextModel.FullTextTableReferenceList);
        Assert.Equal("FreeText", freeTextModel.FullTextTableReferenceList.Single().FullTextFunctionType);
        Assert.Single(freeTextModel.FullTextTableReferenceTableNameLinkList);
        Assert.Single(freeTextModel.FullTextTableReferenceSearchConditionLinkList);
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesRemainingDataTypes_AsSqlDataTypeReferences()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("064_remaining_data_types.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        var sqlDataTypeOptions = model.SqlDataTypeReferenceList
            .Select(row => row.SqlDataTypeOption)
            .OrderBy(static value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "Char",
                "NChar",
                "Numeric",
                "Real",
                "SmallInt",
                "TinyInt"
            ],
            sqlDataTypeOptions);
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesRemainingAnalyticFunctions_AsGenericFunctionCallsWithOverClauses()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("060_remaining_analytic_functions.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        var functionNames = model.FunctionCallFunctionNameLinkList
            .Select(link => model.IdentifierList.Single(identifier => string.Equals(identifier.Id, link.IdentifierId, StringComparison.Ordinal)).Value)
            .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.Equal(
            [
                "DENSE_RANK",
                "LAG",
                "LEAD",
                "NTILE",
                "RANK"
            ],
            functionNames,
            StringComparer.OrdinalIgnoreCase);
        Assert.Equal(5, model.FunctionCallOverClauseLinkList.Count);
    }

    [Fact]
    public void ImportFromSqlCode_BacktickQuotedIdentifiers_AreAcceptedAndExportedAsBracketQuoted()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("067_backtick_identifiers.sql");

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql, "dbo.v_test");
        var emitted = service.ExportToSqlCode(model);

        Assert.Contains("[order count]", emitted);
        Assert.Contains("[total shipping cost]", emitted);
        Assert.Contains("[total net profit]", emitted);
        Assert.DoesNotContain("`order count`", emitted);
    }

    [Fact]
    public void ExportToSqlCode_SupportsAllCurrentComparisonOperators()
    {
        const string sql = """
CREATE VIEW dbo.v_comparison_ops AS
SELECT
    s.CustomerId
FROM dbo.Source AS s
WHERE s.Score >= 10
  AND s.Rank < 20
  AND s.Age <= 65
  AND s.Status <> 0
  AND s.Score >= ANY (SELECT
      o.Score
  FROM dbo.Other AS o)
  AND s.Age <= ALL (SELECT
      o.Age
  FROM dbo.Other AS o)
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var emitted = service.ExportToSqlCode(model);

        Assert.Contains("s.Score >= 10", emitted);
        Assert.Contains("s.Rank < 20", emitted);
        Assert.Contains("s.Age <= 65", emitted);
        Assert.Contains("s.Status <> 0", emitted);
        Assert.Contains("s.Score >= ANY", emitted);
        Assert.Contains("s.Age <= ALL", emitted);
    }

    [Fact]
    public async Task ExportToSqlPath_UsesTransformScriptTargetSqlIdentifier()
    {
        const string sql = """
CREATE VIEW dbo.v_original_target
AS
SELECT
    s.CustomerId
FROM dbo.Source AS s
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var script = Assert.Single(model.TransformScriptList);
        script.TargetSqlIdentifier = "reporting.v_overridden_target";

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Script.Tests", Guid.NewGuid().ToString("N"));
        var outputFilePath = Path.Combine(tempRoot, "out.sql");

        try
        {
            Directory.CreateDirectory(tempRoot);
            await service.ExportToSqlPathAsync(model, outputFilePath);

            var emitted = await File.ReadAllTextAsync(outputFilePath);
            Assert.Contains("CREATE VIEW reporting.v_overridden_target", emitted);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ExportToSqlPath_EmitsCreateFunctionEnvelope_ForInlineTvf()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("066_inline_tvf.sql");

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Script.Tests", Guid.NewGuid().ToString("N"));
        var outputFilePath = Path.Combine(tempRoot, "tvf-out.sql");

        try
        {
            Directory.CreateDirectory(tempRoot);
            await service.ExportToSqlPathAsync(model, outputFilePath);

            var emitted = await File.ReadAllTextAsync(outputFilePath);
            Assert.Contains("CREATE FUNCTION dbo.fn_customer_orders", emitted);
            Assert.Contains("RETURNS TABLE", emitted);
            Assert.Contains("RETURN", emitted);
            Assert.Contains("@CustomerId int", emitted);
            Assert.Contains("@FromDate date", emitted);
            Assert.DoesNotContain("CREATE VIEW", emitted);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    [Fact]
    public void ImportFromSqlCode_FailsExplicitly_ForNonInlineTvfWrapper()
    {
        const string sql = """
CREATE FUNCTION dbo.fn_non_inline
(
    @CustomerId int
)
RETURNS @Output TABLE
(
    CustomerId int
)
AS
BEGIN
    INSERT INTO @Output (CustomerId)
    SELECT
        @CustomerId;
    RETURN;
END
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
        Assert.Contains("inline table-valued", exception.Message);
    }

    [Fact]
    public void ImportFromSqlFile_FailsExplicitly_ForBareSelectSingleFileInputs()
    {
        const string sql = """
SELECT
    s.CustomerId
FROM dbo.Source AS s
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlFile(
                MetaTransformScriptTestHelper.WriteTempSqlFile("bare-select.sql", sql)));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
    }

    [Fact]
    public void ImportFromSqlFile_ParsesSetAndGoWrappedSingleFileViewScripts()
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

        var model = new MetaTransformScriptSqlService().ImportFromSqlFile(
            MetaTransformScriptTestHelper.WriteTempSqlFile("set-go.sql", sql));

        var script = Assert.Single(model.TransformScriptList);
        Assert.Equal("dbo.v_set_go", script.Name);
        Assert.Equal("dbo.v_set_go", script.TargetSqlIdentifier);
        Assert.Single(model.TransformScriptViewColumnsItemList);
    }

    [Fact]
    public void ImportFromSqlFile_FailsExplicitly_ForUnsupportedCreateViewOptions()
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
            () => new MetaTransformScriptSqlService().ImportFromSqlFile(
                MetaTransformScriptTestHelper.WriteTempSqlFile("schemabinding.sql", sql)));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
        Assert.Contains("WITH SCHEMABINDING", exception.Message);
    }

    [Fact]
    public void ImportFromSqlFile_FailsExplicitly_ForWithCheckOption()
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
            () => new MetaTransformScriptSqlService().ImportFromSqlFile(
                MetaTransformScriptTestHelper.WriteTempSqlFile("check-option.sql", sql)));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
        Assert.Contains("WITH CHECK OPTION", exception.Message);
    }

    [Fact]
    public void ImportFromSqlFile_FailsExplicitly_ForUnsupportedAuxiliaryBatches()
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
            () => new MetaTransformScriptSqlService().ImportFromSqlFile(
                MetaTransformScriptTestHelper.WriteTempSqlFile("use-batch.sql", sql)));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
        Assert.Contains("Auxiliary batch 'USE' is not supported", exception.Message);
    }

    [Fact]
    public void ImportFromSqlFile_FailsExplicitly_WhenPathIsDirectory()
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), "meta-bi", "metatransformscript-tests", Guid.NewGuid().ToString("N"), "sql-file-dir");
        Directory.CreateDirectory(directoryPath);

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlFile(directoryPath));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.SourcePathNotFound, exception.Kind);
    }
}
