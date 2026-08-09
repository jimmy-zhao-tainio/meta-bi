#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class BlockForeignKeyDifference
    {
        public string Id { get; set; } = string.Empty;

        public string DifferenceSummary { get; set; } = string.Empty;

        public string LiveForeignKeyId { get; set; } = string.Empty;

        public string SourceForeignKeyId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
