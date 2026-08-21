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
    private readonly List<RuntimeMutationEffect> mutationEffects = [];
    private readonly Stack<IReadOnlySet<string>> orderByOutputAliasScopeStack = [];
    private readonly Stack<IReadOnlySet<string>> nonNullableColumnScopeStack = [];
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
        : this(
            new TransformScriptNavigator(model ?? throw new ArgumentNullException(nameof(model))),
            sourceSchemaResolver,
            targetSchemaResolver,
            executeSystemName,
            executeSystemDefaultSchemaName)
    {
    }

    internal TransformBindingSession(
        TransformScriptNavigator navigator,
        MetaSchemaTableResolver? sourceSchemaResolver,
        MetaSchemaTableResolver? targetSchemaResolver,
        string? executeSystemName,
        string? executeSystemDefaultSchemaName)
    {
        this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        this.sourceSchemaResolver = sourceSchemaResolver;
        this.targetSchemaResolver = targetSchemaResolver;
        this.executeSystemName = executeSystemName?.Trim() ?? string.Empty;
        this.executeSystemDefaultSchemaName = executeSystemDefaultSchemaName?.Trim() ?? string.Empty;
    }

    public TransformBindingResult BindTransform(
        TransformScript transformScript)
    {
        ArgumentNullException.ThrowIfNull(transformScript);

        mutationEffects.Clear();

        var scriptObjectKind = navigator.GetTransformScriptObjectKind(transformScript);
        if (string.Equals(scriptObjectKind, "ScalarFunction", StringComparison.OrdinalIgnoreCase))
        {
            return CreateResult(transformScript, null, null, null);
        }

        if (string.Equals(scriptObjectKind, "StoredProcedure", StringComparison.OrdinalIgnoreCase))
        {
            return BindStoredProcedureTransform(transformScript);
        }

        var statementKind = navigator.GetTransformScriptStatementKind(transformScript);
        isInlineTableValuedFunction = string.Equals(scriptObjectKind, "InlineTableValuedFunction", StringComparison.OrdinalIgnoreCase);
        activeTransformFunctionParameterNames.Clear();
        foreach (var functionParameterName in navigator.GetTransformScriptFunctionParameterNames(transformScript))
        {
            activeTransformFunctionParameterNames.Add(functionParameterName);
        }

        if (statementKind is not TransformScriptStatementKind.Select)
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
        TransformScriptStatementKind statementKind)
    {
        if (statementKind is TransformScriptStatementKind.Unsupported)
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
            case TransformScriptStatementKind.Insert:
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
            case TransformScriptStatementKind.Update:
                inputRowset = BindMutationFromClause(navigator.TryGetUpdateStatementFromClause(transformScript), visibleSources);
                break;
            case TransformScriptStatementKind.Delete:
                inputRowset = BindMutationFromClause(navigator.TryGetDeleteStatementFromClause(transformScript), visibleSources);
                break;
            case TransformScriptStatementKind.Merge:
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
        if (targetSchemaResolver is not null)
        {
            BindMutationSearchCondition(
                transformScript,
                statementKind,
                targetRowset,
                inputRowset,
                visibleSources);
            BuildMutationEffects(
                transformScript,
                statementKind,
                targetRowset,
                inputRowset,
                visibleSources);
        }

        return CreateResult(transformScript, scope, inputRowset, targetRowset);
    }

    private void BindMutationSearchCondition(
        TransformScript transformScript,
        TransformScriptStatementKind statementKind,
        RuntimeRowset targetRowset,
        RuntimeRowset? inputRowset,
        IReadOnlyList<RuntimeTableSource> visibleSources)
    {
        BooleanExpression? searchCondition = null;
        string? targetAlias = null;

        switch (statementKind)
        {
            case TransformScriptStatementKind.Update:
                searchCondition = navigator.TryGetUpdateStatementSearchCondition(transformScript);
                targetAlias = navigator.TryGetUpdateStatementTargetAlias(transformScript);
                break;

            case TransformScriptStatementKind.Delete:
                searchCondition = navigator.TryGetDeleteStatementSearchCondition(transformScript);
                break;

            case TransformScriptStatementKind.Merge:
                searchCondition = navigator.TryGetMergeStatementSearchCondition(transformScript);
                targetAlias = navigator.TryGetMergeStatementTargetAlias(transformScript);
                break;
        }

        if (searchCondition is null)
        {
            return;
        }

        var scope = CreateMutationValueScope(transformScript, targetRowset, visibleSources, targetAlias);
        BindBooleanExpression(searchCondition, scope, inputRowset, groupingContext: null);
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
                ordinal,
                column.DataType))
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
        TransformScriptStatementKind statementKind)
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
                        columns.Count,
                        CreateRuntimeColumnDataType(
                            field,
                            $"{targetResolution.Table.CanonicalSqlIdentifier}.{field.FieldName}")));
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

    private TransformBindingResult BindStoredProcedureTransform(TransformScript transformScript)
    {
        if (!ValidateStoredProcedureContract(transformScript))
        {
            return CreateResult(transformScript, null, null, null);
        }

        var inputRowsets = new List<RuntimeRowsetInput>();
        var ordinal = 0;

        foreach (var operation in navigator.GetStoredProcedureOperations(transformScript))
        {
            var operationKind = NormalizeStoredProcedureOperationKind(operation.OperationKind);
            if (operationKind is null)
            {
                issues.Add(new TransformBindingIssue(
                    "StoredProcedureOperationKindInvalid",
                    $"Stored procedure transform script '{transformScript.Name}' declares operation '{operation.Id}' with unsupported OperationKind '{operation.OperationKind}'.",
                    operation.Id));
                continue;
            }

            var sqlIdentifier = operation.SqlIdentifier?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(sqlIdentifier))
            {
                issues.Add(new TransformBindingIssue(
                    "StoredProcedureOperationSqlIdentifierMissing",
                    $"Stored procedure transform script '{transformScript.Name}' declares a {operationKind} operation with a blank SqlIdentifier.",
                    operation.Id));
                continue;
            }

            if (string.Equals(operationKind, "Call", StringComparison.Ordinal))
            {
                continue;
            }

            var derivationKind = string.Equals(operationKind, "Read", StringComparison.Ordinal)
                ? "Source"
                : "Target";
            var inputRole = $"StoredProcedure{operationKind}";
            var rowsetName = string.IsNullOrWhiteSpace(operation.AccessRole)
                ? $"{inputRole}:{operation.Ordinal}"
                : operation.AccessRole.Trim();
            var rowsetId = $"{transformScript.Id}:stored-procedure-operation:{ordinal + 1}";
            var columns = string.Equals(operationKind, "Read", StringComparison.Ordinal)
                ? ResolveSourceColumns(sqlIdentifier, rowsetId)
                : ResolveTargetColumns(sqlIdentifier, rowsetId, transformScript, operation.Id);
            var rowset = CreateDeclaredStoredProcedureRowset(
                rowsetId,
                rowsetName,
                derivationKind,
                inputRole,
                operation.Id,
                sqlIdentifier,
                columns);
            inputRowsets.Add(new RuntimeRowsetInput(ordinal, inputRole, rowset));
            ordinal++;
        }

        var resultRowsets = navigator.GetStoredProcedureResultRowsets(transformScript)
            .Select((item, index) => CreateDeclaredStoredProcedureResultRowset(transformScript, item, index))
            .ToArray();

        var topLevelRowset = resultRowsets.Length == 1
            ? resultRowsets[0]
            : null;
        var inputRowset = inputRowsets.Count == 0
            ? null
            : new RuntimeRowset(
                $"{transformScript.Id}:stored-procedure-declared-access-rowset",
                $"StoredProcedureDeclaredAccess:{transformScript.Name}",
                "DeclaredAccess",
                "StoredProcedureAccess",
                transformScript.Id,
                null,
                [],
                inputRowsets);
        if (inputRowset is not null)
        {
            TrackRowset(inputRowset);
        }

        return CreateResult(transformScript, null, inputRowset, topLevelRowset);
    }

    private bool ValidateStoredProcedureContract(TransformScript transformScript)
    {
        var contracts = navigator.GetStoredProcedureContracts(transformScript);
        if (contracts.Count == 1)
        {
            var resultRowsetCount = navigator.GetStoredProcedureResultRowsets(transformScript).Count;
            if (resultRowsetCount > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "StoredProcedureResultRowsetInvalid",
                    $"Stored procedure transform script '{transformScript.Name}' declares {resultRowsetCount} result rowsets. Stored procedure contracts support at most one result rowset.",
                    transformScript.Id));
                return false;
            }

            return true;
        }

        if (contracts.Count == 0)
        {
            issues.Add(new TransformBindingIssue(
                "StoredProcedureContractMissing",
                $"Stored procedure transform script '{transformScript.Name}' does not have a StoredProcedureContract row.",
                transformScript.Id));
            return false;
        }

        issues.Add(new TransformBindingIssue(
            "StoredProcedureContractInvalid",
            $"Stored procedure transform script '{transformScript.Name}' has multiple StoredProcedureContract rows.",
            transformScript.Id));
        return false;
    }

    private static string? NormalizeStoredProcedureOperationKind(string? operationKind)
    {
        if (string.IsNullOrWhiteSpace(operationKind))
        {
            return null;
        }

        return operationKind.Trim().ToLowerInvariant() switch
        {
            "read" => "Read",
            "append" => "Append",
            "replace" => "Replace",
            "reset" => "Reset",
            "mutation" => "Mutation",
            "call" => "Call",
            _ => null
        };
    }

    private RuntimeRowset CreateDeclaredStoredProcedureResultRowset(
        TransformScript transformScript,
        StoredProcedureResultRowsetItem rowset,
        int index)
    {
        var columns = navigator.GetStoredProcedureResultColumns(rowset)
            .Select((column, columnIndex) => new RuntimeColumn(
                $"{rowset.Id}:column:{columnIndex + 1}",
                column.Name,
                columnIndex))
            .ToArray();

        var result = new RuntimeRowset(
            $"{transformScript.Id}:stored-procedure-result-rowset:{index + 1}",
            string.IsNullOrWhiteSpace(rowset.Name) ? $"StoredProcedureResult:{index + 1}" : rowset.Name.Trim(),
            "Output",
            "StoredProcedureResult",
            rowset.Id,
            null,
            columns,
            []);
        TrackRowset(result);
        return result;
    }

    private RuntimeRowset CreateDeclaredStoredProcedureRowset(
        string id,
        string name,
        string derivationKind,
        string rowsetRole,
        string metaTransformScriptEntityId,
        string sqlIdentifier,
        IReadOnlyList<RuntimeColumn> columns)
    {
        var rowset = new RuntimeRowset(
            id,
            name,
            derivationKind,
            rowsetRole,
            metaTransformScriptEntityId,
            sqlIdentifier,
            columns,
            []);
        TrackRowset(rowset);
        return rowset;
    }

    private IReadOnlyList<RuntimeColumn> ResolveSourceColumns(
        string sqlIdentifier,
        string rowsetId)
    {
        var resolution = ResolveSourceSchemaIdentifier(sqlIdentifier);
        return resolution.IsResolved && resolution.Table is not null
            ? CreateSchemaColumns(rowsetId, resolution.Table.Fields)
            : [];
    }

    private IReadOnlyList<RuntimeColumn> ResolveTargetColumns(
        string sqlIdentifier,
        string rowsetId,
        TransformScript transformScript,
        string declarationId)
    {
        var resolver = targetSchemaResolver ?? sourceSchemaResolver;
        if (resolver is null)
        {
            return [];
        }

        var resolution = resolver.ResolveSqlIdentifier(sqlIdentifier);
        if (!resolution.IsResolved || resolution.Table is null)
        {
            issues.Add(new TransformBindingIssue(
                "StoredProcedureOperationTargetNotResolved",
                $"Stored procedure transform script '{transformScript.Name}' declares target operation '{sqlIdentifier}', but it was not found in the sanctioned target schema.",
                declarationId));
            return [];
        }

        return CreateSchemaColumns(rowsetId, resolution.Table.Fields);
    }

    private static IReadOnlyList<RuntimeColumn> CreateSchemaColumns(
        string columnScopeId,
        IReadOnlyList<ResolvedSchemaField> fields)
    {
        return fields
            .OrderBy(item => item.Ordinal)
            .ThenBy(item => item.FieldName, StringComparer.OrdinalIgnoreCase)
            .Select((field, ordinal) => new RuntimeColumn(
                $"{columnScopeId}:column:{ordinal + 1}",
                field.FieldName,
                ordinal,
                CreateRuntimeColumnDataType(field, field.FieldName)))
            .ToArray();
    }

    private static RuntimeColumnDataType CreateRuntimeColumnDataType(
        ResolvedSchemaField field,
        string displayName)
    {
        return new RuntimeColumnDataType(
            field.MetaDataTypeId,
            field.IsNullable,
            field.Length,
            field.Precision,
            field.Scale,
            displayName);
    }

    private void InitializeCommonTableExpressionsForMutation(TransformScript transformScript)
    {
        InitializeCommonTableExpressions(navigator.GetCommonTableExpressions(transformScript));
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
            issues)
        {
            MutationEffects = mutationEffects.ToArray()
        };
    }

    private void InitializeCommonTableExpressions(SelectStatement selectStatement)
    {
        InitializeCommonTableExpressions(navigator.GetCommonTableExpressions(selectStatement));
    }

    private void InitializeCommonTableExpressions(IReadOnlyList<CommonTableExpression> commonTableExpressions)
    {
        commonTableExpressionDefinitionsByName.Clear();
        commonTableExpressionBindingStateByName.Clear();
        commonTableExpressionRowsetByName.Clear();

        foreach (var item in commonTableExpressions.Select((cte, ordinal) => (Cte: cte, Ordinal: ordinal)))
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
