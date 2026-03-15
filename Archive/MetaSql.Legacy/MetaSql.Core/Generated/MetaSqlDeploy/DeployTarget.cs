namespace MetaSqlDeploy
{
    public sealed class DeployTarget
    {
        public string Id { get; internal set; } = string.Empty;
        public string ConnectionString { get; internal set; } = string.Empty;
        public string ConnectionStringEnvVar { get; internal set; } = string.Empty;
        public string DesiredSql { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string TraitsFile { get; internal set; } = string.Empty;
        public string DeployConfigurationId { get; internal set; } = string.Empty;
        public DeployConfiguration DeployConfiguration { get; internal set; } = new DeployConfiguration();
    }
}
