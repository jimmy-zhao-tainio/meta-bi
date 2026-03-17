internal static partial class Program
{
    private static Task<int> RunGenerateMetaSqlAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintGenerateMetaSqlHelp();
            return Task.FromResult(0);
        }

        var parse = ParseGenerateMetaSqlArgs(args, 1);
        if (!parse.Ok)
        {
            return Task.FromResult(Fail(parse.ErrorMessage, "meta-datavault-raw generate-metasql --help"));
        }

        return Task.FromResult(Fail(
            "generate-metasql is currently a stub.",
            "DataVault-to-Sql projection has been removed. No MetaSql workspace is generated.",
            4));
    }

    private static (bool Ok, string WorkspacePath, string ImplementationWorkspacePath, string OutputPath, string DatabaseName, string ErrorMessage) ParseGenerateMetaSqlArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var implementationWorkspacePath = string.Empty;
        var outputPath = string.Empty;
        var databaseName = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "--workspace can only be provided once.");
                workspacePath = args[++i];
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

        if (string.IsNullOrWhiteSpace(workspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing required option --implementation-workspace <path>.");
        if (string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing required option --out <path>.");
        if (string.IsNullOrWhiteSpace(databaseName)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing required option --database-name <name>.");
        return (true, workspacePath, implementationWorkspacePath, outputPath, databaseName, string.Empty);
    }

    private static void PrintGenerateMetaSqlHelp()
    {
        Presenter.WriteInfo("Command: generate-metasql");
        Presenter.WriteUsage("meta-datavault-raw generate-metasql --workspace <path> --implementation-workspace <path> --database-name <name> --out <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Stub only. DataVault-to-Sql projection is not currently implemented.");
        Presenter.WriteInfo("  The command surface is kept for future work, but no MetaSql workspace is generated.");
    }
}
