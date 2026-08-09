#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class DropTable
    {
        public string Id { get; set; } = string.Empty;

        public string LiveTableId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
