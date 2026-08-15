using MetaPipeline;
using MetaSchema;
using MetaTransformBinding;
using MetaTransformScript;

namespace MetaSchemaAdapter;

/// <summary>
/// Executes an existing bound transform through external provider capabilities and MetaPipeline.
/// </summary>
public sealed class MetaSchemaAdapterExecutionService
{
    private readonly MetaPipelineExecutionWorkspaceResolver workspaceResolver;
    private readonly BufferedPipelineExecutionService bufferedExecutionService;

    public MetaSchemaAdapterExecutionService(
        MetaPipelineExecutionWorkspaceResolver? workspaceResolver = null,
        BufferedPipelineExecutionService? bufferedExecutionService = null)
    {
        this.workspaceResolver = workspaceResolver ?? new MetaPipelineExecutionWorkspaceResolver();
        this.bufferedExecutionService = bufferedExecutionService ?? new BufferedPipelineExecutionService();
    }

    /// <summary>
    /// Resolves the selected transform and binding, then executes the corresponding provider capability.
    /// </summary>
    public async Task<MetaSchemaAdapterExecutionResult> ExecuteAsync(
        IMetaSchemaAdapter executionAdapter,
        IMetaSchemaAdapter? targetAdapter,
        MetaSchemaAdapterExecutionRequest request,
        IProgress<BufferedPipelineExecutionProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(executionAdapter);
        ArgumentNullException.ThrowIfNull(request);
        EnsureAdapterId(executionAdapter, "execution");

        var definition = workspaceResolver.ResolveByIds(
            request.TransformWorkspacePath,
            request.BindingWorkspacePath,
            request.TransformScriptId,
            request.TransformBindingId,
            request.TargetIdentifier);
        var schema = await LoadWorkspaceAsync<MetaSchemaModel>(
                request.ExecutionSchemaWorkspacePath,
                cancellationToken)
            .ConfigureAwait(false);
        var transforms = await LoadWorkspaceAsync<MetaTransformScriptModel>(
                request.TransformWorkspacePath,
                cancellationToken)
            .ConfigureAwait(false);
        var binding = await LoadWorkspaceAsync<MetaTransformBindingModel>(
                request.BindingWorkspacePath,
                cancellationToken)
            .ConfigureAwait(false);

        if (!definition.IsSelect)
        {
            if (executionAdapter is not IMetaSchemaMutationAdapter mutationAdapter)
            {
                throw MissingCapability(executionAdapter, nameof(IMetaSchemaMutationAdapter));
            }

            var mutation = await mutationAdapter.ExecuteMutationAsync(
                    new MetaSchemaMutationRequest(
                        request.ExecutionConnectionReference,
                        schema,
                        transforms,
                        binding,
                        definition.TransformScriptId,
                        definition.TransformBindingId,
                        request.TimeoutSeconds,
                        request.ExecutionContext),
                    cancellationToken)
                .ConfigureAwait(false)
                ?? throw new InvalidOperationException(
                    $"MetaSchema adapter '{executionAdapter.Id}' returned no mutation result.");
            return new MetaSchemaAdapterMutationExecutionResult(
                definition,
                executionAdapter.Id.Trim(),
                mutation);
        }

        if (request.BatchSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request),
                request.BatchSize,
                "BatchSize must be greater than zero.");
        }

        if (executionAdapter is not IMetaSchemaTransformAdapter transformAdapter)
        {
            throw MissingCapability(executionAdapter, nameof(IMetaSchemaTransformAdapter));
        }

        if (targetAdapter is not IMetaSchemaTargetWriteAdapter targetWriteAdapter)
        {
            throw new InvalidOperationException(
                "A row-producing transform requires a target adapter implementing " +
                $"{nameof(IMetaSchemaTargetWriteAdapter)}.");
        }

        EnsureAdapterId(targetWriteAdapter, "target");
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TargetConnectionReference);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TargetSchemaWorkspacePath);
        var targetIdentifier = definition.TargetSqlIdentifier
            ?? throw new InvalidOperationException(
                $"Bound transform '{definition.TransformScriptName}' does not resolve a target identifier.");
        var outputShape = definition.RowStreamShape
            ?? throw new InvalidOperationException(
                $"Bound transform '{definition.TransformScriptName}' does not resolve an output row shape.");
        var targetSchema = await LoadWorkspaceAsync<MetaSchemaModel>(
                request.TargetSchemaWorkspacePath,
                cancellationToken)
            .ConfigureAwait(false);

        var source = await transformAdapter.CreateRowStreamSourceAsync(
                new MetaSchemaRowStreamRequest(
                    request.ExecutionConnectionReference,
                    schema,
                    transforms,
                    binding,
                    definition.TransformScriptId,
                    definition.TransformBindingId,
                    outputShape,
                    request.BatchSize,
                    request.TimeoutSeconds,
                    request.ExecutionContext),
                cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException(
                $"MetaSchema adapter '{executionAdapter.Id}' returned no row-stream source.");
        var target = await targetWriteAdapter.CreateInsertRowsOperationAsync(
                new MetaSchemaInsertRowsRequest(
                    request.TargetConnectionReference,
                    targetSchema,
                    targetIdentifier,
                    outputShape,
                    request.TimeoutSeconds,
                    request.ExecutionContext),
                cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException(
                $"MetaSchema adapter '{targetWriteAdapter.Id}' returned no target write operation.");

        await using (target.ConfigureAwait(false))
        {
            var execution = await bufferedExecutionService.ExecuteAsync(
                    source,
                    target,
                    progress,
                    cancellationToken)
                .ConfigureAwait(false);
            return new MetaSchemaAdapterRowStreamExecutionResult(
                definition,
                executionAdapter.Id.Trim(),
                targetWriteAdapter.Id.Trim(),
                execution);
        }
    }

    private static Task<TModel> LoadWorkspaceAsync<TModel>(
        string workspacePath,
        CancellationToken cancellationToken)
        where TModel : class, new()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.LoadAsync<TModel>(
            Path.GetFullPath(workspacePath),
            searchUpward: false,
            cancellationToken);
    }

    private static void EnsureAdapterId(IMetaSchemaAdapter adapter, string role)
    {
        if (string.IsNullOrWhiteSpace(adapter.Id))
        {
            throw new InvalidOperationException(
                $"The {role} MetaSchema adapter must have a stable non-empty Id.");
        }
    }

    private static InvalidOperationException MissingCapability(
        IMetaSchemaAdapter adapter,
        string capability) =>
        new($"MetaSchema adapter '{adapter.Id}' does not implement {capability}.");
}

/// <summary>
/// Selects one existing transform and binding for provider execution.
/// </summary>
public sealed record MetaSchemaAdapterExecutionRequest(
    string ExecutionSchemaWorkspacePath,
    string? TargetSchemaWorkspacePath,
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    string ExecutionConnectionReference,
    string? TargetConnectionReference,
    string TransformScriptId,
    string TransformBindingId,
    string? TargetIdentifier = null,
    int BatchSize = 1000,
    int? TimeoutSeconds = null,
    MetaPipelineExecutionContext? ExecutionContext = null);

/// <summary>
/// Describes one provider execution resolved through the existing transform and binding workspaces.
/// </summary>
public abstract record MetaSchemaAdapterExecutionResult(
    MetaPipelineExecutionDefinition Definition,
    string ExecutionAdapterId);

/// <summary>
/// Reports a row-producing provider execution performed by MetaPipeline.
/// </summary>
public sealed record MetaSchemaAdapterRowStreamExecutionResult(
    MetaPipelineExecutionDefinition Definition,
    string ExecutionAdapterId,
    string TargetAdapterId,
    BufferedPipelineExecutionResult PipelineResult)
    : MetaSchemaAdapterExecutionResult(Definition, ExecutionAdapterId);

/// <summary>
/// Reports a mutating provider execution.
/// </summary>
public sealed record MetaSchemaAdapterMutationExecutionResult(
    MetaPipelineExecutionDefinition Definition,
    string ExecutionAdapterId,
    MetaSchemaMutationResult MutationResult)
    : MetaSchemaAdapterExecutionResult(Definition, ExecutionAdapterId);
