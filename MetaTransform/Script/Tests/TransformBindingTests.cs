using MetaSchema;
using MetaTransform.Binding;
using MetaTransformBinding;
using MetaTransformScript;
using MetaTransformScript.Sql.Parsing;

public sealed class TransformBindingTests
{
    [Fact]
    public void BindSimpleSelectWithAliasesAndLiteralAlias_DerivesExpectedOutputRowset()
    {
        var model = ParseCorpus("001_basic_select.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.NotNull(bound.TopLevelRowset);
        Assert.Equal(
            ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"],
            bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
        Assert.Equal(3, bound.ColumnReferences.Count);
    }

    [Fact]
    public void BindSelectStarAcrossJoinedNamedSources_WithSchema_DerivesStarOutputColumns()
    {
        var model = ParseCorpus("002_select_star.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customers", ["CustomerId", "CustomerName"]),
            ("dbo", "Orders", ["OrderId", "CustomerId"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.NotNull(bound.TopLevelRowset);
        Assert.Equal(
            ["CustomerId", "CustomerName", "OrderId"],
            bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
    }

    [Fact]
    public void BindAmbiguousUnqualifiedColumn_ProducesExplicitIssue()
    {
        var sql = """
CREATE VIEW dbo.v_ambiguous AS
SELECT
    Id
FROM dbo.Customer AS c
INNER JOIN dbo.[Order] AS o
    ON o.CustomerId = c.Id;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customer", ["Id", "CustomerId"]),
            ("dbo", "Order", ["Id", "CustomerId"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        var issue = Assert.Single(bound.Issues, item => item.Code == "ColumnReferenceAmbiguous");
        Assert.Equal("ColumnReferenceAmbiguous", issue.Code);
    }

    [Fact]
    public void BindDateAddDatePartToken_DoesNotResolveDatePartAsColumn()
    {
        var sql = """
CREATE VIEW dbo.v_dateadd AS
SELECT
    DATEADD(day, 1, s.CreatedAt) AS NextDate
FROM dbo.SourceTable AS s;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["CreatedAt"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.DoesNotContain(
            bound.ColumnReferences,
            item => item.IdentifierParts.Count == 1 &&
                    string.Equals(item.IdentifierParts[0], "day", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(["NextDate"], bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
    }

    [Fact]
    public void BindDateDiffDatePartToken_DoesNotResolveDatePartAsColumn()
    {
        var sql = """
CREATE VIEW dbo.v_datediff AS
SELECT
    DATEDIFF(day, s.StartDate, s.EndDate) AS DayDelta
FROM dbo.SourceTable AS s;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["StartDate", "EndDate"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.DoesNotContain(
            bound.ColumnReferences,
            item => item.IdentifierParts.Count == 1 &&
                    string.Equals(item.IdentifierParts[0], "day", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(["DayDelta"], bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
    }

    [Fact]
    public void BindSelectWithoutAlias_UsesTargetSchemaColumnNameWhenAvailable()
    {
        var sql = """
CREATE VIEW dbo.v_target_named AS
SELECT
    SUM(s.Amount)
FROM dbo.Sales AS s;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["Amount"]),
            ("dbo", "v_target_named", ["TotalAmount"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.Equal(["TotalAmount"], bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
    }

    [Fact]
    public void BindCorrelatedSubquery_UnqualifiedInnerColumn_ResolvesInnerScopeFirst()
    {
        var sql = """
CREATE VIEW dbo.v_correlated AS
WITH returns_cte AS
(
    SELECT
        r.StoreId,
        r.CustomerId,
        r.ReturnAmount
    FROM dbo.StoreReturns AS r
)
SELECT
    c.CustomerId
FROM returns_cte AS c
WHERE c.ReturnAmount >
(
    SELECT AVG(ReturnAmount)
    FROM returns_cte AS c2
    WHERE c2.StoreId = c.StoreId
);
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "StoreReturns", ["StoreId", "CustomerId", "ReturnAmount"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.DoesNotContain(bound.Issues, item => item.Code == "ColumnReferenceAmbiguous");
        Assert.False(bound.HasErrors);
    }

    [Fact]
    public void BindOrderByCase_UsesSelectAliasReferenceWithinExpression()
    {
        var sql = """
CREATE VIEW dbo.v_order_alias AS
SELECT
    s.Id + 1 AS lochierarchy
FROM dbo.Source AS s
ORDER BY
    CASE WHEN lochierarchy > 0 THEN lochierarchy END;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.DoesNotContain(bound.Issues, item => item.Code == "ColumnReferenceNotFound");
        Assert.False(bound.HasErrors);
    }

    [Fact]
    public void BindGroupedAggregateCasePredicate_DoesNotRequireGroupedColumnsInsideAggregateArgument()
    {
        var sql = """
CREATE VIEW dbo.v_grouped_case AS
SELECT
    s.GroupId,
    SUM(CASE WHEN s.Flag = 1 THEN s.Amount ELSE 0 END) AS FlaggedAmount
FROM dbo.Source AS s
GROUP BY s.GroupId;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["GroupId", "Flag", "Amount"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.DoesNotContain(bound.Issues, item => item.Code == "UngroupedColumnReference");
        Assert.False(bound.HasErrors);
    }

    [Fact]
    public void BindSelectScalarWithoutAlias_UsesDeterministicFallbackOutputName()
    {
        var sql = """
CREATE VIEW dbo.v_expr_name AS
SELECT
    s.Id + 1
FROM dbo.Source AS s;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.Equal(["Expr1"], bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
    }

    [Fact]
    public void BindGroupByScalarExpression_DoesNotProduceUnsupportedGroupingExpressionIssue()
    {
        var sql = """
CREATE VIEW dbo.v_group_expr AS
SELECT
    LEFT(s.Name, 3) AS NamePrefix,
    COUNT(*) AS Cnt
FROM dbo.Source AS s
GROUP BY LEFT(s.Name, 3);
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Name"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.DoesNotContain(bound.Issues, item => item.Code == "UnsupportedGroupingExpressionShape");
        Assert.DoesNotContain(bound.Issues, item => item.Code == "UngroupedColumnReference");
        Assert.False(bound.HasErrors);
    }

    [Fact]
    public void BindGroupedStatisticalAggregate_DoesNotTreatAggregateArgumentAsUngrouped()
    {
        var sql = """
CREATE VIEW dbo.v_group_stat AS
SELECT
    s.CategoryId,
    STDDEV_SAMP(s.Amount) / AVG(s.Amount) AS Cov
FROM dbo.Sales AS s
GROUP BY s.CategoryId;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CategoryId", "Amount"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.DoesNotContain(bound.Issues, item => item.Code == "UngroupedColumnReference");
        Assert.False(bound.HasErrors);
    }

    [Fact]
    public void BindSourceLessSelect_DerivesFinalOutputRowsetWithoutTableSources()
    {
        var sql = """
CREATE VIEW dbo.v_constants AS
SELECT
    1 AS Id,
    'One' AS Name;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, CreateSourceSchema());

        Assert.Empty(bindingModel.TableSourceList);
        Assert.Empty(bindingModel.SourceTargetList);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalRowset = Assert.Single(bindingModel.RowsetList, item => item.Id == finalLink.RowsetId);
        Assert.Equal(
            ["Id", "Name"],
            bindingModel.ColumnList
                .Where(item => item.RowsetId == finalRowset.Id)
                .OrderBy(item => int.Parse(item.Ordinal))
                .Select(item => item.Name)
                .ToArray());
    }

    [Fact]
    public void BindSimpleSelect_EmitsBindingModelWithFinalOutputRowset()
    {
        var model = ParseCorpus("001_basic_select.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var binding = Assert.Single(bindingModel.TransformBindingList);
        Assert.Equal(model.TransformScriptList[0].Id, binding.MetaTransformScriptTransformScriptId);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalRowset = Assert.Single(bindingModel.RowsetList, item => item.Id == finalLink.RowsetId);
        Assert.Equal(
            ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"],
            bindingModel.ColumnList
                .Where(item => item.RowsetId == finalRowset.Id)
                .OrderBy(item => int.Parse(item.Ordinal))
                .Select(item => item.Name)
                .ToArray());

        Assert.Equal(3, bindingModel.ColumnReferenceList.Count);
        Assert.All(
            bindingModel.ColumnReferenceList,
            item =>
            {
                Assert.Contains(bindingModel.ColumnList, column => column.Id == item.ColumnId);
                Assert.Contains(bindingModel.TableSourceList, source => source.Id == item.TableSourceId);
            });
    }

    [Fact]
    public void BindJoinedSources_EmitsIntermediateJoinRowsetGraph()
    {
        var model = ParseCorpus("002_select_star.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customers", ["CustomerId", "CustomerName"]),
            ("dbo", "Orders", ["OrderId", "CustomerId"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var joinRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "Join");
        var joinInputs = bindingModel.SourceTargetList
            .Where(item => item.TargetId == joinRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .ToArray();

        Assert.Equal(2, joinInputs.Length);
        Assert.All(joinInputs, input => Assert.Contains(
            bindingModel.RowsetList,
            rowset => rowset.Id == input.SourceId && rowset.DerivationKind == "Source"));

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalInput = Assert.Single(bindingModel.SourceTargetList, item => item.TargetId == finalLink.RowsetId);
        Assert.Equal(joinRowset.Id, finalInput.SourceId);
    }

    [Fact]
    public void BindQueryDerivedTable_EmitsInnerProjectionAndDerivedTableRowsets()
    {
        var sql = """
CREATE VIEW dbo.v_dt AS
SELECT
    d.CustomerId,
    d.CustomerName
FROM
(
    SELECT
        c.CustomerId,
        c.CustomerName
    FROM dbo.Customers AS c
) AS d;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customers", ["CustomerId", "CustomerName"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var derivedTableRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "DerivedTable");
        var derivedTableInput = Assert.Single(bindingModel.SourceTargetList, item => item.TargetId == derivedTableRowset.Id);
        var innerProjectionRowset = Assert.Single(bindingModel.RowsetList, item => item.Id == derivedTableInput.SourceId);
        Assert.Equal("Projection", innerProjectionRowset.DerivationKind);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalInput = Assert.Single(bindingModel.SourceTargetList, item => item.TargetId == finalLink.RowsetId);
        Assert.Equal(derivedTableRowset.Id, finalInput.SourceId);

        var derivedTableSource = Assert.Single(bindingModel.TableSourceList, item => item.RowsetId == derivedTableRowset.Id);
        Assert.Equal("d", derivedTableSource.ExposedName);
    }

    [Fact]
    public void BindInlineDerivedTable_EmitsInlineDerivedRowsetAndFinalProjection()
    {
        var model = ParseCorpus("021_inline_values.sql");

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, CreateSourceSchema());

        var inlineDerivedRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "InlineDerivedTable");
        var inlineDerivedColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == inlineDerivedRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["Id", "Name"], inlineDerivedColumns);

        var inlineSource = Assert.Single(bindingModel.TableSourceList, item => item.RowsetId == inlineDerivedRowset.Id);
        Assert.Equal("src", inlineSource.ExposedName);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalInput = Assert.Single(bindingModel.SourceTargetList, item => item.TargetId == finalLink.RowsetId);
        Assert.Equal(inlineDerivedRowset.Id, finalInput.SourceId);

        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["Id", "Name"], finalColumns);
    }

    [Fact]
    public void BindInlineDerivedTable_WithoutColumnAliases_ProducesExplicitIssue()
    {
        var sql = """
CREATE VIEW dbo.v_values_no_aliases AS
SELECT
    src.Id
FROM
(
    VALUES
        (1, 'One')
) AS src;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var bound = new TransformBindingService().BindSingleTransform(model, CreateSourceSchema());

        Assert.Contains(bound.Issues, item => item.Code == "InlineDerivedTableColumnAliasesRequired");
        Assert.True(bound.HasErrors);
    }

    [Fact]
    public void BindCommonTableExpressionWithColumnAliases_EmitsCteRowsetAndFinalProjection()
    {
        var model = ParseCorpus("042_cte_column_list.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var cteRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "CommonTableExpression");
        var cteColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == cteRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "TotalAmount"], cteColumns);

        var cteSource = Assert.Single(bindingModel.TableSourceList, item => item.RowsetId == cteRowset.Id);
        Assert.Equal("CustomerAmounts", cteSource.ExposedName);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalInput = Assert.Single(bindingModel.SourceTargetList, item => item.TargetId == finalLink.RowsetId);
        Assert.Equal(cteRowset.Id, finalInput.SourceId);
    }

    [Fact]
    public void BindRecursiveCommonTableExpression_DerivesRecursiveCteRowsetShape()
    {
        var model = ParseCorpus("043_recursive_cte_column_list.sql");

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, CreateSourceSchema());

        var cteRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "CommonTableExpression");
        var cteColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == cteRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["N"], cteColumns);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["N"], finalColumns);
    }

    [Fact]
    public void BindRecursiveCommonTableExpression_WithoutExplicitColumnList_DerivesAnchorNamedShape()
    {
        var model = ParseCorpus("017_cte.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id", "ParentId"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var recursiveCteRowset = Assert.Single(bindingModel.RowsetList, item => item.Name == "recursive_cte");
        var recursiveCteColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == recursiveCteRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["Id", "ParentId", "Depth"], recursiveCteColumns);
    }

    [Fact]
    public void BindSetOperations_DerivesSetOperationRowsetsAndFinalOutput()
    {
        var model = ParseCorpus("013_set_operations.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "A", ["Id", "Code"]),
            ("dbo", "B", ["Id", "Code"]),
            ("dbo", "C", ["Id", "Code"]),
            ("dbo", "D", ["Id", "Code"]),
            ("dbo", "E", ["Id", "Code"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Contains(bindingModel.RowsetList, item => item.DerivationKind == "SetOperation");

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalRowset = Assert.Single(bindingModel.RowsetList, item => item.Id == finalLink.RowsetId);
        Assert.Equal("SetOperation", finalRowset.DerivationKind);

        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["Id", "Code"], finalColumns);
    }

    [Fact]
    public void BindSetOperation_WithDifferentInputColumnAliases_UsesFirstInputColumnNames()
    {
        var sql = """
CREATE VIEW dbo.v_set_alias AS
SELECT
    a.Id AS BrandId
FROM dbo.A AS a
UNION ALL
SELECT
    b.Id AS ItemBrandId
FROM dbo.B AS b;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "A", ["Id"]),
            ("dbo", "B", ["Id"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.NotNull(bound.TopLevelRowset);
        Assert.Equal(["BrandId"], bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
    }

    [Fact]
    public void BindPivotTableReference_DerivesPivotRowsetAndFinalProjection()
    {
        var model = ParseCorpus("005_pivot.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CustomerId", "CategoryCode", "Amount"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);
        Assert.False(bound.HasErrors);

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var pivotRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "Pivot");
        var pivotColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == pivotRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "A", "B"], pivotColumns);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "A", "B"], finalColumns);
    }

    [Fact]
    public void BindUnpivotTableReference_DerivesUnpivotRowsetAndFinalProjection()
    {
        var model = ParseCorpus("006_unpivot.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "PivotSource", ["CustomerId", "A", "B"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);
        Assert.False(bound.HasErrors);

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var unpivotRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "Unpivot");
        var unpivotColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == unpivotRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "CategoryCode", "Amount"], unpivotColumns);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "CategoryCode", "Amount"], finalColumns);
    }

    [Fact]
    public void BindQueryParentheses_BindsParenthesizedSetBranches()
    {
        var model = ParseCorpus("024_query_parentheses.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id", "Code"]),
            ("dbo", "Target", ["Id", "Code"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.NotNull(bound.TopLevelRowset);
        Assert.Equal("SetOperation", bound.TopLevelRowset!.DerivationKind);
        Assert.Equal(["Id", "Code"], bound.TopLevelRowset.Columns.Select(item => item.Name).ToArray());
        Assert.Equal(4, bound.ColumnReferences.Count);
    }

    [Fact]
    public void BindJoinParentheses_BindsParenthesizedJoinTableReference()
    {
        var model = ParseCorpus("031_join_parentheses.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "A", ["Id", "BId", "CId"]),
            ("dbo", "B", ["Id"]),
            ("dbo", "C", ["Id", "Name"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.NotNull(bound.TopLevelRowset);
        Assert.Equal(["Id", "Name"], bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
        Assert.Contains(bound.Rowsets, item => item.DerivationKind == "Join");
    }

    [Fact]
    public void BindCrossApplyQueryDerivedTable_CanResolveOuterScopeColumns()
    {
        var sql = """
CREATE VIEW dbo.v_apply_dt AS
SELECT
    s.Id,
    applySource.SourceId
FROM dbo.SourceTable AS s
CROSS APPLY
(
    SELECT
        s.Id AS SourceId
) AS applySource;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["Id", "CsvValue"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var applyRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "Apply");
        var applyInputs = bindingModel.SourceTargetList
            .Where(item => item.TargetId == applyRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .ToArray();
        Assert.Equal(["ApplyLeft", "ApplyRight"], applyInputs.Select(item => item.InputRole).ToArray());

        var applySource = Assert.Single(bindingModel.TableSourceList, item => item.ExposedName == "applySource");
        Assert.Contains(bindingModel.RowsetList, item => item.Id == applySource.RowsetId && item.DerivationKind == "DerivedTable");

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalInput = Assert.Single(bindingModel.SourceTargetList, item => item.TargetId == finalLink.RowsetId);
        Assert.Equal(applyRowset.Id, finalInput.SourceId);
        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["Id", "SourceId"], finalColumns);
    }

    [Fact]
    public void BindCrossApplyFunctionTableReference_WithColumnAliases_DerivesFunctionRowset()
    {
        var sql = """
CREATE VIEW dbo.v_apply_fn AS
SELECT
    s.Id,
    splitItem.ValueText
FROM dbo.SourceTable AS s
CROSS APPLY dbo.fnSplit(s.CsvValue) AS splitItem(ValueText);
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["Id", "CsvValue"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var functionRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "FunctionTableReference");
        var functionColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == functionRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["ValueText"], functionColumns);

        var functionSource = Assert.Single(bindingModel.TableSourceList, item => item.ExposedName == "splitItem");
        Assert.Equal(functionRowset.Id, functionSource.RowsetId);
    }

    [Fact]
    public void BindCrossApplyFunctionTableReference_WithoutColumnAliases_InfersReferencedColumns()
    {
        var model = ParseCorpus("004_apply_sources.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["Id", "CsvValue"]),
            ("dbo", "Orders", ["Amount", "SourceId"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);
        Assert.False(bound.HasErrors);

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var functionRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "FunctionTableReference");
        var functionColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == functionRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();

        Assert.Equal(["ValueText"], functionColumns);
    }

    [Fact]
    public void BindOpenJson_CanInferDefaultOutputShapeFromScript()
    {
        var model = ParseCorpus("022_openjson.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "JsonSource", ["Id", "JsonPayload"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.NotNull(bound.TopLevelRowset);
        Assert.Equal(["Id", "key", "value", "type"], bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
        Assert.Equal(5, bound.ColumnReferences.Count);
    }

    [Fact]
    public void BindBuiltInGlobalFunctionTableReferences_CanBindSanctionedShapes()
    {
        var model = ParseCorpus("026_builtin_table_functions.sql");

        var bound = new TransformBindingService().BindSingleTransform(model, CreateSourceSchema());

        Assert.False(bound.HasErrors);
        Assert.NotNull(bound.TopLevelRowset);
        Assert.Equal(["value"], bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
        Assert.Equal(2, bound.ColumnReferences.Count);
    }

    [Fact]
    public void BindUnknownGlobalFunctionTableReference_InfersReferencedColumns()
    {
        var sql = """
CREATE VIEW dbo.v_unknown_global_function AS
SELECT
    g.SomeColumn
FROM SomeGlobalTableFunction(1) AS g;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var bound = new TransformBindingService().BindSingleTransform(model, CreateSourceSchema());
        Assert.False(bound.HasErrors);

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, CreateSourceSchema());
        var functionRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "FunctionTableReference");
        var functionColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == functionRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["SomeColumn"], functionColumns);
    }

    [Fact]
    public void BindCorrelatedScalarSubqueryAndExists_PreservesCorrelationBinding()
    {
        var model = ParseCorpus("011_subqueries_and_correlation.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customers", ["CustomerId"]),
            ("dbo", "Orders", ["CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Equal(6, bindingModel.ColumnReferenceList.Count);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "MaxAmount", "HasOrders"], finalColumns);

        Assert.Contains(
            bindingModel.TableSourceList,
            item => item.ExposedName == "c");
        Assert.Contains(
            bindingModel.TableSourceList,
            item => item.ExposedName == "o");
        Assert.Contains(
            bindingModel.TableSourceList,
            item => item.ExposedName == "o2");
    }

    [Fact]
    public void BindSubqueryPredicates_BindsCorrelatedAndUncorrelatedPredicateSubqueries()
    {
        var model = ParseCorpus("012_subquery_predicates.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id", "Amount", "Code", "GroupId"]),
            ("dbo", "Target", ["Amount", "Code", "GroupId", "SourceId"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Equal(11, bindingModel.ColumnReferenceList.Count);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["Id"], finalColumns);
    }

    [Fact]
    public void BindWherePredicates_TraversesBetweenInLikeAndIsNull()
    {
        var model = ParseCorpus("007_where_predicates.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id", "Status", "Amount", "Code", "Name", "DeletedAt", "Score"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();

        Assert.Equal(7, resolvedColumnNames.Length);
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Id", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Status", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Amount", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Code", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Name", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "DeletedAt", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Score", StringComparison.Ordinal)));
    }

    [Fact]
    public void BindDistinctPredicate_TraversesBothOperands()
    {
        var model = ParseCorpus("025_distinct_predicate.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id", "Code", "LegacyCode"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();

        Assert.Equal(3, resolvedColumnNames.Length);
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Id", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Code", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "LegacyCode", StringComparison.Ordinal)));
    }

    [Fact]
    public void BindContainsPredicate_TraversesFullTextColumnReference()
    {
        var model = ParseCorpus("027_fulltext.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Products", ["ProductId", "Description"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();

        Assert.Equal(2, resolvedColumnNames.Length);
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "ProductId", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Description", StringComparison.Ordinal)));
    }

    [Fact]
    public void BindFreeTextPredicate_TraversesFullTextColumnReference()
    {
        var model = ParseCorpus("061_freetext.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Products", ["ProductId", "Description"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();

        Assert.Equal(2, resolvedColumnNames.Length);
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "ProductId", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Description", StringComparison.Ordinal)));
    }

    [Theory]
    [InlineData("028_fulltext_table.sql")]
    [InlineData("062_freetext_table.sql")]
    public void BindFullTextTableReference_DerivesKeyRankFunctionRowset(string corpusFile)
    {
        var model = ParseCorpus(corpusFile);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Products", ["ProductId", "Description"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);
        Assert.False(bound.HasErrors);

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var functionRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "FunctionTableReference");
        var functionColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == functionRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["KEY", "RANK"], functionColumns);

        var functionSource = Assert.Single(bindingModel.TableSourceList, item => item.RowsetId == functionRowset.Id);
        Assert.Equal("ft", functionSource.ExposedName);
        Assert.True(string.IsNullOrWhiteSpace(functionRowset.SqlIdentifier));

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["KEY", "RANK"], finalColumns);

        Assert.Equal(2, bindingModel.ColumnReferenceList.Count);
    }

    [Fact]
    public void BindXmlMethodCallTargets_TraversesReceiverColumnReferences()
    {
        var model = ParseCorpus("020_xml_namespaces_and_methods.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "XmlSource", ["Id", "XmlPayload"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);
        Assert.False(bound.HasErrors);

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();

        Assert.Equal(4, resolvedColumnNames.Length);
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Id", StringComparison.Ordinal)));
        Assert.Equal(3, resolvedColumnNames.Count(item => string.Equals(item, "XmlPayload", StringComparison.Ordinal)));
    }

    [Fact]
    public void BindXmlNodesTableReference_DerivesApplyRightRowsetAndBindsMethodTarget()
    {
        var model = ParseCorpus("055_xml_nodes.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "XmlSource", ["Id", "XmlPayload"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);
        Assert.False(bound.HasErrors);

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var xmlNodesRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "FunctionTableReference" && item.Name.StartsWith("XMLNODES:", StringComparison.Ordinal));
        var xmlNodesColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == xmlNodesRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["Item"], xmlNodesColumns);

        var xmlNodesSource = Assert.Single(bindingModel.TableSourceList, item => item.RowsetId == xmlNodesRowset.Id);
        Assert.Equal("n", xmlNodesSource.ExposedName);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["Id", "ItemCode"], finalColumns);

        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();
        Assert.Equal(3, resolvedColumnNames.Length);
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Id", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "XmlPayload", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Item", StringComparison.Ordinal)));
    }

    [Fact]
    public void BindExtractFunction_DoesNotTreatDatePartTokenAsColumnReference()
    {
        var model = ParseCorpus("030_time_zone_extract.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id", "CreatedAt"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);
        Assert.False(bound.HasErrors);

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();

        Assert.Equal(3, resolvedColumnNames.Length);
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Id", StringComparison.Ordinal)));
        Assert.Equal(2, resolvedColumnNames.Count(item => string.Equals(item, "CreatedAt", StringComparison.Ordinal)));
        Assert.DoesNotContain(resolvedColumnNames, item => string.Equals(item, "MONTH", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void BindTableSampleExpressions_TraversesSampleNumberAndRepeatSeed()
    {
        var sql = """
CREATE VIEW dbo.v_tablesample_traversal AS
SELECT
    a.Id
FROM dbo.A AS a
CROSS APPLY dbo.B AS b TABLESAMPLE (a.Id PERCENT) REPEATABLE (a.Id);
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "A", ["Id"]),
            ("dbo", "B", ["Id"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);
        Assert.False(bound.HasErrors);

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();

        Assert.Equal(3, resolvedColumnNames.Length);
        Assert.Equal(3, resolvedColumnNames.Count(item => string.Equals(item, "Id", StringComparison.Ordinal)));
    }

    [Fact]
    public void BindSequenceAndGlobalExpressions_AreAcceptedAsScalarLeaves()
    {
        var model = ParseCorpus("036_sequence_and_globals.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", []));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.Empty(bound.ColumnReferences);
    }

    [Fact]
    public void BindValueExpressions_BindsThroughCommonScalarExpressionShells()
    {
        var model = ParseCorpus("014_value_expressions.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id", "Status", "Amount", "PreferredName", "LegalName", "Code", "IsActive", "Priority", "Score", "CreatedAt", "CreatedAtText", "CustomerName"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Equal(14, bindingModel.ColumnReferenceList.Count);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(
            ["Id", "StatusText", "AmountBand", "DisplayName", "NormalizedCode", "ActiveAmount", "PriorityText", "AmountDecimal", "ScoreInt", "CreatedAtText", "ParsedCreatedAt", "CollatedCustomerName"],
            finalColumns);
    }

    [Fact]
    public void BindParenthesizedScalarExpressions_BindsInnerArithmeticAndCastArguments()
    {
        var model = ParseCorpus("047_parenthesized_scalar_expressions.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Equal(4, bindingModel.ColumnReferenceList.Count);
    }

    [Fact]
    public void BindNestedScalarSubqueries_BindsAggregateArgumentsInsideNestedQueryBoundaries()
    {
        var model = ParseCorpus("045_nested_subqueries.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customer", ["CustomerId"]),
            ("dbo", "Sales", ["CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Equal(4, bindingModel.ColumnReferenceList.Count);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "NestedMaxAmount"], finalColumns);
    }

    [Fact]
    public void BindGroupByHaving_EmitsGroupedRowsetAndFinalProjection()
    {
        var model = ParseCorpus("008_group_by_having.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var groupedRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "Grouping");
        var groupedColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == groupedRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId"], groupedColumns);

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalInput = Assert.Single(bindingModel.SourceTargetList, item => item.TargetId == finalLink.RowsetId);
        Assert.Equal(groupedRowset.Id, finalInput.SourceId);

        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "OrderCount", "TotalAmount"], finalColumns);
    }

    [Fact]
    public void BindGroupedSelect_WithUngroupedColumnReference_ProducesExplicitIssue()
    {
        var sql = """
CREATE VIEW dbo.v_bad_grouping AS
SELECT
    s.CustomerId,
    s.Amount
FROM dbo.Sales AS s
GROUP BY s.CustomerId;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CustomerId", "Amount"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.Contains(bound.Issues, item => item.Code == "UngroupedColumnReference");
        Assert.True(bound.HasErrors);
    }

    [Fact]
    public void BindGroupingSets_TraversesCompositeAndGrandTotalGroupingSpecifications()
    {
        var model = ParseCorpus("009_grouping_sets.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["RegionId", "CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var groupedRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "Grouping");
        var groupedColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == groupedRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["RegionId", "CustomerId"], groupedColumns);

        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();
        Assert.Equal(9, resolvedColumnNames.Length);
        Assert.Equal(5, resolvedColumnNames.Count(item => string.Equals(item, "RegionId", StringComparison.Ordinal)));
        Assert.Equal(3, resolvedColumnNames.Count(item => string.Equals(item, "CustomerId", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Amount", StringComparison.Ordinal)));
    }

    [Fact]
    public void BindRollup_TraversesNestedGroupingArguments()
    {
        var sql = """
CREATE VIEW dbo.v_rollup AS
SELECT
    s.RegionId,
    s.CustomerId,
    SUM(s.Amount) AS TotalAmount
FROM dbo.Sales AS s
GROUP BY ROLLUP (s.RegionId, s.CustomerId);
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["RegionId", "CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var groupedRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "Grouping");
        var groupedColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == groupedRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["RegionId", "CustomerId"], groupedColumns);
    }

    [Fact]
    public void BindCube_TraversesNestedGroupingArguments()
    {
        var sql = """
CREATE VIEW dbo.v_cube AS
SELECT
    s.RegionId,
    s.CustomerId,
    SUM(s.Amount) AS TotalAmount
FROM dbo.Sales AS s
GROUP BY CUBE (s.RegionId, s.CustomerId);
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["RegionId", "CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var groupedRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "Grouping");
        var groupedColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == groupedRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["RegionId", "CustomerId"], groupedColumns);
    }

    [Fact]
    public void BindGroupByAll_BindsSameGroupedVisibilityContract()
    {
        var model = ParseCorpus("048_group_by_all.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CustomerId"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var groupedRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "Grouping");
        Assert.Equal($"GroupedAll:QuerySpecification:1", groupedRowset.Name);

        var groupedColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == groupedRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId"], groupedColumns);

        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == Assert.Single(bindingModel.OutputRowsetList).RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "RowCount"], finalColumns);
    }

    [Fact]
    public void BindWindowFunctions_TraversesPartitionAndOrderExpressions()
    {
        var model = ParseCorpus("015_window_functions.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CustomerId", "OrderId", "Amount", "OrderDate"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();

        Assert.Equal(12, resolvedColumnNames.Length);
        Assert.Equal(5, resolvedColumnNames.Count(item => string.Equals(item, "CustomerId", StringComparison.Ordinal)));
        Assert.Equal(3, resolvedColumnNames.Count(item => string.Equals(item, "OrderDate", StringComparison.Ordinal)));
        Assert.Equal(3, resolvedColumnNames.Count(item => string.Equals(item, "Amount", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "OrderId", StringComparison.Ordinal)));
    }

    [Fact]
    public void BindNamedWindowClause_TraversesWindowDefinitions()
    {
        var model = ParseCorpus("016_named_window.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CustomerId", "Amount", "OrderDate"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();

        Assert.Equal(5, resolvedColumnNames.Length);
        Assert.Equal(2, resolvedColumnNames.Count(item => string.Equals(item, "CustomerId", StringComparison.Ordinal)));
        Assert.Equal(2, resolvedColumnNames.Count(item => string.Equals(item, "Amount", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "OrderDate", StringComparison.Ordinal)));
    }

    [Fact]
    public void BindRangeWindowFrames_TraversesWindowOrderExpressions()
    {
        var model = ParseCorpus("059_range_window_frames.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();

        Assert.Equal(4, resolvedColumnNames.Length);
        Assert.Equal(3, resolvedColumnNames.Count(item => string.Equals(item, "CustomerId", StringComparison.Ordinal)));
        Assert.Equal(1, resolvedColumnNames.Count(item => string.Equals(item, "Amount", StringComparison.Ordinal)));
    }

    [Fact]
    public void BindPercentileWithinGroup_TraversesWithinGroupAndOverExpressions()
    {
        var model = ParseCorpus("057_percentile_within_group.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var resolvedColumnNames = bindingModel.ColumnReferenceList
            .Select(item => bindingModel.ColumnList.Single(column => column.Id == item.ColumnId).Name)
            .ToArray();

        Assert.Equal(5, resolvedColumnNames.Length);
        Assert.Equal(3, resolvedColumnNames.Count(item => string.Equals(item, "CustomerId", StringComparison.Ordinal)));
        Assert.Equal(2, resolvedColumnNames.Count(item => string.Equals(item, "Amount", StringComparison.Ordinal)));
    }

    [Fact]
    public void BindQueryModifiers_DistinctTopOrderBy_BindsWithoutRowsetShapeMutation()
    {
        var sql = """
CREATE VIEW dbo.v_distinct_top_ordered AS
SELECT DISTINCT TOP (100) PERCENT WITH TIES
    s.Id AS SourceId,
    s.OrderDate
FROM dbo.Source AS s
ORDER BY
    SourceId ASC,
    s.OrderDate DESC;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id", "OrderDate"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.DoesNotContain(bound.Issues, item => item.Code == "TopWithTiesRequiresOrderBy");
        Assert.False(bound.HasErrors);
        Assert.NotNull(bound.TopLevelRowset);
        var finalColumns = bound.TopLevelRowset!.Columns
            .OrderBy(item => item.Ordinal)
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["SourceId", "OrderDate"], finalColumns);

        Assert.Equal(3, bound.ColumnReferences.Count);
    }

    [Fact]
    public void BindQueryModifiers_OffsetFetch_TraversesOrderingAndOffsetExpressions()
    {
        var model = ParseCorpus("019_offset_fetch.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id", "OrderDate"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.Equal(3, bound.ColumnReferences.Count);
    }

    [Fact]
    public void BindTopWithTiesWithoutOrderBy_ProducesExplicitIssue()
    {
        var sql = """
CREATE VIEW dbo.v_top_without_order AS
SELECT TOP (5) WITH TIES
    s.Id
FROM dbo.Source AS s;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.Contains(bound.Issues, item => item.Code == "TopWithTiesRequiresOrderBy");
        Assert.True(bound.HasErrors);
    }

    [Fact]
    public void BindingModel_CanRoundTripAsWorkspaceArtifact()
    {
        var model = ParseCorpus("001_basic_select.sql");

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));

        try
        {
            bindingModel.SaveToXmlWorkspace(workspacePath);
            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);

            Assert.Single(reloaded.TransformBindingList);
            Assert.Single(reloaded.OutputRowsetList);
            Assert.NotEmpty(reloaded.RowsetList);
            Assert.NotEmpty(reloaded.ColumnList);
        }
        finally
        {
            if (Directory.Exists(workspacePath))
            {
                Directory.Delete(workspacePath, recursive: true);
            }
        }
    }

    [Fact]
    public void BindingWorkspaceService_CanMaterializeBindingWorkspaceFromTransformWorkspaceOnly()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var result = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                bindingWorkspacePath);

            Assert.Equal(bindingWorkspacePath, result.WorkspacePath);
            Assert.Equal(1, result.TransformScriptCount);
            Assert.Equal(1, result.TransformBindingCount);
            Assert.Equal(1, result.SourceCount);
            Assert.Equal(1, result.TargetCount);
            Assert.Equal(0, result.IssueCount);
            Assert.Equal(0, result.ErrorCount);

            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(bindingWorkspacePath, searchUpward: false);
            Assert.Single(reloaded.TransformBindingList);
            Assert.Single(reloaded.OutputRowsetList);
            Assert.NotEmpty(reloaded.RowsetList);
            Assert.NotEmpty(reloaded.ColumnList);
            var source = Assert.Single(reloaded.RowsetList, item =>
                string.Equals(item.DerivationKind, "Source", StringComparison.Ordinal) &&
                string.Equals(item.SqlIdentifier, "dbo.SourceTable", StringComparison.Ordinal));
            var target = Assert.Single(reloaded.TransformBindingTargetList);
            Assert.Equal("dbo.CustomerSummary", target.SqlIdentifier);
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
    public void BindingWorkspaceService_CanResolveThreePartTargetIdentifiers()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "Warehouse.dbo.CustomerSummary";

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var result = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                bindingWorkspacePath);

            Assert.Equal(1, result.TargetCount);
            Assert.Equal(0, result.IssueCount);

            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(bindingWorkspacePath, searchUpward: false);
            var target = Assert.Single(reloaded.TransformBindingTargetList);
            Assert.Equal("Warehouse.dbo.CustomerSummary", target.SqlIdentifier);
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
    public void BindingWorkspaceService_PersistsUnresolvedTargetsWithoutSchemaValidation()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var result = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                bindingWorkspacePath);

            Assert.Equal(0, result.IssueCount);
            Assert.Equal(0, result.ErrorCount);

            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(bindingWorkspacePath, searchUpward: false);
            Assert.Single(reloaded.TransformBindingTargetList);
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
    public void BindingWorkspaceService_CanRepresentSameScriptBodyWithDifferentTargetsByDuplicatingTransformScriptRows()
    {
        const string sql = """
CREATE VIEW sales.CustomerSummary AS
SELECT
    s.CustomerId,
    s.CustomerName,
    s.CreatedAt AS CreatedAtAlias,
    1 AS LiteralValue
FROM dbo.SourceTable AS s;
GO
CREATE VIEW reporting.CustomerSummaryReplica AS
SELECT
    s.CustomerId,
    s.CustomerName,
    s.CreatedAt AS CreatedAtAlias,
    1 AS LiteralValue
FROM dbo.SourceTable AS s;
GO
""";
        var transformModel = new MetaTransformScript.Sql.MetaTransformScriptSqlService().ImportFromSqlCode(sql);
        Assert.Equal(2, transformModel.TransformScriptList.Count);
        Assert.All(
            transformModel.TransformScriptList,
            script => Assert.False(string.IsNullOrWhiteSpace(script.TargetSqlIdentifier)));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var result = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                bindingWorkspacePath);

            Assert.Equal(2, result.TransformScriptCount);
            Assert.Equal(2, result.TransformBindingCount);
            Assert.Equal(2, result.TargetCount);
            Assert.Equal(0, result.IssueCount);

            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(bindingWorkspacePath, searchUpward: false);
            Assert.Equal(2, reloaded.TransformBindingTargetList.Count);
            Assert.Contains(reloaded.TransformBindingTargetList, item => string.Equals(item.SqlIdentifier, "sales.CustomerSummary", StringComparison.Ordinal));
            Assert.Contains(reloaded.TransformBindingTargetList, item => string.Equals(item.SqlIdentifier, "reporting.CustomerSummaryReplica", StringComparison.Ordinal));
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
    public void BindingWorkspaceService_WithFourPartTargetIdentifier_FailsExplicitly()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "Linked.Warehouse.dbo.CustomerSummary";

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var ex = Assert.Throws<InvalidOperationException>(() => new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                bindingWorkspacePath));

            Assert.Contains("supports table, schema.table, or database.schema.table", ex.Message);
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
    public void BindingWorkspaceService_DoesNotPerformTargetSchemaValidation()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var result = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                bindingWorkspacePath);

            Assert.Equal(0, result.IssueCount);
            Assert.Equal(0, result.ErrorCount);

            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(bindingWorkspacePath, searchUpward: false);
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
    public void ValidationService_ResolvesSourceAndTargetIdentifiersAndRecordsConformance()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerSummaryId", "CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        var identityField = Assert.Single(schemaModel.FieldList, item =>
            string.Equals(item.TableId, "Table:2", StringComparison.Ordinal) &&
            string.Equals(item.Name, "CustomerSummaryId", StringComparison.Ordinal));
        identityField.IsIdentity = "true";
        identityField.IdentitySeed = "1";
        identityField.IdentityIncrement = "1";

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var validated = new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel);

            var validation = Assert.Single(validated.ValidationList);
            Assert.Equal($"{validated.TransformBindingList[0].Id}:validation", validation.Id);

            var sourceValidation = Assert.Single(validated.ValidationSourceRowsetLinkList);
            Assert.Equal("Table:1", sourceValidation.MetaSchemaTableId);
            Assert.Equal(validation.Id, sourceValidation.ValidationId);

            var targetValidation = Assert.Single(validated.ValidationTargetRowsetLinkList);
            Assert.Equal("Table:2", targetValidation.MetaSchemaTableId);
            Assert.Equal(validation.Id, targetValidation.ValidationId);

            Assert.Equal(3, validated.ValidationSourceColumnLinkList.Count);
            Assert.Equal(4, validated.ValidationTargetColumnLinkList.Count);
            Assert.Equal(2, validated.ValidationTargetColumnTypeExactList.Count);
            Assert.Empty(validated.ValidationTargetColumnTypeSanctionedConversionList);
            Assert.Equal(2, validated.ValidationTargetColumnTypeNotClassifiedList.Count);
            Assert.Empty(validated.ValidationTargetIgnoredColumnList);

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
    public void ValidationService_WithTargetMismatch_FailsHard()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias"]));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var ex = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel));
            Assert.Equal("TargetRowsetColumnCountMismatch", ex.Code);
            Assert.Contains("non-identity column(s)", ex.Message);
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
    public void ValidationService_WithAnonymousExprOutputs_ValidatesNamedWriteColumnsOnly()
    {
        var sql = """
CREATE VIEW dbo.v_expr_target AS
SELECT
    s.CustomerId AS CustomerId,
    s.CreatedAt + 1
FROM dbo.SourceTable AS s;
""";
        var transformModel = new MetaTransformScriptSqlParser().ParseSqlCode(sql);
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId"]));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var validated = new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel);
            Assert.Single(validated.ValidationTargetColumnLinkList);
            Assert.Equal("CustomerId", Assert.Single(bindingResult.Model.ColumnList, item =>
                string.Equals(item.Id, validated.ValidationTargetColumnLinkList[0].ColumnId, StringComparison.Ordinal)).Name);
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
    public void ValidationService_WithDuplicateOutputColumnNames_UsesFirstWriteMapping()
    {
        var sql = """
CREATE VIEW dbo.v_dup_target AS
SELECT
    a.CustomerId,
    b.CustomerId
FROM dbo.SourceA AS a
INNER JOIN dbo.SourceB AS b
    ON b.CustomerId = a.CustomerId;
""";
        var transformModel = new MetaTransformScriptSqlParser().ParseSqlCode(sql);
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceA", ["CustomerId"]),
            ("dbo", "SourceB", ["CustomerId"]),
            ("dbo", "CustomerSummary", ["CustomerId"]));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var validated = new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel);
            Assert.Single(validated.ValidationTargetColumnLinkList);
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
    public void ValidationService_WithIgnoredTargetColumns_AllowsPlatformColumns()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue", "LoadUtc"]));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var validated = new TransformBindingValidationService().ApplyValidation(
                bindingResult.Model,
                schemaModel,
                TransformBindingValidationOptions.Create(["LoadUtc"]));

            Assert.Equal(4, validated.ValidationTargetColumnLinkList.Count);
            var ignoredTargetColumn = Assert.Single(validated.ValidationTargetIgnoredColumnList);

            var loadUtcField = Assert.Single(schemaModel.FieldList, item =>
                string.Equals(item.TableId, "Table:2", StringComparison.Ordinal) &&
                string.Equals(item.Name, "LoadUtc", StringComparison.Ordinal));
            Assert.Equal(loadUtcField.Id, ignoredTargetColumn.MetaSchemaFieldId);

            var targetRowsetValidation = Assert.Single(validated.ValidationTargetRowsetLinkList);
            Assert.Equal(targetRowsetValidation.Id, ignoredTargetColumn.ValidationTargetRowsetLinkId);

            Assert.DoesNotContain(
                validated.ValidationTargetColumnLinkList,
                item => string.Equals(item.MetaSchemaFieldId, loadUtcField.Id, StringComparison.Ordinal));
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
    public void ValidationService_WithNullableTargetColumnOmitted_PassesWriteContract()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue", "OptionalComment"]));
        SetFieldIsNullable(schemaModel, "Table:2", "OptionalComment", true);

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var validated = new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel);

            Assert.Equal(4, validated.ValidationTargetColumnLinkList.Count);
            Assert.DoesNotContain(validated.ValidationTargetColumnLinkList, item =>
                string.Equals(item.MetaSchemaFieldId, "Field:2:5", StringComparison.Ordinal));
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
    public void ValidationService_WithRequiredTargetColumnMissing_FailsHard()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue", "MandatoryAuditId"]));
        SetFieldIsNullable(schemaModel, "Table:2", "MandatoryAuditId", false);

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var ex = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel));
            Assert.Equal("TargetRequiredColumnMissing", ex.Code);
            Assert.Contains("MandatoryAuditId", ex.Message);
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
    public void ValidationService_WithUnknownIgnoredTargetColumn_FailsHard()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var ex = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingValidationService().ApplyValidation(
                    bindingResult.Model,
                    schemaModel,
                    TransformBindingValidationOptions.Create(["DoesNotExist"])));
            Assert.Equal("TargetIgnoredColumnNotFound", ex.Code);
            Assert.Contains("DoesNotExist", ex.Message);
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
    public void ValidationService_WithTargetTypeConformanceMismatch_FailsHard()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        SetFieldMetaDataTypeId(schemaModel, "Table:1", "CustomerId", "sqlserver:type:int");
        SetFieldMetaDataTypeId(schemaModel, "Table:1", "CustomerName", "sqlserver:type:nvarchar");
        SetFieldMetaDataTypeId(schemaModel, "Table:1", "CreatedAt", "sqlserver:type:datetime");
        SetFieldMetaDataTypeId(schemaModel, "Table:2", "CustomerId", "sqlserver:type:datetime");
        SetFieldMetaDataTypeId(schemaModel, "Table:2", "CustomerName", "sqlserver:type:nvarchar");
        SetFieldMetaDataTypeId(schemaModel, "Table:2", "CreatedAtAlias", "sqlserver:type:datetime");
        SetFieldMetaDataTypeId(schemaModel, "Table:2", "LiteralValue", "sqlserver:type:int");

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var ex = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel));
            Assert.Equal("TargetColumnTypeConformanceMismatch", ex.Code);
            Assert.Contains("CustomerId", ex.Message);
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
    public void ValidationService_WithSanctionedTypeConversion_RecordsConformanceKind()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        SetFieldMetaDataTypeId(schemaModel, "Table:1", "CustomerId", "sqlserver:type:smallmoney");
        SetFieldMetaDataTypeId(schemaModel, "Table:2", "CustomerId", "sqlserver:type:decimal");

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var validated = new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel);
            var targetColumnLinks = validated.ValidationTargetColumnLinkList;
            Assert.Equal(4, targetColumnLinks.Count);
            Assert.Single(validated.ValidationTargetColumnTypeExactList);
            Assert.Single(validated.ValidationTargetColumnTypeSanctionedConversionList);
            Assert.Equal(2, validated.ValidationTargetColumnTypeNotClassifiedList.Count);

            var customerIdColumn = Assert.Single(bindingResult.Model.ColumnList, item =>
                string.Equals(item.Name, "CustomerId", StringComparison.Ordinal) &&
                string.Equals(item.RowsetId, bindingResult.Model.OutputRowsetList[0].RowsetId, StringComparison.Ordinal));
            var customerIdTargetColumnLink = Assert.Single(targetColumnLinks, item =>
                string.Equals(item.ColumnId, customerIdColumn.Id, StringComparison.Ordinal));
            var customerIdCompatibility = Assert.Single(validated.ValidationTargetColumnTypeSanctionedConversionList, item =>
                string.Equals(item.ValidationTargetColumnLinkId, customerIdTargetColumnLink.Id, StringComparison.Ordinal));
            Assert.Equal("sqlserver:type:smallmoney", customerIdCompatibility.SourceMetaDataTypeId);
            Assert.Equal("sqlserver:type:decimal", customerIdCompatibility.TargetMetaDataTypeId);
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
    public void ValidationService_WithAnsiStringToUnicodeTarget_RecordsSanctionedConversion()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        SetFieldMetaDataTypeId(schemaModel, "Table:1", "CustomerId", "sqlserver:type:char");
        SetFieldMetaDataTypeId(schemaModel, "Table:2", "CustomerId", "sqlserver:type:nvarchar");

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var validated = new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel);
            var targetColumnLinks = validated.ValidationTargetColumnLinkList;
            var customerIdColumn = Assert.Single(bindingResult.Model.ColumnList, item =>
                string.Equals(item.Name, "CustomerId", StringComparison.Ordinal) &&
                string.Equals(item.RowsetId, bindingResult.Model.OutputRowsetList[0].RowsetId, StringComparison.Ordinal));
            var customerIdTargetColumnLink = Assert.Single(targetColumnLinks, item =>
                string.Equals(item.ColumnId, customerIdColumn.Id, StringComparison.Ordinal));
            var customerIdCompatibility = Assert.Single(validated.ValidationTargetColumnTypeSanctionedConversionList, item =>
                string.Equals(item.ValidationTargetColumnLinkId, customerIdTargetColumnLink.Id, StringComparison.Ordinal));

            Assert.Equal("sqlserver:type:char", customerIdCompatibility.SourceMetaDataTypeId);
            Assert.Equal("sqlserver:type:nvarchar", customerIdCompatibility.TargetMetaDataTypeId);
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
    public void ValidationService_WithNumericToUnicodeTarget_RecordsSanctionedConversion()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        SetFieldMetaDataTypeId(schemaModel, "Table:1", "CustomerId", "sqlserver:type:int");
        SetFieldMetaDataTypeId(schemaModel, "Table:2", "CustomerId", "sqlserver:type:nvarchar");

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var validated = new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel);
            var targetColumnLinks = validated.ValidationTargetColumnLinkList;
            var customerIdColumn = Assert.Single(bindingResult.Model.ColumnList, item =>
                string.Equals(item.Name, "CustomerId", StringComparison.Ordinal) &&
                string.Equals(item.RowsetId, bindingResult.Model.OutputRowsetList[0].RowsetId, StringComparison.Ordinal));
            var customerIdTargetColumnLink = Assert.Single(targetColumnLinks, item =>
                string.Equals(item.ColumnId, customerIdColumn.Id, StringComparison.Ordinal));
            var customerIdCompatibility = Assert.Single(validated.ValidationTargetColumnTypeSanctionedConversionList, item =>
                string.Equals(item.ValidationTargetColumnLinkId, customerIdTargetColumnLink.Id, StringComparison.Ordinal));

            Assert.Equal("sqlserver:type:int", customerIdCompatibility.SourceMetaDataTypeId);
            Assert.Equal("sqlserver:type:nvarchar", customerIdCompatibility.TargetMetaDataTypeId);
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
    public void ValidationService_WithTargetNullabilityConformanceMismatch_FailsHard()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        SetFieldIsNullable(schemaModel, "Table:1", "CustomerId", true);
        SetFieldIsNullable(schemaModel, "Table:2", "CustomerId", false);

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var ex = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel));
            Assert.Equal("TargetColumnNullabilityConformanceMismatch", ex.Code);
            Assert.Contains("CustomerId", ex.Message);
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
    public void ValidationService_WithTargetLengthConformanceMismatch_FailsHard()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        SetFieldMetaDataTypeId(schemaModel, "Table:1", "CustomerName", "sqlserver:type:nvarchar");
        SetFieldMetaDataTypeId(schemaModel, "Table:2", "CustomerName", "sqlserver:type:nvarchar");
        SetFieldDataTypeDetail(schemaModel, "Table:1", "CustomerName", "Length", 100);
        SetFieldDataTypeDetail(schemaModel, "Table:2", "CustomerName", "Length", 50);

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var ex = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel));
            Assert.Equal("TargetColumnLengthConformanceMismatch", ex.Code);
            Assert.Contains("CustomerName", ex.Message);
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
    public void ValidationService_WithTargetPrecisionConformanceMismatch_FailsHard()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        SetFieldMetaDataTypeId(schemaModel, "Table:1", "CustomerId", "sqlserver:type:decimal");
        SetFieldMetaDataTypeId(schemaModel, "Table:2", "CustomerId", "sqlserver:type:decimal");
        SetFieldDataTypeDetail(schemaModel, "Table:1", "CustomerId", "Precision", 18);
        SetFieldDataTypeDetail(schemaModel, "Table:2", "CustomerId", "Precision", 10);

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var ex = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel));
            Assert.Equal("TargetColumnPrecisionConformanceMismatch", ex.Code);
            Assert.Contains("CustomerId", ex.Message);
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
    public void ValidationService_WithTargetScaleConformanceMismatch_FailsHard()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        SetFieldMetaDataTypeId(schemaModel, "Table:1", "CustomerId", "sqlserver:type:decimal");
        SetFieldMetaDataTypeId(schemaModel, "Table:2", "CustomerId", "sqlserver:type:decimal");
        SetFieldDataTypeDetail(schemaModel, "Table:1", "CustomerId", "Scale", 6);
        SetFieldDataTypeDetail(schemaModel, "Table:2", "CustomerId", "Scale", 2);

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var ex = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel));
            Assert.Equal("TargetColumnScaleConformanceMismatch", ex.Code);
            Assert.Contains("CustomerId", ex.Message);
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
    public void ValidationService_WithUnsanctionedSourceFieldType_FailsHard()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        SetFieldMetaDataTypeId(schemaModel, "Table:1", "CustomerId", "custom:type:unsupported");

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var ex = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel));
            Assert.Equal("SourceSchemaFieldMetaDataTypeNotSanctioned", ex.Code);
            Assert.Contains("custom:type:unsupported", ex.Message);
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
    public void ValidationService_WithAmbiguousOnePartSourceIdentifier_FailsHard()
    {
        var sql = """
CREATE VIEW dbo.v_ambiguous_source AS
SELECT
    s.CustomerId
FROM SourceTable AS s;
""";
        var transformModel = new MetaTransformScriptSqlParser().ParseSqlCode(sql);
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId"]),
            ("sales", "SourceTable", ["CustomerId"]),
            ("dbo", "CustomerSummary", ["CustomerId"]));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var ex = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel));
            Assert.Equal("SourceSchemaTableAmbiguous", ex.Code);
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
    public void ValidationService_WithAmbiguousOnePartTargetIdentifier_FailsHard()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]),
            ("reporting", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var ex = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel));
            Assert.Equal("TargetSchemaTableAmbiguous", ex.Code);
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
    public void ValidationWorkspaceService_CanMaterializeValidatedWorkspaceFromBindingAndSchemaWorkspaces()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerSummaryId", "CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        var identityField = Assert.Single(schemaModel.FieldList, item =>
            string.Equals(item.TableId, "Table:2", StringComparison.Ordinal) &&
            string.Equals(item.Name, "CustomerSummaryId", StringComparison.Ordinal));
        identityField.IsIdentity = "true";
        identityField.IdentitySeed = "1";
        identityField.IdentityIncrement = "1";

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");
        var schemaWorkspacePath = Path.Combine(tempRoot, "SchemaWorkspace");
        var validatedWorkspacePath = Path.Combine(tempRoot, "ValidatedBindingWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);
            schemaModel.SaveToXmlWorkspace(schemaWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                bindingWorkspacePath);
            Assert.Equal(0, bindingResult.ErrorCount);

            var result = new TransformBindingValidationWorkspaceService().ValidateWorkspace(
                bindingWorkspacePath,
                schemaWorkspacePath,
                validatedWorkspacePath);

            Assert.Equal(1, result.TransformBindingCount);
            Assert.Equal(1, result.SourceRowsetValidationCount);
            Assert.Equal(1, result.TargetRowsetValidationCount);
            Assert.Equal(3, result.SourceColumnValidationCount);
            Assert.Equal(4, result.TargetColumnValidationCount);

            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(validatedWorkspacePath, searchUpward: false);
            Assert.Single(reloaded.ValidationList);
            Assert.Single(reloaded.ValidationSourceRowsetLinkList);
            Assert.Single(reloaded.ValidationTargetRowsetLinkList);
            Assert.Equal(3, reloaded.ValidationSourceColumnLinkList.Count);
            Assert.Equal(4, reloaded.ValidationTargetColumnLinkList.Count);
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
    public void ValidationService_CanRoundTripValidationRowsAsWorkspaceArtifact()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].TargetSqlIdentifier = "dbo.CustomerSummary";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"]));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var validatedWorkspacePath = Path.Combine(tempRoot, "ValidatedBindingWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                Path.Combine(tempRoot, "BindingWorkspace"));

            var validated = new TransformBindingValidationService().ApplyValidation(bindingResult.Model, schemaModel);
            validated.SaveToXmlWorkspace(validatedWorkspacePath);

            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(validatedWorkspacePath, searchUpward: false);
            Assert.Single(reloaded.ValidationList);
            Assert.Single(reloaded.ValidationSourceRowsetLinkList);
            Assert.Single(reloaded.ValidationTargetRowsetLinkList);
            Assert.Equal(3, reloaded.ValidationSourceColumnLinkList.Count);
            Assert.Equal(4, reloaded.ValidationTargetColumnLinkList.Count);
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
    public void BindInlineTableValuedFunction_WiresSourceRowsetsWithoutTreatingParametersAsSourceColumns()
    {
        var model = ParseCorpus("066_inline_tvf.sql");

        var sourceSchema = CreateSourceSchema(
            ("sales", "CustomerOrder", ["CustomerId", "OrderDate", "OrderAmount"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.DoesNotContain(bound.Issues, item => item.Code == "FunctionParameterReferenceNotFound");

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var sourceRowset = Assert.Single(bindingModel.RowsetList, item => item.DerivationKind == "Source");
        var sourceColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == sourceRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();

        Assert.Equal(["CustomerId", "OrderDate", "OrderAmount"], sourceColumns);
        Assert.DoesNotContain(sourceColumns, static item => item.StartsWith("@", StringComparison.Ordinal));

        var finalLink = Assert.Single(bindingModel.OutputRowsetList);
        var finalColumns = bindingModel.ColumnList
            .Where(item => item.RowsetId == finalLink.RowsetId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "OrderDate", "OrderAmount"], finalColumns);
    }

    [Fact]
    public void BindInlineTableValuedFunction_WithUnknownParameterReference_ProducesExplicitIssue()
    {
        const string sql = """
CREATE FUNCTION dbo.fn_bad_parameter
(
    @CustomerId int
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        o.CustomerId
    FROM sales.CustomerOrder AS o
    WHERE o.CustomerId = @MissingParameter
);
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        var sourceSchema = CreateSourceSchema(
            ("sales", "CustomerOrder", ["CustomerId"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        var issue = Assert.Single(bound.Issues, item => item.Code == "FunctionParameterReferenceNotFound");
        Assert.Contains("@MissingParameter", issue.Message);
        Assert.True(bound.HasErrors);
    }

    private static MetaTransformScriptModel ParseCorpus(string fileName)
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

        return new MetaTransformScriptSqlParser().ParseSqlCode(File.ReadAllText(path));
    }

    private static MetaSchemaModel CreateSourceSchema(params (string SchemaName, string TableName, string[] Columns)[] tables)
    {
        return CreateSchema("TestSystem", tables);
    }

    private static MetaSchemaModel CreateSchema(string systemName, params (string SchemaName, string TableName, string[] Columns)[] tables)
    {
        var model = MetaSchemaModel.CreateEmpty();
        var system = new MetaSchema.System
        {
            Id = "System:1",
            Name = systemName
        };
        model.SystemList.Add(system);

        var schemaIdsByName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var tableOrdinal = 0;

        foreach (var table in tables)
        {
            if (!schemaIdsByName.TryGetValue(table.SchemaName, out var schemaId))
            {
                schemaId = $"Schema:{schemaIdsByName.Count + 1}";
                schemaIdsByName.Add(table.SchemaName, schemaId);
                model.SchemaList.Add(new Schema
                {
                    Id = schemaId,
                    SystemId = system.Id,
                    Name = table.SchemaName
                });
            }

            var tableId = $"Table:{++tableOrdinal}";
            model.TableList.Add(new Table
            {
                Id = tableId,
                SchemaId = schemaId,
                Name = table.TableName
            });

            for (var i = 0; i < table.Columns.Length; i++)
            {
                model.FieldList.Add(new Field
                {
                    Id = $"Field:{tableOrdinal}:{i + 1}",
                    TableId = tableId,
                    Name = table.Columns[i],
                    MetaDataTypeId = "sqlserver:type:int",
                    IsNullable = "false",
                    Ordinal = i.ToString()
                });
            }
        }

        return model;
    }

    private static void SetFieldMetaDataTypeId(MetaSchemaModel schemaModel, string tableId, string fieldName, string metaDataTypeId)
    {
        var field = Assert.Single(schemaModel.FieldList, item =>
            string.Equals(item.TableId, tableId, StringComparison.Ordinal) &&
            string.Equals(item.Name, fieldName, StringComparison.Ordinal));
        field.MetaDataTypeId = metaDataTypeId;
    }

    private static void SetFieldIsNullable(MetaSchemaModel schemaModel, string tableId, string fieldName, bool isNullable)
    {
        var field = Assert.Single(schemaModel.FieldList, item =>
            string.Equals(item.TableId, tableId, StringComparison.Ordinal) &&
            string.Equals(item.Name, fieldName, StringComparison.Ordinal));
        field.IsNullable = isNullable ? "true" : "false";
    }

    private static void SetFieldDataTypeDetail(
        MetaSchemaModel schemaModel,
        string tableId,
        string fieldName,
        string detailName,
        int detailValue)
    {
        var field = Assert.Single(schemaModel.FieldList, item =>
            string.Equals(item.TableId, tableId, StringComparison.Ordinal) &&
            string.Equals(item.Name, fieldName, StringComparison.Ordinal));

        var existing = schemaModel.FieldDataTypeDetailList
            .SingleOrDefault(item =>
                string.Equals(item.FieldId, field.Id, StringComparison.Ordinal) &&
                string.Equals(item.Name, detailName, StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
        {
            existing.Value = detailValue.ToString();
            return;
        }

        schemaModel.FieldDataTypeDetailList.Add(new FieldDataTypeDetail
        {
            Id = $"FieldDataTypeDetail:{schemaModel.FieldDataTypeDetailList.Count + 1}",
            FieldId = field.Id,
            Name = detailName,
            Value = detailValue.ToString()
        });
    }
}

