using Meta.Core.Operations;
using Meta.Core.Services;

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

    private static WorkspaceOp CreateUpsertOperation(
        string entityName,
        RowPatch rowPatch)
    {
        return new WorkspaceOp
        {
            Type = WorkspaceOpTypes.BulkUpsertRows,
            EntityName = entityName,
            RowPatches = { rowPatch },
        };
    }

    private static RowPatch CreateRowPatch(
        string id,
        IReadOnlyDictionary<string, string>? values = null,
        IReadOnlyDictionary<string, string>? relationships = null)
    {
        var rowPatch = new RowPatch
        {
            Id = id,
        };

        if (values is not null)
        {
            foreach (var value in values)
            {
                rowPatch.Values[value.Key] = value.Value;
            }
        }

        if (relationships is not null)
        {
            foreach (var relationship in relationships)
            {
                rowPatch.RelationshipIds[relationship.Key] = relationship.Value;
            }
        }

        return rowPatch;
    }

    private static void ApplyInstanceUpserts(
        string workspacePath,
        params WorkspaceOp[] operations)
    {
        ApplyInstanceUpserts(workspacePath, (IEnumerable<WorkspaceOp>)operations);
    }

    private static void ApplyInstanceUpserts(
        string workspacePath,
        IEnumerable<WorkspaceOp> operations)
    {
        var workspaceService = new WorkspaceService();
        var workspace = workspaceService.LoadAsync(workspacePath, searchUpward: false)
            .GetAwaiter()
            .GetResult();

        foreach (var operation in operations)
        {
            BulkRelationshipResolver.ResolveRelationshipIds(workspace, operation);
            WorkspaceOperationApplier.Apply(workspace, operation);
        }

        workspaceService.SaveAsync(workspace)
            .GetAwaiter()
            .GetResult();
    }

    private static MetaPipeline.ConnectionReference GetOrAddConnectionReference(
        MetaPipeline.MetaPipelineModel model,
        MetaPipeline.Pipeline pipeline,
        string name,
        string environmentVariableName)
    {
        var matches = model.ConnectionReferenceList
            .Where(item => string.Equals(item.PipelineId, pipeline.Id, StringComparison.Ordinal)
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

        return new MetaPipeline.ConnectionReference
        {
            Id = ScopedId(pipeline.Id, name),
            PipelineId = pipeline.Id,
            Name = name.Trim(),
            EnvironmentVariableName = environmentVariableName.Trim(),
        };
    }

    private static string ResolveTaskLabel(
        MetaPipeline.MetaPipelineModel model,
        MetaPipeline.PipelineTask task)
    {
        if (model.TransformExecutionTaskList.Any(item => string.Equals(item.PipelineTaskId, task.Id, StringComparison.Ordinal)))
        {
            return "TransformExecution";
        }

        var targetWriteTask = model.TargetWriteTaskList.SingleOrDefault(item =>
            string.Equals(item.PipelineTaskId, task.Id, StringComparison.Ordinal));
        if (targetWriteTask is not null)
        {
            var hasInsertRows = model.InsertRowsTargetWriteTaskList.Any(item =>
                string.Equals(item.TargetWriteTaskId, targetWriteTask.Id, StringComparison.Ordinal));
            return hasInsertRows ? "TargetWrite:InsertRows" : "TargetWrite";
        }

        return "PipelineTask";
    }

    private static void EnsureTaskNameAvailable(
        MetaPipeline.MetaPipelineModel model,
        MetaPipeline.Pipeline pipeline,
        string taskName)
    {
        if (model.PipelineTaskList.Any(item =>
                string.Equals(item.PipelineId, pipeline.Id, StringComparison.Ordinal)
                && string.Equals(item.Name, taskName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new MetaPipeline.MetaPipelineConfigurationException(
                $"Task '{taskName}' already exists in pipeline '{pipeline.Name}'.");
        }
    }

    private static int ResolveNextTaskOrdinal(
        MetaPipeline.MetaPipelineModel model,
        MetaPipeline.Pipeline pipeline)
    {
        var maxOrdinal = model.PipelineTaskList
            .Where(item => string.Equals(item.PipelineId, pipeline.Id, StringComparison.Ordinal))
            .Select(static item => ParseOrdinalOrZero(item.Ordinal))
            .DefaultIfEmpty(0)
            .Max();
        return maxOrdinal + 1;
    }

    private static int ParseOrdinalOrZero(string value) =>
        int.TryParse(value, out var ordinal) ? ordinal : 0;

    private static int ParseOrdinalOrMax(string value) =>
        int.TryParse(value, out var ordinal) ? ordinal : int.MaxValue;

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
