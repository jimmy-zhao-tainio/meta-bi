internal static partial class Program
{
    private static MetaPipeline.Pipeline ResolvePipeline(
        MetaPipeline.MetaPipelineModel model,
        string pipelineName)
    {
        var matches = model.PipelineList
            .Where(item => string.Equals(item.Name, pipelineName.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return matches.Length switch
        {
            0 => throw new MetaPipeline.MetaPipelineConfigurationException($"Pipeline '{pipelineName}' was not found."),
            > 1 => throw new MetaPipeline.MetaPipelineConfigurationException($"Pipeline name '{pipelineName}' is ambiguous."),
            _ => matches[0],
        };
    }

    private static void EnsurePipelineNameAvailable(
        MetaPipeline.MetaPipelineModel model,
        string pipelineName)
    {
        if (model.PipelineList.Any(item =>
                string.Equals(item.Name, pipelineName.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new MetaPipeline.MetaPipelineConfigurationException(
                $"Pipeline '{pipelineName}' already exists.");
        }
    }

    private static MetaPipeline.ConnectionReference GetOrAddConnectionReference(
        MetaPipeline.MetaPipelineModel model,
        MetaPipeline.Pipeline pipeline,
        string name,
        string environmentVariableName)
    {
        var matches = model.ConnectionReferenceList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal)
                           && string.Equals(item.Name, name.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (matches.Length > 1)
        {
            throw new MetaPipeline.MetaPipelineConfigurationException(
                $"Connection reference '{name}' is ambiguous for pipeline '{pipeline.Name}'.");
        }

        if (matches.Length == 1)
        {
            var match = matches[0];
            if (!string.Equals(match.EnvironmentVariableName, environmentVariableName.Trim(), StringComparison.Ordinal))
            {
                throw new MetaPipeline.MetaPipelineConfigurationException(
                    $"Connection reference '{name}' already points to environment variable '{match.EnvironmentVariableName}'.");
            }

            return match;
        }

        var connection = new MetaPipeline.ConnectionReference
        {
            Id = ScopedId(pipeline.Id, name),
            Pipeline = pipeline,
            Name = name.Trim(),
            EnvironmentVariableName = environmentVariableName.Trim(),
        };
        model.ConnectionReferenceList.Add(connection);
        return connection;
    }

    private static string ResolveTaskLabel(
        MetaPipeline.MetaPipelineModel model,
        MetaPipeline.PipelineTask task)
    {
        if (model.ExecutableTaskList.Any(item => string.Equals(item.PipelineTask.Id, task.Id, StringComparison.Ordinal)))
        {
            return "Executable";
        }

        if (model.TransformExecutionTaskList.Any(item => string.Equals(item.PipelineTask.Id, task.Id, StringComparison.Ordinal)))
        {
            return "TransformExecution";
        }

        var targetWriteTask = model.TargetWriteTaskList.SingleOrDefault(item =>
            string.Equals(item.PipelineTask.Id, task.Id, StringComparison.Ordinal));
        if (targetWriteTask is not null)
        {
            var hasInsertRows = model.InsertRowsTargetWriteTaskList.Any(item =>
                string.Equals(item.TargetWriteTask.Id, targetWriteTask.Id, StringComparison.Ordinal));
            if (hasInsertRows)
            {
                return "TargetWrite:InsertRows";
            }

            return "TargetWrite";
        }

        return "PipelineTask";
    }

    private static void EnsureTaskNameAvailable(
        MetaPipeline.MetaPipelineModel model,
        MetaPipeline.Pipeline pipeline,
        string taskName)
    {
        if (model.PipelineTaskList.Any(item =>
                string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal)
                && string.Equals(item.Name, taskName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new MetaPipeline.MetaPipelineConfigurationException(
                $"Task '{taskName}' already exists in pipeline '{pipeline.Name}'.");
        }
    }

    private static MetaPipeline.PipelineTask? ResolveCurrentTerminalTask(
        MetaPipeline.MetaPipelineModel model,
        MetaPipeline.Pipeline pipeline)
    {
        var tasks = model.PipelineTaskList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal))
            .ToArray();
        if (tasks.Length == 0)
        {
            return null;
        }

        var predecessorTaskIds = model.TaskDependencyList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal))
            .Select(static item => item.Predecessor.Id)
            .ToHashSet(StringComparer.Ordinal);

        var terminalTasks = tasks
            .Where(item => !predecessorTaskIds.Contains(item.Id))
            .ToArray();

        return terminalTasks.Length switch
        {
            1 => terminalTasks[0],
            _ => throw new MetaPipeline.MetaPipelineConfigurationException(
                $"Pipeline '{pipeline.Name}' must have exactly one terminal task before appending a new step."),
        };
    }

    private static void AddSerialDependency(
        MetaPipeline.MetaPipelineModel model,
        MetaPipeline.Pipeline pipeline,
        MetaPipeline.PipelineTask predecessorTask,
        MetaPipeline.PipelineTask successorTask)
    {
        model.TaskDependencyList.Add(new MetaPipeline.TaskDependency
        {
            Id = ScopedId(predecessorTask.Id, "Before", successorTask.Id),
            Pipeline = pipeline,
            Predecessor = predecessorTask,
            Successor = successorTask,
        });
    }

    private static MetaPipeline.PipelineTask ResolvePipelineTask(
        MetaPipeline.MetaPipelineModel model,
        MetaPipeline.Pipeline pipeline,
        string taskName)
    {
        var matches = model.PipelineTaskList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal)
                           && string.Equals(item.Name, taskName.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return matches.Length switch
        {
            0 => throw new MetaPipeline.MetaPipelineConfigurationException(
                $"Task '{taskName}' was not found in pipeline '{pipeline.Name}'."),
            > 1 => throw new MetaPipeline.MetaPipelineConfigurationException(
                $"Task name '{taskName}' is ambiguous in pipeline '{pipeline.Name}'."),
            _ => matches[0],
        };
    }

    private static IReadOnlyList<MetaPipeline.PipelineTask> ResolveOrderedPipelineTasks(
        MetaPipeline.MetaPipelineModel model,
        MetaPipeline.Pipeline pipeline)
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
            .ToArray();
        if (dependencies.Length == 0)
        {
            throw new MetaPipeline.MetaPipelineConfigurationException(
                $"Pipeline '{pipeline.Name}' has multiple tasks but no TaskDependency rows. Serial pipelines must declare task order.");
        }

        var successorByPredecessor = new Dictionary<string, string>(StringComparer.Ordinal);
        var predecessorBySuccessor = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var dependency in dependencies)
        {
            if (!tasksById.ContainsKey(dependency.Predecessor.Id))
            {
                throw new MetaPipeline.MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' references missing predecessor '{dependency.Predecessor.Id}'.");
            }

            if (!tasksById.ContainsKey(dependency.Successor.Id))
            {
                throw new MetaPipeline.MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' references missing successor '{dependency.Successor.Id}'.");
            }

            if (string.Equals(dependency.Predecessor.Id, dependency.Successor.Id, StringComparison.Ordinal))
            {
                throw new MetaPipeline.MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' points a task to itself.");
            }

            if (!successorByPredecessor.TryAdd(dependency.Predecessor.Id, dependency.Successor.Id))
            {
                throw new MetaPipeline.MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' task '{tasksById[dependency.Predecessor.Id].Name}' has multiple successors.");
            }

            if (!predecessorBySuccessor.TryAdd(dependency.Successor.Id, dependency.Predecessor.Id))
            {
                throw new MetaPipeline.MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' task '{tasksById[dependency.Successor.Id].Name}' has multiple predecessors.");
            }
        }

        var startTasks = tasks
            .Where(item => !predecessorBySuccessor.ContainsKey(item.Id))
            .ToArray();
        if (startTasks.Length != 1)
        {
            throw new MetaPipeline.MetaPipelineConfigurationException(
                $"Pipeline '{pipeline.Name}' must have exactly one first task for serial execution.");
        }

        var ordered = new List<MetaPipeline.PipelineTask>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var current = startTasks[0];
        while (true)
        {
            if (!seen.Add(current.Id))
            {
                throw new MetaPipeline.MetaPipelineConfigurationException(
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
            throw new MetaPipeline.MetaPipelineConfigurationException(
                $"Pipeline '{pipeline.Name}' TaskDependency rows do not form one connected serial chain.");
        }

        return ordered;
    }


    private static string NaturalId(string name)
    {
        var id = name.Trim();
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new MetaPipeline.MetaPipelineConfigurationException("Instance id cannot be empty.");
        }

        return id;
    }

    private static string ScopedId(params string[] parts)
    {
        return string.Join(".", parts.Select(NaturalId));
    }
}
