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

        var schemaObjectFunctionTableReference = navigator.TryGetSchemaObjectFunctionTableReference(tableReference);
        if (schemaObjectFunctionTableReference is not null)
        {
            return BindSchemaObjectFunctionTableReference(
                tableReference,
                schemaObjectFunctionTableReference,
                inheritedVisibleTableSources);
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

    private RuntimeTableReferenceBinding? BindNamedTableReference(
        TableReference tableReference,
        NamedTableReference namedTableReference,
        int visibleCommonTableExpressionOrdinal)
    {
        var identifierParts = navigator.GetNamedTableReferenceParts(namedTableReference);
        if (identifierParts.Count == 0)
        {
            issues.Add(new TransformBindingIssue(
                "UnsupportedSchemaObjectNameShape",
                $"Named table reference '{namedTableReference.Id}' does not expose a supported multipart identifier.",
                namedTableReference.Id));
            return null;
        }

        if (identifierParts.Count == 1)
        {
            var commonTableExpressionBinding = TryBindCommonTableExpressionReference(
                tableReference,
                identifierParts[0],
                visibleCommonTableExpressionOrdinal);

            if (commonTableExpressionBinding.IsResolved)
            {
                return commonTableExpressionBinding.Binding;
            }
        }

        var sqlIdentifier = string.Join(".", identifierParts);
        var exposedName = navigator.TryGetTableAlias(tableReference) ?? identifierParts.Last();
        var columns = new List<RuntimeColumn>();

        var rowset = new RuntimeRowset(
            $"{tableReference.Id}:rowset",
            sqlIdentifier,
            "Source",
            "Source",
            tableReference.Id,
            sqlIdentifier,
            columns,
            []);

        TrackRowset(rowset);

        var tableSource = new RuntimeTableSource(
            tableReference.Id,
            exposedName,
            sqlIdentifier,
            rowset);

        TrackTableSource(tableSource);
        return new RuntimeTableReferenceBinding(rowset, [tableSource]);
    }

    private CommonTableExpressionReferenceBindingResult TryBindCommonTableExpressionReference(
        TableReference tableReference,
        string commonTableExpressionName,
        int visibleCommonTableExpressionOrdinal)
    {
        if (!commonTableExpressionDefinitionsByName.TryGetValue(commonTableExpressionName, out var definition))
        {
            return CommonTableExpressionReferenceBindingResult.Unresolved;
        }

        if (definition.Ordinal > visibleCommonTableExpressionOrdinal)
        {
            issues.Add(new TransformBindingIssue(
                "CommonTableExpressionForwardReferenceNotAllowed",
                $"CommonTableExpression '{commonTableExpressionName}' is not yet visible at this binding location.",
                tableReference.Id));
            return CommonTableExpressionReferenceBindingResult.Resolved(null);
        }

        var rowset = BindCommonTableExpression(definition);
        if (rowset is null)
        {
            return CommonTableExpressionReferenceBindingResult.Resolved(null);
        }

        var exposedName = navigator.TryGetTableAlias(tableReference) ?? definition.Name;
        var tableSource = new RuntimeTableSource(
            tableReference.Id,
            exposedName,
            string.Empty,
            rowset);

        TrackTableSource(tableSource);
        return CommonTableExpressionReferenceBindingResult.Resolved(
            new RuntimeTableReferenceBinding(rowset, [tableSource]));
    }

    private RuntimeRowset? BindCommonTableExpression(RuntimeCommonTableExpressionDefinition definition)
    {
        if (commonTableExpressionBindingStateByName.TryGetValue(definition.Name, out var bindingState))
        {
            switch (bindingState)
            {
                case RuntimeCommonTableExpressionBindingState.Resolved:
                    return commonTableExpressionRowsetByName.GetValueOrDefault(definition.Name);
                case RuntimeCommonTableExpressionBindingState.Binding:
                    return commonTableExpressionRowsetByName.GetValueOrDefault(definition.Name);
                case RuntimeCommonTableExpressionBindingState.Failed:
                    return null;
            }
        }

        commonTableExpressionBindingStateByName[definition.Name] = RuntimeCommonTableExpressionBindingState.Binding;
        var expectedOutputColumnNames =
            definition.ColumnAliases.Count > 0
                ? definition.ColumnAliases
                : TryDeriveOutputColumnNamesFromQueryExpression(definition.QueryExpressionId);
        if (expectedOutputColumnNames is not null && expectedOutputColumnNames.Count > 0)
        {
            commonTableExpressionRowsetByName[definition.Name] = CreateCommonTableExpressionPlaceholder(definition, expectedOutputColumnNames);
        }

        var innerBinding = BindQueryExpression(
            definition.QueryExpressionId,
            $"{definition.QueryExpressionId}:output-rowset",
            $"{definition.Name}:inner",
            null,
            definition.Ordinal,
            [],
            null,
            expectedOutputColumnNames);

        if (innerBinding is null)
        {
            commonTableExpressionBindingStateByName[definition.Name] = RuntimeCommonTableExpressionBindingState.Failed;
            commonTableExpressionRowsetByName[definition.Name] = null;
            return null;
        }

        if (expectedOutputColumnNames is not null && expectedOutputColumnNames.Count != innerBinding.OutputRowset.Columns.Count)
        {
            issues.Add(new TransformBindingIssue(
                "CommonTableExpressionColumnAliasCountMismatch",
                $"CommonTableExpression '{definition.Name}' exposes {expectedOutputColumnNames.Count} expected output columns but its query produces {innerBinding.OutputRowset.Columns.Count} columns.",
                definition.Id));
            commonTableExpressionBindingStateByName[definition.Name] = RuntimeCommonTableExpressionBindingState.Failed;
            commonTableExpressionRowsetByName[definition.Name] = null;
            return null;
        }

        var columns = innerBinding.OutputRowset.Columns
            .Select((column, ordinal) => new RuntimeColumn(
                $"{definition.Id}:column:{ordinal + 1}",
                expectedOutputColumnNames is null || expectedOutputColumnNames.Count == 0 ? column.Name : expectedOutputColumnNames[ordinal],
                ordinal))
            .ToArray();

        var rowset = new RuntimeRowset(
            $"{definition.Id}:rowset",
            definition.Name,
            "CommonTableExpression",
            null,
            definition.Id,
            null,
            columns,
            [new RuntimeRowsetInput(0, "Input", innerBinding.OutputRowset)]);

        TrackRowset(rowset);
        commonTableExpressionBindingStateByName[definition.Name] = RuntimeCommonTableExpressionBindingState.Resolved;
        commonTableExpressionRowsetByName[definition.Name] = rowset;
        return rowset;
    }

    private RuntimeRowset CreateCommonTableExpressionPlaceholder(
        RuntimeCommonTableExpressionDefinition definition,
        IReadOnlyList<string> outputColumnNames)
    {
        return new RuntimeRowset(
            $"{definition.Id}:placeholder-rowset",
            $"{definition.Name}:placeholder",
            "CommonTableExpressionPlaceholder",
            null,
            definition.Id,
            null,
            outputColumnNames
                .Select((columnName, ordinal) => new RuntimeColumn(
                    $"{definition.Id}:placeholder-column:{ordinal + 1}",
                    columnName,
                    ordinal))
                .ToArray(),
            []);
    }

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

    private RuntimeTableReferenceBinding? BindSchemaObjectFunctionTableReference(
        TableReference tableReference,
        SchemaObjectFunctionTableReference functionTableReference,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources)
    {
        var columnAliases = navigator.GetTableReferenceColumnAliases(tableReference);
        if (columnAliases.Count == 0)
        {
            issues.Add(new TransformBindingIssue(
                "FunctionTableReferenceColumnAliasesRequired",
                $"Function table reference '{functionTableReference.Id}' does not expose column aliases, so its rowset shape is not yet sanctioned for binding.",
                tableReference.Id));
            return null;
        }

        var parameterScope = new BindingScope(inheritedVisibleTableSources);
        foreach (var parameter in navigator.GetSchemaObjectFunctionTableReferenceParameters(functionTableReference))
        {
            BindScalarExpression(parameter, parameterScope, null, null, false);
        }

        var functionNameParts = navigator.GetSchemaObjectFunctionTableReferenceNameParts(functionTableReference);
        var functionName = functionNameParts.Count == 0
            ? functionTableReference.Id
            : string.Join(".", functionNameParts);
        var exposedName = navigator.TryGetTableAlias(tableReference) ?? functionNameParts.LastOrDefault() ?? functionName;

        var columns = columnAliases
            .Select((columnName, ordinal) => new RuntimeColumn(
                $"{tableReference.Id}:column:{ordinal + 1}",
                columnName,
                ordinal))
            .ToArray();

        var rowset = new RuntimeRowset(
            $"{tableReference.Id}:rowset",
            functionName,
            "FunctionTableReference",
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
