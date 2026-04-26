namespace MetaPipeline;

public sealed record MetaPipelineExecutionDefinition(
    string TransformScriptId,
    string TransformScriptName,
    string TransformBindingId,
    string SourceSql,
    string TargetSqlIdentifier,
    PipelineRowStreamShape RowStreamShape)
{
    public IReadOnlyList<PipelineColumn> Columns => RowStreamShape.Columns;
}
