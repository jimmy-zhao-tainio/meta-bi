using MetaSql;
using Meta.Core.Connections;
using MetaSqlDeployManifest;

internal static partial class Program
{
    private static async Task<int> RunDeployAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintDeployHelp();
            return 0;
        }

        var parse = ParseDeployArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-sql deploy --help");
        }

        var manifestWorkspacePath = Path.GetFullPath(parse.ManifestWorkspacePath);
        var sourceWorkspacePath = Path.GetFullPath(parse.SourceWorkspacePath);

        string connectionString;
        try
        {
            connectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
                parse.ConnectionEnvironmentVariableName);
        }
        catch (ConnectionEnvironmentVariableException exception)
        {
            return Fail(exception.Message, "meta-sql deploy --help");
        }

        try
        {
            var manifestModel = await MetaSqlDeployManifestModel
                .LoadFromXmlWorkspaceAsync(manifestWorkspacePath, searchUpward: false)
                .ConfigureAwait(false);
            var deployService = new MetaSqlDeployService();
            var result = await deployService.DeployAsync(
                    new MetaSqlDeployRequest
                    {
                        ManifestWorkspacePath = manifestWorkspacePath,
                        SourceWorkspacePath = sourceWorkspacePath,
                        ConnectionString = connectionString,
                    })
                .ConfigureAwait(false);

            var details = new List<(string Label, string Value)>
            {
                ("Deployed", FormatManifestDeploySummary(manifestModel)),
            };
            if (result.DatabaseCreated)
            {
                details.Insert(0, ("Database", "created"));
            }

            Presenter.WriteOk("Deployed changes", details.ToArray());
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "deploy failed.",
                "recreate the deploy-plan manifest and retry.",
                5,
                new[]
                {
                    $"  ManifestWorkspace: {manifestWorkspacePath}",
                    $"  SourceWorkspace: {sourceWorkspacePath}",
                    $"  {ConnectionEnvironmentVariableResolver.FormatReference(parse.ConnectionEnvironmentVariableName)}",
                    $"  {ex.Message}",
                });
        }
    }

    private static void PrintDeployHelp()
    {
        Presenter.WriteInfo("Command: deploy");
        Presenter.WriteUsage("meta-sql deploy --manifest-workspace <path> --source-workspace <path> --connection-env <name>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads the deploy manifest and source MetaSql workspace.");
        Presenter.WriteInfo("  --connection-env names the environment variable that contains the SQL Server connection string.");
        Presenter.WriteInfo("  Refuses when the manifest contains Block entries.");
        Presenter.WriteInfo("  Refuses when source/live instance fingerprints no longer match.");
        Presenter.WriteInfo("  Always validates and applies the full manifest scope. Filtered subset deploy is not supported.");
        Presenter.WriteInfo("  Creates the database first when the manifest expects a missing database.");
        Presenter.WriteInfo("  Executes deploy statements without wrapping the full deploy in one SQL transaction.");
        Presenter.WriteInfo("  If later statements fail after database creation, the database remains and the failure reports that explicitly.");
    }
}
