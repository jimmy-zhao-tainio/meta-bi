using Meta.Integration;
using Meta.Operations.Domain;
using MetaBi.Tests.Common;
using MetaConvert.TransformScriptToSql;
using MetaSql;
using MetaTransformScript;
using MetaTransformScript.Sql;
using MetaWeave.Core;
using MetaWeaveScript.Execution;
using MetaWeaveScript.Sql;

public sealed class TransformScriptToSqlWeaveTests
{
    [Fact]
    public async Task ForwardWeave_ModuleShellMatchesEstablishedConverter()
    {
        var source = CreateModuleWorkspace();
        var expected = TransformScriptToSqlCSharpReference.Convert(
            new MetaTransformScriptSqlService().ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            expected.DatabaseList
                .OrderBy(item => item.Id, StringComparer.Ordinal)
                .Select(item => (item.Id, item.Name, item.Collation)),
            actual.DatabaseList
                .OrderBy(item => item.Id, StringComparer.Ordinal)
                .Select(item => (item.Id, item.Name, item.Collation)));
        Assert.Equal(
            expected.SchemaList
                .OrderBy(item => item.Id, StringComparer.Ordinal)
                .Select(item => (item.Id, item.Name, DatabaseId: item.Database.Id)),
            actual.SchemaList
                .OrderBy(item => item.Id, StringComparer.Ordinal)
                .Select(item => (item.Id, item.Name, DatabaseId: item.Database.Id)));
        Assert.Equal(
            expected.StoredProcedureList
                .OrderBy(item => item.Id, StringComparer.Ordinal)
                .Select(item =>
                    (item.Id, item.Name, SchemaId: item.Schema.Id, item.DefinitionSql, item.DeployOrdinal)),
            actual.StoredProcedureList
                .OrderBy(item => item.Id, StringComparer.Ordinal)
                .Select(item =>
                    (item.Id, item.Name, SchemaId: item.Schema.Id, item.DefinitionSql, item.DeployOrdinal)));
        Assert.Equal(
            expected.ViewList
                .OrderBy(item => item.Id, StringComparer.Ordinal)
                .Select(item =>
                    (item.Id, item.Name, SchemaId: item.Schema.Id, DefinitionSql: CanonicalSql(item.DefinitionSql), item.DeployOrdinal)),
            actual.ViewList
                .OrderBy(item => item.Id, StringComparer.Ordinal)
                .Select(item =>
                    (item.Id, item.Name, SchemaId: item.Schema.Id, DefinitionSql: CanonicalSql(item.DefinitionSql), item.DeployOrdinal)));
        Assert.Equal(
            expected.FunctionList
                .OrderBy(item => item.Id, StringComparer.Ordinal)
                .Select(item =>
                    (item.Id, item.Name, SchemaId: item.Schema.Id, DefinitionSql: CanonicalSql(item.DefinitionSql), item.DeployOrdinal, item.FunctionKind)),
            actual.FunctionList
                .OrderBy(item => item.Id, StringComparer.Ordinal)
                .Select(item =>
                    (item.Id, item.Name, SchemaId: item.Schema.Id, DefinitionSql: CanonicalSql(item.DefinitionSql), item.DeployOrdinal, item.FunctionKind)));
    }

    [Fact]
    public async Task ForwardWeave_RendersScalarLeavesLikeEstablishedConverter()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            """
            CREATE VIEW [stage].[vScalarLeaves]
            AS
            SELECT
                .5 AS Half,
                .5E2 AS ExponentUpper,
                .5e-2 AS ExponentLower,
                -.5 AS NegativeHalf,
                +.5 AS PositiveHalf,
                N'O''Brien' AS [Display Name],
                0xAB AS BinaryValue,
                NULL AS MissingValue,
                @@SPID AS ServerProcessId,
                (1 + 2) * 3 AS ArithmeticValue
            """);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            new MetaTransformScriptSqlService().ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        var expectedView = Assert.Single(expected.ViewList);
        var actualView = Assert.Single(actual.ViewList);
        Assert.Equal(expectedView.Id, actualView.Id);
        Assert.Equal(expectedView.Name, actualView.Name);
        Assert.Equal(expectedView.Schema.Id, actualView.Schema.Id);
        Assert.Equal(CanonicalSql(expectedView.DefinitionSql), CanonicalSql(actualView.DefinitionSql));
        Assert.Equal(expectedView.DeployOrdinal, actualView.DeployOrdinal);
    }

    [Fact]
    public async Task ForwardWeave_RendersCaseExpressionsLikeEstablishedConverter()
    {
        var service = new MetaTransformScriptSqlService();
        var source = service.ImportFromSqlCode(
            """
            CREATE VIEW reporting.vCaseExpressions
            AS
            SELECT
                CASE s.StatusCode
                    WHEN 1 THEN 'Open'
                    WHEN 2 THEN 'Closed'
                    ELSE 'Unknown'
                END AS StatusName,
                CASE
                    WHEN s.Amount >= 100 THEN 'Large'
                    WHEN s.Amount > 0 THEN 'Small'
                    ELSE 'None'
                END AS AmountBand
            FROM dbo.Source AS s
            """);
        service.ImportSqlCode(
            source,
            """
            CREATE FUNCTION util.fnAmountBand(@value int)
            RETURNS nvarchar(10)
            AS
            BEGIN
                RETURN CASE
                    WHEN @value >= 100 THEN 'Large'
                    WHEN @value > 0 THEN 'Small'
                    ELSE 'None'
                END;
            END
            """,
            targetSqlIdentifier: null);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            service.ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        var expectedView = CanonicalSql(Assert.Single(expected.ViewList).DefinitionSql);
        var actualView = CanonicalSql(Assert.Single(actual.ViewList).DefinitionSql);
        Assert.True(
            string.Equals(expectedView, actualView, StringComparison.Ordinal),
            $"Expected view SQL:{Environment.NewLine}{expectedView}{Environment.NewLine}Actual view SQL:{Environment.NewLine}{actualView}");
        var expectedFunction = CanonicalSql(Assert.Single(expected.FunctionList).DefinitionSql);
        var actualFunction = CanonicalSql(Assert.Single(actual.FunctionList).DefinitionSql);
        Assert.True(
            string.Equals(expectedFunction, actualFunction, StringComparison.Ordinal),
            $"Expected function SQL:{Environment.NewLine}{expectedFunction}{Environment.NewLine}Actual function SQL:{Environment.NewLine}{actualFunction}");
    }

    [Fact]
    public async Task ForwardWeave_RendersTypeConversionCallsLikeEstablishedConverter()
    {
        var service = new MetaTransformScriptSqlService();
        var source = service.ImportFromSqlCode(
            """
            CREATE VIEW reporting.vTypeConversions
            AS
            SELECT
                CAST(s.Amount AS decimal(18, 4)) AS CastAmount,
                TRY_CAST(s.AmountText AS decimal(18, 4)) AS TryCastAmount,
                CONVERT(varchar(30), s.CreatedAt, 126) AS CreatedAtText,
                TRY_CONVERT(datetime2, s.CreatedAtText, 126) AS TryConvertedAt
            FROM dbo.Source AS s
            """);
        service.ImportSqlCode(
            source,
            """
            CREATE FUNCTION util.fnDecimal(@value int)
            RETURNS decimal(18, 4)
            AS
            BEGIN
                RETURN CAST(@value AS decimal(18, 4));
            END
            """,
            targetSqlIdentifier: null);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            service.ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            CanonicalSql(Assert.Single(expected.ViewList).DefinitionSql),
            CanonicalSql(Assert.Single(actual.ViewList).DefinitionSql));
        Assert.Equal(
            CanonicalSql(Assert.Single(expected.FunctionList).DefinitionSql),
            CanonicalSql(Assert.Single(actual.FunctionList).DefinitionSql));
    }

    [Fact]
    public async Task ForwardWeave_RendersScalarCallsLikeEstablishedConverter()
    {
        var service = new MetaTransformScriptSqlService();
        var source = service.ImportFromSqlCode(
            """
            CREATE VIEW reporting.vScalarCalls
            AS
            SELECT
                PARSE('12' AS int) AS ParsedValue,
                TRY_PARSE('31/12/2025' AS date USING N'en-GB') AS ParsedDate,
                LEFT(s.Name, 3) AS NamePrefix,
                RIGHT(s.Name, 2) AS NameSuffix,
                s.CreatedAt AT TIME ZONE 'UTC' AS ZonedAt,
                NEXT VALUE FOR dbo.OutputSequence AS NextId,
                CURRENT_TIMESTAMP AS ObservedAt,
                ABS(s.Amount) AS AbsoluteAmount,
                COUNT(DISTINCT s.Id) AS DistinctIds,
                dbo.fnScore(s.Amount) AS Score
            FROM dbo.Source AS s
            """);
        service.ImportSqlCode(
            source,
            """
            CREATE FUNCTION util.fnPrefix(@value nvarchar(30))
            RETURNS nvarchar(3)
            AS
            BEGIN
                RETURN LEFT(@value, 3);
            END
            """,
            targetSqlIdentifier: null);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            service.ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            CanonicalSql(Assert.Single(expected.ViewList).DefinitionSql),
            CanonicalSql(Assert.Single(actual.ViewList).DefinitionSql));
        Assert.Equal(
            CanonicalSql(Assert.Single(expected.FunctionList).DefinitionSql),
            CanonicalSql(Assert.Single(actual.FunctionList).DefinitionSql));
    }

    [Fact]
    public async Task ForwardWeave_RendersOrderedAndWindowCallsLikeEstablishedConverter()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            """
            CREATE VIEW reporting.vWindowCalls
            AS
            SELECT
                ROW_NUMBER() OVER (PARTITION BY s.GroupId ORDER BY s.Id DESC) AS RowNumber,
                SUM(s.Amount) OVER (
                    PARTITION BY s.GroupId
                    ORDER BY s.Id ASC
                    ROWS BETWEEN 1 PRECEDING AND CURRENT ROW
                ) AS RunningAmount,
                STRING_AGG(s.Name, ',') WITHIN GROUP (ORDER BY s.Name ASC) AS Names,
                PERCENTILE_CONT(.5) WITHIN GROUP (ORDER BY s.Amount) OVER (PARTITION BY s.GroupId) AS MedianAmount
            FROM dbo.Source AS s
            """);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            new MetaTransformScriptSqlService().ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            CanonicalSql(Assert.Single(expected.ViewList).DefinitionSql),
            CanonicalSql(Assert.Single(actual.ViewList).DefinitionSql));
    }

    [Fact]
    public async Task ForwardWeave_RendersRecursiveCtesLikeEstablishedConverter()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            """
            CREATE VIEW reporting.vHierarchy
            AS
            WITH roots (Id, ParentId, Depth) AS
            (
                SELECT
                    n.Id AS Id,
                    n.ParentId AS ParentId,
                    0 AS Depth
                FROM dbo.Node AS n
                WHERE n.ParentId IS NULL
            ),
            walk (Id, ParentId, Depth) AS
            (
                SELECT
                    r.Id AS Id,
                    r.ParentId AS ParentId,
                    r.Depth AS Depth
                FROM roots AS r
                UNION ALL
                SELECT
                    n.Id AS Id,
                    n.ParentId AS ParentId,
                    w.Depth + 1 AS Depth
                FROM dbo.Node AS n
                INNER JOIN walk AS w ON w.Id = n.ParentId
            )
            SELECT
                w.Id AS Id,
                w.ParentId AS ParentId,
                w.Depth AS Depth
            FROM walk AS w
            """);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            new MetaTransformScriptSqlService().ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            CanonicalSql(Assert.Single(expected.ViewList).DefinitionSql),
            CanonicalSql(Assert.Single(actual.ViewList).DefinitionSql));
    }

    [Fact]
    public async Task ForwardWeave_RendersXmlNamespacesWithCtesLikeEstablishedConverter()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            """
            CREATE VIEW reporting.vXmlValues
            AS
            WITH XMLNAMESPACES ('urn:test' AS ns, DEFAULT 'urn:default'),
            source_rows AS
            (
                SELECT
                    s.Id AS Id,
                    s.XmlPayload AS XmlPayload
                FROM dbo.XmlSource AS s
            )
            SELECT
                s.Id AS Id,
                s.XmlPayload.value('(/ns:Root/Id/text())[1]', 'int') AS XmlId
            FROM source_rows AS s
            """);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            new MetaTransformScriptSqlService().ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            CanonicalSql(Assert.Single(expected.ViewList).DefinitionSql),
            CanonicalSql(Assert.Single(actual.ViewList).DefinitionSql));
    }

    [Fact]
    public async Task ForwardWeave_RendersSubqueriesAndPredicatesLikeEstablishedConverter()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            """
            CREATE VIEW reporting.vPredicateCoverage
            AS
            SELECT
                s.Id AS Id,
                (SELECT MAX(x.Amount) FROM dbo.Other AS x WHERE x.SourceId = s.Id) AS MaximumAmount
            FROM dbo.Source AS s
            WHERE s.Amount BETWEEN 1 AND 100
                AND s.StatusCode NOT BETWEEN 7 AND 9
                AND s.CategoryId IN (1, 2, 3)
                AND s.Id NOT IN (SELECT x.SourceId FROM dbo.Excluded AS x)
                AND s.Amount > ALL (SELECT x.Amount FROM dbo.Threshold AS x)
                AND s.Name LIKE 'A!_%' ESCAPE '!'
                AND s.LeftValue IS DISTINCT FROM s.RightValue
                AND EXISTS (SELECT 1 AS Value FROM dbo.Related AS r WHERE r.SourceId = s.Id)
            """);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            new MetaTransformScriptSqlService().ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            CanonicalSql(Assert.Single(expected.ViewList).DefinitionSql),
            CanonicalSql(Assert.Single(actual.ViewList).DefinitionSql));
    }

    [Fact]
    public async Task ForwardWeave_RendersQueryClausesLikeEstablishedConverter()
    {
        var service = new MetaTransformScriptSqlService();
        var source = service.ImportFromSqlCode(
            """
            CREATE VIEW reporting.vGroupedAmounts
            AS
            SELECT TOP (10) PERCENT WITH TIES
                s.CategoryId AS CategoryId,
                SUM(s.Amount) AS TotalAmount
            FROM dbo.Source AS s
            GROUP BY s.CategoryId
            HAVING SUM(s.Amount) > 0
            ORDER BY SUM(s.Amount) DESC
            """);
        service.ImportSqlCode(
            source,
            """
            CREATE VIEW reporting.vPagedSource
            AS
            SELECT
                s.Id AS Id,
                s.Name AS Name
            FROM dbo.Source AS s
            ORDER BY s.Id ASC
            OFFSET 5 ROWS FETCH NEXT 10 ROWS ONLY
            """,
            targetSqlIdentifier: null);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            service.ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            expected.ViewList
                .OrderBy(item => item.Id, StringComparer.Ordinal)
                .Select(item => CanonicalSql(item.DefinitionSql)),
            actual.ViewList
                .OrderBy(item => item.Id, StringComparer.Ordinal)
                .Select(item => CanonicalSql(item.DefinitionSql)));
    }

    [Fact]
    public async Task ForwardWeave_RendersNamedWindowsLikeEstablishedConverter()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            """
            CREATE VIEW reporting.vNamedWindows
            AS
            SELECT
                SUM(s.Amount) OVER BaseWindow AS GroupAmount,
                AVG(s.Amount) OVER FramedWindow AS MovingAverage
            FROM dbo.Source AS s
            WINDOW
                BaseWindow AS (PARTITION BY s.GroupId ORDER BY s.Id ASC),
                FramedWindow AS (BaseWindow ROWS BETWEEN 2 PRECEDING AND CURRENT ROW)
            """);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            new MetaTransformScriptSqlService().ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            CanonicalSql(Assert.Single(expected.ViewList).DefinitionSql),
            CanonicalSql(Assert.Single(actual.ViewList).DefinitionSql));
    }

    [Fact]
    public async Task ForwardWeave_RendersDerivedAndFunctionRowsetsLikeEstablishedConverter()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            """
            CREATE VIEW reporting.vDerivedRowsets
            AS
            SELECT
                d.Id AS Id,
                f.Value AS FunctionValue,
                v.Label AS Label,
                n.Item AS XmlItem
            FROM
            (
                SELECT
                    s.Id AS Id,
                    s.XmlPayload AS XmlPayload
                FROM dbo.Source AS s
            ) AS d(Id, XmlPayload)
            CROSS APPLY dbo.fnExpand(d.Id) AS f(Value)
            CROSS APPLY
            (
                VALUES
                    (1, 'First'),
                    (2, 'Second')
            ) AS v(Id, Label)
            CROSS APPLY d.XmlPayload.nodes('/root/item') AS n(Item)
            """);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            new MetaTransformScriptSqlService().ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            CanonicalSql(Assert.Single(expected.ViewList).DefinitionSql),
            CanonicalSql(Assert.Single(actual.ViewList).DefinitionSql));
    }

    [Fact]
    public async Task ForwardWeave_RendersExplicitViewColumnsLikeEstablishedConverter()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            """
            CREATE VIEW reporting.vExplicitColumns
            (
                CustomerId,
                [Display Name]
            )
            AS
            SELECT
                s.Id,
                s.Name
            FROM dbo.Source AS s
            """);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            new MetaTransformScriptSqlService().ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            CanonicalSql(Assert.Single(expected.ViewList).DefinitionSql),
            CanonicalSql(Assert.Single(actual.ViewList).DefinitionSql));
    }

    [Fact]
    public async Task ForwardWeave_RendersFullTextFormsLikeEstablishedConverter()
    {
        var source = ImportCorpusModules(
            "027_fulltext.sql",
            "028_fulltext_table.sql",
            "061_freetext.sql",
            "062_freetext_table.sql");

        await AssertWeaveMatchesEstablishedConverter(source);
    }

    [Fact]
    public async Task ForwardWeave_RendersGlobalFunctionRowsetsLikeEstablishedConverter()
    {
        var source = ImportCorpusModules(
            "022_openjson.sql",
            "026_builtin_table_functions.sql");

        await AssertWeaveMatchesEstablishedConverter(source);
    }

    [Fact]
    public async Task ForwardWeave_RendersPivotAndUnpivotLikeEstablishedConverter()
    {
        var source = ImportCorpusModules(
            "005_pivot.sql",
            "006_unpivot.sql");

        await AssertWeaveMatchesEstablishedConverter(source);
    }

    [Fact]
    public async Task ForwardWeave_RendersTableHintsAndSamplingLikeEstablishedConverter()
    {
        var service = new MetaTransformScriptSqlService();
        var source = service.ImportFromSqlCode(
            """
            CREATE VIEW reporting.vHints
            AS
            SELECT
                s.Id AS Id
            FROM dbo.Source AS s WITH (NOLOCK, INDEX(SourceIndex), FORCESEEK)
            """);
        service.ImportSqlCode(
            source,
            MetaTransformScriptTestHelper.LoadCorpus("023_table_sample.sql"),
            targetSqlIdentifier: null);

        await AssertWeaveMatchesEstablishedConverter(source);
    }

    [Fact]
    public async Task ForwardWeave_ModuleShellRejectsRawStatements()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            "INSERT INTO dbo.Target (Id) SELECT 1 AS Id;",
            "LoadTarget");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.False(result.IsSuccess);
        Assert.Null(result.OutputWorkspace);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "TransformScriptModuleClassificationInvalid");
    }

    [Fact]
    public async Task ForwardWeave_RejectsSyntaxRejectedByEstablishedConverter()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            """
            CREATE VIEW dbo.vOdbcEscape
            AS
            SELECT s.Id AS Id
            FROM dbo.Source AS s
            WHERE s.Name LIKE 'A!_%' ESCAPE '!'
            """);
        Assert.Single(source.LikePredicateList).OdbcEscape = "true";

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.False(result.IsSuccess);
        Assert.Null(result.OutputWorkspace);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "TransformScriptRendererUnsupported");
    }

    [Theory]
    [InlineData("BinaryExpression")]
    [InlineData("UnaryExpression")]
    [InlineData("BooleanBinaryExpression")]
    [InlineData("BooleanComparisonExpression")]
    [InlineData("QualifiedJoin")]
    [InlineData("UnqualifiedJoin")]
    public async Task ForwardWeave_RejectsNullRendererDiscriminators(string discriminator)
    {
        var source = CreateAdversarialRendererWorkspace();
        var expectedId = discriminator switch
        {
            "BinaryExpression" => Clear(
                Assert.Single(source.BinaryExpressionList),
                static item => item.BinaryExpressionType = null),
            "UnaryExpression" => Clear(
                Assert.Single(source.UnaryExpressionList),
                static item => item.UnaryExpressionType = null),
            "BooleanBinaryExpression" => Clear(
                Assert.Single(source.BooleanBinaryExpressionList),
                static item => item.BinaryExpressionType = null),
            "BooleanComparisonExpression" => Clear(
                source.BooleanComparisonExpressionList[0],
                static item => item.ComparisonType = null),
            "QualifiedJoin" => Clear(
                Assert.Single(source.QualifiedJoinList),
                static item => item.QualifiedJoinType = null),
            "UnqualifiedJoin" => Clear(
                Assert.Single(source.UnqualifiedJoinList),
                static item => item.UnqualifiedJoinType = null),
            _ => throw new InvalidOperationException($"Unknown discriminator '{discriminator}'.")
        };

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.False(result.IsSuccess);
        Assert.Null(result.OutputWorkspace);
        Assert.Contains(
            result.Issues,
            issue =>
                issue.Code == "TransformScriptRendererUnsupported" &&
                issue.RequirementName == "RendererCoverage" &&
                issue.Message.Contains(expectedId, StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("NeitherTokenNorChild")]
    [InlineData("TokenAndChild")]
    [InlineData("ChildIdWithoutChildKind")]
    [InlineData("ChildKindWithoutChildId")]
    [InlineData("DuplicateSlotPath")]
    public async Task ForwardWeave_RejectsMalformedRenderEventProtocol(string failureReason)
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            "CREATE VIEW dbo.vProtocolProbe AS SELECT 1 AS Id;");

        var result = await ExecuteWeaveAsync(
            source,
            "ProbeDb",
            direction => ReplaceRenderEvents(direction, ProtocolProbeSql(failureReason)));

        Assert.False(result.IsSuccess);
        Assert.Null(result.OutputWorkspace);
        Assert.Contains(
            result.Issues,
            issue =>
                issue.Code == "TransformScriptRenderEventProtocolInvalid" &&
                issue.RequirementName == "RenderEventProtocol" &&
                issue.Message.Contains("RelationName=render_events", StringComparison.Ordinal) &&
                issue.Message.Contains("ParentKind=ProtocolProbe", StringComparison.Ordinal) &&
                issue.Message.Contains("SlotPath=010", StringComparison.Ordinal) &&
                issue.Message.Contains($"FailureReason={failureReason}", StringComparison.Ordinal));
    }

    [Fact]
    public async Task ForwardWeave_RejectsMultipleScalarSubtypesThroughRenderEventProtocol()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            "CREATE VIEW dbo.vSubtypeProbe AS SELECT 1 + 2 AS Value;");
        var binary = Assert.Single(source.BinaryExpressionList);
        var firstExpression = Assert.Single(source.BinaryExpressionFirstExpressionLinkList).ScalarExpression;
        var unary = new UnaryExpression
        {
            Id = "AdversarialUnaryExpression",
            ScalarExpression = binary.ScalarExpression,
            UnaryExpressionType = "Positive"
        };
        source.UnaryExpressionList.Add(unary);
        source.UnaryExpressionExpressionLinkList.Add(new UnaryExpressionExpressionLink
        {
            Id = "AdversarialUnaryExpressionExpressionLink",
            UnaryExpression = unary,
            ScalarExpression = firstExpression
        });

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.False(result.IsSuccess);
        Assert.Contains(
            result.Issues,
            issue =>
                issue.Code == "TransformScriptRenderEventProtocolInvalid" &&
                issue.Message.Contains("ParentKind=ScalarExpression", StringComparison.Ordinal) &&
                issue.Message.Contains($"ParentId={binary.ScalarExpression.Id}", StringComparison.Ordinal) &&
                issue.Message.Contains("FailureReason=DuplicateSlotPath", StringComparison.Ordinal));
    }

    [Fact]
    public async Task ForwardWeave_NormalizesDuplicateSparseAndNonZeroOrdinalsIntoStableSlots()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            """
            CREATE VIEW dbo.vOrdinalProbe
            AS
            SELECT COALESCE(a.Name, b.Name) AS ChosenName, a.Id AS Id
            FROM dbo.SourceA AS a, dbo.SourceB AS b
            """);

        SetOrdinals(source.QuerySpecificationSelectElementsItemList, "7", "7");
        SetOrdinals(source.FromClauseTableReferencesItemList, "4", "4");
        SetOrdinals(source.CoalesceExpressionExpressionsItemList, "9", "21");
        var multipartItems = source.MultiPartIdentifierIdentifiersItemList
            .GroupBy(item => item.MultiPartIdentifier.Id, StringComparer.Ordinal)
            .First(group => group.Count() > 1)
            .ToArray();
        SetOrdinals(multipartItems, "3", "3");

        await AssertWeaveMatchesEstablishedConverter(source);
        var first = await ExecuteWeaveAsync(source, "ProbeDb");
        var second = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(first.IsSuccess, FormatIssues(first));
        Assert.True(second.IsSuccess, FormatIssues(second));
        var firstSql = Assert.Single(TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            first.OutputWorkspace!,
            static () => new MetaSqlModel()).ViewList).DefinitionSql;
        var secondSql = Assert.Single(TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            second.OutputWorkspace!,
            static () => new MetaSqlModel()).ViewList).DefinitionSql;
        Assert.Equal(firstSql, secondSql);
    }

    [Fact]
    public async Task ForwardWeave_RendersJoinsAndWhereLikeEstablishedConverter()
    {
        var source = new MetaTransformScriptSqlService().ImportFromSqlCode(
            """
            CREATE VIEW reporting.vActiveSource
            AS
            SELECT
                s.Id AS Id,
                s.Amount + 1 AS NextAmount,
                NULLIF(o.Name, '') AS OtherName,
                COALESCE(p.Name, 'none') AS OptionalName,
                IIF(s.Amount > 0, 'positive', 'other') AS AmountKind
            FROM dbo.Source AS s
            INNER JOIN dbo.Other AS o ON o.Id = s.OtherId
            LEFT OUTER JOIN dbo.Optional AS p ON p.Id = s.OptionalId
            WHERE (s.IsActive = 1 AND s.Amount >= .5) OR s.DeletedAt IS NULL
            """);
        var expected = TransformScriptToSqlCSharpReference.Convert(
            new MetaTransformScriptSqlService().ExportModuleDefinitions(source),
            "ProbeDb");

        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            CanonicalSql(Assert.Single(expected.ViewList).DefinitionSql),
            CanonicalSql(Assert.Single(actual.ViewList).DefinitionSql));
    }

    private static MetaTransformScriptModel CreateModuleWorkspace()
    {
        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(
            """
            CREATE VIEW stage.vCustomer
            AS
            SELECT 1 AS CustomerId
            """);
        service.ImportSqlCode(
            model,
            """
            CREATE FUNCTION stage.fnCustomer(@CustomerId int)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT @CustomerId AS CustomerId
            )
            """,
            targetSqlIdentifier: null);
        service.ImportSqlCode(
            model,
            """
            CREATE FUNCTION util.fnAddOne(@value int)
            RETURNS int
            AS
            BEGIN
                RETURN @value + 1;
            END
            """,
            targetSqlIdentifier: null);
        service.ImportSqlCode(
            model,
            """
            CREATE FUNCTION util.fnNormalize(@value decimal(18, 4), @label nvarchar(50))
            RETURNS decimal(18, 4)
            AS
            BEGIN
                RETURN IIF(@value > 0, @value, 0);
            END
            """,
            targetSqlIdentifier: null);
        service.ImportSqlCode(
            model,
            """
            CREATE PROCEDURE ops.RunReview
            AS
            BEGIN
                SELECT 1 AS ReviewRunId;
            END
            """,
            targetSqlIdentifier: null);
        return model;
    }

    private static MetaTransformScriptModel CreateAdversarialRendererWorkspace() =>
        new MetaTransformScriptSqlService().ImportFromSqlCode(
            """
            CREATE VIEW dbo.vAdversarialRenderer
            AS
            SELECT -(a.Value + 1) AS ArithmeticValue
            FROM dbo.SourceA AS a
            INNER JOIN dbo.SourceB AS b ON a.Id = b.Id
            CROSS JOIN dbo.SourceC AS c
            WHERE a.Value = b.Value AND a.Name <> b.Name
            """);

    private static string Clear<T>(T item, Action<T> clear)
        where T : class
    {
        clear(item);
        return (string)item.GetType().GetProperty("Id")!.GetValue(item)!;
    }

    private static void SetOrdinals<T>(IReadOnlyList<T> items, params string[] ordinals)
        where T : class
    {
        Assert.Equal(ordinals.Length, items.Count);
        var property = typeof(T).GetProperty("Ordinal")!;
        for (var index = 0; index < items.Count; index++)
        {
            property.SetValue(items[index], ordinals[index]);
        }
    }

    private static MetaWeaveScriptDirection ReplaceRenderEvents(
        MetaWeaveScriptDirection direction,
        string sql)
    {
        var selectStatement = new MetaWeaveScriptSqlService().ImportIntoModel(direction.Model, sql);
        return direction with
        {
            Relations = direction.Relations.Select(relation =>
                string.Equals(relation.Name, "render_events", StringComparison.OrdinalIgnoreCase)
                    ? new MetaWeaveScriptRelation(relation.Name, selectStatement)
                    : relation).ToArray()
        };
    }

    private static string ProtocolProbeSql(string failureReason) => failureReason switch
    {
        "NeitherTokenNorChild" =>
            "SELECT v.ParentKind AS ParentKind, v.ParentId AS ParentId, v.SlotPath AS SlotPath, v.Token AS Token, v.ChildKind AS ChildKind, v.ChildId AS ChildId FROM (VALUES ('ProtocolProbe', 'probe', '010', NULL, NULL, NULL)) AS v(ParentKind, ParentId, SlotPath, Token, ChildKind, ChildId);",
        "TokenAndChild" =>
            "SELECT v.ParentKind AS ParentKind, v.ParentId AS ParentId, v.SlotPath AS SlotPath, v.Token AS Token, v.ChildKind AS ChildKind, v.ChildId AS ChildId FROM (VALUES ('ProtocolProbe', 'probe', '010', 'token', 'Identifier', 'child')) AS v(ParentKind, ParentId, SlotPath, Token, ChildKind, ChildId);",
        "ChildIdWithoutChildKind" =>
            "SELECT v.ParentKind AS ParentKind, v.ParentId AS ParentId, v.SlotPath AS SlotPath, v.Token AS Token, v.ChildKind AS ChildKind, v.ChildId AS ChildId FROM (VALUES ('ProtocolProbe', 'probe', '010', NULL, NULL, 'child')) AS v(ParentKind, ParentId, SlotPath, Token, ChildKind, ChildId);",
        "ChildKindWithoutChildId" =>
            "SELECT v.ParentKind AS ParentKind, v.ParentId AS ParentId, v.SlotPath AS SlotPath, v.Token AS Token, v.ChildKind AS ChildKind, v.ChildId AS ChildId FROM (VALUES ('ProtocolProbe', 'probe', '010', NULL, 'Identifier', NULL)) AS v(ParentKind, ParentId, SlotPath, Token, ChildKind, ChildId);",
        "DuplicateSlotPath" =>
            "SELECT v.ParentKind AS ParentKind, v.ParentId AS ParentId, v.SlotPath AS SlotPath, v.Token AS Token, v.ChildKind AS ChildKind, v.ChildId AS ChildId FROM (VALUES ('ProtocolProbe', 'probe', '010', 'first', NULL, NULL), ('ProtocolProbe', 'probe', '010', 'second', NULL, NULL)) AS v(ParentKind, ParentId, SlotPath, Token, ChildKind, ChildId);",
        _ => throw new InvalidOperationException($"Unknown protocol failure reason '{failureReason}'.")
    };

    private static async Task<MetaWeaveScriptApplicationResult> ExecuteWeaveAsync(
        MetaTransformScriptModel source,
        string databaseName,
        Func<MetaWeaveScriptDirection, MetaWeaveScriptDirection>? configureDirection = null)
    {
        var repositoryRoot = CliTestRunner.FindRepositoryRoot();
        var targetContract = await TypedWorkspaceModelMapper.LoadStateAsync(
            Path.Combine(repositoryRoot, "MetaSql", "Workspace"));
        var emptyTarget = new InMemoryWorkspace(
            targetContract.Model.Clone(),
            new GenericInstance { ModelName = targetContract.Model.Name });

        var direction = new MetaWeaveScriptDirectionLoader().Load(
                Path.Combine(
                    repositoryRoot,
                    "MetaConvert",
                    "Weaves",
                    "TransformScriptToSql"),
                "forward");
        direction = configureDirection?.Invoke(direction) ?? direction;

        return new MetaWeaveScriptExecutionService().ExecuteDirection(
            direction,
            new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
            {
                ["transform"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(source),
            },
            emptyTarget,
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["databaseName"] = databaseName,
            });
    }

    private static MetaTransformScriptModel ImportCorpusModules(params string[] fileNames)
    {
        var service = new MetaTransformScriptSqlService();
        MetaTransformScriptModel? source = null;

        foreach (var fileName in fileNames)
        {
            var sql = MetaTransformScriptTestHelper.LoadCorpus(fileName);
            if (source is null)
            {
                source = service.ImportFromSqlCode(sql);
            }
            else
            {
                service.ImportSqlCode(source, sql, targetSqlIdentifier: null);
            }
        }

        return source ?? throw new InvalidOperationException("At least one corpus module is required.");
    }

    private static async Task AssertWeaveMatchesEstablishedConverter(MetaTransformScriptModel source)
    {
        var expected = TransformScriptToSqlCSharpReference.Convert(
            new MetaTransformScriptSqlService().ExportModuleDefinitions(source),
            "ProbeDb");
        var result = await ExecuteWeaveAsync(source, "ProbeDb");

        Assert.True(result.IsSuccess, FormatIssues(result));
        var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => new MetaSqlModel());
        Assert.Equal(
            expected.DatabaseList.OrderBy(item => item.Id, StringComparer.Ordinal).Select(item => (item.Id, item.Name, item.Collation)),
            actual.DatabaseList.OrderBy(item => item.Id, StringComparer.Ordinal).Select(item => (item.Id, item.Name, item.Collation)));
        Assert.Equal(
            expected.SchemaList.OrderBy(item => item.Id, StringComparer.Ordinal).Select(item => (item.Id, item.Name, DatabaseId: item.Database.Id)),
            actual.SchemaList.OrderBy(item => item.Id, StringComparer.Ordinal).Select(item => (item.Id, item.Name, DatabaseId: item.Database.Id)));
        Assert.Equal(
            expected.StoredProcedureList.OrderBy(item => item.Id, StringComparer.Ordinal).Select(item => (item.Id, item.Name, SchemaId: item.Schema.Id, item.DefinitionSql, item.DeployOrdinal)),
            actual.StoredProcedureList.OrderBy(item => item.Id, StringComparer.Ordinal).Select(item => (item.Id, item.Name, SchemaId: item.Schema.Id, item.DefinitionSql, item.DeployOrdinal)));
        Assert.Equal(
            expected.ViewList.OrderBy(item => item.Id, StringComparer.Ordinal).Select(item => (item.Id, item.Name, SchemaId: item.Schema.Id, DefinitionSql: CanonicalSql(item.DefinitionSql), item.DeployOrdinal)),
            actual.ViewList.OrderBy(item => item.Id, StringComparer.Ordinal).Select(item => (item.Id, item.Name, SchemaId: item.Schema.Id, DefinitionSql: CanonicalSql(item.DefinitionSql), item.DeployOrdinal)));
        Assert.Equal(
            expected.FunctionList.OrderBy(item => item.Id, StringComparer.Ordinal).Select(item => (item.Id, item.Name, SchemaId: item.Schema.Id, DefinitionSql: CanonicalSql(item.DefinitionSql), item.DeployOrdinal, item.FunctionKind)),
            actual.FunctionList.OrderBy(item => item.Id, StringComparer.Ordinal).Select(item => (item.Id, item.Name, SchemaId: item.Schema.Id, DefinitionSql: CanonicalSql(item.DefinitionSql), item.DeployOrdinal, item.FunctionKind)));
    }

    private static string FormatIssues(MetaWeaveScriptApplicationResult result) =>
        string.Join(
            Environment.NewLine,
            result.Issues.Select(issue => $"{issue.Code}: {issue.Message}"));

    private static string CanonicalSql(string sql) =>
        sql.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal);
}
