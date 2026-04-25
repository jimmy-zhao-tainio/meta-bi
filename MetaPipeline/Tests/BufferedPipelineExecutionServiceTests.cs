using System.Runtime.CompilerServices;

namespace MetaPipeline.Tests;

public sealed class BufferedPipelineExecutionServiceTests
{
    [Fact]
    public async Task ExecuteAsync_ForwardsBufferedBatchesAndCountsRows()
    {
        var shape = CreateShape("CustomerId", "CustomerName");
        var source = new FakeRowStreamSource(
            shape,
            [
                [
                    new object?[] { 1, "A" },
                    new object?[] { 2, "B" },
                ],
                [
                    new object?[] { 3, "C" },
                ],
            ]);
        var writer = new RecordingBatchWriter(shape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, writer);

        Assert.True(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.None, result.FailureStage);
        Assert.Equal(string.Empty, result.FailureMessage);
        Assert.Equal(3, result.RowCount);
        Assert.Equal(2, result.BatchCount);
        Assert.Equal(2, writer.Batches.Count);
        Assert.Equal(3, writer.Batches.Sum(static batch => batch.RowCount));
    }

    [Fact]
    public async Task ExecuteAsync_WhenSourceProducesInvalidBatch_ReturnsSourceReadFailureBeforeWritingBatch()
    {
        var shape = CreateShape("CustomerId", "CustomerName");
        var source = new FakeRowStreamSource(
            shape,
            [
                [
                    new object?[] { 1, "A" },
                ],
                [
                    new object?[] { 2 },
                ],
            ]);
        var writer = new RecordingBatchWriter(shape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, writer);

        Assert.False(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.SourceRead, result.FailureStage);
        Assert.Equal(1, result.RowCount);
        Assert.Equal(1, result.BatchCount);
        Assert.Contains("expects 2", result.FailureMessage, StringComparison.Ordinal);
        Assert.Single(writer.Batches);
    }

    [Fact]
    public async Task ExecuteAsync_WhenWriterFails_ReturnsCompletedProgressBeforeFailure()
    {
        var shape = CreateShape("CustomerId");
        var source = new FakeRowStreamSource(
            shape,
            [
                [new object?[] { 1 }],
                [new object?[] { 2 }],
            ]);
        var writer = new FailingSecondBatchWriter(shape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, writer);

        Assert.False(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.TargetWrite, result.FailureStage);
        Assert.Equal(1, result.RowCount);
        Assert.Equal(1, result.BatchCount);
        Assert.Contains("simulated writer failure", result.FailureMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExecuteAsync_WhenWriterShapeDoesNotMatchSourceShape_ReturnsShapeValidationFailureBeforeReading()
    {
        var sourceShape = CreateShape("CustomerId");
        var writerShape = CreateShape("OtherId");
        var source = new FakeRowStreamSource(
            sourceShape,
            [
                [new object?[] { 1 }],
            ]);
        var writer = new RecordingBatchWriter(writerShape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, writer);

        Assert.False(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.ShapeValidation, result.FailureStage);
        Assert.Equal(0, result.RowCount);
        Assert.Equal(0, result.BatchCount);
        Assert.Contains("target writer shape", result.FailureMessage, StringComparison.Ordinal);
        Assert.Empty(writer.Batches);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBatchShapeDoesNotMatchSourceShape_ReturnsShapeValidationFailureBeforeWritingBatch()
    {
        var sourceShape = CreateShape("CustomerId");
        var batchShape = CreateShape("OtherId");
        var source = new PrebuiltBatchRowStreamSource(
            sourceShape,
            [
                new PipelineDataBatch(sourceShape, [new object?[] { 1 }]),
                new PipelineDataBatch(batchShape, [new object?[] { 2 }]),
            ]);
        var writer = new RecordingBatchWriter(sourceShape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, writer);

        Assert.False(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.ShapeValidation, result.FailureStage);
        Assert.Equal(1, result.RowCount);
        Assert.Equal(1, result.BatchCount);
        Assert.Contains("batch 2 shape", result.FailureMessage, StringComparison.Ordinal);
        Assert.Single(writer.Batches);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSourceFailsAfterBatch_ReturnsSourceReadFailureWithCompletedProgress()
    {
        var shape = CreateShape("CustomerId");
        var source = new FailingSecondBatchSource(shape);
        var writer = new RecordingBatchWriter(shape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, writer);

        Assert.False(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.SourceRead, result.FailureStage);
        Assert.Equal(1, result.RowCount);
        Assert.Equal(1, result.BatchCount);
        Assert.Contains("simulated source failure", result.FailureMessage, StringComparison.Ordinal);
    }

    private static PipelineRowStreamShape CreateShape(params string[] columnNames)
    {
        return new PipelineRowStreamShape(
            columnNames
                .Select(static (columnName, index) => new PipelineColumn(columnName, index))
                .ToArray());
    }

    private sealed class FakeRowStreamSource : IPipelineRowStreamSource
    {
        private readonly IReadOnlyList<IReadOnlyList<object?[]>> batches;

        public FakeRowStreamSource(
            PipelineRowStreamShape shape,
            IReadOnlyList<IReadOnlyList<object?[]>> batches)
        {
            Shape = shape;
            this.batches = batches;
        }

        public PipelineRowStreamShape Shape { get; }

        public async IAsyncEnumerable<PipelineDataBatch> ReadBatchesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var rows in batches)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return new PipelineDataBatch(Shape, rows);
            }
        }
    }

    private sealed class PrebuiltBatchRowStreamSource : IPipelineRowStreamSource
    {
        private readonly IReadOnlyList<PipelineDataBatch> batches;

        public PrebuiltBatchRowStreamSource(
            PipelineRowStreamShape shape,
            IReadOnlyList<PipelineDataBatch> batches)
        {
            Shape = shape;
            this.batches = batches;
        }

        public PipelineRowStreamShape Shape { get; }

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

    private sealed class RecordingBatchWriter : IPipelineRowStreamWriter
    {
        public RecordingBatchWriter(PipelineRowStreamShape shape)
        {
            Shape = shape;
        }

        public PipelineRowStreamShape Shape { get; }

        public List<PipelineDataBatch> Batches { get; } = new();

        public Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default)
        {
            Batches.Add(batch);
            return Task.CompletedTask;
        }
    }

    private sealed class FailingSecondBatchWriter : IPipelineRowStreamWriter
    {
        private int callCount;

        public FailingSecondBatchWriter(PipelineRowStreamShape shape)
        {
            Shape = shape;
        }

        public PipelineRowStreamShape Shape { get; }

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

    private sealed class FailingSecondBatchSource : IPipelineRowStreamSource
    {
        public FailingSecondBatchSource(PipelineRowStreamShape shape)
        {
            Shape = shape;
        }

        public PipelineRowStreamShape Shape { get; }

        public async IAsyncEnumerable<PipelineDataBatch> ReadBatchesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            yield return new PipelineDataBatch(Shape, [new object?[] { 1 }]);
            throw new InvalidOperationException("simulated source failure");
        }
    }
}
