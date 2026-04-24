using System.Runtime.CompilerServices;

namespace MetaPipeline.Tests;

public sealed class BufferedPipelineTransferServiceTests
{
    [Fact]
    public async Task TransferAsync_ForwardsBufferedBatchesAndCountsRows()
    {
        var source = new FakeBatchSource(
            [
                new PipelineColumn("CustomerId", 1),
                new PipelineColumn("CustomerName", 2),
            ],
            [
                new PipelineDataBatch(
                    [
                        new object?[] { 1, "A" },
                        new object?[] { 2, "B" },
                    ]),
                new PipelineDataBatch(
                    [
                        new object?[] { 3, "C" },
                    ]),
            ]);
        var writer = new RecordingBatchWriter();

        var result = await new BufferedPipelineTransferService().TransferAsync(source, writer);

        Assert.True(result.Succeeded);
        Assert.Equal(string.Empty, result.FailureMessage);
        Assert.Equal(3, result.RowCount);
        Assert.Equal(2, result.BatchCount);
        Assert.Equal(2, writer.Batches.Count);
        Assert.Equal(3, writer.Batches.Sum(static batch => batch.RowCount));
    }

    [Fact]
    public async Task TransferAsync_WhenBatchShapeDoesNotMatchColumns_ReturnsFailedResultBeforeWritingBatch()
    {
        var source = new FakeBatchSource(
            [
                new PipelineColumn("CustomerId", 1),
                new PipelineColumn("CustomerName", 2),
            ],
            [
                new PipelineDataBatch(
                    [
                        new object?[] { 1, "A" },
                    ]),
                new PipelineDataBatch(
                    [
                        new object?[] { 2 },
                    ]),
            ]);
        var writer = new RecordingBatchWriter();

        var result = await new BufferedPipelineTransferService().TransferAsync(source, writer);

        Assert.False(result.Succeeded);
        Assert.Equal(1, result.RowCount);
        Assert.Equal(1, result.BatchCount);
        Assert.Contains("expects 2", result.FailureMessage, StringComparison.Ordinal);
        Assert.Single(writer.Batches);
    }

    [Fact]
    public async Task TransferAsync_WhenWriterFails_ReturnsCompletedProgressBeforeFailure()
    {
        var source = new FakeBatchSource(
            [
                new PipelineColumn("CustomerId", 1),
            ],
            [
                new PipelineDataBatch([new object?[] { 1 }]),
                new PipelineDataBatch([new object?[] { 2 }]),
            ]);
        var writer = new FailingSecondBatchWriter();

        var result = await new BufferedPipelineTransferService().TransferAsync(source, writer);

        Assert.False(result.Succeeded);
        Assert.Equal(1, result.RowCount);
        Assert.Equal(1, result.BatchCount);
        Assert.Contains("simulated writer failure", result.FailureMessage, StringComparison.Ordinal);
    }

    private sealed class FakeBatchSource : IPipelineBatchSource
    {
        private readonly IReadOnlyList<PipelineDataBatch> batches;

        public FakeBatchSource(
            IReadOnlyList<PipelineColumn> columns,
            IReadOnlyList<PipelineDataBatch> batches)
        {
            Columns = columns;
            this.batches = batches;
        }

        public IReadOnlyList<PipelineColumn> Columns { get; }

        public async IAsyncEnumerable<PipelineDataBatch> ReadBatchesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var batch in batches)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return batch;
            }
        }
    }

    private sealed class RecordingBatchWriter : IPipelineBatchWriter
    {
        public List<PipelineDataBatch> Batches { get; } = new();

        public Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default)
        {
            Batches.Add(batch);
            return Task.CompletedTask;
        }
    }

    private sealed class FailingSecondBatchWriter : IPipelineBatchWriter
    {
        private int callCount;

        public Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default)
        {
            callCount++;
            if (callCount == 2)
            {
                throw new InvalidOperationException("simulated writer failure");
            }

            return Task.CompletedTask;
        }
    }
}
