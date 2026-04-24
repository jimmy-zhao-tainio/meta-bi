namespace MetaPipeline;

public sealed record MetaPipelineExecutionDefinition(
    string TransformScriptId,
    string TransformScriptName,
    string SourceSql,
    string TargetSqlIdentifier,
    IReadOnlyList<PipelineColumn> Columns);
