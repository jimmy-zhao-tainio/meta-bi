namespace MetaSqlDeploy
{
    public sealed class DeployConfiguration
    {
        public string Id { get; internal set; } = string.Empty;
        public string MigrationRoot { get; internal set; } = string.Empty;
        public string RootMode { get; internal set; } = string.Empty;
    }
}
