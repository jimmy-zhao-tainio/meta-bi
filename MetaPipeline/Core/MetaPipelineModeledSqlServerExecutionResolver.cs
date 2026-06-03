namespace MetaPipeline;

public sealed class MetaPipelineModeledSqlServerExecutionResolver
{
    public MetaPipelineModeledSqlServerExecutionPlan Resolve(
        MetaPipelineModeledSqlServerExecutionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TransformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.BindingWorkspacePath);

        var pipelineWorkspacePath = Path.GetFullPath(request.PipelineWorkspacePath);
        var transformWorkspacePath = Path.GetFullPath(request.TransformWorkspacePath);
        var bindingWorkspacePath = Path.GetFullPath(request.BindingWorkspacePath);
        var model = MetaPipelineModel.LoadFromXmlWorkspace(pipelineWorkspacePath, searchUpward: false);
        var pipeline = ResolvePipeline(model, request.PipelineName);
        var orderedTasks = ResolveSerialTaskOrder(model, pipeline);
        var steps = new List<MetaPipelineModeledSqlServerExecutionStep>();
        var workspaceResolver = new MetaPipelineExecutionWorkspaceResolver();

        for (var index = 0; index < orderedTasks.Count; index++)
        {
            var transformTask = orderedTasks[index];
            var transformExecution = ResolveTransformExecutionTask(model, transformTask);
            if (transformExecution is null)
            {
                if (TryResolveTargetWritePlan(model, pipeline, transformTask) is not null)
                {
                    throw new MetaPipelineConfigurationException(
                        $"Target write task '{transformTask.Name}' is not immediately preceded by the SELECT-kind transform it materializes.");
                }

                throw new MetaPipelineConfigurationException(
                    $"Pipeline task '{transformTask.Name}' has no supported execution detail.");
            }

            var transformScriptId = RequireValue(
                transformExecution.TransformScriptId,
                $"Transform task '{transformTask.Name}' must name a transform script id.");
            var transformBindingId = RequireValue(
                transformExecution.TransformBindingId,
                $"Transform task '{transformTask.Name}' must name a transform binding id.");
            var executionConnection = ResolveConnectionReference(
                model,
                pipeline,
                transformExecution.ExecutionConnectionReference.Id,
                "execution",
                transformTask.Name);
            var timeoutSeconds = ResolveTimeoutSeconds(transformTask.Name, transformExecution.TimeoutSeconds);

            var nextTask = index + 1 < orderedTasks.Count ? orderedTasks[index + 1] : null;
            var targetWritePlan = nextTask is null
                ? null
                : TryResolveTargetWritePlan(model, pipeline, nextTask);
            var executionDefinition = workspaceResolver.ResolveByIds(
                transformWorkspacePath,
                bindingWorkspacePath,
                transformScriptId,
                transformBindingId,
                targetWritePlan?.TargetSqlIdentifier);

            if (!executionDefinition.IsSelect)
            {
                if (targetWritePlan is not null)
                {
                    throw new MetaPipelineConfigurationException(
                        $"Transform script '{executionDefinition.TransformScriptName}' is not SELECT-kind and cannot feed an InsertRows target write.");
                }

                steps.Add(new MetaPipelineModeledSqlServerExecutionStep(
                    transformTask.Id,
                    transformTask.Name,
                    null,
                    null,
                    executionDefinition.TransformScriptId,
                    executionDefinition.TransformBindingId,
                    executionDefinition.TransformScriptName,
                    executionConnection.Name,
                    RequireValue(executionConnection.EnvironmentVariableName, $"Execution connection reference '{executionConnection.Name}' must name an environment variable."),
                    null,
                    null,
                    IsSelect: false,
                    null,
                    "None",
                    0,
                    timeoutSeconds,
                    "SqlServer"));
                continue;
            }

            if (targetWritePlan is null)
            {
                throw new MetaPipelineConfigurationException(
                    $"Transform script '{executionDefinition.TransformScriptName}' is SELECT-kind and must feed exactly one InsertRows target write.");
            }

            var targetConnection = ResolveConnectionReference(
                model,
                pipeline,
                targetWritePlan.TargetWriteTask.TargetConnectionReference.Id,
                "target",
                targetWritePlan.TargetWritePipelineTask.Name);

            var rowStream = EnsureSingleSharedRowStream(model, transformTask, targetWritePlan.TargetWritePipelineTask);
            EnsureModeledRowStreamShapeMatchesResolvedShape(
                model,
                rowStream,
                executionDefinition.RowStreamShape ?? throw new MetaPipelineConfigurationException(
                    $"Transform script '{executionDefinition.TransformScriptName}' is SELECT-kind but did not resolve a row-stream shape."),
                transformTask.Name,
                targetWritePlan.TargetWritePipelineTask.Name);

            steps.Add(new MetaPipelineModeledSqlServerExecutionStep(
                transformTask.Id,
                transformTask.Name,
                targetWritePlan.TargetWritePipelineTask.Id,
                targetWritePlan.TargetWritePipelineTask.Name,
                executionDefinition.TransformScriptId,
                executionDefinition.TransformBindingId,
                executionDefinition.TransformScriptName,
                executionConnection.Name,
                RequireValue(executionConnection.EnvironmentVariableName, $"Execution connection reference '{executionConnection.Name}' must name an environment variable."),
                targetConnection.Name,
                RequireValue(targetConnection.EnvironmentVariableName, $"Target connection reference '{targetConnection.Name}' must name an environment variable."),
                IsSelect: true,
                executionDefinition.TargetSqlIdentifier,
                targetWritePlan.TargetWriteModelName,
                targetWritePlan.BatchSize,
                timeoutSeconds,
                targetWritePlan.TargetDataTypeSystemName));
            index++;
        }

        if (steps.Count == 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Pipeline '{pipeline.Name}' must declare at least one TransformExecution task.");
        }

        return new MetaPipelineModeledSqlServerExecutionPlan(
            pipelineWorkspacePath,
            pipeline.Id,
            pipeline.Name,
            transformWorkspacePath,
            bindingWorkspacePath,
            steps);
    }

    public MetaPipelineModeledSqlServerExecutionPlan ResolveStep(
        MetaPipelineModeledSqlServerExecutionStepRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.StepName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TransformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.BindingWorkspacePath);

        var pipelineWorkspacePath = Path.GetFullPath(request.PipelineWorkspacePath);
        var transformWorkspacePath = Path.GetFullPath(request.TransformWorkspacePath);
        var bindingWorkspacePath = Path.GetFullPath(request.BindingWorkspacePath);
        var model = MetaPipelineModel.LoadFromXmlWorkspace(pipelineWorkspacePath, searchUpward: false);
        var pipeline = ResolvePipeline(model, request.PipelineName);
        var transformTask = ResolveTransformPipelineTask(model, pipeline, request.StepName);
        var transformExecution = ResolveTransformExecutionTask(model, transformTask)
            ?? throw new MetaPipelineConfigurationException(
                $"Pipeline task '{transformTask.Name}' is not a transform execution step.");

        var step = ResolveTransformExecutionStep(
            model,
            pipeline,
            transformTask,
            transformExecution,
            transformWorkspacePath,
            bindingWorkspacePath);

        return new MetaPipelineModeledSqlServerExecutionPlan(
            pipelineWorkspacePath,
            pipeline.Id,
            pipeline.Name,
            transformWorkspacePath,
            bindingWorkspacePath,
            [step]);
    }

    private static MetaPipelineModeledSqlServerExecutionStep ResolveTransformExecutionStep(
        MetaPipelineModel model,
        Pipeline pipeline,
        PipelineTask transformTask,
        TransformExecutionTask transformExecution,
        string transformWorkspacePath,
        string bindingWorkspacePath)
    {
        var transformScriptId = RequireValue(
            transformExecution.TransformScriptId,
            $"Transform task '{transformTask.Name}' must name a transform script id.");
        var transformBindingId = RequireValue(
            transformExecution.TransformBindingId,
            $"Transform task '{transformTask.Name}' must name a transform binding id.");
        var executionConnection = ResolveConnectionReference(
            model,
            pipeline,
            transformExecution.ExecutionConnectionReference.Id,
            "execution",
            transformTask.Name);
        var timeoutSeconds = ResolveTimeoutSeconds(transformTask.Name, transformExecution.TimeoutSeconds);
        var targetWritePlan = TryResolvePairedTargetWritePlan(model, pipeline, transformTask);
        var executionDefinition = new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
            transformWorkspacePath,
            bindingWorkspacePath,
            transformScriptId,
            transformBindingId,
            targetWritePlan?.TargetSqlIdentifier);

        if (!executionDefinition.IsSelect)
        {
            if (targetWritePlan is not null)
            {
                throw new MetaPipelineConfigurationException(
                    $"Transform script '{executionDefinition.TransformScriptName}' is not SELECT-kind and cannot feed an InsertRows target write.");
            }

            return new MetaPipelineModeledSqlServerExecutionStep(
                transformTask.Id,
                transformTask.Name,
                null,
                null,
                executionDefinition.TransformScriptId,
                executionDefinition.TransformBindingId,
                executionDefinition.TransformScriptName,
                executionConnection.Name,
                RequireValue(executionConnection.EnvironmentVariableName, $"Execution connection reference '{executionConnection.Name}' must name an environment variable."),
                null,
                null,
                IsSelect: false,
                null,
                "None",
                0,
                timeoutSeconds,
                "SqlServer");
        }

        if (targetWritePlan is null)
        {
            throw new MetaPipelineConfigurationException(
                $"Transform script '{executionDefinition.TransformScriptName}' is SELECT-kind and must feed exactly one InsertRows target write.");
        }

        var targetConnection = ResolveConnectionReference(
            model,
            pipeline,
            targetWritePlan.TargetWriteTask.TargetConnectionReference.Id,
            "target",
            targetWritePlan.TargetWritePipelineTask.Name);

        var rowStream = EnsureSingleSharedRowStream(model, transformTask, targetWritePlan.TargetWritePipelineTask);
        EnsureModeledRowStreamShapeMatchesResolvedShape(
            model,
            rowStream,
            executionDefinition.RowStreamShape ?? throw new MetaPipelineConfigurationException(
                $"Transform script '{executionDefinition.TransformScriptName}' is SELECT-kind but did not resolve a row-stream shape."),
            transformTask.Name,
            targetWritePlan.TargetWritePipelineTask.Name);

        return new MetaPipelineModeledSqlServerExecutionStep(
            transformTask.Id,
            transformTask.Name,
            targetWritePlan.TargetWritePipelineTask.Id,
            targetWritePlan.TargetWritePipelineTask.Name,
            executionDefinition.TransformScriptId,
            executionDefinition.TransformBindingId,
            executionDefinition.TransformScriptName,
            executionConnection.Name,
            RequireValue(executionConnection.EnvironmentVariableName, $"Execution connection reference '{executionConnection.Name}' must name an environment variable."),
            targetConnection.Name,
            RequireValue(targetConnection.EnvironmentVariableName, $"Target connection reference '{targetConnection.Name}' must name an environment variable."),
            IsSelect: true,
            executionDefinition.TargetSqlIdentifier,
            targetWritePlan.TargetWriteModelName,
            targetWritePlan.BatchSize,
            timeoutSeconds,
            targetWritePlan.TargetDataTypeSystemName);
    }

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

    private static IReadOnlyList<PipelineTask> ResolveSerialTaskOrder(
        MetaPipelineModel model,
        Pipeline pipeline)
    {
        var tasks = model.PipelineTaskList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal))
            .ToArray();
        if (tasks.Length == 0)
        {
            throw new MetaPipelineConfigurationException($"Pipeline '{pipeline.Name}' has no PipelineTask rows.");
        }

        if (tasks.Length == 1)
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
            var predecessorId = dependency.Predecessor.Id;
            var successorId = dependency.Successor.Id;
            if (!tasksById.ContainsKey(predecessorId))
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' references missing predecessor '{predecessorId}'.");
            }

            if (!tasksById.ContainsKey(successorId))
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' references missing successor '{successorId}'.");
            }

            if (string.Equals(predecessorId, successorId, StringComparison.Ordinal))
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' dependency '{dependency.Id}' points a task to itself.");
            }

            if (!successorByPredecessor.TryAdd(predecessorId, successorId))
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' task '{tasksById[predecessorId].Name}' has multiple successors. Serial execution requires one successor at most.");
            }

            if (!predecessorBySuccessor.TryAdd(successorId, predecessorId))
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' task '{tasksById[successorId].Name}' has multiple predecessors. Serial execution requires one predecessor at most.");
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

        for (var index = 1; index < ordered.Count; index++)
        {
            var previousOrdinal = ParseNonNegativeOrdinal(
                ordered[index - 1].Ordinal,
                $"Pipeline '{pipeline.Name}' task '{ordered[index - 1].Name}' contains invalid Ordinal '{ordered[index - 1].Ordinal}'.");
            var currentOrdinal = ParseNonNegativeOrdinal(
                ordered[index].Ordinal,
                $"Pipeline '{pipeline.Name}' task '{ordered[index].Name}' contains invalid Ordinal '{ordered[index].Ordinal}'.");
            if (currentOrdinal <= previousOrdinal)
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline '{pipeline.Name}' serial task order must follow increasing Ordinal values.");
            }
        }

        return ordered;
    }

    private static PipelineTask ResolveTransformPipelineTask(
        MetaPipelineModel model,
        Pipeline pipeline,
        string stepName)
    {
        var trimmed = stepName.Trim();
        var matches = model.PipelineTaskList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal))
            .Where(item =>
                string.Equals(item.Id, trimmed, StringComparison.Ordinal) ||
                string.Equals(item.Name, trimmed, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return matches.Length switch
        {
            0 => throw new MetaPipelineConfigurationException($"Pipeline '{pipeline.Name}' step '{stepName}' was not found."),
            > 1 => throw new MetaPipelineConfigurationException($"Pipeline '{pipeline.Name}' step selector '{stepName}' is ambiguous."),
            _ => matches[0],
        };
    }

    private static TransformExecutionTask? ResolveTransformExecutionTask(
        MetaPipelineModel model,
        PipelineTask task)
    {
        var matches = model.TransformExecutionTaskList
            .Where(item => string.Equals(item.PipelineTask.Id, task.Id, StringComparison.Ordinal))
            .ToArray();

        return matches.Length switch
        {
            0 => null,
            1 => matches[0],
            _ => throw new MetaPipelineConfigurationException(
                $"Pipeline task '{task.Name}' has multiple TransformExecutionTask detail rows."),
        };
    }

    private static ConnectionReference ResolveConnectionReference(
        MetaPipelineModel model,
        Pipeline pipeline,
        string connectionReferenceId,
        string role,
        string taskName)
    {
        var connectionReference = model.ConnectionReferenceList.SingleOrDefault(item =>
            string.Equals(item.Id, connectionReferenceId, StringComparison.Ordinal));
        if (connectionReference is null)
        {
            throw new MetaPipelineConfigurationException(
                $"Task '{taskName}' references a missing {role} connection reference.");
        }

        if (!string.Equals(connectionReference.Pipeline.Id, pipeline.Id, StringComparison.Ordinal))
        {
            throw new MetaPipelineConfigurationException(
                $"Task '{taskName}' references a {role} connection from another pipeline.");
        }

        return connectionReference;
    }

    private static ResolvedTargetWritePlan? TryResolveTargetWritePlan(
        MetaPipelineModel model,
        Pipeline pipeline,
        PipelineTask targetWritePipelineTask)
    {
        var targetWriteDetails = model.TargetWriteTaskList
            .Where(item => string.Equals(item.PipelineTask.Id, targetWritePipelineTask.Id, StringComparison.Ordinal))
            .ToArray();

        if (targetWriteDetails.Length == 0)
        {
            return null;
        }

        var targetWrite = targetWriteDetails.Length switch
        {
            > 1 => throw new MetaPipelineConfigurationException(
                $"Pipeline task '{targetWritePipelineTask.Name}' has multiple TargetWriteTask detail rows."),
            _ => targetWriteDetails[0],
        };

        if (!string.Equals(targetWritePipelineTask.Pipeline.Id, pipeline.Id, StringComparison.Ordinal))
        {
            throw new MetaPipelineConfigurationException(
                $"Target write detail '{targetWrite.Id}' points to a task outside pipeline '{pipeline.Name}'.");
        }

        var insertRowsDetails = model.InsertRowsTargetWriteTaskList
            .Where(item => string.Equals(item.TargetWriteTask.Id, targetWrite.Id, StringComparison.Ordinal))
            .ToArray();
        if (insertRowsDetails.Length == 1)
        {
            var insertRows = insertRowsDetails[0];
            return new ResolvedTargetWritePlan(
                targetWritePipelineTask,
                targetWrite,
                "InsertRows",
                RequireValue(insertRows.TargetSqlIdentifier, $"Target write task '{targetWritePipelineTask.Name}' must name a target SQL identifier."),
                ResolveBatchSize(targetWritePipelineTask.Name, insertRows.BatchSize),
                ResolveTargetDataTypeSystemName(insertRows.TargetDataTypeSystemName));
        }

        throw new MetaPipelineConfigurationException(
            $"Target write task '{targetWritePipelineTask.Name}' must declare exactly one supported detail row: InsertRowsTargetWriteTask.");
    }

    private static ResolvedTargetWritePlan? TryResolvePairedTargetWritePlan(
        MetaPipelineModel model,
        Pipeline pipeline,
        PipelineTask transformTask)
    {
        var successorIds = model.TaskDependencyList
            .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal))
            .Where(item => string.Equals(item.Predecessor.Id, transformTask.Id, StringComparison.Ordinal))
            .Select(static item => item.Successor.Id)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var targetWritePlans = successorIds
            .Select(successorId => model.PipelineTaskList.SingleOrDefault(item => string.Equals(item.Id, successorId, StringComparison.Ordinal)))
            .Where(static item => item is not null)
            .Select(item => TryResolveTargetWritePlan(model, pipeline, item!))
            .Where(static item => item is not null)
            .Cast<ResolvedTargetWritePlan>()
            .ToArray();

        return targetWritePlans.Length switch
        {
            0 => null,
            1 => targetWritePlans[0],
            _ => throw new MetaPipelineConfigurationException(
                $"Transform task '{transformTask.Name}' feeds multiple target write tasks. Execute-step requires one paired target write at most.")
        };
    }

    private static RowStream EnsureSingleSharedRowStream(
        MetaPipelineModel model,
        PipelineTask transformTask,
        PipelineTask targetWriteTask)
    {
        var producer = ResolveSingle(
            model.RowStreamProducerList.Where(item => string.Equals(item.PipelineTask.Id, transformTask.Id, StringComparison.Ordinal)),
            $"Transform task '{transformTask.Name}' must produce exactly one row stream.");
        var consumer = ResolveSingle(
            model.RowStreamConsumerList.Where(item => string.Equals(item.PipelineTask.Id, targetWriteTask.Id, StringComparison.Ordinal)),
            $"Target write task '{targetWriteTask.Name}' must consume exactly one row stream.");

        if (!string.Equals(producer.RowStream.Id, consumer.RowStream.Id, StringComparison.Ordinal))
        {
            throw new MetaPipelineConfigurationException(
                $"Transform task '{transformTask.Name}' and target write task '{targetWriteTask.Name}' must share the same row stream.");
        }

        var rowStream = model.RowStreamList.SingleOrDefault(item => string.Equals(item.Id, producer.RowStream.Id, StringComparison.Ordinal));
        if (rowStream is null)
        {
            throw new MetaPipelineConfigurationException(
                $"Row stream '{producer.RowStream.Id}' referenced by task '{transformTask.Name}' was not found.");
        }

        return rowStream;
    }

    private static void EnsureModeledRowStreamShapeMatchesResolvedShape(
        MetaPipelineModel model,
        RowStream rowStream,
        PipelineRowStreamShape resolvedShape,
        string transformTaskName,
        string targetWriteTaskName)
    {
        var rowStreamName = RequireValue(
            rowStream.Name,
            $"Row stream '{rowStream.Id}' must have a Name.");
        var modeledColumns = model.RowStreamColumnList
            .Where(item => string.Equals(item.RowStream.Id, rowStream.Id, StringComparison.Ordinal))
            .Select(item => new
            {
                Name = RequireValue(
                    item.Name,
                    $"Row stream '{rowStreamName}' contains a column with blank Name."),
                Ordinal = ParseNonNegativeOrdinal(
                    item.Ordinal,
                    $"Row stream '{rowStreamName}' column '{item.Name}' contains invalid Ordinal '{item.Ordinal}'.")
            })
            .OrderBy(item => item.Ordinal)
            .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (modeledColumns.Length == 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Row stream '{rowStreamName}' has no RowStreamColumn rows.");
        }

        var resolvedColumns = resolvedShape.Columns
            .Select(item => new
            {
                Name = RequireValue(
                    item.Name,
                    $"Resolved row stream shape for transform task '{transformTaskName}' contains a blank column name."),
                item.Ordinal,
            })
            .OrderBy(item => item.Ordinal)
            .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var shapeMatches = modeledColumns.Length == resolvedColumns.Length;
        if (shapeMatches)
        {
            for (var index = 0; index < modeledColumns.Length; index++)
            {
                if (modeledColumns[index].Ordinal != resolvedColumns[index].Ordinal
                    || !string.Equals(modeledColumns[index].Name, resolvedColumns[index].Name, StringComparison.OrdinalIgnoreCase))
                {
                    shapeMatches = false;
                    break;
                }
            }
        }

        if (!shapeMatches)
        {
            var modeled = RenderColumnShape(
                modeledColumns.Select(item => (item.Ordinal, item.Name)));
            var resolved = RenderColumnShape(
                resolvedColumns.Select(item => (item.Ordinal, item.Name)));
            throw new MetaPipelineConfigurationException(
                $"Row stream '{rowStreamName}' for transform task '{transformTaskName}' and target write task '{targetWriteTaskName}' no longer matches the resolved binding output shape. Modeled: {modeled}. Resolved: {resolved}. Re-run add-step to refresh the modeled row stream shape.");
        }
    }

    private static int ResolveBatchSize(
        string targetWriteTaskName,
        string? configuredBatchSize)
    {
        if (string.IsNullOrWhiteSpace(configuredBatchSize))
        {
            return 1000;
        }

        if (!int.TryParse(configuredBatchSize, out var batchSize) || batchSize <= 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Target write task '{targetWriteTaskName}' has invalid BatchSize '{configuredBatchSize}'. Expected a positive integer.");
        }

        return batchSize;
    }

    private static string ResolveTargetDataTypeSystemName(string? configuredName) =>
        string.IsNullOrWhiteSpace(configuredName)
            ? "SqlServer"
            : configuredName.Trim();

    private static int? ResolveTimeoutSeconds(
        string transformTaskName,
        string? configuredTimeoutSeconds)
    {
        if (string.IsNullOrWhiteSpace(configuredTimeoutSeconds))
        {
            return null;
        }

        if (!int.TryParse(configuredTimeoutSeconds, out var timeoutSeconds) || timeoutSeconds < 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Transform task '{transformTaskName}' has invalid TimeoutSeconds '{configuredTimeoutSeconds}'. Expected a non-negative integer; 0 means no timeout.");
        }

        return timeoutSeconds;
    }

    private static string RenderColumnShape(IEnumerable<(int Ordinal, string Name)> columns) =>
        string.Join(", ", columns.Select(item => item.Ordinal.ToString() + ":" + item.Name));

    private static int ParseNonNegativeOrdinal(string value, string errorMessage)
    {
        if (!int.TryParse(value, out var ordinal) || ordinal < 0)
        {
            throw new MetaPipelineConfigurationException(errorMessage);
        }

        return ordinal;
    }

    private static T ResolveSingle<T>(
        IEnumerable<T> rows,
        string errorMessage)
    {
        var matches = rows.ToArray();
        return matches.Length == 1
            ? matches[0]
            : throw new MetaPipelineConfigurationException(errorMessage);
    }

    private static string RequireValue(string? value, string errorMessage) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new MetaPipelineConfigurationException(errorMessage)
            : value.Trim();

    private sealed record ResolvedTargetWritePlan(
        PipelineTask TargetWritePipelineTask,
        TargetWriteTask TargetWriteTask,
        string TargetWriteModelName,
        string TargetSqlIdentifier,
        int BatchSize,
        string TargetDataTypeSystemName);
}
