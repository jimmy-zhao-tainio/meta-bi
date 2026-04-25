using Meta.Core.Connections;
using Meta.Core.Presentation;

internal static class Program
{
    private static readonly ConsolePresenter Presenter = new();

    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return 0;
        }

        if (string.Equals(args[0], "execute", StringComparison.OrdinalIgnoreCase))
        {
            return await RunExecuteAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", "meta-pipeline help");
    }

    private static async Task<int> RunExecuteAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintExecuteHelp();
            return 0;
        }

        if (string.Equals(args[1], "sqlserver", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintExecuteSqlServerHelp();
                return 0;
            }

            var parse = ParseExecuteSqlServerArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, "meta-pipeline execute sqlserver --help");
            }

            try
            {
                var sourceConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(parse.SourceConnectionEnvironmentVariableName);
                var targetConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(parse.TargetConnectionEnvironmentVariableName);

                var result = await new MetaPipeline.MetaPipelineSqlServerExecutionService().ExecuteAsync(
                    new MetaPipeline.MetaPipelineSqlServerExecutionRequest(
                        parse.TransformWorkspacePath,
                        parse.BindingWorkspacePath,
                        sourceConnectionString,
                        targetConnectionString,
                        parse.TransformScriptName,
                        parse.TargetSqlIdentifier,
                        parse.BatchSize)).ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    return Fail(
                        "sqlserver execution failed.",
                        "check the selected script, target, and reachable databases, then retry.",
                        4,
                        new[]
                        {
                            $"  Script: {result.TransformScriptName}",
                            $"  Target: {result.TargetSqlIdentifier}",
                            $"  CompletedRows: {result.RowCount}",
                            $"  CompletedBatches: {result.BatchCount}",
                            $"  FailureStage: {result.FailureStage}",
                            $"  Failure: {result.FailureMessage}",
                        });
                }

                Presenter.WriteOk($"Executed {result.RowCount} row{(result.RowCount == 1 ? string.Empty : "s")}");
                Presenter.WriteKeyValueBlock("Execution", new[]
                {
                    ("Status", result.Status.ToString()),
                    ("Script", result.TransformScriptName),
                    ("Target", result.TargetSqlIdentifier),
                    ("Columns", result.ColumnCount.ToString()),
                    ("Rows", result.RowCount.ToString()),
                    ("Batches", result.BatchCount.ToString()),
                });
                return 0;
            }
            catch (ConnectionEnvironmentVariableException ex)
            {
                return Fail(ex.Message, "set the named connection environment variable and retry.");
            }
            catch (MetaPipeline.MetaPipelineConfigurationException ex)
            {
                return Fail(
                    ex.Message,
                    "check the transform/binding workspaces and retry.",
                    4,
                    new[]
                    {
                        $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                        $"  BindingWorkspace: {Path.GetFullPath(parse.BindingWorkspacePath)}",
                    });
            }
            catch (Exception ex)
            {
                return Fail(
                    "sqlserver execution failed.",
                    "check the workspaces, connection environment variables, and reachable databases, then retry.",
                    4,
                    new[]
                    {
                        $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                        $"  BindingWorkspace: {Path.GetFullPath(parse.BindingWorkspacePath)}",
                        $"  SourceConnectionEnv: {parse.SourceConnectionEnvironmentVariableName}",
                        $"  TargetConnectionEnv: {parse.TargetConnectionEnvironmentVariableName}",
                        $"  {ex.Message}",
                    });
            }
        }

        return Fail($"unknown execute target '{args[1]}'.", "meta-pipeline execute --help");
    }

    private static (
        bool Ok,
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        string SourceConnectionEnvironmentVariableName,
        string TargetConnectionEnvironmentVariableName,
        string TransformScriptName,
        string? TargetSqlIdentifier,
        int BatchSize,
        string ErrorMessage) ParseExecuteSqlServerArgs(
        string[] args,
        int startIndex)
    {
        var transformWorkspacePath = string.Empty;
        var bindingWorkspacePath = string.Empty;
        var sourceConnectionEnvironmentVariableName = string.Empty;
        var targetConnectionEnvironmentVariableName = string.Empty;
        var transformScriptName = string.Empty;
        string? targetSqlIdentifier = null;
        var batchSize = 1000;
        var batchSizeSpecified = false;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--transform-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --transform-workspace.");
                if (!string.IsNullOrWhiteSpace(transformWorkspacePath)) return FailParse("--transform-workspace can only be provided once.");
                transformWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--binding-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --binding-workspace.");
                if (!string.IsNullOrWhiteSpace(bindingWorkspacePath)) return FailParse("--binding-workspace can only be provided once.");
                bindingWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--source-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --source-connection-env.");
                if (!string.IsNullOrWhiteSpace(sourceConnectionEnvironmentVariableName)) return FailParse("--source-connection-env can only be provided once.");
                sourceConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--target-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --target-connection-env.");
                if (!string.IsNullOrWhiteSpace(targetConnectionEnvironmentVariableName)) return FailParse("--target-connection-env can only be provided once.");
                targetConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--script", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --script.");
                if (!string.IsNullOrWhiteSpace(transformScriptName)) return FailParse("--script can only be provided once.");
                transformScriptName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--target", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --target.");
                if (!string.IsNullOrWhiteSpace(targetSqlIdentifier)) return FailParse("--target can only be provided once.");
                targetSqlIdentifier = args[++i];
                continue;
            }

            if (string.Equals(arg, "--batch-size", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --batch-size.");
                if (batchSizeSpecified) return FailParse("--batch-size can only be provided once.");
                var raw = args[++i];
                if (!int.TryParse(raw, out batchSize) || batchSize <= 0)
                {
                    return FailParse($"invalid value '{raw}' for --batch-size. Expected a positive integer.");
                }

                batchSizeSpecified = true;
                continue;
            }

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(transformWorkspacePath)) return FailParse("missing required option --transform-workspace <path>.");
        if (string.IsNullOrWhiteSpace(bindingWorkspacePath)) return FailParse("missing required option --binding-workspace <path>.");
        if (string.IsNullOrWhiteSpace(transformScriptName)) return FailParse("missing required option --script <name>.");
        if (string.IsNullOrWhiteSpace(sourceConnectionEnvironmentVariableName)) return FailParse("missing required option --source-connection-env <name>.");
        if (string.IsNullOrWhiteSpace(targetConnectionEnvironmentVariableName)) return FailParse("missing required option --target-connection-env <name>.");

        return (
            true,
            transformWorkspacePath,
            bindingWorkspacePath,
            sourceConnectionEnvironmentVariableName,
            targetConnectionEnvironmentVariableName,
            transformScriptName,
            string.IsNullOrWhiteSpace(targetSqlIdentifier) ? null : targetSqlIdentifier,
            batchSize,
            string.Empty);

        (bool Ok, string TransformWorkspacePath, string BindingWorkspacePath, string SourceConnectionEnvironmentVariableName, string TargetConnectionEnvironmentVariableName, string TransformScriptName, string? TargetSqlIdentifier, int BatchSize, string ErrorMessage) FailParse(string message)
        {
            return (
                false,
                transformWorkspacePath,
                bindingWorkspacePath,
                sourceConnectionEnvironmentVariableName,
                targetConnectionEnvironmentVariableName,
                transformScriptName,
                targetSqlIdentifier,
                batchSize,
                message);
        }
    }

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        Presenter.WriteUsage("meta-pipeline <command> [options]");
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog(
            "Commands:",
            new[]
            {
                ("execute", "Execute one bound transform script and write its row stream."),
                ("help", "Show this help."),
            });
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-pipeline execute --help");
    }

    private static void PrintExecuteHelp()
    {
        Presenter.WriteInfo("Command: execute");
        Presenter.WriteUsage("meta-pipeline execute <target> [options]");
        Presenter.WriteCommandCatalog(
            "Targets:",
            new[]
            {
                ("sqlserver", "Execute one bound transform script from SQL Server source to SQL Server target."),
            });
        Presenter.WriteNext("meta-pipeline execute sqlserver --help");
    }

    private static void PrintExecuteSqlServerHelp()
    {
        Presenter.WriteInfo("Command: execute sqlserver");
        Presenter.WriteUsage("meta-pipeline execute sqlserver --transform-workspace <path> --binding-workspace <path> --script <name> --source-connection-env <name> --target-connection-env <name> [--target <sql-identifier>] [--batch-size <n>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Executes one bound transform script using SQL Server source read and SQL Server target write.");
        Presenter.WriteInfo("  --script is always required because stage 1 runs one transform script per execution.");
        Presenter.WriteInfo("  If the selected binding contains multiple targets, --target is required.");
        Presenter.WriteInfo("  --source-connection-env and --target-connection-env name shell-visible environment variables.");
        Presenter.WriteInfo("  The command resolves those variable names to connection strings at runtime.");
        Presenter.WriteInfo("  Stage 1 execution supports parameterless transform scripts and one selected target per run.");
        Presenter.WriteInfo("  --batch-size is the bounded in-memory row buffer size. Default: 1000.");
        Presenter.WriteInfo("Example:");
        Presenter.WriteInfo("  meta-pipeline execute sqlserver --transform-workspace .\\TransformWS --binding-workspace .\\BindingWS --source-connection-env SOURCE_DB --target-connection-env TARGET_DB --script dbo.v_customer_load");
    }

    private static int Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details.Where(static detail => !string.IsNullOrWhiteSpace(detail)));
        }

        renderedDetails.Add($"Next: {next}");
        Presenter.WriteFailure(message, renderedDetails);
        return exitCode;
    }
}
