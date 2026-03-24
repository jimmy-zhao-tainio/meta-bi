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
        Meta.Core.Domain.Workspace sourceWorkspace)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(sourceWorkspace);

        var sourceFingerprint = MetaSqlInstanceFingerprint.Compute(sourceWorkspace);
        if (!string.Equals(sourceFingerprint, root.SourceInstanceFingerprint, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Manifest source fingerprint mismatch. Recreate deploy-plan from the current source workspace.");
        }
    }

    public void ValidateLiveFingerprint(
        MetaSqlDeployManifest.DeployManifest root,
        Meta.Core.Domain.Workspace liveWorkspace)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(liveWorkspace);

        var liveFingerprint = MetaSqlInstanceFingerprint.Compute(liveWorkspace);
        if (!string.Equals(liveFingerprint, root.LiveInstanceFingerprint, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Manifest live fingerprint mismatch. Live schema changed after deploy-plan. Regenerate the manifest.");
        }
    }
}
