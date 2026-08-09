#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class DropForeignKey
    {
        public string Id { get; set; } = string.Empty;

        public string LiveForeignKeyId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
