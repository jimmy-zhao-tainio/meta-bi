namespace MetaPipeline;

public sealed class MetaPipelineWorkspaceService
{
    public MetaPipelineModel CreateWorkspace() => MetaPipelineModel.CreateEmpty();

    public void AddPipeline(
        MetaPipelineModel model,
        string name,
        string description)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        EnsurePipelineNameAvailable(model, name);

        var pipeline = new Pipeline
        {
            Id = NaturalId(name),
            Name = name.Trim(),
            Description = description.Trim(),
        };
        model.PipelineList.Add(pipeline);
    }

    public MetaPipelineWorkspaceInspectionResult Inspect(MetaPipelineModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var pipelines = model.PipelineList
            .OrderBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
            .Select(pipeline =>
            {
                var tasks = ResolveOrderedPipelineTasks(model, pipeline)
                    .Select(task => new MetaPipelineTaskInspectionResult(
                        task.Name,
                        ResolveTaskLabel(model, task)))
                    .ToArray();

                return new MetaPipelinePipelineInspectionResult(
                    pipeline.Id,
                    pipeline.Name,
                    tasks);
            })
            .ToArray();

        return new MetaPipelineWorkspaceInspectionResult(
            model.PipelineList.Count,
            model.PipelineTaskList.Count,
            model.ConnectionReferenceList.Count,
            model.RowStreamList.Count,
            model.RowStreamColumnList.Count,
            model.TaskDependencyList.Count,
            pipelines);
    }

    public void AddExecutableStep(
        MetaPipelineModel model,
        MetaPipelineAddExecutableStepRequest request)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(request);

        var pipeline = ResolvePipeline(model, request.PipelineName);
        var taskName = ResolveExecutableStepName(request.StepName, request.ExecutablePath);
        EnsureTaskNameAvailable(model, pipeline, taskName);
        var taskId = ScopedId(pipeline.Id, taskName);
        var previousTerminalTask = ResolveCurrentTerminalTask(model, pipeline);

        var task = new PipelineTask
        {
            Id = taskId,
            Pipeline = pipeline,
            Name = taskName,
        };
        model.PipelineTaskList.Add(task);
        model.ExecutableTaskList.Add(new ExecutableTask
        {
            Id = ScopedId(taskId, "Executable"),
            PipelineTask = task,
            ExecutablePath = request.ExecutablePath.Trim(),
            Arguments = request.Arguments.Trim(),
            WorkingDirectory = request.WorkingDirectory.Trim(),
            SuccessExitCode = request.SuccessExitCodeSpecified ? request.SuccessExitCode.ToString() : null,
            TimeoutSeconds = request.TimeoutSecondsSpecified ? request.TimeoutSeconds!.Value.ToString() : null,
        });

        if (previousTerminalTask is not null)
        {
            AddSerialDependency(model, pipeline, previousTerminalTask, task);
        }

    }

    public void AddStep(
        MetaPipelineModel model,
        MetaPipelineAddStepRequest request)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(request);

        var pipeline = ResolvePipeline(model, request.PipelineName);
        var selection = new MetaPipelineTransformSelectionResolver().Resolve(
            request.TransformWorkspacePath,
            request.BindingWorkspacePath,
            request.Script,
            request.Binding);
        var executionDefinition = new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
            request.TransformWorkspacePath,
            request.BindingWorkspacePath,
            selection.TransformScriptId,
            selection.TransformBindingId,
            request.TargetSqlIdentifier);

        var executionConnection = GetOrAddConnectionReference(
            model,
            pipeline,
            request.ExecutionConnectionEnvironmentVariableName,
            request.ExecutionConnectionEnvironmentVariableName);
        var taskBaseName = ResolveStepName(request.StepName, executionDefinition.TransformScriptName);
        var transformTaskName = taskBaseName;
        EnsureTaskNameAvailable(model, pipeline, transformTaskName);
        var transformTaskId = ScopedId(pipeline.Id, transformTaskName);
        var previousTerminalTask = ResolveCurrentTerminalTask(model, pipeline);

        if (!executionDefinition.IsSelect)
        {
            if (!string.IsNullOrWhiteSpace(request.TargetConnectionEnvironmentVariableName)
                || request.TargetWriteModelSpecified
                || request.BatchSizeSpecified
                || request.TargetDataTypeSystemSpecified)
            {
                throw new MetaPipelineConfigurationException(
                    $"Transform script '{executionDefinition.TransformScriptName}' is not SELECT-kind and cannot use target connection, target write, batch-size, or target-data-type-system options.");
            }

            var mutationTransformTask = new PipelineTask
            {
                Id = transformTaskId,
                Pipeline = pipeline,
                Name = transformTaskName,
            };
            model.PipelineTaskList.Add(mutationTransformTask);
            model.TransformExecutionTaskList.Add(new TransformExecutionTask
            {
                Id = ScopedId(transformTaskId, "TransformExecution"),
                PipelineTask = mutationTransformTask,
                ExecutionConnectionReference = executionConnection,
                TransformScriptId = executionDefinition.TransformScriptId,
                TransformBindingId = executionDefinition.TransformBindingId
                    ?? throw new MetaPipelineConfigurationException("Transform execution requires a transform binding."),
                TransformWorkspacePath = Path.GetFullPath(request.TransformWorkspacePath),
                BindingWorkspacePath = Path.GetFullPath(request.BindingWorkspacePath),
                TimeoutSeconds = request.TimeoutSecondsSpecified ? request.TimeoutSeconds!.Value.ToString() : null,
            });

            if (previousTerminalTask is not null)
            {
                AddSerialDependency(model, pipeline, previousTerminalTask, mutationTransformTask);
            }

            return;
        }

        if (string.IsNullOrWhiteSpace(request.TargetConnectionEnvironmentVariableName))
        {
            throw new MetaPipelineConfigurationException(
                "SELECT-kind transform tasks require --target-connection-env <name>.");
        }

        var targetConnection = GetOrAddConnectionReference(
            model,
            pipeline,
            request.TargetConnectionEnvironmentVariableName,
            request.TargetConnectionEnvironmentVariableName);
        if (string.Equals(executionConnection.Name, targetConnection.Name, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(executionConnection.EnvironmentVariableName, targetConnection.EnvironmentVariableName, StringComparison.Ordinal))
        {
            throw new MetaPipelineConfigurationException(
                $"Connection reference '{executionConnection.Name}' cannot point to both environment variable '{executionConnection.EnvironmentVariableName}' and '{targetConnection.EnvironmentVariableName}'.");
        }

        var targetWriteTaskName = taskBaseName + ".target-write";
        EnsureTaskNameAvailable(model, pipeline, targetWriteTaskName);

        var targetWriteTaskId = ScopedId(pipeline.Id, targetWriteTaskName);
        var rowStreamName = taskBaseName + ".rows";
        var rowStreamId = ScopedId(pipeline.Id, rowStreamName);
        var targetSqlIdentifier = RequireValue(
            executionDefinition.TargetSqlIdentifier,
            "SELECT-kind transform tasks require a target SQL identifier.");
        var transformBindingId = RequireValue(
            executionDefinition.TransformBindingId,
            "SELECT-kind transform tasks require a transform binding.");
        var rowStreamShape = executionDefinition.RowStreamShape
            ?? throw new MetaPipelineConfigurationException(
                "SELECT-kind transform tasks require a resolved row-stream shape.");
        _ = SqlServerMultipartIdentifier.Parse(targetSqlIdentifier);

        var transformTask = new PipelineTask
        {
            Id = transformTaskId,
            Pipeline = pipeline,
            Name = transformTaskName,
        };
        model.PipelineTaskList.Add(transformTask);
        model.TransformExecutionTaskList.Add(new TransformExecutionTask
        {
            Id = ScopedId(transformTaskId, "TransformExecution"),
            PipelineTask = transformTask,
            ExecutionConnectionReference = executionConnection,
            TransformScriptId = executionDefinition.TransformScriptId,
            TransformBindingId = transformBindingId,
            TransformWorkspacePath = Path.GetFullPath(request.TransformWorkspacePath),
            BindingWorkspacePath = Path.GetFullPath(request.BindingWorkspacePath),
            TimeoutSeconds = request.TimeoutSecondsSpecified ? request.TimeoutSeconds!.Value.ToString() : null,
        });

        var rowStream = new RowStream
        {
            Id = rowStreamId,
            Pipeline = pipeline,
            Name = rowStreamName,
        };
        model.RowStreamList.Add(rowStream);

        foreach (var column in rowStreamShape.Columns)
        {
            model.RowStreamColumnList.Add(new RowStreamColumn
            {
                Id = ScopedId(rowStreamId, column.Name),
                RowStream = rowStream,
                Name = column.Name,
                Ordinal = column.Ordinal.ToString(),
            });
        }

        var targetWriteDetailId = ScopedId(targetWriteTaskId, "TargetWrite");
        model.RowStreamProducerList.Add(new RowStreamProducer
        {
            Id = ScopedId(transformTaskId, "Produces", rowStreamId),
            PipelineTask = transformTask,
            RowStream = rowStream,
        });

        var targetWritePipelineTask = new PipelineTask
        {
            Id = targetWriteTaskId,
            Pipeline = pipeline,
            Name = targetWriteTaskName,
        };
        model.PipelineTaskList.Add(targetWritePipelineTask);
        var targetWriteTask = new TargetWriteTask
        {
            Id = targetWriteDetailId,
            PipelineTask = targetWritePipelineTask,
            TargetConnectionReference = targetConnection,
        };
        model.TargetWriteTaskList.Add(targetWriteTask);
        model.InsertRowsTargetWriteTaskList.Add(new InsertRowsTargetWriteTask
        {
            Id = ScopedId(targetWriteDetailId, "InsertRows"),
            TargetWriteTask = targetWriteTask,
            TargetSqlIdentifier = targetSqlIdentifier,
            BatchSize = request.BatchSizeSpecified ? request.BatchSize.ToString() : null,
            TargetDataTypeSystemName = request.TargetDataTypeSystemSpecified ? request.TargetDataTypeSystemName.Trim() : null,
        });

        model.RowStreamConsumerList.Add(new RowStreamConsumer
        {
            Id = ScopedId(targetWriteTaskId, "Consumes", rowStreamId),
            PipelineTask = targetWritePipelineTask,
            RowStream = rowStream,
        });
        if (previousTerminalTask is not null)
        {
            AddSerialDependency(model, pipeline, previousTerminalTask, transformTask);
        }

        AddSerialDependency(model, pipeline, transformTask, targetWritePipelineTask);

    }

    private static string RequireValue(string? value, string errorMessage) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new MetaPipelineConfigurationException(errorMessage)
            : value.Trim();

    private static Pipeline ResolvePipeline(
        MetaPipelineModel model,
        string pipelineName)
    {
        var matches = model.PipelineList
            .Where(item => string.Equals(item.Name, pipelineName.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return matches.Length switch
        {
            0 => throw new MetaPipelineConfigurationException($"Pipeline '{pipelineName}' was not found."),
            > 1 => throw new MetaPipelineConfigurationException($"Pipeline name '{pipelineName}' is ambiguous."),
            _ => matches[0],
        };
    }

    private static void EnsurePipelineNameAvailable(
        MetaPipelineModel model,
        string pipelineName)
    {
        if (model.PipelineList.Any(item =>
                string.Equals(item.Name, pipelineName.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new MetaPipelineConfigurationException(
                $"Pipeline '{pipelineName}' already exists.");
        }
    }

    private static ConnectionReference GetOrAddConnectionReference(
        MetaPipelineModel model,
        Pipeline pipeline,
        string name,
        string environmentVariableName)
    {
        var matches = model.ConnectionReferenceList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal)
                           && string.Equals(item.Name, name.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (matches.Length > 1)
        {
            throw new MetaPipelineConfigurationException(
                $"Connection reference '{name}' is ambiguous for pipeline '{pipeline.Name}'.");
        }

        if (matches.Length == 1)
        {
            var match = matches[0];
            if (!string.Equals(match.EnvironmentVariableName, environmentVariableName.Trim(), StringComparison.Ordinal))
            {
                throw new MetaPipelineConfigurationException(
                    $"Connection reference '{name}' already points to environment variable '{match.EnvironmentVariableName}'.");
            }

            return match;
        }

        var connection = new ConnectionReference
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
        MetaPipelineModel model,
        PipelineTask task)
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
        MetaPipelineModel model,
        Pipeline pipeline,
        string taskName)
    {
        if (model.PipelineTaskList.Any(item =>
                string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal)
                && string.Equals(item.Name, taskName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new MetaPipelineConfigurationException(
                $"Task '{taskName}' already exists in pipeline '{pipeline.Name}'.");
        }
    }

    private static PipelineTask? ResolveCurrentTerminalTask(
        MetaPipelineModel model,
        Pipeline pipeline)
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
            _ => throw new MetaPipelineConfigurationException(
                $"Pipeline '{pipeline.Name}' must have exactly one terminal task before appending a new step."),
        };
    }

    private static void AddSerialDependency(
        MetaPipelineModel model,
        Pipeline pipeline,
        PipelineTask predecessorTask,
        PipelineTask successorTask)
    {
        model.TaskDependencyList.Add(new TaskDependency
        {
            Id = ScopedId(predecessorTask.Id, "Before", successorTask.Id),
            Pipeline = pipeline,
            Predecessor = predecessorTask,
            Successor = successorTask,
        });
    }

    private static IReadOnlyList<PipelineTask> ResolveOrderedPipelineTasks(
        MetaPipelineModel model,
        Pipeline pipeline)
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
            throw new MetaPipelineConfigurationException(
                $"Pipeline '{pipeline.Name}' has multiple tasks but no TaskDependency rows. Serial pipelines must declare task order.");
        }

        var successorByPredecessor = new Dictionary<string, string>(StringComparer.Ordinal);
        var predecessorBySuccessor = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var dependency in dependencies)
        {
            if (!tasksById.ContainsKey(dependency.Predecessor.Id))
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' references missing predecessor '{dependency.Predecessor.Id}'.");
            }

            if (!tasksById.ContainsKey(dependency.Successor.Id))
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' references missing successor '{dependency.Successor.Id}'.");
            }

            if (string.Equals(dependency.Predecessor.Id, dependency.Successor.Id, StringComparison.Ordinal))
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' points a task to itself.");
            }

            if (!successorByPredecessor.TryAdd(dependency.Predecessor.Id, dependency.Successor.Id))
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' task '{tasksById[dependency.Predecessor.Id].Name}' has multiple successors.");
            }

            if (!predecessorBySuccessor.TryAdd(dependency.Successor.Id, dependency.Predecessor.Id))
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' task '{tasksById[dependency.Successor.Id].Name}' has multiple predecessors.");
            }
        }

        var startTasks = tasks
            .Where(item => !predecessorBySuccessor.ContainsKey(item.Id))
            .ToArray();
        if (startTasks.Length != 1)
        {
            throw new MetaPipelineConfigurationException(
                $"Pipeline '{pipeline.Name}' must have exactly one first task for serial execution.");
        }

        var ordered = new List<PipelineTask>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var current = startTasks[0];
        while (true)
        {
            if (!seen.Add(current.Id))
            {
                throw new MetaPipelineConfigurationException(
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
            throw new MetaPipelineConfigurationException(
                $"Pipeline '{pipeline.Name}' TaskDependency rows do not form one connected serial chain.");
        }

        return ordered;
    }

    private static string ResolveStepName(string stepName, string transformScriptName)
    {
        if (!string.IsNullOrWhiteSpace(stepName))
        {
            return stepName.Trim();
        }

        var derived = DeriveStepName(transformScriptName);
        if (string.IsNullOrWhiteSpace(derived))
        {
            throw new MetaPipelineConfigurationException(
                $"Transform script '{transformScriptName}' cannot be used to derive a step name. Use --step-name <name>.");
        }

        return derived;
    }

    private static string ResolveExecutableStepName(string stepName, string executablePath)
    {
        if (!string.IsNullOrWhiteSpace(stepName))
        {
            return stepName.Trim();
        }

        var fileName = Path.GetFileNameWithoutExtension(executablePath.Trim());
        var derived = DeriveStepName(fileName);
        if (string.IsNullOrWhiteSpace(derived))
        {
            throw new MetaPipelineConfigurationException(
                $"Executable path '{executablePath}' cannot be used to derive a step name. Use --step-name <name>.");
        }

        return derived;
    }

    private static string DeriveStepName(string value)
    {
        var output = new char[value.Length];
        var length = 0;
        var previousWasSeparator = false;

        foreach (var character in value.Trim())
        {
            if (char.IsLetterOrDigit(character))
            {
                output[length++] = char.ToLowerInvariant(character);
                previousWasSeparator = false;
                continue;
            }

            if (length > 0 && !previousWasSeparator)
            {
                output[length++] = '-';
                previousWasSeparator = true;
            }
        }

        while (length > 0 && output[length - 1] == '-')
        {
            length--;
        }

        return length == 0 ? string.Empty : new string(output, 0, length);
    }

    private static string NaturalId(string name)
    {
        var id = name.Trim();
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new MetaPipelineConfigurationException("Instance id cannot be empty.");
        }

        return id;
    }

    private static string ScopedId(params string[] parts)
    {
        return string.Join(".", parts.Select(NaturalId));
    }
}

public sealed record MetaPipelineWorkspaceCreationResult(string WorkspacePath);

public sealed record MetaPipelineWorkspaceInspectionResult(
    int PipelineCount,
    int TaskCount,
    int ConnectionCount,
    int RowStreamCount,
    int RowStreamColumnCount,
    int DependencyCount,
    IReadOnlyList<MetaPipelinePipelineInspectionResult> Pipelines);

public sealed record MetaPipelinePipelineInspectionResult(
    string Id,
    string Name,
    IReadOnlyList<MetaPipelineTaskInspectionResult> Tasks);

public sealed record MetaPipelineTaskInspectionResult(
    string Name,
    string Label);

public sealed record MetaPipelineAddExecutableStepRequest(
    string PipelineName,
    string StepName,
    string ExecutablePath,
    string Arguments,
    string WorkingDirectory,
    int SuccessExitCode,
    bool SuccessExitCodeSpecified,
    int? TimeoutSeconds,
    bool TimeoutSecondsSpecified);

public sealed record MetaPipelineAddStepRequest(
    string PipelineName,
    string StepName,
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    string Script,
    string Binding,
    string ExecutionConnectionEnvironmentVariableName,
    string TargetConnectionEnvironmentVariableName,
    string? TargetSqlIdentifier,
    string TargetWriteModelName,
    bool TargetWriteModelSpecified,
    int BatchSize,
    bool BatchSizeSpecified,
    int? TimeoutSeconds,
    bool TimeoutSecondsSpecified,
    string TargetDataTypeSystemName,
    bool TargetDataTypeSystemSpecified);
