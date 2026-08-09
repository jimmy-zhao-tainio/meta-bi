#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class BlockFunctionDifference
    {
        public string Id { get; set; } = string.Empty;

        public string DifferenceSummary { get; set; } = string.Empty;

        public string LiveFunctionId { get; set; } = string.Empty;

        public string SourceFunctionId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
