using MetaSql.Extractors.SqlServer;

namespace MetaSql;

/// <summary>
/// Validates source/live instance fingerprints against manifest expectations.
/// </summary>
internal sealed class DeployManifestFingerprintValidator
{
    public void ValidatePreconditions(MetaSqlDeployRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ManifestWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.SourceWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ConnectionString);
    }

    public void ValidateSourceFingerprint(
        MetaSqlDeployManifest.DeployManifest root,
        Meta.Operations.Domain.InMemoryWorkspace sourceWorkspace,
        SqlServerDataTypeCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(sourceWorkspace);

        var sourceFingerprint = MetaSqlInstanceFingerprint.Compute(sourceWorkspace, catalog);
        if (!string.Equals(sourceFingerprint, root.SourceInstanceFingerprint, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Manifest source fingerprint mismatch. The source workspace or selected SQL Server type contract changed, or the manifest uses an older fingerprint format. Regenerate deploy-plan.");
        }
    }

    public void ValidateLiveFingerprint(
        MetaSqlDeployManifest.DeployManifest root,
        Meta.Operations.Domain.InMemoryWorkspace liveWorkspace,
        SqlServerDataTypeCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(liveWorkspace);

        var liveFingerprint = MetaSqlInstanceFingerprint.Compute(liveWorkspace, catalog);
        if (!string.Equals(liveFingerprint, root.LiveInstanceFingerprint, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Manifest live fingerprint mismatch. The live schema or selected SQL Server type contract changed after deploy-plan, or the manifest uses an older fingerprint format. Regenerate deploy-plan.");
        }
    }
}
