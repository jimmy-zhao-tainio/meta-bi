namespace MetaPipeline;

public sealed record MetaPipelineExecutionResult(
    MetaPipelineExecutionStatus Status,
    string TransformScriptName,
    string TargetSqlIdentifier,
    string TargetWriteOperationName,
    string TargetWriteModelName,
    int ColumnCount,
    long RowCount,
    int BatchCount,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset CompletedAtUtc,
    PipelineExecutionFailureStage FailureStage,
    string FailureMessage,
    string FailureTaskName,
    IReadOnlyList<MetaPipelineExecutionTaskResult> TaskResults)
{
    public bool Succeeded => Status == MetaPipelineExecutionStatus.Succeeded;
}

public enum MetaPipelineExecutionStatus
{
    Succeeded,
    Failed,
}

public sealed record MetaPipelineExecutionTaskResult(
    string TaskName,
    string TaskKind,
    MetaPipelineExecutionTaskStatus Status,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset CompletedAtUtc,
    long RowCount,
    int BatchCount,
    PipelineExecutionFailureStage FailureStage,
    string FailureMessage,
    Guid? TaskRunId = null,
    long? AuditId = null,
    int? TimeoutSeconds = null,
    int? ExitCode = null,
    string? TransformScriptId = null,
    string? TransformScriptName = null);

public enum MetaPipelineExecutionTaskStatus
{
    Succeeded,
    Failed,
    Skipped,
}
