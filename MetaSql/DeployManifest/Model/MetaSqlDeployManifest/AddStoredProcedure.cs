#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class AddStoredProcedure
    {
        public string Id { get; set; } = string.Empty;

        public string SourceStoredProcedureId { get; set; } = string.Empty;

        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
