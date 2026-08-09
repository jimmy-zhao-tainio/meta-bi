#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class DropIndex
    {
        public string Id { get; set; } = string.Empty;

        public string LiveIndexId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
