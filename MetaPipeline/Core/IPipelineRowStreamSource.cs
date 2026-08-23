namespace MetaPipeline;

public interface IPipelineRowStreamSource
{
    PipelineRowStreamShape Shape { get; }

    IAsyncEnumerable<PipelineDataBatch> ReadBatchesAsync(CancellationToken cancellationToken = default);
}
