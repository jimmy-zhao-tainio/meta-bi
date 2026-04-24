namespace MetaPipeline;

public sealed class PipelineDataBatch
{
    public PipelineDataBatch(IReadOnlyList<object?[]> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        Rows = rows;
    }

    public IReadOnlyList<object?[]> Rows { get; }

    public int RowCount => Rows.Count;
}
