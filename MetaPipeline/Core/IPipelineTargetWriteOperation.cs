namespace MetaPipeline;

public interface IPipelineTargetWriteOperation : IAsyncDisposable
{
    string Name { get; }

    PipelineRowStreamShape Shape { get; }

    ValueTask BeginAsync(CancellationToken cancellationToken = default);

    Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default);

    ValueTask CompleteAsync(CancellationToken cancellationToken = default);
}
