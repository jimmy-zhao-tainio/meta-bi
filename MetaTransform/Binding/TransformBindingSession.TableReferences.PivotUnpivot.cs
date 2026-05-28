using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private RuntimeTableReferenceBinding? BindPivotedTableReference(
        TableReference tableReference,
        PivotedTableReference pivotedTableReference,
        int visibleCommonTableExpressionOrdinal,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources,
        RuntimeRowset? inheritedInputRowset)
    {
        var sourceTableReference = navigator.TryGetPivotedTableReferenceSourceTableReference(pivotedTableReference);
        if (sourceTableReference is null)
        {
            issues.Add(new TransformBindingIssue(
                "PivotTableReferenceSourceMissing",
                $"Pivoted table reference '{pivotedTableReference.Id}' is missing its source table reference.",
                tableReference.Id));
            return null;
        }

        var sourceBinding = BindTableReference(
            sourceTableReference,
            visibleCommonTableExpressionOrdinal,
            inheritedVisibleTableSources,
            inheritedInputRowset);
        if (sourceBinding is null)
        {
            return null;
        }

        if (string.Equals(sourceBinding.Rowset.DerivationKind, "Source", StringComparison.Ordinal))
        {
            issues.Add(new TransformBindingIssue(
                "PivotSourceShapeRequiresDerivedInput",
                $"Pivoted table reference '{pivotedTableReference.Id}' requires a syntax-derived source rowset shape (for example a derived table or CTE projection).",
                tableReference.Id));
            return null;
        }

        var localVisibleTableSources = sourceBinding.VisibleTableSources.ToArray();
        var scope = new BindingScope(
            localVisibleTableSources.Concat(inheritedVisibleTableSources).ToArray(),
            localVisibleTableSources.Length);

        var valueColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var valueColumnReference in navigator.GetPivotedTableReferenceValueColumns(pivotedTableReference))
        {
            var bound = BindColumnReference(valueColumnReference, scope, groupingContext: null, withinAggregate: false);
            if (bound is not null)
            {
                boundColumnReferences.Add(bound);
                valueColumnNames.Add(bound.ResolvedColumn.Name);
            }
            else
            {
                var parts = navigator.GetColumnReferenceParts(valueColumnReference);
                var fallbackName = parts.LastOrDefault();
                if (!string.IsNullOrWhiteSpace(fallbackName))
                {
                    valueColumnNames.Add(fallbackName);
                }
            }
        }

        var pivotColumnReference = navigator.TryGetPivotedTableReferencePivotColumn(pivotedTableReference);
        if (pivotColumnReference is null)
        {
            issues.Add(new TransformBindingIssue(
                "PivotTableReferencePivotColumnMissing",
                $"Pivoted table reference '{pivotedTableReference.Id}' is missing its pivot column reference.",
                tableReference.Id));
            return null;
        }

        string? pivotColumnName = null;
        var boundPivotColumn = BindColumnReference(pivotColumnReference, scope, groupingContext: null, withinAggregate: false);
        if (boundPivotColumn is not null)
        {
            boundColumnReferences.Add(boundPivotColumn);
            pivotColumnName = boundPivotColumn.ResolvedColumn.Name;
        }
        else
        {
            var pivotParts = navigator.GetColumnReferenceParts(pivotColumnReference);
            pivotColumnName = pivotParts.LastOrDefault();
        }

        var inColumns = navigator.GetPivotedTableReferenceInColumns(pivotedTableReference);
        if (inColumns.Count == 0)
        {
            issues.Add(new TransformBindingIssue(
                "PivotTableReferenceInColumnsMissing",
                $"Pivoted table reference '{pivotedTableReference.Id}' is missing its IN column list.",
                tableReference.Id));
            return null;
        }

        var passthroughColumns = sourceBinding.Rowset.Columns
            .Where(item =>
                !valueColumnNames.Contains(item.Name) &&
                !string.Equals(item.Name, pivotColumnName, StringComparison.OrdinalIgnoreCase))
            .Select((column, ordinal) => new RuntimeColumn(
                $"{tableReference.Id}:column:{ordinal + 1}",
                column.Name,
                ordinal))
            .ToList();

        for (var ordinal = 0; ordinal < inColumns.Count; ordinal++)
        {
            passthroughColumns.Add(new RuntimeColumn(
                $"{tableReference.Id}:column:{passthroughColumns.Count + 1}",
                inColumns[ordinal],
                passthroughColumns.Count));
        }

        var exposedName = navigator.TryGetTableAlias(tableReference);
        if (string.IsNullOrWhiteSpace(exposedName))
        {
            issues.Add(new TransformBindingIssue(
                "PivotTableReferenceAliasMissing",
                $"Pivoted table reference '{pivotedTableReference.Id}' does not expose a required alias.",
                tableReference.Id));
            return null;
        }

        var rowset = new RuntimeRowset(
            $"{tableReference.Id}:rowset",
            exposedName,
            "Pivot",
            null,
            tableReference.Id,
            null,
            passthroughColumns,
            [new RuntimeRowsetInput(0, "Input", sourceBinding.Rowset)]);

        TrackRowset(rowset);

        var tableSource = new RuntimeTableSource(
            tableReference.Id,
            exposedName,
            string.Empty,
            rowset);

        TrackTableSource(tableSource);
        return new RuntimeTableReferenceBinding(rowset, [tableSource]);
    }

    private RuntimeTableReferenceBinding? BindUnpivotedTableReference(
        TableReference tableReference,
        UnpivotedTableReference unpivotedTableReference,
        int visibleCommonTableExpressionOrdinal,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources,
        RuntimeRowset? inheritedInputRowset)
    {
        var sourceTableReference = navigator.TryGetUnpivotedTableReferenceSourceTableReference(unpivotedTableReference);
        if (sourceTableReference is null)
        {
            issues.Add(new TransformBindingIssue(
                "UnpivotTableReferenceSourceMissing",
                $"Unpivoted table reference '{unpivotedTableReference.Id}' is missing its source table reference.",
                tableReference.Id));
            return null;
        }

        var sourceBinding = BindTableReference(
            sourceTableReference,
            visibleCommonTableExpressionOrdinal,
            inheritedVisibleTableSources,
            inheritedInputRowset);
        if (sourceBinding is null)
        {
            return null;
        }

        if (string.Equals(sourceBinding.Rowset.DerivationKind, "Source", StringComparison.Ordinal))
        {
            issues.Add(new TransformBindingIssue(
                "UnpivotSourceShapeRequiresDerivedInput",
                $"Unpivoted table reference '{unpivotedTableReference.Id}' requires a syntax-derived source rowset shape (for example a derived table or CTE projection).",
                tableReference.Id));
            return null;
        }

        var localVisibleTableSources = sourceBinding.VisibleTableSources.ToArray();
        var scope = new BindingScope(
            localVisibleTableSources.Concat(inheritedVisibleTableSources).ToArray(),
            localVisibleTableSources.Length);
        var inColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var inColumnReference in navigator.GetUnpivotedTableReferenceInColumns(unpivotedTableReference))
        {
            var bound = BindColumnReference(inColumnReference, scope, groupingContext: null, withinAggregate: false);
            if (bound is not null)
            {
                boundColumnReferences.Add(bound);
                inColumnNames.Add(bound.ResolvedColumn.Name);
            }
            else
            {
                var parts = navigator.GetColumnReferenceParts(inColumnReference);
                var fallbackName = parts.LastOrDefault();
                if (!string.IsNullOrWhiteSpace(fallbackName))
                {
                    inColumnNames.Add(fallbackName);
                }
            }
        }

        var valueColumnName = navigator.TryGetUnpivotedTableReferenceValueColumnName(unpivotedTableReference);
        if (string.IsNullOrWhiteSpace(valueColumnName))
        {
            issues.Add(new TransformBindingIssue(
                "UnpivotValueColumnMissing",
                $"Unpivoted table reference '{unpivotedTableReference.Id}' is missing its value column name.",
                tableReference.Id));
            return null;
        }

        var pivotColumnName = navigator.TryGetUnpivotedTableReferencePivotColumnName(unpivotedTableReference);
        if (string.IsNullOrWhiteSpace(pivotColumnName))
        {
            issues.Add(new TransformBindingIssue(
                "UnpivotPivotColumnMissing",
                $"Unpivoted table reference '{unpivotedTableReference.Id}' is missing its pivot column name.",
                tableReference.Id));
            return null;
        }

        var outputColumns = sourceBinding.Rowset.Columns
            .Where(item => !inColumnNames.Contains(item.Name))
            .Select((column, ordinal) => new RuntimeColumn(
                $"{tableReference.Id}:column:{ordinal + 1}",
                column.Name,
                ordinal))
            .ToList();

        outputColumns.Add(new RuntimeColumn(
            $"{tableReference.Id}:column:{outputColumns.Count + 1}",
            pivotColumnName,
            outputColumns.Count));
        outputColumns.Add(new RuntimeColumn(
            $"{tableReference.Id}:column:{outputColumns.Count + 1}",
            valueColumnName,
            outputColumns.Count));

        var exposedName = navigator.TryGetTableAlias(tableReference);
        if (string.IsNullOrWhiteSpace(exposedName))
        {
            issues.Add(new TransformBindingIssue(
                "UnpivotTableReferenceAliasMissing",
                $"Unpivoted table reference '{unpivotedTableReference.Id}' does not expose a required alias.",
                tableReference.Id));
            return null;
        }

        var rowset = new RuntimeRowset(
            $"{tableReference.Id}:rowset",
            exposedName,
            "Unpivot",
            null,
            tableReference.Id,
            null,
            outputColumns,
            [new RuntimeRowsetInput(0, "Input", sourceBinding.Rowset)]);

        TrackRowset(rowset);

        var tableSource = new RuntimeTableSource(
            tableReference.Id,
            exposedName,
            string.Empty,
            rowset);

        TrackTableSource(tableSource);
        return new RuntimeTableReferenceBinding(rowset, [tableSource]);
    }
}
