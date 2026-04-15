using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private RuntimeTableReferenceBinding? BindTableReference(
        TableReference tableReference,
        int visibleCommonTableExpressionOrdinal,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources,
        RuntimeRowset? inheritedInputRowset)
    {
        var namedTableReference = navigator.TryGetNamedTableReference(tableReference);
        if (namedTableReference is not null)
        {
            return BindNamedTableReference(tableReference, namedTableReference, visibleCommonTableExpressionOrdinal);
        }

        var queryDerivedTable = navigator.TryGetQueryDerivedTable(tableReference);
        if (queryDerivedTable is not null)
        {
            return BindQueryDerivedTable(
                tableReference,
                queryDerivedTable,
                visibleCommonTableExpressionOrdinal,
                inheritedVisibleTableSources,
                inheritedInputRowset);
        }

        var inlineDerivedTable = navigator.TryGetInlineDerivedTable(tableReference);
        if (inlineDerivedTable is not null)
        {
            return BindInlineDerivedTable(
                tableReference,
                inlineDerivedTable,
                inheritedVisibleTableSources,
                inheritedInputRowset);
        }

        var pivotedTableReference = navigator.TryGetPivotedTableReference(tableReference);
        if (pivotedTableReference is not null)
        {
            return BindPivotedTableReference(
                tableReference,
                pivotedTableReference,
                visibleCommonTableExpressionOrdinal,
                inheritedVisibleTableSources,
                inheritedInputRowset);
        }

        var unpivotedTableReference = navigator.TryGetUnpivotedTableReference(tableReference);
        if (unpivotedTableReference is not null)
        {
            return BindUnpivotedTableReference(
                tableReference,
                unpivotedTableReference,
                visibleCommonTableExpressionOrdinal,
                inheritedVisibleTableSources,
                inheritedInputRowset);
        }

        var globalFunctionTableReference = navigator.TryGetGlobalFunctionTableReference(tableReference);
        if (globalFunctionTableReference is not null)
        {
            return BindGlobalFunctionTableReference(
                tableReference,
                globalFunctionTableReference,
                inheritedVisibleTableSources);
        }

        var schemaObjectFunctionTableReference = navigator.TryGetSchemaObjectFunctionTableReference(tableReference);
        if (schemaObjectFunctionTableReference is not null)
        {
            return BindSchemaObjectFunctionTableReference(
                tableReference,
                schemaObjectFunctionTableReference,
                inheritedVisibleTableSources);
        }

        var parenthesizedJoinReference = navigator.TryGetJoinParenthesisInnerJoinReference(tableReference);
        if (parenthesizedJoinReference is not null)
        {
            return BindTableReference(
                parenthesizedJoinReference,
                visibleCommonTableExpressionOrdinal,
                inheritedVisibleTableSources,
                inheritedInputRowset);
        }

        var joinChildren = navigator.TryGetJoinChildren(tableReference);
        if (joinChildren is not null)
        {
            var joinOperator = navigator.TryGetJoinOperator(tableReference);
            var isApply =
                string.Equals(joinOperator, "CrossApply", StringComparison.Ordinal) ||
                string.Equals(joinOperator, "OuterApply", StringComparison.Ordinal);

            RuntimeTableReferenceBinding? first = null;
            RuntimeTableReferenceBinding? second = null;

            if (joinChildren.Value.First is not null)
            {
                first = BindTableReference(
                    joinChildren.Value.First,
                    visibleCommonTableExpressionOrdinal,
                    inheritedVisibleTableSources,
                    inheritedInputRowset);
            }

            if (joinChildren.Value.Second is not null)
            {
                second = BindTableReference(
                    joinChildren.Value.Second,
                    visibleCommonTableExpressionOrdinal,
                    isApply
                        ? inheritedVisibleTableSources.Concat(first?.VisibleTableSources ?? []).ToArray()
                        : inheritedVisibleTableSources,
                    isApply
                        ? ComposeJoinSideInputRowset(tableReference, inheritedInputRowset, first?.Rowset)
                        : inheritedInputRowset);
            }

            if (first is null || second is null)
            {
                return null;
            }

            var joinedColumns = first.Rowset.Columns
                .Concat(second.Rowset.Columns)
                .Select((column, ordinal) => new RuntimeColumn(
                    $"{tableReference.Id}:column:{ordinal + 1}",
                    column.Name,
                    ordinal))
                .ToArray();

            var inputRolePrefix = isApply ? "Apply" : "Join";
            var joinRowset = new RuntimeRowset(
                $"{tableReference.Id}:rowset",
                $"{joinOperator ?? "Join"}:{tableReference.Id}",
                isApply ? "Apply" : "Join",
                null,
                tableReference.Id,
                null,
                joinedColumns,
                [
                    new RuntimeRowsetInput(0, $"{inputRolePrefix}Left", first.Rowset),
                    new RuntimeRowsetInput(1, $"{inputRolePrefix}Right", second.Rowset)
                ]);

            TrackRowset(joinRowset);

            return new RuntimeTableReferenceBinding(
                joinRowset,
                first.VisibleTableSources.Concat(second.VisibleTableSources).ToArray());
        }

        issues.Add(new TransformBindingIssue(
            "UnsupportedTableReferenceShape",
            $"TableReference '{tableReference.Id}' is not yet supported by binding.",
            tableReference.Id));
        return null;
    }

    private RuntimeRowset? ComposeJoinSideInputRowset(
        TableReference joinTableReference,
        RuntimeRowset? inheritedInputRowset,
        RuntimeRowset? leftRowset)
    {
        if (inheritedInputRowset is null)
        {
            return leftRowset;
        }

        if (leftRowset is null)
        {
            return inheritedInputRowset;
        }

        var columns = inheritedInputRowset.Columns
            .Concat(leftRowset.Columns)
            .Select((column, ordinal) => new RuntimeColumn(
                $"{joinTableReference.Id}:apply-input-column:{ordinal + 1}",
                column.Name,
                ordinal))
            .ToArray();

        var rowset = new RuntimeRowset(
            $"{joinTableReference.Id}:apply-input-rowset",
            $"ApplyInput:{joinTableReference.Id}",
            "ApplyInput",
            null,
            joinTableReference.Id,
            null,
            columns,
            [
                new RuntimeRowsetInput(0, "OuterScope", inheritedInputRowset),
                new RuntimeRowsetInput(1, "ApplyLeft", leftRowset)
            ]);

        TrackRowset(rowset);
        return rowset;
    }
}
