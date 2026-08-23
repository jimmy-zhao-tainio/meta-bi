using MO = MetaOrchestration;

namespace MetaOrchestration.Core.Runtime;

internal static class RuntimeDefinitionFactory
{
    public static RuntimeDefinition Create(
        MO.MetaOrchestrationModel model,
        MO.RunPlan runPlan,
        IReadOnlyList<MO.PlannedTask> plannedTasks,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]> locksByPlannedTaskId,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependenciesByTaskProfileId,
        ResolvedOrchestrationRetryPolicy retryPolicy)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(runPlan);
        ArgumentNullException.ThrowIfNull(plannedTasks);
        ArgumentNullException.ThrowIfNull(locksByPlannedTaskId);
        ArgumentNullException.ThrowIfNull(dependenciesByTaskProfileId);
        ArgumentNullException.ThrowIfNull(retryPolicy);

        var tasksByProfileId = plannedTasks
            .GroupBy(static item => item.TaskAccessProfile.Id, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group.OrderBy(static item => item.Id, StringComparer.Ordinal).First(),
                StringComparer.Ordinal);
        var runtimeTasksById = plannedTasks.ToDictionary(
            static item => item.TaskAccessProfile.MetaPipelinePipelineTaskId,
            item => CreateTask(item, locksByPlannedTaskId),
            StringComparer.Ordinal);
        var pipelines = plannedTasks
            .GroupBy(static item => item.PipelineReference.Name, StringComparer.OrdinalIgnoreCase)
            .OrderBy(static item => item.Key, StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var first = group.First();
                return new RuntimePipelineDefinition(
                    first.PipelineReference.Name,
                    first.PipelineReference.MetaPipelinePipelineId,
                    group
                        .OrderBy(static item => ParseOrdinal(item.Ordinal))
                        .ThenBy(static item => item.Id, StringComparer.Ordinal)
                        .Select(item => runtimeTasksById[item.TaskAccessProfile.MetaPipelinePipelineTaskId])
                        .ToArray());
            })
            .ToArray();
        var dependencies = dependenciesByTaskProfileId
            .SelectMany(item => item.Value.Select(dependency => CreateDependency(
                item.Key,
                dependency,
                tasksByProfileId,
                plannedTasks)))
            .ToArray();
        var lockPolicies = model.LockCompatibilityPolicyList
            .Where(static item => string.Equals(item.Status, "Active", StringComparison.OrdinalIgnoreCase))
            .Select(static item => new RuntimeLockCompatibilityPolicy(item.LeftEffect, item.RightEffect))
            .ToArray();

        return new RuntimeDefinition(
            pipelines,
            new RuntimeRetryPolicy(
                retryPolicy.MaxAttempts,
                TimeSpan.FromMilliseconds(Math.Max(0, retryPolicy.InitialDelayMilliseconds)),
                retryPolicy.RetryReadOnlyTasksByDefault,
                retryPolicy.RetryWriteTasksByDefault,
                retryPolicy.RetryableFailureClasses.ToHashSet(StringComparer.OrdinalIgnoreCase)),
            dependencies,
            lockPolicies);
    }

    private static RuntimeTaskDefinition CreateTask(
        MO.PlannedTask plannedTask,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]> locksByPlannedTaskId)
    {
        var taskId = plannedTask.TaskAccessProfile.MetaPipelinePipelineTaskId;
        if (string.IsNullOrWhiteSpace(taskId))
        {
            throw new InvalidOperationException($"Planned task '{plannedTask.Id}' has no MetaPipelinePipelineTaskId.");
        }

        var lockRequests = locksByPlannedTaskId.TryGetValue(plannedTask.Id, out var locks)
            ? locks
                .Select(lockItem => new RuntimeLockRequest(
                    string.IsNullOrWhiteSpace(lockItem.DataObject.NormalizedKey)
                        ? lockItem.DataObject.Id
                        : lockItem.DataObject.NormalizedKey,
                    lockItem.LockMode,
                    plannedTask.Id))
                .ToArray()
            : [];

        return new RuntimeTaskDefinition(
            taskId,
            plannedTask.TaskAccessProfile.TaskName,
            plannedTask.PipelineReference.Name,
            plannedTask.PipelineReference.MetaPipelinePipelineId,
            plannedTask.Id,
            plannedTask.TaskAccessProfile.Id,
            lockRequests);
    }

    private static RuntimeDependency CreateDependency(
        string successorProfileId,
        OrchestrationExecutionDependency dependency,
        IReadOnlyDictionary<string, MO.PlannedTask> tasksByProfileId,
        IReadOnlyList<MO.PlannedTask> plannedTasks)
    {
        var successor = tasksByProfileId.TryGetValue(successorProfileId, out var successorTask)
            ? successorTask
            : plannedTasks.FirstOrDefault(item => string.Equals(item.TaskAccessProfile.Id, successorProfileId, StringComparison.Ordinal));
        if (successor is null)
        {
            throw new InvalidOperationException($"Dependency successor task profile '{successorProfileId}' is not part of the run plan.");
        }

        tasksByProfileId.TryGetValue(dependency.PredecessorTaskProfileId, out var predecessor);
        return new RuntimeDependency(
            successor.TaskAccessProfile.MetaPipelinePipelineTaskId,
            successor.TaskAccessProfile.Id,
            dependency.PredecessorTaskProfileId,
            predecessor?.PipelineReference.Name ?? string.Empty,
            predecessor?.TaskAccessProfile.TaskName ?? string.Empty,
            dependency.Condition,
            string.Empty,
            dependency.Reason ?? string.Empty);
    }

    private static int ParseOrdinal(string ordinal) =>
        int.TryParse(ordinal, out var parsed) ? parsed : int.MaxValue;
}
