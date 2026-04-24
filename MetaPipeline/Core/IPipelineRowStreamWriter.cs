namespace MetaPipeline;

public interface IPipelineRowStreamWriter
{
    Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default);
}
