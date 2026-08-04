using Meta.Adapters;
using Meta.Core.Domain;
using Meta.Core.Serialization;
using Meta.Core.Services;

namespace MetaSql;

public sealed class MetaSqlDiffService
{
    private readonly ServiceCollection _services;

    public MetaSqlDiffService()
        : this(new ServiceCollection())
    {
    }

    internal MetaSqlDiffService(ServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public async Task<InstanceDiffBuildResult> BuildEqualDiffWorkspaceAsync(
        string sourceWorkspacePath,
        string liveWorkspacePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(liveWorkspacePath);

        var sourceWorkspace = await XmlWorkspaceReader
            .OpenAsync(sourceWorkspacePath, cancellationToken)
            .ConfigureAwait(false);
        var liveWorkspace = await XmlWorkspaceReader
            .OpenAsync(liveWorkspacePath, cancellationToken)
            .ConfigureAwait(false);

        return BuildEqualDiffWorkspace(
            sourceWorkspace.State,
            liveWorkspace.State);
    }

    public InstanceDiffBuildResult BuildEqualDiffWorkspace(
        InMemoryWorkspace sourceWorkspace,
        InMemoryWorkspace liveWorkspace)
    {
        ArgumentNullException.ThrowIfNull(sourceWorkspace);
        ArgumentNullException.ThrowIfNull(liveWorkspace);
        EnsureMetaSqlWorkspace(sourceWorkspace, nameof(sourceWorkspace));
        EnsureMetaSqlWorkspace(liveWorkspace, nameof(liveWorkspace));

        return _services.InstanceDiffService.BuildEqualDiffWorkspace(
            sourceWorkspace,
            liveWorkspace);
    }

    internal static void EnsureMetaSqlWorkspace(InMemoryWorkspace workspace, string parameterName)
    {
        if (!string.Equals(workspace.Model.Name, "MetaSql", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"{parameterName} must use the MetaSql model. Actual model: '{workspace.Model.Name}'.");
        }

    }
}
