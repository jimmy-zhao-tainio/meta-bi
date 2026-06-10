namespace MetaPipeline;

public sealed record MetaPipelineModeledExecutionRequest(
    string PipelineWorkspacePath,
    string PipelineName,
    string TransformWorkspacePath,
    string BindingWorkspacePath);

public sealed record MetaPipelineModeledExecutionStepRequest(
    string PipelineWorkspacePath,
    string PipelineName,
    string StepName,
    string TransformWorkspacePath,
    string BindingWorkspacePath);

public sealed record MetaPipelineModeledExecutionPlan(
    string PipelineWorkspacePath,
    string PipelineId,
    string PipelineName,
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    IReadOnlyList<MetaPipelineModeledExecutionStep> Steps)
{
    public MetaPipelineModeledExecutionStep FirstStep =>
        Steps.Count > 0
            ? Steps[0]
            : throw new MetaPipelineConfigurationException($"Pipeline '{PipelineName}' has no executable steps.");

    public string TransformTaskId => FirstStep.TaskId;
    public string TransformTaskName => FirstStep.TaskName;
    public string? TargetWriteTaskId => FirstStep.TargetWriteTaskId;
    public string? TargetWriteTaskName => FirstStep.TargetWriteTaskName;
    public string TransformScriptId => FirstStep.TransformScriptId ?? string.Empty;
    public string TransformBindingId => FirstStep.TransformBindingId ?? string.Empty;
    public string TransformScriptName => FirstStep.TransformScriptName ?? string.Empty;
    public string ExecutionConnectionReferenceName => FirstStep.ExecutionConnectionReferenceName ?? string.Empty;
    public string ExecutionConnectionEnvironmentVariableName => FirstStep.ExecutionConnectionEnvironmentVariableName ?? string.Empty;
    public string? TargetConnectionReferenceName => FirstStep.TargetConnectionReferenceName;
    public string? TargetConnectionEnvironmentVariableName => FirstStep.TargetConnectionEnvironmentVariableName;
    public bool IsSelect => FirstStep.IsSelect;
    public string? TargetSqlIdentifier => FirstStep.TargetSqlIdentifier;
    public string TargetWriteModelName => FirstStep.TargetWriteModelName ?? "None";
    public int BatchSize => FirstStep.BatchSize;
    public int? TimeoutSeconds => FirstStep.TimeoutSeconds;
    public string TargetDataTypeSystemName => FirstStep.TargetDataTypeSystemName ?? string.Empty;
}

public sealed record MetaPipelineModeledExecutionStep(
    string TaskId,
    string TaskName,
    MetaPipelineModeledExecutionStepKind StepKind,
    string? TargetWriteTaskId,
    string? TargetWriteTaskName,
    string? TransformScriptId,
    string? TransformBindingId,
    string? TransformScriptName,
    string? ExecutionConnectionReferenceName,
    string? ExecutionConnectionEnvironmentVariableName,
    string? TargetConnectionReferenceName,
    string? TargetConnectionEnvironmentVariableName,
    bool IsSelect,
    string? TargetSqlIdentifier,
    string? TargetWriteModelName,
    int BatchSize,
    int? TimeoutSeconds,
    string? TargetDataTypeSystemName,
    string? ExecutablePath,
    string? Arguments,
    string? WorkingDirectory,
    int? SuccessExitCode)
{
    public string TransformTaskId => TaskId;
    public string TransformTaskName => TaskName;
}

public enum MetaPipelineModeledExecutionStepKind
{
    TransformExecution,
    Executable,
}
