namespace MetaPipeline;

public sealed record MetaPipelineSqlServerTransferRequest(
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    string SourceConnectionString,
    string TargetConnectionString,
    string TransformScriptName,
    string? TargetSqlIdentifier = null,
    int BatchSize = 1000);
