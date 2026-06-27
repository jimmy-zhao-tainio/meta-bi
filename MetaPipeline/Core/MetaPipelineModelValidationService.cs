namespace MetaPipeline;

public sealed class MetaPipelineModelValidationService
{
    public MetaPipelineModelValidationResult ValidatePipeline(
        MetaPipelineModel model,
        string pipelineName)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(pipelineName);

        var errors = new List<string>();
        var pipeline = ResolvePipeline(model, pipelineName, errors);
        if (pipeline is null)
        {
            return MetaPipelineModelValidationResult.From(errors);
        }

        var pipelineTasks = model.PipelineTaskList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal))
            .ToArray();
        if (pipelineTasks.Length == 0)
        {
            errors.Add($"Pipeline '{pipeline.Name}' has no PipelineTask rows.");
            return MetaPipelineModelValidationResult.From(errors);
        }

        ValidateTaskDependencies(model, pipeline, pipelineTasks, errors);
        ValidateTaskDetails(model, pipeline, pipelineTasks, errors);
        ValidateRowStreamLinks(model, pipeline, pipelineTasks, errors);

        return MetaPipelineModelValidationResult.From(errors);
    }

    private static Pipeline? ResolvePipeline(
        MetaPipelineModel model,
        string pipelineName,
        ICollection<string> errors)
    {
        var matches = model.PipelineList
            .Where(item => string.Equals(item.Name, pipelineName.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (matches.Length == 0)
        {
            errors.Add($"Pipeline '{pipelineName}' was not found.");
            return null;
        }

        if (matches.Length > 1)
        {
            errors.Add($"Pipeline name '{pipelineName}' is ambiguous.");
            return null;
        }

        return matches[0];
    }

    private static void ValidateTaskDependencies(
        MetaPipelineModel model,
        Pipeline pipeline,
        IReadOnlyList<PipelineTask> pipelineTasks,
        ICollection<string> errors)
    {
        var taskIds = pipelineTasks
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var dependency in model.TaskDependencyList.Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal)))
        {
            if (!taskIds.Contains(dependency.Predecessor.Id))
            {
                errors.Add($"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' references missing predecessor '{dependency.Predecessor.Id}'.");
            }

            if (!taskIds.Contains(dependency.Successor.Id))
            {
                errors.Add($"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' references missing successor '{dependency.Successor.Id}'.");
            }

            if (string.Equals(dependency.Predecessor.Id, dependency.Successor.Id, StringComparison.Ordinal))
            {
                errors.Add($"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' points a task to itself.");
            }
        }

        ValidateSerialTaskChain(model, pipeline, pipelineTasks, errors);
    }

    private static void ValidateSerialTaskChain(
        MetaPipelineModel model,
        Pipeline pipeline,
        IReadOnlyList<PipelineTask> pipelineTasks,
        ICollection<string> errors)
    {
        if (pipelineTasks.Count <= 1)
        {
            return;
        }

        var taskIds = pipelineTasks
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);
        var tasksById = pipelineTasks.ToDictionary(static item => item.Id, StringComparer.Ordinal);
        var dependencies = model.TaskDependencyList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal)
                           && taskIds.Contains(item.Predecessor.Id)
                           && taskIds.Contains(item.Successor.Id)
                           && !string.Equals(item.Predecessor.Id, item.Successor.Id, StringComparison.Ordinal))
            .ToArray();

        if (dependencies.Length == 0)
        {
            errors.Add($"Pipeline '{pipeline.Name}' has multiple tasks but no TaskDependency rows. Serial pipelines must declare task order.");
            return;
        }

        var successorByPredecessor = new Dictionary<string, string>(StringComparer.Ordinal);
        var predecessorBySuccessor = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var dependency in dependencies)
        {
            if (!successorByPredecessor.TryAdd(dependency.Predecessor.Id, dependency.Successor.Id))
            {
                errors.Add($"Pipeline '{pipeline.Name}' task '{tasksById[dependency.Predecessor.Id].Name}' has multiple successors.");
            }

            if (!predecessorBySuccessor.TryAdd(dependency.Successor.Id, dependency.Predecessor.Id))
            {
                errors.Add($"Pipeline '{pipeline.Name}' task '{tasksById[dependency.Successor.Id].Name}' has multiple predecessors.");
            }
        }

        if (errors.Count > 0)
        {
            return;
        }

        var startTasks = pipelineTasks
            .Where(item => !predecessorBySuccessor.ContainsKey(item.Id))
            .ToArray();
        if (startTasks.Length != 1)
        {
            errors.Add($"Pipeline '{pipeline.Name}' must have exactly one first task for serial execution.");
            return;
        }

        var ordered = new List<PipelineTask>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var current = startTasks[0];
        while (true)
        {
            if (!seen.Add(current.Id))
            {
                errors.Add($"Pipeline '{pipeline.Name}' contains a cycle in TaskDependency rows.");
                return;
            }

            ordered.Add(current);
            if (!successorByPredecessor.TryGetValue(current.Id, out var successorId))
            {
                break;
            }

            current = tasksById[successorId];
        }

        if (ordered.Count != pipelineTasks.Count)
        {
            errors.Add($"Pipeline '{pipeline.Name}' TaskDependency rows do not form one connected serial chain.");
            return;
        }

    }

    private static void ValidateTaskDetails(
        MetaPipelineModel model,
        Pipeline pipeline,
        IReadOnlyList<PipelineTask> pipelineTasks,
        ICollection<string> errors)
    {
        var pipelineTaskIdSet = pipelineTasks
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);

        var executableTaskDetailCount = model.ExecutableTaskList
            .Count(item => pipelineTaskIdSet.Contains(item.PipelineTask.Id));
        var transformTaskDetailCount = model.TransformExecutionTaskList
            .Where(item => pipelineTaskIdSet.Contains(item.PipelineTask.Id))
            .Select(static item => item.PipelineTask.Id)
            .Count();
        if (executableTaskDetailCount + transformTaskDetailCount == 0)
        {
            errors.Add($"Pipeline '{pipeline.Name}' must declare at least one executable task detail row.");
        }

        foreach (var task in pipelineTasks)
        {
            var detailCount = 0;
            if (model.ExecutableTaskList.Any(item => string.Equals(item.PipelineTask.Id, task.Id, StringComparison.Ordinal)))
            {
                detailCount++;
            }

            if (model.TransformExecutionTaskList.Any(item => string.Equals(item.PipelineTask.Id, task.Id, StringComparison.Ordinal)))
            {
                detailCount++;
            }

            if (model.TargetWriteTaskList.Any(item => string.Equals(item.PipelineTask.Id, task.Id, StringComparison.Ordinal)))
            {
                detailCount++;
            }

            if (detailCount == 0)
            {
                errors.Add($"Pipeline '{pipeline.Name}' task '{task.Name}' has no supported detail row.");
            }
            else if (detailCount > 1)
            {
                errors.Add($"Pipeline '{pipeline.Name}' task '{task.Name}' maps to multiple detail kinds.");
            }
        }

        foreach (var targetWriteTask in model.TargetWriteTaskList.Where(item => pipelineTaskIdSet.Contains(item.PipelineTask.Id)))
        {
            var insertRowsDetails = model.InsertRowsTargetWriteTaskList
                .Count(item => string.Equals(item.TargetWriteTask.Id, targetWriteTask.Id, StringComparison.Ordinal));
            if (insertRowsDetails != 1)
            {
                errors.Add($"Target write task '{targetWriteTask.PipelineTask.Id}' must have exactly one detail row: InsertRowsTargetWriteTask.");
            }
        }
    }

    private static void ValidateRowStreamLinks(
        MetaPipelineModel model,
        Pipeline pipeline,
        IReadOnlyList<PipelineTask> pipelineTasks,
        ICollection<string> errors)
    {
        var pipelineTaskIds = pipelineTasks
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);
        var rowStreamsById = model.RowStreamList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal))
            .ToDictionary(static item => item.Id, StringComparer.Ordinal);

        foreach (var producer in model.RowStreamProducerList.Where(item => pipelineTaskIds.Contains(item.PipelineTask.Id)))
        {
            if (!rowStreamsById.ContainsKey(producer.RowStream.Id))
            {
                errors.Add($"RowStreamProducer '{producer.Id}' references row stream '{producer.RowStream.Id}' outside pipeline '{pipeline.Name}'.");
            }
        }

        foreach (var consumer in model.RowStreamConsumerList.Where(item => pipelineTaskIds.Contains(item.PipelineTask.Id)))
        {
            if (!rowStreamsById.ContainsKey(consumer.RowStream.Id))
            {
                errors.Add($"RowStreamConsumer '{consumer.Id}' references row stream '{consumer.RowStream.Id}' outside pipeline '{pipeline.Name}'.");
            }
        }
    }
}

public sealed record MetaPipelineModelValidationResult(
    bool IsValid,
    IReadOnlyList<string> Errors)
{
    public static MetaPipelineModelValidationResult From(IReadOnlyList<string> errors) =>
        new(errors.Count == 0, errors);
}
