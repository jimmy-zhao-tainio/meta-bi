namespace MetaPipeline;

public sealed class PipelineDataBatch
{
    public PipelineDataBatch(
        PipelineRowStreamShape shape,
        IReadOnlyList<object?[]> rows)
    {
        ArgumentNullException.ThrowIfNull(shape);
        ArgumentNullException.ThrowIfNull(rows);

        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            var row = rows[rowIndex];
            if (row.Length != shape.ColumnCount)
            {
                throw new MetaPipelineConfigurationException(
                    $"Buffered row {rowIndex + 1} contains {row.Length} values but the pipeline shape expects {shape.ColumnCount}.");
            }
        }

        Shape = shape;
        Rows = rows;
    }

    public PipelineRowStreamShape Shape { get; }

    public IReadOnlyList<object?[]> Rows { get; }

    public int RowCount => Rows.Count;
}
