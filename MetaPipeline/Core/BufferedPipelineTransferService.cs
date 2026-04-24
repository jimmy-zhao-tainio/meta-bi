namespace MetaPipeline;

public sealed class BufferedPipelineTransferService
{
    public async Task<BufferedPipelineTransferResult> TransferAsync(
        IPipelineBatchSource source,
        IPipelineBatchWriter writer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(writer);

        long rowCount = 0;
        var batchCount = 0;

        try
        {
            await foreach (var batch in source.ReadBatchesAsync(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (batch.RowCount == 0)
                {
                    continue;
                }

                ValidateBatchShape(batch, source.Columns.Count, batchCount + 1);

                await writer.WriteBatchAsync(batch, cancellationToken).ConfigureAwait(false);
                rowCount += batch.RowCount;
                batchCount++;
            }

            return BufferedPipelineTransferResult.Success(rowCount, batchCount);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return BufferedPipelineTransferResult.Failed(rowCount, batchCount, ex.Message);
        }
    }

    private static void ValidateBatchShape(
        PipelineDataBatch batch,
        int expectedColumnCount,
        int batchOrdinal)
    {
        if (expectedColumnCount <= 0)
        {
            throw new MetaPipelineConfigurationException(
                "Pipeline source must expose at least one column.");
        }

        for (var rowIndex = 0; rowIndex < batch.Rows.Count; rowIndex++)
        {
            var row = batch.Rows[rowIndex];
            if (row.Length != expectedColumnCount)
            {
                throw new MetaPipelineConfigurationException(
                    $"Batch {batchOrdinal} row {rowIndex + 1} contains {row.Length} values but the pipeline shape expects {expectedColumnCount}.");
            }
        }
    }
}

public sealed record BufferedPipelineTransferResult(
    long RowCount,
    int BatchCount,
    bool Succeeded,
    string FailureMessage)
{
    public static BufferedPipelineTransferResult Success(long rowCount, int batchCount) =>
        new(rowCount, batchCount, true, string.Empty);

    public static BufferedPipelineTransferResult Failed(long rowCount, int batchCount, string failureMessage) =>
        new(rowCount, batchCount, false, failureMessage);
}
