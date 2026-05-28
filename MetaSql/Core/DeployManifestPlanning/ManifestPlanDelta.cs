namespace MetaSql;

/// <summary>
/// Mutable planning delta passed between dispatch/expansion stages.
/// </summary>
internal sealed class ManifestPlanDelta
{
    public required MetaSqlDeployManifest.MetaSqlDeployManifestModel ManifestModel { get; init; }
    public required MetaSqlDeployManifest.DeployManifest Root { get; init; }
    public int AddCount { get; set; }
    public int DropCount { get; set; }
    public int AlterCount { get; set; }
    public int TruncateCount { get; set; }
    public int ReplaceCount { get; set; }
    public int BlockCount { get; set; }

    public MetaSqlDeployManifestBuildResult BuildResult()
    {
        return new MetaSqlDeployManifestBuildResult
        {
            ManifestModel = ManifestModel,
            AddCount = AddCount,
            DropCount = DropCount,
            AlterCount = AlterCount,
            TruncateCount = TruncateCount,
            ReplaceCount = ReplaceCount,
            BlockCount = BlockCount,
        };
    }
}
