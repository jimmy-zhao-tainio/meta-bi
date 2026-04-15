using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private RuntimeRowset? ComposeInputRowset(
        IReadOnlyList<RuntimeTableReferenceBinding> tableBindings,
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
            .Select((column, ordinal) => new RuntimeColumn(
                $"{fromClause.Id}:column:{ordinal + 1}",
                column.Name,
                ordinal))
            .ToArray();

        var fromRowset = new RuntimeRowset(
            $"{fromClause.Id}:rowset",
            $"From:{fromClause.Id}",
            "From",
            null,
            fromClause.Id,
            null,
            composedColumns,
            tableBindings
                .Select((item, ordinal) => new RuntimeRowsetInput(ordinal, "Input", item.Rowset))
                .ToArray());

        TrackRowset(fromRowset);
        return fromRowset;
    }

    private RuntimeRowset BindSelectElements(
        QuerySpecification querySpecification,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        string outputRowsetId,
        string outputRowsetName,
        string? outputRowsetRole,
        IReadOnlyList<string>? expectedOutputColumnNames,
        RuntimeGroupingContext? groupingContext)
    {
        var outputColumns = new List<RuntimeColumn>();

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

        return new RuntimeRowset(
            outputRowsetId,
            outputRowsetName,
            "Projection",
            outputRowsetRole,
            querySpecification.Id,
            null,
            outputColumns,
            inputRowset is null
                ? []
                : [new RuntimeRowsetInput(0, "Input", inputRowset)]);
    }

    private void BindSelectScalarExpression(
        SelectElement selectElement,
        SelectScalarExpression selectScalarExpression,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        List<RuntimeColumn> outputColumns,
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

        RuntimeColumnReference? boundColumnReference = null;
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

        outputColumns.Add(new RuntimeColumn(
            $"{selectElement.Id}:output",
            outputName,
            outputColumns.Count));
    }

    private void BindSelectStarExpression(
        SelectElement selectElement,
        SelectStarExpression selectStarExpression,
        BindingScope scope,
        List<RuntimeColumn> outputColumns,
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
        IEnumerable<RuntimeTableSource> sourcesToExpand;

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

        var unresolvedSourceExpansions = sourcesToExpand
            .Where(static item => string.Equals(item.Rowset.DerivationKind, "Source", StringComparison.Ordinal) &&
                                  item.Rowset.Columns.Count == 0)
            .ToArray();
        if (unresolvedSourceExpansions.Length > 0)
        {
            issues.Add(new TransformBindingIssue(
                "SelectStarRequiresValidationSchema",
                $"Select star on '{selectElement.Id}' depends on source rowset shape that Binding does not derive from syntax alone.",
                selectElement.Id));
            return;
        }

        foreach (var source in sourcesToExpand)
        {
            foreach (var column in source.Rowset.Columns)
            {
                outputColumns.Add(new RuntimeColumn(
                    $"{selectElement.Id}:output:{outputColumns.Count}",
                    column.Name,
                    outputColumns.Count));
            }
        }
    }

    private RuntimeColumnReference? BindColumnReference(
        ColumnReferenceExpression columnReferenceExpression,
        BindingScope scope,
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

        if (TryBindFunctionParameterReference(columnReferenceExpression, parts))
        {
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
                var inferableSources = scope.VisibleTableSources
                    .Where(CanInferSourceColumn)
                    .ToArray();

                if (inferableSources.Length == 1)
                {
                    var inferredColumn = EnsureInferredSourceColumn(inferableSources[0], parts[0]);
                    return ValidateGroupedColumnReference(
                        columnReferenceExpression,
                        parts,
                        inferredColumn,
                        inferableSources[0],
                        groupingContext,
                        withinAggregate);
                }

                if (inferableSources.Length > 1)
                {
                    issues.Add(new TransformBindingIssue(
                        "ColumnReferenceRequiresValidationSchema",
                        $"Column '{parts[0]}' could belong to more than one visible source rowset; Binding cannot resolve it from syntax alone.",
                        columnReferenceExpression.Id));
                    return null;
                }

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
                if (CanInferSourceColumn(matchedSources[0]))
                {
                    var inferredColumn = EnsureInferredSourceColumn(matchedSources[0], parts[1]);
                    return ValidateGroupedColumnReference(
                        columnReferenceExpression,
                        parts,
                        inferredColumn,
                        matchedSources[0],
                        groupingContext,
                        withinAggregate);
                }

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

    private bool TryBindFunctionParameterReference(
        ColumnReferenceExpression columnReferenceExpression,
        IReadOnlyList<string> identifierParts)
    {
        if (identifierParts.Count != 1)
        {
            return false;
        }

        var name = identifierParts[0].Trim();
        if (!name.StartsWith("@", StringComparison.Ordinal))
        {
            return false;
        }

        if (isInlineTableValuedFunction)
        {
            if (!activeTransformFunctionParameterNames.Contains(name))
            {
                issues.Add(new TransformBindingIssue(
                    "FunctionParameterReferenceNotFound",
                    $"Function parameter '{name}' is referenced but not declared on the active inline TVF transform script.",
                    columnReferenceExpression.Id));
            }

            return true;
        }

        issues.Add(new TransformBindingIssue(
            "ScalarVariableReferenceNotSupported",
            $"Scalar variable reference '{name}' is not currently supported outside inline TVF parameter binding.",
            columnReferenceExpression.Id));
        return true;
    }

    private RuntimeColumnReference? ValidateGroupedColumnReference(
        ColumnReferenceExpression columnReferenceExpression,
        IReadOnlyList<string> parts,
        RuntimeColumn column,
        RuntimeTableSource tableSource,
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

        return new RuntimeColumnReference(columnReferenceExpression.Id, parts, column, tableSource);
    }

    private static string NormalizeColumnReferenceSignature(IReadOnlyList<string> parts) =>
        string.Join(".", parts).Trim().ToUpperInvariant();

    private static bool CanInferSourceColumn(RuntimeTableSource tableSource)
    {
        return string.Equals(tableSource.Rowset.DerivationKind, "Source", StringComparison.Ordinal);
    }

    private static RuntimeColumn EnsureInferredSourceColumn(
        RuntimeTableSource tableSource,
        string columnName)
    {
        var existingColumn = tableSource.Rowset.Columns
            .FirstOrDefault(item => string.Equals(item.Name, columnName, StringComparison.OrdinalIgnoreCase));
        if (existingColumn is not null)
        {
            return existingColumn;
        }

        if (tableSource.Rowset.Columns is not List<RuntimeColumn> mutableColumns)
        {
            throw new InvalidOperationException(
                $"Source rowset '{tableSource.Rowset.Id}' does not expose mutable columns for inferred binding.");
        }

        var inferredColumn = new RuntimeColumn(
            $"{tableSource.SyntaxTableReferenceId}:source-column:{mutableColumns.Count + 1}",
            columnName,
            mutableColumns.Count);
        mutableColumns.Add(inferredColumn);
        return inferredColumn;
    }

    private void TrackRowset(RuntimeRowset rowset)
    {
        if (boundRowsets.Any(item => string.Equals(item.Id, rowset.Id, StringComparison.Ordinal)))
        {
            return;
        }

        boundRowsets.Add(rowset);
    }

    private void TrackTableSource(RuntimeTableSource tableSource)
    {
        if (boundTableSources.Any(item => string.Equals(item.SyntaxTableReferenceId, tableSource.SyntaxTableReferenceId, StringComparison.Ordinal)))
        {
            return;
        }

        boundTableSources.Add(tableSource);
    }
}
