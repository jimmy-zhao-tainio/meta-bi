namespace MetaPipeline;

public sealed record MetaPipelineExecutionContext(
    Guid? PipelineRunId,
    Guid? TaskRunId,
    long? AuditId,
    DateTimeOffset TaskStartedAtUtc,
    string? PipelineName = null,
    string? TaskName = null,
    string? TaskKind = null);
