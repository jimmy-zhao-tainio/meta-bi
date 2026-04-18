using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private readonly TransformScriptNavigator navigator;
    private readonly List<TransformBindingIssue> issues = [];
    private readonly List<RuntimeTableSource> boundTableSources = [];
    private readonly List<RuntimeColumnReference> boundColumnReferences = [];
    private readonly List<RuntimeRowset> boundRowsets = [];
    private readonly HashSet<string> activeTransformFunctionParameterNames = new(StringComparer.OrdinalIgnoreCase);
    private bool isInlineTableValuedFunction;
    private readonly Dictionary<string, RuntimeCommonTableExpressionDefinition> commonTableExpressionDefinitionsByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, RuntimeCommonTableExpressionBindingState> commonTableExpressionBindingStateByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, RuntimeRowset?> commonTableExpressionRowsetByName = new(StringComparer.OrdinalIgnoreCase);

    public TransformBindingSession(
        MetaTransformScriptModel model)
    {
        navigator = new TransformScriptNavigator(model);
    }

    public TransformBindingResult BindTransform(
        TransformScript transformScript)
    {
        ArgumentNullException.ThrowIfNull(transformScript);

        var scriptObjectKind = navigator.GetTransformScriptObjectKind(transformScript);
        isInlineTableValuedFunction = string.Equals(scriptObjectKind, "InlineTableValuedFunction", StringComparison.OrdinalIgnoreCase);
        activeTransformFunctionParameterNames.Clear();
        foreach (var functionParameterName in navigator.GetTransformScriptFunctionParameterNames(transformScript))
        {
            activeTransformFunctionParameterNames.Add(functionParameterName);
        }

        var selectStatement = navigator.TryGetSelectStatement(transformScript);
        if (selectStatement is null)
        {
            issues.Add(new TransformBindingIssue(
                "TransformScriptSelectStatementMissing",
                $"TransformScript '{transformScript.Name}' is missing its SelectStatement link.",
                transformScript.Id));

            return CreateResult(transformScript, null, null, null);
        }

        InitializeCommonTableExpressions(selectStatement);

        var topLevelQueryExpressionId = navigator.TryGetSelectStatementQueryExpressionId(selectStatement);
        if (string.IsNullOrWhiteSpace(topLevelQueryExpressionId))
        {
            issues.Add(new TransformBindingIssue(
                "TransformScriptQueryExpressionMissing",
                $"SelectStatement '{selectStatement.Id}' is missing its QueryExpression link.",
                selectStatement.Id));

            return CreateResult(transformScript, null, null, null);
        }

        var topLevelBinding = BindQueryExpression(
            topLevelQueryExpressionId,
            $"{topLevelQueryExpressionId}:output-rowset",
            "FinalOutput",
            "FinalOutput",
            int.MaxValue,
            [],
            null,
            null);

        return CreateResult(
            transformScript,
            topLevelBinding?.Scope,
            topLevelBinding?.InputRowset,
            topLevelBinding?.OutputRowset);
    }

    private TransformBindingResult CreateResult(
        TransformScript transformScript,
        BindingScope? topLevelScope,
        RuntimeRowset? topLevelInputRowset,
        RuntimeRowset? topLevelRowset)
    {
        return new TransformBindingResult(
            transformScript.Id,
            transformScript.Name,
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
            commonTableExpressionBindingStateByName[name] = RuntimeCommonTableExpressionBindingState.NotResolved;
            commonTableExpressionRowsetByName[name] = null;
        }
    }

    private readonly record struct CommonTableExpressionReferenceBindingResult(
        bool IsResolved,
        RuntimeTableReferenceBinding? Binding)
    {
        public static CommonTableExpressionReferenceBindingResult Unresolved => new(false, null);

        public static CommonTableExpressionReferenceBindingResult Resolved(RuntimeTableReferenceBinding? binding) =>
            new(true, binding);
    }

    private sealed record RuntimeCommonTableExpressionDefinition(
        string Id,
        string Name,
        string QueryExpressionId,
        IReadOnlyList<string> ColumnAliases,
        int Ordinal);

    private sealed record RuntimeGroupingContext(
        RuntimeRowset GroupedRowset,
        IReadOnlySet<string> GroupingKeySignatures);

    private enum RuntimeCommonTableExpressionBindingState
    {
        NotResolved,
        Binding,
        Resolved,
        Failed
    }

}
