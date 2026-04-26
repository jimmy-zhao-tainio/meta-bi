namespace MetaPipeline;

public sealed record MetaPipelineSqlServerExecutionRequest(
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    string SourceConnectionString,
    string TargetConnectionString,
    string TransformScriptId,
    string TransformBindingId,
    string? TargetSqlIdentifier = null,
    int BatchSize = 1000);
