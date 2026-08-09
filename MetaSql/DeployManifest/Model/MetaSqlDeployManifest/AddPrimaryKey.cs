#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class AddPrimaryKey
    {
        public string Id { get; set; } = string.Empty;

        public string SourcePrimaryKeyId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
