using Meta.Adapters;
using Meta.Core.Domain;
using Meta.Core.Services;

namespace MetaSql;

public sealed class MetaSqlDiffService
{
    private static readonly Lazy<string> ExpectedContractSignature =
        new(() => MetaSqlModel.CreateEmpty()
            .ToXmlWorkspace(Path.Combine(Path.GetTempPath(), "MetaSql.Core", "Contract"))
            .Model
            .ComputeContractSignature());

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
        string desiredWorkspacePath,
        string liveWorkspacePath,
        bool searchUpward = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(desiredWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(liveWorkspacePath);

        var desiredWorkspace = await _services.WorkspaceService
            .LoadAsync(desiredWorkspacePath, searchUpward, cancellationToken)
            .ConfigureAwait(false);
        var liveWorkspace = await _services.WorkspaceService
            .LoadAsync(liveWorkspacePath, searchUpward, cancellationToken)
            .ConfigureAwait(false);

        return BuildEqualDiffWorkspace(
            desiredWorkspace,
            liveWorkspace,
            liveWorkspace.WorkspaceRootPath ?? Path.GetFullPath(liveWorkspacePath));
    }

    public InstanceDiffBuildResult BuildEqualDiffWorkspace(
        Workspace desiredWorkspace,
        Workspace liveWorkspace,
        string liveWorkspacePath)
    {
        ArgumentNullException.ThrowIfNull(desiredWorkspace);
        ArgumentNullException.ThrowIfNull(liveWorkspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(liveWorkspacePath);

        EnsureMetaSqlWorkspace(desiredWorkspace, nameof(desiredWorkspace));
        EnsureMetaSqlWorkspace(liveWorkspace, nameof(liveWorkspace));

        return _services.InstanceDiffService.BuildEqualDiffWorkspace(
            desiredWorkspace,
            liveWorkspace,
            liveWorkspacePath);
    }

    private static void EnsureMetaSqlWorkspace(Workspace workspace, string parameterName)
    {
        if (!string.Equals(workspace.Model.Name, "MetaSql", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"{parameterName} must use the MetaSql model. Actual model: '{workspace.Model.Name}'.");
        }

        var actualSignature = workspace.Model.ComputeContractSignature();
        if (!string.Equals(actualSignature, ExpectedContractSignature.Value, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"{parameterName} does not match the sanctioned MetaSql contract.");
        }
    }
}
