#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class BlockTableDifference
    {
        public string Id { get; set; } = string.Empty;

        public string DifferenceSummary { get; set; } = string.Empty;

        public string LiveTableId { get; set; } = string.Empty;

        public string SourceTableId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
