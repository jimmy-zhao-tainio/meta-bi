using System.Globalization;
using MetaSchema;
using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private readonly TransformScriptNavigator navigator;
    private readonly SourceSchemaIndex sourceSchemaIndex;
    private readonly List<TransformBindingIssue> issues = [];
    private readonly List<RuntimeBoundTableSource> boundTableSources = [];
    private readonly List<RuntimeBoundColumnReference> boundColumnReferences = [];
    private readonly List<RuntimeBoundRowset> boundRowsets = [];
    private readonly Dictionary<string, RuntimeCommonTableExpressionDefinition> commonTableExpressionDefinitionsByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, RuntimeCommonTableExpressionBindingState> commonTableExpressionBindingStateByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, RuntimeBoundRowset?> commonTableExpressionRowsetByName = new(StringComparer.OrdinalIgnoreCase);

    public TransformBindingSession(
        MetaTransformScriptModel model,
        MetaSchemaModel sourceSchema)
    {
        navigator = new TransformScriptNavigator(model);
        sourceSchemaIndex = new SourceSchemaIndex(sourceSchema);
    }

    public BoundTransform BindTransform(
        TransformScript transformScript,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentNullException.ThrowIfNull(transformScript);

        var activeLanguageProfileId = ResolveActiveLanguageProfile(transformScript, activeLanguageProfileIdOverride);
        if (string.IsNullOrWhiteSpace(activeLanguageProfileId))
        {
            issues.Add(new TransformBindingIssue(
                "ActiveLanguageProfileMissing",
                $"TransformScript '{transformScript.Name}' does not resolve an active language profile.",
                transformScript.Id));

            return CreateResult(transformScript, activeLanguageProfileId, null, null, null);
        }

        var selectStatement = navigator.TryGetSelectStatement(transformScript);
        if (selectStatement is null)
        {
            issues.Add(new TransformBindingIssue(
                "TransformScriptSelectStatementMissing",
                $"TransformScript '{transformScript.Name}' is missing its SelectStatement link.",
                transformScript.Id));

            return CreateResult(transformScript, activeLanguageProfileId, null, null, null);
        }

        InitializeCommonTableExpressions(selectStatement);

        var topLevelQueryExpressionId = navigator.TryGetSelectStatementQueryExpressionId(selectStatement);
        if (string.IsNullOrWhiteSpace(topLevelQueryExpressionId))
        {
            issues.Add(new TransformBindingIssue(
                "TransformScriptQueryExpressionMissing",
                $"SelectStatement '{selectStatement.Id}' is missing its QueryExpression link.",
                selectStatement.Id));

            return CreateResult(transformScript, activeLanguageProfileId, null, null, null);
        }

        var topLevelBinding = BindQueryExpression(
            topLevelQueryExpressionId,
            $"{topLevelQueryExpressionId}:output-rowset",
            transformScript.Name,
            "FinalOutput",
            int.MaxValue,
            [],
            null,
            null);

        return CreateResult(
            transformScript,
            activeLanguageProfileId,
            topLevelBinding?.Scope,
            topLevelBinding?.InputRowset,
            topLevelBinding?.OutputRowset);
    }

    private BoundTransform CreateResult(
        TransformScript transformScript,
        string activeLanguageProfileId,
        BoundScope? topLevelScope,
        RuntimeBoundRowset? topLevelInputRowset,
        RuntimeBoundRowset? topLevelRowset)
    {
        return new BoundTransform(
            transformScript.Id,
            transformScript.Name,
            activeLanguageProfileId,
            topLevelScope,
            topLevelInputRowset,
            topLevelRowset,
            boundTableSources,
            boundColumnReferences,
            boundRowsets,
            issues);
    }

    private void InitializeCommonTableExpressions(SelectStatement selectStatement)
    {
        commonTableExpressionDefinitionsByName.Clear();
        commonTableExpressionBindingStateByName.Clear();
        commonTableExpressionRowsetByName.Clear();

        foreach (var item in navigator.GetCommonTableExpressions(selectStatement).Select((cte, ordinal) => (Cte: cte, Ordinal: ordinal)))
        {
            var name = navigator.TryGetCommonTableExpressionName(item.Cte);
            var queryExpressionId = navigator.TryGetCommonTableExpressionQueryExpressionId(item.Cte);
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(queryExpressionId))
            {
                issues.Add(new TransformBindingIssue(
                    "CommonTableExpressionDefinitionIncomplete",
                    $"CommonTableExpression '{item.Cte.Id}' is missing its expression name or query expression.",
                    item.Cte.Id));
                continue;
            }

            var definition = new RuntimeCommonTableExpressionDefinition(
                item.Cte.Id,
                name,
                queryExpressionId,
                navigator.GetCommonTableExpressionColumnAliases(item.Cte),
                item.Ordinal);

            commonTableExpressionDefinitionsByName[name] = definition;
            commonTableExpressionBindingStateByName[name] = RuntimeCommonTableExpressionBindingState.NotBound;
            commonTableExpressionRowsetByName[name] = null;
        }
    }

    private static string ResolveActiveLanguageProfile(TransformScript transformScript, string? activeLanguageProfileIdOverride)
    {
        if (!string.IsNullOrWhiteSpace(activeLanguageProfileIdOverride))
        {
            return activeLanguageProfileIdOverride.Trim();
        }

        return string.IsNullOrWhiteSpace(transformScript.LanguageProfileId)
            ? string.Empty
            : transformScript.LanguageProfileId.Trim();
    }

    private readonly record struct CommonTableExpressionReferenceBindingResult(
        bool IsResolved,
        RuntimeBoundTableReferenceBinding? Binding)
    {
        public static CommonTableExpressionReferenceBindingResult Unresolved => new(false, null);

        public static CommonTableExpressionReferenceBindingResult Resolved(RuntimeBoundTableReferenceBinding? binding) =>
            new(true, binding);
    }

    private sealed record RuntimeCommonTableExpressionDefinition(
        string Id,
        string Name,
        string QueryExpressionId,
        IReadOnlyList<string> ColumnAliases,
        int Ordinal);

    private sealed record RuntimeGroupingContext(
        RuntimeBoundRowset GroupedRowset,
        IReadOnlySet<string> GroupingKeySignatures);

    private enum RuntimeCommonTableExpressionBindingState
    {
        NotBound,
        Binding,
        Bound,
        Failed
    }

    private sealed record SourceFieldDefinition(
        string FieldId,
        string FieldName,
        int Ordinal);

    private sealed record SourceTableDefinition(
        string TableId,
        string SchemaName,
        string TableName,
        IReadOnlyList<SourceFieldDefinition> Fields);

    private sealed class SourceSchemaIndex
    {
        private readonly IReadOnlyList<SourceTableDefinition> tables;

        public SourceSchemaIndex(MetaSchemaModel model)
        {
            var schemaNamesById = model.SchemaList.ToDictionary(item => item.Id, item => item.Name, StringComparer.Ordinal);
            var fieldRowsByTableId = model.FieldList
                .GroupBy(item => item.TableId, StringComparer.Ordinal)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .OrderBy(item => ParseOrdinal(item.Ordinal))
                        .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                        .Select(item => new SourceFieldDefinition(item.Id, item.Name, ParseOrdinal(item.Ordinal)))
                        .ToArray(),
                    StringComparer.Ordinal);

            tables = model.TableList
                .Select(item => new SourceTableDefinition(
                    item.Id,
                    schemaNamesById.GetValueOrDefault(item.SchemaId) ?? string.Empty,
                    item.Name,
                    fieldRowsByTableId.GetValueOrDefault(item.Id) ?? []))
                .ToArray();
        }

        public SourceTableDefinition? ResolveTable(
            IReadOnlyList<string> identifierParts,
            string syntaxId,
            List<TransformBindingIssue> issues)
        {
            if (identifierParts.Count == 1)
            {
                var matches = tables
                    .Where(item => string.Equals(item.TableName, identifierParts[0], StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (matches.Length == 0)
                {
                    issues.Add(new TransformBindingIssue(
                        "SourceTableNotFound",
                        $"Source table '{identifierParts[0]}' was not found in the sanctioned source schema.",
                        syntaxId));
                    return null;
                }

                if (matches.Length > 1)
                {
                    issues.Add(new TransformBindingIssue(
                        "SourceTableAmbiguous",
                        $"Source table '{identifierParts[0]}' resolves ambiguously across schemas.",
                        syntaxId));
                    return null;
                }

                return matches[0];
            }

            var schemaName = identifierParts[0];
            var tableName = identifierParts[1];
            var qualifiedMatches = tables
                .Where(item =>
                    string.Equals(item.SchemaName, schemaName, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(item.TableName, tableName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (qualifiedMatches.Length == 0)
            {
                issues.Add(new TransformBindingIssue(
                    "SourceTableNotFound",
                    $"Source table '{schemaName}.{tableName}' was not found in the sanctioned source schema.",
                    syntaxId));
                return null;
            }

            if (qualifiedMatches.Length > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "SourceTableAmbiguous",
                    $"Source table '{schemaName}.{tableName}' resolves ambiguously in the sanctioned source schema.",
                    syntaxId));
                return null;
            }

            return qualifiedMatches[0];
        }

        private static int ParseOrdinal(string ordinal)
        {
            return int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                ? value
                : int.MaxValue;
        }
    }
}
