namespace MetaPipeline;

public sealed record MetaPipelineModeledSqlServerExecutionRequest(
    string PipelineWorkspacePath,
    string PipelineName,
    string TransformWorkspacePath,
    string BindingWorkspacePath);

public sealed record MetaPipelineModeledSqlServerExecutionStepRequest(
    string PipelineWorkspacePath,
    string PipelineName,
    string StepName,
    string TransformWorkspacePath,
    string BindingWorkspacePath);

public sealed record MetaPipelineModeledSqlServerExecutionPlan(
    string PipelineWorkspacePath,
    string PipelineId,
    string PipelineName,
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    IReadOnlyList<MetaPipelineModeledSqlServerExecutionStep> Steps)
{
    public MetaPipelineModeledSqlServerExecutionStep FirstStep =>
        Steps.Count > 0
            ? Steps[0]
            : throw new MetaPipelineConfigurationException($"Pipeline '{PipelineName}' has no executable steps.");

    public string TransformTaskId => FirstStep.TransformTaskId;
    public string TransformTaskName => FirstStep.TransformTaskName;
    public string? TargetWriteTaskId => FirstStep.TargetWriteTaskId;
    public string? TargetWriteTaskName => FirstStep.TargetWriteTaskName;
    public string TransformScriptId => FirstStep.TransformScriptId;
    public string TransformBindingId => FirstStep.TransformBindingId;
    public string TransformScriptName => FirstStep.TransformScriptName;
    public string ExecutionConnectionReferenceName => FirstStep.ExecutionConnectionReferenceName;
    public string ExecutionConnectionEnvironmentVariableName => FirstStep.ExecutionConnectionEnvironmentVariableName;
    public string? TargetConnectionReferenceName => FirstStep.TargetConnectionReferenceName;
    public string? TargetConnectionEnvironmentVariableName => FirstStep.TargetConnectionEnvironmentVariableName;
    public bool IsSelect => FirstStep.IsSelect;
    public string? TargetSqlIdentifier => FirstStep.TargetSqlIdentifier;
    public string TargetWriteModelName => FirstStep.TargetWriteModelName;
    public int BatchSize => FirstStep.BatchSize;
    public int? TimeoutSeconds => FirstStep.TimeoutSeconds;
    public string TargetDataTypeSystemName => FirstStep.TargetDataTypeSystemName;
}

public sealed record MetaPipelineModeledSqlServerExecutionStep(
    string TransformTaskId,
    string TransformTaskName,
    string? TargetWriteTaskId,
    string? TargetWriteTaskName,
    string TransformScriptId,
    string TransformBindingId,
    string TransformScriptName,
    string ExecutionConnectionReferenceName,
    string ExecutionConnectionEnvironmentVariableName,
    string? TargetConnectionReferenceName,
    string? TargetConnectionEnvironmentVariableName,
    bool IsSelect,
    string? TargetSqlIdentifier,
    string TargetWriteModelName,
    int BatchSize,
    int? TimeoutSeconds,
    string TargetDataTypeSystemName);
