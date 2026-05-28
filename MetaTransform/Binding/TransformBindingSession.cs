using MetaTransformScript;
using MetaSchema;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private readonly TransformScriptNavigator navigator;
    private readonly List<TransformBindingIssue> issues = [];
    private readonly List<RuntimeTableSource> boundTableSources = [];
    private readonly List<RuntimeColumnReference> boundColumnReferences = [];
    private readonly List<RuntimeRowset> boundRowsets = [];
    private readonly Stack<IReadOnlySet<string>> orderByOutputAliasScopeStack = [];
    private readonly HashSet<string> activeTransformFunctionParameterNames = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> boundScalarFunctionBodyScriptIds = new(StringComparer.Ordinal);
    private bool isInlineTableValuedFunction;
    private readonly Dictionary<string, RuntimeCommonTableExpressionDefinition> commonTableExpressionDefinitionsByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, RuntimeCommonTableExpressionBindingState> commonTableExpressionBindingStateByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, RuntimeRowset?> commonTableExpressionRowsetByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly MetaSchemaTableResolver? sourceSchemaResolver;
    private readonly MetaSchemaTableResolver? targetSchemaResolver;
    private readonly string executeSystemName;
    private readonly string executeSystemDefaultSchemaName;

    public TransformBindingSession(
        MetaTransformScriptModel model)
        : this(model, sourceSchema: null)
    {
    }

    public TransformBindingSession(
        MetaTransformScriptModel model,
        MetaSchemaModel? sourceSchema)
        : this(
            model,
            sourceSchema is null ? null : new MetaSchemaTableResolver(sourceSchema),
            targetSchemaResolver: sourceSchema is null ? null : new MetaSchemaTableResolver(sourceSchema),
            executeSystemName: string.Empty,
            executeSystemDefaultSchemaName: string.Empty)
    {
    }

    internal TransformBindingSession(
        MetaTransformScriptModel model,
        MetaSchemaTableResolver? sourceSchemaResolver,
        MetaSchemaTableResolver? targetSchemaResolver,
        string? executeSystemName,
        string? executeSystemDefaultSchemaName)
    {
        navigator = new TransformScriptNavigator(model);
        this.sourceSchemaResolver = sourceSchemaResolver;
        this.targetSchemaResolver = targetSchemaResolver;
        this.executeSystemName = executeSystemName?.Trim() ?? string.Empty;
        this.executeSystemDefaultSchemaName = executeSystemDefaultSchemaName?.Trim() ?? string.Empty;
    }

    public TransformBindingResult BindTransform(
        TransformScript transformScript)
    {
        ArgumentNullException.ThrowIfNull(transformScript);

        var scriptObjectKind = navigator.GetTransformScriptObjectKind(transformScript);
        if (string.Equals(scriptObjectKind, "ScalarFunction", StringComparison.OrdinalIgnoreCase))
        {
            return CreateResult(transformScript, null, null, null);
        }

        var statementKind = navigator.GetTransformScriptStatementKind(transformScript);
        isInlineTableValuedFunction = string.Equals(scriptObjectKind, "InlineTableValuedFunction", StringComparison.OrdinalIgnoreCase);
        activeTransformFunctionParameterNames.Clear();
        foreach (var functionParameterName in navigator.GetTransformScriptFunctionParameterNames(transformScript))
        {
            activeTransformFunctionParameterNames.Add(functionParameterName);
        }

        if (statementKind is not BoundStatementKind.Select)
        {
            return BindMutationTransform(transformScript, statementKind);
        }

        var selectStatement = navigator.TryGetSelectStatement(transformScript);
        if (selectStatement is null)
        {
            issues.Add(new TransformBindingIssue(
                "TransformScriptSelectStatementMissing",
                $"TransformScript '{transformScript.Name}' is not a SELECT-kind transform script.",
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
            ResolveExpectedOutputColumnNames(transformScript));

        return CreateResult(
            transformScript,
            topLevelBinding?.Scope,
            topLevelBinding?.InputRowset,
            topLevelBinding?.OutputRowset);
    }

    private TransformBindingResult BindMutationTransform(
        TransformScript transformScript,
        BoundStatementKind statementKind)
    {
        if (statementKind is BoundStatementKind.Unsupported)
        {
            issues.Add(new TransformBindingIssue(
                "TransformScriptStatementKindUnsupported",
                $"TransformScript '{transformScript.Name}' does not expose a supported statement kind for binding.",
                transformScript.Id));

            return CreateResult(transformScript, null, null, null);
        }

        InitializeCommonTableExpressionsForMutation(transformScript);

        var targetRowset = CreateMutationTargetRowset(transformScript, statementKind);
        RuntimeRowset? inputRowset = null;
        var visibleSources = new List<RuntimeTableSource>();

        switch (statementKind)
        {
            case BoundStatementKind.Insert:
                var queryExpressionId = navigator.TryGetInsertStatementQueryExpressionId(transformScript);
                if (!string.IsNullOrWhiteSpace(queryExpressionId))
                {
                    var queryBinding = BindQueryExpression(
                        queryExpressionId,
                        $"{queryExpressionId}:mutation-source-rowset",
                        $"{statementKind}:Source",
                        "MutationSource",
                        int.MaxValue,
                        [],
                        null,
                        expectedOutputColumnNames: null);
                    inputRowset = queryBinding?.OutputRowset;
                    if (queryBinding is not null)
                    {
                        visibleSources.AddRange(queryBinding.Scope.VisibleTableSources);
                    }
                }

                break;
            case BoundStatementKind.Update:
                inputRowset = BindMutationFromClause(navigator.TryGetUpdateStatementFromClause(transformScript), visibleSources);
                break;
            case BoundStatementKind.Delete:
                inputRowset = BindMutationFromClause(navigator.TryGetDeleteStatementFromClause(transformScript), visibleSources);
                break;
            case BoundStatementKind.Merge:
                var mergeSource = navigator.TryGetMergeStatementSourceTableReference(transformScript);
                if (mergeSource is not null)
                {
                    var sourceBinding = BindTableReference(mergeSource, int.MaxValue, [], null);
                    inputRowset = sourceBinding?.Rowset;
                    if (sourceBinding is not null)
                    {
                        visibleSources.AddRange(sourceBinding.VisibleTableSources);
                    }
                }

                break;
        }

        var scope = new BindingScope(visibleSources);
        return CreateResult(transformScript, scope, inputRowset, targetRowset);
    }

    private RuntimeRowset? BindMutationFromClause(
        FromClause? fromClause,
        ICollection<RuntimeTableSource> visibleSources)
    {
        if (fromClause is null)
        {
            return null;
        }

        var tableReferences = navigator.GetTableReferences(fromClause);
        RuntimeRowset? inputRowset = null;
        var inputs = new List<RuntimeRowsetInput>();
        foreach (var tableReference in tableReferences.Select((item, ordinal) => (Item: item, Ordinal: ordinal)))
        {
            var binding = BindTableReference(tableReference.Item, int.MaxValue, [], inputRowset);
            if (binding is null)
            {
                continue;
            }

            foreach (var visibleTableSource in binding.VisibleTableSources)
            {
                visibleSources.Add(visibleTableSource);
            }
            inputs.Add(new RuntimeRowsetInput(tableReference.Ordinal, "MutationFrom", binding.Rowset));
            inputRowset = binding.Rowset;
        }

        if (inputs.Count == 0)
        {
            return null;
        }

        var columns = inputs
            .SelectMany(item => item.Rowset.Columns)
            .Select((column, ordinal) => new RuntimeColumn(
                $"{fromClause.Id}:mutation-input-column:{ordinal + 1}",
                column.Name,
                ordinal))
            .ToArray();

        var rowset = new RuntimeRowset(
            $"{fromClause.Id}:mutation-input-rowset",
            $"MutationInput:{fromClause.Id}",
            "MutationInput",
            "MutationInput",
            fromClause.Id,
            null,
            columns,
            inputs);
        TrackRowset(rowset);
        return rowset;
    }

    private RuntimeRowset CreateMutationTargetRowset(
        TransformScript transformScript,
        BoundStatementKind statementKind)
    {
        var targetSqlIdentifier = navigator.TryGetMutationTargetSqlIdentifier(transformScript);
        if (string.IsNullOrWhiteSpace(targetSqlIdentifier))
        {
            issues.Add(new TransformBindingIssue(
                "MutationTargetMissing",
                $"TransformScript '{transformScript.Name}' is {statementKind}-kind but does not expose a mutation target.",
                transformScript.Id));
            targetSqlIdentifier = transformScript.Name;
        }

        var columns = new List<RuntimeColumn>();
        var resolver = targetSchemaResolver ?? sourceSchemaResolver;
        if (resolver is not null)
        {
            var targetResolution = resolver.ResolveSqlIdentifier(targetSqlIdentifier);
            if (targetResolution.IsResolved)
            {
                foreach (var field in targetResolution.Table!.Fields
                             .OrderBy(item => item.Ordinal)
                             .ThenBy(item => item.FieldName, StringComparer.OrdinalIgnoreCase))
                {
                    columns.Add(new RuntimeColumn(
                        $"{transformScript.Id}:mutation-target-column:{columns.Count + 1}",
                        field.FieldName,
                        columns.Count));
                }
            }
        }

        var rowset = new RuntimeRowset(
            $"{transformScript.Id}:mutation-target-rowset",
            targetSqlIdentifier,
            "Target",
            "MutationTarget",
            transformScript.Id,
            targetSqlIdentifier,
            columns,
            []);
        TrackRowset(rowset);
        return rowset;
    }

    private void InitializeCommonTableExpressionsForMutation(TransformScript transformScript)
    {
        var selectStatement = navigator.TryGetSelectStatement(transformScript);
        if (selectStatement is not null)
        {
            InitializeCommonTableExpressions(selectStatement);
        }
        else
        {
            commonTableExpressionDefinitionsByName.Clear();
            commonTableExpressionBindingStateByName.Clear();
            commonTableExpressionRowsetByName.Clear();
        }
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

    private IReadOnlyList<string>? ResolveExpectedOutputColumnNames(TransformScript transformScript)
    {
        if (sourceSchemaResolver is null)
        {
            return null;
        }

        var targetSqlIdentifier = navigator.TryGetTransformScriptTargetSqlIdentifier(transformScript)?.Trim();
        if (string.IsNullOrWhiteSpace(targetSqlIdentifier))
        {
            return null;
        }

        var targetResolution = (targetSchemaResolver ?? sourceSchemaResolver).ResolveSqlIdentifier(targetSqlIdentifier);
        if (!targetResolution.IsResolved || targetResolution.Table is null)
        {
            return null;
        }

        var expectedColumns = targetResolution.Table.Fields
            .Where(static item => !item.IsIdentity)
            .OrderBy(static item => item.Ordinal)
            .Select(static item => item.FieldName)
            .ToArray();

        return expectedColumns.Length == 0
            ? null
            : expectedColumns;
    }

    private bool IsOrderByOutputAliasReference(string identifier)
    {
        if (orderByOutputAliasScopeStack.Count == 0)
        {
            return false;
        }

        return orderByOutputAliasScopeStack.Peek().Contains(identifier);
    }

    private SchemaTableResolutionResult ResolveSourceSchemaIdentifier(string sqlIdentifier)
    {
        if (sourceSchemaResolver is null)
        {
            return new SchemaTableResolutionResult(
                [],
                sqlIdentifier,
                null,
                SchemaTableResolutionFailureKind.NotFound);
        }

        if (string.IsNullOrWhiteSpace(executeSystemName))
        {
            return sourceSchemaResolver.ResolveSqlIdentifier(sqlIdentifier);
        }

        var expanded = SourceSqlIdentifierExpansion.Expand(
            sqlIdentifier,
            executeSystemName,
            executeSystemDefaultSchemaName);
        if (!expanded.IsSuccess)
        {
            issues.Add(new TransformBindingIssue(
                expanded.FailureKind switch
                {
                    SourceSqlIdentifierExpansionFailureKind.MissingIdentifier => "SourceSchemaIdentifierMissing",
                    SourceSqlIdentifierExpansionFailureKind.MissingExecuteSystem => "SourceSchemaExecuteSystemMissing",
                    SourceSqlIdentifierExpansionFailureKind.MissingDefaultSchemaName => "SourceSchemaExecuteSystemDefaultSchemaNameMissing",
                    SourceSqlIdentifierExpansionFailureKind.UnsupportedIdentifierShape => "SourceSchemaIdentifierShapeUnsupported",
                    _ => "SourceSchemaResolutionFailed"
                },
                expanded.FailureKind switch
                {
                    SourceSqlIdentifierExpansionFailureKind.MissingIdentifier =>
                        $"Source identifier '{sqlIdentifier}' is blank and cannot be resolved.",
                    SourceSqlIdentifierExpansionFailureKind.MissingExecuteSystem =>
                        $"Source identifier '{sqlIdentifier}' requires execute-system context.",
                    SourceSqlIdentifierExpansionFailureKind.MissingDefaultSchemaName =>
                        $"Source identifier '{sqlIdentifier}' requires execute-system-default-schema-name because it is one-part.",
                    SourceSqlIdentifierExpansionFailureKind.UnsupportedIdentifierShape =>
                        $"Source identifier '{sqlIdentifier}' uses an unsupported identifier shape.",
                    _ =>
                        $"Source identifier '{sqlIdentifier}' could not be resolved."
                }));

            return new SchemaTableResolutionResult(
                [],
                sqlIdentifier,
                null,
                SchemaTableResolutionFailureKind.UnsupportedIdentifierShape);
        }

        return sourceSchemaResolver.ResolveIdentifierParts(expanded.ExpandedIdentifierParts);
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
        IReadOnlySet<string> GroupingKeySignatures,
        IReadOnlySet<string> GroupingKeyColumnIdentities);

    private enum RuntimeCommonTableExpressionBindingState
    {
        NotResolved,
        Binding,
        Resolved,
        Failed
    }

}
