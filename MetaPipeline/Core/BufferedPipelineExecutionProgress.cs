namespace MetaPipeline;

public sealed record BufferedPipelineExecutionProgress(
    long RowCount,
    int BatchCount,
    long EstimatedByteCount,
    DateTimeOffset StartedAtUtc);
