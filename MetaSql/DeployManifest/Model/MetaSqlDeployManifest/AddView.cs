#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class AddView
    {
        public string Id { get; set; } = string.Empty;

        public string SourceViewId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
