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
                parse.DatabaseName,
                parse.SchemaName).ConfigureAwait(false);

            Presenter.WriteOk(
                "raw metasql generated",
                ("Workspace", workspacePath),
                ("Database", parse.DatabaseName),
                ("Schema", parse.SchemaName),
                ("CurrentMetaSqlWorkspace", outputWorkspacePath));
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "raw metasql generation failed.",
                "check the workspace, implementation workspace, database name, and schema, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Database: {parse.DatabaseName}",
                    $"  Schema: {parse.SchemaName}",
                    $"  Output: {outputWorkspacePath}",
                    $"  {ex.Message}",
                });
        }
    }

    private static (bool Ok, string WorkspacePath, string ImplementationWorkspacePath, string OutputPath, string DatabaseName, string SchemaName, string ErrorMessage) ParseGenerateMetaSqlArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var implementationWorkspacePath = string.Empty;
        var outputPath = string.Empty;
        var databaseName = string.Empty;
        var schemaName = "dbo";

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "--workspace can only be provided once.");
                workspacePath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--implementation-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "missing value for --implementation-workspace.");
                if (!string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "--implementation-workspace can only be provided once.");
                implementationWorkspacePath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--out", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "missing value for --out.");
                if (!string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "--out can only be provided once.");
                outputPath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--database-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "missing value for --database-name.");
                if (!string.IsNullOrWhiteSpace(databaseName)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "--database-name can only be provided once.");
                databaseName = args[++i];
                continue;
            }
            if (string.Equals(arg, "--schema", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "missing value for --schema.");
                schemaName = args[++i];
                continue;
            }

            return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "missing required option --implementation-workspace <path>.");
        if (string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "missing required option --out <path>.");
        if (string.IsNullOrWhiteSpace(databaseName)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "missing required option --database-name <name>.");
        if (string.IsNullOrWhiteSpace(schemaName)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, "missing required option --schema <name>.");
        return (true, workspacePath, implementationWorkspacePath, outputPath, databaseName, schemaName, string.Empty);
    }

    private static void PrintGenerateMetaSqlHelp()
    {
        Presenter.WriteInfo("Command: generate-metasql");
        Presenter.WriteUsage("meta-datavault-raw generate-metasql --workspace <path> --implementation-workspace <path> --database-name <name> --schema <name> --out <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Converts the current sanctioned MetaRawDataVault workspace to a current MetaSql workspace.");
        Presenter.WriteInfo("  Does not query any live database.");
        Presenter.WriteInfo("  Saves the generated current MetaSql workspace at --out.");
    }
}
