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
        var targetWriteOperation = new RecordingTargetWriteOperation(shape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, targetWriteOperation);

        Assert.True(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.None, result.FailureStage);
        Assert.Equal(string.Empty, result.FailureMessage);
        Assert.Equal(3, result.RowCount);
        Assert.Equal(2, result.BatchCount);
        Assert.True(targetWriteOperation.Began);
        Assert.True(targetWriteOperation.Completed);
        Assert.Equal(2, targetWriteOperation.Batches.Count);
        Assert.Equal(3, targetWriteOperation.Batches.Sum(static batch => batch.RowCount));
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
        var targetWriteOperation = new RecordingTargetWriteOperation(shape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, targetWriteOperation);

        Assert.False(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.SourceRead, result.FailureStage);
        Assert.Equal(1, result.RowCount);
        Assert.Equal(1, result.BatchCount);
        Assert.Contains("expects 2", result.FailureMessage, StringComparison.Ordinal);
        Assert.Single(targetWriteOperation.Batches);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTargetWriteFails_ReturnsCompletedProgressBeforeFailure()
    {
        var shape = CreateShape("CustomerId");
        var source = new FakeRowStreamSource(
            shape,
            [
                [new object?[] { 1 }],
                [new object?[] { 2 }],
            ]);
        var targetWriteOperation = new FailingSecondBatchTargetWriteOperation(shape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, targetWriteOperation);

        Assert.False(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.TargetWrite, result.FailureStage);
        Assert.Equal(1, result.RowCount);
        Assert.Equal(1, result.BatchCount);
        Assert.Contains("simulated target write failure", result.FailureMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTargetWriteShapeDoesNotMatchSourceShape_ReturnsShapeValidationFailureBeforeReading()
    {
        var sourceShape = CreateShape("CustomerId");
        var writerShape = CreateShape("OtherId");
        var source = new FakeRowStreamSource(
            sourceShape,
            [
                [new object?[] { 1 }],
            ]);
        var targetWriteOperation = new RecordingTargetWriteOperation(writerShape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, targetWriteOperation);

        Assert.False(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.ShapeValidation, result.FailureStage);
        Assert.Equal(0, result.RowCount);
        Assert.Equal(0, result.BatchCount);
        Assert.Contains("target write operation shape", result.FailureMessage, StringComparison.Ordinal);
        Assert.False(targetWriteOperation.Began);
        Assert.Empty(targetWriteOperation.Batches);
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
        var targetWriteOperation = new RecordingTargetWriteOperation(sourceShape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, targetWriteOperation);

        Assert.False(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.ShapeValidation, result.FailureStage);
        Assert.Equal(1, result.RowCount);
        Assert.Equal(1, result.BatchCount);
        Assert.Contains("batch 2 shape", result.FailureMessage, StringComparison.Ordinal);
        Assert.Single(targetWriteOperation.Batches);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSourceFailsAfterBatch_ReturnsSourceReadFailureWithCompletedProgress()
    {
        var shape = CreateShape("CustomerId");
        var source = new FailingSecondBatchSource(shape);
        var targetWriteOperation = new RecordingTargetWriteOperation(shape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, targetWriteOperation);

        Assert.False(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.SourceRead, result.FailureStage);
        Assert.Equal(1, result.RowCount);
        Assert.Equal(1, result.BatchCount);
        Assert.Contains("simulated source failure", result.FailureMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTargetWriteBeginFails_ReturnsTargetWriteFailureBeforeReading()
    {
        var shape = CreateShape("CustomerId");
        var source = new FakeRowStreamSource(
            shape,
            [
                [new object?[] { 1 }],
            ]);
        var targetWriteOperation = new FailingBeginTargetWriteOperation(shape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, targetWriteOperation);

        Assert.False(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.TargetWrite, result.FailureStage);
        Assert.Equal(0, result.RowCount);
        Assert.Equal(0, result.BatchCount);
        Assert.Contains("simulated begin failure", result.FailureMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTargetWriteCompleteFails_ReturnsTargetWriteFailureWithCompletedProgress()
    {
        var shape = CreateShape("CustomerId");
        var source = new FakeRowStreamSource(
            shape,
            [
                [new object?[] { 1 }],
                [new object?[] { 2 }],
            ]);
        var targetWriteOperation = new FailingCompleteTargetWriteOperation(shape);

        var result = await new BufferedPipelineExecutionService().ExecuteAsync(source, targetWriteOperation);

        Assert.False(result.Succeeded);
        Assert.Equal(PipelineExecutionFailureStage.TargetWrite, result.FailureStage);
        Assert.Equal(2, result.RowCount);
        Assert.Equal(2, result.BatchCount);
        Assert.Contains("simulated complete failure", result.FailureMessage, StringComparison.Ordinal);
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

    private class RecordingTargetWriteOperation : IPipelineTargetWriteOperation
    {
        public RecordingTargetWriteOperation(PipelineRowStreamShape shape)
        {
            Shape = shape;
        }

        public string Name => "RecordingTargetWrite";

        public PipelineRowStreamShape Shape { get; }

        public List<PipelineDataBatch> Batches { get; } = new();

        public bool Began { get; private set; }

        public bool Completed { get; private set; }

        public ValueTask BeginAsync(CancellationToken cancellationToken = default)
        {
            Began = true;
            return ValueTask.CompletedTask;
        }

        public Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default)
        {
            Batches.Add(batch);
            return Task.CompletedTask;
        }

        public virtual ValueTask CompleteAsync(CancellationToken cancellationToken = default)
        {
            Completed = true;
            return ValueTask.CompletedTask;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    private sealed class FailingSecondBatchTargetWriteOperation : IPipelineTargetWriteOperation
    {
        private int callCount;

        public FailingSecondBatchTargetWriteOperation(PipelineRowStreamShape shape)
        {
            Shape = shape;
        }

        public string Name => "FailingSecondBatchTargetWrite";

        public PipelineRowStreamShape Shape { get; }

        public ValueTask BeginAsync(CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default)
        {
            callCount++;
            if (callCount == 2)
            {
                throw new InvalidOperationException("simulated target write failure");
            }

            return Task.CompletedTask;
        }

        public ValueTask CompleteAsync(CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    private sealed class FailingBeginTargetWriteOperation : IPipelineTargetWriteOperation
    {
        public FailingBeginTargetWriteOperation(PipelineRowStreamShape shape)
        {
            Shape = shape;
        }

        public string Name => "FailingBeginTargetWrite";

        public PipelineRowStreamShape Shape { get; }

        public ValueTask BeginAsync(CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("simulated begin failure");

        public Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public ValueTask CompleteAsync(CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    private sealed class FailingCompleteTargetWriteOperation : RecordingTargetWriteOperation
    {
        public FailingCompleteTargetWriteOperation(PipelineRowStreamShape shape)
            : base(shape)
        {
        }

        public override ValueTask CompleteAsync(CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("simulated complete failure");
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
