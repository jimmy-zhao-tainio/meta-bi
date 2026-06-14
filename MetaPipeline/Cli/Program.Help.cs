using Meta.Core.Presentation.Cli;

internal static partial class Program
{
    private static readonly Lazy<IReadOnlyList<CliCommandRoute>> CommandRoutesLazy = new(BuildCommandRoutes);
    private static readonly Lazy<IReadOnlyDictionary<string, CliCommandRoute>> CommandRoutesByNameLazy = new(
        () => CommandRoutes.ToDictionary(route => route.Definition.Name, StringComparer.OrdinalIgnoreCase));
    private static readonly Lazy<CliAppDefinition> CliLazy = new(
        () => new CliAppDefinition(
            "meta-pipeline",
            new[]
            {
                "meta-pipeline --new-workspace <path>",
                "meta-pipeline <command> [options]",
            },
            CommandRoutes.Select(route => route.Definition).ToArray(),
            new[]
            {
                "--new-workspace creates an empty sanctioned MetaPipeline workspace."
            },
            "meta-pipeline add-pipeline --help"));

    private static IReadOnlyList<CliCommandRoute> CommandRoutes => CommandRoutesLazy.Value;

    private static IReadOnlyDictionary<string, CliCommandRoute> CommandRoutesByName => CommandRoutesByNameLazy.Value;

    private static CliAppDefinition Cli => CliLazy.Value;

    internal static CliAppDefinition CreateAppDefinition() => Cli;

    private static IReadOnlyList<CliCommandRoute> BuildCommandRoutes() =>
        new[]
        {
            new CliCommandRoute(
                new CliCommandDefinition(
                    "execute",
                    "Execute a modeled pipeline's serial task chain.",
                    new[]
                    {
                        "meta-pipeline execute --workspace <path> --pipeline <name> [--data-type-conversion-workspace <path>] [--pipeline-db-connection-env <name>]"
                    },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaPipeline workspace that contains the modeled serial task chain."),
                        new CliOptionDefinition("--pipeline <name>", "Required. Pipeline name to execute."),
                        new CliOptionDefinition("--data-type-conversion-workspace <path>", "Optional conversion policy workspace; omitted uses the built-in defaults."),
                        new CliOptionDefinition("--pipeline-db-connection-env <name>", "Optional shell-visible environment variable for an initialized MetaPipeline operational DB.")
                    },
                    new[]
                    {
                        "Executes the serial PipelineTask chain declared in a MetaPipeline workspace.",
                        "Every transform task carries its own modeled transform workspace and binding workspace.",
                        "Executable tasks do not require transform or binding workspaces.",
                        "SELECT-kind scripts must feed exactly one InsertRows target write.",
                        "Non-SELECT scripts execute directly and must not feed a TargetWrite task.",
                        "Connection references in the model name shell-visible environment variables.",
                        "The command resolves those variable names to connection strings at runtime.",
                        "SELECT-kind InsertRows tasks use their modeled target data type system; omitted defaults to SqlServer.",
                        "--pipeline-db-connection-env records diagnostic logs, audit logs, task runs, metrics, fingerprints, audit ids, and failures in an initialized operational DB.",
                        "In an attached console, execution shows compact live progress with step count, elapsed time, rows, batches, and B/KB/MB/GB rate.",
                        "The command validates the modeled pipeline before execution."
                    },
                    new[]
                    {
                        "meta-pipeline execute --workspace .\\PipelineWS --pipeline CustomerLoad"
                    }),
                RunExecuteAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "execute-worker",
                    "Execute a modeled pipeline under an orchestration worker protocol.",
                    new[]
                    {
                        "meta-pipeline execute-worker --workspace <path> --pipeline <name> --control-pipe <name> [--control-pipe-connect-timeout-seconds <n>] [--data-type-conversion-workspace <path>] [--pipeline-db-connection-env <name>]"
                    },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaPipeline workspace that contains the modeled serial task chain."),
                        new CliOptionDefinition("--pipeline <name>", "Required. Pipeline name to execute as a worker."),
                        new CliOptionDefinition("--control-pipe <name>", "Required. Named pipe used for orchestration worker control messages."),
                        new CliOptionDefinition("--control-pipe-connect-timeout-seconds <n>", "Optional timeout while connecting to the orchestration control pipe. 0 or omitted means no timeout."),
                        new CliOptionDefinition("--data-type-conversion-workspace <path>", "Optional conversion policy workspace; omitted uses the built-in defaults."),
                        new CliOptionDefinition("--pipeline-db-connection-env <name>", "Optional shell-visible environment variable for an initialized MetaPipeline operational DB.")
                    },
                    new[]
                    {
                        "This command is an orchestration worker boundary, not an interactive user surface.",
                        "The process loads the whole modeled pipeline once and preserves that pipeline context.",
                        "It uses the named pipe control channel for typed WorkerOnline/WorkerReady/PipelineStarted/TaskReady events and StartPipeline, GrantTask, StopPipeline, or FailPipeline commands.",
                        "The worker waits for StartPipeline before it emits PipelineStarted or any TaskReady task boundary.",
                        "If StartPipeline carries a task id, the worker resumes at that task boundary and does not replay earlier tasks in the same pipeline.",
                        "stdout and stderr are diagnostics only; they are not the worker control plane.",
                        "After TaskFailed it waits at the failed task boundary for retry, stop, or fail commands instead of advancing automatically.",
                        "MetaOrchestration owns cross-pipeline task synchronization; MetaPipeline owns in-process pipeline execution and operational DB evidence."
                    },
                    new[]
                    {
                        "meta-pipeline execute-worker --workspace .\\PipelineWS --pipeline CustomerLoad --control-pipe meta-worker-123"
                    }),
                RunExecuteWorkerAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "execute-step",
                    "Execute one modeled pipeline step.",
                    new[]
                    {
                        "meta-pipeline execute-step --workspace <path> --pipeline <name> --step-name <name-or-id> [--data-type-conversion-workspace <path>] [--pipeline-db-connection-env <name>]"
                    },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaPipeline workspace that contains the modeled step."),
                        new CliOptionDefinition("--pipeline <name>", "Required. Pipeline name containing the step."),
                        new CliOptionDefinition("--step-name <name-or-id>", "Required. Pipeline task name or id to execute."),
                        new CliOptionDefinition("--data-type-conversion-workspace <path>", "Optional conversion policy workspace; omitted uses the built-in defaults."),
                        new CliOptionDefinition("--pipeline-db-connection-env <name>", "Optional shell-visible environment variable for an initialized MetaPipeline operational DB.")
                    },
                    new[]
                    {
                        "Executes exactly one PipelineTask declared in a MetaPipeline workspace.",
                        "Executable steps can be selected without transform or binding workspaces.",
                        "The command does not traverse predecessor or successor tasks.",
                        "SELECT-kind steps execute their paired InsertRows target write when modeled.",
                        "Non-SELECT steps execute directly through the modeled execution connection.",
                        "Connection references in the model name shell-visible environment variables.",
                        "This command is a diagnostic/debugging surface. MetaOrchestration uses execute-worker so pipeline context is not erased between tasks."
                    },
                    new[]
                    {
                        "meta-pipeline execute-step --workspace .\\PipelineWS --pipeline CustomerLoad --step-name load-customers"
                    }),
                RunExecuteStepAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "execute-sqlserver",
                    "Execute the direct SQL Server runtime slice.",
                    new[]
                    {
                        "meta-pipeline execute-sqlserver --transform-workspace <path> --binding-workspace <path> --script <name-or-id> [--binding <id>] --execution-connection-env <name> [--target-connection-env <name>] [--target <sql-identifier>] [--batch-size <n>] [--timeout-seconds <n>] [--target-data-type-system <name>] [--data-type-conversion-workspace <path>] [--pipeline-db-connection-env <name>]"
                    },
                    new[]
                    {
                        new CliOptionDefinition("--transform-workspace <path>", "Required. MetaTransformScript workspace containing the script."),
                        new CliOptionDefinition("--binding-workspace <path>", "Required. MetaTransformBinding workspace containing script binding rows."),
                        new CliOptionDefinition("--script <name-or-id>", "Required. TransformScript.Name or TransformScript.Id to execute."),
                        new CliOptionDefinition("--binding <id>", "Optional binding id when the selected script has multiple bindings."),
                        new CliOptionDefinition("--execution-connection-env <name>", "Required. Shell-visible environment variable for the execution SQL Server connection."),
                        new CliOptionDefinition("--target-connection-env <name>", "Required for SELECT-kind scripts. Shell-visible environment variable for the target SQL Server connection."),
                        new CliOptionDefinition("--target <sql-identifier>", "Target table identifier when a SELECT binding has multiple targets."),
                        new CliOptionDefinition("--batch-size <n>", "Bounded in-memory row buffer size. Default: 1000."),
                        new CliOptionDefinition("--timeout-seconds <n>", "SQL command and bulk-copy timeout seconds. 0 or omitted means no command timeout."),
                        new CliOptionDefinition("--target-data-type-system <name>", "Runtime target type family for InsertRows. Default: SqlServer."),
                        new CliOptionDefinition("--data-type-conversion-workspace <path>", "Optional conversion policy workspace; omitted uses the built-in defaults."),
                        new CliOptionDefinition("--pipeline-db-connection-env <name>", "Optional shell-visible environment variable for an initialized MetaPipeline operational DB.")
                    },
                    new[]
                    {
                        "Executes one transform script against SQL Server.",
                        "--script resolves exact TransformScript.Name first, then exact TransformScript.Id.",
                        "If exactly one binding references the selected script, --binding can be omitted.",
                        "Use --binding only when the selected script has multiple bindings.",
                        "SELECT-kind scripts additionally require --target-connection-env.",
                        "Non-SELECT scripts execute directly through the execution connection and do not use a target.",
                        "If a SELECT binding contains multiple targets, --target is required.",
                        "Connection env options name shell-visible environment variables.",
                        "The command resolves those variable names to connection strings at runtime.",
                        "Stage 1 execution supports parameterless transform scripts and one selected target per run.",
                        "--data-type-conversion-workspace selects the conversion policy workspace; omitted uses the built-in defaults.",
                        "--pipeline-db-connection-env records diagnostic logs, audit logs, task runs, metrics, fingerprints, audit ids, and failures in an initialized operational DB.",
                        "In an attached console, execution shows compact live progress with step count, elapsed time, rows, batches, and B/KB/MB/GB rate."
                    },
                    new[]
                    {
                        "meta-pipeline execute-sqlserver --transform-workspace .\\TransformWS --binding-workspace .\\BindingWS --script dbo.v_customer_load --execution-connection-env EXECUTION_DB --target-connection-env TARGET_DB"
                    }),
                RunExecuteSqlServerAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "create-pipeline-db",
                    "Create or update the MetaPipeline operational DB.",
                    new[]
                    {
                        "meta-pipeline create-pipeline-db --pipeline-db-connection-env <name> [--pipeline-db-name <name>]"
                    },
                    new[]
                    {
                        new CliOptionDefinition("--pipeline-db-connection-env <name>", "Required. Shell-visible environment variable with a SQL Server connection string that can create the database."),
                        new CliOptionDefinition("--pipeline-db-name <name>", "Operational database name. Default: MetaPipeline.")
                    },
                    new[]
                    {
                        "Creates the SQL Server MetaPipeline operational database if needed and creates or updates its operational schema.",
                        "--pipeline-db-name defaults to MetaPipeline.",
                        "The operational DB stores diagnostic logs, audit logs, metrics, task runs, workspace fingerprints, audit ids, and failures only.",
                        "It does not store model truth, scheduling state, watermarks, checkpoints, or orchestration semantics."
                    },
                    new[]
                    {
                        "meta-pipeline create-pipeline-db --pipeline-db-connection-env META_PIPELINE_SQLSERVER --pipeline-db-name MetaPipeline"
                    }),
                args => RunCommandWithHelpAsync(args, "create-pipeline-db", commandArgs => RunCreatePipelineDbAsync(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "prune-pipeline-db",
                    "Prune old MetaPipeline operational diagnostic logs.",
                    new[]
                    {
                        "meta-pipeline prune-pipeline-db --pipeline-db-connection-env <name> --retention-days <days> [--dry-run]"
                    },
                    new[]
                    {
                        new CliOptionDefinition("--pipeline-db-connection-env <name>", "Required. Shell-visible environment variable for the initialized MetaPipeline operational DB."),
                        new CliOptionDefinition("--retention-days <days>", "Required. Delete eligible diagnostic rows older than this retention window."),
                        new CliOptionDefinition("--dry-run", "Report eligible rows without deleting them.")
                    },
                    new[]
                    {
                        "Deletes only RunDiagnosticsLog rows for completed runs older than the retention window.",
                        "PipelineRun, TaskRun, RunMetric, RunLog, RunFingerprint, RunFailure, and audit ids are preserved for audit lineage.",
                        "Running runs are not touched because only completed runs with CompletedAtUtc older than the cutoff are eligible.",
                        "This is explicit maintenance; meta-pipeline does not install SQL Agent jobs."
                    },
                    new[]
                    {
                        "meta-pipeline prune-pipeline-db --pipeline-db-connection-env META_PIPELINE_DB --retention-days 30 --dry-run"
                    }),
                args => RunCommandWithHelpAsync(args, "prune-pipeline-db", commandArgs => RunPrunePipelineDbAsync(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "add-pipeline",
                    "Add one Pipeline instance to a MetaPipeline workspace.",
                    new[]
                    {
                        "meta-pipeline add-pipeline --workspace <path> --name <name> [--description <text>]"
                    },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. Existing MetaPipeline workspace to update."),
                        new CliOptionDefinition("--name <name>", "Required. Pipeline name."),
                        new CliOptionDefinition("--description <text>", "Optional pipeline description.")
                    },
                    new[]
                    {
                        "Adds one Pipeline instance to an existing MetaPipeline workspace."
                    }),
                args => RunCommandWithHelpAsync(args, "add-pipeline", commandArgs => RunAddPipeline(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "add-step",
                    "Add one transform-backed step to a pipeline.",
                    new[]
                    {
                        "meta-pipeline add-step --workspace <path> --pipeline <name> --script <name-or-id> --transform-workspace <path> --binding-workspace <path> --execution-connection-env <name> [--step-name <name>] [--binding <id>] [--target-connection-env <name>] [--target <sql-identifier>] [--target-write <insert-rows>] [--batch-size <n>] [--timeout-seconds <n>] [--target-data-type-system <name>]"
                    },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. Existing MetaPipeline workspace to update."),
                        new CliOptionDefinition("--pipeline <name>", "Required. Pipeline that receives the new serial step."),
                        new CliOptionDefinition("--script <name-or-id>", "Required. TransformScript.Name or TransformScript.Id to model."),
                        new CliOptionDefinition("--transform-workspace <path>", "Required. MetaTransformScript workspace used for script selection."),
                        new CliOptionDefinition("--binding-workspace <path>", "Required. MetaTransformBinding workspace used for binding selection."),
                        new CliOptionDefinition("--execution-connection-env <name>", "Required. Shell-visible environment variable named by the modeled execution connection."),
                        new CliOptionDefinition("--step-name <name>", "Optional step name; omitted derives a deterministic name from the script name."),
                        new CliOptionDefinition("--binding <id>", "Optional binding id when the selected script has multiple bindings."),
                        new CliOptionDefinition("--target-connection-env <name>", "Required for SELECT-kind scripts. Shell-visible environment variable named by the modeled target connection."),
                        new CliOptionDefinition("--target <sql-identifier>", "Target table identifier when a SELECT binding has multiple targets."),
                        new CliOptionDefinition("--target-write <insert-rows>", "SELECT-kind target write model. The only supported value is insert-rows."),
                        new CliOptionDefinition("--batch-size <n>", "Bounded in-memory row buffer size for InsertRows. Default: 1000."),
                        new CliOptionDefinition("--timeout-seconds <n>", "SQL command and bulk-copy timeout seconds for the transform execution. 0 or omitted means no timeout."),
                        new CliOptionDefinition("--target-data-type-system <name>", "InsertRows target type family. Default: SqlServer.")
                    },
                    new[]
                    {
                        "Appends transform-backed task instances to the pipeline's serial task chain.",
                        "--script resolves exact TransformScript.Name first, then exact TransformScript.Id.",
                        "If exactly one binding references the selected script, --binding can be omitted.",
                        "Use --binding only when the selected script has multiple bindings.",
                        "SELECT-kind scripts require target options; add-step records a row stream and InsertRows target write.",
                        "Non-SELECT scripts record only a TransformExecution task and execution connection.",
                        "If a SELECT binding contains multiple targets, --target is required.",
                        "Connection env options name shell-visible environment variables; connection strings are not stored.",
                        "Use meta-pipeline execute to execute the modeled transform task."
                    },
                    new[]
                    {
                        "meta-pipeline add-step --workspace .\\PipelineWS --pipeline CustomerLoad --step-name load-customers --script dbo.v_customer_load --transform-workspace .\\TransformWS --binding-workspace .\\BindingWS --execution-connection-env EXECUTION_DB --target-connection-env TARGET_DB --target dbo.TargetCustomer --target-write insert-rows --batch-size 1000"
                    }),
                args => RunCommandWithHelpAsync(args, "add-step", commandArgs => RunAddStep(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "add-executable-step",
                    "Add one executable process step to a pipeline.",
                    new[]
                    {
                        "meta-pipeline add-executable-step --workspace <path> --pipeline <name> --executable <path> [--step-name <name>] [--arguments <text>] [--working-directory <path>] [--success-exit-code <n>] [--timeout-seconds <n>]"
                    },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. Existing MetaPipeline workspace to update."),
                        new CliOptionDefinition("--pipeline <name>", "Required. Pipeline that receives the new serial step."),
                        new CliOptionDefinition("--executable <path>", "Required. Executable path or executable name resolvable by the operating system."),
                        new CliOptionDefinition("--step-name <name>", "Optional step name; omitted derives a deterministic name from the executable file name."),
                        new CliOptionDefinition("--arguments <text>", "Optional raw command-line arguments passed to the executable."),
                        new CliOptionDefinition("--working-directory <path>", "Optional process working directory."),
                        new CliOptionDefinition("--success-exit-code <n>", "Expected process exit code. Default: 0."),
                        new CliOptionDefinition("--timeout-seconds <n>", "Process timeout seconds. 0 or omitted means no timeout.")
                    },
                    new[]
                    {
                        "Appends one executable-backed task instance to the pipeline's serial task chain.",
                        "The executable path, arguments, working directory, expected success exit code, and optional timeout are modeled in the workspace.",
                        "Runtime success is determined by the real process exit code.",
                        "Connection strings are not involved in executable tasks.",
                        "Use meta-pipeline execute to execute the modeled executable task."
                    },
                    new[]
                    {
                        "meta-pipeline add-executable-step --workspace .\\PipelineWS --pipeline CustomerLoad --step-name prepare-files --executable dotnet --arguments \"--info\""
                    }),
                args => RunCommandWithHelpAsync(args, "add-executable-step", commandArgs => RunAddExecutableStep(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "inspect",
                    "Show a compact MetaPipeline workspace summary.",
                    new[]
                    {
                        "meta-pipeline inspect --workspace <path>"
                    },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaPipeline workspace to inspect.")
                    },
                    new[]
                    {
                        "Loads a MetaPipeline workspace and prints pipeline/task instance counts."
                    }),
                args => RunCommandWithHelpAsync(args, "inspect", commandArgs => RunInspect(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "help",
                    "Show this help.",
                    new[] { "meta-pipeline help" }),
                _ =>
                {
                    PrintHelp();
                    return Task.FromResult(0);
                }),
        };

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static Task<int> RunCommandWithHelpAsync(
        string[] args,
        string commandName,
        Func<string[], Task<int>> executeAsync)
    {
        if (args.Length >= 2 && IsHelpToken(args[1]))
        {
            PrintCommandHelp(commandName);
            return Task.FromResult(0);
        }

        return executeAsync(args);
    }

    private static Task<int> RunCommandWithHelpAsync(
        string[] args,
        string commandName,
        Func<string[], int> execute)
    {
        if (args.Length >= 2 && IsHelpToken(args[1]))
        {
            PrintCommandHelp(commandName);
            return Task.FromResult(0);
        }

        return Task.FromResult(execute(args));
    }

    private static void PrintHelp()
    {
        CliHelpRenderer.WriteAppHelp(Presenter, Cli);
    }

    private static void PrintCommandHelp(string commandName)
    {
        CliHelpRenderer.WriteCommandHelp(Presenter, Cli, Cli.GetCommand(commandName));
    }

    private static void PrintExecuteHelp() => PrintCommandHelp("execute");

    private static void PrintExecuteStepHelp() => PrintCommandHelp("execute-step");

    private static void PrintExecuteWorkerHelp() => PrintCommandHelp("execute-worker");

    private static void PrintExecuteSqlServerHelp() => PrintCommandHelp("execute-sqlserver");

    private static void PrintCreatePipelineDbHelp() => PrintCommandHelp("create-pipeline-db");

    private static void PrintPrunePipelineDbHelp() => PrintCommandHelp("prune-pipeline-db");

    private static void PrintAddPipelineHelp() => PrintCommandHelp("add-pipeline");

    private static void PrintInspectHelp() => PrintCommandHelp("inspect");

    private static void PrintAddStepHelp() => PrintCommandHelp("add-step");

    private static void PrintAddExecutableStepHelp() => PrintCommandHelp("add-executable-step");

    private static string HelpCommand(string commandName) => Cli.GetCommand(commandName).HelpCommand(Cli.Name);

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
