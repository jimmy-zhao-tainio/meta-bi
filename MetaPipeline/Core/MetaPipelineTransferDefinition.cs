namespace MetaPipeline;

public sealed record MetaPipelineTransferDefinition(
    string TransformScriptId,
    string TransformScriptName,
    string SourceSql,
    string TargetSqlIdentifier,
    IReadOnlyList<PipelineColumn> Columns);
