namespace MetaPipeline;

public sealed record MetaPipelineExecutionResult(
    MetaPipelineExecutionStatus Status,
    string TransformScriptName,
    string TargetSqlIdentifier,
    int ColumnCount,
    long RowCount,
    int BatchCount,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset CompletedAtUtc,
    PipelineExecutionFailureStage FailureStage,
    string FailureMessage)
{
    public bool Succeeded => Status == MetaPipelineExecutionStatus.Succeeded;
}

public enum MetaPipelineExecutionStatus
{
    Succeeded,
    Failed,
}
