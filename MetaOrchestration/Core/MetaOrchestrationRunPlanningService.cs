using System.Globalization;
using System.Text;
using MO = MetaOrchestration;

namespace MetaOrchestration.Core;

public sealed class MetaOrchestrationRunPlanningService
{
    public const string DefaultRunPlanName = "DefaultRunPlan";

    public OrchestrationPolicyResult AddTaskOrderingResolution(
        MO.MetaOrchestrationModel model,
        string fromTaskSelector,
        string toTaskSelector,
        string? objectSelector,
        string? reason,
        string dependencyCondition = "OnSuccess")
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(fromTaskSelector);
        ArgumentException.ThrowIfNullOrWhiteSpace(toTaskSelector);

        var plan = RequireSinglePlan(model);
        var predecessor = ResolveTask(model, fromTaskSelector);
        var successor = ResolveTask(model, toTaskSelector);
        if (ReferenceEquals(predecessor, successor))
        {
            throw new InvalidOperationException("A task ordering resolution cannot order a task before itself.");
        }

        var dataObject = string.IsNullOrWhiteSpace(objectSelector)
            ? null
            : ResolveDataObject(model, objectSelector);
        var normalizedCondition = NormalizeDependencyCondition(dependencyCondition);
        var resolutionKind = string.Equals(normalizedCondition, "OnSuccess", StringComparison.Ordinal)
            ? "ExplicitTaskOrder"
            : "ExplicitTaskDependency";
        var name = $"{predecessor.TaskName} before {successor.TaskName}";
        var id = NaturalId(plan.Id, "task-dependency", normalizedCondition, predecessor.Id, "before", successor.Id, dataObject?.NormalizedKey ?? "all");

        var existing = model.TaskOrderingResolutionList.SingleOrDefault(item => string.Equals(item.Id, id, StringComparison.Ordinal));
        if (existing is not null)
        {
            existing.Name = name;
            existing.ResolutionKind = resolutionKind;
            existing.DependencyCondition = normalizedCondition;
            existing.Status = "Active";
            existing.Reason = NormalizeOptional(reason);
            existing.DataObject = dataObject;
            existing.Predecessor = predecessor;
            existing.Successor = successor;
            return new OrchestrationPolicyResult(existing.Id, "TaskOrderingResolution", "Updated");
        }

        var resolution = new MO.TaskOrderingResolution
        {
            Id = id,
            OrchestrationPlan = plan,
            Name = name,
            ResolutionKind = resolutionKind,
            DependencyCondition = normalizedCondition,
            Status = "Active",
            Reason = NormalizeOptional(reason),
            DataObject = dataObject,
            Predecessor = predecessor,
            Successor = successor
        };
        model.TaskOrderingResolutionList.Add(resolution);

        return new OrchestrationPolicyResult(resolution.Id, "TaskOrderingResolution", "Added");
    }

    public OrchestrationPolicyResult AddConcurrentAppendPolicy(
        MO.MetaOrchestrationModel model,
        string objectSelector,
        string? reason)
    {
        return AddLockCompatibilityPolicy(
            model,
            objectSelector,
            "Append",
            "Append",
            "AllowConcurrent",
            reason,
            policyKind: "ConcurrentAppendPolicy");
    }

    public OrchestrationPolicyResult AddLockCompatibilityPolicy(
        MO.MetaOrchestrationModel model,
        string objectSelector,
        string leftEffect,
        string rightEffect,
        string lockBehavior,
        string? reason,
        string policyKind = "LockCompatibilityPolicy")
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(objectSelector);
        ArgumentException.ThrowIfNullOrWhiteSpace(leftEffect);
        ArgumentException.ThrowIfNullOrWhiteSpace(rightEffect);
        ArgumentException.ThrowIfNullOrWhiteSpace(lockBehavior);

        var plan = RequireSinglePlan(model);
        var dataObject = ResolveDataObject(model, objectSelector);
        var normalizedLeftEffect = NormalizeWriteEffect(leftEffect);
        var normalizedRightEffect = NormalizeWriteEffect(rightEffect);
        var normalizedBehavior = NormalizeLockBehavior(lockBehavior);
        if (string.Equals(normalizedBehavior, "AllowConcurrent", StringComparison.OrdinalIgnoreCase) &&
            !CanAllowConcurrent(normalizedLeftEffect, normalizedRightEffect))
        {
            throw new InvalidOperationException("AllowConcurrent is currently supported only for Append/Append lock policy. Use serialize for other same-object interactions until a stronger safety model exists.");
        }

        var normalizedPolicyKind = NormalizePolicyKind(policyKind);
        var id = NaturalId(
            plan.Id,
            "lock-policy",
            dataObject.NormalizedKey,
            normalizedLeftEffect,
            normalizedRightEffect,
            normalizedBehavior);

        var existing = model.LockCompatibilityPolicyList.SingleOrDefault(item => string.Equals(item.Id, id, StringComparison.Ordinal));
        if (existing is not null)
        {
            existing.PolicyKind = normalizedPolicyKind;
            existing.LeftEffect = normalizedLeftEffect;
            existing.RightEffect = normalizedRightEffect;
            existing.LockBehavior = normalizedBehavior;
            existing.Status = "Active";
            existing.Reason = NormalizeOptional(reason);
            existing.DataObject = dataObject;
            return new OrchestrationPolicyResult(existing.Id, "LockCompatibilityPolicy", "Updated");
        }

        var policy = new MO.LockCompatibilityPolicy
        {
            Id = id,
            OrchestrationPlan = plan,
            DataObject = dataObject,
            PolicyKind = normalizedPolicyKind,
            LeftEffect = normalizedLeftEffect,
            RightEffect = normalizedRightEffect,
            LockBehavior = normalizedBehavior,
            Status = "Active",
            Reason = NormalizeOptional(reason)
        };
        model.LockCompatibilityPolicyList.Add(policy);

        return new OrchestrationPolicyResult(policy.Id, "LockCompatibilityPolicy", "Added");
    }

    public OrchestrationRunPlanResult BuildRunPlan(MO.MetaOrchestrationModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var plan = RequireSinglePlan(model);
        if (!string.Equals(plan.DagStatus, "Complete", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Cannot build run plan for orchestration plan '{plan.Name}' because DagStatus is '{plan.DagStatus}'.");
        }

        var blockingDagIssues = model.DependencyIssueList
            .Where(static item => IsTrue(item.BlocksDag))
            .ToArray();
        if (blockingDagIssues.Length > 0)
        {
            throw new InvalidOperationException($"Cannot build run plan for orchestration plan '{plan.Name}' because {blockingDagIssues.Length} issue(s) block the dependency graph.");
        }

        var unresolvedRunPlanningIssues = model.DependencyIssueList
            .Where(static item =>
                IsTrue(item.BlocksAutomaticRunPlanning) ||
                (string.Equals(item.IssueDomain, "Synchronization", StringComparison.OrdinalIgnoreCase) &&
                 string.Equals(item.Severity, "RequiresPolicy", StringComparison.OrdinalIgnoreCase)))
            .Where(item => !IsResolvedByPolicy(model, item))
            .ToArray();
        if (unresolvedRunPlanningIssues.Length > 0)
        {
            throw new InvalidOperationException($"Cannot build run plan for orchestration plan '{plan.Name}' because {unresolvedRunPlanningIssues.Length} issue(s) require explicit run-planning policy.");
        }

        var taskRows = model.TaskAccessProfileList
            .OrderBy(static item => item.PipelineReference.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => ParseOrdinal(item.Ordinal))
            .ThenBy(static item => item.TaskName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.Id, StringComparer.Ordinal)
            .ToArray();
        var predecessors = taskRows.ToDictionary(static item => item.Id, static _ => new HashSet<string>(StringComparer.Ordinal), StringComparer.Ordinal);
        var successors = taskRows.ToDictionary(static item => item.Id, static _ => new HashSet<string>(StringComparer.Ordinal), StringComparer.Ordinal);

        foreach (var dependency in model.TaskDependencyList)
        {
            AddEdge(predecessors, successors, dependency.Predecessor.Id, dependency.Successor.Id);
        }

        foreach (var resolution in model.TaskOrderingResolutionList.Where(static item => IsActive(item.Status)))
        {
            AddEdge(predecessors, successors, resolution.Predecessor.Id, resolution.Successor.Id);
        }

        var locksByTaskId = model.TaskObjectEffectList
            .Where(static item => !string.Equals(item.LockMode, "None", StringComparison.OrdinalIgnoreCase))
            .GroupBy(static item => item.TaskAccessProfile.Id, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group
                    .OrderBy(static item => item.DataObject.NormalizedKey, StringComparer.OrdinalIgnoreCase)
                    .Select(static item => new TaskLockRequest(item.DataObject, item, item.LockMode))
                    .ToArray(),
                StringComparer.Ordinal);
        var activeLockPolicies = model.LockCompatibilityPolicyList
            .Where(static item => IsActive(item.Status))
            .ToArray();

        var orderedTasks = BuildDependencyOrderedTasks(taskRows, predecessors);

        var runPlanId = NaturalId(plan.Id, "run-plan", DefaultRunPlanName);
        RemoveExistingRunPlans(model, plan);

        var runPlan = new MO.RunPlan
        {
            Id = runPlanId,
            OrchestrationPlan = plan,
            Name = DefaultRunPlanName,
            RunPlanStatus = "Ready",
            Reason = "Generated from task dependencies, active ordering resolutions, task-object effects, and lock compatibility policy."
        };
        model.RunPlanList.Add(runPlan);

        var globalOrdinal = 0;
        foreach (var task in orderedTasks)
        {
            var plannedTask = new MO.PlannedTask
            {
                RunPlan = runPlan,
                TaskAccessProfile = task,
                PipelineReference = task.PipelineReference,
                Id = NaturalId(runPlan.Id, "task", (++globalOrdinal).ToString(CultureInfo.InvariantCulture)),
                Ordinal = globalOrdinal.ToString(CultureInfo.InvariantCulture),
                Reason = BuildPlannedTaskReason(task, predecessors[task.Id])
            };
            model.PlannedTaskList.Add(plannedTask);

            if (!locksByTaskId.TryGetValue(task.Id, out var locks))
            {
                continue;
            }

            var lockOrdinal = 0;
            foreach (var taskLock in locks)
            {
                var policySource = FindPolicySourceForLock(taskLock, activeLockPolicies);
                model.PlannedTaskLockList.Add(new MO.PlannedTaskLock
                {
                    Id = NaturalId(plannedTask.Id, "lock", (++lockOrdinal).ToString(CultureInfo.InvariantCulture)),
                    PlannedTask = plannedTask,
                    TaskObjectEffect = taskLock.TaskObjectEffect,
                    DataObject = taskLock.DataObject,
                    LockCompatibilityPolicy = policySource,
                    LockMode = taskLock.LockMode,
                    Reason = BuildPlannedTaskLockReason(taskLock, policySource)
                });
            }
        }

        return new OrchestrationRunPlanResult(runPlan.Id, runPlan.Name, runPlan.RunPlanStatus, globalOrdinal);
    }

    private static IReadOnlyList<MO.TaskAccessProfile> BuildDependencyOrderedTasks(
        IReadOnlyList<MO.TaskAccessProfile> taskRows,
        IReadOnlyDictionary<string, HashSet<string>> predecessors)
    {
        var pending = taskRows.Select(static item => item.Id).ToHashSet(StringComparer.Ordinal);
        var completed = new HashSet<string>(StringComparer.Ordinal);
        var orderedTasks = new List<MO.TaskAccessProfile>(taskRows.Count);

        while (pending.Count > 0)
        {
            var eligible = taskRows
                .Where(item => pending.Contains(item.Id) && predecessors[item.Id].All(completed.Contains))
                .ToArray();
            if (eligible.Length == 0)
            {
                throw new InvalidOperationException("Cannot build run plan because task dependencies contain a cycle or unresolved predecessor.");
            }

            foreach (var task in eligible)
            {
                pending.Remove(task.Id);
                completed.Add(task.Id);
                orderedTasks.Add(task);
            }
        }

        return orderedTasks;
    }

    private static void RemoveExistingRunPlans(MO.MetaOrchestrationModel model, MO.OrchestrationPlan plan)
    {
        var existingRunPlans = model.RunPlanList
            .Where(item => ReferenceEquals(item.OrchestrationPlan, plan))
            .ToArray();
        if (existingRunPlans.Length == 0)
        {
            return;
        }

        var tasks = model.PlannedTaskList
            .Where(task => existingRunPlans.Any(runPlan => ReferenceEquals(task.RunPlan, runPlan)))
            .ToArray();
        var locks = model.PlannedTaskLockList
            .Where(taskLock => tasks.Any(task => ReferenceEquals(taskLock.PlannedTask, task)))
            .ToArray();
        foreach (var taskLock in locks)
        {
            model.PlannedTaskLockList.Remove(taskLock);
        }

        foreach (var task in tasks)
        {
            model.PlannedTaskList.Remove(task);
        }

        foreach (var runPlan in existingRunPlans)
        {
            model.RunPlanList.Remove(runPlan);
        }
    }

    private static void AddEdge(
        IReadOnlyDictionary<string, HashSet<string>> predecessors,
        IReadOnlyDictionary<string, HashSet<string>> successors,
        string predecessorId,
        string successorId)
    {
        if (string.Equals(predecessorId, successorId, StringComparison.Ordinal))
        {
            return;
        }

        if (predecessors.TryGetValue(successorId, out var predecessorSet))
        {
            predecessorSet.Add(predecessorId);
        }

        if (successors.TryGetValue(predecessorId, out var successorSet))
        {
            successorSet.Add(successorId);
        }
    }

    private static MO.LockCompatibilityPolicy? FindPolicySourceForLock(
        TaskLockRequest taskLockRequest,
        IReadOnlyList<MO.LockCompatibilityPolicy> activeLockPolicies)
    {
        return activeLockPolicies
            .Where(policy => string.Equals(policy.DataObject.Id, taskLockRequest.DataObject.Id, StringComparison.Ordinal))
            .Where(policy =>
                string.Equals(policy.LeftEffect, taskLockRequest.TaskObjectEffect.WriteEffect, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(policy.RightEffect, taskLockRequest.TaskObjectEffect.WriteEffect, StringComparison.OrdinalIgnoreCase))
            .OrderBy(static policy => policy.PolicyKind, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static policy => policy.Id, StringComparer.Ordinal)
            .FirstOrDefault();
    }

    private static bool EffectsMatch(MO.LockCompatibilityPolicy policy, string leftEffect, string rightEffect)
    {
        return
            (string.Equals(policy.LeftEffect, leftEffect, StringComparison.OrdinalIgnoreCase) &&
             string.Equals(policy.RightEffect, rightEffect, StringComparison.OrdinalIgnoreCase)) ||
            (string.Equals(policy.LeftEffect, rightEffect, StringComparison.OrdinalIgnoreCase) &&
             string.Equals(policy.RightEffect, leftEffect, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsResolvedByPolicy(MO.MetaOrchestrationModel model, MO.DependencyIssue issue)
    {
        if (string.Equals(issue.IssueDomain, "Determinism", StringComparison.OrdinalIgnoreCase))
        {
            return issue.DataObject is not null &&
                   model.TaskOrderingResolutionList.Any(resolution =>
                       IsActive(resolution.Status) &&
                       resolution.DataObject is not null &&
                       string.Equals(resolution.DataObject.Id, issue.DataObject.Id, StringComparison.Ordinal));
        }

        if (string.Equals(issue.IssueDomain, "Synchronization", StringComparison.OrdinalIgnoreCase))
        {
            if (issue.DataObject is null)
            {
                return false;
            }

            var effects = model.TaskObjectEffectList
                .Where(effect => string.Equals(effect.DataObject.Id, issue.DataObject.Id, StringComparison.Ordinal))
                .Where(static effect => IsTrue(effect.RequiresSynchronization))
                .Select(static effect => effect.WriteEffect)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return model.LockCompatibilityPolicyList.Any(policy =>
                IsActive(policy.Status) &&
                string.Equals(policy.DataObject.Id, issue.DataObject.Id, StringComparison.Ordinal) &&
                effects.Contains(policy.LeftEffect, StringComparer.OrdinalIgnoreCase) &&
                effects.Contains(policy.RightEffect, StringComparer.OrdinalIgnoreCase));
        }

        return false;
    }

    private static string BuildPlannedTaskReason(
        MO.TaskAccessProfile task,
        IReadOnlySet<string> predecessorIds)
    {
        var predecessorText = predecessorIds.Count == 0
            ? "no task predecessors"
            : $"{predecessorIds.Count} task predecessor(s)";
        return $"Dependency-ordered after {predecessorText}.";
    }

    private static string BuildPlannedTaskLockReason(
        TaskLockRequest taskLock,
        MO.LockCompatibilityPolicy? policySource)
    {
        var baseReason = $"Derived from task-object effect {taskLock.TaskObjectEffect.WriteEffect}/{taskLock.TaskObjectEffect.AccessPurpose} on {taskLock.DataObject.SqlIdentifier}.";
        if (policySource is null)
        {
            return baseReason;
        }

        return $"{baseReason} Policy {policySource.Id} applies to {policySource.LeftEffect}/{policySource.RightEffect} with {policySource.LockBehavior}.";
    }

    private static MO.OrchestrationPlan RequireSinglePlan(MO.MetaOrchestrationModel model)
    {
        return model.OrchestrationPlanList.Count == 1
            ? model.OrchestrationPlanList[0]
            : throw new InvalidOperationException($"Expected exactly one orchestration plan, found {model.OrchestrationPlanList.Count}.");
    }

    private static MO.TaskAccessProfile ResolveTask(MO.MetaOrchestrationModel model, string selector)
    {
        var trimmed = selector.Trim();
        var matches = model.TaskAccessProfileList
            .Where(item =>
                string.Equals(item.Id, trimmed, StringComparison.Ordinal) ||
                string.Equals(item.MetaPipelinePipelineTaskId, trimmed, StringComparison.Ordinal) ||
                string.Equals(item.TaskName, trimmed, StringComparison.OrdinalIgnoreCase) ||
                string.Equals($"{item.PipelineReference.Name}.{item.TaskName}", trimmed, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return matches.Length switch
        {
            1 => matches[0],
            0 => throw new InvalidOperationException($"Could not resolve task '{selector}'. Use task id, task name, or Pipeline.Task."),
            _ => throw new InvalidOperationException($"Task selector '{selector}' matched {matches.Length} tasks. Use task id or Pipeline.Task.")
        };
    }

    private static MO.DataObject ResolveDataObject(MO.MetaOrchestrationModel model, string selector)
    {
        var normalized = NormalizeObjectKey(selector);
        var matches = model.DataObjectList
            .Where(item =>
                string.Equals(item.Id, selector.Trim(), StringComparison.Ordinal) ||
                string.Equals(item.NormalizedKey, normalized, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(item.SqlIdentifier, selector.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return matches.Length switch
        {
            1 => matches[0],
            0 => throw new InvalidOperationException($"Could not resolve data object '{selector}'."),
            _ => throw new InvalidOperationException($"Data object selector '{selector}' matched {matches.Length} objects.")
        };
    }

    private static string NormalizeWriteEffect(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "none" => "None",
            "append" => "Append",
            "replace" => "Replace",
            "reset" or "reset-only" or "resetonly" => "ResetOnly",
            "mutation" => "Mutation",
            "keyed-upsert" or "keyedupsert" => "KeyedUpsert",
            "conditional-keyed-upsert" or "conditionalkeyedupsert" => "ConditionalKeyedUpsert",
            "operational-append" or "operationalappend" => "OperationalAppend",
            "unclassified" => "Unclassified",
            _ => throw new InvalidOperationException($"Unsupported write effect '{value}'.")
        };
    }

    private static string NormalizeLockBehavior(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "allow" or "allow-concurrent" or "allowconcurrent" or "parallel" => "AllowConcurrent",
            "serialize" or "serialized" => "Serialize",
            _ => throw new InvalidOperationException($"Unsupported lock behavior '{value}'.")
        };
    }

    private static string NormalizePolicyKind(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "concurrent-append-policy" or "concurrentappendpolicy" or "concurrent-append" or "concurrentappend" => "ConcurrentAppendPolicy",
            "keyed-upsert-policy" or "keyedupsertpolicy" or "keyed-upsert" or "keyedupsert" => "KeyedUpsertPolicy",
            "lock-compatibility-policy" or "lockcompatibilitypolicy" or "lock-compatibility" or "lockcompatibility" => "LockCompatibilityPolicy",
            _ => "LockCompatibilityPolicy"
        };
    }

    private static string NormalizeDependencyCondition(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "success" or "succeeded" or "on-success" or "onsuccess" => "OnSuccess",
            "failure" or "failed" or "on-failure" or "onfailure" => "OnFailure",
            _ => throw new InvalidOperationException($"Unsupported dependency condition '{value}'. Expected success or failure.")
        };
    }

    private static bool CanAllowConcurrent(string leftEffect, string rightEffect)
    {
        return string.Equals(leftEffect, "Append", StringComparison.OrdinalIgnoreCase) &&
               string.Equals(rightEffect, "Append", StringComparison.OrdinalIgnoreCase);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsActive(string value) =>
        string.Equals(value, "Active", StringComparison.OrdinalIgnoreCase);

    private static bool IsTrue(string value) =>
        string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

    private static int ParseOrdinal(string value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ordinal)
            ? ordinal
            : int.MaxValue;

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

    private sealed record TaskLockRequest(MO.DataObject DataObject, MO.TaskObjectEffect TaskObjectEffect, string LockMode);
}

public sealed record OrchestrationPolicyResult(string Id, string PolicyType, string Action);

public sealed record OrchestrationRunPlanResult(
    string RunPlanId,
    string Name,
    string Status,
    int PlannedTasks);
