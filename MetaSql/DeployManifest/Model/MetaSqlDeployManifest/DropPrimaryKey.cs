#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class DropPrimaryKey
    {
        public string Id { get; set; } = string.Empty;

        public string LivePrimaryKeyId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
