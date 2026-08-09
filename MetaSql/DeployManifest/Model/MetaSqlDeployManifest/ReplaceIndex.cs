#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class ReplaceIndex
    {
        public string Id { get; set; } = string.Empty;

        public string LiveIndexId { get; set; } = string.Empty;

        public string SourceIndexId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
