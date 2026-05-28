namespace MetaPipeline;

public sealed class BufferedPipelineExecutionService
{
    public async Task<BufferedPipelineExecutionResult> ExecuteAsync(
        IPipelineRowStreamSource source,
        IPipelineTargetWriteOperation targetWriteOperation,
        IProgress<BufferedPipelineExecutionProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(targetWriteOperation);

        var startedAtUtc = DateTimeOffset.UtcNow;
        long rowCount = 0;
        long byteCount = 0;
        var batchCount = 0;

        try
        {
            source.Shape.EnsureCompatibleWith(targetWriteOperation.Shape, "target write operation shape");
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
                byteCount,
                PipelineExecutionFailureStage.ShapeValidation,
                ex.Message);
        }

        try
        {
            await targetWriteOperation.BeginAsync(cancellationToken).ConfigureAwait(false);
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
                byteCount,
                PipelineExecutionFailureStage.TargetWrite,
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
                    byteCount,
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
                    byteCount,
                    PipelineExecutionFailureStage.ShapeValidation,
                    ex.Message);
            }

            try
            {
                await targetWriteOperation.WriteBatchAsync(batch, cancellationToken).ConfigureAwait(false);
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
                    byteCount,
                    PipelineExecutionFailureStage.TargetWrite,
                    ex.Message);
            }

            rowCount += batch.RowCount;
            batchCount++;
            if (progress is not null)
            {
                byteCount += PipelineDataBatchByteEstimator.EstimatePayloadBytes(batch);
                progress.Report(new BufferedPipelineExecutionProgress(rowCount, batchCount, byteCount, startedAtUtc));
            }
        }

        try
        {
            await targetWriteOperation.CompleteAsync(cancellationToken).ConfigureAwait(false);
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
                byteCount,
                PipelineExecutionFailureStage.TargetWrite,
                ex.Message);
        }

        return BufferedPipelineExecutionResult.Success(rowCount, batchCount, byteCount);
    }
}

public sealed record BufferedPipelineExecutionResult(
    long RowCount,
    int BatchCount,
    long EstimatedByteCount,
    bool Succeeded,
    PipelineExecutionFailureStage FailureStage,
    string FailureMessage)
{
    public static BufferedPipelineExecutionResult Success(long rowCount, int batchCount, long estimatedByteCount) =>
        new(rowCount, batchCount, estimatedByteCount, true, PipelineExecutionFailureStage.None, string.Empty);

    public static BufferedPipelineExecutionResult Failed(
        long rowCount,
        int batchCount,
        long estimatedByteCount,
        PipelineExecutionFailureStage failureStage,
        string failureMessage) =>
        new(rowCount, batchCount, estimatedByteCount, false, failureStage, failureMessage);
}

public enum PipelineExecutionFailureStage
{
    None,
    SourceRead,
    TransformExecution,
    ShapeValidation,
    TargetWrite,
}
