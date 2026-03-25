using MetaSql;

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

        try
        {
            var deployService = new MetaSqlDeployService();
            var result = await deployService.DeployAsync(
                    new MetaSqlDeployRequest
                    {
                        ManifestWorkspacePath = manifestWorkspacePath,
                        SourceWorkspacePath = sourceWorkspacePath,
                        ConnectionString = parse.ConnectionString,
                    })
                .ConfigureAwait(false);

            Presenter.WriteOk(
                "deploy complete",
                ("AppliedAddCount", result.AppliedAddCount.ToString()),
                ("AppliedDropCount", result.AppliedDropCount.ToString()),
                ("AppliedAlterCount", result.AppliedAlterCount.ToString()),
                ("AppliedTruncateCount", result.AppliedTruncateCount.ToString()),
                ("AppliedReplaceCount", result.AppliedReplaceCount.ToString()),
                ("ExecutedStatementCount", result.ExecutedStatementCount.ToString()));
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
                    $"  {ex.Message}",
                });
        }
    }

    private static void PrintDeployHelp()
    {
        Presenter.WriteInfo("Command: deploy");
        Presenter.WriteUsage("meta-sql deploy --manifest-workspace <path> --source-workspace <path> --connection-string <value>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads the deploy manifest and source MetaSql workspace.");
        Presenter.WriteInfo("  Refuses when the manifest contains Block entries.");
        Presenter.WriteInfo("  Refuses when source/live instance fingerprints no longer match.");
        Presenter.WriteInfo("  Always validates and applies the full manifest scope. Filtered subset deploy is not supported.");
        Presenter.WriteInfo("  Applies add/drop/alter/replace operations in one SQL transaction.");
    }
}
