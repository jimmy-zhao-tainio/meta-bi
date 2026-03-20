using Meta.Core.Presentation;
using MetaSql;
using MetaSql.Extractors.SqlServer;

internal static partial class Program
{
    private static readonly ConsolePresenter Presenter = new();

    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return 0;
        }

        if (string.Equals(args[0], "deploy-test", StringComparison.OrdinalIgnoreCase))
        {
            return await RunDeployTestAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", "meta-sql help");
    }

    private static (bool Ok, string SourceWorkspacePath, string OutputPath, string ConnectionString, string? SchemaName, string? TableName, string ErrorMessage) ParseDiffLikeArgs(string[] args, int startIndex)
    {
        var sourceWorkspacePath = string.Empty;
        var outputPath = string.Empty;
        var connectionString = string.Empty;
        string? schemaName = null;
        string? tableName = null;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--source-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionString, schemaName, tableName, "missing value for --source-workspace.");
                if (!string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, sourceWorkspacePath, outputPath, connectionString, schemaName, tableName, "--source-workspace can only be provided once.");
                sourceWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--out", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionString, schemaName, tableName, "missing value for --out.");
                if (!string.IsNullOrWhiteSpace(outputPath)) return (false, sourceWorkspacePath, outputPath, connectionString, schemaName, tableName, "--out can only be provided once.");
                outputPath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--connection-string", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionString, schemaName, tableName, "missing value for --connection-string.");
                if (!string.IsNullOrWhiteSpace(connectionString)) return (false, sourceWorkspacePath, outputPath, connectionString, schemaName, tableName, "--connection-string can only be provided once.");
                connectionString = args[++i];
                continue;
            }

            if (string.Equals(arg, "--schema", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionString, schemaName, tableName, "missing value for --schema.");
                schemaName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--table", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionString, schemaName, tableName, "missing value for --table.");
                tableName = args[++i];
                continue;
            }

            return (false, sourceWorkspacePath, outputPath, connectionString, schemaName, tableName, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, sourceWorkspacePath, outputPath, connectionString, schemaName, tableName, "missing required option --source-workspace <path>.");
        if (string.IsNullOrWhiteSpace(connectionString)) return (false, sourceWorkspacePath, outputPath, connectionString, schemaName, tableName, "missing required option --connection-string <value>.");

        return (true, sourceWorkspacePath, outputPath, connectionString, schemaName, tableName, string.Empty);
    }

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        Presenter.WriteUsage("meta-sql <command> [options]");
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog(
            "Commands:",
            new[]
            {
                ("help", "Show this help."),
                ("deploy-test", "Create a deploy manifest (add/drop/block) from desired vs live MetaSql.")
            });
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-sql deploy-test --help");
    }

    private static int Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details);
        }

        renderedDetails.Add($"Next: {next}");
        Presenter.WriteFailure(message, renderedDetails);
        return exitCode;
    }
}
