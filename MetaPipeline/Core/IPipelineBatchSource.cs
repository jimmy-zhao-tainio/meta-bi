namespace MetaPipeline;

public interface IPipelineBatchSource
{
    IReadOnlyList<PipelineColumn> Columns { get; }

    IAsyncEnumerable<PipelineDataBatch> ReadBatchesAsync(CancellationToken cancellationToken = default);
}
