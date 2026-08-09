#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class DropFunction
    {
        public string Id { get; set; } = string.Empty;

        public string LiveFunctionId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
