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
            return BindNamedTableReference(
                tableReference,
                namedTableReference,
                visibleCommonTableExpressionOrdinal,
                inheritedVisibleTableSources);
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

        var xmlNodesTableReference = navigator.TryGetXmlNodesTableReference(tableReference);
        if (xmlNodesTableReference is not null)
        {
            return BindXmlNodesTableReference(
                tableReference,
                xmlNodesTableReference,
                inheritedVisibleTableSources,
                inheritedInputRowset);
        }

        var fullTextTableReference = navigator.TryGetFullTextTableReference(tableReference);
        if (fullTextTableReference is not null)
        {
            return BindFullTextTableReference(
                tableReference,
                fullTextTableReference,
                inheritedVisibleTableSources);
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

            // The join predicate sees the inputs before outer-join null extension.
            var predicateSources = first.VisibleTableSources.Concat(second.VisibleTableSources).ToArray();

            if (joinOperator is "RightOuter" or "FullOuter") first = NullExtendJoinSide(first);
            if (joinOperator is "LeftOuter" or "FullOuter" or "OuterApply") second = NullExtendJoinSide(second);

            var joinedColumns = first.Rowset.Columns
                .Concat(second.Rowset.Columns)
                .Select((column, ordinal) => new RuntimeColumn(
                    $"{tableReference.Id}:column:{ordinal + 1}",
                    column.Name,
                    ordinal,
                    column.DataType))
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

            var visibleSources = first.VisibleTableSources.Concat(second.VisibleTableSources).ToArray();
            if (navigator.TryGetJoinSearchCondition(tableReference) is { } condition)
                BindBooleanExpression(condition,
                    new BindingScope(predicateSources.Concat(inheritedVisibleTableSources).ToArray(), predicateSources.Length),
                    joinRowset, groupingContext: null);
            return new RuntimeTableReferenceBinding(
                joinRowset,
                visibleSources);
        }

        issues.Add(new TransformBindingIssue(
            "UnsupportedTableReferenceShape",
            $"TableReference '{tableReference.Id}' is not yet supported by binding.",
            tableReference.Id));
        return null;
    }

    private static RuntimeTableReferenceBinding NullExtendJoinSide(RuntimeTableReferenceBinding binding)
    {
        static RuntimeRowset Extend(RuntimeRowset rowset) => rowset.Columns.All(column => column.DataType is null) ? rowset : rowset with
        {
            Columns = rowset.Columns.Select(column => column with
            {
                DataType = column.DataType is { } type ? type with { IsNullable = true } : null
            }).ToArray()
        };
        return new RuntimeTableReferenceBinding(Extend(binding.Rowset),
            binding.VisibleTableSources.Select(source => source with { Rowset = Extend(source.Rowset) }).ToArray());
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
                ordinal,
                column.DataType))
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
