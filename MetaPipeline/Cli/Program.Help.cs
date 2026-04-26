internal static partial class Program
{
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
                ("execute-sqlserver", "Execute the direct SQL Server runtime slice."),
                ("init", "Create an empty MetaPipeline workspace."),
                ("add-pipeline", "Add one Pipeline instance to a MetaPipeline workspace."),
                ("add-transform", "Add transform-backed task instances to a pipeline."),
                ("inspect", "Show a compact MetaPipeline workspace summary."),
                ("help", "Show this help."),
            });
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-pipeline init --help");
    }

    private static void PrintExecuteHelp()
    {
        Presenter.WriteInfo("Command: execute");
        Presenter.WriteUsage("meta-pipeline execute --workspace <path> --pipeline <name> --task <name> --transform-workspace <path> --binding-workspace <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Executes one TransformExecution task declared in a MetaPipeline workspace.");
        Presenter.WriteInfo("  --task is always required; stage 1 executes one transform task per invocation.");
        Presenter.WriteInfo("  The modeled task must feed exactly one TargetWrite task.");
        Presenter.WriteInfo("  Stage 1 execute supports InsertRows target writes, realized through SQL Server bulk copy.");
        Presenter.WriteInfo("  Connection references in the model name shell-visible environment variables.");
        Presenter.WriteInfo("  The command resolves those variable names to connection strings at runtime.");
        Presenter.WriteInfo("Example:");
        Presenter.WriteInfo("  meta-pipeline execute --workspace .\\PipelineWS --pipeline CustomerLoad --task load-customers --transform-workspace .\\TransformWS --binding-workspace .\\BindingWS");
    }

    private static void PrintExecuteSqlServerHelp()
    {
        Presenter.WriteInfo("Command: execute-sqlserver");
        Presenter.WriteUsage("meta-pipeline execute-sqlserver --transform-workspace <path> --binding-workspace <path> --transform-script-id <id> --transform-binding-id <id> --source-connection-env <name> --target-connection-env <name> [--target <sql-identifier>] [--batch-size <n>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Executes one bound transform script using SQL Server source read and SQL Server target write.");
        Presenter.WriteInfo("  --transform-script-id and --transform-binding-id are always required because stage 1 runs one explicit transform binding per execution.");
        Presenter.WriteInfo("  If the selected binding contains multiple targets, --target is required.");
        Presenter.WriteInfo("  --source-connection-env and --target-connection-env name shell-visible environment variables.");
        Presenter.WriteInfo("  The command resolves those variable names to connection strings at runtime.");
        Presenter.WriteInfo("  Stage 1 execution supports parameterless transform scripts and one selected target per run.");
        Presenter.WriteInfo("  --batch-size is the bounded in-memory row buffer size. Default: 1000.");
        Presenter.WriteInfo("Example:");
        Presenter.WriteInfo("  meta-pipeline execute-sqlserver --transform-workspace .\\TransformWS --binding-workspace .\\BindingWS --transform-script-id TransformScript:1 --transform-binding-id TransformScript:1:binding --source-connection-env SOURCE_DB --target-connection-env TARGET_DB");
    }

    private static void PrintInitHelp()
    {
        Presenter.WriteInfo("Command: init");
        Presenter.WriteUsage("meta-pipeline init --new-workspace <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Creates an empty sanctioned MetaPipeline XML workspace.");
    }

    private static void PrintAddPipelineHelp()
    {
        Presenter.WriteInfo("Command: add-pipeline");
        Presenter.WriteUsage("meta-pipeline add-pipeline --workspace <path> --name <name> [--description <text>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Adds one Pipeline instance to an existing MetaPipeline XML workspace.");
    }

    private static void PrintInspectHelp()
    {
        Presenter.WriteInfo("Command: inspect");
        Presenter.WriteUsage("meta-pipeline inspect --workspace <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads a MetaPipeline workspace and prints pipeline/task instance counts.");
    }

    private static void PrintAddTransformHelp()
    {
        Presenter.WriteInfo("Command: add-transform");
        Presenter.WriteUsage("meta-pipeline add-transform --workspace <path> --pipeline <name> --task <name> --transform-workspace <path> --binding-workspace <path> --transform-script-id <id> --transform-binding-id <id> --source-connection-ref <name> --source-connection-env <name> --target-connection-ref <name> --target-connection-env <name> [--target <sql-identifier>] [--batch-size <n>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Records the current executable slice as pipeline task instances.");
        Presenter.WriteInfo("  Reads the selected transform binding and records the declared row-stream columns.");
        Presenter.WriteInfo("  Transform script and binding selection is by Id, not by display/object name.");
        Presenter.WriteInfo("  The transform task produces a row stream; the InsertRows target-write task consumes it.");
        Presenter.WriteInfo("  If the selected binding contains multiple targets, --target is required.");
        Presenter.WriteInfo("  Connection env options name shell-visible environment variables; connection strings are not stored.");
        Presenter.WriteInfo("  Use meta-pipeline execute to execute the modeled transform task.");
        Presenter.WriteInfo("Example:");
        Presenter.WriteInfo("  meta-pipeline add-transform --workspace .\\PipelineWS --pipeline CustomerLoad --task load-customers --transform-workspace .\\TransformWS --binding-workspace .\\BindingWS --transform-script-id TransformScript:1 --transform-binding-id TransformScript:1:binding --source-connection-ref source --source-connection-env SOURCE_DB --target-connection-ref target --target-connection-env TARGET_DB --target dbo.TargetCustomer --batch-size 1000");
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
