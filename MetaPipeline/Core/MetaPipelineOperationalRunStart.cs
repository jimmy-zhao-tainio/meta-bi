namespace MetaPipeline;

public sealed record MetaPipelineOperationalRunStart(
    string? PipelineWorkspacePath = null,
    string? PipelineId = null,
    string? PipelineName = null,
    string? TransformTaskId = null,
    string? TransformTaskName = null,
    string? TargetWriteTaskId = null,
    string? TargetWriteTaskName = null,
    string? TransformWorkspacePath = null,
    string? BindingWorkspacePath = null,
    string? TransformScriptId = null,
    string? TransformBindingId = null,
    string? TransformScriptName = null,
    string? ExecutionConnectionReferenceName = null,
    string? ExecutionConnectionEnvironmentVariableName = null,
    string? TargetConnectionReferenceName = null,
    string? TargetConnectionEnvironmentVariableName = null,
    string? TargetSqlIdentifier = null,
    string? TargetWriteModelName = null,
    int? BatchSize = null);
