using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private RuntimeBoundRowset? ComposeInputRowset(
        IReadOnlyList<RuntimeBoundTableReferenceBinding> tableBindings,
        FromClause fromClause)
    {
        if (tableBindings.Count == 0)
        {
            return null;
        }

        if (tableBindings.Count == 1)
        {
            return tableBindings[0].Rowset;
        }

        var composedColumns = tableBindings
            .SelectMany(item => item.Rowset.Columns)
            .Select((column, ordinal) => new RuntimeBoundColumn(
                $"{fromClause.Id}:column:{ordinal + 1}",
                column.Name,
                ordinal,
                column.SourceFieldId,
                column.SourceTableId))
            .ToArray();

        var fromRowset = new RuntimeBoundRowset(
            $"{fromClause.Id}:rowset",
            $"From:{fromClause.Id}",
            "From",
            null,
            fromClause.Id,
            null,
            composedColumns,
            tableBindings
                .Select((item, ordinal) => new RuntimeBoundRowsetInput(ordinal, "Input", item.Rowset))
                .ToArray());

        TrackRowset(fromRowset);
        return fromRowset;
    }

    private RuntimeBoundRowset BindSelectElements(
        QuerySpecification querySpecification,
        BoundScope scope,
        RuntimeBoundRowset? inputRowset,
        string outputRowsetId,
        string outputRowsetName,
        string? outputRowsetRole,
        IReadOnlyList<string>? expectedOutputColumnNames,
        RuntimeGroupingContext? groupingContext)
    {
        var outputColumns = new List<RuntimeBoundColumn>();

        foreach (var item in navigator.GetSelectElements(querySpecification).Select((selectElement, ordinal) => (SelectElement: selectElement, Ordinal: ordinal)))
        {
            var selectScalarExpression = navigator.TryGetSelectScalarExpression(item.SelectElement);
            if (selectScalarExpression is not null)
            {
                BindSelectScalarExpression(
                    item.SelectElement,
                    selectScalarExpression,
                    scope,
                    inputRowset,
                    outputColumns,
                    groupingContext,
                    expectedOutputColumnNames is not null && item.Ordinal < expectedOutputColumnNames.Count
                        ? expectedOutputColumnNames[item.Ordinal]
                        : null);
                continue;
            }

            var selectStarExpression = navigator.TryGetSelectStarExpression(item.SelectElement);
            if (selectStarExpression is not null)
            {
                BindSelectStarExpression(item.SelectElement, selectStarExpression, scope, outputColumns, groupingContext);
                continue;
            }

            issues.Add(new TransformBindingIssue(
                "UnsupportedSelectElementShape",
                $"SelectElement '{item.SelectElement.Id}' is not yet supported by binding.",
                item.SelectElement.Id));
        }

        return new RuntimeBoundRowset(
            outputRowsetId,
            outputRowsetName,
            "Projection",
            outputRowsetRole,
            querySpecification.Id,
            null,
            outputColumns,
            inputRowset is null
                ? []
                : [new RuntimeBoundRowsetInput(0, "Input", inputRowset)]);
    }

    private void BindSelectScalarExpression(
        SelectElement selectElement,
        SelectScalarExpression selectScalarExpression,
        BoundScope scope,
        RuntimeBoundRowset? inputRowset,
        List<RuntimeBoundColumn> outputColumns,
        RuntimeGroupingContext? groupingContext,
        string? expectedOutputColumnName)
    {
        var scalarExpression = navigator.TryGetSelectScalarExpressionBody(selectScalarExpression);
        if (scalarExpression is null)
        {
            issues.Add(new TransformBindingIssue(
                "SelectScalarExpressionBodyMissing",
                $"SelectScalarExpression '{selectScalarExpression.Id}' is missing its expression body.",
                selectScalarExpression.Id));
            return;
        }

        RuntimeBoundColumnReference? boundColumnReference = null;
        var directColumnReference = navigator.TryGetDirectColumnReference(scalarExpression);
        if (directColumnReference is not null)
        {
            boundColumnReference = BindColumnReference(directColumnReference, scope, groupingContext, withinAggregate: false);
            if (boundColumnReference is not null)
            {
                boundColumnReferences.Add(boundColumnReference);
            }
        }
        else
        {
            BindScalarExpression(scalarExpression, scope, inputRowset, groupingContext, withinAggregate: false);
        }

        var outputName = navigator.TryGetSelectScalarExpressionAlias(selectScalarExpression);
        if (string.IsNullOrWhiteSpace(outputName) && boundColumnReference is not null)
        {
            outputName = boundColumnReference.ResolvedColumn.Name;
        }

        if (string.IsNullOrWhiteSpace(outputName) && directColumnReference is not null)
        {
            outputName = navigator.GetColumnReferenceParts(directColumnReference).LastOrDefault();
        }

        if (string.IsNullOrWhiteSpace(outputName) && !string.IsNullOrWhiteSpace(expectedOutputColumnName))
        {
            outputName = expectedOutputColumnName;
        }

        if (string.IsNullOrWhiteSpace(outputName))
        {
            issues.Add(new TransformBindingIssue(
                "UnsupportedSelectOutputName",
                $"SelectElement '{selectElement.Id}' does not currently expose a supported output name in binding.",
                selectElement.Id));
            return;
        }

        outputColumns.Add(new RuntimeBoundColumn(
            $"{selectElement.Id}:output",
            outputName,
            outputColumns.Count,
            boundColumnReference?.ResolvedColumn.SourceFieldId,
            boundColumnReference?.ResolvedColumn.SourceTableId));
    }

    private void BindSelectStarExpression(
        SelectElement selectElement,
        SelectStarExpression selectStarExpression,
        BoundScope scope,
        List<RuntimeBoundColumn> outputColumns,
        RuntimeGroupingContext? groupingContext)
    {
        if (groupingContext is not null)
        {
            issues.Add(new TransformBindingIssue(
                "GroupedSelectStarNotSupported",
                $"SelectElement '{selectElement.Id}' uses '*' within a grouped query, which is not yet supported by binding.",
                selectElement.Id));
            return;
        }

        var qualifierParts = navigator.GetSelectStarQualifierParts(selectStarExpression);
        IEnumerable<RuntimeBoundTableSource> sourcesToExpand;

        if (qualifierParts.Count == 0)
        {
            sourcesToExpand = scope.VisibleTableSources;
        }
        else if (qualifierParts.Count == 1)
        {
            var matchedSources = scope.VisibleTableSources
                .Where(item => string.Equals(item.ExposedName, qualifierParts[0], StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (matchedSources.Length == 0)
            {
                issues.Add(new TransformBindingIssue(
                    "SelectStarQualifierNotFound",
                    $"Select star qualifier '{qualifierParts[0]}' does not match any visible table source.",
                    selectStarExpression.Id));
                return;
            }

            if (matchedSources.Length > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "SelectStarQualifierAmbiguous",
                    $"Select star qualifier '{qualifierParts[0]}' matches more than one visible table source.",
                    selectStarExpression.Id));
                return;
            }

            sourcesToExpand = matchedSources;
        }
        else
        {
            issues.Add(new TransformBindingIssue(
                "UnsupportedSelectStarQualifierShape",
                $"Select star qualifier on '{selectStarExpression.Id}' uses {qualifierParts.Count} identifier parts; binding supports single-part qualifiers only.",
                selectStarExpression.Id));
            return;
        }

        foreach (var source in sourcesToExpand)
        {
            foreach (var column in source.Rowset.Columns)
            {
                outputColumns.Add(new RuntimeBoundColumn(
                    $"{selectElement.Id}:output:{outputColumns.Count}",
                    column.Name,
                    outputColumns.Count,
                    column.SourceFieldId,
                    column.SourceTableId));
            }
        }
    }

    private RuntimeBoundColumnReference? BindColumnReference(
        ColumnReferenceExpression columnReferenceExpression,
        BoundScope scope,
        RuntimeGroupingContext? groupingContext = null,
        bool withinAggregate = false)
    {
        var parts = navigator.GetColumnReferenceParts(columnReferenceExpression);
        if (parts.Count == 0)
        {
            if (withinAggregate)
            {
                return null;
            }

            issues.Add(new TransformBindingIssue(
                "ColumnReferenceMissingIdentifier",
                $"ColumnReferenceExpression '{columnReferenceExpression.Id}' is missing its multipart identifier.",
                columnReferenceExpression.Id));
            return null;
        }

        if (parts.Count == 1)
        {
            var matches = scope.VisibleTableSources
                .SelectMany(source => source.Rowset.Columns.Select(column => (Source: source, Column: column)))
                .Where(item => string.Equals(item.Column.Name, parts[0], StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (matches.Length == 0)
            {
                issues.Add(new TransformBindingIssue(
                    "ColumnReferenceNotFound",
                    $"Column '{parts[0]}' is not visible in the current query scope.",
                    columnReferenceExpression.Id));
                return null;
            }

            if (matches.Length > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "ColumnReferenceAmbiguous",
                    $"Column '{parts[0]}' resolves ambiguously across visible table sources.",
                    columnReferenceExpression.Id));
                return null;
            }

            return ValidateGroupedColumnReference(
                columnReferenceExpression,
                parts,
                matches[0].Column,
                matches[0].Source,
                groupingContext,
                withinAggregate);
        }

        if (parts.Count == 2)
        {
            var matchedSources = scope.VisibleTableSources
                .Where(item => string.Equals(item.ExposedName, parts[0], StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (matchedSources.Length == 0)
            {
                issues.Add(new TransformBindingIssue(
                    "ColumnQualifierNotFound",
                    $"Column qualifier '{parts[0]}' is not visible in the current query scope.",
                    columnReferenceExpression.Id));
                return null;
            }

            if (matchedSources.Length > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "ColumnQualifierAmbiguous",
                    $"Column qualifier '{parts[0]}' matches more than one visible table source.",
                    columnReferenceExpression.Id));
                return null;
            }

            var matchedColumns = matchedSources[0].Rowset.Columns
                .Where(item => string.Equals(item.Name, parts[1], StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (matchedColumns.Length == 0)
            {
                issues.Add(new TransformBindingIssue(
                    "QualifiedColumnReferenceNotFound",
                    $"Column '{parts[1]}' is not exposed by table source '{parts[0]}'.",
                    columnReferenceExpression.Id));
                return null;
            }

            if (matchedColumns.Length > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "QualifiedColumnReferenceAmbiguous",
                    $"Column '{parts[1]}' resolves ambiguously within table source '{parts[0]}'.",
                    columnReferenceExpression.Id));
                return null;
            }

            return ValidateGroupedColumnReference(
                columnReferenceExpression,
                parts,
                matchedColumns[0],
                matchedSources[0],
                groupingContext,
                withinAggregate);
        }

        issues.Add(new TransformBindingIssue(
            "UnsupportedColumnReferenceShape",
            $"ColumnReferenceExpression '{columnReferenceExpression.Id}' uses {parts.Count} identifier parts; binding supports one-part or two-part references only.",
            columnReferenceExpression.Id));
        return null;
    }

    private RuntimeBoundColumnReference? ValidateGroupedColumnReference(
        ColumnReferenceExpression columnReferenceExpression,
        IReadOnlyList<string> parts,
        RuntimeBoundColumn column,
        RuntimeBoundTableSource tableSource,
        RuntimeGroupingContext? groupingContext,
        bool withinAggregate)
    {
        if (groupingContext is not null && !withinAggregate)
        {
            var signature = NormalizeColumnReferenceSignature(parts);
            if (!groupingContext.GroupingKeySignatures.Contains(signature))
            {
                issues.Add(new TransformBindingIssue(
                    "UngroupedColumnReference",
                    $"Column reference '{string.Join(".", parts)}' is not part of the grouped key set and is used outside an aggregate context.",
                    columnReferenceExpression.Id));
                return null;
            }
        }

        return new RuntimeBoundColumnReference(columnReferenceExpression.Id, parts, column, tableSource);
    }

    private static string NormalizeColumnReferenceSignature(IReadOnlyList<string> parts) =>
        string.Join(".", parts).Trim().ToUpperInvariant();

    private void TrackRowset(RuntimeBoundRowset rowset)
    {
        if (boundRowsets.Any(item => string.Equals(item.Id, rowset.Id, StringComparison.Ordinal)))
        {
            return;
        }

        boundRowsets.Add(rowset);
    }

    private void TrackTableSource(RuntimeBoundTableSource tableSource)
    {
        if (boundTableSources.Any(item => string.Equals(item.SyntaxTableReferenceId, tableSource.SyntaxTableReferenceId, StringComparison.Ordinal)))
        {
            return;
        }

        boundTableSources.Add(tableSource);
    }
}
