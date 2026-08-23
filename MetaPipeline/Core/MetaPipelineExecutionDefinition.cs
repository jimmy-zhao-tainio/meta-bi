namespace MetaPipeline;

public sealed record MetaPipelineExecutionDefinition(
    string TransformScriptId,
    string TransformScriptName,
    string TransformBindingId,
    string SourceSql,
    bool IsSelect,
    string? TargetSqlIdentifier = null,
    PipelineRowStreamShape? RowStreamShape = null)
{
    public IReadOnlyList<PipelineColumn> Columns => RowStreamShape?.Columns ?? [];
}
