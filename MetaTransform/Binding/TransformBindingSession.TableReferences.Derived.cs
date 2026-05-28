using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private RuntimeTableReferenceBinding? BindQueryDerivedTable(
        TableReference tableReference,
        QueryDerivedTable queryDerivedTable,
        int visibleCommonTableExpressionOrdinal,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources,
        RuntimeRowset? inheritedInputRowset)
    {
        var queryExpressionId = navigator.TryGetQueryDerivedTableQueryExpressionId(queryDerivedTable);
        if (string.IsNullOrWhiteSpace(queryExpressionId))
        {
            issues.Add(new TransformBindingIssue(
                "UnsupportedDerivedTableQueryShape",
                $"QueryDerivedTable '{queryDerivedTable.Id}' is missing its inner query expression.",
                queryDerivedTable.Id));
            return null;
        }

        var exposedName = navigator.TryGetTableAlias(tableReference);
        if (string.IsNullOrWhiteSpace(exposedName))
        {
            issues.Add(new TransformBindingIssue(
                "DerivedTableAliasMissing",
                $"QueryDerivedTable '{queryDerivedTable.Id}' does not expose a required alias.",
                tableReference.Id));
            return null;
        }

        var innerBinding = BindQueryExpression(
            queryExpressionId,
            $"{queryExpressionId}:output-rowset",
            $"{exposedName}:inner",
            null,
            visibleCommonTableExpressionOrdinal,
            inheritedVisibleTableSources,
            inheritedInputRowset,
            null);

        if (innerBinding is null)
        {
            return null;
        }

        var columnAliases = navigator.GetTableReferenceColumnAliases(tableReference);
        if (columnAliases.Count > 0 && columnAliases.Count != innerBinding.OutputRowset.Columns.Count)
        {
            issues.Add(new TransformBindingIssue(
                "DerivedTableColumnAliasCountMismatch",
                $"QueryDerivedTable '{queryDerivedTable.Id}' exposes {columnAliases.Count} column aliases but its query produces {innerBinding.OutputRowset.Columns.Count} columns.",
                tableReference.Id));
            return null;
        }

        var derivedColumns = innerBinding.OutputRowset.Columns
            .Select((column, ordinal) => new RuntimeColumn(
                $"{tableReference.Id}:column:{ordinal + 1}",
                columnAliases.Count == 0 ? column.Name : columnAliases[ordinal],
                ordinal))
            .ToArray();

        var rowset = new RuntimeRowset(
            $"{tableReference.Id}:rowset",
            exposedName,
            "DerivedTable",
            null,
            tableReference.Id,
            null,
            derivedColumns,
            [new RuntimeRowsetInput(0, "Input", innerBinding.OutputRowset)]);

        TrackRowset(rowset);

        var tableSource = new RuntimeTableSource(
            tableReference.Id,
            exposedName,
            string.Empty,
            rowset);

        TrackTableSource(tableSource);
        return new RuntimeTableReferenceBinding(rowset, [tableSource]);
    }

    private RuntimeTableReferenceBinding? BindInlineDerivedTable(
        TableReference tableReference,
        InlineDerivedTable inlineDerivedTable,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources,
        RuntimeRowset? inheritedInputRowset)
    {
        var exposedName = navigator.TryGetTableAlias(tableReference);
        if (string.IsNullOrWhiteSpace(exposedName))
        {
            issues.Add(new TransformBindingIssue(
                "InlineDerivedTableAliasMissing",
                $"InlineDerivedTable '{inlineDerivedTable.Id}' does not expose a required alias.",
                tableReference.Id));
            return null;
        }

        var columnAliases = navigator.GetTableReferenceColumnAliases(tableReference);
        if (columnAliases.Count == 0)
        {
            issues.Add(new TransformBindingIssue(
                "InlineDerivedTableColumnAliasesRequired",
                $"InlineDerivedTable '{inlineDerivedTable.Id}' does not expose column aliases, so its rowset shape is not yet sanctioned for binding.",
                tableReference.Id));
            return null;
        }

        var rowValues = navigator.GetInlineDerivedTableRowValues(inlineDerivedTable);
        if (rowValues.Count == 0)
        {
            issues.Add(new TransformBindingIssue(
                "InlineDerivedTableRowValuesMissing",
                $"InlineDerivedTable '{inlineDerivedTable.Id}' is missing its row values.",
                inlineDerivedTable.Id));
            return null;
        }

        var valueScope = new BindingScope(inheritedVisibleTableSources);
        var expectedWidth = columnAliases.Count;
        for (var rowOrdinal = 0; rowOrdinal < rowValues.Count; rowOrdinal++)
        {
            var columnValues = navigator.GetRowValueColumnValues(rowValues[rowOrdinal]);
            if (columnValues.Count != expectedWidth)
            {
                issues.Add(new TransformBindingIssue(
                    "InlineDerivedTableColumnCountMismatch",
                    $"InlineDerivedTable '{inlineDerivedTable.Id}' row {rowOrdinal + 1} exposes {columnValues.Count} values but the table alias declares {expectedWidth} columns.",
                    rowValues[rowOrdinal].Id));
                return null;
            }

            foreach (var columnValue in columnValues)
            {
                BindScalarExpression(columnValue, valueScope, inheritedInputRowset, null, withinAggregate: false);
            }
        }

        var columns = columnAliases
            .Select((columnName, ordinal) => new RuntimeColumn(
                $"{tableReference.Id}:column:{ordinal + 1}",
                columnName,
                ordinal))
            .ToArray();

        var rowset = new RuntimeRowset(
            $"{tableReference.Id}:rowset",
            exposedName,
            "InlineDerivedTable",
            null,
            tableReference.Id,
            null,
            columns,
            []);

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
