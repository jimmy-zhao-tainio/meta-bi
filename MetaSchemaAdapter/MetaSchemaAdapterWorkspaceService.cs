using MetaSchema;

namespace MetaSchemaAdapter;

/// <summary>
/// Creates ordinary MetaSchema workspaces from external provider discovery.
/// </summary>
public sealed class MetaSchemaAdapterWorkspaceService
{
    /// <summary>
    /// Discovers an external system and writes the resulting MetaSchema model to a new workspace.
    /// </summary>
    public async Task<MetaSchemaAdapterDiscoveryWorkspaceResult> DiscoverToWorkspaceAsync(
        IMetaSchemaDiscoveryAdapter adapter,
        MetaSchemaAdapterDiscoveryWorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(adapter);
        ArgumentNullException.ThrowIfNull(request);
        EnsureAdapterId(adapter);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ConnectionReference);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.SystemName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.NewWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Representation);

        var model = await adapter.DiscoverSchemaAsync(
                new MetaSchemaDiscoveryRequest(
                    request.ConnectionReference,
                    request.SystemName),
                cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException(
                $"MetaSchema adapter '{adapter.Id}' returned no discovery model.");
        var workspacePath = Path.GetFullPath(request.NewWorkspacePath);
        var representation = request.Representation.Trim().ToLowerInvariant();

        await Meta.Integration.TypedWorkspaceModelMapper.CreateAsync(
                model,
                workspacePath,
                representation,
                request.ConnectionEnvironmentVariable,
                cancellationToken)
            .ConfigureAwait(false);

        return new MetaSchemaAdapterDiscoveryWorkspaceResult(
            adapter.Id.Trim(),
            workspacePath,
            representation,
            model.SystemList.Count,
            model.SchemaList.Count,
            model.SchemaObjectList.Count,
            model.TableList.Count,
            model.ViewList.Count,
            model.FieldList.Count,
            model.KeyList.Count,
            model.TableRelationshipList.Count);
    }

    private static void EnsureAdapterId(IMetaSchemaAdapter adapter)
    {
        if (string.IsNullOrWhiteSpace(adapter.Id))
        {
            throw new InvalidOperationException("A MetaSchema adapter must have a stable non-empty Id.");
        }
    }
}

/// <summary>
/// Describes one provider discovery that will create a MetaSchema workspace.
/// </summary>
/// <param name="ConnectionReference">The provider-resolved connection name.</param>
/// <param name="SystemName">The system name to use in MetaSchema.</param>
/// <param name="NewWorkspacePath">The new workspace directory.</param>
/// <param name="Representation">The workspace representation: xml, csharp, or sql.</param>
/// <param name="ConnectionEnvironmentVariable">The SQL workspace connection environment variable when the selected representation is sql.</param>
public sealed record MetaSchemaAdapterDiscoveryWorkspaceRequest(
    string ConnectionReference,
    string SystemName,
    string NewWorkspacePath,
    string Representation = "xml",
    string? ConnectionEnvironmentVariable = null);

/// <summary>
/// Reports the workspace and MetaSchema row counts produced by adapter discovery.
/// </summary>
public sealed record MetaSchemaAdapterDiscoveryWorkspaceResult(
    string AdapterId,
    string WorkspacePath,
    string Representation,
    int SystemCount,
    int SchemaCount,
    int SchemaObjectCount,
    int TableCount,
    int ViewCount,
    int FieldCount,
    int KeyCount,
    int TableRelationshipCount);
