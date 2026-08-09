#nullable enable

namespace MetaSqlDeployManifest
{
    public sealed class DeployManifest
    {
        public string Id { get; set; } = string.Empty;

        public string CreatedUtc { get; set; } = string.Empty;

        public string ExpectedLiveDatabasePresence { get; set; } = string.Empty;

        public string LiveInstanceFingerprint { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string SourceInstanceFingerprint { get; set; } = string.Empty;

        public string? TargetDescription { get; set; }

    }
}
