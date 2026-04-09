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
        IReadOnlyList<RuntimeBoundTableSource> inheritedVisibleTableSources,
        RuntimeBoundRowset? inheritedInputRowset,
        IReadOnlyList<string>? expectedOutputColumnNames)
    {
        var querySpecification = navigator.TryGetQuerySpecification(queryExpressionId);
        if (querySpecification is not null)
        {
            return BindQuerySpecification(
                querySpecification,
                outputRowsetId,
                outputRowsetName,
                outputRowsetRole,
                visibleCommonTableExpressionOrdinal,
                inheritedVisibleTableSources,
                inheritedInputRowset,
                expectedOutputColumnNames);
        }

        var binaryQueryExpression = navigator.TryGetBinaryQueryExpression(queryExpressionId);
        if (binaryQueryExpression is not null)
        {
            return BindBinaryQueryExpression(
                binaryQueryExpression,
                outputRowsetId,
                outputRowsetName,
                outputRowsetRole,
                visibleCommonTableExpressionOrdinal,
                inheritedVisibleTableSources,
                inheritedInputRowset,
                expectedOutputColumnNames);
        }

        issues.Add(new TransformBindingIssue(
            "UnsupportedQueryExpressionShape",
            $"QueryExpression '{queryExpressionId}' is not yet supported by binding.",
            queryExpressionId));
        return null;
    }

    private RuntimeQueryBindingResult? BindBinaryQueryExpression(
        BinaryQueryExpression binaryQueryExpression,
        string outputRowsetId,
        string outputRowsetName,
        string? outputRowsetRole,
        int visibleCommonTableExpressionOrdinal,
        IReadOnlyList<RuntimeBoundTableSource> inheritedVisibleTableSources,
        RuntimeBoundRowset? inheritedInputRowset,
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

        var rowset = new RuntimeBoundRowset(
            outputRowsetId,
            outputRowsetName,
            "SetOperation",
            outputRowsetRole,
            binaryQueryExpression.Id,
            null,
            outputColumns,
            [
                new RuntimeBoundRowsetInput(0, "First", firstBinding.OutputRowset),
                new RuntimeBoundRowsetInput(1, "Second", secondBinding.OutputRowset)
            ]);

        TrackRowset(rowset);

        return new RuntimeQueryBindingResult(
            new BoundScope(inheritedVisibleTableSources),
            rowset,
            rowset);
    }

    private RuntimeQueryBindingResult BindQuerySpecification(
        QuerySpecification querySpecification,
        string outputRowsetId,
        string outputRowsetName,
        string? outputRowsetRole,
        int visibleCommonTableExpressionOrdinal,
        IReadOnlyList<RuntimeBoundTableSource> inheritedVisibleTableSources,
        RuntimeBoundRowset? inheritedInputRowset,
        IReadOnlyList<string>? expectedOutputColumnNames)
    {
        var tableBindings = new List<RuntimeBoundTableReferenceBinding>();
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

        var scope = new BoundScope(
            inheritedVisibleTableSources
                .Concat(tableBindings.SelectMany(item => item.VisibleTableSources))
                .ToArray());

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

        var outputRowset = BindSelectElements(querySpecification, scope, projectionInputRowset, outputRowsetId, outputRowsetName, outputRowsetRole, expectedOutputColumnNames, groupingContext);
        TrackRowset(outputRowset);
        return new RuntimeQueryBindingResult(scope, inputRowset, outputRowset);
    }

    private RuntimeGroupingContext? BindGrouping(
        QuerySpecification querySpecification,
        BoundScope scope,
        RuntimeBoundRowset? inputRowset)
    {
        var groupByClause = navigator.TryGetGroupByClause(querySpecification);
        if (groupByClause is null)
        {
            return null;
        }

        var groupingKeyColumns = new List<RuntimeBoundColumn>();
        var groupingKeySignatures = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var groupingSpecification in navigator.GetGroupingSpecifications(groupByClause))
        {
            var expressionGroupingSpecification = navigator.TryGetExpressionGroupingSpecification(groupingSpecification);
            if (expressionGroupingSpecification is null)
            {
                issues.Add(new TransformBindingIssue(
                    "UnsupportedGroupingSpecificationShape",
                    $"GroupingSpecification '{groupingSpecification.Id}' is not yet supported by binding.",
                    groupingSpecification.Id));
                continue;
            }

            var expression = navigator.TryGetExpressionGroupingSpecificationExpression(expressionGroupingSpecification);
            if (expression is null)
            {
                issues.Add(new TransformBindingIssue(
                    "GroupingSpecificationExpressionMissing",
                    $"ExpressionGroupingSpecification '{expressionGroupingSpecification.Id}' is missing its scalar expression.",
                    expressionGroupingSpecification.Id));
                continue;
            }

            var directColumnReference = navigator.TryGetDirectColumnReference(expression);
            if (directColumnReference is null)
            {
                BindScalarExpression(expression, scope, inputRowset, null, false);
                issues.Add(new TransformBindingIssue(
                    "UnsupportedGroupingExpressionShape",
                    $"GroupingSpecification '{groupingSpecification.Id}' is not yet supported unless it is a direct column reference.",
                    groupingSpecification.Id));
                continue;
            }

            var boundColumnReference = BindColumnReference(directColumnReference, scope, null, false);
            if (boundColumnReference is null)
            {
                continue;
            }

            boundColumnReferences.Add(boundColumnReference);

            var signature = NormalizeColumnReferenceSignature(boundColumnReference.IdentifierParts);
            groupingKeySignatures.Add(signature);

            groupingKeyColumns.Add(new RuntimeBoundColumn(
                $"{groupingSpecification.Id}:group-column:{groupingKeyColumns.Count + 1}",
                boundColumnReference.ResolvedColumn.Name,
                groupingKeyColumns.Count,
                boundColumnReference.ResolvedColumn.SourceFieldId,
                boundColumnReference.ResolvedColumn.SourceTableId));
        }

        var groupedRowset = new RuntimeBoundRowset(
            $"{querySpecification.Id}:grouped-rowset",
            $"Grouped:{querySpecification.Id}",
            "Grouping",
            null,
            querySpecification.Id,
            null,
            groupingKeyColumns,
            inputRowset is null
                ? []
                : [new RuntimeBoundRowsetInput(0, "Input", inputRowset)]);

        TrackRowset(groupedRowset);
        return new RuntimeGroupingContext(groupedRowset, groupingKeySignatures);
    }

    private RuntimeBoundRowset? ComposeQueryInputRowset(
        QuerySpecification querySpecification,
        RuntimeBoundRowset? inheritedInputRowset,
        RuntimeBoundRowset? localInputRowset)
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
            .Select((column, ordinal) => new RuntimeBoundColumn(
                $"{querySpecification.Id}:input-column:{ordinal + 1}",
                column.Name,
                ordinal,
                column.SourceFieldId,
                column.SourceTableId))
            .ToArray();

        var rowset = new RuntimeBoundRowset(
            $"{querySpecification.Id}:input-rowset",
            $"Input:{querySpecification.Id}",
            "Input",
            null,
            querySpecification.Id,
            null,
            columns,
            [
                new RuntimeBoundRowsetInput(0, "OuterScope", inheritedInputRowset),
                new RuntimeBoundRowsetInput(1, "LocalScope", localInputRowset)
            ]);

        TrackRowset(rowset);
        return rowset;
    }

    private RuntimeBoundColumn[]? ReconcileSetOperationColumns(
        BinaryQueryExpression binaryQueryExpression,
        RuntimeBoundRowset firstRowset,
        RuntimeBoundRowset secondRowset)
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
            .Select((pair, ordinal) =>
            {
                if (!string.Equals(pair.First.Name, pair.Second.Name, StringComparison.OrdinalIgnoreCase))
                {
                    issues.Add(new TransformBindingIssue(
                        "SetOperationColumnNameMismatch",
                        $"BinaryQueryExpression '{binaryQueryExpression.Id}' column {ordinal + 1} is named '{pair.First.Name}' on the first input and '{pair.Second.Name}' on the second input.",
                        binaryQueryExpression.Id));
                }

                return new RuntimeBoundColumn(
                    $"{binaryQueryExpression.Id}:column:{ordinal + 1}",
                    pair.First.Name,
                    ordinal,
                    pair.First.SourceFieldId,
                    pair.First.SourceTableId);
            })
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
