#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class AddForeignKey
    {
        public string Id { get; set; } = string.Empty;

        public string SourceForeignKeyId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
