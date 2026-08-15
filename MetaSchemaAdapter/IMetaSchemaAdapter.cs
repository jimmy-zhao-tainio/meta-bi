using MetaPipeline;
using MetaSchema;

namespace MetaSchemaAdapter;

/// <summary>
/// Identifies an external-system provider whose capabilities use MetaSchema contracts.
/// </summary>
public interface IMetaSchemaAdapter
{
    /// <summary>
    /// Gets the stable provider identity.
    /// </summary>
    string Id { get; }
}

/// <summary>
/// Discovers an external system as a MetaSchema model.
/// </summary>
public interface IMetaSchemaDiscoveryAdapter : IMetaSchemaAdapter
{
    /// <summary>
    /// Discovers the requested external system into a MetaSchema model.
    /// </summary>
    ValueTask<MetaSchemaModel> DiscoverSchemaAsync(
        MetaSchemaDiscoveryRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Executes bound MetaTransformScript semantics against an external system.
/// </summary>
public interface IMetaSchemaTransformAdapter : IMetaSchemaAdapter
{
    /// <summary>
    /// Creates a row stream for a transform already resolved by MetaTransformBinding.
    /// </summary>
    ValueTask<IPipelineRowStreamSource> CreateRowStreamSourceAsync(
        MetaSchemaRowStreamRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Executes bound mutating MetaTransformScript semantics against an external system.
/// </summary>
public interface IMetaSchemaMutationAdapter : IMetaSchemaAdapter
{
    /// <summary>
    /// Executes a mutating transform already resolved by MetaTransformBinding.
    /// </summary>
    ValueTask<MetaSchemaMutationResult> ExecuteMutationAsync(
        MetaSchemaMutationRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Creates a MetaPipeline target operation for inserting bound rows.
/// </summary>
public interface IMetaSchemaTargetWriteAdapter : IMetaSchemaAdapter
{
    /// <summary>
    /// Creates the provider-owned write operation consumed by MetaPipeline.
    /// </summary>
    ValueTask<IPipelineTargetWriteOperation> CreateInsertRowsOperationAsync(
        MetaSchemaInsertRowsRequest request,
        CancellationToken cancellationToken = default);
}
