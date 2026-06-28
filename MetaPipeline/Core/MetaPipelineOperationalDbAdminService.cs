using Meta.Core.Connections;

namespace MetaPipeline;

public sealed class MetaPipelineOperationalDbAdminService
{
    public async Task CreateDatabaseAndBootstrapAsync(
        string connectionEnvironmentVariableName,
        string databaseName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionEnvironmentVariableName);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var connectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
            connectionEnvironmentVariableName);
        await new MetaPipelineOperationalDbStore(connectionString)
            .CreateDatabaseAndBootstrapAsync(databaseName)
            .ConfigureAwait(false);
    }

    public async Task PruneAsync(
        string connectionEnvironmentVariableName,
        int retentionDays,
        bool dryRun)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionEnvironmentVariableName);

        var connectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
            connectionEnvironmentVariableName);
        await new MetaPipelineOperationalDbStore(connectionString)
            .PruneAsync(retentionDays, dryRun)
            .ConfigureAwait(false);
    }
}
