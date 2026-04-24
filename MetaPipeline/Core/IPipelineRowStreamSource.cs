namespace MetaPipeline;

public interface IPipelineRowStreamSource
{
    IReadOnlyList<PipelineColumn> Columns { get; }

    IAsyncEnumerable<PipelineDataBatch> ReadBatchesAsync(CancellationToken cancellationToken = default);
}
