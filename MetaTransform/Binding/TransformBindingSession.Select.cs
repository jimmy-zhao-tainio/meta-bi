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
                ordinal,
                column.DataType))
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
        RuntimeColumnDataType? outputDataType = null;
        var directColumnReference = navigator.TryGetDirectColumnReference(scalarExpression);
        if (directColumnReference is not null)
        {
            boundColumnReference = BindColumnReference(directColumnReference, scope, groupingContext, withinAggregate: false);
            if (boundColumnReference is not null)
            {
                boundColumnReferences.Add(boundColumnReference);
                outputDataType = boundColumnReference.ResolvedColumn.DataType;
            }
        }
        else
        {
            BindScalarExpression(scalarExpression, scope, inputRowset, groupingContext, withinAggregate: false);
            var literal = navigator.TryGetLiteral(scalarExpression);
            if (literal is not null)
            {
                outputDataType = CreateLiteralDataType(literal);
            }
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
            outputColumns.Count,
            outputDataType));
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
                    outputColumns.Count,
                    column.DataType));
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
                    $"Column reference '{FormatIdentifierParts(parts)}' resolves ambiguously across visible table sources. Visible table sources: {FormatVisibleTableSources(scope.VisibleTableSources)}.",
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
                $"Column reference '{FormatIdentifierParts(parts)}' is not visible in the current query scope. Visible table sources: {FormatVisibleTableSources(scope.VisibleTableSources)}.",
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
                        $"Column qualifier '{parts[0]}' from column reference '{FormatIdentifierParts(parts)}' is not visible in the current query scope. Visible table sources: {FormatVisibleTableSources(scope.VisibleTableSources)}.",
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
                            $"Column '{parts[1]}' from column reference '{FormatIdentifierParts(parts)}' is not exposed by table source '{FormatTableSource(matchedSources[0])}'.",
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
                    $"Column '{parts[1]}' from column reference '{FormatIdentifierParts(parts)}' is not exposed by table source '{FormatTableSource(matchedSources[0])}'.",
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

        var qualifiedMultipartReference = TryBindQualifiedMultipartColumnReference(
            parts,
            syntaxEntityId,
            localVisibleTableSources,
            inheritedVisibleTableSources,
            groupingContext,
            withinAggregate);
        if (qualifiedMultipartReference is not null)
        {
            return qualifiedMultipartReference;
        }

        issues.Add(new TransformBindingIssue(
            "UnsupportedColumnReferenceShape",
            $"Column reference '{FormatIdentifierParts(parts)}' uses {parts.Count} identifier parts; binding supports one-part, two-part, or source-qualified multipart references.",
            syntaxEntityId));
        return null;
    }

    private RuntimeColumnReference? TryBindQualifiedMultipartColumnReference(
        IReadOnlyList<string> parts,
        string syntaxEntityId,
        IReadOnlyList<RuntimeTableSource> localVisibleTableSources,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources,
        RuntimeGroupingContext? groupingContext,
        bool withinAggregate)
    {
        if (parts.Count <= 2)
        {
            return null;
        }

        var exposedNameMatchedSources = localVisibleTableSources
            .Where(item => string.Equals(item.ExposedName, parts[0], StringComparison.OrdinalIgnoreCase))
            .Concat(inheritedVisibleTableSources.Where(item => string.Equals(item.ExposedName, parts[0], StringComparison.OrdinalIgnoreCase)))
            .Distinct()
            .ToArray();

        if (exposedNameMatchedSources.Length == 1)
        {
            var candidateColumnNames = new[]
            {
                string.Join(".", parts.Skip(1)),
                string.Concat(parts.Skip(1))
            };

            foreach (var candidateColumnName in candidateColumnNames)
            {
                var matchedColumns = exposedNameMatchedSources[0].Rowset.Columns
                    .Where(item => string.Equals(item.Name, candidateColumnName, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (matchedColumns.Length == 1)
                {
                    return ValidateGroupedColumnReference(
                        syntaxEntityId,
                        parts,
                        matchedColumns[0],
                        exposedNameMatchedSources[0],
                        groupingContext,
                        withinAggregate);
                }
            }
        }

        var sourceQualifiedReference = TryBindSourceQualifiedMultipartColumnReference(
            parts,
            syntaxEntityId,
            localVisibleTableSources,
            inheritedVisibleTableSources,
            groupingContext,
            withinAggregate);
        if (sourceQualifiedReference is not null)
        {
            return sourceQualifiedReference;
        }

        return null;
    }

    private RuntimeColumnReference? TryBindSourceQualifiedMultipartColumnReference(
        IReadOnlyList<string> parts,
        string syntaxEntityId,
        IReadOnlyList<RuntimeTableSource> localVisibleTableSources,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources,
        RuntimeGroupingContext? groupingContext,
        bool withinAggregate)
    {
        if (parts.Count <= 2)
        {
            return null;
        }

        var sourceQualifierParts = parts.Take(parts.Count - 1).ToArray();
        var columnName = parts[^1];

        var matchedLocalSources = localVisibleTableSources
            .Where(item => TableSourceIdentifierMatches(item, sourceQualifierParts))
            .ToArray();
        RuntimeTableSource[] matchedSources;
        if (matchedLocalSources.Length > 0)
        {
            matchedSources = matchedLocalSources;
        }
        else
        {
            matchedSources = inheritedVisibleTableSources
                .Where(item => TableSourceIdentifierMatches(item, sourceQualifierParts))
                .ToArray();
        }

        if (matchedSources.Length == 0)
        {
            return null;
        }

        if (matchedSources.Length > 1)
        {
            issues.Add(new TransformBindingIssue(
                "ColumnQualifierAmbiguous",
                $"Column qualifier '{string.Join(".", sourceQualifierParts)}' from column reference '{FormatIdentifierParts(parts)}' matches more than one visible table source.",
                syntaxEntityId));
            return null;
        }

        var tableSource = matchedSources[0];
        var matchedColumns = tableSource.Rowset.Columns
            .Where(item => string.Equals(item.Name, columnName, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (matchedColumns.Length == 0)
        {
            if (CanInferSourceColumn(tableSource))
            {
                if (sourceSchemaResolver is not null &&
                    tableSource.Rowset.Columns.Count > 0)
                {
                    issues.Add(new TransformBindingIssue(
                        "QualifiedColumnReferenceNotFound",
                        $"Column '{columnName}' from column reference '{FormatIdentifierParts(parts)}' is not exposed by table source '{FormatTableSource(tableSource)}'.",
                        syntaxEntityId));
                    return null;
                }

                var inferredColumn = EnsureInferredSourceColumn(tableSource, columnName);
                return ValidateGroupedColumnReference(
                    syntaxEntityId,
                    parts,
                    inferredColumn,
                    tableSource,
                    groupingContext,
                    withinAggregate);
            }

            issues.Add(new TransformBindingIssue(
                "QualifiedColumnReferenceNotFound",
                $"Column '{columnName}' from column reference '{FormatIdentifierParts(parts)}' is not exposed by table source '{FormatTableSource(tableSource)}'.",
                syntaxEntityId));
            return null;
        }

        if (matchedColumns.Length > 1)
        {
            issues.Add(new TransformBindingIssue(
                "QualifiedColumnReferenceAmbiguous",
                $"Column '{columnName}' resolves ambiguously within table source '{string.Join(".", sourceQualifierParts)}'.",
                syntaxEntityId));
            return null;
        }

        return ValidateGroupedColumnReference(
            syntaxEntityId,
            parts,
            matchedColumns[0],
            tableSource,
            groupingContext,
            withinAggregate);
    }

    private bool TableSourceIdentifierMatches(
        RuntimeTableSource tableSource,
        IReadOnlyList<string> qualifierParts)
    {
        if (IdentifierPartsMatch(GetIdentifierParts(tableSource.SqlIdentifier), qualifierParts))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(executeSystemName))
        {
            return false;
        }

        var expanded = SourceSqlIdentifierExpansion.Expand(
            tableSource.SqlIdentifier,
            executeSystemName,
            executeSystemDefaultSchemaName);
        return expanded.IsSuccess &&
               IdentifierPartsMatch(expanded.ExpandedIdentifierParts, qualifierParts);
    }

    private static bool IdentifierPartsMatch(
        IReadOnlyList<string> left,
        IReadOnlyList<string> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        for (var i = 0; i < left.Count; i++)
        {
            if (!string.Equals(
                    NormalizeIdentifierPart(left[i]),
                    NormalizeIdentifierPart(right[i]),
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlyList<string> GetIdentifierParts(string sqlIdentifier)
    {
        if (string.IsNullOrWhiteSpace(sqlIdentifier))
        {
            return [];
        }

        return sqlIdentifier
            .Split('.', StringSplitOptions.TrimEntries)
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Select(NormalizeIdentifierPart)
            .ToArray();
    }

    private static string NormalizeIdentifierPart(string value)
    {
        var trimmed = value.Trim();
        return trimmed.Length >= 2 && trimmed[0] == '[' && trimmed[^1] == ']'
            ? trimmed[1..^1].Trim()
            : trimmed;
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

        if (isInlineTableValuedFunction || activeTransformFunctionParameterNames.Count > 0)
        {
            if (!activeTransformFunctionParameterNames.Contains(name))
            {
                issues.Add(new TransformBindingIssue(
                    "FunctionParameterReferenceNotFound",
                    $"Function parameter '{name}' is referenced but not declared on the active transform function script.",
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
            var identity = GetColumnReferenceIdentity(column, tableSource);
            if (!groupingContext.GroupingKeySignatures.Contains(signature) &&
                !groupingContext.GroupingKeyColumnIdentities.Contains(identity))
            {
                issues.Add(new TransformBindingIssue(
                    "UngroupedColumnReference",
                    $"Column reference '{FormatIdentifierParts(parts)}' is not part of the grouped key set and is used outside an aggregate context. Grouped keys: {FormatGroupingKeys(groupingContext)}. Resolved table source: {FormatTableSource(tableSource)}.",
                    syntaxEntityId));
                return null;
            }
        }

        return new RuntimeColumnReference(syntaxEntityId, parts, column, tableSource);
    }

    private static string FormatIdentifierParts(IReadOnlyList<string> parts) =>
        parts.Count == 0
            ? "<empty>"
            : string.Join(".", parts);

    private static string FormatVisibleTableSources(IEnumerable<RuntimeTableSource> tableSources)
    {
        var formatted = tableSources
            .Select(FormatTableSource)
            .ToArray();
        return formatted.Length == 0
            ? "<none>"
            : string.Join("; ", formatted);
    }

    private static string FormatTableSource(RuntimeTableSource tableSource)
    {
        var sqlIdentifier = string.IsNullOrWhiteSpace(tableSource.SqlIdentifier)
            ? "<derived>"
            : tableSource.SqlIdentifier;
        return $"{tableSource.ExposedName} ({sqlIdentifier}; columns: {FormatColumnNames(tableSource.Rowset.Columns)})";
    }

    private static string FormatColumnNames(IReadOnlyList<RuntimeColumn> columns)
    {
        if (columns.Count == 0)
        {
            return "<unknown>";
        }

        const int maxColumns = 12;
        var names = columns
            .Take(maxColumns)
            .Select(static item => item.Name)
            .ToArray();
        var suffix = columns.Count > maxColumns
            ? $", ... +{columns.Count - maxColumns}"
            : string.Empty;
        return string.Join(", ", names) + suffix;
    }

    private static string FormatGroupingKeys(RuntimeGroupingContext groupingContext)
    {
        var signatures = groupingContext.GroupingKeySignatures
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return signatures.Length == 0
            ? "<none>"
            : string.Join(", ", signatures);
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

    private static string GetColumnReferenceIdentity(
        RuntimeColumn column,
        RuntimeTableSource tableSource) =>
        $"{tableSource.SyntaxTableReferenceId}:{column.Id}";

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
