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
            outputName = $"Expr{outputColumns.Count + 1}";
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
        var localVisibleTableSources = GetLocalVisibleTableSources(scope);
        var inheritedVisibleTableSources = GetInheritedVisibleTableSources(scope);
        IEnumerable<RuntimeTableSource> sourcesToExpand;

        if (qualifierParts.Count == 0)
        {
            sourcesToExpand = localVisibleTableSources.Length > 0
                ? localVisibleTableSources
                : scope.VisibleTableSources;
        }
        else if (qualifierParts.Count == 1)
        {
            var matchedLocalSources = localVisibleTableSources
                .Where(item => string.Equals(item.ExposedName, qualifierParts[0], StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (matchedLocalSources.Length > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "SelectStarQualifierAmbiguous",
                    $"Select star qualifier '{qualifierParts[0]}' matches more than one visible table source.",
                    selectStarExpression.Id));
                return;
            }

            if (matchedLocalSources.Length == 1)
            {
                sourcesToExpand = matchedLocalSources;
            }
            else
            {
                var matchedInheritedSources = inheritedVisibleTableSources
                    .Where(item => string.Equals(item.ExposedName, qualifierParts[0], StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (matchedInheritedSources.Length == 0)
                {
                    issues.Add(new TransformBindingIssue(
                        "SelectStarQualifierNotFound",
                        $"Select star qualifier '{qualifierParts[0]}' does not match any visible table source.",
                        selectStarExpression.Id));
                    return;
                }

                if (matchedInheritedSources.Length > 1)
                {
                    issues.Add(new TransformBindingIssue(
                        "SelectStarQualifierAmbiguous",
                        $"Select star qualifier '{qualifierParts[0]}' matches more than one visible table source.",
                        selectStarExpression.Id));
                    return;
                }

                sourcesToExpand = matchedInheritedSources;
            }
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
            .Where(static item => CanInferSourceColumn(item) &&
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
        return BindColumnReferenceFromIdentifierParts(
            parts,
            columnReferenceExpression.Id,
            scope,
            groupingContext,
            withinAggregate);
    }

    private RuntimeColumnReference? BindColumnReferenceFromIdentifierParts(
        IReadOnlyList<string> parts,
        string syntaxEntityId,
        BindingScope scope,
        RuntimeGroupingContext? groupingContext,
        bool withinAggregate)
    {
        if (parts.Count == 0)
        {
            if (withinAggregate)
            {
                return null;
            }

            issues.Add(new TransformBindingIssue(
                "ColumnReferenceMissingIdentifier",
                $"Column reference '{syntaxEntityId}' is missing its multipart identifier.",
                syntaxEntityId));
            return null;
        }

        if (TryBindFunctionParameterReference(syntaxEntityId, parts))
        {
            return null;
        }

        if (parts.Count == 1 && IsOrderByOutputAliasReference(parts[0]))
        {
            return null;
        }

        var localVisibleTableSources = GetLocalVisibleTableSources(scope);
        var inheritedVisibleTableSources = GetInheritedVisibleTableSources(scope);

        if (parts.Count == 1)
        {
            var localMatches = localVisibleTableSources
                .SelectMany(source => source.Rowset.Columns.Select(column => (Source: source, Column: column)))
                .Where(item => string.Equals(item.Column.Name, parts[0], StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (localMatches.Length > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "ColumnReferenceAmbiguous",
                    $"Column '{parts[0]}' resolves ambiguously across visible table sources.",
                    syntaxEntityId));
                return null;
            }

            if (localMatches.Length == 1)
            {
                return ValidateGroupedColumnReference(
                    syntaxEntityId,
                    parts,
                    localMatches[0].Column,
                    localMatches[0].Source,
                    groupingContext,
                    withinAggregate);
            }

            var inheritedMatches = inheritedVisibleTableSources
                .SelectMany(source => source.Rowset.Columns.Select(column => (Source: source, Column: column)))
                .Where(item => string.Equals(item.Column.Name, parts[0], StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (inheritedMatches.Length > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "ColumnReferenceAmbiguous",
                    $"Column '{parts[0]}' resolves ambiguously across visible table sources.",
                    syntaxEntityId));
                return null;
            }

            if (inheritedMatches.Length == 1)
            {
                return ValidateGroupedColumnReference(
                    syntaxEntityId,
                    parts,
                    inheritedMatches[0].Column,
                    inheritedMatches[0].Source,
                    groupingContext,
                    withinAggregate);
            }

            if (TryInferColumnReferenceFromSources(
                    syntaxEntityId,
                    parts,
                    localVisibleTableSources,
                    groupingContext,
                    withinAggregate,
                    out var inferredLocalReference))
            {
                return inferredLocalReference;
            }

            if (TryInferColumnReferenceFromSources(
                    syntaxEntityId,
                    parts,
                    inheritedVisibleTableSources,
                    groupingContext,
                    withinAggregate,
                    out var inferredInheritedReference))
            {
                return inferredInheritedReference;
            }

            issues.Add(new TransformBindingIssue(
                "ColumnReferenceNotFound",
                $"Column '{parts[0]}' is not visible in the current query scope.",
                syntaxEntityId));
            return null;
        }

        if (parts.Count == 2)
        {
            var matchedLocalSources = localVisibleTableSources
                .Where(item => string.Equals(item.ExposedName, parts[0], StringComparison.OrdinalIgnoreCase))
                .ToArray();

            RuntimeTableSource[] matchedSources;
            if (matchedLocalSources.Length > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "ColumnQualifierAmbiguous",
                    $"Column qualifier '{parts[0]}' matches more than one visible table source.",
                    syntaxEntityId));
                return null;
            }

            if (matchedLocalSources.Length == 1)
            {
                matchedSources = matchedLocalSources;
            }
            else
            {
                var matchedInheritedSources = inheritedVisibleTableSources
                    .Where(item => string.Equals(item.ExposedName, parts[0], StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (matchedInheritedSources.Length == 0)
                {
                    issues.Add(new TransformBindingIssue(
                        "ColumnQualifierNotFound",
                        $"Column qualifier '{parts[0]}' is not visible in the current query scope.",
                        syntaxEntityId));
                    return null;
                }

                if (matchedInheritedSources.Length > 1)
                {
                    issues.Add(new TransformBindingIssue(
                        "ColumnQualifierAmbiguous",
                        $"Column qualifier '{parts[0]}' matches more than one visible table source.",
                        syntaxEntityId));
                    return null;
                }

                matchedSources = matchedInheritedSources;
            }

            var matchedColumns = matchedSources[0].Rowset.Columns
                .Where(item => string.Equals(item.Name, parts[1], StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (matchedColumns.Length == 0)
            {
                if (CanInferSourceColumn(matchedSources[0]))
                {
                    if (sourceSchemaResolver is not null &&
                        matchedSources[0].Rowset.Columns.Count > 0)
                    {
                        issues.Add(new TransformBindingIssue(
                            "QualifiedColumnReferenceNotFound",
                            $"Column '{parts[1]}' is not exposed by table source '{parts[0]}'.",
                            syntaxEntityId));
                        return null;
                    }

                    var inferredColumn = EnsureInferredSourceColumn(matchedSources[0], parts[1]);
                    return ValidateGroupedColumnReference(
                        syntaxEntityId,
                        parts,
                        inferredColumn,
                        matchedSources[0],
                        groupingContext,
                        withinAggregate);
                }

                issues.Add(new TransformBindingIssue(
                    "QualifiedColumnReferenceNotFound",
                    $"Column '{parts[1]}' is not exposed by table source '{parts[0]}'.",
                    syntaxEntityId));
                return null;
            }

            if (matchedColumns.Length > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "QualifiedColumnReferenceAmbiguous",
                    $"Column '{parts[1]}' resolves ambiguously within table source '{parts[0]}'.",
                    syntaxEntityId));
                return null;
            }

            return ValidateGroupedColumnReference(
                syntaxEntityId,
                parts,
                matchedColumns[0],
                matchedSources[0],
                groupingContext,
                withinAggregate);
        }

        issues.Add(new TransformBindingIssue(
            "UnsupportedColumnReferenceShape",
            $"Column reference '{syntaxEntityId}' uses {parts.Count} identifier parts; binding supports one-part or two-part references only.",
            syntaxEntityId));
        return null;
    }

    private bool TryBindFunctionParameterReference(
        string syntaxEntityId,
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
                    syntaxEntityId));
            }

            return true;
        }

        issues.Add(new TransformBindingIssue(
            "ScalarVariableReferenceNotSupported",
            $"Scalar variable reference '{name}' is not currently supported outside inline TVF parameter binding.",
            syntaxEntityId));
        return true;
    }

    private RuntimeColumnReference? ValidateGroupedColumnReference(
        string syntaxEntityId,
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
                    syntaxEntityId));
                return null;
            }
        }

        return new RuntimeColumnReference(syntaxEntityId, parts, column, tableSource);
    }

    private bool TryInferColumnReferenceFromSources(
        string syntaxEntityId,
        IReadOnlyList<string> parts,
        IReadOnlyList<RuntimeTableSource> candidateSources,
        RuntimeGroupingContext? groupingContext,
        bool withinAggregate,
        out RuntimeColumnReference? inferredColumnReference)
    {
        inferredColumnReference = null;

        if (candidateSources.Count == 0)
        {
            return false;
        }

        var inferableSources = candidateSources
            .Where(CanInferSourceColumn)
            .ToArray();
        if (inferableSources.Length == 0)
        {
            return false;
        }

        var unresolvedInferableSources = inferableSources
            .Where(static item => item.Rowset.Columns.Count == 0)
            .ToArray();

        if (sourceSchemaResolver is not null)
        {
            if (unresolvedInferableSources.Length == 1)
            {
                var inferredColumn = EnsureInferredSourceColumn(unresolvedInferableSources[0], parts[0]);
                inferredColumnReference = ValidateGroupedColumnReference(
                    syntaxEntityId,
                    parts,
                    inferredColumn,
                    unresolvedInferableSources[0],
                    groupingContext,
                    withinAggregate);
                return true;
            }

            if (unresolvedInferableSources.Length > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "ColumnReferenceRequiresValidationSchema",
                    $"Column '{parts[0]}' could belong to more than one visible source rowset; Binding cannot resolve it from syntax alone.",
                    syntaxEntityId));
                return true;
            }

            return false;
        }

        if (inferableSources.Length == 1)
        {
            var inferredColumn = EnsureInferredSourceColumn(inferableSources[0], parts[0]);
            inferredColumnReference = ValidateGroupedColumnReference(
                syntaxEntityId,
                parts,
                inferredColumn,
                inferableSources[0],
                groupingContext,
                withinAggregate);
            return true;
        }

        if (inferableSources.Length > 1)
        {
            issues.Add(new TransformBindingIssue(
                "ColumnReferenceRequiresValidationSchema",
                $"Column '{parts[0]}' could belong to more than one visible source rowset; Binding cannot resolve it from syntax alone.",
                syntaxEntityId));
            return true;
        }

        return false;
    }

    private static RuntimeTableSource[] GetLocalVisibleTableSources(BindingScope scope)
    {
        var localCount = Math.Clamp(
            scope.LocalVisibleTableSourceCount,
            0,
            scope.VisibleTableSources.Count);
        if (localCount == 0)
        {
            return [];
        }

        return scope.VisibleTableSources.Take(localCount).ToArray();
    }

    private static RuntimeTableSource[] GetInheritedVisibleTableSources(BindingScope scope)
    {
        var localCount = Math.Clamp(
            scope.LocalVisibleTableSourceCount,
            0,
            scope.VisibleTableSources.Count);
        if (localCount >= scope.VisibleTableSources.Count)
        {
            return [];
        }

        return scope.VisibleTableSources.Skip(localCount).ToArray();
    }

    private static string NormalizeColumnReferenceSignature(IReadOnlyList<string> parts) =>
        string.Join(".", parts).Trim().ToUpperInvariant();

    private static bool CanInferSourceColumn(RuntimeTableSource tableSource)
    {
        if (string.Equals(tableSource.Rowset.DerivationKind, "Source", StringComparison.Ordinal))
        {
            return true;
        }

        return string.Equals(tableSource.Rowset.DerivationKind, "FunctionTableReference", StringComparison.Ordinal) &&
               tableSource.Rowset.Columns.Count == 0;
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
