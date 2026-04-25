namespace MetaPipeline;

public sealed class BufferedPipelineExecutionService
{
    public async Task<BufferedPipelineExecutionResult> ExecuteAsync(
        IPipelineRowStreamSource source,
        IPipelineRowStreamWriter writer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(writer);

        long rowCount = 0;
        var batchCount = 0;

        try
        {
            source.Shape.EnsureCompatibleWith(writer.Shape, "target writer shape");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return BufferedPipelineExecutionResult.Failed(
                rowCount,
                batchCount,
                PipelineExecutionFailureStage.ShapeValidation,
                ex.Message);
        }

        await using var batches = source
            .ReadBatchesAsync(cancellationToken)
            .GetAsyncEnumerator(cancellationToken);

        while (true)
        {
            PipelineDataBatch batch;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!await batches.MoveNextAsync().ConfigureAwait(false))
                {
                    break;
                }

                batch = batches.Current;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return BufferedPipelineExecutionResult.Failed(
                    rowCount,
                    batchCount,
                    PipelineExecutionFailureStage.SourceRead,
                    ex.Message);
            }

            if (batch.RowCount == 0)
            {
                continue;
            }

            try
            {
                source.Shape.EnsureCompatibleWith(batch.Shape, $"batch {batchCount + 1} shape");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return BufferedPipelineExecutionResult.Failed(
                    rowCount,
                    batchCount,
                    PipelineExecutionFailureStage.ShapeValidation,
                    ex.Message);
            }

            try
            {
                await writer.WriteBatchAsync(batch, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return BufferedPipelineExecutionResult.Failed(
                    rowCount,
                    batchCount,
                    PipelineExecutionFailureStage.TargetWrite,
                    ex.Message);
            }

            rowCount += batch.RowCount;
            batchCount++;
        }

        return BufferedPipelineExecutionResult.Success(rowCount, batchCount);
    }
}

public sealed record BufferedPipelineExecutionResult(
    long RowCount,
    int BatchCount,
    bool Succeeded,
    PipelineExecutionFailureStage FailureStage,
    string FailureMessage)
{
    public static BufferedPipelineExecutionResult Success(long rowCount, int batchCount) =>
        new(rowCount, batchCount, true, PipelineExecutionFailureStage.None, string.Empty);

    public static BufferedPipelineExecutionResult Failed(
        long rowCount,
        int batchCount,
        PipelineExecutionFailureStage failureStage,
        string failureMessage) =>
        new(rowCount, batchCount, false, failureStage, failureMessage);
}

public enum PipelineExecutionFailureStage
{
    None,
    SourceRead,
    ShapeValidation,
    TargetWrite,
}
