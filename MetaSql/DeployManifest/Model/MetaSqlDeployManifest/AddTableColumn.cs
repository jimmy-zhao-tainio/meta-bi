#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class AddTableColumn
    {
        public string Id { get; set; } = string.Empty;

        public string SourceTableColumnId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
