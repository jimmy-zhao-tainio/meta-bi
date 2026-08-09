#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class BlockPrimaryKeyDifference
    {
        public string Id { get; set; } = string.Empty;

        public string DifferenceSummary { get; set; } = string.Empty;

        public string LivePrimaryKeyId { get; set; } = string.Empty;

        public string SourcePrimaryKeyId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
