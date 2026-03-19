using MetaDataVault.ToMetaSql;
using MetaSql;
using MetaSql.Extractors.SqlServer;

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
        var outputRootPath = Path.GetFullPath(parse.OutputPath);
        var desiredMetaSqlPath = Path.Combine(outputRootPath, "desired-metasql");
        var liveMetaSqlPath = Path.Combine(outputRootPath, "live-metasql");
        var diffRootPath = Path.Combine(outputRootPath, "diff-metasql");

        try
        {
            Directory.CreateDirectory(outputRootPath);

            var desiredWorkspace = await Converter.ConvertAsync(
                workspacePath,
                desiredMetaSqlPath,
                implementationWorkspacePath,
                parse.DatabaseName,
                parse.SchemaName).ConfigureAwait(false);
            await MetaSqlTooling.SaveWorkspaceAsync(desiredWorkspace).ConfigureAwait(false);

            var extractor = new SqlServerMetaSqlExtractor();
            var liveWorkspace = extractor.ExtractMetaSqlWorkspace(new SqlServerExtractRequest
            {
                NewWorkspacePath = liveMetaSqlPath,
                ConnectionString = parse.ConnectionString,
                SchemaName = parse.SchemaName,
            });
            await MetaSqlTooling.SaveWorkspaceAsync(liveWorkspace).ConfigureAwait(false);

            var diffService = new MetaSqlDiffService();
            var diffResult = diffService.BuildEqualDiffWorkspace(
                desiredWorkspace,
                liveWorkspace,
                diffRootPath);

            if (!diffResult.HasDifferences)
            {
                Presenter.WriteOk(
                    "raw metasql parity verified",
                    ("Workspace", workspacePath),
                    ("Database", parse.DatabaseName),
                    ("Schema", parse.SchemaName),
                    ("Verdict", "no difference"));
                return 0;
            }

            await MetaSqlTooling.SaveWorkspaceAsync(diffResult.DiffWorkspace).ConfigureAwait(false);
            return Fail(
                "raw metasql parity mismatch.",
                "inspect the saved MetaSql workspaces and diff workspace.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Database: {parse.DatabaseName}",
                    $"  Schema: {parse.SchemaName}",
                    $"  DesiredMetaSql: {desiredMetaSqlPath}",
                    $"  LiveMetaSql: {liveMetaSqlPath}",
                    $"  DiffMetaSql: {diffResult.DiffWorkspacePath}",
                    $"  LeftRows: {diffResult.LeftRowCount}",
                    $"  RightRows: {diffResult.RightRowCount}",
                    $"  LeftProperties: {diffResult.LeftPropertyCount}",
                    $"  RightProperties: {diffResult.RightPropertyCount}",
                });
        }
        catch (Exception ex)
        {
            return Fail(
                "raw metasql parity check failed.",
                "check the workspace, implementation workspace, connection string, and database name, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Database: {parse.DatabaseName}",
                    $"  Schema: {parse.SchemaName}",
                    $"  Output: {outputRootPath}",
                    $"  {ex.Message}",
                });
        }
    }

    private static (bool Ok, string WorkspacePath, string ImplementationWorkspacePath, string OutputPath, string DatabaseName, string ConnectionString, string SchemaName, string ErrorMessage) ParseGenerateMetaSqlArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var implementationWorkspacePath = string.Empty;
        var outputPath = string.Empty;
        var databaseName = string.Empty;
        var connectionString = string.Empty;
        var schemaName = "dbo";

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "--workspace can only be provided once.");
                workspacePath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--implementation-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "missing value for --implementation-workspace.");
                if (!string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "--implementation-workspace can only be provided once.");
                implementationWorkspacePath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--out", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "missing value for --out.");
                if (!string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "--out can only be provided once.");
                outputPath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--database-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "missing value for --database-name.");
                if (!string.IsNullOrWhiteSpace(databaseName)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "--database-name can only be provided once.");
                databaseName = args[++i];
                continue;
            }
            if (string.Equals(arg, "--connection-string", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "missing value for --connection-string.");
                if (!string.IsNullOrWhiteSpace(connectionString)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "--connection-string can only be provided once.");
                connectionString = args[++i];
                continue;
            }
            if (string.Equals(arg, "--schema", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "missing value for --schema.");
                schemaName = args[++i];
                continue;
            }

            return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "missing required option --implementation-workspace <path>.");
        if (string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "missing required option --out <path>.");
        if (string.IsNullOrWhiteSpace(databaseName)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "missing required option --database-name <name>.");
        if (string.IsNullOrWhiteSpace(connectionString)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "missing required option --connection-string <value>.");
        if (string.IsNullOrWhiteSpace(schemaName)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, "missing required option --schema <name>.");
        return (true, workspacePath, implementationWorkspacePath, outputPath, databaseName, connectionString, schemaName, string.Empty);
    }

    private static void PrintGenerateMetaSqlHelp()
    {
        Presenter.WriteInfo("Command: generate-metasql");
        Presenter.WriteUsage("meta-datavault-raw generate-metasql --workspace <path> --implementation-workspace <path> --database-name <name> --connection-string <value> --schema <name> --out <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Converts the sanctioned MetaRawDataVault workspace to MetaSql.");
        Presenter.WriteInfo("  Extracts the live SQL Server schema to MetaSql for the requested schema.");
        Presenter.WriteInfo("  Runs equal diff and prints a parity verdict.");
        Presenter.WriteInfo("  Saves desired/live/diff MetaSql workspaces under --out so drift is inspectable.");
    }
}
