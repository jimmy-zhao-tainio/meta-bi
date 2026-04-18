using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private RuntimeQueryBindingResult? BindQueryExpression(
        string queryExpressionId,
        string outputRowsetId,
        string outputRowsetName,
        string? outputRowsetRole,
        int visibleCommonTableExpressionOrdinal,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources,
        RuntimeRowset? inheritedInputRowset,
        IReadOnlyList<string>? expectedOutputColumnNames)
    {
        var querySpecification = navigator.TryGetQuerySpecification(queryExpressionId);
        if (querySpecification is not null)
        {
            var querySpecificationBinding = BindQuerySpecification(
                querySpecification,
                outputRowsetId,
                outputRowsetName,
                outputRowsetRole,
                visibleCommonTableExpressionOrdinal,
                inheritedVisibleTableSources,
                inheritedInputRowset,
                expectedOutputColumnNames);
            BindQueryExpressionModifiers(queryExpressionId, querySpecification, querySpecificationBinding);
            return querySpecificationBinding;
        }

        var binaryQueryExpression = navigator.TryGetBinaryQueryExpression(queryExpressionId);
        if (binaryQueryExpression is not null)
        {
            var binaryExpressionBinding = BindBinaryQueryExpression(
                binaryQueryExpression,
                outputRowsetId,
                outputRowsetName,
                outputRowsetRole,
                visibleCommonTableExpressionOrdinal,
                inheritedVisibleTableSources,
                inheritedInputRowset,
                expectedOutputColumnNames);
            BindQueryExpressionModifiers(queryExpressionId, null, binaryExpressionBinding);
            return binaryExpressionBinding;
        }

        var queryParenthesisExpression = navigator.TryGetQueryParenthesisExpression(queryExpressionId);
        if (queryParenthesisExpression is not null)
        {
            var innerQueryExpressionId = navigator.TryGetQueryParenthesisExpressionInnerQueryExpressionId(queryParenthesisExpression);
            if (string.IsNullOrWhiteSpace(innerQueryExpressionId))
            {
                issues.Add(new TransformBindingIssue(
                    "QueryParenthesisExpressionInnerQueryMissing",
                    $"QueryParenthesisExpression '{queryParenthesisExpression.Id}' is missing its inner query expression.",
                    queryParenthesisExpression.Id));
                return null;
            }

            var innerBinding = BindQueryExpression(
                innerQueryExpressionId,
                outputRowsetId,
                outputRowsetName,
                outputRowsetRole,
                visibleCommonTableExpressionOrdinal,
                inheritedVisibleTableSources,
                inheritedInputRowset,
                expectedOutputColumnNames);
            BindQueryExpressionModifiers(queryExpressionId, null, innerBinding);
            return innerBinding;
        }

        issues.Add(new TransformBindingIssue(
            "UnsupportedQueryExpressionShape",
            $"QueryExpression '{queryExpressionId}' is not yet supported by binding.",
            queryExpressionId));
        return null;
    }

    private void BindQueryExpressionModifiers(
        string queryExpressionId,
        QuerySpecification? querySpecification,
        RuntimeQueryBindingResult? binding)
    {
        if (binding is null)
        {
            return;
        }

        var queryLevelOrderByClause = navigator.TryGetQueryExpressionOrderByClause(queryExpressionId);

        if (querySpecification is not null)
        {
            BindUniqueRowFilter(querySpecification);

            var topRowFilter = navigator.TryGetTopRowFilter(querySpecification);
            if (topRowFilter is not null)
            {
                BindTopRowFilter(topRowFilter, binding.Scope, binding.InputRowset, queryLevelOrderByClause is not null);
            }
        }

        if (queryLevelOrderByClause is not null)
        {
            BindQueryOrderByClause(
                queryLevelOrderByClause,
                binding.Scope,
                binding.InputRowset,
                binding.OutputRowset);
        }

        var offsetClause = navigator.TryGetQueryExpressionOffsetClause(queryExpressionId);
        if (offsetClause is not null)
        {
            BindOffsetClause(
                offsetClause,
                binding.Scope,
                binding.InputRowset,
                queryLevelOrderByClause is not null);
        }
    }

    private void BindUniqueRowFilter(QuerySpecification querySpecification)
    {
        if (navigator.HasUnsupportedUniqueRowFilter(querySpecification))
        {
            issues.Add(new TransformBindingIssue(
                "UnsupportedUniqueRowFilter",
                $"QuerySpecification '{querySpecification.Id}' uses unsupported unique row filter '{querySpecification.UniqueRowFilter}'.",
                querySpecification.Id));
        }
    }

    private void BindTopRowFilter(
        TopRowFilter topRowFilter,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        bool hasQueryLevelOrderBy)
    {
        var topExpression = navigator.TryGetTopRowFilterExpression(topRowFilter);
        if (topExpression is null)
        {
            issues.Add(new TransformBindingIssue(
                "TopRowFilterExpressionMissing",
                $"TopRowFilter '{topRowFilter.Id}' is missing its expression.",
                topRowFilter.Id));
        }
        else
        {
            BindScalarExpression(topExpression, scope, inputRowset, groupingContext: null, withinAggregate: false);
        }

        if (navigator.IsTopRowFilterWithTies(topRowFilter) && !hasQueryLevelOrderBy)
        {
            issues.Add(new TransformBindingIssue(
                "TopWithTiesRequiresOrderBy",
                $"TopRowFilter '{topRowFilter.Id}' uses WITH TIES without a query-level ORDER BY clause.",
                topRowFilter.Id));
        }
    }

    private void BindQueryOrderByClause(
        OrderByClause orderByClause,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        RuntimeRowset outputRowset)
    {
        var outputAliasNames = outputRowset.Columns
            .Select(item => item.Name)
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        orderByOutputAliasScopeStack.Push(outputAliasNames);
        try
        {
            foreach (var orderByElement in navigator.GetOrderByElements(orderByClause))
            {
                var expression = navigator.TryGetExpressionWithSortOrderExpression(orderByElement);
                if (expression is null)
                {
                    continue;
                }

                if (TryResolveQueryOrderByOutputAlias(expression, outputRowset))
                {
                    continue;
                }

                BindScalarExpression(expression, scope, inputRowset, groupingContext: null, withinAggregate: false);
            }
        }
        finally
        {
            orderByOutputAliasScopeStack.Pop();
        }
    }

    private bool TryResolveQueryOrderByOutputAlias(
        ScalarExpression expression,
        RuntimeRowset outputRowset)
    {
        var directColumnReference = navigator.TryGetDirectColumnReference(expression);
        if (directColumnReference is null)
        {
            return false;
        }

        var parts = navigator.GetColumnReferenceParts(directColumnReference);
        if (parts.Count != 1)
        {
            return false;
        }

        return outputRowset.Columns.Any(item => string.Equals(item.Name, parts[0], StringComparison.OrdinalIgnoreCase));
    }

    private void BindOffsetClause(
        OffsetClause offsetClause,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        bool hasQueryLevelOrderBy)
    {
        if (!hasQueryLevelOrderBy)
        {
            issues.Add(new TransformBindingIssue(
                "OffsetClauseRequiresOrderBy",
                $"OffsetClause '{offsetClause.Id}' requires a query-level ORDER BY clause.",
                offsetClause.Id));
        }

        var offsetExpression = navigator.TryGetOffsetClauseOffsetExpression(offsetClause);
        if (offsetExpression is null)
        {
            issues.Add(new TransformBindingIssue(
                "OffsetClauseOffsetExpressionMissing",
                $"OffsetClause '{offsetClause.Id}' is missing its offset expression.",
                offsetClause.Id));
        }
        else
        {
            BindScalarExpression(offsetExpression, scope, inputRowset, groupingContext: null, withinAggregate: false);
        }

        var fetchExpression = navigator.TryGetOffsetClauseFetchExpression(offsetClause);
        if (fetchExpression is not null)
        {
            BindScalarExpression(fetchExpression, scope, inputRowset, groupingContext: null, withinAggregate: false);
        }
    }

    private RuntimeQueryBindingResult? BindBinaryQueryExpression(
        BinaryQueryExpression binaryQueryExpression,
        string outputRowsetId,
        string outputRowsetName,
        string? outputRowsetRole,
        int visibleCommonTableExpressionOrdinal,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources,
        RuntimeRowset? inheritedInputRowset,
        IReadOnlyList<string>? expectedOutputColumnNames)
    {
        var children = navigator.TryGetBinaryQueryExpressionChildren(binaryQueryExpression);
        if (children is null)
        {
            issues.Add(new TransformBindingIssue(
                "BinaryQueryExpressionChildrenMissing",
                $"BinaryQueryExpression '{binaryQueryExpression.Id}' is missing one or both child query expressions.",
                binaryQueryExpression.Id));
            return null;
        }

        var firstBinding = BindQueryExpression(
            children.Value.FirstQueryExpressionId,
            $"{binaryQueryExpression.Id}:first-output-rowset",
            $"{outputRowsetName}:first",
            null,
            visibleCommonTableExpressionOrdinal,
            inheritedVisibleTableSources,
            inheritedInputRowset,
            expectedOutputColumnNames);
        var reconciledExpectedOutputColumnNames = expectedOutputColumnNames ??
            firstBinding?.OutputRowset.Columns.Select(item => item.Name).ToArray();
        var secondBinding = BindQueryExpression(
            children.Value.SecondQueryExpressionId,
            $"{binaryQueryExpression.Id}:second-output-rowset",
            $"{outputRowsetName}:second",
            null,
            visibleCommonTableExpressionOrdinal,
            inheritedVisibleTableSources,
            inheritedInputRowset,
            reconciledExpectedOutputColumnNames);

        if (firstBinding is null || secondBinding is null)
        {
            issues.Add(new TransformBindingIssue(
                "SetOperationInputBindingFailed",
                $"BinaryQueryExpression '{binaryQueryExpression.Id}' could not bind one or both input query expressions.",
                binaryQueryExpression.Id));
            return null;
        }

        var outputColumns = ReconcileSetOperationColumns(binaryQueryExpression, firstBinding.OutputRowset, secondBinding.OutputRowset);
        if (outputColumns is null)
        {
            return null;
        }

        var rowset = new RuntimeRowset(
            outputRowsetId,
            outputRowsetName,
            "SetOperation",
            outputRowsetRole,
            binaryQueryExpression.Id,
            null,
            outputColumns,
            [
                new RuntimeRowsetInput(0, "First", firstBinding.OutputRowset),
                new RuntimeRowsetInput(1, "Second", secondBinding.OutputRowset)
            ]);

        TrackRowset(rowset);

        return new RuntimeQueryBindingResult(
            new BindingScope(inheritedVisibleTableSources),
            rowset,
            rowset);
    }

    private RuntimeQueryBindingResult BindQuerySpecification(
        QuerySpecification querySpecification,
        string outputRowsetId,
        string outputRowsetName,
        string? outputRowsetRole,
        int visibleCommonTableExpressionOrdinal,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources,
        RuntimeRowset? inheritedInputRowset,
        IReadOnlyList<string>? expectedOutputColumnNames)
    {
        var tableBindings = new List<RuntimeTableReferenceBinding>();
        var fromClause = navigator.TryGetFromClause(querySpecification);
        foreach (var tableReference in fromClause is null ? [] : navigator.GetTableReferences(fromClause))
        {
            var binding = BindTableReference(
                tableReference,
                visibleCommonTableExpressionOrdinal,
                inheritedVisibleTableSources,
                inheritedInputRowset);
            if (binding is not null)
            {
                tableBindings.Add(binding);
            }
        }

        var localVisibleTableSources = tableBindings
            .SelectMany(item => item.VisibleTableSources)
            .ToArray();
        var scope = new BindingScope(
            localVisibleTableSources
                .Concat(inheritedVisibleTableSources)
                .ToArray(),
            localVisibleTableSources.Length);

        var localInputRowset = fromClause is null ? null : ComposeInputRowset(tableBindings, fromClause);
        var inputRowset = ComposeQueryInputRowset(querySpecification, inheritedInputRowset, localInputRowset);
        var whereSearchCondition = navigator.TryGetWhereSearchCondition(querySpecification);
        if (whereSearchCondition is not null)
        {
            BindBooleanExpression(whereSearchCondition, scope, inputRowset, null);
        }

        var groupingContext = BindGrouping(querySpecification, scope, inputRowset);
        var projectionInputRowset = groupingContext?.GroupedRowset ?? inputRowset;

        var havingSearchCondition = navigator.TryGetHavingSearchCondition(querySpecification);
        if (havingSearchCondition is not null)
        {
            BindBooleanExpression(havingSearchCondition, scope, inputRowset, groupingContext);
        }

        BindQueryWindowClause(querySpecification, scope, projectionInputRowset, groupingContext);

        var outputRowset = BindSelectElements(querySpecification, scope, projectionInputRowset, outputRowsetId, outputRowsetName, outputRowsetRole, expectedOutputColumnNames, groupingContext);
        TrackRowset(outputRowset);
        return new RuntimeQueryBindingResult(scope, inputRowset, outputRowset);
    }

    private RuntimeGroupingContext? BindGrouping(
        QuerySpecification querySpecification,
        BindingScope scope,
        RuntimeRowset? inputRowset)
    {
        var groupByClause = navigator.TryGetGroupByClause(querySpecification);
        if (groupByClause is null)
        {
            return null;
        }

        var isGroupByAll = navigator.IsGroupByAll(groupByClause);
        var groupingKeyColumns = new List<RuntimeColumn>();
        var groupingKeySignatures = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var emittedGroupingKeySignatures = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var groupingSpecification in navigator.GetGroupingSpecifications(groupByClause))
        {
            BindGroupingSpecification(
                groupingSpecification,
                scope,
                inputRowset,
                groupingKeyColumns,
                groupingKeySignatures,
                emittedGroupingKeySignatures);
        }

        var groupedRowset = new RuntimeRowset(
            $"{querySpecification.Id}:grouped-rowset",
            isGroupByAll ? $"GroupedAll:{querySpecification.Id}" : $"Grouped:{querySpecification.Id}",
            "Grouping",
            null,
            querySpecification.Id,
            null,
            groupingKeyColumns,
            inputRowset is null
                ? []
                : [new RuntimeRowsetInput(0, "Input", inputRowset)]);

        TrackRowset(groupedRowset);
        return new RuntimeGroupingContext(groupedRowset, groupingKeySignatures);
    }

    private void BindGroupingSpecification(
        GroupingSpecification groupingSpecification,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        List<RuntimeColumn> groupingKeyColumns,
        HashSet<string> groupingKeySignatures,
        HashSet<string> emittedGroupingKeySignatures)
    {
        var expressionGroupingSpecification = navigator.TryGetExpressionGroupingSpecification(groupingSpecification);
        if (expressionGroupingSpecification is not null)
        {
            BindExpressionGroupingSpecification(
                groupingSpecification,
                expressionGroupingSpecification,
                scope,
                inputRowset,
                groupingKeyColumns,
                groupingKeySignatures,
                emittedGroupingKeySignatures);
            return;
        }

        var groupingSetsGroupingSpecification = navigator.TryGetGroupingSetsGroupingSpecification(groupingSpecification);
        if (groupingSetsGroupingSpecification is not null)
        {
            foreach (var item in navigator.GetGroupingSets(groupingSetsGroupingSpecification))
            {
                BindGroupingSpecification(item, scope, inputRowset, groupingKeyColumns, groupingKeySignatures, emittedGroupingKeySignatures);
            }

            return;
        }

        var rollupGroupingSpecification = navigator.TryGetRollupGroupingSpecification(groupingSpecification);
        if (rollupGroupingSpecification is not null)
        {
            foreach (var item in navigator.GetRollupArguments(rollupGroupingSpecification))
            {
                BindGroupingSpecification(item, scope, inputRowset, groupingKeyColumns, groupingKeySignatures, emittedGroupingKeySignatures);
            }

            return;
        }

        var cubeGroupingSpecification = navigator.TryGetCubeGroupingSpecification(groupingSpecification);
        if (cubeGroupingSpecification is not null)
        {
            foreach (var item in navigator.GetCubeArguments(cubeGroupingSpecification))
            {
                BindGroupingSpecification(item, scope, inputRowset, groupingKeyColumns, groupingKeySignatures, emittedGroupingKeySignatures);
            }

            return;
        }

        var compositeGroupingSpecification = navigator.TryGetCompositeGroupingSpecification(groupingSpecification);
        if (compositeGroupingSpecification is not null)
        {
            foreach (var item in navigator.GetCompositeGroupingItems(compositeGroupingSpecification))
            {
                BindGroupingSpecification(item, scope, inputRowset, groupingKeyColumns, groupingKeySignatures, emittedGroupingKeySignatures);
            }

            return;
        }

        if (navigator.IsGrandTotalGroupingSpecification(groupingSpecification))
        {
            return;
        }

        issues.Add(new TransformBindingIssue(
            "UnsupportedGroupingSpecificationShape",
            $"GroupingSpecification '{groupingSpecification.Id}' is not yet supported by binding.",
            groupingSpecification.Id));
    }

    private void BindExpressionGroupingSpecification(
        GroupingSpecification groupingSpecification,
        ExpressionGroupingSpecification expressionGroupingSpecification,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        List<RuntimeColumn> groupingKeyColumns,
        HashSet<string> groupingKeySignatures,
        HashSet<string> emittedGroupingKeySignatures)
    {
        var expression = navigator.TryGetExpressionGroupingSpecificationExpression(expressionGroupingSpecification);
        if (expression is null)
        {
            issues.Add(new TransformBindingIssue(
                "GroupingSpecificationExpressionMissing",
                $"ExpressionGroupingSpecification '{expressionGroupingSpecification.Id}' is missing its scalar expression.",
                expressionGroupingSpecification.Id));
            return;
        }

        var directColumnReference = navigator.TryGetDirectColumnReference(expression);
        if (directColumnReference is not null)
        {
            var boundColumnReference = BindColumnReference(directColumnReference, scope, null, false);
            if (boundColumnReference is null)
            {
                return;
            }

            boundColumnReferences.Add(boundColumnReference);

            var signature = NormalizeColumnReferenceSignature(boundColumnReference.IdentifierParts);
            groupingKeySignatures.Add(signature);
            if (!emittedGroupingKeySignatures.Add(signature))
            {
                return;
            }

            groupingKeyColumns.Add(new RuntimeColumn(
                $"{groupingSpecification.Id}:group-column:{groupingKeyColumns.Count + 1}",
                boundColumnReference.ResolvedColumn.Name,
                groupingKeyColumns.Count));
            return;
        }

        var capturedReferenceStart = boundColumnReferences.Count;
        BindScalarExpression(expression, scope, inputRowset, null, false);
        var capturedReferences = boundColumnReferences
            .Skip(capturedReferenceStart)
            .ToArray();

        if (capturedReferences.Length == 0)
        {
            var expressionSignature = $"EXPR:{expression.Id}";
            groupingKeySignatures.Add(expressionSignature);
            if (!emittedGroupingKeySignatures.Add(expressionSignature))
            {
                return;
            }

            groupingKeyColumns.Add(new RuntimeColumn(
                $"{groupingSpecification.Id}:group-column:{groupingKeyColumns.Count + 1}",
                $"GroupExpr{groupingKeyColumns.Count + 1}",
                groupingKeyColumns.Count));
            return;
        }

        foreach (var capturedReference in capturedReferences)
        {
            var signature = NormalizeColumnReferenceSignature(capturedReference.IdentifierParts);
            groupingKeySignatures.Add(signature);
        }

        var outputColumnName = capturedReferences[0].ResolvedColumn.Name;
        var outputSignature = NormalizeColumnReferenceSignature(capturedReferences[0].IdentifierParts);
        if (!emittedGroupingKeySignatures.Add(outputSignature))
        {
            return;
        }

        groupingKeyColumns.Add(new RuntimeColumn(
            $"{groupingSpecification.Id}:group-column:{groupingKeyColumns.Count + 1}",
            outputColumnName,
            groupingKeyColumns.Count));
    }

    private RuntimeRowset? ComposeQueryInputRowset(
        QuerySpecification querySpecification,
        RuntimeRowset? inheritedInputRowset,
        RuntimeRowset? localInputRowset)
    {
        if (inheritedInputRowset is null)
        {
            return localInputRowset;
        }

        if (localInputRowset is null)
        {
            return inheritedInputRowset;
        }

        var columns = inheritedInputRowset.Columns
            .Concat(localInputRowset.Columns)
            .Select((column, ordinal) => new RuntimeColumn(
                $"{querySpecification.Id}:input-column:{ordinal + 1}",
                column.Name,
                ordinal))
            .ToArray();

        var rowset = new RuntimeRowset(
            $"{querySpecification.Id}:input-rowset",
            $"Input:{querySpecification.Id}",
            "Input",
            null,
            querySpecification.Id,
            null,
            columns,
            [
                new RuntimeRowsetInput(0, "OuterScope", inheritedInputRowset),
                new RuntimeRowsetInput(1, "LocalScope", localInputRowset)
            ]);

        TrackRowset(rowset);
        return rowset;
    }

    private RuntimeColumn[]? ReconcileSetOperationColumns(
        BinaryQueryExpression binaryQueryExpression,
        RuntimeRowset firstRowset,
        RuntimeRowset secondRowset)
    {
        if (firstRowset.Columns.Count != secondRowset.Columns.Count)
        {
            issues.Add(new TransformBindingIssue(
                "SetOperationColumnCountMismatch",
                $"BinaryQueryExpression '{binaryQueryExpression.Id}' produces mismatched column counts ({firstRowset.Columns.Count} vs {secondRowset.Columns.Count}).",
                binaryQueryExpression.Id));
            return null;
        }

        return firstRowset.Columns
            .Zip(secondRowset.Columns, (firstColumn, secondColumn) => (First: firstColumn, Second: secondColumn))
            .Select((pair, ordinal) => new RuntimeColumn(
                $"{binaryQueryExpression.Id}:column:{ordinal + 1}",
                pair.First.Name,
                ordinal))
            .ToArray();
    }

    private IReadOnlyList<string>? TryDeriveOutputColumnNamesFromQueryExpression(string queryExpressionId)
    {
        var querySpecification = navigator.TryGetQuerySpecification(queryExpressionId);
        if (querySpecification is not null)
        {
            return TryDeriveOutputColumnNamesFromQuerySpecification(querySpecification);
        }

        var binaryQueryExpression = navigator.TryGetBinaryQueryExpression(queryExpressionId);
        if (binaryQueryExpression is null)
        {
            return null;
        }

        var children = navigator.TryGetBinaryQueryExpressionChildren(binaryQueryExpression);
        return children is null
            ? null
            : TryDeriveOutputColumnNamesFromQueryExpression(children.Value.FirstQueryExpressionId);
    }

    private IReadOnlyList<string>? TryDeriveOutputColumnNamesFromQuerySpecification(QuerySpecification querySpecification)
    {
        var names = new List<string>();

        foreach (var selectElement in navigator.GetSelectElements(querySpecification))
        {
            var selectScalarExpression = navigator.TryGetSelectScalarExpression(selectElement);
            if (selectScalarExpression is null)
            {
                return null;
            }

            var name = navigator.TryGetSelectScalarExpressionAlias(selectScalarExpression);
            if (!string.IsNullOrWhiteSpace(name))
            {
                names.Add(name);
                continue;
            }

            var scalarExpression = navigator.TryGetSelectScalarExpressionBody(selectScalarExpression);
            if (scalarExpression is null)
            {
                return null;
            }

            var directColumnReference = navigator.TryGetDirectColumnReference(scalarExpression);
            if (directColumnReference is null)
            {
                return null;
            }

            var parts = navigator.GetColumnReferenceParts(directColumnReference);
            var directName = parts.LastOrDefault();
            if (string.IsNullOrWhiteSpace(directName))
            {
                return null;
            }

            names.Add(directName);
        }

        return names;
    }
}
