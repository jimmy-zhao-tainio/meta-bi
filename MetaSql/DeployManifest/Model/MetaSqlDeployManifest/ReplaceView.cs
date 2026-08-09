#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class ReplaceView
    {
        public string Id { get; set; } = string.Empty;

        public string LiveViewId { get; set; } = string.Empty;

        public string SourceViewId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
