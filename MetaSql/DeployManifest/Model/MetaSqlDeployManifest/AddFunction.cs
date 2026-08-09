#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class AddFunction
    {
        public string Id { get; set; } = string.Empty;

        public string SourceFunctionId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
