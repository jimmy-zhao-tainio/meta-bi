using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private RuntimeTableReferenceBinding? BindNamedTableReference(
        TableReference tableReference,
        NamedTableReference namedTableReference,
        int visibleCommonTableExpressionOrdinal,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources)
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

        var tableSampleNumber = navigator.TryGetNamedTableReferenceTableSampleNumber(namedTableReference);
        var tableSampleRepeatSeed = navigator.TryGetNamedTableReferenceTableSampleRepeatSeed(namedTableReference);
        if (tableSampleNumber is not null || tableSampleRepeatSeed is not null)
        {
            var tableSampleScope = new BindingScope(inheritedVisibleTableSources);
            if (tableSampleNumber is not null)
            {
                BindScalarExpression(tableSampleNumber, tableSampleScope, null, groupingContext: null, withinAggregate: false);
            }

            if (tableSampleRepeatSeed is not null)
            {
                BindScalarExpression(tableSampleRepeatSeed, tableSampleScope, null, groupingContext: null, withinAggregate: false);
            }
        }

        var sqlIdentifier = string.Join(".", identifierParts);
        var exposedName = navigator.TryGetTableAlias(tableReference) ?? identifierParts.Last();
        var columns = new List<RuntimeColumn>();
        if (sourceSchemaResolver is not null)
        {
            var sourceResolution = ResolveSourceSchemaIdentifier(sqlIdentifier);
            if (sourceResolution.IsResolved)
            {
                foreach (var field in sourceResolution.Table!.Fields
                             .OrderBy(item => item.Ordinal)
                             .ThenBy(item => item.FieldName, StringComparer.OrdinalIgnoreCase))
                {
                    columns.Add(new RuntimeColumn(
                        $"{tableReference.Id}:source-column:{columns.Count + 1}",
                        field.FieldName,
                        columns.Count));
                }
            }
        }

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
}
