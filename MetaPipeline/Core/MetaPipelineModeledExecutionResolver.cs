namespace MetaPipeline;

public sealed class MetaPipelineModeledExecutionResolver
{
    public MetaPipelineModeledExecutionPlan Resolve(
        MetaPipelineModeledExecutionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineName);

        var pipelineWorkspacePath = Path.GetFullPath(request.PipelineWorkspacePath);
        var model = MetaPipelineModel.LoadFromXmlWorkspace(pipelineWorkspacePath, searchUpward: false);
        return Resolve(model, pipelineWorkspacePath, request.PipelineName);
    }

    public MetaPipelineModeledExecutionPlan Resolve(
        MetaPipelineModel model,
        string pipelineWorkspacePath,
        string pipelineName)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(pipelineWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(pipelineName);

        pipelineWorkspacePath = Path.GetFullPath(pipelineWorkspacePath);
        var pipeline = ResolvePipeline(model, pipelineName);
        var orderedTasks = ResolveSerialTaskOrder(model, pipeline);
        var steps = new List<MetaPipelineModeledExecutionStep>();
        var workspaceResolver = new MetaPipelineExecutionWorkspaceResolver();

        for (var index = 0; index < orderedTasks.Count; index++)
        {
            var pipelineTask = orderedTasks[index];
            var executableTask = ResolveExecutableTask(model, pipelineTask);
            var transformExecution = ResolveTransformExecutionTask(model, pipelineTask);
            if (executableTask is not null && transformExecution is not null)
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline task '{pipelineTask.Name}' maps to multiple execution detail kinds.");
            }

            if (executableTask is not null)
            {
                steps.Add(ResolveExecutableStep(pipelineTask, executableTask));
                continue;
            }

            if (transformExecution is null)
            {
                if (TryResolveTargetWritePlan(model, pipeline, pipelineTask) is not null)
                {
                    throw new MetaPipelineConfigurationException(
                        $"Target write task '{pipelineTask.Name}' is not immediately preceded by the SELECT-kind transform it materializes.");
                }

                throw new MetaPipelineConfigurationException(
                    $"Pipeline task '{pipelineTask.Name}' has no supported execution detail.");
            }

            var transformWorkspacePath = RequireWorkspacePath(
                transformExecution.TransformWorkspacePath,
                $"Transform task '{pipelineTask.Name}' must name TransformWorkspacePath.");
            var bindingWorkspacePath = RequireWorkspacePath(
                transformExecution.BindingWorkspacePath,
                $"Transform task '{pipelineTask.Name}' must name BindingWorkspacePath.");
            var transformScriptId = RequireValue(
                transformExecution.TransformScriptId,
                $"Transform task '{pipelineTask.Name}' must name a transform script id.");
            var transformBindingId = RequireValue(
                transformExecution.TransformBindingId,
                $"Transform task '{pipelineTask.Name}' must name a transform binding id.");
            var executionConnection = ResolveConnectionReference(
                model,
                pipeline,
                transformExecution.ExecutionConnectionReference.Id,
                "execution",
                pipelineTask.Name);
            var timeoutSeconds = ResolveTimeoutSeconds(pipelineTask.Name, transformExecution.TimeoutSeconds);

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

                steps.Add(new MetaPipelineModeledExecutionStep(
                    pipelineTask.Id,
                    pipelineTask.Name,
                    MetaPipelineModeledExecutionStepKind.TransformExecution,
                    null,
                    null,
                    transformWorkspacePath,
                    bindingWorkspacePath,
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
                    "SqlServer",
                    null,
                    null,
                    null,
                    null));
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

            var rowStream = EnsureSingleSharedRowStream(model, pipelineTask, targetWritePlan.TargetWritePipelineTask);
            EnsureModeledRowStreamShapeMatchesResolvedShape(
                model,
                rowStream,
                executionDefinition.RowStreamShape ?? throw new MetaPipelineConfigurationException(
                    $"Transform script '{executionDefinition.TransformScriptName}' is SELECT-kind but did not resolve a row-stream shape."),
                pipelineTask.Name,
                targetWritePlan.TargetWritePipelineTask.Name);

            steps.Add(new MetaPipelineModeledExecutionStep(
                pipelineTask.Id,
                pipelineTask.Name,
                MetaPipelineModeledExecutionStepKind.TransformExecution,
                targetWritePlan.TargetWritePipelineTask.Id,
                targetWritePlan.TargetWritePipelineTask.Name,
                transformWorkspacePath,
                bindingWorkspacePath,
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
                targetWritePlan.TargetDataTypeSystemName,
                null,
                null,
                null,
                null));
            index++;
        }

        if (steps.Count == 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Pipeline '{pipeline.Name}' must declare at least one executable task detail row.");
        }

        return new MetaPipelineModeledExecutionPlan(
            pipelineWorkspacePath,
            pipeline.Id,
            pipeline.Name,
            ResolvePlanWorkspacePath(steps, static step => step.TransformWorkspacePath),
            ResolvePlanWorkspacePath(steps, static step => step.BindingWorkspacePath),
            steps);
    }

    public MetaPipelineModeledExecutionPlan ResolveStep(
        MetaPipelineModeledExecutionStepRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.StepName);

        var pipelineWorkspacePath = Path.GetFullPath(request.PipelineWorkspacePath);
        var model = MetaPipelineModel.LoadFromXmlWorkspace(pipelineWorkspacePath, searchUpward: false);
        return ResolveStep(model, pipelineWorkspacePath, request.PipelineName, request.StepName);
    }

    public MetaPipelineModeledExecutionPlan ResolveStep(
        MetaPipelineModel model,
        string pipelineWorkspacePath,
        string pipelineName,
        string stepName)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(pipelineWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(pipelineName);
        ArgumentException.ThrowIfNullOrWhiteSpace(stepName);

        pipelineWorkspacePath = Path.GetFullPath(pipelineWorkspacePath);
        var pipeline = ResolvePipeline(model, pipelineName);
        var pipelineTask = ResolvePipelineTask(model, pipeline, stepName);
        var executableTask = ResolveExecutableTask(model, pipelineTask);
        var transformExecution = ResolveTransformExecutionTask(model, pipelineTask);
        if (executableTask is not null && transformExecution is not null)
        {
            throw new MetaPipelineConfigurationException(
                $"Pipeline task '{pipelineTask.Name}' maps to multiple execution detail kinds.");
        }

        var step = executableTask is not null
            ? ResolveExecutableStep(pipelineTask, executableTask)
            : ResolveTransformExecutionStep(
                model,
                pipeline,
                pipelineTask,
                transformExecution
                ?? throw new MetaPipelineConfigurationException(
                    $"Pipeline task '{pipelineTask.Name}' is not an executable or transform execution step."));

        return new MetaPipelineModeledExecutionPlan(
            pipelineWorkspacePath,
            pipeline.Id,
            pipeline.Name,
            step.TransformWorkspacePath ?? string.Empty,
            step.BindingWorkspacePath ?? string.Empty,
            [step]);
    }

    private static MetaPipelineModeledExecutionStep ResolveExecutableStep(
        PipelineTask pipelineTask,
        ExecutableTask executableTask)
    {
        return new MetaPipelineModeledExecutionStep(
            pipelineTask.Id,
            pipelineTask.Name,
            MetaPipelineModeledExecutionStepKind.Executable,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            IsSelect: false,
            null,
            "None",
            0,
            ResolveTimeoutSeconds(pipelineTask.Name, executableTask.TimeoutSeconds),
            null,
            RequireValue(executableTask.ExecutablePath, $"Executable task '{pipelineTask.Name}' must name an executable path."),
            NormalizeOptionalValue(executableTask.Arguments),
            NormalizeOptionalValue(executableTask.WorkingDirectory),
            ResolveSuccessExitCode(pipelineTask.Name, executableTask.SuccessExitCode));
    }

    private static MetaPipelineModeledExecutionStep ResolveTransformExecutionStep(
        MetaPipelineModel model,
        Pipeline pipeline,
        PipelineTask transformTask,
        TransformExecutionTask transformExecution)
    {
        var transformWorkspacePath = RequireWorkspacePath(
            transformExecution.TransformWorkspacePath,
            $"Transform task '{transformTask.Name}' must name TransformWorkspacePath.");
        var bindingWorkspacePath = RequireWorkspacePath(
            transformExecution.BindingWorkspacePath,
            $"Transform task '{transformTask.Name}' must name BindingWorkspacePath.");
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

            return new MetaPipelineModeledExecutionStep(
                transformTask.Id,
                transformTask.Name,
                MetaPipelineModeledExecutionStepKind.TransformExecution,
                null,
                null,
                transformWorkspacePath,
                bindingWorkspacePath,
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
                "SqlServer",
                null,
                null,
                null,
                null);
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

        return new MetaPipelineModeledExecutionStep(
            transformTask.Id,
            transformTask.Name,
            MetaPipelineModeledExecutionStepKind.TransformExecution,
            targetWritePlan.TargetWritePipelineTask.Id,
            targetWritePlan.TargetWritePipelineTask.Name,
            transformWorkspacePath,
            bindingWorkspacePath,
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
            targetWritePlan.TargetDataTypeSystemName,
            null,
            null,
            null,
            null);
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

        return ordered;
    }

    private static PipelineTask ResolvePipelineTask(
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

    private static ExecutableTask? ResolveExecutableTask(
        MetaPipelineModel model,
        PipelineTask task)
    {
        var matches = model.ExecutableTaskList
            .Where(item => string.Equals(item.PipelineTask.Id, task.Id, StringComparison.Ordinal))
            .ToArray();

        return matches.Length switch
        {
            0 => null,
            1 => matches[0],
            _ => throw new MetaPipelineConfigurationException(
                $"Pipeline task '{task.Name}' has multiple ExecutableTask detail rows."),
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
        string taskName,
        string? configuredTimeoutSeconds)
    {
        if (string.IsNullOrWhiteSpace(configuredTimeoutSeconds))
        {
            return null;
        }

        if (!int.TryParse(configuredTimeoutSeconds, out var timeoutSeconds) || timeoutSeconds < 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Task '{taskName}' has invalid TimeoutSeconds '{configuredTimeoutSeconds}'. Expected a non-negative integer; 0 means no timeout.");
        }

        return timeoutSeconds;
    }

    private static int ResolveSuccessExitCode(
        string taskName,
        string? configuredSuccessExitCode)
    {
        if (string.IsNullOrWhiteSpace(configuredSuccessExitCode))
        {
            return 0;
        }

        if (!int.TryParse(configuredSuccessExitCode, out var successExitCode))
        {
            throw new MetaPipelineConfigurationException(
                $"Executable task '{taskName}' has invalid SuccessExitCode '{configuredSuccessExitCode}'. Expected an integer.");
        }

        return successExitCode;
    }

    private static string ResolvePlanWorkspacePath(
        IReadOnlyList<MetaPipelineModeledExecutionStep> steps,
        Func<MetaPipelineModeledExecutionStep, string?> selectPath)
    {
        var paths = steps
            .Select(selectPath)
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Select(static item => item!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return paths.Length switch
        {
            0 => string.Empty,
            1 => paths[0],
            _ => "<per-task>"
        };
    }

    private static string? NormalizeOptionalValue(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();

    private static string RequireWorkspacePath(string? path, string errorMessage) =>
        string.IsNullOrWhiteSpace(path)
            ? throw new MetaPipelineConfigurationException(errorMessage)
            : Path.GetFullPath(path);

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
