using System.Runtime.CompilerServices;
using MetaPipeline;
using MetaSchema;
using MetaSchemaAdapter;
using MetaTransformBinding;
using MetaTransformScript;

namespace MetaSchemaAdapter.Tests;

public sealed class AdapterContractTests
{
    [Fact]
    public async Task AdapterCapabilitiesMeetAtExistingModeledAndPipelineBoundaries()
    {
        var adapter = new WitnessAdapter();
        var shape = new PipelineRowStreamShape([new PipelineColumn("Value", 0)]);
        var transforms = MetaTransformScriptModel.CreateEmpty();
        var binding = MetaTransformBindingModel.CreateEmpty();

        var schema = await ((IMetaSchemaDiscoveryAdapter)adapter).DiscoverSchemaAsync(
            new MetaSchemaDiscoveryRequest("TEST_SOURCE", "Test"));
        var source = await ((IMetaSchemaTransformAdapter)adapter).CreateRowStreamSourceAsync(
            new MetaSchemaRowStreamRequest(
                "TEST_SOURCE",
                schema,
                transforms,
                binding,
                "transform:test",
                "binding:test",
                shape));
        await using var target = await ((IMetaSchemaTargetWriteAdapter)adapter).CreateInsertRowsOperationAsync(
            new MetaSchemaInsertRowsRequest("TEST_TARGET", schema, "Target.Values", shape));

        var pipelineResult = await new BufferedPipelineExecutionService().ExecuteAsync(source, target);
        var mutationResult = await ((IMetaSchemaMutationAdapter)adapter).ExecuteMutationAsync(
            new MetaSchemaMutationRequest(
                "TEST_SOURCE",
                schema,
                transforms,
                binding,
                "transform:mutation",
                "binding:mutation"));

        Assert.Equal("witness", adapter.Id);
        Assert.IsType<MetaSchemaModel>(schema);
        Assert.Same(schema, adapter.RowStreamRequest!.Schema);
        Assert.Same(transforms, adapter.RowStreamRequest.Transforms);
        Assert.Same(binding, adapter.RowStreamRequest.Binding);
        Assert.Equal("transform:test", adapter.RowStreamRequest.TransformScriptId);
        Assert.Equal("binding:test", adapter.RowStreamRequest.TransformBindingId);
        Assert.Same(transforms, adapter.MutationRequest!.Transforms);
        Assert.Same(binding, adapter.MutationRequest.Binding);
        Assert.True(pipelineResult.Succeeded);
        Assert.Equal(1, pipelineResult.RowCount);
        Assert.Equal(3, mutationResult.AffectedRowCount);
        Assert.Single(adapter.WrittenRows);
        Assert.Equal("value", adapter.WrittenRows[0][0]);
    }

    private sealed class WitnessAdapter :
        IMetaSchemaDiscoveryAdapter,
        IMetaSchemaTransformAdapter,
        IMetaSchemaMutationAdapter,
        IMetaSchemaTargetWriteAdapter
    {
        public string Id => "witness";

        public List<object?[]> WrittenRows { get; } = [];

        public MetaSchemaRowStreamRequest? RowStreamRequest { get; private set; }

        public MetaSchemaMutationRequest? MutationRequest { get; private set; }

        public ValueTask<MetaSchemaModel> DiscoverSchemaAsync(
            MetaSchemaDiscoveryRequest request,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(MetaSchemaModel.CreateEmpty());

        public ValueTask<IPipelineRowStreamSource> CreateRowStreamSourceAsync(
            MetaSchemaRowStreamRequest request,
            CancellationToken cancellationToken = default)
        {
            RowStreamRequest = request;
            return ValueTask.FromResult<IPipelineRowStreamSource>(new WitnessRowStreamSource(request.OutputShape));
        }

        public ValueTask<MetaSchemaMutationResult> ExecuteMutationAsync(
            MetaSchemaMutationRequest request,
            CancellationToken cancellationToken = default)
        {
            MutationRequest = request;
            return ValueTask.FromResult(new MetaSchemaMutationResult(3));
        }

        public ValueTask<IPipelineTargetWriteOperation> CreateInsertRowsOperationAsync(
            MetaSchemaInsertRowsRequest request,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult<IPipelineTargetWriteOperation>(
                new WitnessTargetWriteOperation(request.Shape, WrittenRows));
    }

    private sealed class WitnessRowStreamSource(PipelineRowStreamShape shape) : IPipelineRowStreamSource
    {
        public PipelineRowStreamShape Shape { get; } = shape;

        public async IAsyncEnumerable<PipelineDataBatch> ReadBatchesAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new PipelineDataBatch(Shape, [["value"]]);
            await Task.CompletedTask;
        }
    }

    private sealed class WitnessTargetWriteOperation(
        PipelineRowStreamShape shape,
        List<object?[]> rows) : IPipelineTargetWriteOperation
    {
        public string Name => "WitnessInsertRows";

        public PipelineRowStreamShape Shape { get; } = shape;

        public ValueTask BeginAsync(CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public Task WriteBatchAsync(
            PipelineDataBatch batch,
            CancellationToken cancellationToken = default)
        {
            rows.AddRange(batch.Rows);
            return Task.CompletedTask;
        }

        public ValueTask CompleteAsync(CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
