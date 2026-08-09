#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class AddSchema
    {
        public string Id { get; set; } = string.Empty;

        public string SourceSchemaId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
