using MetaSql;
using Meta.Core.Connections;
using MetaSqlDeployManifest;

internal static partial class Program
{
    private static async Task<int> RunDeployAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintCommandHelp("deploy");
            return 0;
        }

        var parse = ParseDeployArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("deploy"));
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
            return Fail(
                "Cannot deploy SQL manifest.",
                "set the named connection environment variable and retry.",
                4,
                [$"  {exception.Message}"]);
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
                $"  {ConnectionEnvironmentVariableResolver.FormatReference(parse.ConnectionEnvironmentVariableName)}",
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

    private static void WriteDetails(IEnumerable<(string Label, string Value)> details)
    {
        Presenter.WriteInfo(string.Empty);
        foreach (var (label, value) in details)
        {
            Presenter.WriteInfo($"{label}: {value}");
        }
    }

}
