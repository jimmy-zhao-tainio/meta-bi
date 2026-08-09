#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class DropView
    {
        public string Id { get; set; } = string.Empty;

        public string LiveViewId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
