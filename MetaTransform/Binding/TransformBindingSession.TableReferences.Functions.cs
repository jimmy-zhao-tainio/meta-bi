using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private RuntimeTableReferenceBinding? BindXmlNodesTableReference(
        TableReference tableReference,
        XmlNodesTableReference xmlNodesTableReference,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources,
        RuntimeRowset? inheritedInputRowset)
    {
        var targetExpression = navigator.TryGetXmlNodesTableReferenceTargetExpression(xmlNodesTableReference);
        if (targetExpression is null)
        {
            issues.Add(new TransformBindingIssue(
                "XmlNodesTableReferenceTargetExpressionMissing",
                $"XML nodes table reference '{xmlNodesTableReference.Id}' does not expose its target expression.",
                tableReference.Id));
            return null;
        }

        var parameterScope = new BindingScope(inheritedVisibleTableSources);
        BindScalarExpression(targetExpression, parameterScope, inheritedInputRowset, null, false);

        var columnAliases = navigator.GetTableReferenceColumnAliases(tableReference);
        if (columnAliases.Count == 0)
        {
            issues.Add(new TransformBindingIssue(
                "XmlNodesTableReferenceColumnAliasesRequired",
                $"XML nodes table reference '{xmlNodesTableReference.Id}' does not expose column aliases, so its rowset shape is not yet sanctioned for binding.",
                tableReference.Id));
            return null;
        }

        var columns = columnAliases
            .Select((columnName, ordinal) => new RuntimeColumn(
                $"{tableReference.Id}:column:{ordinal + 1}",
                columnName,
                ordinal))
            .ToArray();

        var rowset = new RuntimeRowset(
            $"{tableReference.Id}:rowset",
            $"XMLNODES:{tableReference.Id}",
            "FunctionTableReference",
            null,
            tableReference.Id,
            null,
            columns,
            []);

        TrackRowset(rowset);

        var exposedName = navigator.TryGetTableAlias(tableReference) ?? "XmlNodes";
        var tableSource = new RuntimeTableSource(
            tableReference.Id,
            exposedName,
            string.Empty,
            rowset);

        TrackTableSource(tableSource);
        return new RuntimeTableReferenceBinding(rowset, [tableSource]);
    }

    private RuntimeTableReferenceBinding? BindFullTextTableReference(
        TableReference tableReference,
        FullTextTableReference fullTextTableReference,
        IReadOnlyList<RuntimeTableSource> inheritedVisibleTableSources)
    {
        var tableNameParts = navigator.GetFullTextTableReferenceTableNameParts(fullTextTableReference);
        if (tableNameParts.Count == 0)
        {
            issues.Add(new TransformBindingIssue(
                "FullTextTableReferenceTableNameMissing",
                $"Full-text table reference '{fullTextTableReference.Id}' does not expose a supported source table name.",
                tableReference.Id));
            return null;
        }

        foreach (var columnReference in navigator.GetFullTextTableReferenceColumns(fullTextTableReference))
        {
            if (navigator.GetColumnReferenceParts(columnReference).Count == 0)
            {
                issues.Add(new TransformBindingIssue(
                    "FullTextTableReferenceColumnMissingIdentifier",
                    $"Full-text table reference '{fullTextTableReference.Id}' includes a column argument without an identifier.",
                    columnReference.Id));
                return null;
            }
        }

        var parameterScope = new BindingScope(inheritedVisibleTableSources);
        var searchCondition = navigator.TryGetFullTextTableReferenceSearchCondition(fullTextTableReference);
        if (searchCondition is null)
        {
            issues.Add(new TransformBindingIssue(
                "FullTextTableReferenceSearchConditionMissing",
                $"Full-text table reference '{fullTextTableReference.Id}' does not expose its search condition expression.",
                tableReference.Id));
            return null;
        }

        BindScalarExpression(searchCondition, parameterScope, null, null, false);

        var functionName = TryResolveFullTextTableFunctionName(fullTextTableReference.FullTextFunctionType);
        if (functionName is null)
        {
            issues.Add(new TransformBindingIssue(
                "FullTextTableReferenceFunctionTypeUnsupported",
                $"Full-text table reference '{fullTextTableReference.Id}' has unsupported FullTextFunctionType '{fullTextTableReference.FullTextFunctionType}'.",
                tableReference.Id));
            return null;
        }

        var sourceSqlIdentifier = string.Join(".", tableNameParts);
        var rowset = new RuntimeRowset(
            $"{tableReference.Id}:rowset",
            $"{functionName}({sourceSqlIdentifier})",
            "FunctionTableReference",
            null,
            tableReference.Id,
            string.Empty,
            [
                new RuntimeColumn($"{tableReference.Id}:column:1", "KEY", 0),
                new RuntimeColumn($"{tableReference.Id}:column:2", "RANK", 1)
            ],
            []);

        TrackRowset(rowset);

        var exposedName = navigator.TryGetTableAlias(tableReference) ?? functionName;
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

    private static string? TryResolveFullTextTableFunctionName(string functionType)
    {
        return functionType.Trim() switch
        {
            "Contains" => "CONTAINSTABLE",
            "FreeText" => "FREETEXTTABLE",
            _ => null
        };
    }
}
