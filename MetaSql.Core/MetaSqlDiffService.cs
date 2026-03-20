using Meta.Adapters;
using Meta.Core.Domain;
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
        bool searchUpward = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(liveWorkspacePath);

        var sourceWorkspace = await _services.WorkspaceService
            .LoadAsync(sourceWorkspacePath, searchUpward, cancellationToken)
            .ConfigureAwait(false);
        var liveWorkspace = await _services.WorkspaceService
            .LoadAsync(liveWorkspacePath, searchUpward, cancellationToken)
            .ConfigureAwait(false);

        return BuildEqualDiffWorkspace(
            sourceWorkspace,
            liveWorkspace,
            liveWorkspace.WorkspaceRootPath ?? Path.GetFullPath(liveWorkspacePath));
    }

    public InstanceDiffBuildResult BuildEqualDiffWorkspace(
        Workspace sourceWorkspace,
        Workspace liveWorkspace,
        string liveWorkspacePath)
    {
        ArgumentNullException.ThrowIfNull(sourceWorkspace);
        ArgumentNullException.ThrowIfNull(liveWorkspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(liveWorkspacePath);

        EnsureMetaSqlWorkspace(sourceWorkspace, nameof(sourceWorkspace));
        EnsureMetaSqlWorkspace(liveWorkspace, nameof(liveWorkspace));

        return _services.InstanceDiffService.BuildEqualDiffWorkspace(
            sourceWorkspace,
            liveWorkspace,
            liveWorkspacePath);
    }

    internal static void EnsureMetaSqlWorkspace(Workspace workspace, string parameterName)
    {
        if (!string.Equals(workspace.Model.Name, "MetaSql", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"{parameterName} must use the MetaSql model. Actual model: '{workspace.Model.Name}'.");
        }

    }
}
