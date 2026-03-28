using MetaDataVault.ToMetaSql;

internal static partial class Program
{
    private static async Task<int> RunGenerateMetaSqlAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintGenerateMetaSqlHelp();
            return 0;
        }

        var parse = ParseGenerateMetaSqlArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-datavault-raw generate-metasql --help");
        }

        var workspacePath = Path.GetFullPath(parse.WorkspacePath);
        var implementationWorkspacePath = Path.GetFullPath(parse.ImplementationWorkspacePath);
        var outputWorkspacePath = Path.GetFullPath(parse.OutputPath);

        try
        {
            Directory.CreateDirectory(outputWorkspacePath);
            await Converter.ConvertAsync(
                workspacePath,
                outputWorkspacePath,
                implementationWorkspacePath,
                parse.DatabaseName).ConfigureAwait(false);

            Presenter.WriteOk($"Generated {Path.GetFileName(outputWorkspacePath)}");
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "raw metasql generation failed.",
                "check the workspace, implementation workspace, and database name, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Database: {parse.DatabaseName}",
                    $"  Output: {outputWorkspacePath}",
                    $"  {ex.Message}",
                });
        }
    }

    private static (bool Ok, string WorkspacePath, string ImplementationWorkspacePath, string OutputPath, string DatabaseName, string ErrorMessage) ParseGenerateMetaSqlArgs(string[] args, int startIndex)
    {
        var workspacePath = ".";
        var workspaceSpecified = false;
        var implementationWorkspacePath = string.Empty;
        var outputPath = string.Empty;
        var databaseName = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing value for --workspace.");
                if (workspaceSpecified) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "--workspace can only be provided once.");
                workspacePath = args[++i];
                workspaceSpecified = true;
                continue;
            }
            if (string.Equals(arg, "--implementation-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing value for --implementation-workspace.");
                if (!string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "--implementation-workspace can only be provided once.");
                implementationWorkspacePath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--out", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing value for --out.");
                if (!string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "--out can only be provided once.");
                outputPath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--database-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing value for --database-name.");
                if (!string.IsNullOrWhiteSpace(databaseName)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "--database-name can only be provided once.");
                databaseName = args[++i];
                continue;
            }

            return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing required option --implementation-workspace <path>.");
        if (string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing required option --out <path>.");
        if (string.IsNullOrWhiteSpace(databaseName)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing required option --database-name <name>.");
        return (true, workspacePath, implementationWorkspacePath, outputPath, databaseName, string.Empty);
    }

    private static void PrintGenerateMetaSqlHelp()
    {
        Presenter.WriteInfo("Command: generate-metasql");
        Presenter.WriteUsage("meta-datavault-raw generate-metasql [--workspace <path>] --implementation-workspace <path> --database-name <name> --out <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Converts the current sanctioned MetaRawDataVault workspace to a current MetaSql workspace.");
        Presenter.WriteInfo("  Target schema comes from the sanctioned MetaDataVaultImplementation workspace.");
        Presenter.WriteInfo("  Does not query any live database.");
        Presenter.WriteInfo("  Saves the generated current MetaSql workspace at --out.");
        Presenter.WriteInfo("  Defaults to the current working directory when --workspace is omitted.");
    }
}
