namespace MetaPipeline;

public interface IPipelineRowStreamWriter
{
    PipelineRowStreamShape Shape { get; }

    Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default);
}
