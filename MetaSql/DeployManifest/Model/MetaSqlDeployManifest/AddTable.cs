#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class AddTable
    {
        public string Id { get; set; } = string.Empty;

        public string SourceTableId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
