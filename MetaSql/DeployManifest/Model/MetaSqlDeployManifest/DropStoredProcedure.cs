#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class DropStoredProcedure
    {
        public string Id { get; set; } = string.Empty;

        public string LiveStoredProcedureId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
