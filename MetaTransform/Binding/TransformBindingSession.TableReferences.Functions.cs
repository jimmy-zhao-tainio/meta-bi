using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
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

    private RuntimeTableReferenceBinding? BindGlobalFunctionTableReference(
        TableReference tableReference,
        GlobalFunctionTableReference globalFunctionTableReference,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources)
    {
        var functionName = navigator.TryGetGlobalFunctionTableReferenceName(globalFunctionTableReference);
        if (string.IsNullOrWhiteSpace(functionName))
        {
            issues.Add(new TransformBindingIssue(
                "GlobalFunctionTableReferenceNameMissing",
                $"Global function table reference '{globalFunctionTableReference.Id}' does not expose a function name.",
                tableReference.Id));
            return null;
        }

        var parameterScope = new BindingScope(inheritedVisibleTableSources);
        foreach (var parameter in navigator.GetGlobalFunctionTableReferenceParameters(globalFunctionTableReference))
        {
            BindScalarExpression(parameter, parameterScope, null, null, false);
        }

        var inferredColumnNames = TryInferGlobalFunctionOutputColumns(functionName);
        if (inferredColumnNames.Count == 0)
        {
            issues.Add(new TransformBindingIssue(
                "GlobalFunctionTableReferenceOutputShapeNotSupported",
                $"Global function table reference '{functionName}' does not yet have a sanctioned output rowset shape in binding.",
                tableReference.Id));
            return null;
        }

        var columns = inferredColumnNames
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

        var exposedName = navigator.TryGetTableAlias(tableReference) ?? functionName;
        var tableSource = new RuntimeTableSource(
            tableReference.Id,
            exposedName,
            functionName,
            rowset);

        TrackTableSource(tableSource);
        return new RuntimeTableReferenceBinding(rowset, [tableSource]);
    }

    private static IReadOnlyList<string> TryInferGlobalFunctionOutputColumns(string functionName)
    {
        return functionName.Trim().ToUpperInvariant() switch
        {
            "OPENJSON" => ["key", "value", "type"],
            "GENERATE_SERIES" => ["value"],
            "STRING_SPLIT" => ["value"],
            _ => []
        };
    }
}
