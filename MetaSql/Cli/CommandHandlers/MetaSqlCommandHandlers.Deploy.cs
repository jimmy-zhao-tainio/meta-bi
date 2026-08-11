using MetaSql;
using Meta.Core.Connections;
using MetaSqlDeployManifest;
using MetaCli.Core;

internal sealed partial class MetaSqlCommandHandlers
{
    public async Task<int> RunDeployAsync(MetaCliInvocation invocation)
    {
        var manifestWorkspacePath = Path.GetFullPath(invocation.Required("manifest-workspace"));
        var sourceWorkspacePath = Path.GetFullPath(invocation.Required("source-workspace"));

        string connectionString;
        var connectionEnvironmentVariableName = invocation.Required("connection-env");
        try
        {
            connectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
                connectionEnvironmentVariableName);
        }
        catch (ConnectionEnvironmentVariableException exception)
        {
            return Fail(
                "Cannot deploy SQL manifest.",
                "set the named connection environment variable and retry.",
                4,
                [$"  {exception.Message}"]);
        }

        try
        {
            var deployService = new MetaSqlDeployService();
            var result = await deployService.DeployAsync(
                    new MetaSqlDeployRequest
                    {
                        ManifestWorkspacePath = manifestWorkspacePath,
                        SourceWorkspacePath = sourceWorkspacePath,
                        ConnectionString = connectionString,
                    })
                .ConfigureAwait(false);
            var manifestModel = await Meta.Integration.TypedWorkspaceModelMapper.LoadAsync<MetaSqlDeployManifestModel>(manifestWorkspacePath, searchUpward: false)
                .ConfigureAwait(false);

            var details = new List<(string Label, string Value)>
            {
                ("Deployed", FormatManifestDeploySummary(manifestModel)),
            };
            if (result.DatabaseCreated)
            {
                details.Insert(0, ("Database", "created"));
            }

            Presenter.WriteOk();
            WriteDetails(details);
            return 0;
        }
        catch (Exception ex)
        {
            var details = new List<string>
            {
                $"  ManifestWorkspace: {manifestWorkspacePath}",
                $"  SourceWorkspace: {sourceWorkspacePath}",
                $"  {ConnectionEnvironmentVariableResolver.FormatReference(connectionEnvironmentVariableName)}",
                $"  {ex.Message}",
            };
            details.AddRange(FormatInnerExceptionMessages(ex));
            return Fail(
                "Cannot deploy SQL manifest.",
                "recreate the deploy-plan manifest and retry.",
                5,
                details);
        }
    }

    private static IEnumerable<string> FormatInnerExceptionMessages(Exception exception)
    {
        var inner = exception.InnerException;
        while (inner is not null)
        {
            yield return $"  Caused by: {inner.Message}";
            inner = inner.InnerException;
        }
    }

    private void WriteDetails(IEnumerable<(string Label, string Value)> details)
    {
        Presenter.WriteInfo(string.Empty);
        foreach (var (label, value) in details)
        {
            Presenter.WriteInfo($"{label}: {value}");
        }
    }

}
