#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class TruncateTableColumnData
    {
        public string Id { get; set; } = string.Empty;

        public string LiveTableColumnId { get; set; } = string.Empty;

        public string SourceTableColumnId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
