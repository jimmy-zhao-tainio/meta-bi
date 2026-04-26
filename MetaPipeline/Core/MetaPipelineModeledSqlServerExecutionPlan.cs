namespace MetaPipeline;

public sealed record MetaPipelineModeledSqlServerExecutionRequest(
    string PipelineWorkspacePath,
    string PipelineName,
    string TaskName,
    string TransformWorkspacePath,
    string BindingWorkspacePath);

public sealed record MetaPipelineModeledSqlServerExecutionPlan(
    string PipelineWorkspacePath,
    string PipelineName,
    string TransformTaskName,
    string TargetWriteTaskName,
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    string TransformScriptId,
    string TransformBindingId,
    string TransformScriptName,
    string SourceConnectionReferenceName,
    string SourceConnectionEnvironmentVariableName,
    string TargetConnectionReferenceName,
    string TargetConnectionEnvironmentVariableName,
    string TargetSqlIdentifier,
    string TargetWriteModelName,
    int BatchSize);
