using System.Globalization;
using System.Text;
using MetaTransform.Binding;
using MetaTransformBinding;
using MetaTransformScript;
using MP = MetaPipeline;
using MO = MetaOrchestration;

namespace MetaOrchestration.Core;

public sealed class MetaOrchestrationAnalysisService
{
    private const string TaskKindTransformExecution = "TransformExecution";
    private const string TaskKindExecutable = "Executable";

    private readonly TransformScriptStatementKindService statementKindService = new();

    public OrchestrationAnalysisResult Analyze(OrchestrationAnalysisRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PlanName);

        var pipelineWorkspacePath = Path.GetFullPath(request.PipelineWorkspacePath);
        var pipelineModel = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MP.MetaPipelineModel>(pipelineWorkspacePath, searchUpward: false);

        return AnalyzeProfiles(
            request.PlanName,
            request.Description,
            CreateProfiles(pipelineModel));
    }

    public OrchestrationAnalysisResult AnalyzeProfiles(
        string planName,
        string? description,
        IReadOnlyList<PipelineDependencyProfile> profiles)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(planName);
        ArgumentNullException.ThrowIfNull(profiles);

        var graph = AnalyzeGraph(profiles);

        return new OrchestrationAnalysisResult(
            planName.Trim(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            profiles,
            graph.TaskObjectEffects,
            graph.TaskDependencies,
            graph.PipelineDependencies,
            graph.Issues,
            graph.DagStatus,
            graph.DeterminismStatus,
            graph.SynchronizationStatus);
    }

    public MO.MetaOrchestrationModel AnalyzeToModel(OrchestrationAnalysisRequest request)
    {
        return CreateModel(Analyze(request), Path.GetFullPath(request.PipelineWorkspacePath));
    }

    public MO.MetaOrchestrationModel CreateModel(
        OrchestrationAnalysisResult result,
        string pipelineWorkspacePath)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentException.ThrowIfNullOrWhiteSpace(pipelineWorkspacePath);

        return BuildModel(result, Path.GetFullPath(pipelineWorkspacePath));
    }

    private IReadOnlyList<PipelineDependencyProfile> CreateProfiles(
        MP.MetaPipelineModel pipelineModel)
    {
        var workspaceCache = new TransformWorkspaceProfileCache(statementKindService);

        return pipelineModel.PipelineList
            .OrderBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.Id, StringComparer.Ordinal)
            .Select(pipeline => CreatePipelineProfile(
                pipelineModel,
                pipeline,
                workspaceCache))
            .ToArray();
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<StoredProcedureContractOperation>> BuildStoredProcedureOperationsByScriptId(
        MetaTransformScriptModel transformModel)
    {
        var contractsByStoredProcedureId = transformModel.StoredProcedureContractList
            .GroupBy(static item => item.ScriptObjectStoredProcedure.Id, StringComparer.Ordinal)
            .Where(static group => group.Count() == 1)
            .ToDictionary(static group => group.Key, static group => group.Single(), StringComparer.Ordinal);
        var operationsByContractId = transformModel.StoredProcedureContractOperationList
            .GroupBy(static item => item.StoredProcedureContract.Id, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => (IReadOnlyList<StoredProcedureContractOperation>)group
                    .OrderBy(static item => ParseOrdinalOrMax(item.Ordinal))
                    .ThenBy(static item => item.OperationKind, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(static item => item.SqlIdentifier, StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                StringComparer.Ordinal);

        return transformModel.ScriptObjectStoredProcedureList
            .Where(item => contractsByStoredProcedureId.ContainsKey(item.Id))
            .ToDictionary(
                static item => item.TransformScript.Id,
                item => operationsByContractId.GetValueOrDefault(contractsByStoredProcedureId[item.Id].Id) ?? [],
                StringComparer.Ordinal);
    }

    private static PipelineDependencyProfile CreatePipelineProfile(
        MP.MetaPipelineModel pipelineModel,
        MP.Pipeline pipeline,
        TransformWorkspaceProfileCache workspaceCache)
    {
        var taskProfiles = new List<PipelineTaskAccessProfile>();
        var issues = new List<PipelineDependencyProfileIssue>();
        var taskDetails = ResolvePipelineTasksInDependencyOrder(pipelineModel, pipeline)
            .Select(task => new PipelineTaskDetail(
                task,
                ResolveTransformExecutionTask(pipelineModel, task),
                ResolveExecutableTask(pipelineModel, task),
                Ordinal: 0))
            .Where(static item => item.TransformExecution is not null || item.Executable is not null)
            .Select((item, index) => item with { Ordinal = index + 1 })
            .ToArray();

        foreach (var task in taskDetails)
        {
            if (task.Executable is not null && task.TransformExecution is null)
            {
                taskProfiles.Add(CreateExecutableTaskProfile(task.Executable, task.Ordinal));
                continue;
            }

            var taskProfile = CreateTaskProfile(
                pipelineModel,
                pipeline,
                task.TransformExecution
                ?? throw new InvalidOperationException(
                    $"Pipeline '{pipeline.Name}' task '{task.PipelineTask.Name}' has no transform execution detail."),
                task.Ordinal,
                workspaceCache);
            taskProfiles.Add(taskProfile.Profile);
            issues.AddRange(taskProfile.Issues);
        }

        var pipelineAccesses = taskProfiles
            .SelectMany(item => item.ObjectAccesses.Select(access => new { Task = item, Access = access }))
            .GroupBy(item => item.Access.ObjectKey, StringComparer.OrdinalIgnoreCase)
            .Select(group => CreatePipelineObjectAccess(group.Select(item => (item.Task, item.Access)).ToArray()))
            .OrderBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.SqlIdentifier, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new PipelineDependencyProfile(
            pipeline.Id,
            pipeline.Name,
            taskProfiles,
            pipelineAccesses,
            issues);
    }

    private static MP.TransformExecutionTask? ResolveTransformExecutionTask(
        MP.MetaPipelineModel pipelineModel,
        MP.PipelineTask pipelineTask)
    {
        var matches = pipelineModel.TransformExecutionTaskList
            .Where(item => string.Equals(item.PipelineTask.Id, pipelineTask.Id, StringComparison.Ordinal))
            .ToArray();

        return matches.Length switch
        {
            0 => null,
            1 => matches[0],
            _ => throw new InvalidOperationException(
                $"Pipeline task '{pipelineTask.Name}' has multiple TransformExecutionTask detail rows."),
        };
    }

    private static MP.ExecutableTask? ResolveExecutableTask(
        MP.MetaPipelineModel pipelineModel,
        MP.PipelineTask pipelineTask)
    {
        var matches = pipelineModel.ExecutableTaskList
            .Where(item => string.Equals(item.PipelineTask.Id, pipelineTask.Id, StringComparison.Ordinal))
            .ToArray();

        return matches.Length switch
        {
            0 => null,
            1 => matches[0],
            _ => throw new InvalidOperationException(
                $"Pipeline task '{pipelineTask.Name}' has multiple ExecutableTask detail rows."),
        };
    }

    private static PipelineTaskAccessProfile CreateExecutableTaskProfile(
        MP.ExecutableTask executable,
        int ordinal)
    {
        return new PipelineTaskAccessProfile(
            executable.PipelineTask.Id,
            executable.PipelineTask.Name,
            TaskKindExecutable,
            ordinal,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            TaskKindExecutable,
            []);
    }

    private static TaskProfileResult CreateTaskProfile(
        MP.MetaPipelineModel pipelineModel,
        MP.Pipeline pipeline,
        MP.TransformExecutionTask execution,
        int ordinal,
        TransformWorkspaceProfileCache workspaceCache)
    {
        var accesses = new List<PipelineObjectAccessProfile>();
        var issues = new List<PipelineDependencyProfileIssue>();
        var transformScriptName = execution.TransformScriptId;
        var statementKind = BoundStatementKind.Unsupported;
        var functionParameterCount = 0;
        TransformWorkspaceProfileContext? workspaceContext = null;
        TransformBinding? resolvedBinding = null;

        if (string.IsNullOrWhiteSpace(execution.TransformWorkspacePath) ||
            string.IsNullOrWhiteSpace(execution.BindingWorkspacePath))
        {
            issues.Add(CreateIssue(
                OrchestrationIssueCode.MissingScriptOrBinding,
                OrchestrationIssueDomain.ProfileResolution,
                "Error",
                blocksDag: true,
                blocksAutomaticRunPlanning: true,
                $"Pipeline '{pipeline.Name}' task '{execution.PipelineTask.Name}' is missing TransformWorkspacePath or BindingWorkspacePath.",
                null,
                [pipeline.Id]));
        }
        else
        {
            try
            {
                workspaceContext = workspaceCache.Get(
                    execution.TransformWorkspacePath,
                    execution.BindingWorkspacePath);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException)
            {
                issues.Add(CreateIssue(
                    OrchestrationIssueCode.MissingScriptOrBinding,
                    OrchestrationIssueDomain.ProfileResolution,
                    "Error",
                    blocksDag: true,
                    blocksAutomaticRunPlanning: true,
                    $"Pipeline '{pipeline.Name}' task '{execution.PipelineTask.Name}' could not load its transform/binding workspace context. {ex.Message}",
                    null,
                    [pipeline.Id]));
            }
        }

        if (workspaceContext is not null &&
            workspaceContext.TransformScriptsById.TryGetValue(execution.TransformScriptId, out var transformScript))
        {
            transformScriptName = transformScript.Name;
            workspaceContext.StatementKindsByScriptId.TryGetValue(transformScript.Id, out statementKind);
            workspaceContext.FunctionParameterCountsByScriptId.TryGetValue(transformScript.Id, out functionParameterCount);
        }
        else if (workspaceContext is not null)
        {
            issues.Add(CreateIssue(
                OrchestrationIssueCode.MissingScriptOrBinding,
                OrchestrationIssueDomain.ProfileResolution,
                "Error",
                blocksDag: true,
                blocksAutomaticRunPlanning: true,
                $"Pipeline '{pipeline.Name}' task '{execution.PipelineTask.Name}' references transform script id '{execution.TransformScriptId}', but it was not found in the transform workspace.",
                null,
                [pipeline.Id]));
        }

        var isScalarFunctionTask = statementKind is BoundStatementKind.ScalarFunction;
        var isParameterizedFunctionTask = !isScalarFunctionTask && functionParameterCount > 0;
        var canContributeObjectAccesses = !isScalarFunctionTask && !isParameterizedFunctionTask;

        if (workspaceContext is not null &&
            workspaceContext.BindingsById.TryGetValue(execution.TransformBindingId, out var binding))
        {
            resolvedBinding = binding;
            if (!string.Equals(binding.MetaTransformScriptTransformScriptId, execution.TransformScriptId, StringComparison.Ordinal))
            {
                issues.Add(CreateIssue(
                    OrchestrationIssueCode.MissingScriptOrBinding,
                    OrchestrationIssueDomain.ProfileResolution,
                    "Error",
                    blocksDag: true,
                    blocksAutomaticRunPlanning: true,
                    $"Pipeline '{pipeline.Name}' task '{execution.PipelineTask.Name}' references binding '{execution.TransformBindingId}', but that binding belongs to transform script id '{binding.MetaTransformScriptTransformScriptId}'.",
                    null,
                    [pipeline.Id]));
            }

            if (canContributeObjectAccesses)
            {
                if (statementKind is BoundStatementKind.StoredProcedure)
                {
                    foreach (var operation in workspaceContext.StoredProcedureOperationsByScriptId.GetValueOrDefault(execution.TransformScriptId) ?? [])
                    {
                        var access = TryCreateStoredProcedureOperationAccess(operation);
                        if (access is not null)
                        {
                            accesses.Add(access);
                        }
                    }
                }
                else
                {
                    foreach (var source in ResolveSourceSqlIdentifiers(workspaceContext.BindingModel, binding))
                    {
                        accesses.Add(CreateAccess(source, OrchestrationObjectAccessKind.Read, "Source", "Bound source rowset", accesses.Count));
                    }

                if (IsMutationStatementKind(statementKind))
                {
                    foreach (var target in ResolveTargetSqlIdentifiers(workspaceContext.BindingModel, binding))
                    {
                        accesses.Add(CreateAccess(target, ResolveMutationTargetAccessKind(statementKind), "Target", $"Bound {statementKind} target", accesses.Count));
                    }
                    }
                }
            }
        }
        else if (workspaceContext is not null)
        {
            issues.Add(CreateIssue(
                OrchestrationIssueCode.MissingScriptOrBinding,
                OrchestrationIssueDomain.ProfileResolution,
                "Error",
                blocksDag: true,
                blocksAutomaticRunPlanning: true,
                $"Pipeline '{pipeline.Name}' task '{execution.PipelineTask.Name}' references binding id '{execution.TransformBindingId}', but it was not found in the binding workspace.",
                null,
                [pipeline.Id]));
        }

        if (isScalarFunctionTask)
        {
            issues.Add(CreateIssue(
                OrchestrationIssueCode.NonExecutableTransformScript,
                OrchestrationIssueDomain.ProfileResolution,
                "Error",
                blocksDag: true,
                blocksAutomaticRunPlanning: true,
                $"Pipeline '{pipeline.Name}' task '{execution.PipelineTask.Name}' references scalar function transform script '{transformScriptName}'. Scalar function definitions are dependency-free helper objects and cannot be scheduled as pipeline transform tasks.",
                null,
                [pipeline.Id]));
        }
        else if (isParameterizedFunctionTask)
        {
            issues.Add(CreateIssue(
                OrchestrationIssueCode.NonExecutableTransformScript,
                OrchestrationIssueDomain.ProfileResolution,
                "Error",
                blocksDag: true,
                blocksAutomaticRunPlanning: true,
                $"Pipeline '{pipeline.Name}' task '{execution.PipelineTask.Name}' references parameterized transform script '{transformScriptName}'. Parameterized function definitions are query helper objects and cannot be scheduled as pipeline transform tasks.",
                null,
                [pipeline.Id]));
        }

        if (statementKind is BoundStatementKind.Unsupported)
        {
            issues.Add(CreateIssue(
                OrchestrationIssueCode.MissingScriptOrBinding,
                OrchestrationIssueDomain.ProfileResolution,
                "Error",
                blocksDag: true,
                blocksAutomaticRunPlanning: true,
                $"Pipeline '{pipeline.Name}' task '{execution.PipelineTask.Name}' references transform script '{transformScriptName}', but its statement kind is unsupported for binding-driven orchestration.",
                null,
                [pipeline.Id]));
        }

        if (canContributeObjectAccesses)
        {
            var insertRowsOrdinal = statementKind is BoundStatementKind.StoredProcedure
                ? int.MaxValue
                : accesses.Count;
            foreach (var target in QualifyTargetSqlIdentifiersFromBindingValidation(
                workspaceContext?.BindingModel,
                resolvedBinding,
                ResolveInsertRowsTargets(pipelineModel, execution.PipelineTask)))
            {
                accesses.Add(CreateAccess(target, OrchestrationObjectAccessKind.Write, "InsertRowsTarget", "Row-producing InsertRows target write", insertRowsOrdinal));
            }
        }

        var profile = new PipelineTaskAccessProfile(
            execution.PipelineTask.Id,
            execution.PipelineTask.Name,
            TaskKindTransformExecution,
            ordinal,
            execution.TransformScriptId,
            transformScriptName,
            execution.TransformBindingId,
            execution.TransformWorkspacePath,
            execution.BindingWorkspacePath,
            statementKind.ToString(),
            statementKind is BoundStatementKind.StoredProcedure
                ? accesses
                    .OrderBy(static item => item.Ordinal)
                    .ThenBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase)
                    .ToArray()
                : accesses
                    .GroupBy(access => $"{access.ObjectKey}|{access.AccessKind}|{access.AccessRole}", StringComparer.OrdinalIgnoreCase)
                    .Select(static group => group.First())
                    .OrderBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(static item => item.AccessKind.ToString(), StringComparer.Ordinal)
                    .ThenBy(static item => item.AccessRole, StringComparer.Ordinal)
                    .ToArray());

        return new TaskProfileResult(profile, issues);
    }

    private static bool IsMutationStatementKind(BoundStatementKind statementKind) =>
        statementKind is BoundStatementKind.Insert
            or BoundStatementKind.Update
            or BoundStatementKind.Delete
            or BoundStatementKind.Truncate
            or BoundStatementKind.Merge;

    private static OrchestrationObjectAccessKind ResolveMutationTargetAccessKind(BoundStatementKind statementKind) =>
        statementKind switch
        {
            BoundStatementKind.Insert => OrchestrationObjectAccessKind.Write,
            BoundStatementKind.Update => OrchestrationObjectAccessKind.ReadWrite,
            BoundStatementKind.Delete => OrchestrationObjectAccessKind.ReadWrite,
            BoundStatementKind.Truncate => OrchestrationObjectAccessKind.ResetWrite,
            BoundStatementKind.Merge => OrchestrationObjectAccessKind.ReadWrite,
            _ => throw new ArgumentOutOfRangeException(nameof(statementKind), statementKind, "Statement kind is not a mutation statement kind.")
        };

    private static PipelineObjectAccessProfile? TryCreateStoredProcedureOperationAccess(
        StoredProcedureContractOperation operation)
    {
        var operationKind = NormalizeStoredProcedureOperationKind(operation.OperationKind);
        if (operationKind is null ||
            string.Equals(operationKind, "Call", StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(operation.SqlIdentifier))
        {
            return null;
        }

        var accessKind = operationKind switch
        {
            "Read" => OrchestrationObjectAccessKind.Read,
            "Append" => OrchestrationObjectAccessKind.Write,
            "Replace" => OrchestrationObjectAccessKind.Write,
            "Reset" => OrchestrationObjectAccessKind.ResetWrite,
            "Mutation" => OrchestrationObjectAccessKind.ReadWrite,
            _ => OrchestrationObjectAccessKind.Read
        };
        var accessRole = string.IsNullOrWhiteSpace(operation.AccessRole)
            ? $"StoredProcedure{operationKind}"
            : operation.AccessRole.Trim();
        var reason = $"Declared stored procedure {operationKind} operation";
        if (!string.IsNullOrWhiteSpace(operation.Notes))
        {
            reason = $"{reason}: {operation.Notes.Trim()}";
        }

        return CreateAccess(
            operation.SqlIdentifier,
            accessKind,
            accessRole,
            reason,
            ParseOrdinalOrMax(operation.Ordinal),
            operationKind);
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

    private static IReadOnlyList<string> ResolveSourceSqlIdentifiers(
        MetaTransformBindingModel bindingModel,
        TransformBinding binding)
    {
        return bindingModel.RowsetList
            .Where(item =>
                string.Equals(item.TransformBinding.Id, binding.Id, StringComparison.Ordinal) &&
                string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(item.SqlIdentifier))
            .Select(static item => item.SqlIdentifier!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<string> ResolveTargetSqlIdentifiers(
        MetaTransformBindingModel bindingModel,
        TransformBinding binding)
    {
        var targets = bindingModel.TransformBindingTargetList
            .Where(item => string.Equals(item.TransformBinding.Id, binding.Id, StringComparison.Ordinal))
            .Select(static item => item.SqlIdentifier!.Trim())
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (targets.Length > 0)
        {
            return QualifyTargetSqlIdentifiersFromBindingValidation(bindingModel, binding, targets);
        }

        var rowsetTargets = bindingModel.RowsetList
            .Where(item =>
                string.Equals(item.TransformBinding.Id, binding.Id, StringComparison.Ordinal) &&
                string.Equals(item.DerivationKind, "Target", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(item.SqlIdentifier))
            .Select(static item => item.SqlIdentifier!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return QualifyTargetSqlIdentifiersFromBindingValidation(bindingModel, binding, rowsetTargets);
    }

    private static IReadOnlyList<string> QualifyTargetSqlIdentifiersFromBindingValidation(
        MetaTransformBindingModel? bindingModel,
        TransformBinding? binding,
        IReadOnlyList<string> sqlIdentifiers)
    {
        if (bindingModel is null || binding is null || sqlIdentifiers.Count == 0)
        {
            return sqlIdentifiers;
        }

        return sqlIdentifiers
            .Select(sqlIdentifier => TryResolveValidatedTargetSqlIdentifier(bindingModel, binding, sqlIdentifier) ?? sqlIdentifier)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string? TryResolveValidatedTargetSqlIdentifier(
        MetaTransformBindingModel bindingModel,
        TransformBinding binding,
        string sqlIdentifier)
    {
        var targetIds = bindingModel.TransformBindingTargetList
            .Where(item =>
                string.Equals(item.TransformBinding.Id, binding.Id, StringComparison.Ordinal) &&
                string.Equals(NormalizeObjectKey(item.SqlIdentifier), NormalizeObjectKey(sqlIdentifier), StringComparison.Ordinal))
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);

        if (targetIds.Count == 0)
        {
            return null;
        }

        foreach (var validationTarget in bindingModel.ValidationTargetRowsetLinkList
            .Where(item => targetIds.Contains(item.TransformBindingTarget.Id))
            .OrderBy(static item => item.Id, StringComparer.Ordinal))
        {
            if (TryFormatMetaSchemaTableId(validationTarget.MetaSchemaTableId, out var validatedIdentifier))
            {
                return validatedIdentifier;
            }
        }

        return null;
    }

    private static bool TryFormatMetaSchemaTableId(string metaSchemaTableId, out string sqlIdentifier)
    {
        sqlIdentifier = string.Empty;
        if (string.IsNullOrWhiteSpace(metaSchemaTableId))
        {
            return false;
        }

        var parts = metaSchemaTableId.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 6 ||
            !string.Equals(parts[0], "sqlserver", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var systemName = parts[1];
        var schemaIndex = Array.FindIndex(parts, static item => string.Equals(item, "schema", StringComparison.OrdinalIgnoreCase));
        var tableIndex = Array.FindIndex(parts, static item => string.Equals(item, "table", StringComparison.OrdinalIgnoreCase));
        if (schemaIndex < 0 ||
            tableIndex < 0 ||
            schemaIndex + 1 >= parts.Length ||
            tableIndex + 1 >= parts.Length)
        {
            return false;
        }

        var schemaName = parts[schemaIndex + 1];
        var tableName = parts[tableIndex + 1];
        if (string.IsNullOrWhiteSpace(systemName) ||
            string.IsNullOrWhiteSpace(schemaName) ||
            string.IsNullOrWhiteSpace(tableName))
        {
            return false;
        }

        sqlIdentifier = $"{FormatSqlIdentifierPart(systemName)}.{FormatSqlIdentifierPart(schemaName)}.{FormatSqlIdentifierPart(tableName)}";
        return true;
    }

    private static string FormatSqlIdentifierPart(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.Length > 0 &&
            (char.IsLetter(trimmed[0]) || trimmed[0] == '_') &&
            trimmed.All(static character => char.IsLetterOrDigit(character) || character == '_'))
        {
            return trimmed;
        }

        return "[" + trimmed.Replace("]", "]]", StringComparison.Ordinal) + "]";
    }

    private static IReadOnlyList<string> ResolveInsertRowsTargets(
        MP.MetaPipelineModel pipelineModel,
        MP.PipelineTask transformTask)
    {
        var producedRowstreamIds = pipelineModel.RowStreamProducerList
            .Where(item => string.Equals(item.PipelineTask.Id, transformTask.Id, StringComparison.Ordinal))
            .Select(static item => item.RowStream.Id)
            .ToHashSet(StringComparer.Ordinal);

        if (producedRowstreamIds.Count == 0)
        {
            return [];
        }

        var consumerTaskIds = pipelineModel.RowStreamConsumerList
            .Where(item => producedRowstreamIds.Contains(item.RowStream.Id))
            .Select(static item => item.PipelineTask.Id)
            .ToHashSet(StringComparer.Ordinal);

        var targetWriteTaskIds = pipelineModel.TargetWriteTaskList
            .Where(item => consumerTaskIds.Contains(item.PipelineTask.Id))
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);

        return pipelineModel.InsertRowsTargetWriteTaskList
            .Where(item => targetWriteTaskIds.Contains(item.TargetWriteTask.Id))
            .Select(static item => item.TargetSqlIdentifier.Trim())
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static PipelineObjectAccessProfile CreateAccess(
        string sqlIdentifier,
        OrchestrationObjectAccessKind accessKind,
        string accessRole,
        string reason,
        int ordinal = 0,
        string? operationKind = null)
    {
        var trimmed = sqlIdentifier.Trim();
        return new PipelineObjectAccessProfile(
            trimmed,
            NormalizeObjectKey(trimmed),
            accessKind,
            accessRole,
            ordinal,
            operationKind,
            reason);
    }

    private static PipelineObjectAccessProfile CreatePipelineObjectAccess(
        IReadOnlyList<(PipelineTaskAccessProfile Task, PipelineObjectAccessProfile Access)> accesses)
    {
        var first = accesses
            .OrderBy(static item => item.Access.SqlIdentifier, StringComparer.OrdinalIgnoreCase)
            .First();

        var aggregateKind = AggregateAccessKind(accesses.Select(static item => item.Access.AccessKind));
        var reason = string.Join(
            "; ",
            accesses
                .OrderBy(static item => item.Task.Ordinal)
                .ThenBy(static item => item.Task.TaskName, StringComparer.OrdinalIgnoreCase)
                .Select(static item => $"{item.Task.TaskName}:{item.Access.AccessKind}")
                .Distinct(StringComparer.Ordinal));

        return new PipelineObjectAccessProfile(
            first.Access.SqlIdentifier,
            first.Access.ObjectKey,
            aggregateKind,
            "Pipeline",
            0,
            null,
            reason);
    }

    private static OrchestrationObjectAccessKind AggregateAccessKind(IEnumerable<OrchestrationObjectAccessKind> kinds)
    {
        var set = kinds.ToHashSet();
        if (set.Contains(OrchestrationObjectAccessKind.ResetWrite))
        {
            return OrchestrationObjectAccessKind.ResetWrite;
        }

        if (set.Contains(OrchestrationObjectAccessKind.ReadWrite))
        {
            return OrchestrationObjectAccessKind.ReadWrite;
        }

        if (set.Contains(OrchestrationObjectAccessKind.Write))
        {
            return OrchestrationObjectAccessKind.Write;
        }

        return OrchestrationObjectAccessKind.Read;
    }

    private static GraphAnalysis AnalyzeGraph(IReadOnlyList<PipelineDependencyProfile> pipelines)
    {
        var issues = pipelines
            .SelectMany(static item => item.Issues)
            .ToList();
        var taskObjectEffects = BuildTaskObjectEffects(pipelines);
        var taskEdges = new Dictionary<string, TaskDependencyEdge>(StringComparer.Ordinal);

        AddSerialTaskEdges(taskEdges, pipelines);
        AddDataDependencyTaskEdges(taskEdges, taskObjectEffects);

        issues.AddRange(AnalyzeWriteSemantics(taskObjectEffects));
        issues.AddRange(FindTaskCycleIssues(pipelines, taskEdges.Values));

        var pipelineDependencies = BuildPipelineDependencies(taskEdges.Values);
        var orderedIssues = issues
            .OrderBy(static item => item.BlocksDag ? 0 : 1)
            .ThenBy(static item => item.Domain.ToString(), StringComparer.Ordinal)
            .ThenBy(static item => item.Code.ToString(), StringComparer.Ordinal)
            .ThenBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.Message, StringComparer.Ordinal)
            .ToArray();

        return new GraphAnalysis(
            taskObjectEffects
                .OrderBy(static item => item.PipelineName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(static item => item.TaskName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            taskEdges.Values
                .OrderBy(static item => item.PredecessorTaskId, StringComparer.Ordinal)
                .ThenBy(static item => item.SuccessorTaskId, StringComparer.Ordinal)
                .ThenBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            pipelineDependencies,
            orderedIssues,
            orderedIssues.Any(static item => item.BlocksDag) ? "Invalid" : "Complete",
            orderedIssues.Any(static item => item.Domain == OrchestrationIssueDomain.Determinism && item.BlocksAutomaticRunPlanning)
                ? "RequiresExplicitOrdering"
                : "Deterministic",
            ResolveSynchronizationStatus(orderedIssues));
    }

    private static IReadOnlyList<TaskObjectEffectProfile> BuildTaskObjectEffects(
        IReadOnlyList<PipelineDependencyProfile> pipelines)
    {
        var effects = new List<TaskObjectEffectProfile>();

        foreach (var pipeline in pipelines)
        {
            var resetOrdinalsByObject = pipeline.Tasks
                .SelectMany(task => task.ObjectAccesses.Select(access => new { Task = task, Access = access }))
                .Where(static item => item.Access.AccessKind == OrchestrationObjectAccessKind.ResetWrite)
                .GroupBy(static item => item.Access.ObjectKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    static group => group.Key,
                    static group => group.Select(static item => item.Task.Ordinal).OrderBy(static item => item).ToArray(),
                    StringComparer.OrdinalIgnoreCase);

            var taskObjectGroups = pipeline.Tasks
                .SelectMany(task => task.ObjectAccesses.Select(access => new { Task = task, Access = access }))
                .GroupBy(item => $"{item.Task.PipelineTaskId}|{item.Access.ObjectKey}", StringComparer.OrdinalIgnoreCase);

            foreach (var taskObjectGroup in taskObjectGroups)
            {
                var ordered = taskObjectGroup
                    .OrderBy(static item => item.Task.Ordinal)
                    .ThenBy(static item => item.Access.Ordinal)
                    .ThenBy(static item => item.Task.TaskName, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                var task = ordered[0].Task;
                var objectKey = ordered[0].Access.ObjectKey;
                var hasPriorResetBeforeTask = resetOrdinalsByObject.TryGetValue(objectKey, out var resetOrdinals) &&
                                    resetOrdinals.Any(ordinal => ordinal < task.Ordinal);

                effects.Add(ClassifyTaskObjectEffect(
                    pipeline,
                    task,
                    ordered.Select(static item => item.Access).ToArray(),
                    hasPriorResetBeforeTask));
            }
        }

        return effects;
    }

    private static TaskObjectEffectProfile ClassifyTaskObjectEffect(
        PipelineDependencyProfile pipeline,
        PipelineTaskAccessProfile task,
        IReadOnlyList<PipelineObjectAccessProfile> accesses,
        bool hasPriorResetBeforeTask)
    {
        var orderedAccesses = accesses
            .OrderBy(static item => item.Ordinal)
            .ThenBy(static item => item.AccessRole, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var roles = accesses
            .Select(static item => item.AccessRole)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var kinds = accesses
            .Select(static item => item.AccessKind)
            .ToHashSet();
        var first = accesses
            .OrderBy(static item => item.Ordinal)
            .ThenBy(static item => item.SqlIdentifier, StringComparer.OrdinalIgnoreCase)
            .First();
        var reason = string.Join(
            "; ",
            orderedAccesses
                .Select(static item => string.IsNullOrWhiteSpace(item.OperationKind)
                    ? $"{item.AccessRole}:{item.AccessKind}"
                    : $"{item.Ordinal}:{item.OperationKind}:{item.AccessRole}:{item.AccessKind}"));

        if (roles.Contains("InferredMemberRepair"))
        {
            return CreateEffect(
                pipeline,
                task,
                first,
                OrchestrationAccessDirection.ReadWrite,
                OrchestrationWriteEffect.ConditionalKeyedUpsert,
                OrchestrationAccessPurpose.InferredMemberRepair,
                createsDataDependency: true,
                isPublishedProducer: false,
                requiresSynchronization: true,
                OrchestrationLockMode.KeyedUpsert,
                reason);
        }

        if (roles.Contains("Lookup"))
        {
            return CreateEffect(
                pipeline,
                task,
                first,
                OrchestrationAccessDirection.Read,
                OrchestrationWriteEffect.None,
                OrchestrationAccessPurpose.Lookup,
                createsDataDependency: true,
                isPublishedProducer: false,
                requiresSynchronization: false,
                OrchestrationLockMode.SharedRead,
                reason);
        }

        var hasRead = kinds.Contains(OrchestrationObjectAccessKind.Read);
        var hasWrite = kinds.Contains(OrchestrationObjectAccessKind.Write);
        var hasReadWrite = kinds.Contains(OrchestrationObjectAccessKind.ReadWrite);
        var hasReset = kinds.Contains(OrchestrationObjectAccessKind.ResetWrite);
        var mutatingAccesses = orderedAccesses
            .Where(static item => item.AccessKind is OrchestrationObjectAccessKind.Write or OrchestrationObjectAccessKind.ReadWrite or OrchestrationObjectAccessKind.ResetWrite)
            .ToArray();
        var finalMutatingAccess = mutatingAccesses.LastOrDefault();

        if (!hasWrite && !hasReadWrite && !hasReset)
        {
            return CreateEffect(
                pipeline,
                task,
                first,
                OrchestrationAccessDirection.Read,
                OrchestrationWriteEffect.None,
                roles.Contains("Audit") ? OrchestrationAccessPurpose.Audit : OrchestrationAccessPurpose.SourceInput,
                createsDataDependency: true,
                isPublishedProducer: false,
                requiresSynchronization: false,
                OrchestrationLockMode.SharedRead,
                reason);
        }

        if (finalMutatingAccess is not null &&
            finalMutatingAccess.AccessKind == OrchestrationObjectAccessKind.ResetWrite)
        {
            return CreateEffect(
                pipeline,
                task,
                first,
                OrchestrationAccessDirection.Write,
                OrchestrationWriteEffect.ResetOnly,
                OrchestrationAccessPurpose.TargetMutation,
                createsDataDependency: false,
                isPublishedProducer: false,
                requiresSynchronization: true,
                OrchestrationLockMode.Exclusive,
                reason);
        }

        var hasResetBeforeFinalWrite = hasPriorResetBeforeTask ||
                                       (finalMutatingAccess is not null &&
                                        orderedAccesses.Any(access =>
                                            access.AccessKind == OrchestrationObjectAccessKind.ResetWrite &&
                                            access.Ordinal < finalMutatingAccess.Ordinal));
        var finalOperationKind = finalMutatingAccess?.OperationKind;
        var isTargetLoad = roles.Contains("InsertRowsTarget") ||
                           string.Equals(task.StatementKind, BoundStatementKind.Select.ToString(), StringComparison.Ordinal) ||
                           string.Equals(finalOperationKind, "Append", StringComparison.Ordinal) ||
                           string.Equals(finalOperationKind, "Replace", StringComparison.Ordinal);

        if (isTargetLoad)
        {
            var writeEffect = hasResetBeforeFinalWrite ||
                              string.Equals(finalOperationKind, "Replace", StringComparison.Ordinal)
                ? OrchestrationWriteEffect.Replace
                : OrchestrationWriteEffect.Append;
            return CreateEffect(
                pipeline,
                task,
                first,
                hasRead || hasReadWrite ? OrchestrationAccessDirection.ReadWrite : OrchestrationAccessDirection.Write,
                writeEffect,
                OrchestrationAccessPurpose.TargetLoad,
                createsDataDependency: true,
                isPublishedProducer: true,
                requiresSynchronization: true,
                writeEffect == OrchestrationWriteEffect.Replace ? OrchestrationLockMode.ReplaceWrite : OrchestrationLockMode.AppendWrite,
                reason);
        }

        var mutationWriteEffect = string.Equals(task.StatementKind, BoundStatementKind.Insert.ToString(), StringComparison.Ordinal)
            ? (hasResetBeforeFinalWrite ? OrchestrationWriteEffect.Replace : OrchestrationWriteEffect.Append)
            : OrchestrationWriteEffect.Mutation;
        var lockMode = mutationWriteEffect switch
        {
            OrchestrationWriteEffect.Append => OrchestrationLockMode.AppendWrite,
            OrchestrationWriteEffect.Replace => OrchestrationLockMode.ReplaceWrite,
            _ => OrchestrationLockMode.MutationWrite
        };

        return CreateEffect(
            pipeline,
            task,
            first,
            hasRead || hasReadWrite ? OrchestrationAccessDirection.ReadWrite : OrchestrationAccessDirection.Write,
            mutationWriteEffect,
            string.Equals(task.StatementKind, BoundStatementKind.Insert.ToString(), StringComparison.Ordinal)
                ? OrchestrationAccessPurpose.TargetLoad
                : OrchestrationAccessPurpose.TargetMutation,
            createsDataDependency: true,
            isPublishedProducer: true,
            requiresSynchronization: true,
            lockMode,
            reason);
    }

    private static TaskObjectEffectProfile CreateEffect(
        PipelineDependencyProfile pipeline,
        PipelineTaskAccessProfile task,
        PipelineObjectAccessProfile access,
        OrchestrationAccessDirection accessDirection,
        OrchestrationWriteEffect writeEffect,
        OrchestrationAccessPurpose accessPurpose,
        bool createsDataDependency,
        bool isPublishedProducer,
        bool requiresSynchronization,
        OrchestrationLockMode lockMode,
        string reason) =>
        new(
            task.PipelineTaskId,
            task.TaskName,
            pipeline.PipelineId,
            pipeline.PipelineName,
            access.SqlIdentifier,
            access.ObjectKey,
            accessDirection,
            writeEffect,
            accessPurpose,
            createsDataDependency,
            isPublishedProducer,
            requiresSynchronization,
            lockMode,
            reason);

    private static void AddSerialTaskEdges(
        IDictionary<string, TaskDependencyEdge> edges,
        IReadOnlyList<PipelineDependencyProfile> pipelines)
    {
        foreach (var pipeline in pipelines)
        {
            var tasks = pipeline.Tasks
                .OrderBy(static item => item.Ordinal)
                .ThenBy(static item => item.TaskName, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            for (var index = 1; index < tasks.Length; index++)
            {
                AddTaskEdge(
                    edges,
                    tasks[index - 1],
                    tasks[index],
                    pipeline,
                    pipeline,
                    string.Empty,
                    "Serial",
                    $"Pipeline '{pipeline.PipelineName}' task '{tasks[index - 1].TaskName}' precedes '{tasks[index].TaskName}'.");
            }
        }
    }

    private static void AddDataDependencyTaskEdges(
        IDictionary<string, TaskDependencyEdge> edges,
        IReadOnlyList<TaskObjectEffectProfile> effects)
    {
        foreach (var objectGroup in effects.GroupBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase))
        {
            var producers = objectGroup
                .Where(static item => item.CreatesDataDependency && item.IsPublishedProducer)
                .ToArray();
            var consumers = objectGroup
                .Where(static item => item.CreatesDataDependency && !item.IsPublishedProducer && IsDependencyConsumer(item))
                .ToArray();

            foreach (var producer in producers)
            {
                foreach (var consumer in consumers)
                {
                    if (string.Equals(producer.PipelineTaskId, consumer.PipelineTaskId, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    AddTaskEdge(
                        edges,
                        producer,
                        consumer,
                        objectGroup.Key,
                        "Data",
                        $"Task '{producer.TaskName}' produces '{producer.SqlIdentifier}' consumed by task '{consumer.TaskName}'.");
                }
            }
        }
    }

    private static bool IsDependencyConsumer(TaskObjectEffectProfile effect) =>
        effect.AccessDirection is OrchestrationAccessDirection.Read or OrchestrationAccessDirection.ReadWrite &&
        effect.AccessPurpose is OrchestrationAccessPurpose.SourceInput or OrchestrationAccessPurpose.Lookup or OrchestrationAccessPurpose.InferredMemberRepair;

    private static IReadOnlyList<PipelineDependencyProfileIssue> AnalyzeWriteSemantics(
        IReadOnlyList<TaskObjectEffectProfile> effects)
    {
        var issues = new List<PipelineDependencyProfileIssue>();

        foreach (var objectGroup in effects.GroupBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase))
        {
            var group = objectGroup.ToArray();
            if (group.Length <= 1)
            {
                continue;
            }

            foreach (var reset in group.Where(static item => item.WriteEffect == OrchestrationWriteEffect.ResetOnly))
            {
                var samePipelineReplacement = group.Any(item =>
                    string.Equals(item.PipelineId, reset.PipelineId, StringComparison.Ordinal) &&
                    item.WriteEffect == OrchestrationWriteEffect.Replace);
                var otherPipelineTouches = group.Any(item => !string.Equals(item.PipelineId, reset.PipelineId, StringComparison.Ordinal));
                if (otherPipelineTouches && !samePipelineReplacement)
                {
                    issues.Add(CreateIssue(
                        OrchestrationIssueCode.UnsafeSharedReset,
                        OrchestrationIssueDomain.Dependency,
                        "Error",
                        blocksDag: true,
                        blocksAutomaticRunPlanning: true,
                        $"Pipeline '{reset.PipelineName}' resets '{reset.SqlIdentifier}' without a same-pipeline replacement while other pipelines also touch that object.",
                        objectGroup.Key,
                        group.Select(static item => item.PipelineId).Distinct(StringComparer.Ordinal).ToArray()));
                }
            }

            var writers = group
                .Where(static item => item.WriteEffect != OrchestrationWriteEffect.None)
                .Where(item => item.WriteEffect != OrchestrationWriteEffect.ResetOnly || !group.Any(other =>
                    string.Equals(other.PipelineId, item.PipelineId, StringComparison.Ordinal) &&
                    other.WriteEffect == OrchestrationWriteEffect.Replace))
                .ToArray();
            var crossPipelineWriters = writers
                .GroupBy(static item => item.PipelineId, StringComparer.Ordinal)
                .Select(static item => item.First())
                .ToArray();

            if (crossPipelineWriters.Length <= 1)
            {
                continue;
            }

            var writeEffects = crossPipelineWriters.Select(static item => item.WriteEffect).ToHashSet();
            if (writeEffects.IsSubsetOf([OrchestrationWriteEffect.Append, OrchestrationWriteEffect.OperationalAppend]))
            {
                issues.Add(CreateIssue(
                    OrchestrationIssueCode.SharedAppendWritersRequirePolicy,
                    OrchestrationIssueDomain.Synchronization,
                    "Informational",
                    blocksDag: false,
                    blocksAutomaticRunPlanning: false,
                    $"Multiple pipelines append to '{crossPipelineWriters[0].SqlIdentifier}'; the dependency DAG is valid, and concurrent append requires an explicit lock policy.",
                    objectGroup.Key,
                    crossPipelineWriters.Select(static item => item.PipelineId).ToArray()));
                continue;
            }

            if (writeEffects.Contains(OrchestrationWriteEffect.Replace) && writeEffects.Count > 1)
            {
                issues.Add(CreateIssue(
                    OrchestrationIssueCode.WriteOrderAmbiguity,
                    OrchestrationIssueDomain.Determinism,
                    "RequiresPolicy",
                    blocksDag: false,
                    blocksAutomaticRunPlanning: true,
                    $"Replacement and non-replacement writes both affect '{crossPipelineWriters[0].SqlIdentifier}', so final state depends on explicit write order.",
                    objectGroup.Key,
                    crossPipelineWriters.Select(static item => item.PipelineId).ToArray()));
            }
            else if (crossPipelineWriters.Count(static item => item.WriteEffect == OrchestrationWriteEffect.Replace) > 1)
            {
                issues.Add(CreateIssue(
                    OrchestrationIssueCode.MultipleReplacementProducers,
                    OrchestrationIssueDomain.Determinism,
                    "RequiresPolicy",
                    blocksDag: false,
                    blocksAutomaticRunPlanning: true,
                    $"Multiple pipelines replace '{crossPipelineWriters[0].SqlIdentifier}', so one authoritative producer must be selected or explicitly ordered.",
                    objectGroup.Key,
                    crossPipelineWriters.Select(static item => item.PipelineId).ToArray()));
            }
            else if (crossPipelineWriters.Any(static item => item.WriteEffect == OrchestrationWriteEffect.Mutation))
            {
                issues.Add(CreateIssue(
                    OrchestrationIssueCode.WriteOrderAmbiguity,
                    OrchestrationIssueDomain.Determinism,
                    "RequiresPolicy",
                    blocksDag: false,
                    blocksAutomaticRunPlanning: true,
                    $"Multiple pipelines mutate '{crossPipelineWriters[0].SqlIdentifier}', so orchestration needs explicit ordering or policy before automatic run planning.",
                    objectGroup.Key,
                    crossPipelineWriters.Select(static item => item.PipelineId).ToArray()));
            }

            if (crossPipelineWriters.Count(static item => item.RequiresSynchronization) > 1)
            {
                issues.Add(CreateIssue(
                    OrchestrationIssueCode.SynchronizationRequired,
                    OrchestrationIssueDomain.Synchronization,
                    writeEffects.Contains(OrchestrationWriteEffect.ConditionalKeyedUpsert) ? "RequiresPolicy" : "Informational",
                    blocksDag: false,
                    blocksAutomaticRunPlanning: false,
                    $"Concurrent access to '{crossPipelineWriters[0].SqlIdentifier}' needs lock-aware run planning; this is separate from dependency ordering.",
                    objectGroup.Key,
                    crossPipelineWriters.Select(static item => item.PipelineId).ToArray()));
            }
        }

        return issues;
    }

    private static IReadOnlyList<PipelineDependencyProfileIssue> FindTaskCycleIssues(
        IReadOnlyList<PipelineDependencyProfile> pipelines,
        IEnumerable<TaskDependencyEdge> edges)
    {
        var taskPipelineById = pipelines
            .SelectMany(pipeline => pipeline.Tasks.Select(task => new { task.PipelineTaskId, pipeline.PipelineId }))
            .ToDictionary(static item => item.PipelineTaskId, static item => item.PipelineId, StringComparer.Ordinal);
        var taskIds = taskPipelineById.Keys.ToHashSet(StringComparer.Ordinal);
        var adjacency = taskIds.ToDictionary(static item => item, static _ => new List<string>(), StringComparer.Ordinal);
        foreach (var edge in edges)
        {
            if (adjacency.TryGetValue(edge.PredecessorTaskId, out var successors))
            {
                successors.Add(edge.SuccessorTaskId);
            }
        }

        var permanent = new HashSet<string>(StringComparer.Ordinal);
        var temporary = new HashSet<string>(StringComparer.Ordinal);
        var stack = new Stack<string>();
        var cycles = new List<PipelineDependencyProfileIssue>();

        foreach (var taskId in taskIds.OrderBy(static item => item, StringComparer.Ordinal))
        {
            Visit(taskId);
        }

        return cycles;

        bool Visit(string taskId)
        {
            if (permanent.Contains(taskId))
            {
                return false;
            }

            if (temporary.Contains(taskId))
            {
                var cycle = stack.Reverse().SkipWhile(item => !string.Equals(item, taskId, StringComparison.Ordinal)).Concat([taskId]).ToArray();
                cycles.Add(CreateIssue(
                    OrchestrationIssueCode.DependencyCycle,
                    OrchestrationIssueDomain.Dependency,
                    "Error",
                    blocksDag: true,
                    blocksAutomaticRunPlanning: true,
                    $"Task dependencies contain a cycle: {string.Join(" -> ", cycle)}.",
                    null,
                    cycle
                        .Select(item => taskPipelineById.TryGetValue(item, out var pipelineId) ? pipelineId : string.Empty)
                        .Where(static item => !string.IsNullOrWhiteSpace(item))
                        .Distinct(StringComparer.Ordinal)
                        .ToArray()));
                return true;
            }

            temporary.Add(taskId);
            stack.Push(taskId);
            foreach (var successor in adjacency[taskId].OrderBy(static item => item, StringComparer.Ordinal))
            {
                Visit(successor);
            }

            _ = stack.Pop();
            temporary.Remove(taskId);
            permanent.Add(taskId);
            return false;
        }
    }

    private static IReadOnlyList<PipelineDependencyEdge> BuildPipelineDependencies(IEnumerable<TaskDependencyEdge> taskEdges)
    {
        return taskEdges
            .Where(static item => !string.Equals(item.PredecessorPipelineId, item.SuccessorPipelineId, StringComparison.Ordinal))
            .Where(static item => string.Equals(item.DependencyKind, "Data", StringComparison.Ordinal))
            .GroupBy(static item => $"{item.PredecessorPipelineId}->{item.SuccessorPipelineId}", StringComparer.Ordinal)
            .Select(static group =>
            {
                var first = group.OrderBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase).First();
                return new PipelineDependencyEdge(
                    first.PredecessorPipelineId,
                    first.SuccessorPipelineId,
                    "Inferred",
                    string.Join("; ", group.Select(static item => item.Reason).Distinct(StringComparer.Ordinal)));
            })
            .OrderBy(static item => item.PredecessorPipelineId, StringComparer.Ordinal)
            .ThenBy(static item => item.SuccessorPipelineId, StringComparer.Ordinal)
            .ToArray();
    }

    private static void AddTaskEdge(
        IDictionary<string, TaskDependencyEdge> edges,
        TaskObjectEffectProfile predecessor,
        TaskObjectEffectProfile successor,
        string objectKey,
        string dependencyKind,
        string reason)
    {
        if (string.Equals(predecessor.PipelineTaskId, successor.PipelineTaskId, StringComparison.Ordinal))
        {
            return;
        }

        var key = $"{predecessor.PipelineTaskId}->{successor.PipelineTaskId}:{dependencyKind}:{objectKey}";
        edges.TryAdd(key, new TaskDependencyEdge(
            predecessor.PipelineTaskId,
            successor.PipelineTaskId,
            predecessor.PipelineId,
            successor.PipelineId,
            objectKey,
            dependencyKind,
            reason));
    }

    private static void AddTaskEdge(
        IDictionary<string, TaskDependencyEdge> edges,
        PipelineTaskAccessProfile predecessor,
        PipelineTaskAccessProfile successor,
        PipelineDependencyProfile predecessorPipeline,
        PipelineDependencyProfile successorPipeline,
        string objectKey,
        string dependencyKind,
        string reason)
    {
        if (string.Equals(predecessor.PipelineTaskId, successor.PipelineTaskId, StringComparison.Ordinal))
        {
            return;
        }

        var key = $"{predecessor.PipelineTaskId}->{successor.PipelineTaskId}:{dependencyKind}:{objectKey}";
        edges.TryAdd(key, new TaskDependencyEdge(
            predecessor.PipelineTaskId,
            successor.PipelineTaskId,
            predecessorPipeline.PipelineId,
            successorPipeline.PipelineId,
            objectKey,
            dependencyKind,
            reason));
    }

    private static string ResolveSynchronizationStatus(IReadOnlyList<PipelineDependencyProfileIssue> issues)
    {
        if (issues.Any(static item => item.Domain == OrchestrationIssueDomain.Synchronization && string.Equals(item.Severity, "RequiresPolicy", StringComparison.Ordinal)))
        {
            return "RequiresPolicy";
        }

        return issues.Any(static item => item.Domain == OrchestrationIssueDomain.Synchronization)
            ? "HasConstraints"
            : "Complete";
    }

    private static MO.MetaOrchestrationModel BuildModel(
        OrchestrationAnalysisResult result,
        string pipelineWorkspacePath)
    {
        var model = MO.MetaOrchestrationModel.CreateEmpty();
        var plan = new MO.OrchestrationPlan
        {
            Id = NaturalId("plan", result.PlanName),
            Name = result.PlanName,
            Description = result.Description,
            DagStatus = result.DagStatus,
            DeterminismStatus = result.DeterminismStatus,
            SynchronizationStatus = result.SynchronizationStatus
        };
        model.OrchestrationPlanList.Add(plan);

        var pipelineRows = new Dictionary<string, MO.PipelineReference>(StringComparer.Ordinal);
        foreach (var pipeline in result.Pipelines.OrderBy(static item => item.PipelineName, StringComparer.OrdinalIgnoreCase))
        {
            var row = new MO.PipelineReference
            {
                Id = NaturalId(plan.Id, "pipeline", pipeline.PipelineName),
                OrchestrationPlan = plan,
                Name = pipeline.PipelineName,
                MetaPipelinePipelineId = pipeline.PipelineId,
                PipelineWorkspacePath = pipelineWorkspacePath
            };
            model.PipelineReferenceList.Add(row);
            pipelineRows.Add(pipeline.PipelineId, row);
        }

        var objects = result.Pipelines
            .SelectMany(static pipeline => pipeline.Tasks)
            .SelectMany(static task => task.ObjectAccesses)
            .GroupBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.OrderBy(static item => item.SqlIdentifier, StringComparer.OrdinalIgnoreCase).First())
            .OrderBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var objectRows = new Dictionary<string, MO.DataObject>(StringComparer.OrdinalIgnoreCase);
        foreach (var dataObject in objects)
        {
            var row = new MO.DataObject
            {
                Id = NaturalId(plan.Id, "object", dataObject.ObjectKey),
                OrchestrationPlan = plan,
                SqlIdentifier = dataObject.SqlIdentifier,
                NormalizedKey = dataObject.ObjectKey
            };
            model.DataObjectList.Add(row);
            objectRows.Add(dataObject.ObjectKey, row);
        }

        var taskRows = new Dictionary<string, MO.TaskAccessProfile>(StringComparer.Ordinal);
        foreach (var pipeline in result.Pipelines.OrderBy(static item => item.PipelineName, StringComparer.OrdinalIgnoreCase))
        {
            foreach (var task in pipeline.Tasks.OrderBy(static item => item.Ordinal).ThenBy(static item => item.TaskName, StringComparer.OrdinalIgnoreCase))
            {
                var row = new MO.TaskAccessProfile
                {
                    Id = NaturalId(pipelineRows[pipeline.PipelineId].Id, "task", task.TaskName),
                    PipelineReference = pipelineRows[pipeline.PipelineId],
                    MetaPipelinePipelineTaskId = task.PipelineTaskId,
                    TaskName = task.TaskName,
                    TaskKind = task.TaskKind,
                    Ordinal = task.Ordinal.ToString(CultureInfo.InvariantCulture),
                    TransformScriptId = task.TransformScriptId,
                    TransformScriptName = task.TransformScriptName,
                    TransformBindingId = task.TransformBindingId,
                    TransformWorkspacePath = task.TransformWorkspacePath,
                    BindingWorkspacePath = task.BindingWorkspacePath,
                    StatementKind = task.StatementKind
                };
                model.TaskAccessProfileList.Add(row);
                taskRows.Add(task.PipelineTaskId, row);

                var accessOrdinal = 0;
                foreach (var access in task.ObjectAccesses.OrderBy(static item => item.Ordinal).ThenBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase).ThenBy(static item => item.AccessKind.ToString(), StringComparer.Ordinal))
                {
                    model.ObjectAccessList.Add(new MO.ObjectAccess
                    {
                        Id = NaturalId(row.Id, "access", (++accessOrdinal).ToString(CultureInfo.InvariantCulture)),
                        TaskAccessProfile = row,
                        DataObject = objectRows[access.ObjectKey],
                        Ordinal = access.Ordinal.ToString(CultureInfo.InvariantCulture),
                        AccessKind = access.AccessKind.ToString(),
                        AccessRole = access.AccessRole,
                        OperationKind = access.OperationKind,
                        Reason = access.Reason
                    });
                }
            }

            foreach (var access in pipeline.ObjectAccesses.OrderBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase))
            {
                model.PipelineObjectAccessList.Add(new MO.PipelineObjectAccess
                {
                    Id = NaturalId(pipelineRows[pipeline.PipelineId].Id, "object", access.ObjectKey),
                    PipelineReference = pipelineRows[pipeline.PipelineId],
                    DataObject = objectRows[access.ObjectKey],
                    AccessKind = access.AccessKind.ToString(),
                    Reason = access.Reason
                });
            }
        }

        foreach (var effect in result.TaskObjectEffects
                     .OrderBy(static item => item.PipelineName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(static item => item.TaskName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase))
        {
            model.TaskObjectEffectList.Add(new MO.TaskObjectEffect
            {
                Id = NaturalId(taskRows[effect.PipelineTaskId].Id, "effect", effect.ObjectKey),
                TaskAccessProfile = taskRows[effect.PipelineTaskId],
                DataObject = objectRows[effect.ObjectKey],
                AccessDirection = effect.AccessDirection.ToString(),
                WriteEffect = effect.WriteEffect.ToString(),
                AccessPurpose = effect.AccessPurpose.ToString(),
                CreatesDataDependency = BoolText(effect.CreatesDataDependency),
                IsPublishedProducer = BoolText(effect.IsPublishedProducer),
                RequiresSynchronization = BoolText(effect.RequiresSynchronization),
                LockMode = effect.LockMode.ToString(),
                Reason = effect.Reason
            });
        }

        foreach (var dependency in result.TaskDependencies)
        {
            model.TaskDependencyList.Add(new MO.TaskDependency
            {
                Id = NaturalId(
                    plan.Id,
                    "task-dependency",
                    dependency.PredecessorTaskId,
                    "before",
                    dependency.SuccessorTaskId,
                    dependency.DependencyKind,
                    dependency.ObjectKey),
                OrchestrationPlan = plan,
                Predecessor = taskRows[dependency.PredecessorTaskId],
                Successor = taskRows[dependency.SuccessorTaskId],
                DataObject = string.IsNullOrWhiteSpace(dependency.ObjectKey) ? null : objectRows[dependency.ObjectKey],
                DependencyKind = dependency.DependencyKind,
                DependencyCondition = "OnSuccess",
                Reason = dependency.Reason
            });
        }

        foreach (var dependency in result.Dependencies)
        {
            model.PipelineDependencyList.Add(new MO.PipelineDependency
            {
                Id = NaturalId(plan.Id, "dependency", dependency.PredecessorPipelineId, "before", dependency.SuccessorPipelineId),
                OrchestrationPlan = plan,
                Predecessor = pipelineRows[dependency.PredecessorPipelineId],
                Successor = pipelineRows[dependency.SuccessorPipelineId],
                DependencyKind = dependency.DependencyKind,
                Reason = dependency.Reason
            });
        }

        var issueOrdinal = 0;
        foreach (var issue in result.Issues)
        {
            var issueRow = new MO.DependencyIssue
            {
                Id = NaturalId(plan.Id, "issue", (++issueOrdinal).ToString(CultureInfo.InvariantCulture)),
                OrchestrationPlan = plan,
                DataObject = issue.ObjectKey is not null && objectRows.TryGetValue(issue.ObjectKey, out var objectRow) ? objectRow : null,
                Code = issue.Code.ToString(),
                IssueDomain = issue.Domain.ToString(),
                Severity = issue.Severity,
                BlocksDag = BoolText(issue.BlocksDag),
                BlocksAutomaticRunPlanning = BoolText(issue.BlocksAutomaticRunPlanning),
                Message = issue.Message
            };
            model.DependencyIssueList.Add(issueRow);

            var participantOrdinal = 0;
            foreach (var pipelineId in issue.PipelineIds.Distinct(StringComparer.Ordinal).OrderBy(static item => item, StringComparer.Ordinal))
            {
                if (!pipelineRows.TryGetValue(pipelineId, out var pipelineRow))
                {
                    continue;
                }

                model.DependencyIssuePipelineList.Add(new MO.DependencyIssuePipeline
                {
                    Id = NaturalId(issueRow.Id, "pipeline", (++participantOrdinal).ToString(CultureInfo.InvariantCulture)),
                    DependencyIssue = issueRow,
                    PipelineReference = pipelineRow,
                    Role = "Participant"
                });
            }
        }

        return model;
    }

    private static PipelineDependencyProfileIssue CreateIssue(
        OrchestrationIssueCode code,
        OrchestrationIssueDomain domain,
        string severity,
        bool blocksDag,
        bool blocksAutomaticRunPlanning,
        string message,
        string? objectKey,
        IReadOnlyList<string> pipelineIds) =>
        new(
            code,
            domain,
            severity,
            blocksDag,
            blocksAutomaticRunPlanning,
            message,
            objectKey,
            pipelineIds);

    private static string BoolText(bool value) => value ? "true" : "false";

    private static string NormalizeObjectKey(string sqlIdentifier)
    {
        var parts = sqlIdentifier
            .Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(static part => part.Trim())
            .Select(static part =>
            {
                if (part.Length >= 2 && part[0] == '[' && part[^1] == ']')
                {
                    return part[1..^1].Replace("]]", "]", StringComparison.Ordinal);
                }

                if (part.Length >= 2 && part[0] == '"' && part[^1] == '"')
                {
                    return part[1..^1].Replace("\"\"", "\"", StringComparison.Ordinal);
                }

                return part;
            })
            .Select(static part => part.ToUpperInvariant());

        return string.Join(".", parts);
    }

    private static string NaturalId(params string[] parts)
    {
        var builder = new StringBuilder();
        foreach (var part in parts)
        {
            if (builder.Length > 0)
            {
                builder.Append(':');
            }

            var emitted = false;
            foreach (var character in part.Trim())
            {
                if (char.IsLetterOrDigit(character))
                {
                    builder.Append(char.ToLowerInvariant(character));
                    emitted = true;
                    continue;
                }

                if (emitted && builder[^1] != '-')
                {
                    builder.Append('-');
                }
            }

            if (builder.Length > 0 && builder[^1] == '-')
            {
                builder.Length--;
            }
        }

        return builder.Length == 0 ? "id" : builder.ToString();
    }

    private static IReadOnlyList<MP.PipelineTask> ResolvePipelineTasksInDependencyOrder(
        MP.MetaPipelineModel model,
        MP.Pipeline pipeline)
    {
        var tasks = model.PipelineTaskList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal))
            .ToArray();
        if (tasks.Length <= 1)
        {
            return tasks;
        }

        var tasksById = tasks.ToDictionary(static item => item.Id, StringComparer.Ordinal);
        var dependencies = model.TaskDependencyList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal))
            .Where(item => tasksById.ContainsKey(item.Predecessor.Id) && tasksById.ContainsKey(item.Successor.Id))
            .ToArray();
        if (dependencies.Length == 0)
        {
            throw new InvalidOperationException(
                $"Pipeline '{pipeline.Name}' has multiple tasks but no TaskDependency rows. Serial pipelines must declare task order.");
        }

        var successorByPredecessor = new Dictionary<string, string>(StringComparer.Ordinal);
        var predecessorBySuccessor = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var dependency in dependencies)
        {
            var predecessorId = dependency.Predecessor.Id;
            var successorId = dependency.Successor.Id;
            if (string.Equals(predecessorId, successorId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' points a task at itself.");
            }

            if (!successorByPredecessor.TryAdd(predecessorId, successorId))
            {
                throw new InvalidOperationException(
                    $"Pipeline '{pipeline.Name}' task '{tasksById[predecessorId].Name}' has multiple successors.");
            }

            if (!predecessorBySuccessor.TryAdd(successorId, predecessorId))
            {
                throw new InvalidOperationException(
                    $"Pipeline '{pipeline.Name}' task '{tasksById[successorId].Name}' has multiple predecessors.");
            }
        }

        var roots = tasks
            .Where(item => !predecessorBySuccessor.ContainsKey(item.Id))
            .ToArray();
        if (roots.Length != 1)
        {
            throw new InvalidOperationException(
                $"Pipeline '{pipeline.Name}' TaskDependency rows must form one serial chain.");
        }

        var ordered = new List<MP.PipelineTask>();
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var current = roots[0];
        while (true)
        {
            if (!visited.Add(current.Id))
            {
                throw new InvalidOperationException(
                    $"Pipeline '{pipeline.Name}' contains a cycle in TaskDependency rows.");
            }

            ordered.Add(current);
            if (!successorByPredecessor.TryGetValue(current.Id, out var successorId))
            {
                break;
            }

            current = tasksById[successorId];
        }

        if (ordered.Count != tasks.Length)
        {
            throw new InvalidOperationException(
                $"Pipeline '{pipeline.Name}' TaskDependency rows do not form one connected serial chain.");
        }

        return ordered;
    }

    private static int ParseOrdinalOrMax(string value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ordinal)
            ? ordinal
            : int.MaxValue;

    private sealed record GraphAnalysis(
        IReadOnlyList<TaskObjectEffectProfile> TaskObjectEffects,
        IReadOnlyList<TaskDependencyEdge> TaskDependencies,
        IReadOnlyList<PipelineDependencyEdge> PipelineDependencies,
        IReadOnlyList<PipelineDependencyProfileIssue> Issues,
        string DagStatus,
        string DeterminismStatus,
        string SynchronizationStatus);

    private sealed record PipelineTaskDetail(
        MP.PipelineTask PipelineTask,
        MP.TransformExecutionTask? TransformExecution,
        MP.ExecutableTask? Executable,
        int Ordinal);

    private sealed class TransformWorkspaceProfileCache(TransformScriptStatementKindService statementKindService)
    {
        private readonly Dictionary<string, TransformWorkspaceProfileContext> cache = new(StringComparer.OrdinalIgnoreCase);

        public TransformWorkspaceProfileContext Get(string transformWorkspacePath, string bindingWorkspacePath)
        {
            var fullTransformWorkspacePath = Path.GetFullPath(transformWorkspacePath);
            var fullBindingWorkspacePath = Path.GetFullPath(bindingWorkspacePath);
            var key = $"{fullTransformWorkspacePath}|{fullBindingWorkspacePath}";
            if (cache.TryGetValue(key, out var existing))
            {
                return existing;
            }

            var transformModel = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(fullTransformWorkspacePath, searchUpward: false);
            var bindingModel = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaTransformBindingModel>(fullBindingWorkspacePath, searchUpward: false);
            var context = new TransformWorkspaceProfileContext(
                fullTransformWorkspacePath,
                fullBindingWorkspacePath,
                bindingModel,
                transformModel.TransformScriptList.ToDictionary(static item => item.Id, StringComparer.Ordinal),
                statementKindService.GetStatementKindsByTransformScriptId(transformModel),
                transformModel.TransformScriptFunctionParametersItemList
                    .GroupBy(static item => item.TransformScript.Id, StringComparer.Ordinal)
                    .ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.Ordinal),
                BuildStoredProcedureOperationsByScriptId(transformModel),
                bindingModel.TransformBindingList.ToDictionary(static item => item.Id, StringComparer.Ordinal));
            cache.Add(key, context);
            return context;
        }
    }

    private sealed record TransformWorkspaceProfileContext(
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        MetaTransformBindingModel BindingModel,
        IReadOnlyDictionary<string, TransformScript> TransformScriptsById,
        IReadOnlyDictionary<string, BoundStatementKind> StatementKindsByScriptId,
        IReadOnlyDictionary<string, int> FunctionParameterCountsByScriptId,
        IReadOnlyDictionary<string, IReadOnlyList<StoredProcedureContractOperation>> StoredProcedureOperationsByScriptId,
        IReadOnlyDictionary<string, TransformBinding> BindingsById);

    private sealed record TaskProfileResult(
        PipelineTaskAccessProfile Profile,
        IReadOnlyList<PipelineDependencyProfileIssue> Issues);
}
