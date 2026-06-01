using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaOrchestration.Core;
using MO = MetaOrchestration;

internal static partial class Program
{
    private static readonly ConsolePresenter Presenter = new();
    private static readonly Lazy<IReadOnlyList<CliCommandRoute>> CommandRoutesLazy = new(BuildCommandRoutes);
    private static readonly Lazy<IReadOnlyDictionary<string, CliCommandRoute>> CommandRoutesByNameLazy = new(
        () => CommandRoutes.ToDictionary(route => route.Definition.Name, StringComparer.OrdinalIgnoreCase));
    private static readonly Lazy<CliAppDefinition> CliLazy = new(
        () => new CliAppDefinition(
            "meta-orchestration",
            new[]
            {
                "meta-orchestration --pipeline-workspace <path> --transform-workspace <path> --binding-workspace <path> --new-workspace <path> [--description <text>]",
                "meta-orchestration <command> [options]",
            },
            CommandRoutes.Select(route => route.Definition).ToArray(),
            new[]
            {
                "--new-workspace creates a MetaOrchestration workspace by inferring from bound MetaPipeline transform steps.",
                "Binding must already exist; orchestration does not parse or bind SQL itself.",
                "The workspace separates dependency DAG status from determinism and synchronization status.",
                "Data dependencies are inferred from published producers to dependency consumers.",
                "Same-object writer interactions become determinism or synchronization issues instead of artificial dependency edges."
            },
            "meta-orchestration refresh-run-plan --help"));

    private static IReadOnlyList<CliCommandRoute> CommandRoutes => CommandRoutesLazy.Value;

    private static IReadOnlyDictionary<string, CliCommandRoute> CommandRoutesByName => CommandRoutesByNameLazy.Value;

    private static CliAppDefinition Cli => CliLazy.Value;

    private static IReadOnlyList<CliCommandRoute> BuildCommandRoutes() =>
        new[]
        {
            new CliCommandRoute(
                new CliCommandDefinition(
                    "inspect",
                    "Inspect a MetaOrchestration workspace.",
                    new[] { "meta-orchestration inspect --workspace <path>" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaOrchestration workspace to inspect.")
                    },
                    new[]
                    {
                        "Shows DAG, determinism, synchronization, dependency, effect, retry-policy, and issue summaries."
                    }),
                args => RunCommandWithHelpAsync(args, "inspect", commandArgs => RunInspect(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "list-issues",
                    "List analyzer issues recorded in an orchestration workspace.",
                    new[] { "meta-orchestration list-issues --workspace <path>" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaOrchestration workspace to inspect.")
                    },
                    new[]
                    {
                        "Lists dependency, determinism, synchronization, and policy issues without changing analysis evidence."
                    }),
                args => RunCommandWithHelpAsync(args, "list-issues", commandArgs => RunListIssues(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "explain-issue",
                    "Explain one analyzer issue and its participating pipelines.",
                    new[] { "meta-orchestration explain-issue --workspace <path> --issue <id-or-unique-code>" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaOrchestration workspace to inspect."),
                        new CliOptionDefinition("--issue <id-or-unique-code>", "Required. Issue id or unique issue code.")
                    },
                    new[]
                    {
                        "Shows issue domain, severity, blocking flags, object, message, and participating pipelines."
                    }),
                args => RunCommandWithHelpAsync(args, "explain-issue", commandArgs => RunExplainIssue(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "add-dependency",
                    "Record an explicit success or failure dependency between tasks.",
                    new[] { "meta-orchestration add-dependency --workspace <path> --from-task <task> --to-task <task> --condition success|failure [--object <sql-identifier>] [--reason <text>]" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaOrchestration workspace to update."),
                        new CliOptionDefinition("--from-task <task>", "Required. Predecessor task selector."),
                        new CliOptionDefinition("--to-task <task>", "Required. Successor task selector."),
                        new CliOptionDefinition("--condition success|failure", "Required. Whether the successor follows predecessor success or failure."),
                        new CliOptionDefinition("--object <sql-identifier>", "Optional object selector for object-scoped dependency resolution."),
                        new CliOptionDefinition("--reason <text>", "Optional reason recorded with the policy row.")
                    },
                    new[]
                    {
                        "Adds an explicit conditional DAG edge between planned tasks.",
                        "Success edges run the successor only when the predecessor succeeds.",
                        "Failure edges run the successor only when the predecessor fails.",
                        "Task selectors may be task id, task name, MetaPipeline task id, or Pipeline.Task."
                    }),
                args => RunCommandWithHelpAsync(args, "add-dependency", commandArgs => RunAddOrder(commandArgs, startIndex: 1, commandName: "add-dependency"))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "add-order",
                    "Record an explicit success dependency/order resolution in an orchestration workspace.",
                    new[] { "meta-orchestration add-order --workspace <path> --from-task <task> --to-task <task> [--condition success|failure] [--object <sql-identifier>] [--reason <text>]" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaOrchestration workspace to update."),
                        new CliOptionDefinition("--from-task <task>", "Required. Predecessor task selector."),
                        new CliOptionDefinition("--to-task <task>", "Required. Successor task selector."),
                        new CliOptionDefinition("--condition success|failure", "Optional dependency condition. Default: success."),
                        new CliOptionDefinition("--object <sql-identifier>", "Optional object selector for object-scoped dependency resolution."),
                        new CliOptionDefinition("--reason <text>", "Optional reason recorded with the policy row.")
                    },
                    new[]
                    {
                        "Adds an explicit task dependency resolution for a determinism issue.",
                        "The default condition is success. Use add-dependency when authoring failure branches.",
                        "Failure dependencies are graph edges, not post-run action hooks.",
                        "Task selectors may be task id, task name, MetaPipeline task id, or Pipeline.Task."
                    }),
                args => RunCommandWithHelpAsync(args, "add-order", commandArgs => RunAddOrder(commandArgs, startIndex: 1, commandName: "add-order"))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "allow-concurrent-append",
                    "Allow concurrent execution for multiple Append effects on one object.",
                    new[] { "meta-orchestration allow-concurrent-append --workspace <path> --object <sql-identifier> [--reason <text>]" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaOrchestration workspace to update."),
                        new CliOptionDefinition("--object <sql-identifier>", "Required. Data object whose append writers can overlap."),
                        new CliOptionDefinition("--reason <text>", "Optional reason recorded with the policy row.")
                    },
                    new[]
                    {
                        "Adds a scoped Append/Append lock compatibility policy for concurrent append writers."
                    }),
                args => RunCommandWithHelpAsync(args, "allow-concurrent-append", commandArgs => RunAllowConcurrentAppend(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "set-lock-policy",
                    "Record scoped lock compatibility for one object/effect interaction.",
                    new[] { "meta-orchestration set-lock-policy --workspace <path> --object <sql-identifier> --left-effect <effect> --right-effect <effect> --behavior <serialize|allow> [--reason <text>]" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaOrchestration workspace to update."),
                        new CliOptionDefinition("--object <sql-identifier>", "Required. Data object whose effect interaction is being resolved."),
                        new CliOptionDefinition("--left-effect <effect>", "Required. Left write effect, such as Append, Replace, Mutation, KeyedUpsert, or ConditionalKeyedUpsert."),
                        new CliOptionDefinition("--right-effect <effect>", "Required. Right write effect, such as Append, Replace, Mutation, KeyedUpsert, or ConditionalKeyedUpsert."),
                        new CliOptionDefinition("--behavior <serialize|allow>", "Required. Lock behavior for the object/effect pair."),
                        new CliOptionDefinition("--reason <text>", "Optional reason recorded with the policy row.")
                    },
                    new[]
                    {
                        "Adds or updates scoped lock compatibility for an object/effect interaction.",
                        "allow is currently accepted only for Append/Append; use serialize for other pairs."
                    }),
                args => RunCommandWithHelpAsync(args, "set-lock-policy", commandArgs => RunSetLockPolicy(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "refresh-run-plan",
                    "Refresh lock-aware run-plan rows in an orchestration workspace.",
                    new[] { "meta-orchestration refresh-run-plan --workspace <path>" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaOrchestration workspace to update.")
                    },
                    new[]
                    {
                        "Writes the run plan, default run-plan retry policy assignment, planned tasks, and task locks into the existing orchestration workspace.",
                        "The DAG must be complete and run-planning policy must resolve blocking determinism/synchronization issues.",
                        "Execute refreshes the run plan automatically; this command is for preflight and inspection workflows."
                    }),
                args => RunCommandWithHelpAsync(args, "refresh-run-plan", commandArgs => RunRefreshRunPlan(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "inspect-run-plan",
                    "Inspect the planned task dependency graph.",
                    new[] { "meta-orchestration inspect-run-plan --workspace <path>" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaOrchestration workspace to inspect.")
                    },
                    new[]
                    {
                        "Shows the task dependency graph as an adjacency list.",
                        "The graph is printed from dependency rows, not from planned-task order.",
                        "Use issue/policy inspection commands when you need the reasoning behind the plan."
                    }),
                args => RunCommandWithHelpAsync(args, "inspect-run-plan", commandArgs => RunInspectRunPlan(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "execute",
                    "Execute the current run plan by coordinating meta-pipeline worker processes.",
                    new[] { "meta-orchestration execute --workspace <path> --pipeline-workspace <path> --transform-workspace <path> --binding-workspace <path> [--data-type-conversion-workspace <path>] [--pipeline-db-connection-env <name>] [--max-degree-of-parallelism <n>] [--run-artifacts-root <path>] [--worker-event-timeout-seconds <n>]" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaOrchestration workspace containing the analysis and run-plan rows."),
                        new CliOptionDefinition("--pipeline-workspace <path>", "Required. MetaPipeline workspace used by child pipeline workers."),
                        new CliOptionDefinition("--transform-workspace <path>", "Required. MetaTransformScript workspace used by child pipeline workers."),
                        new CliOptionDefinition("--binding-workspace <path>", "Required. MetaTransformBinding workspace used by child pipeline workers."),
                        new CliOptionDefinition("--data-type-conversion-workspace <path>", "Optional conversion policy workspace passed to child workers."),
                        new CliOptionDefinition("--pipeline-db-connection-env <name>", "Optional operational DB connection env passed to child workers."),
                        new CliOptionDefinition("--max-degree-of-parallelism <n>", "Maximum concurrently granted pipeline tasks. Default: 1."),
                        new CliOptionDefinition("--run-artifacts-root <path>", "Optional operational root for run journals, worker logs, and workspace execution leases."),
                        new CliOptionDefinition("--worker-event-timeout-seconds <n>", "Optional fail-safe timeout for silent worker protocol periods. Default: 1800.")
                    },
                    new[]
                    {
                        "Refreshes run-plan rows from current workspace state, then executes the run plan.",
                        "Each MetaPipeline pipeline is launched once as a worker with a named pipe control channel.",
                        "Orchestration sends StartPipeline after WorkerReady, before any task grants.",
                        "Orchestration grants TaskReady work or stops a worker at a blocked task.",
                        "Workers parked at TaskReady do not count as silent; activation and running grants do.",
                        "Retry policy is read from modeled RetryPolicy/RetryPolicyFailureClass/RunPlanRetryPolicy rows, not from a command-line switch.",
                        "Failed tasks block OnSuccess dependents, enable OnFailure branches, and leave unrelated paths running.",
                        "Task dependencies and locks define runtime eligibility; --max-degree-of-parallelism throttles concurrent task grants.",
                        "Execution takes an exclusive lease for the orchestration workspace; different workspaces may execute concurrently.",
                        "MetaPipeline remains the owner of transform execution and operational DB evidence."
                    }),
                args => RunCommandWithHelpAsync(args, "execute", commandArgs => RunExecuteAsync(commandArgs, startIndex: 1))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "help",
                    "Show this help.",
                    new[] { "meta-orchestration help" }),
                _ =>
                {
                    PrintHelp();
                    return Task.FromResult(0);
                }),
        };

    private static async Task<int> Main(string[] args)
    {
        if (Meta.Core.Presentation.Cli.CliVersion.TryWriteVersion(Presenter, Cli.Name, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return 0;
        }

        if (args[0].StartsWith("--", StringComparison.Ordinal))
        {
            return RunInfer(args, startIndex: 0);
        }

        if (CommandRoutesByName.TryGetValue(args[0], out var route))
        {
            return await route.ExecuteAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", $"{Cli.Name} help");
    }

    private static int RunInfer(string[] args, int startIndex)
    {
        var parse = ParseInferArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, $"{Cli.Name} --help");
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.OutputWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Fail(
                targetValidation.ErrorMessage,
                "choose a new folder or empty the target directory and retry.",
                4,
                targetValidation.Details);
        }

        try
        {
            var request = new OrchestrationAnalysisRequest(
                parse.PipelineWorkspacePath,
                parse.TransformWorkspacePath,
                parse.BindingWorkspacePath,
                "Default",
                parse.Description);

            using var activity = CliActivityLine.Start("Creating");
            var service = new MetaOrchestrationAnalysisService();
            var result = service.Analyze(request);
            var model = service.CreateModel(result, parse.PipelineWorkspacePath);
            model.SaveToXmlWorkspace(targetValidation.FullPath);

            if (!result.IsCompleteDag)
            {
                activity.Dispose();
                return Fail(
                    "MetaOrchestration DAG is incomplete.",
                    "inspect the workspace issues and add explicit dependency resolutions before execution.",
                    4);
            }

            activity.Succeed();
            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Cannot infer orchestration.",
                "check the pipeline, transform, and binding workspaces, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static int RunInspect(string[] args, int startIndex)
    {
        var parse = ParseWorkspaceOnlyArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("inspect"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MO.MetaOrchestrationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            var plan = model.OrchestrationPlanList.SingleOrDefault();

            Presenter.WriteKeyValueBlock("MetaOrchestration", new[]
            {
                ("Plan", plan?.Name ?? string.Empty),
                ("DagStatus", plan?.DagStatus ?? string.Empty),
                ("DeterminismStatus", plan?.DeterminismStatus ?? string.Empty),
                ("SynchronizationStatus", plan?.SynchronizationStatus ?? string.Empty),
                ("Pipelines", model.PipelineReferenceList.Count.ToString()),
                ("Objects", model.DataObjectList.Count.ToString()),
                ("TaskProfiles", model.TaskAccessProfileList.Count.ToString()),
                ("TaskEffects", model.TaskObjectEffectList.Count.ToString()),
                ("TaskDependencies", model.TaskDependencyList.Count.ToString()),
                ("PipelineDependencies", model.PipelineDependencyList.Count.ToString()),
                ("TaskOrderingResolutions", model.TaskOrderingResolutionList.Count.ToString()),
                ("LockCompatibilityPolicies", model.LockCompatibilityPolicyList.Count.ToString()),
                ("RetryPolicies", model.RetryPolicyList.Count.ToString()),
                ("RunPlans", model.RunPlanList.Count.ToString()),
                ("PlannedTasks", model.PlannedTaskList.Count.ToString()),
                ("Issues", model.DependencyIssueList.Count.ToString()),
            });

            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Cannot inspect orchestration workspace.",
                "check the workspace path and instance data integrity, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static int RunAddOrder(string[] args, int startIndex, string commandName)
    {
        var parse = ParseAddOrderArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand(commandName));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MO.MetaOrchestrationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            var service = new MetaOrchestrationRunPlanningService();
            service.AddTaskOrderingResolution(
                model,
                parse.FromTask,
                parse.ToTask,
                parse.ObjectSelector,
                parse.Reason,
                parse.DependencyCondition);
            model.SaveToXmlWorkspace(workspacePath);

            Presenter.WriteOk();
            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Could not add task ordering resolution.",
                "check the workspace, task selectors, dependency condition, and optional object selector, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static int RunListIssues(string[] args, int startIndex)
    {
        var parse = ParseWorkspaceOnlyArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("list-issues"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MO.MetaOrchestrationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);

            Presenter.WriteKeyValueBlock("MetaOrchestration", new[]
            {
                ("Issues", model.DependencyIssueList.Count.ToString()),
            });
            PrintIssues(model, take: int.MaxValue);
            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Could not list orchestration issues.",
                "check the workspace path and instance data integrity, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static int RunExplainIssue(string[] args, int startIndex)
    {
        var parse = ParseExplainIssueArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("explain-issue"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MO.MetaOrchestrationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            var issue = ResolveIssue(model, parse.IssueSelector);
            PrintIssueDetails(model, issue);
            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Could not explain orchestration issue.",
                "check the workspace and issue selector, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static int RunAllowConcurrentAppend(string[] args, int startIndex)
    {
        var parse = ParseObjectReasonArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("allow-concurrent-append"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MO.MetaOrchestrationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            var service = new MetaOrchestrationRunPlanningService();
            service.AddConcurrentAppendPolicy(model, parse.ObjectSelector, parse.Reason);
            model.SaveToXmlWorkspace(workspacePath);

            Presenter.WriteOk();
            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Could not add concurrent append policy.",
                "check the workspace and object selector, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static int RunSetLockPolicy(string[] args, int startIndex)
    {
        var parse = ParseSetLockPolicyArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("set-lock-policy"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MO.MetaOrchestrationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            var service = new MetaOrchestrationRunPlanningService();
            service.AddLockCompatibilityPolicy(
                model,
                parse.ObjectSelector,
                parse.LeftEffect,
                parse.RightEffect,
                parse.LockBehavior,
                parse.Reason);
            model.SaveToXmlWorkspace(workspacePath);

            Presenter.WriteOk();
            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Could not set lock compatibility policy.",
                "check the workspace, object selector, effects, and lock behavior, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static int RunRefreshRunPlan(string[] args, int startIndex)
    {
        var parse = ParseRunPlanArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("refresh-run-plan"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MO.MetaOrchestrationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            using (var activity = CliActivityLine.Start("Building"))
            {
                var service = new MetaOrchestrationRunPlanningService();
                service.BuildRunPlan(model);
                model.SaveToXmlWorkspace(workspacePath);

                activity.Succeed();
            }

            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Cannot refresh run plan.",
                "resolve blocking DAG, determinism, or synchronization policy issues, then retry refresh-run-plan.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static int RunInspectRunPlan(string[] args, int startIndex)
    {
        var parse = ParseWorkspaceOnlyArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("inspect-run-plan"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MO.MetaOrchestrationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);

            PrintRunPlanGraph(model);
            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Cannot inspect run plan.",
                "check the workspace path and run-plan rows, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static ParsedInferArgs ParseInferArgs(string[] args, int startIndex)
    {
        var pipelineWorkspace = string.Empty;
        var transformWorkspace = string.Empty;
        var bindingWorkspace = string.Empty;
        var outputWorkspace = string.Empty;
        string? description = null;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var option = args[i];
            if (i + 1 >= args.Length)
            {
                return ParsedInferArgs.Fail($"missing value for {option}.");
            }

            var value = args[++i];
            if (!seen.Add(option))
            {
                return ParsedInferArgs.Fail($"{option} can only be provided once.");
            }

            switch (option.ToLowerInvariant())
            {
                case "--pipeline-workspace":
                    pipelineWorkspace = value;
                    break;
                case "--transform-workspace":
                    transformWorkspace = value;
                    break;
                case "--binding-workspace":
                    bindingWorkspace = value;
                    break;
                case "--new-workspace":
                    outputWorkspace = value;
                    break;
                case "--description":
                    description = value;
                    break;
                default:
                    return ParsedInferArgs.Fail($"unknown option '{option}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(pipelineWorkspace)) return ParsedInferArgs.Fail("missing required option --pipeline-workspace <path>.");
        if (string.IsNullOrWhiteSpace(transformWorkspace)) return ParsedInferArgs.Fail("missing required option --transform-workspace <path>.");
        if (string.IsNullOrWhiteSpace(bindingWorkspace)) return ParsedInferArgs.Fail("missing required option --binding-workspace <path>.");
        if (string.IsNullOrWhiteSpace(outputWorkspace)) return ParsedInferArgs.Fail("missing required option --new-workspace <path>.");

        return new ParsedInferArgs(true, pipelineWorkspace, transformWorkspace, bindingWorkspace, outputWorkspace, description, string.Empty);
    }

    private static ParsedWorkspaceArgs ParseWorkspaceOnlyArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var seen = false;
        for (var i = startIndex; i < args.Length; i++)
        {
            var option = args[i];
            if (!string.Equals(option, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                return new ParsedWorkspaceArgs(false, workspacePath, $"unknown option '{option}'.");
            }

            if (seen)
            {
                return new ParsedWorkspaceArgs(false, workspacePath, "--workspace can only be provided once.");
            }

            if (i + 1 >= args.Length)
            {
                return new ParsedWorkspaceArgs(false, workspacePath, "missing value for --workspace.");
            }

            workspacePath = args[++i];
            seen = true;
        }

        return string.IsNullOrWhiteSpace(workspacePath)
            ? new ParsedWorkspaceArgs(false, string.Empty, "missing required option --workspace <path>.")
            : new ParsedWorkspaceArgs(true, workspacePath, string.Empty);
    }

    private static ParsedAddOrderArgs ParseAddOrderArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var fromTask = string.Empty;
        var toTask = string.Empty;
        var dependencyCondition = "success";
        string? objectSelector = null;
        string? reason = null;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var option = args[i];
            if (i + 1 >= args.Length)
            {
                return ParsedAddOrderArgs.Fail($"missing value for {option}.");
            }

            var value = args[++i];
            if (!seen.Add(option))
            {
                return ParsedAddOrderArgs.Fail($"{option} can only be provided once.");
            }

            switch (option.ToLowerInvariant())
            {
                case "--workspace":
                    workspacePath = value;
                    break;
                case "--from-task":
                    fromTask = value;
                    break;
                case "--to-task":
                    toTask = value;
                    break;
                case "--condition":
                    dependencyCondition = value;
                    break;
                case "--object":
                    objectSelector = value;
                    break;
                case "--reason":
                    reason = value;
                    break;
                default:
                    return ParsedAddOrderArgs.Fail($"unknown option '{option}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return ParsedAddOrderArgs.Fail("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(fromTask)) return ParsedAddOrderArgs.Fail("missing required option --from-task <task>.");
        if (string.IsNullOrWhiteSpace(toTask)) return ParsedAddOrderArgs.Fail("missing required option --to-task <task>.");
        if (string.IsNullOrWhiteSpace(dependencyCondition)) return ParsedAddOrderArgs.Fail("--condition cannot be blank.");

        return new ParsedAddOrderArgs(true, workspacePath, fromTask, toTask, dependencyCondition, objectSelector, reason, string.Empty);
    }

    private static ParsedExplainIssueArgs ParseExplainIssueArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var issueSelector = string.Empty;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var option = args[i];
            if (i + 1 >= args.Length)
            {
                return ParsedExplainIssueArgs.Fail($"missing value for {option}.");
            }

            var value = args[++i];
            if (!seen.Add(option))
            {
                return ParsedExplainIssueArgs.Fail($"{option} can only be provided once.");
            }

            switch (option.ToLowerInvariant())
            {
                case "--workspace":
                    workspacePath = value;
                    break;
                case "--issue":
                    issueSelector = value;
                    break;
                default:
                    return ParsedExplainIssueArgs.Fail($"unknown option '{option}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return ParsedExplainIssueArgs.Fail("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(issueSelector)) return ParsedExplainIssueArgs.Fail("missing required option --issue <id-or-code>.");

        return new ParsedExplainIssueArgs(true, workspacePath, issueSelector, string.Empty);
    }

    private static ParsedObjectReasonArgs ParseObjectReasonArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var objectSelector = string.Empty;
        string? reason = null;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var option = args[i];
            if (i + 1 >= args.Length)
            {
                return ParsedObjectReasonArgs.Fail($"missing value for {option}.");
            }

            var value = args[++i];
            if (!seen.Add(option))
            {
                return ParsedObjectReasonArgs.Fail($"{option} can only be provided once.");
            }

            switch (option.ToLowerInvariant())
            {
                case "--workspace":
                    workspacePath = value;
                    break;
                case "--object":
                    objectSelector = value;
                    break;
                case "--reason":
                    reason = value;
                    break;
                default:
                    return ParsedObjectReasonArgs.Fail($"unknown option '{option}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return ParsedObjectReasonArgs.Fail("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(objectSelector)) return ParsedObjectReasonArgs.Fail("missing required option --object <sql-identifier>.");

        return new ParsedObjectReasonArgs(true, workspacePath, objectSelector, reason, string.Empty);
    }

    private static ParsedSetLockPolicyArgs ParseSetLockPolicyArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var objectSelector = string.Empty;
        var leftEffect = string.Empty;
        var rightEffect = string.Empty;
        var behavior = string.Empty;
        string? reason = null;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var option = args[i];
            if (i + 1 >= args.Length)
            {
                return ParsedSetLockPolicyArgs.Fail($"missing value for {option}.");
            }

            var value = args[++i];
            if (!seen.Add(option))
            {
                return ParsedSetLockPolicyArgs.Fail($"{option} can only be provided once.");
            }

            switch (option.ToLowerInvariant())
            {
                case "--workspace":
                    workspacePath = value;
                    break;
                case "--object":
                    objectSelector = value;
                    break;
                case "--left-effect":
                    leftEffect = value;
                    break;
                case "--right-effect":
                    rightEffect = value;
                    break;
                case "--behavior":
                    behavior = value;
                    break;
                case "--reason":
                    reason = value;
                    break;
                default:
                    return ParsedSetLockPolicyArgs.Fail($"unknown option '{option}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return ParsedSetLockPolicyArgs.Fail("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(objectSelector)) return ParsedSetLockPolicyArgs.Fail("missing required option --object <sql-identifier>.");
        if (string.IsNullOrWhiteSpace(leftEffect)) return ParsedSetLockPolicyArgs.Fail("missing required option --left-effect <effect>.");
        if (string.IsNullOrWhiteSpace(rightEffect)) return ParsedSetLockPolicyArgs.Fail("missing required option --right-effect <effect>.");
        if (string.IsNullOrWhiteSpace(behavior)) return ParsedSetLockPolicyArgs.Fail("missing required option --behavior <serialize|allow>.");

        return new ParsedSetLockPolicyArgs(true, workspacePath, objectSelector, leftEffect, rightEffect, behavior, reason, string.Empty);
    }

    private static ParsedRunPlanArgs ParseRunPlanArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var option = args[i];
            if (i + 1 >= args.Length)
            {
                return ParsedRunPlanArgs.Fail($"missing value for {option}.");
            }

            var value = args[++i];
            if (!seen.Add(option))
            {
                return ParsedRunPlanArgs.Fail($"{option} can only be provided once.");
            }

            switch (option.ToLowerInvariant())
            {
                case "--workspace":
                    workspacePath = value;
                    break;
                default:
                    return ParsedRunPlanArgs.Fail($"unknown option '{option}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return ParsedRunPlanArgs.Fail("missing required option --workspace <path>.");

        return new ParsedRunPlanArgs(true, workspacePath, string.Empty);
    }

    private static void PrintIssues(MO.MetaOrchestrationModel model, int take)
    {
        foreach (var issue in model.DependencyIssueList
                     .OrderBy(static item => item.Code, StringComparer.Ordinal)
                     .ThenBy(static item => item.Message, StringComparer.Ordinal)
                     .Take(take))
        {
            Presenter.WriteInfo($"  {issue.Id}: {issue.Code} [{issue.IssueDomain}/{issue.Severity}] BlocksDag={issue.BlocksDag} BlocksRunPlan={issue.BlocksAutomaticRunPlanning}");
            Presenter.WriteInfo($"    {issue.Message}");
        }
    }

    private static void PrintIssueDetails(MO.MetaOrchestrationModel model, MO.DependencyIssue issue)
    {
        Presenter.WriteKeyValueBlock("MetaOrchestration", new[]
        {
            ("Issue", issue.Id),
            ("Code", issue.Code),
            ("Domain", issue.IssueDomain),
            ("Severity", issue.Severity),
            ("BlocksDag", issue.BlocksDag),
            ("BlocksAutomaticRunPlanning", issue.BlocksAutomaticRunPlanning),
            ("Object", issue.DataObject?.SqlIdentifier ?? string.Empty),
            ("Message", issue.Message),
        });

        var pipelines = model.DependencyIssuePipelineList
            .Where(item => ReferenceEquals(item.DependencyIssue, issue))
            .OrderBy(static item => item.Role, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.PipelineReference.Name, StringComparer.OrdinalIgnoreCase)
            .Select(static item => $"{item.Role}: {item.PipelineReference.Name}")
            .ToArray();
        foreach (var pipeline in pipelines)
        {
            Presenter.WriteInfo($"  {pipeline}");
        }
    }

    private static MO.DependencyIssue ResolveIssue(MO.MetaOrchestrationModel model, string selector)
    {
        var trimmed = selector.Trim();
        var matches = model.DependencyIssueList
            .Where(item =>
                string.Equals(item.Id, trimmed, StringComparison.Ordinal) ||
                string.Equals(item.Code, trimmed, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return matches.Length switch
        {
            1 => matches[0],
            0 => throw new InvalidOperationException($"Could not resolve issue '{selector}'. Use issue id or unique issue code."),
            _ => throw new InvalidOperationException($"Issue selector '{selector}' matched {matches.Length} issues. Use issue id.")
        };
    }

    private static void PrintRunPlanGraph(MO.MetaOrchestrationModel model)
    {
        var runPlans = model.RunPlanList
            .OrderBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.Id, StringComparer.Ordinal)
            .ToArray();
        if (runPlans.Length == 0)
        {
            Presenter.WriteInfo("No run plan.");
            return;
        }

        for (var runPlanIndex = 0; runPlanIndex < runPlans.Length; runPlanIndex++)
        {
            if (runPlanIndex > 0)
            {
                Presenter.WriteInfo(string.Empty);
            }

            var runPlan = runPlans[runPlanIndex];
            var status = string.Equals(runPlan.RunPlanStatus, "Ready", StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : $" [{runPlan.RunPlanStatus}]";
            Presenter.WriteInfo($"{runPlan.Name}{status}");
            var retryPolicy = model.RunPlanRetryPolicyList
                .Where(item => ReferenceEquals(item.RunPlan, runPlan))
                .Where(static item => string.Equals(item.PolicyRole, "Default", StringComparison.OrdinalIgnoreCase))
                .Select(static item => item.RetryPolicy)
                .OrderBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(static item => item.Id, StringComparer.Ordinal)
                .FirstOrDefault();
            if (retryPolicy is not null)
            {
                Presenter.WriteInfo($"RetryPolicy: {retryPolicy.Name} (MaxAttempts={retryPolicy.MaxAttempts})");
            }

            var tasks = model.PlannedTaskList
                .Where(item => ReferenceEquals(item.RunPlan, runPlan))
                .OrderBy(static item => ParseOrdinal(item.Ordinal))
                .ThenBy(static item => item.Id, StringComparer.Ordinal)
                .ToArray();
            if (tasks.Length == 0)
            {
                Presenter.WriteInfo("PlannedTasks: 0");
                continue;
            }

            var plannedTaskProfileIds = tasks
                .Select(static item => item.TaskAccessProfile.Id)
                .ToHashSet(StringComparer.Ordinal);
            var edges = BuildRunPlanGraphEdges(model, plannedTaskProfileIds);
            var edgesByPredecessorId = edges
                .GroupBy(static item => item.Predecessor.Id, StringComparer.Ordinal)
                .ToDictionary(
                    static group => group.Key,
                    static group => group
                        .OrderBy(static item => FormatGraphTaskName(item.Successor), StringComparer.OrdinalIgnoreCase)
                        .ThenBy(static item => item.Kind, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(static item => item.Condition, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(static item => item.ObjectSqlIdentifier ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                        .ToArray(),
                    StringComparer.Ordinal);
            Presenter.WriteInfo($"PlannedTasks: {tasks.Length}");
            Presenter.WriteInfo($"DependencyEdges: {edges.Length}");
            Presenter.WriteInfo("Graph:");

            foreach (var task in tasks
                         .OrderBy(static item => FormatGraphTaskName(item), StringComparer.OrdinalIgnoreCase)
                         .ThenBy(static item => item.Id, StringComparer.Ordinal))
            {
                Presenter.WriteInfo($"  {FormatGraphTaskName(task)}");
                if (!edgesByPredecessorId.TryGetValue(task.TaskAccessProfile.Id, out var outgoingEdges))
                {
                    Presenter.WriteInfo("    (no outgoing dependencies)");
                    continue;
                }

                foreach (var edge in outgoingEdges)
                {
                    Presenter.WriteInfo($"    --> {FormatGraphTaskName(edge.Successor)} [{FormatGraphEdgeLabel(edge)}]");
                }
            }
        }
    }

    private static RunPlanGraphEdge[] BuildRunPlanGraphEdges(
        MO.MetaOrchestrationModel model,
        IReadOnlySet<string> plannedTaskProfileIds)
    {
        var edges = new List<RunPlanGraphEdge>();
        foreach (var dependency in model.TaskDependencyList)
        {
            if (!plannedTaskProfileIds.Contains(dependency.Predecessor.Id) ||
                !plannedTaskProfileIds.Contains(dependency.Successor.Id))
            {
                continue;
            }

            edges.Add(new RunPlanGraphEdge(
                dependency.Predecessor,
                dependency.Successor,
                dependency.DependencyKind,
                dependency.DependencyCondition,
                dependency.DataObject?.SqlIdentifier));
        }

        foreach (var resolution in model.TaskOrderingResolutionList.Where(static item => IsActive(item.Status)))
        {
            if (!plannedTaskProfileIds.Contains(resolution.Predecessor.Id) ||
                !plannedTaskProfileIds.Contains(resolution.Successor.Id))
            {
                continue;
            }

            edges.Add(new RunPlanGraphEdge(
                resolution.Predecessor,
                resolution.Successor,
                resolution.ResolutionKind,
                resolution.DependencyCondition,
                resolution.DataObject?.SqlIdentifier));
        }

        return edges
            .OrderBy(static item => FormatGraphTaskName(item.Predecessor), StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => FormatGraphTaskName(item.Successor), StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.Kind, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.Condition, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.ObjectSqlIdentifier ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string FormatGraphEdgeLabel(RunPlanGraphEdge edge)
    {
        var label = $"{edge.Condition}/{edge.Kind}";
        return string.IsNullOrWhiteSpace(edge.ObjectSqlIdentifier)
            ? label
            : $"{label}/{edge.ObjectSqlIdentifier}";
    }

    private static string FormatGraphTaskName(MO.TaskAccessProfile task) =>
        $"{task.PipelineReference.Name}.{task.TaskName}";

    private static string FormatGraphTaskName(MO.PlannedTask plannedTask) =>
        FormatGraphTaskName(plannedTask.TaskAccessProfile);

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

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        CliHelpRenderer.WriteAppHelp(Presenter, Cli);
    }

    private static void PrintCommandHelp(string commandName)
    {
        CliHelpRenderer.WriteCommandHelp(Presenter, Cli, Cli.GetCommand(commandName));
    }

    private static void PrintInspectHelp() => PrintCommandHelp("inspect");

    private static void PrintAddOrderHelp() => PrintCommandHelp("add-order");

    private static void PrintAddDependencyHelp() => PrintCommandHelp("add-dependency");

    private static void PrintListIssuesHelp() => PrintCommandHelp("list-issues");

    private static void PrintExplainIssueHelp() => PrintCommandHelp("explain-issue");

    private static void PrintAllowConcurrentAppendHelp() => PrintCommandHelp("allow-concurrent-append");

    private static void PrintSetLockPolicyHelp() => PrintCommandHelp("set-lock-policy");

    private static void PrintRefreshRunPlanHelp() => PrintCommandHelp("refresh-run-plan");

    private static void PrintInspectRunPlanHelp() => PrintCommandHelp("inspect-run-plan");

    private static void PrintExecuteHelp() => PrintCommandHelp("execute");

    private static string HelpCommand(string commandName) => Cli.GetCommand(commandName).HelpCommand(Cli.Name);

    private static int Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details is not null)
        {
            renderedDetails.AddRange(details.Where(static item => !string.IsNullOrWhiteSpace(item)));
        }

        renderedDetails.Add($"Next: {next}");
        Presenter.WriteFailure(message, renderedDetails);
        return exitCode;
    }

    private sealed record RunPlanGraphEdge(
        MO.TaskAccessProfile Predecessor,
        MO.TaskAccessProfile Successor,
        string Kind,
        string Condition,
        string? ObjectSqlIdentifier);

    private sealed record ParsedInferArgs(
        bool Ok,
        string PipelineWorkspacePath,
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        string OutputWorkspacePath,
        string? Description,
        string ErrorMessage)
    {
        public static ParsedInferArgs Fail(string errorMessage) =>
            new(false, string.Empty, string.Empty, string.Empty, string.Empty, null, errorMessage);
    }

    private sealed record ParsedWorkspaceArgs(
        bool Ok,
        string WorkspacePath,
        string ErrorMessage);

    private sealed record ParsedAddOrderArgs(
        bool Ok,
        string WorkspacePath,
        string FromTask,
        string ToTask,
        string DependencyCondition,
        string? ObjectSelector,
        string? Reason,
        string ErrorMessage)
    {
        public static ParsedAddOrderArgs Fail(string errorMessage) =>
            new(false, string.Empty, string.Empty, string.Empty, string.Empty, null, null, errorMessage);
    }

    private sealed record ParsedExplainIssueArgs(
        bool Ok,
        string WorkspacePath,
        string IssueSelector,
        string ErrorMessage)
    {
        public static ParsedExplainIssueArgs Fail(string errorMessage) =>
            new(false, string.Empty, string.Empty, errorMessage);
    }

    private sealed record ParsedObjectReasonArgs(
        bool Ok,
        string WorkspacePath,
        string ObjectSelector,
        string? Reason,
        string ErrorMessage)
    {
        public static ParsedObjectReasonArgs Fail(string errorMessage) =>
            new(false, string.Empty, string.Empty, null, errorMessage);
    }

    private sealed record ParsedSetLockPolicyArgs(
        bool Ok,
        string WorkspacePath,
        string ObjectSelector,
        string LeftEffect,
        string RightEffect,
        string LockBehavior,
        string? Reason,
        string ErrorMessage)
    {
        public static ParsedSetLockPolicyArgs Fail(string errorMessage) =>
            new(false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null, errorMessage);
    }

    private sealed record ParsedRunPlanArgs(
        bool Ok,
        string WorkspacePath,
        string ErrorMessage)
    {
        public static ParsedRunPlanArgs Fail(string errorMessage) =>
            new(false, string.Empty, errorMessage);
    }

    private static int ParseOrdinal(string value) =>
        int.TryParse(value, out var ordinal) ? ordinal : int.MaxValue;
}
