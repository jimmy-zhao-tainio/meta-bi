using MetaTransformScript;
using MetaTransformScript.Sql;
using MetaTransformScript.Sql.Parsing;
using MetaDataType;

public sealed class SqlServiceImportExportTests
{
    [Fact]
    public void ImportSqlCodeBatch_MatchesSequentialImportStructureAndSql()
    {
        var requests = new[]
        {
            new SqlCodeImportRequest("CREATE VIEW dbo.v_first AS SELECT 1 AS Value;"),
            new SqlCodeImportRequest("CREATE VIEW dbo.v_second AS SELECT Value + 1 AS NextValue FROM dbo.v_first;")
        };
        var service = new MetaTransformScriptSqlService();

        var sequential = MetaTransformScriptModel.CreateEmpty();
        foreach (var request in requests)
        {
            service.ImportSqlCode(
                sequential,
                request.SqlCode,
                request.TargetSqlIdentifier,
                request.ScriptName);
        }

        var batched = MetaTransformScriptModel.CreateEmpty();
        service.ImportSqlCodeBatch(batched, requests);

        MetaTransformScriptTestHelper.AssertModelListCountsEqual(sequential, batched);
        MetaTransformScriptTestHelper.AssertModelListIdsEqual(sequential, batched);
        Assert.Equal(
            service.ExportModuleDefinitions(sequential),
            service.ExportModuleDefinitions(batched));
    }

    [Fact]
    public void ImportSqlCodeBatch_RollsBackOnlyTheFailingItem()
    {
        var service = new MetaTransformScriptSqlService();
        var expected = MetaTransformScriptModel.CreateEmpty();
        service.ImportSqlCode(expected, "CREATE VIEW dbo.v_first AS SELECT 1 AS Value;", null);

        var actual = MetaTransformScriptModel.CreateEmpty();
        Assert.Throws<MetaTransformScriptSqlImportException>(() =>
            service.ImportSqlCodeBatch(
                actual,
                [
                    new SqlCodeImportRequest("CREATE VIEW dbo.v_first AS SELECT 1 AS Value;"),
                    new SqlCodeImportRequest("CREATE VIEW dbo.v_broken AS SELECT 1 + FROM dbo.v_first;")
                ]));

        MetaTransformScriptTestHelper.AssertModelListCountsEqual(expected, actual);
        MetaTransformScriptTestHelper.AssertModelListIdsEqual(expected, actual);
        Assert.Equal(
            service.ExportModuleDefinitions(expected),
            service.ExportModuleDefinitions(actual));
    }

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
    [InlineData("069_leading_dot_numeric_literals.sql")]
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
        var parserScript = parserModel.TransformScriptList.Single();
        var serviceScript = serviceModel.TransformScriptList.Single();
        Assert.Equal(parserScript.Name, serviceScript.Name);
        Assert.Equal(GetViewTargetSqlIdentifier(parserModel, parserScript), GetViewTargetSqlIdentifier(serviceModel, serviceScript));
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
    public void ImportFromSqlCode_BareSelect_DoesNotAssignDefaultTargetSqlIdentifier()
    {
        const string sql = """
SELECT
    c.CustomerId
FROM sales.Customer AS c
""";

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_inline_target");
        var script = Assert.Single(model.TransformScriptList);
        Assert.Equal("dbo.v_inline_target", script.Name);
        Assert.True(string.IsNullOrWhiteSpace(GetViewTargetSqlIdentifier(model, script)));
    }

    [Theory]
    [InlineData("""
CREATE VIEW v_customer
AS
SELECT
    1 AS CustomerId
""")]
    [InlineData("""
CREATE VIEW Warehouse.dbo.v_customer
AS
SELECT
    1 AS CustomerId
""")]
    [InlineData("""
CREATE FUNCTION fn_customer()
RETURNS TABLE
AS
RETURN
SELECT
    1 AS CustomerId
""")]
    [InlineData("""
CREATE FUNCTION Warehouse.dbo.fn_customer()
RETURNS TABLE
AS
RETURN
SELECT
    1 AS CustomerId
""")]
    public void ImportFromSqlCode_RejectsCreateModuleNames_WithoutExactlyTwoParts(string sql)
    {
        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
        Assert.Contains("names must be two-part", exception.Message, StringComparison.OrdinalIgnoreCase);
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
        Assert.True(string.IsNullOrWhiteSpace(GetViewTargetSqlIdentifier(model, script)));
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
        Assert.True(IsInlineTableValuedFunction(model, script));
        Assert.True(string.IsNullOrWhiteSpace(GetViewTargetSqlIdentifier(model, script)));
        Assert.Empty(model.TransformScriptViewColumnsItemList);
        Assert.Equal(2, model.TransformScriptFunctionParametersItemList.Count);

        var parameterNames = model.TransformScriptFunctionParametersItemList
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => model.IdentifierList.Single(identifier => string.Equals(identifier.Id, item.Identifier.Id, StringComparison.Ordinal)).Value
                ?? throw new InvalidOperationException("Function parameter identifier is missing its value."))
            .ToArray();
        Assert.Equal(["@CustomerId", "@FromDate"], parameterNames);
    }

    [Fact]
    public async Task ImportSingleSqlFileToXmlWorkspaceAsync_CreateView_WithoutTarget_LeavesTargetBlank()
    {
        const string sql = """
CREATE VIEW dbo.v_customer
AS
SELECT
    c.CustomerId
FROM sales.Customer AS c
""";

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Script.Tests", Guid.NewGuid().ToString("N"));
        var sqlPath = MetaTransformScriptTestHelper.WriteTempSqlFile("view-without-target.sql", sql);
        var workspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            var result = await new MetaTransformScriptSqlService().ImportSingleSqlFileToXmlWorkspaceAsync(
                sqlPath,
                null,
                workspacePath);

            var script = Assert.Single(result.Model.TransformScriptList);
            Assert.True(string.IsNullOrWhiteSpace(GetViewTargetSqlIdentifier(result.Model, script)));
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
    public async Task ImportFromSqlCodeToXmlWorkspaceAsync_BareSelect_RequiresTarget()
    {
        const string sql = """
SELECT
    c.CustomerId
FROM sales.Customer AS c
""";

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Script.Tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            var service = new MetaTransformScriptSqlService();
            var exception = await Assert.ThrowsAsync<MetaTransformScriptSqlImportException>(() =>
                service.ImportFromSqlCodeToXmlWorkspaceAsync(sql, null, workspacePath, scriptName: "dbo.v_customer"));

            Assert.Equal(MetaTransformScriptSqlImportFailureKind.InvalidSqlInput, exception.Kind);
            Assert.Contains("requires --target", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("bare SELECT", exception.Message, StringComparison.OrdinalIgnoreCase);
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
    public async Task ImportSingleSqlFileToXmlWorkspaceAsync_CreateView_WithTarget_AssignsTargetSqlIdentifier()
    {
        const string sql = """
CREATE VIEW dbo.v_customer
AS
SELECT
    c.CustomerId
FROM sales.Customer AS c
""";

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Script.Tests", Guid.NewGuid().ToString("N"));
        var sqlPath = MetaTransformScriptTestHelper.WriteTempSqlFile("view-with-target.sql", sql);
        var workspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            var result = await new MetaTransformScriptSqlService().ImportSingleSqlFileToXmlWorkspaceAsync(
                sqlPath,
                "warehouse.CustomerLoad",
                workspacePath);

            var script = Assert.Single(result.Model.TransformScriptList);
            Assert.Equal("warehouse.CustomerLoad", GetViewTargetSqlIdentifier(result.Model, script));
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
    public async Task ImportSingleSqlFileToXmlWorkspaceAsync_InlineTvf_RejectsTarget()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("066_inline_tvf.sql");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Script.Tests", Guid.NewGuid().ToString("N"));
        var sqlPath = MetaTransformScriptTestHelper.WriteTempSqlFile("inline-tvf-reject-target.sql", sql);
        var workspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            var service = new MetaTransformScriptSqlService();
            var exception = await Assert.ThrowsAsync<MetaTransformScriptSqlImportException>(() =>
                service.ImportSingleSqlFileToXmlWorkspaceAsync(sqlPath, "warehouse.CustomerLoad", workspacePath));

            Assert.Equal(MetaTransformScriptSqlImportFailureKind.InvalidSqlInput, exception.Kind);
            Assert.Contains("does not allow --target", exception.Message, StringComparison.OrdinalIgnoreCase);
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
    public async Task ImportSingleSqlFileToXmlWorkspaceAsync_InlineTvf_AllowsNoTarget_AndRoundTrips()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("066_inline_tvf.sql");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Script.Tests", Guid.NewGuid().ToString("N"));
        var sqlPath = MetaTransformScriptTestHelper.WriteTempSqlFile("inline-tvf-no-target.sql", sql);
        var workspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            await new MetaTransformScriptSqlService().ImportSingleSqlFileToXmlWorkspaceAsync(
                sqlPath,
                null,
                workspacePath);

            var reloaded = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(workspacePath, searchUpward: false);
            var script = Assert.Single(reloaded.TransformScriptList);
            Assert.True(IsInlineTableValuedFunction(reloaded, script));
            Assert.True(string.IsNullOrWhiteSpace(GetViewTargetSqlIdentifier(reloaded, script)));
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
            .Select(link => model.IdentifierList.Single(identifier => string.Equals(identifier.Id, link.Identifier.Id, StringComparison.Ordinal)).Value)
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
            .Select(link => model.IdentifierList.Single(identifier => string.Equals(identifier.Id, link.Identifier.Id, StringComparison.Ordinal)).Value)
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
            .Select(link => model.IdentifierList.Single(identifier => string.Equals(identifier.Id, link.Identifier.Id, StringComparison.Ordinal)).Value)
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
            .Select(link => model.WindowDelimiterList.Single(windowDelimiter => string.Equals(windowDelimiter.Id, link.WindowDelimiter.Id, StringComparison.Ordinal)).WindowDelimiterType
                ?? throw new InvalidOperationException("Window delimiter is missing its type."))
            .OrderBy(static value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(["CurrentRow", "UnboundedPreceding"], topDelimiterTypes);

        var bottomDelimiterTypes = model.WindowFrameClauseBottomLinkList
            .Select(link => model.WindowDelimiterList.Single(windowDelimiter => string.Equals(windowDelimiter.Id, link.WindowDelimiter.Id, StringComparison.Ordinal)).WindowDelimiterType
                ?? throw new InvalidOperationException("Window delimiter is missing its type."))
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
            .Select(row => row.SqlDataTypeOption
                ?? throw new InvalidOperationException("SQL data type reference is missing its option."))
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
    public void ImportFromSqlCode_SupportsAllSanctionedSqlServerDataTypes_FromMetaDataType()
    {
        var sanctionedSqlServerTypeNames = MetaDataTypeInstance.BuiltIn.DataTypeList
            .Where(row =>
                string.Equals(row.DataTypeSystem.Id, "SqlServer", StringComparison.Ordinal) &&
                !string.IsNullOrWhiteSpace(row.Name))
            .Select(row => row.Name.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var projectionLines = sanctionedSqlServerTypeNames
            .Select(static (typeName, ordinal) => $"    CAST(s.ValueText AS {typeName}) AS TypeValue{ordinal}")
            .ToArray();
        var sql = """
CREATE VIEW dbo.v_sanctioned_types AS
SELECT
"""
            + string.Join("," + Environment.NewLine, projectionLines)
            + Environment.NewLine
            + """
FROM dbo.Source AS s;
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);

        Assert.Equal(sanctionedSqlServerTypeNames.Length, model.SqlDataTypeReferenceList.Count);

        var emitted = service.ExportToSqlCode(model);
        Assert.Contains("CAST(s.ValueText AS numeric)", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CAST(s.ValueText AS real)", emitted, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_MapsIntegerAliasToInt_AndEmitsCanonicalInt()
    {
        const string sql = """
CREATE VIEW dbo.v_integer_alias AS
SELECT
    CAST(s.CustomerId AS integer) AS CustomerIdAsInteger
FROM dbo.Source AS s
WHERE CAST(s.CustomerId AS integer) >= 0
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var sqlDataTypeOptions = model.SqlDataTypeReferenceList
            .Select(row => row.SqlDataTypeOption)
            .ToArray();

        Assert.All(sqlDataTypeOptions, option => Assert.Equal("Int", option));

        var emitted = service.ExportToSqlCode(model);
        Assert.Contains("CAST(s.CustomerId AS int)", emitted);
        Assert.DoesNotContain("AS integer", emitted, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_MapsSysnameAliasToNVarChar128_AndEmitsCanonicalType()
    {
        const string sql = """
CREATE VIEW dbo.v_sysname_alias AS
SELECT
    CAST(s.ObjectName AS sysname) AS ObjectName
FROM dbo.Source AS s
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);

        var sqlDataType = Assert.Single(model.SqlDataTypeReferenceList);
        Assert.Equal("NVarChar", sqlDataType.SqlDataTypeOption);

        var parameterized = Assert.Single(
            model.ParameterizedDataTypeReferenceList,
            row => string.Equals(row.Id, sqlDataType.ParameterizedDataTypeReference.Id, StringComparison.Ordinal));
        Assert.Single(
            model.ParameterizedDataTypeReferenceParametersItemList,
            row => string.Equals(row.ParameterizedDataTypeReference.Id, parameterized.Id, StringComparison.Ordinal));

        var emitted = service.ExportToSqlCode(model);
        Assert.Contains("CAST(s.ObjectName AS nvarchar(128))", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("AS sysname", emitted, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_MapsMultiTokenTypeAliases_ToCanonicalSqlServerTypes()
    {
        const string sql = """
CREATE VIEW dbo.v_multi_token_type_aliases AS
SELECT
    CAST(s.NameText AS character varying(40)) AS NameValue,
    CAST(s.CodeText AS char varying(10)) AS CodeValue,
    CAST(s.JsonText AS national character varying(max)) AS JsonValue,
    CAST(s.TagText AS national char varying(60)) AS TagValue,
    CAST(s.ShortCode AS national character(12)) AS ShortCodeValue,
    CAST(s.ShortName AS national char(8)) AS ShortNameValue,
    CAST(s.ScoreText AS double precision) AS ScoreValue,
    CAST(s.AmountText AS dec(18, 4)) AS AmountValue
FROM dbo.Source AS s
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var emitted = service.ExportToSqlCode(model);

        Assert.Contains("CAST(s.NameText AS varchar(40))", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CAST(s.CodeText AS varchar(10))", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CAST(s.JsonText AS nvarchar(max))", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CAST(s.TagText AS nvarchar(60))", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CAST(s.ShortCode AS nchar(12))", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CAST(s.ShortName AS nchar(8))", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CAST(s.ScoreText AS float)", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CAST(s.AmountText AS decimal(18, 4))", emitted, StringComparison.OrdinalIgnoreCase);

        Assert.DoesNotContain("character varying", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("char varying", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("national character varying", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("national char varying", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("double precision", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(" dec(", emitted, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlFile_ParsesInlineTvfParameters_WithMultiTokenTypeAliases()
    {
        const string sql = """
CREATE FUNCTION dbo.fn_alias_parameters
(
    @DisplayName national character varying(128),
    @ShortName national char varying(64),
    @Score double precision
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        @DisplayName AS DisplayName,
        @ShortName AS ShortName,
        @Score AS Score
)
""";

        var model = new MetaTransformScriptSqlService().ImportFromSqlFile(
            MetaTransformScriptTestHelper.WriteTempSqlFile("inline-tvf-alias-parameters.sql", sql));

        var script = Assert.Single(model.TransformScriptList);
        Assert.True(IsInlineTableValuedFunction(model, script));
        Assert.Equal(3, model.TransformScriptFunctionParametersItemList.Count);

        var dataTypeOptions = model.TransformScriptFunctionParametersItemList
            .Select(item =>
            {
                var parameterized = Assert.Single(
                    model.ParameterizedDataTypeReferenceList,
                    row => string.Equals(row.DataTypeReference.Id, item.DataTypeReference.Id, StringComparison.Ordinal));
                var sqlDataType = Assert.Single(
                    model.SqlDataTypeReferenceList,
                    row => string.Equals(row.ParameterizedDataTypeReference.Id, parameterized.Id, StringComparison.Ordinal));
                return sqlDataType.SqlDataTypeOption
                    ?? throw new InvalidOperationException("SQL data type reference is missing its option.");
            })
            .OrderBy(static value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(["Float", "NVarChar", "NVarChar"], dataTypeOptions);

        var nvarcharParameterizedReferenceIds = model.TransformScriptFunctionParametersItemList
            .Select(item => model.ParameterizedDataTypeReferenceList.Single(
                row => string.Equals(row.DataTypeReference.Id, item.DataTypeReference.Id, StringComparison.Ordinal)))
            .Where(parameterizedReference => model.SqlDataTypeReferenceList.Any(sqlDataType =>
                string.Equals(sqlDataType.ParameterizedDataTypeReference.Id, parameterizedReference.Id, StringComparison.Ordinal) &&
                string.Equals(sqlDataType.SqlDataTypeOption, "NVarChar", StringComparison.Ordinal)))
            .Select(parameterizedReference => parameterizedReference.Id)
            .OrderBy(static value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(2, nvarcharParameterizedReferenceIds.Length);
        Assert.All(
            nvarcharParameterizedReferenceIds,
            id => Assert.Single(
                model.ParameterizedDataTypeReferenceParametersItemList,
                row => string.Equals(row.ParameterizedDataTypeReference.Id, id, StringComparison.Ordinal)));
    }

    [Fact]
    public void ImportFromSqlCode_MaterializesRemainingAnalyticFunctions_AsGenericFunctionCallsWithOverClauses()
    {
        var sql = MetaTransformScriptTestHelper.LoadCorpus("060_remaining_analytic_functions.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_test");

        var functionNames = model.FunctionCallFunctionNameLinkList
            .Select(link => model.IdentifierList.Single(identifier => string.Equals(identifier.Id, link.Identifier.Id, StringComparison.Ordinal)).Value)
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

    public static IEnumerable<object[]> MutationStatementRoundTripCases()
    {
        yield return
        [
            "insert-values",
            """
INSERT INTO dbo.Target (Id, Name)
VALUES (1, 'A'), (2, 'B')
""",
            "INSERT INTO dbo.Target"
        ];

        yield return
        [
            "insert-select",
            """
WITH src AS
(
    SELECT
        s.Id
    FROM dbo.Source AS s
)
INSERT INTO dbo.Target (Id)
SELECT
    src.Id
FROM src
""",
            "WITH src AS"
        ];

        yield return
        [
            "update",
            """
UPDATE dbo.Target AS t
SET t.Name = s.Name, t.Score = s.Score + 1
FROM dbo.Source AS s
WHERE t.Id = s.Id
""",
            "UPDATE dbo.Target AS t"
        ];

        yield return
        [
            "delete",
            """
DELETE t
FROM dbo.Target AS t
INNER JOIN dbo.Source AS s
    ON t.Id = s.Id
WHERE s.IsDeleted = 1
""",
            "DELETE t"
        ];

        yield return
        [
            "truncate",
            """
TRUNCATE TABLE dbo.Target
""",
            "TRUNCATE TABLE dbo.Target"
        ];

        yield return
        [
            "merge",
            """
MERGE INTO dbo.Target AS t
USING dbo.Source AS s
ON t.Id = s.Id
WHEN MATCHED THEN UPDATE SET t.Name = s.Name
WHEN NOT MATCHED BY TARGET THEN INSERT (Id, Name) VALUES (s.Id, s.Name);
""",
            "MERGE INTO dbo.Target AS t"
        ];

        yield return
        [
            "merge-dw-corner-store",
            """
WITH source_rows AS
(
    SELECT
        src.BusinessKey,
        src.Col1,
        src.Col2,
        src.HashDiff,
        src.LoadDate
    FROM stg.SourceTable AS src
)
MERGE TOP (100) PERCENT INTO dbo.TargetTable WITH (HOLDLOCK) AS tgt
USING
(
    SELECT
        src.BusinessKey,
        src.Col1,
        src.Col2,
        src.HashDiff,
        src.LoadDate
    FROM source_rows AS src
) AS src
ON tgt.BusinessKey = src.BusinessKey
WHEN MATCHED AND tgt.HashDiff <> src.HashDiff THEN UPDATE SET
    tgt.Col1 = src.Col1,
    tgt.Col2 = src.Col2,
    tgt.HashDiff = src.HashDiff,
    tgt.ModifiedDate = src.LoadDate
WHEN NOT MATCHED BY TARGET THEN INSERT (BusinessKey, Col1, Col2, HashDiff, CreatedDate) VALUES (src.BusinessKey, src.Col1, src.Col2, src.HashDiff, src.LoadDate)
WHEN NOT MATCHED BY SOURCE THEN DELETE
OUTPUT
    $action AS MergeAction,
    inserted.BusinessKey AS InsertedBusinessKey,
    deleted.BusinessKey AS DeletedBusinessKey
OPTION (HASH JOIN, MAXDOP 4);
""",
            "OUTPUT"
        ];

        yield return
        [
            "merge-output-into",
            """
MERGE INTO dbo.Target AS t
USING dbo.Source AS s
ON t.Id = s.Id
WHEN MATCHED THEN UPDATE SET t.Name = s.Name
OUTPUT $action AS MergeAction, inserted.Id AS InsertedId
INTO audit.MergeLog (MergeAction, InsertedId);
""",
            "INTO audit.MergeLog"
        ];
    }

    [Theory]
    [MemberData(nameof(MutationStatementRoundTripCases))]
    public void ImportFromSqlCode_RoundTripsMutationStatements(string scriptName, string sql, string expectedFragment)
    {
        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql, scriptName);
        model = MetaTransformScriptTestHelper.RoundTripWorkspace(model, scriptName);

        var script = Assert.Single(model.TransformScriptList);
        Assert.Equal(scriptName, script.Name);
        Assert.Single(model.TransformScriptStatementLinkList);
        Assert.Empty(model.ScriptObjectViewList);
        Assert.Empty(model.ScriptObjectTVFList);

        var emitted = service.ExportToSqlCode(model);
        Assert.Contains(expectedFragment, emitted, StringComparison.OrdinalIgnoreCase);

        var reparsed = service.ImportFromSqlCode(emitted, scriptName);
        Assert.Equal(emitted, service.ExportToSqlCode(reparsed));
    }

    [Fact]
    public void ImportFromSqlCode_MergeModelsConcreteWhenFormsInClauseSequence()
    {
        const string sql = """
MERGE INTO dbo.Target AS t
USING dbo.Source AS s
ON t.Id = s.Id
WHEN MATCHED AND t.IsDeleted = 0 THEN UPDATE SET t.Name = s.Name
WHEN MATCHED THEN DELETE
WHEN NOT MATCHED BY TARGET THEN INSERT (Id, Name) VALUES (s.Id, s.Name)
WHEN NOT MATCHED BY SOURCE THEN DELETE;
""";

        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "merge-when-forms");
        model = MetaTransformScriptTestHelper.RoundTripWorkspace(model, "merge-when-forms");

        var items = model.MergeStatementWhenClausesItemList;
        Assert.Equal(4, items.Count);

        var first = Assert.Single(items, item => item.PreviousMergeWhenClause is null);
        var second = Assert.Single(items, item => item.PreviousMergeWhenClause?.Id == first.Id);
        var third = Assert.Single(items, item => item.PreviousMergeWhenClause?.Id == second.Id);
        var fourth = Assert.Single(items, item => item.PreviousMergeWhenClause?.Id == third.Id);

        Assert.Equal(2, model.MergeMatchedWhenClauseList.Count);
        Assert.Contains(model.MergeMatchedWhenClauseList, item => item.MergeWhenClause.Id == first.MergeWhenClause.Id);
        Assert.Contains(model.MergeMatchedWhenClauseList, item => item.MergeWhenClause.Id == second.MergeWhenClause.Id);
        Assert.Contains(model.MergeNotMatchedByTargetWhenClauseList, item => item.MergeWhenClause.Id == third.MergeWhenClause.Id);
        Assert.Contains(model.MergeNotMatchedBySourceWhenClauseList, item => item.MergeWhenClause.Id == fourth.MergeWhenClause.Id);
        Assert.DoesNotContain(model.MergeNotMatchedByTargetWhenClauseList, item => item.MergeWhenClause.Id == first.MergeWhenClause.Id);
        Assert.DoesNotContain(model.MergeNotMatchedBySourceWhenClauseList, item => item.MergeWhenClause.Id == first.MergeWhenClause.Id);
    }

    [Fact]
    public void ExportToSqlCode_RejectsBranchedMergeWhenClauseSequence()
    {
        const string sql = """
MERGE INTO dbo.Target AS t
USING dbo.Source AS s
ON t.Id = s.Id
WHEN MATCHED AND t.IsDeleted = 0 THEN UPDATE SET t.Name = s.Name
WHEN MATCHED THEN DELETE
WHEN NOT MATCHED BY TARGET THEN INSERT (Id, Name) VALUES (s.Id, s.Name);
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql, "merge-branched-when-clauses");
        var items = model.MergeStatementWhenClausesItemList;
        var first = Assert.Single(items, item => item.PreviousMergeWhenClause is null);
        var third = Assert.Single(items, item => item.PreviousMergeWhenClause is not null && item.PreviousMergeWhenClause.Id != first.Id);
        third.PreviousMergeWhenClause = first;

        var exception = Assert.Throws<InvalidOperationException>(() => service.ExportToSqlCode(model));

        Assert.Contains("branches", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_MergeRequiresSqlServerTerminatingSemicolon()
    {
        const string sql = """
MERGE INTO dbo.Target AS t
USING dbo.Source AS s
ON t.Id = s.Id
WHEN MATCHED THEN UPDATE SET t.Name = s.Name
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "merge-missing-semicolon"));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.ParseFailed, exception.Kind);
        Assert.Contains("semicolon", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_MergeRejectsRepeatedMatchedClausesWithSameAction()
    {
        const string sql = """
MERGE INTO dbo.Target AS t
USING dbo.Source AS s
ON t.Id = s.Id
WHEN MATCHED AND t.HashDiff <> s.HashDiff THEN UPDATE SET t.Name = s.Name
WHEN MATCHED THEN UPDATE SET t.Score = s.Score;
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "merge-invalid-repeated-matched"));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
        Assert.Contains("one must UPDATE and one must DELETE", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_MergeRejectsInvalidActionForNotMatchedByTarget()
    {
        const string sql = """
MERGE INTO dbo.Target AS t
USING dbo.Source AS s
ON t.Id = s.Id
WHEN NOT MATCHED BY TARGET THEN DELETE;
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "merge-invalid-target-action"));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
        Assert.Contains("INSERT actions only", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_RejectsMergeActionPseudoColumnOutsideMergeOutput()
    {
        const string sql = """
SELECT
    $action AS MergeAction
FROM dbo.Source AS s
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql, "dbo.v_invalid_action"));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.ParseFailed, exception.Kind);
        Assert.Contains("$action", exception.Message, StringComparison.OrdinalIgnoreCase);
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
    public void ImportFromSqlCode_ParsesBangNotEqual_AndEmitsCanonicalComparisonOperator()
    {
        const string sql = """
CREATE VIEW dbo.v_bang_not_equal AS
SELECT
    s.Period
FROM dbo.Source AS s
WHERE s.Status <> 0
  AND LEN(CAST(s.Period AS varchar(6))) != 6
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var emitted = service.ExportToSqlCode(model);

        Assert.Contains("s.Status <> 0", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("LEN(CAST(s.Period AS varchar(6))) <> 6", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("!=", emitted, StringComparison.Ordinal);
    }

    [Fact]
    public void ImportFromSqlCode_RejectsDirtyUnicodeComments()
    {
        var sql = "CREATE VIEW dbo.v_comment_noise AS" + Environment.NewLine
            + "-- line comment with \u00A4 \u00C2 \u00A7 mojibake \u0085"
            + "SELECT" + Environment.NewLine
            + "    s.Id AS OutputId" + Environment.NewLine
            + "FROM dbo.Source AS s" + Environment.NewLine
            + "/* block comment with \u00A4 \u00C2 \u00A7 and odd bytes \u001A \u0080 */" + Environment.NewLine
            + "WHERE s.Id = 1";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch, exception.Kind);
    }

    [Fact]
    public void ImportFromSqlCode_RejectsMojibakeNonBreakingSpaceIndentation()
    {
        const string mojibakeNbsp = "\u00C2\u00A0";
        var sql = "CREATE VIEW dbo.v_mojibake_nbsp_indent AS" + Environment.NewLine
            + "SELECT" + Environment.NewLine
            + "    CASE WHEN SourceCode IN (SELECT Code FROM Staging.FilterValue WHERE FilterCode = '92') THEN 1 END AS FlagA,"
            + Environment.NewLine
            + mojibakeNbsp + mojibakeNbsp + "  -- dirty exported indentation before a comment" + Environment.NewLine
            + "    CASE WHEN SourceCode IN (SELECT Code FROM Staging.FilterValue WHERE FilterCode = '93') THEN 1 END AS FlagB"
            + Environment.NewLine
            + "FROM dbo.Source AS s";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch, exception.Kind);
    }

    [Fact]
    public void ImportFromSqlCode_RejectsOrphanMojibakeNonBreakingSpaceLeadIndentation()
    {
        const string orphanMojibakeNbspLead = "\u00C2 ";
        var sql = "CREATE VIEW Staging.DirtyIndent_TargetView AS" + Environment.NewLine
            + "SELECT" + Environment.NewLine
            + "SourceCode," + Environment.NewLine
            + "--1000 Flag A" + Environment.NewLine
            + "CASE WHEN " + Environment.NewLine
            + "/* Filter: 89 DirtyIndent FlagA /" + Environment.NewLine
            + "/ Expression: SourceCode = 1000 */" + Environment.NewLine
            + "SourceCode IN (SELECT Code FROM Staging.FilterValue WHERE FilterCode = '89') THEN 1 END AS FlagA,"
            + Environment.NewLine
            + orphanMojibakeNbspLead + orphanMojibakeNbspLead + " -- dirty exported indentation before a comment" + Environment.NewLine
            + "CASE WHEN SourceCode IN (SELECT Code FROM Staging.FilterValue WHERE FilterCode = '92') THEN 1 END AS FlagB"
            + Environment.NewLine
            + "FROM Staging.Source";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch, exception.Kind);
    }

    [Fact]
    public void ImportFromSqlCode_RejectsMojibakeIdentifiers()
    {
        const char continuationNoBreakSpace = '\u00A0';
        const char continuationPoundSign = '\u00A3';
        const char continuationMacron = '\u00AF';
        var sql = $"""
CREATE VIEW dbo.v_mojibake_identifiers AS
SELECT
    s.Legacy¤Code AS LegacyCode,
    s.NÃ{continuationNoBreakSpace}raRiskId AS NaraGraveRiskId,
    s.NÃ¤raRelationRiskId AS NaraRelationRiskId,
    s.NÃ¥gonRiskId AS NagonRiskId,
    s.NÃ¶jdRiskId AS NojdRiskId,
    s.NÃ{continuationMacron}veRiskId AS NaiveRiskId,
    s.Ã…rKod AS ArKod,
    s.Ã„gareKod AS AgareKod,
    s.Ã–ppenKod AS OppenKod,
    s.KodÂ{continuationPoundSign}Belopp AS KodPundBelopp,
    s.PrÃ¸veKod AS ProveKod,
    s.Kod§Varde AS KodVarde,
    s.[Falt ¤ Â §] AS [Output ¤ Â §]
FROM dbo.[Source ¤ Â §] AS s
WHERE s.Legacy¤Code <> 0
  AND s.NÃ¤raRelationRiskId <> 0
""";

        _ = continuationNoBreakSpace;
        _ = continuationPoundSign;
        _ = continuationMacron;
        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch, exception.Kind);
    }

    [Fact]
    public void ImportFromSqlCode_StillRejectsControlCharactersOutsideCommentsAndIdentifiers()
    {
        var sql = "CREATE VIEW dbo.v_bad_control AS" + Environment.NewLine
            + "SELECT" + Environment.NewLine
            + "    s.Id" + '\u001A' + " AS OutputId" + Environment.NewLine
            + "FROM dbo.Source AS s";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.ParseFailed, exception.Kind);
        Assert.Contains("Unexpected character", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_ParsesNamedTableHints_InFromJoinAndCteQueries()
    {
        const string sql = """
CREATE VIEW dbo.v_table_hints AS
WITH src AS
(
    SELECT
        d.DatumId
    FROM Staging.Datum AS d WITH(NOLOCK)
    JOIN [Management].Incremental.PreparedReadDates AS prd WITH (NOLOCK, INDEX(SomeIndex))
        ON prd.DatumId = d.DatumId
)
SELECT
    src.DatumId
FROM Warehouse.CoreDim AS cd WITH (FORCESEEK)
JOIN src
    ON src.DatumId = cd.DatumId
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var emitted = service.ExportToSqlCode(model);

        Assert.Contains("Staging.Datum AS d WITH (NOLOCK)", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("PreparedReadDates AS prd WITH (NOLOCK, INDEX (SomeIndex))", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Warehouse.CoreDim AS cd WITH (FORCESEEK)", emitted, StringComparison.OrdinalIgnoreCase);

        var reparsed = service.ImportFromSqlCode(emitted, "dbo.v_table_hints");
        Assert.Equal(emitted, service.ExportToSqlCode(reparsed));
    }

    [Fact]
    public void ImportFromSqlCode_SupportsRequestedSqlServerTableHintKeywords()
    {
        const string sql = """
CREATE VIEW dbo.v_table_hint_keywords AS
SELECT
    s.Id
FROM dbo.Source AS s WITH (NOLOCK, FORCESEEK, HOLDLOCK, UPDLOCK, READPAST, TABLOCK, TABLOCKX, ROWLOCK, PAGLOCK, XLOCK)
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var emitted = service.ExportToSqlCode(model);

        Assert.Contains("WITH (NOLOCK, FORCESEEK, HOLDLOCK, UPDLOCK, READPAST, TABLOCK, TABLOCKX, ROWLOCK, PAGLOCK, XLOCK)", emitted, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_ParsesSingleQuotedSelectAliases_AndEmitsCanonicalAliasSyntax()
    {
        const string sql = """
CREATE VIEW dbo.v_single_quoted_aliases AS
SELECT
    dvkat1.dim_value AS 'Funktyp1Kod',
    dvkat1.description 'Funktyp1Namn',
    dvkat1.description AS [Funktyp1Beskrivning],
    dvkat1.dim_value AS Funktyp1KodSafe
FROM dbo.DimensionValue AS dvkat1
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var emitted = service.ExportToSqlCode(model);

        Assert.Contains("AS Funktyp1Kod", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("AS Funktyp1Namn", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("AS 'Funktyp1Kod'", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("description 'Funktyp1Namn'", emitted, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_ParsesUnicodeStringLiterals_InDerivedUnionTable()
    {
        const string sql = """
SELECT *
FROM
(
    SELECT N'104' AS Code, -1 AS TagId
    UNION
    SELECT N'720' AS Code, -1 AS TagId
) A
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql, "dbo.v_union_literal_mapping");
        var emitted = service.ExportToSqlCode(model);

        Assert.Contains("N'104' AS Code", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("N'720' AS Code", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("UNION", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(") AS A", emitted, StringComparison.OrdinalIgnoreCase);

        var reparsed = service.ImportFromSqlCode(emitted, "dbo.v_union_literal_mapping");
        Assert.Equal(emitted, service.ExportToSqlCode(reparsed));
    }

    [Fact]
    public void ImportFromSqlCode_ParsesCreateView_WithDerivedUnionLiteralMapping()
    {
        const string sql = """
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [Staging].[LiteralMapping_TargetView] AS
SELECT
    CAST(NEWID() AS VARCHAR(50)) AS BK,
    Code,
    TagId
FROM
(
    SELECT N'104' AS Code, -1 AS TagId
    UNION
    SELECT N'720' AS Code, -1 AS TagId
) A
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var emitted = service.ExportToSqlCode(model);
        var script = Assert.Single(model.TransformScriptList);

        Assert.Contains("LiteralMapping_TargetView", script.Name, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CAST(NEWID() AS varchar(50)) AS BK", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("N'104' AS Code", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("N'720' AS Code", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("UNION", emitted, StringComparison.OrdinalIgnoreCase);

        var reparsed = service.ImportFromSqlCode(emitted, script.Name);
        Assert.Equal(emitted, service.ExportToSqlCode(reparsed));
    }

    [Fact]
    public void ImportFromSqlCode_ParsesUnicodeStringLiterals_InPredicatesAndCase()
    {
        const string sql = """
CREATE VIEW dbo.v_unicode_literals AS
SELECT
    CASE WHEN src.Code = N'x' THEN N'y' ELSE N'z' END AS MappedCode
FROM dbo.Source AS src
WHERE src.Code = N'x'
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var emitted = service.ExportToSqlCode(model);
        var script = Assert.Single(model.TransformScriptList);

        Assert.Contains("src.Code = N'x'", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("THEN N'y'", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ELSE N'z'", emitted, StringComparison.OrdinalIgnoreCase);

        var reparsed = service.ImportFromSqlCode(emitted, script.Name);
        Assert.Equal(emitted, service.ExportToSqlCode(reparsed));
    }

    [Fact]
    public void ImportFromSqlCode_AllowsTableValuedFunctionReferences_WithoutAlias()
    {
        const string sql = """
CREATE VIEW dbo.v_tvf_without_alias AS
SELECT
    Item
FROM dbo.fnSplitStrings('A}{B}{C', '}{', 1)
WHERE Item LIKE '%A%'
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var emitted = service.ExportToSqlCode(model);

        Assert.Contains("FROM dbo.fnSplitStrings('A}{B}{C', '}{', 1)", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("AS fs", emitted, StringComparison.OrdinalIgnoreCase);

        var reparsed = service.ImportFromSqlCode(emitted, "dbo.v_tvf_without_alias");
        Assert.Equal(emitted, service.ExportToSqlCode(reparsed));
    }

    [Fact]
    public void ImportFromSqlCode_StillRequiresAliases_ForDerivedTables()
    {
        const string sql = """
CREATE VIEW dbo.v_missing_derived_alias AS
SELECT
    q.Id
FROM
(
    SELECT
        s.Id
    FROM dbo.Source AS s
)
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.ParseFailed, exception.Kind);
        Assert.Contains("Derived table references require an alias", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_MapsTimestampAndRowversionToCanonicalBinary8()
    {
        const string sql = """
CREATE VIEW dbo.v_rowversion_aliases AS
SELECT
    CAST(s.Payload AS TIMESTAMP) AS PayloadTimestamp,
    CAST(s.Payload AS ROWVERSION) AS PayloadRowVersion,
    CAST(CAST(s.CreatedAt AS DATETIME) AS TIMESTAMP) AS NestedTimestamp
FROM dbo.Source AS s
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var emitted = service.ExportToSqlCode(model);

        Assert.Contains("CAST(s.Payload AS binary(8)) AS PayloadTimestamp", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CAST(s.Payload AS binary(8)) AS PayloadRowVersion", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CAST(CAST(s.CreatedAt AS datetime) AS binary(8)) AS NestedTimestamp", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(" AS TIMESTAMP", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(" AS ROWVERSION", emitted, StringComparison.OrdinalIgnoreCase);
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
        SetViewTargetSqlIdentifier(model, script, "reporting.v_overridden_target");

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
    public async Task ExportModuleDefinitionsAndSqlFiles_UseStableModuleIdentities()
    {
        const string firstSql = """
CREATE VIEW dbo.v_zebra
AS
SELECT
    1 AS Id
""";
        const string secondSql = """
CREATE VIEW dbo.v_alpha
AS
SELECT
    2 AS Id
""";

        var service = new MetaTransformScriptSqlService();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Script.Tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var outputPath = Path.Combine(tempRoot, "SqlFiles");

        try
        {
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(firstSql, targetSqlIdentifier: null, workspacePath);
            await service.AddSqlCodeToWorkspaceAsync(secondSql, targetSqlIdentifier: null, workspacePath);

            var modules = service.ExportModuleDefinitions(workspacePath);

            Assert.Equal(["v_alpha", "v_zebra"], modules.Select(item => item.ObjectName));
            Assert.Equal([1, 2], modules.Select(item => item.DeployOrdinal));

            await service.ExportToSqlPathAsync(workspacePath, outputPath);

            Assert.True(File.Exists(Path.Combine(outputPath, "views", "dbo", "v_alpha.sql")));
            Assert.True(File.Exists(Path.Combine(outputPath, "views", "dbo", "v_zebra.sql")));
            Assert.Empty(Directory.EnumerateFiles(outputPath, "view_*.sql", SearchOption.AllDirectories));
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

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedFunctionWrapper, exception.Kind);
        Assert.Contains("Multistatement table-valued CREATE FUNCTION wrappers are not supported", exception.Message);
    }

    [Fact]
    public void ImportFromSqlCode_ParsesScalarUdf_ReturnExpression()
    {
        const string sql = """
CREATE FUNCTION [dbo].[fnAddOne]
(
    @value INT
)
RETURNS INT
AS
BEGIN
    RETURN @value + 1;
END
GO
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var emitted = service.ExportToSqlCode(model);

        Assert.Single(model.ScriptObjectScalarFunctionList);
        Assert.Empty(model.TransformScriptStatementLinkList);
        Assert.Contains("CREATE FUNCTION [dbo].[fnAddOne]", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("@value int", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("RETURNS int", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("RETURN @value + 1", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("CREATE VIEW", emitted, StringComparison.OrdinalIgnoreCase);

        var reparsed = service.ImportFromSqlCode(emitted);
        Assert.Equal(emitted, service.ExportToSqlCode(reparsed));
    }

    [Fact]
    public void ImportFromSqlCode_ParsesScalarUdf_ReturnSelectSubquery()
    {
        const string sql = """
CREATE FUNCTION dbo.fn_customer_order_count
(
    @CustomerId int
)
RETURNS bigint
AS
BEGIN
    RETURN
    (
        SELECT COUNT_BIG(*)
        FROM dbo.[Order] AS o
        WHERE o.CustomerId = @CustomerId
    );
END
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        var emitted = service.ExportToSqlCode(model);

        Assert.Single(model.ScriptObjectScalarFunctionList);
        Assert.Contains("CREATE FUNCTION dbo.fn_customer_order_count", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("RETURNS bigint", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("COUNT_BIG(*)", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("FROM dbo.[Order] AS o", emitted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("o.CustomerId = @CustomerId", emitted, StringComparison.OrdinalIgnoreCase);

        var reparsed = service.ImportFromSqlCode(emitted);
        Assert.Equal(emitted, service.ExportToSqlCode(reparsed));
    }

    [Fact]
    public void ImportFromSqlCode_FailsExplicitly_ForUnsupportedProceduralScalarUdfWrapper()
    {
        const string sql = """
CREATE FUNCTION [dbo].[fnTidBK]
(
    @dt DATETIME
)
RETURNS VARCHAR(25)
AS
BEGIN
    DECLARE @seconds INT;

    SET @seconds =
        (
            (
                DATEPART(HOUR,   ISNULL(@dt, '1900-01-01'))
              * 60
              + DATEPART(MINUTE, ISNULL(@dt, '1900-01-01'))
            ) * 60
            + DATEPART(SECOND, ISNULL(@dt, '1900-01-01'))
        );

    IF @seconds = 0
        SET @seconds = 86400;

    RETURN CONVERT(VARCHAR(25), @seconds);
END
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedFunctionWrapper, exception.Kind);
        Assert.Contains("Scalar CREATE FUNCTION bodies are supported only", exception.Message);
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
        Assert.True(string.IsNullOrWhiteSpace(GetViewTargetSqlIdentifier(model, script)));
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

    private static string? GetViewTargetSqlIdentifier(MetaTransformScriptModel model, TransformScript script)
    {
        return model.ScriptObjectViewList.SingleOrDefault(item =>
                string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal))
            ?.TargetSqlIdentifier;
    }

    private static bool IsInlineTableValuedFunction(MetaTransformScriptModel model, TransformScript script)
    {
        return model.ScriptObjectTVFList.Any(item =>
            string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
    }

    private static void SetViewTargetSqlIdentifier(
        MetaTransformScriptModel model,
        TransformScript script,
        string targetSqlIdentifier)
    {
        model.ScriptObjectTVFList.RemoveAll(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));

        var scriptObjectView = model.ScriptObjectViewList.SingleOrDefault(item =>
            string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
        if (scriptObjectView is null)
        {
            model.ScriptObjectViewList.Add(new ScriptObjectView
            {
                Id = Guid.NewGuid().ToString("N"),
                TransformScript = script,
                TargetSqlIdentifier = targetSqlIdentifier
            });
            return;
        }

        scriptObjectView.TargetSqlIdentifier = targetSqlIdentifier;
    }
}
