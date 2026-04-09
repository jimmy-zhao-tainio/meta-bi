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
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.NotNull(bound.TopLevelRowset);
        Assert.Equal(
            ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"],
            bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
        Assert.Equal(3, bound.BoundColumnReferences.Count);
    }

    [Fact]
    public void BindSelectStarAcrossJoinedNamedSources_ExpandsColumnsInVisibleOrder()
    {
        var model = ParseCorpus("002_select_star.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

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
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customer", ["Id", "CustomerId"]),
            ("dbo", "Order", ["Id", "CustomerId"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        var issue = Assert.Single(bound.Issues, item => item.Code == "ColumnReferenceAmbiguous");
        Assert.Equal("ColumnReferenceAmbiguous", issue.Code);
    }

    [Fact]
    public void BindWithoutResolvedLanguageProfile_FailsExplicitly()
    {
        var model = ParseCorpus("001_basic_select.sql");
        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        var issue = Assert.Single(bound.Issues);
        Assert.Equal("ActiveLanguageProfileMissing", issue.Code);
        Assert.Null(bound.TopLevelScope);
        Assert.Null(bound.TopLevelRowset);
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
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, CreateSourceSchema());

        Assert.Empty(bindingModel.BoundTableSourceList);
        Assert.Empty(bindingModel.BoundRowsetInputList);

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.Id == finalLink.ValueId);
        Assert.Equal("FinalOutput", finalRowset.RowsetRole);
        Assert.Equal(
            ["Id", "Name"],
            bindingModel.BoundColumnList
                .Where(item => item.OwnerId == finalRowset.Id)
                .OrderBy(item => int.Parse(item.Ordinal))
                .Select(item => item.Name)
                .ToArray());
    }

    [Fact]
    public void BindSimpleSelect_EmitsBindingModelWithFinalOutputRowset()
    {
        var model = ParseCorpus("001_basic_select.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var binding = Assert.Single(bindingModel.TransformBindingList);
        Assert.Equal(model.TransformScriptList[0].Id, binding.TransformScriptId);

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.Id == finalLink.ValueId);
        Assert.Equal("FinalOutput", finalRowset.RowsetRole);
        Assert.Equal(
            ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"],
            bindingModel.BoundColumnList
                .Where(item => item.OwnerId == finalRowset.Id)
                .OrderBy(item => int.Parse(item.Ordinal))
                .Select(item => item.Name)
                .ToArray());

        Assert.Equal(3, bindingModel.BoundColumnReferenceList.Count);
        Assert.All(
            bindingModel.BoundColumnReferenceList,
            item =>
            {
                Assert.Contains(bindingModel.BoundColumnList, column => column.Id == item.ValueId);
                Assert.Contains(bindingModel.BoundTableSourceList, source => source.Id == item.ResolvedTableSourceId);
            });
    }

    [Fact]
    public void BindJoinedSources_EmitsIntermediateJoinRowsetGraph()
    {
        var model = ParseCorpus("002_select_star.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customers", ["CustomerId", "CustomerName"]),
            ("dbo", "Orders", ["OrderId", "CustomerId"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var joinRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.DerivationKind == "Join");
        var joinInputs = bindingModel.BoundRowsetInputList
            .Where(item => item.OwnerId == joinRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .ToArray();

        Assert.Equal(2, joinInputs.Length);
        Assert.All(joinInputs, input => Assert.Contains(
            bindingModel.BoundRowsetList,
            rowset => rowset.Id == input.ValueId && rowset.DerivationKind == "Source"));

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalInput = Assert.Single(bindingModel.BoundRowsetInputList, item => item.OwnerId == finalLink.ValueId);
        Assert.Equal(joinRowset.Id, finalInput.ValueId);
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
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customers", ["CustomerId", "CustomerName"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var derivedTableRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.DerivationKind == "DerivedTable");
        var derivedTableInput = Assert.Single(bindingModel.BoundRowsetInputList, item => item.OwnerId == derivedTableRowset.Id);
        var innerProjectionRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.Id == derivedTableInput.ValueId);
        Assert.Equal("Projection", innerProjectionRowset.DerivationKind);
        Assert.Empty(innerProjectionRowset.RowsetRole);

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalInput = Assert.Single(bindingModel.BoundRowsetInputList, item => item.OwnerId == finalLink.ValueId);
        Assert.Equal(derivedTableRowset.Id, finalInput.ValueId);

        var derivedTableSource = Assert.Single(bindingModel.BoundTableSourceList, item => item.ValueId == derivedTableRowset.Id);
        Assert.Equal("d", derivedTableSource.ExposedName);
    }

    [Fact]
    public void BindInlineDerivedTable_EmitsInlineDerivedRowsetAndFinalProjection()
    {
        var model = ParseCorpus("021_inline_values.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, CreateSourceSchema());

        Assert.Empty(bindingModel.BoundIssueList);

        var inlineDerivedRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.DerivationKind == "InlineDerivedTable");
        var inlineDerivedColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == inlineDerivedRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["Id", "Name"], inlineDerivedColumns);

        var inlineSource = Assert.Single(bindingModel.BoundTableSourceList, item => item.ValueId == inlineDerivedRowset.Id);
        Assert.Equal("src", inlineSource.ExposedName);

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalInput = Assert.Single(bindingModel.BoundRowsetInputList, item => item.OwnerId == finalLink.ValueId);
        Assert.Equal(inlineDerivedRowset.Id, finalInput.ValueId);

        var finalColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == finalLink.ValueId)
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
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var bound = new TransformBindingService().BindSingleTransform(model, CreateSourceSchema());

        Assert.Contains(bound.Issues, item => item.Code == "InlineDerivedTableColumnAliasesRequired");
        Assert.True(bound.HasErrors);
    }

    [Fact]
    public void BindCommonTableExpressionWithColumnAliases_EmitsCteRowsetAndFinalProjection()
    {
        var model = ParseCorpus("042_cte_column_list.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var cteRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.DerivationKind == "CommonTableExpression");
        var cteColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == cteRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "TotalAmount"], cteColumns);

        var cteSource = Assert.Single(bindingModel.BoundTableSourceList, item => item.ValueId == cteRowset.Id);
        Assert.Equal("CustomerAmounts", cteSource.ExposedName);

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalInput = Assert.Single(bindingModel.BoundRowsetInputList, item => item.OwnerId == finalLink.ValueId);
        Assert.Equal(cteRowset.Id, finalInput.ValueId);
    }

    [Fact]
    public void BindRecursiveCommonTableExpression_DerivesRecursiveCteRowsetShape()
    {
        var model = ParseCorpus("043_recursive_cte_column_list.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, CreateSourceSchema());

        Assert.DoesNotContain(bindingModel.BoundIssueList, item => item.Code == "RecursiveCommonTableExpressionNotYetSupported");

        var cteRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.DerivationKind == "CommonTableExpression");
        var cteColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == cteRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["N"], cteColumns);

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == finalLink.ValueId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["N"], finalColumns);
    }

    [Fact]
    public void BindRecursiveCommonTableExpression_WithoutExplicitColumnList_DerivesAnchorNamedShape()
    {
        var model = ParseCorpus("017_cte.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id", "ParentId"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.DoesNotContain(bindingModel.BoundIssueList, item => item.Code == "RecursiveCommonTableExpressionNotYetSupported");

        var recursiveCteRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.Name == "recursive_cte");
        var recursiveCteColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == recursiveCteRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["Id", "ParentId", "Depth"], recursiveCteColumns);
    }

    [Fact]
    public void BindSetOperations_DerivesSetOperationRowsetsAndFinalOutput()
    {
        var model = ParseCorpus("013_set_operations.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "A", ["Id", "Code"]),
            ("dbo", "B", ["Id", "Code"]),
            ("dbo", "C", ["Id", "Code"]),
            ("dbo", "D", ["Id", "Code"]),
            ("dbo", "E", ["Id", "Code"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.DoesNotContain(bindingModel.BoundIssueList, item => item.Code.StartsWith("SetOperation", StringComparison.Ordinal));
        Assert.Contains(bindingModel.BoundRowsetList, item => item.DerivationKind == "SetOperation");

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.Id == finalLink.ValueId);
        Assert.Equal("SetOperation", finalRowset.DerivationKind);
        Assert.Equal("FinalOutput", finalRowset.RowsetRole);

        var finalColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == finalRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["Id", "Code"], finalColumns);
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
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["Id", "CsvValue"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Empty(bindingModel.BoundIssueList);

        var applyRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.DerivationKind == "Apply");
        var applyInputs = bindingModel.BoundRowsetInputList
            .Where(item => item.OwnerId == applyRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .ToArray();
        Assert.Equal(["ApplyLeft", "ApplyRight"], applyInputs.Select(item => item.InputRole).ToArray());

        var applySource = Assert.Single(bindingModel.BoundTableSourceList, item => item.ExposedName == "applySource");
        Assert.Contains(bindingModel.BoundRowsetList, item => item.Id == applySource.ValueId && item.DerivationKind == "DerivedTable");

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalInput = Assert.Single(bindingModel.BoundRowsetInputList, item => item.OwnerId == finalLink.ValueId);
        Assert.Equal(applyRowset.Id, finalInput.ValueId);
        var finalColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == finalLink.ValueId)
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
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["Id", "CsvValue"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Empty(bindingModel.BoundIssueList);

        var functionRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.DerivationKind == "FunctionTableReference");
        var functionColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == functionRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["ValueText"], functionColumns);

        var functionSource = Assert.Single(bindingModel.BoundTableSourceList, item => item.ExposedName == "splitItem");
        Assert.Equal(functionRowset.Id, functionSource.ValueId);
    }

    [Fact]
    public void BindCrossApplyFunctionTableReference_WithoutColumnAliases_ProducesExplicitIssue()
    {
        var model = ParseCorpus("004_apply_sources.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["Id", "CsvValue"]),
            ("dbo", "Orders", ["Amount", "SourceId"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.Contains(bound.Issues, item => item.Code == "FunctionTableReferenceColumnAliasesRequired");
        Assert.True(bound.HasErrors);
    }

    [Fact]
    public void BindCorrelatedScalarSubqueryAndExists_PreservesCorrelationBinding()
    {
        var model = ParseCorpus("011_subqueries_and_correlation.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customers", ["CustomerId"]),
            ("dbo", "Orders", ["CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Empty(bindingModel.BoundIssueList);
        Assert.Equal(6, bindingModel.BoundColumnReferenceList.Count);

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == finalLink.ValueId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "MaxAmount", "HasOrders"], finalColumns);

        Assert.Contains(
            bindingModel.BoundTableSourceList,
            item => item.ExposedName == "c");
        Assert.Contains(
            bindingModel.BoundTableSourceList,
            item => item.ExposedName == "o");
        Assert.Contains(
            bindingModel.BoundTableSourceList,
            item => item.ExposedName == "o2");
    }

    [Fact]
    public void BindSubqueryPredicates_BindsCorrelatedAndUncorrelatedPredicateSubqueries()
    {
        var model = ParseCorpus("012_subquery_predicates.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id", "Amount", "Code", "GroupId"]),
            ("dbo", "Target", ["Amount", "Code", "GroupId", "SourceId"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Empty(bindingModel.BoundIssueList);
        Assert.Equal(11, bindingModel.BoundColumnReferenceList.Count);

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == finalLink.ValueId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["Id"], finalColumns);
    }

    [Fact]
    public void BindValueExpressions_BindsThroughCommonScalarExpressionShells()
    {
        var model = ParseCorpus("014_value_expressions.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Source", ["Id", "Status", "Amount", "PreferredName", "LegalName", "Code", "IsActive", "Priority", "Score", "CreatedAt", "CreatedAtText", "CustomerName"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Empty(bindingModel.BoundIssueList);
        Assert.Equal(14, bindingModel.BoundColumnReferenceList.Count);

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == finalLink.ValueId)
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
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Empty(bindingModel.BoundIssueList);
        Assert.Equal(4, bindingModel.BoundColumnReferenceList.Count);
    }

    [Fact]
    public void BindNestedScalarSubqueries_BindsAggregateArgumentsInsideNestedQueryBoundaries()
    {
        var model = ParseCorpus("045_nested_subqueries.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customer", ["CustomerId"]),
            ("dbo", "Sales", ["CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Empty(bindingModel.BoundIssueList);
        Assert.Equal(4, bindingModel.BoundColumnReferenceList.Count);

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == finalLink.ValueId)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId", "NestedMaxAmount"], finalColumns);
    }

    [Fact]
    public void BindGroupByHaving_EmitsGroupedRowsetAndFinalProjection()
    {
        var model = ParseCorpus("008_group_by_having.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CustomerId", "Amount"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        Assert.Empty(bindingModel.BoundIssueList);

        var groupedRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.DerivationKind == "Grouping");
        var groupedColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == groupedRowset.Id)
            .OrderBy(item => int.Parse(item.Ordinal))
            .Select(item => item.Name)
            .ToArray();
        Assert.Equal(["CustomerId"], groupedColumns);

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalInput = Assert.Single(bindingModel.BoundRowsetInputList, item => item.OwnerId == finalLink.ValueId);
        Assert.Equal(groupedRowset.Id, finalInput.ValueId);

        var finalColumns = bindingModel.BoundColumnList
            .Where(item => item.OwnerId == finalLink.ValueId)
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
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Sales", ["CustomerId", "Amount"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.Contains(bound.Issues, item => item.Code == "UngroupedColumnReference");
        Assert.True(bound.HasErrors);
    }

    [Fact]
    public void BindingModel_CanRoundTripAsWorkspaceArtifact()
    {
        var model = ParseCorpus("001_basic_select.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));

        try
        {
            bindingModel.SaveToXmlWorkspace(workspacePath);
            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);

            Assert.Single(reloaded.TransformBindingList);
            Assert.Single(reloaded.TransformBindingFinalRowsetLinkList);
            Assert.NotEmpty(reloaded.BoundRowsetList);
            Assert.NotEmpty(reloaded.BoundColumnList);
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
    public void BindingWorkspaceService_CanMaterializeBindingWorkspaceFromTransformAndSchemaWorkspaces()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAt"]));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var schemaWorkspacePath = Path.Combine(tempRoot, "SchemaWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);
            schemaModel.SaveToXmlWorkspace(schemaWorkspacePath);

            var result = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                schemaWorkspacePath,
                bindingWorkspacePath,
                ["dbo.CustomerSummary"]);

            Assert.Equal(bindingWorkspacePath, result.WorkspacePath);
            Assert.Equal(transformModel.TransformScriptList[0].Name, result.TransformScriptName);
            Assert.Equal(1, result.TransformBindingCount);
            Assert.Equal(1, result.SourceCount);
            Assert.Equal(1, result.TargetCount);

            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(bindingWorkspacePath, searchUpward: false);
            Assert.Single(reloaded.TransformBindingList);
            Assert.Single(reloaded.TransformBindingFinalRowsetLinkList);
            Assert.NotEmpty(reloaded.BoundRowsetList);
            Assert.NotEmpty(reloaded.BoundColumnList);
            var source = Assert.Single(reloaded.TransformBindingSourceList);
            Assert.Equal("dbo.SourceTable", source.SqlIdentifier);
            Assert.Equal("Table:1", source.TableId);
            var target = Assert.Single(reloaded.TransformBindingTargetList);
            Assert.Equal("dbo.CustomerSummary", target.SqlIdentifier);
            Assert.Equal("Table:2", target.TableId);
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
        transformModel.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var schemaModel = CreateSchema(
            "Warehouse",
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAt"]));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var schemaWorkspacePath = Path.Combine(tempRoot, "SchemaWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);
            schemaModel.SaveToXmlWorkspace(schemaWorkspacePath);

            var result = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                schemaWorkspacePath,
                bindingWorkspacePath,
                ["Warehouse.dbo.CustomerSummary"]);

            Assert.Equal(1, result.TargetCount);

            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(bindingWorkspacePath, searchUpward: false);
            var target = Assert.Single(reloaded.TransformBindingTargetList);
            Assert.Equal("Warehouse.dbo.CustomerSummary", target.SqlIdentifier);
            Assert.Equal("Table:2", target.TableId);
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
    public void BindingWorkspaceService_CanPersistMultipleTargetsForOneTransform()
    {
        var transformModel = ParseCorpus("001_basic_select.sql");
        transformModel.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var schemaModel = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("reporting", "CustomerSummaryReplica", ["CustomerId", "CustomerName", "CreatedAt"]));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var schemaWorkspacePath = Path.Combine(tempRoot, "SchemaWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);
            schemaModel.SaveToXmlWorkspace(schemaWorkspacePath);

            var result = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                schemaWorkspacePath,
                bindingWorkspacePath,
                ["dbo.CustomerSummary", "reporting.CustomerSummaryReplica"]);

            Assert.Equal(2, result.TargetCount);

            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(bindingWorkspacePath, searchUpward: false);
            var targets = reloaded.TransformBindingTargetList
                .OrderBy(item => item.SqlIdentifier, StringComparer.Ordinal)
                .ToArray();
            Assert.Equal(2, targets.Length);
            Assert.Equal("dbo.CustomerSummary", targets[0].SqlIdentifier);
            Assert.Equal("Table:2", targets[0].TableId);
            Assert.Equal("reporting.CustomerSummaryReplica", targets[1].SqlIdentifier);
            Assert.Equal("Table:3", targets[1].TableId);
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
        transformModel.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var schemaModel = CreateSchema(
            "Warehouse",
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]),
            ("dbo", "CustomerSummary", ["CustomerId", "CustomerName", "CreatedAt"]));

        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var schemaWorkspacePath = Path.Combine(tempRoot, "SchemaWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");

        try
        {
            transformModel.SaveToXmlWorkspace(transformWorkspacePath);
            schemaModel.SaveToXmlWorkspace(schemaWorkspacePath);

            var ex = Assert.Throws<InvalidOperationException>(() => new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspacePath,
                schemaWorkspacePath,
                bindingWorkspacePath,
                ["Linked.Warehouse.dbo.CustomerSummary"]));

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
                    Ordinal = i.ToString()
                });
            }
        }

        return model;
    }
}
