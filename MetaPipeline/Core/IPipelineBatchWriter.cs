namespace MetaPipeline;

public interface IPipelineBatchWriter
{
    Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default);
}
