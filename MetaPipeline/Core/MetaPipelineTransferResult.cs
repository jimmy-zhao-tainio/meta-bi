namespace MetaPipeline;

public sealed record MetaPipelineTransferResult(
    MetaPipelineTransferStatus Status,
    string TransformScriptName,
    string TargetSqlIdentifier,
    int ColumnCount,
    long RowCount,
    int BatchCount,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset CompletedAtUtc,
    string FailureMessage)
{
    public bool Succeeded => Status == MetaPipelineTransferStatus.Succeeded;
}

public enum MetaPipelineTransferStatus
{
    Succeeded,
    Failed,
}
