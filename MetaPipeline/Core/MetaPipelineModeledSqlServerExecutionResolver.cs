namespace MetaPipeline;

public sealed class MetaPipelineModeledSqlServerExecutionResolver
{
    public MetaPipelineModeledSqlServerExecutionPlan Resolve(
        MetaPipelineModeledSqlServerExecutionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TaskName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TransformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.BindingWorkspacePath);

        var pipelineWorkspacePath = Path.GetFullPath(request.PipelineWorkspacePath);
        var model = MetaPipelineModel.LoadFromXmlWorkspace(pipelineWorkspacePath, searchUpward: false);
        var pipeline = ResolvePipeline(model, request.PipelineName);
        var transformTask = ResolveTask(model, pipeline, request.TaskName);
        var transformExecution = ResolveSingle(
            model.TransformExecutionTaskList.Where(item => string.Equals(item.PipelineTaskId, transformTask.Id, StringComparison.Ordinal)),
            $"Transform task '{transformTask.Name}' must have exactly one TransformExecutionTask detail row.");
        var transformScriptId = RequireValue(
            transformExecution.TransformScriptId,
            $"Transform task '{transformTask.Name}' must name a transform script id.");
        var transformBindingId = RequireValue(
            transformExecution.TransformBindingId,
            $"Transform task '{transformTask.Name}' must name a transform binding id.");
        var sourceConnection = ResolveConnectionReference(
            model,
            pipeline,
            transformExecution.SourceConnectionReferenceId,
            "source",
            transformTask.Name);

        var (targetWriteTask, targetWrite, insertRows) = ResolveSingleInsertRowsSuccessor(model, pipeline, transformTask);
        var targetConnection = ResolveConnectionReference(
            model,
            pipeline,
            targetWrite.TargetConnectionReferenceId,
            "target",
            targetWriteTask.Name);

        EnsureSingleSharedRowStream(model, transformTask, targetWriteTask);

        var executionDefinition = new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
            request.TransformWorkspacePath,
            request.BindingWorkspacePath,
            transformScriptId,
            transformBindingId,
            insertRows.TargetSqlIdentifier);

        return new MetaPipelineModeledSqlServerExecutionPlan(
            pipelineWorkspacePath,
            pipeline.Name,
            transformTask.Name,
            targetWriteTask.Name,
            Path.GetFullPath(request.TransformWorkspacePath),
            Path.GetFullPath(request.BindingWorkspacePath),
            executionDefinition.TransformScriptId,
            executionDefinition.TransformBindingId,
            executionDefinition.TransformScriptName,
            sourceConnection.Name,
            RequireValue(sourceConnection.EnvironmentVariableName, $"Source connection reference '{sourceConnection.Name}' must name an environment variable."),
            targetConnection.Name,
            RequireValue(targetConnection.EnvironmentVariableName, $"Target connection reference '{targetConnection.Name}' must name an environment variable."),
            executionDefinition.TargetSqlIdentifier,
            "InsertRows",
            ResolveBatchSize(targetWriteTask, insertRows));
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

    private static PipelineTask ResolveTask(
        MetaPipelineModel model,
        Pipeline pipeline,
        string taskName)
    {
        var matches = model.PipelineTaskList
            .Where(item => string.Equals(item.PipelineId, pipeline.Id, StringComparison.Ordinal)
                           && string.Equals(item.Name, taskName.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return matches.Length switch
        {
            0 => throw new MetaPipelineConfigurationException($"Task '{taskName}' was not found in pipeline '{pipeline.Name}'."),
            > 1 => throw new MetaPipelineConfigurationException($"Task name '{taskName}' is ambiguous in pipeline '{pipeline.Name}'."),
            _ => matches[0],
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

        if (!string.Equals(connectionReference.PipelineId, pipeline.Id, StringComparison.Ordinal))
        {
            throw new MetaPipelineConfigurationException(
                $"Task '{taskName}' references a {role} connection from another pipeline.");
        }

        return connectionReference;
    }

    private static (
        PipelineTask TargetWritePipelineTask,
        TargetWriteTask TargetWriteTask,
        InsertRowsTargetWriteTask InsertRows) ResolveSingleInsertRowsSuccessor(
        MetaPipelineModel model,
        Pipeline pipeline,
        PipelineTask transformTask)
    {
        var successorIds = model.TaskDependencyList
            .Where(item => string.Equals(item.PipelineId, pipeline.Id, StringComparison.Ordinal)
                           && string.Equals(item.PredecessorId, transformTask.Id, StringComparison.Ordinal))
            .Select(static item => item.SuccessorId)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var targetWriteDetails = model.TargetWriteTaskList
            .Where(item => successorIds.Contains(item.PipelineTaskId, StringComparer.Ordinal))
            .ToArray();

        var targetWrite = targetWriteDetails.Length switch
        {
            0 => throw new MetaPipelineConfigurationException(
                $"Transform task '{transformTask.Name}' must have exactly one TargetWrite successor."),
            > 1 => throw new MetaPipelineConfigurationException(
                $"Transform task '{transformTask.Name}' has multiple TargetWrite successors. Stage 1 execute supports one."),
            _ => targetWriteDetails[0],
        };

        var targetWritePipelineTask = model.PipelineTaskList.SingleOrDefault(item =>
            string.Equals(item.Id, targetWrite.PipelineTaskId, StringComparison.Ordinal)
            && string.Equals(item.PipelineId, pipeline.Id, StringComparison.Ordinal));
        if (targetWritePipelineTask is null)
        {
            throw new MetaPipelineConfigurationException(
                $"Target write detail '{targetWrite.Id}' points to a missing pipeline task.");
        }

        var insertRows = ResolveSingle(
            model.InsertRowsTargetWriteTaskList.Where(item => string.Equals(item.TargetWriteTaskId, targetWrite.Id, StringComparison.Ordinal)),
            $"Target write task '{targetWritePipelineTask.Name}' must have exactly one InsertRowsTargetWriteTask detail row.");

        return (targetWritePipelineTask, targetWrite, insertRows);
    }

    private static void EnsureSingleSharedRowStream(
        MetaPipelineModel model,
        PipelineTask transformTask,
        PipelineTask targetWriteTask)
    {
        var producer = ResolveSingle(
            model.RowStreamProducerList.Where(item => string.Equals(item.PipelineTaskId, transformTask.Id, StringComparison.Ordinal)),
            $"Transform task '{transformTask.Name}' must produce exactly one row stream.");
        var consumer = ResolveSingle(
            model.RowStreamConsumerList.Where(item => string.Equals(item.PipelineTaskId, targetWriteTask.Id, StringComparison.Ordinal)),
            $"Target write task '{targetWriteTask.Name}' must consume exactly one row stream.");

        if (!string.Equals(producer.RowStreamId, consumer.RowStreamId, StringComparison.Ordinal))
        {
            throw new MetaPipelineConfigurationException(
                $"Transform task '{transformTask.Name}' and target write task '{targetWriteTask.Name}' must share the same row stream.");
        }

        if (!model.RowStreamList.Any(item => string.Equals(item.Id, producer.RowStreamId, StringComparison.Ordinal)))
        {
            throw new MetaPipelineConfigurationException(
                $"Row stream '{producer.RowStreamId}' referenced by task '{transformTask.Name}' was not found.");
        }
    }

    private static int ResolveBatchSize(
        PipelineTask targetWriteTask,
        InsertRowsTargetWriteTask insertRows)
    {
        if (string.IsNullOrWhiteSpace(insertRows.BatchSize))
        {
            return 1000;
        }

        if (!int.TryParse(insertRows.BatchSize, out var batchSize) || batchSize <= 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Target write task '{targetWriteTask.Name}' has invalid BatchSize '{insertRows.BatchSize}'. Expected a positive integer.");
        }

        return batchSize;
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

    private static string RequireValue(string value, string errorMessage) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new MetaPipelineConfigurationException(errorMessage)
            : value.Trim();
}
