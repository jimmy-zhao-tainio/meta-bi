using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaOrchestration.Core;
using MO = MetaOrchestration;

internal sealed class MetaOrchestrationCommandHandlers
{
    private readonly ConsolePresenter presenter;
    private readonly string appName;

    public MetaOrchestrationCommandHandlers(ConsolePresenter presenter, string appName)
    {
        this.presenter = presenter;
        this.appName = appName;
    }

    public async Task RunInferAsync(
        MetaCliInvocation invocation,
        MetaCliWorkspaces workspaces)
    {
        var pipelineWorkspacePath = invocation.Required("pipeline-workspace");
        var description = invocation.Optional("description");

        try
        {
            var request = new OrchestrationAnalysisRequest(
                pipelineWorkspacePath,
                "Default",
                description);

            using var activity = CliActivityLine.Start("Creating");
            var service = new MetaOrchestrationAnalysisService();
            var result = service.Analyze(request);
            var outputWorkspacePath = new[]
                {
                    invocation.Optional("output-xml"),
                    invocation.Optional("output-csharp"),
                    invocation.Optional("output-sql"),
                }
                .FirstOrDefault(static path => !string.IsNullOrWhiteSpace(path))
                ?? throw new InvalidOperationException("An output workspace path is required.");
            var model = service.CreatePortableModel(result, pipelineWorkspacePath, outputWorkspacePath);
            await workspaces.CreateAsync("output", model).ConfigureAwait(false);

            if (!result.IsCompleteDag)
            {
                activity.Dispose();
                Fail(
                    "MetaOrchestration DAG is incomplete.",
                    "inspect the workspace issues and add explicit dependency resolutions before execution.",
                    4);
            }

            activity.Succeed();
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Cannot infer orchestration.",
                "check the pipeline workspace and any transform/binding workspaces required by transform-backed steps, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    public void RunInspect(MetaCliInvocation invocation, MO.MetaOrchestrationModel model)
    {
        try
        {
            var plan = model.OrchestrationPlanList.SingleOrDefault();

            presenter.WriteKeyValueBlock("MetaOrchestration", new[]
            {
                ("Plan", plan?.Name ?? string.Empty),
                ("DagStatus", plan?.DagStatus ?? string.Empty),
                ("DeterminismStatus", plan?.DeterminismStatus ?? string.Empty),
                ("SynchronizationStatus", plan?.SynchronizationStatus ?? string.Empty),
                ("Pipelines", model.PipelineReferenceList.Count.ToString(CultureInfo.InvariantCulture)),
                ("Objects", model.DataObjectList.Count.ToString(CultureInfo.InvariantCulture)),
                ("TaskProfiles", model.TaskAccessProfileList.Count.ToString(CultureInfo.InvariantCulture)),
                ("TaskEffects", model.TaskObjectEffectList.Count.ToString(CultureInfo.InvariantCulture)),
                ("TaskDependencies", model.TaskDependencyList.Count.ToString(CultureInfo.InvariantCulture)),
                ("PipelineDependencies", model.PipelineDependencyList.Count.ToString(CultureInfo.InvariantCulture)),
                ("TaskOrderingResolutions", model.TaskOrderingResolutionList.Count.ToString(CultureInfo.InvariantCulture)),
                ("LockCompatibilityPolicies", model.LockCompatibilityPolicyList.Count.ToString(CultureInfo.InvariantCulture)),
                ("RetryPolicies", model.RetryPolicyList.Count.ToString(CultureInfo.InvariantCulture)),
                ("RunPlans", model.RunPlanList.Count.ToString(CultureInfo.InvariantCulture)),
                ("PlannedTasks", model.PlannedTaskList.Count.ToString(CultureInfo.InvariantCulture)),
                ("Issues", model.DependencyIssueList.Count.ToString(CultureInfo.InvariantCulture)),
            });
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Cannot inspect orchestration workspace.",
                "check the workspace path and instance data integrity, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    public void RunAddOrder(
        MetaCliInvocation invocation,
        MO.MetaOrchestrationModel model,
        string commandName)
    {
        var fromTask = invocation.Required("from-task");
        var toTask = invocation.Required("to-task");
        var dependencyCondition = invocation.Optional("condition") ?? "success";
        var objectSelector = invocation.Optional("object");
        var reason = invocation.Optional("reason");

        try
        {
            var service = new MetaOrchestrationRunPlanningService();
            service.AddTaskOrderingResolution(
                model,
                fromTask,
                toTask,
                objectSelector,
                reason,
                dependencyCondition);
            presenter.WriteOk();
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Could not add task ordering resolution.",
                $"check the workspace, task selectors, dependency condition, and optional object selector, then retry: {HelpCommand(commandName)}",
                4,
                [$"  {ex.Message}"]);
        }
    }

    public void RunListIssues(MetaCliInvocation invocation, MO.MetaOrchestrationModel model)
    {
        try
        {
            presenter.WriteKeyValueBlock("MetaOrchestration", new[]
            {
                ("Issues", model.DependencyIssueList.Count.ToString(CultureInfo.InvariantCulture)),
            });
            PrintIssues(model, take: int.MaxValue);
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Could not list orchestration issues.",
                "check the workspace path and instance data integrity, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    public void RunExplainIssue(MetaCliInvocation invocation, MO.MetaOrchestrationModel model)
    {
        var issueSelector = invocation.Required("issue");
        try
        {
            var issue = ResolveIssue(model, issueSelector);
            PrintIssueDetails(model, issue);
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Could not explain orchestration issue.",
                "check the workspace and issue selector, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    public void RunAllowConcurrentAppend(MetaCliInvocation invocation, MO.MetaOrchestrationModel model)
    {
        var objectSelector = invocation.Required("object");
        var reason = invocation.Optional("reason");

        try
        {
            var service = new MetaOrchestrationRunPlanningService();
            service.AddConcurrentAppendPolicy(model, objectSelector, reason);
            presenter.WriteOk();
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Could not add concurrent append policy.",
                "check the workspace and object selector, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    public void RunSetLockPolicy(MetaCliInvocation invocation, MO.MetaOrchestrationModel model)
    {
        var objectSelector = invocation.Required("object");
        var leftEffect = invocation.Required("left-effect");
        var rightEffect = invocation.Required("right-effect");
        var behavior = invocation.Required("behavior");
        var reason = invocation.Optional("reason");

        try
        {
            var service = new MetaOrchestrationRunPlanningService();
            service.AddLockCompatibilityPolicy(
                model,
                objectSelector,
                leftEffect,
                rightEffect,
                behavior,
                reason);
            presenter.WriteOk();
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Could not set lock compatibility policy.",
                "check the workspace, object selector, effects, and lock behavior, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    public void RunRefreshRunPlan(MetaCliInvocation invocation, MO.MetaOrchestrationModel model)
    {
        try
        {
            using var activity = CliActivityLine.Start("Building");
            var service = new MetaOrchestrationRunPlanningService();
            service.BuildRunPlan(model);
            activity.Succeed();
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Cannot refresh run plan.",
                "resolve blocking DAG, determinism, or synchronization policy issues, then retry refresh-run-plan.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    public void RunInspectRunPlan(MetaCliInvocation invocation, MO.MetaOrchestrationModel model)
    {
        try
        {
            PrintRunPlanGraph(model);
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Cannot inspect run plan.",
                "check the workspace path and run-plan rows, then retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    public async Task RunExecuteAsync(MetaCliInvocation invocation)
    {
        var workspacePath = ResolveWorkspacePath(invocation);
        var pipelineWorkspacePath = invocation.Required("pipeline-workspace");
        var dataTypeConversionWorkspacePath = invocation.Optional("data-type-conversion-workspace") ?? string.Empty;
        var pipelineDbConnectionEnvironmentVariableName = invocation.Optional("pipeline-db-connection-env") ?? string.Empty;
        var maxDegreeOfParallelism = ReadPositiveInt(
            invocation.Optional("max-degree-of-parallelism") ?? "1",
            "--max-degree-of-parallelism");
        var runArtifactsRootPath = invocation.Optional("run-artifacts-root") ?? string.Empty;
        var workerEventTimeoutSeconds = ReadNonNegativeInt(
            invocation.Optional("worker-event-timeout-seconds"),
            "--worker-event-timeout-seconds");
        var workerActivationTimeoutSeconds = ReadNonNegativeInt(
            invocation.Optional("worker-activation-timeout-seconds"),
            "--worker-activation-timeout-seconds");
        var workerControlPipeConnectTimeoutSeconds = ReadNonNegativeInt(
            invocation.Optional("worker-control-pipe-connect-timeout-seconds"),
            "--worker-control-pipe-connect-timeout-seconds");

        using var progress = OrchestrationExecutionProgressRenderer.TryCreate(maxDegreeOfParallelism);
        try
        {
            var observer = progress is null
                ? null
                : new OrchestrationRuntimeProgressObserver(progress);
            var result = await new MetaOrchestrationRuntimeService()
                .ExecuteAsync(
                    new OrchestrationRuntimeRequest(
                        workspacePath,
                        pipelineWorkspacePath,
                        TransformWorkspacePath: string.Empty,
                        BindingWorkspacePath: string.Empty,
                        dataTypeConversionWorkspacePath,
                        pipelineDbConnectionEnvironmentVariableName,
                        maxDegreeOfParallelism,
                        RunArtifactsRootPath: runArtifactsRootPath,
                        WorkerEventTimeout: workerEventTimeoutSeconds is null
                            ? null
                            : TimeSpan.FromSeconds(workerEventTimeoutSeconds.Value),
                        WorkerActivationTimeout: workerActivationTimeoutSeconds is null
                            ? null
                            : TimeSpan.FromSeconds(workerActivationTimeoutSeconds.Value),
                        WorkerControlPipeConnectTimeout: workerControlPipeConnectTimeoutSeconds is null
                            ? null
                            : TimeSpan.FromSeconds(workerControlPipeConnectTimeoutSeconds.Value)),
                    observer)
                .ConfigureAwait(false);

            progress?.Complete(failed: !result.Succeeded);
            if (!result.Succeeded)
            {
                PrintExecutionIncomplete(result);
            }

            PrintExecutionComplete(result);
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            progress?.Complete(failed: true);
            Fail(
                "Cannot execute orchestration.",
                "check the orchestration workspace, pipeline workspace, child meta-pipeline worker availability, and retry.",
                4,
                [$"  {ex.Message}"]);
        }
        catch (OperationCanceledException ex)
        {
            progress?.Complete(failed: true);
            Fail(
                "Orchestration execution was cancelled.",
                "inspect the run artifacts for the last supervisor state and worker logs before rerunning execute.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private void PrintExecutionIncomplete(OrchestrationRuntimeResult result)
    {
        var failed = result.TaskResults.FirstOrDefault(static item => item.ExitCode != 0);
        var pipelineCount = result.PipelineCount;
        var failedPipelineCount = result.TaskResults
            .Where(static item => item.ExitCode != 0)
            .Select(static item => item.PipelineName)
            .Concat(result.BlockedResults.Select(static item => item.PipelineName))
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();
        var completedPipelineCount = Math.Max(0, pipelineCount - failedPipelineCount);
        var details = new List<string>
        {
            $"{completedPipelineCount.ToString(CultureInfo.InvariantCulture)}/{pipelineCount.ToString(CultureInfo.InvariantCulture)} {Pluralize(pipelineCount, "pipeline", "pipelines")} completed.",
            $"Run artifacts: {result.RunArtifactDirectoryPath}",
        };

        string? next = null;
        if (failed is not null)
        {
            var childFailure = SummarizeChildFailure(failed);
            details.Add(string.Empty);
            details.Add($"First failed: {failed.PipelineName}");
            if (!string.IsNullOrWhiteSpace(childFailure.Reason))
            {
                details.Add(childFailure.Reason);
            }
            else
            {
                details.Add($"Exit code {failed.ExitCode.ToString(CultureInfo.InvariantCulture)}.");
            }

            AddTail(details, "Output", childFailure.OutputLines);
            AddTail(details, "Error", childFailure.ErrorLines);
            next = childFailure.Next;
        }

        if (result.BlockedResults.Count > 0)
        {
            var blockedPipelineCount = result.BlockedResults
                .Select(static item => item.PipelineName)
                .Where(static item => !string.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();
            details.Add($"{blockedPipelineCount.ToString(CultureInfo.InvariantCulture)} {Pluralize(blockedPipelineCount, "pipeline was", "pipelines were")} blocked by dependencies.");
        }

        Fail(
            "Cannot complete orchestration.",
            next ?? "inspect the failed pipeline run, fix the underlying source problem, then rerun execute.",
            4,
            details);
    }

    private void PrintExecutionComplete(OrchestrationRuntimeResult result)
    {
        var pipelineCount = result.PipelineCount;
        presenter.WriteInfo($"{pipelineCount.ToString(CultureInfo.InvariantCulture)} {Pluralize(pipelineCount, "pipeline", "pipelines")} executed successfully.");
    }

    private ChildFailureSummary SummarizeChildFailure(OrchestrationTaskWorkerResult failed)
    {
        var failureMessage = NormalizeChildReason(failed.FailureMessage);
        var outputLines = NormalizeChildLines(failed.StandardOutput);
        var errorLines = NormalizeChildLines(failed.StandardError);
        var reason = failureMessage ?? outputLines.Concat(errorLines)
            .FirstOrDefault(static line =>
                !line.StartsWith("Next:", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(line, "Ok", StringComparison.OrdinalIgnoreCase));
        var next = outputLines.Concat(errorLines)
            .FirstOrDefault(line => line.StartsWith("Next:", StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(next) &&
            next.StartsWith("Next:", StringComparison.OrdinalIgnoreCase))
        {
            next = next["Next:".Length..].Trim();
        }

        next ??= SuggestNextForFailureReason(reason);
        outputLines = outputLines
            .Where(line => !string.Equals(line, reason, StringComparison.Ordinal) &&
                           !line.StartsWith("Next:", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        errorLines = errorLines
            .Where(line => !string.Equals(line, reason, StringComparison.Ordinal) &&
                           !line.StartsWith("Next:", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return new ChildFailureSummary(reason, next, outputLines, errorLines);
    }

    private static string[] NormalizeChildLines(string text)
    {
        return text
            .Split([Environment.NewLine, "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(static line => !string.Equals(line, "Cannot continue.", StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    private static string? NormalizeChildReason(string text)
    {
        var normalized = NormalizeChildLines(text)
            .FirstOrDefault(static line =>
                !line.StartsWith("Next:", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(line, "Ok", StringComparison.OrdinalIgnoreCase));
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static string? SuggestNextForFailureReason(string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return null;
        }

        return reason.Contains("Cannot open database", StringComparison.OrdinalIgnoreCase) ||
               reason.Contains("Login failed", StringComparison.OrdinalIgnoreCase)
            ? "deploy the target database from the generated MetaSql workspace, confirm the connection env vars and login permissions, then rerun execute."
            : null;
    }

    private static void AddTail(ICollection<string> details, string label, IReadOnlyCollection<string> lines)
    {
        if (lines.Count == 0)
        {
            return;
        }

        details.Add(label);
        foreach (var line in lines.TakeLast(6))
        {
            details.Add($"  {line}");
        }
    }

    private static bool IsActive(string value) =>
        string.Equals(value, "Active", StringComparison.OrdinalIgnoreCase);

    private void PrintIssues(MO.MetaOrchestrationModel model, int take)
    {
        foreach (var issue in model.DependencyIssueList
                     .OrderBy(static item => item.Code, StringComparer.Ordinal)
                     .ThenBy(static item => item.Message, StringComparer.Ordinal)
                     .Take(take))
        {
            presenter.WriteInfo($"  {issue.Id}: {issue.Code} [{issue.IssueDomain}/{issue.Severity}] BlocksDag={issue.BlocksDag} BlocksRunPlan={issue.BlocksAutomaticRunPlanning}");
            presenter.WriteInfo($"    {issue.Message}");
        }
    }

    private void PrintIssueDetails(MO.MetaOrchestrationModel model, MO.DependencyIssue issue)
    {
        presenter.WriteKeyValueBlock("MetaOrchestration", new[]
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
            presenter.WriteInfo($"  {pipeline}");
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

    private void PrintRunPlanGraph(MO.MetaOrchestrationModel model)
    {
        var runPlans = model.RunPlanList
            .OrderBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.Id, StringComparer.Ordinal)
            .ToArray();
        if (runPlans.Length == 0)
        {
            presenter.WriteInfo("No run plan.");
            return;
        }

        for (var runPlanIndex = 0; runPlanIndex < runPlans.Length; runPlanIndex++)
        {
            if (runPlanIndex > 0)
            {
                presenter.WriteInfo(string.Empty);
            }

            var runPlan = runPlans[runPlanIndex];
            var status = string.Equals(runPlan.RunPlanStatus, "Ready", StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : $" [{runPlan.RunPlanStatus}]";
            presenter.WriteInfo($"{runPlan.Name}{status}");
            var retryPolicy = model.RunPlanRetryPolicyList
                .Where(item => ReferenceEquals(item.RunPlan, runPlan))
                .Where(static item => string.Equals(item.PolicyRole, "Default", StringComparison.OrdinalIgnoreCase))
                .Select(static item => item.RetryPolicy)
                .OrderBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(static item => item.Id, StringComparer.Ordinal)
                .FirstOrDefault();
            if (retryPolicy is not null)
            {
                presenter.WriteInfo($"RetryPolicy: {retryPolicy.Name} (MaxAttempts={retryPolicy.MaxAttempts})");
            }

            var tasks = model.PlannedTaskList
                .Where(item => ReferenceEquals(item.RunPlan, runPlan))
                .OrderBy(static item => ParseOrdinal(item.Ordinal))
                .ThenBy(static item => item.Id, StringComparer.Ordinal)
                .ToArray();
            if (tasks.Length == 0)
            {
                presenter.WriteInfo("PlannedTasks: 0");
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
            presenter.WriteInfo($"PlannedTasks: {tasks.Length.ToString(CultureInfo.InvariantCulture)}");
            presenter.WriteInfo($"DependencyEdges: {edges.Length.ToString(CultureInfo.InvariantCulture)}");
            presenter.WriteInfo("Graph:");
            foreach (var task in tasks
                         .OrderBy(static item => FormatGraphTaskName(item), StringComparer.OrdinalIgnoreCase)
                         .ThenBy(static item => item.Id, StringComparer.Ordinal))
            {
                presenter.WriteInfo($"  {FormatGraphTaskName(task)}");
                if (!edgesByPredecessorId.TryGetValue(task.TaskAccessProfile.Id, out var outgoingEdges))
                {
                    presenter.WriteInfo("    (no outgoing dependencies)");
                    continue;
                }

                foreach (var edge in outgoingEdges)
                {
                    presenter.WriteInfo($"    --> {FormatGraphTaskName(edge.Successor)} [{FormatGraphEdgeLabel(edge)}]");
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

    private int ReadPositiveInt(string value, string optionName)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) &&
            parsed > 0)
        {
            return parsed;
        }

        Fail($"invalid value for {optionName}. Expected a positive integer.", HelpCommand("execute"));
        throw new UnreachableException();
    }

    private int? ReadNonNegativeInt(string? value, string optionName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) &&
            parsed >= 0)
        {
            return parsed;
        }

        Fail($"invalid value for {optionName}. Expected a non-negative integer; 0 means no timeout.", HelpCommand("execute"));
        throw new UnreachableException();
    }

    private string HelpCommand(string commandName) => $"{appName} help {commandName}";

    private static string ResolveWorkspacePath(MetaCliInvocation invocation) =>
        Path.GetFullPath(invocation.Optional("workspace") ?? Directory.GetCurrentDirectory());

    [DoesNotReturn]
    private void Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details is not null)
        {
            renderedDetails.AddRange(details.Where(static item => !string.IsNullOrWhiteSpace(item)));
        }

        renderedDetails.Add($"Next: {next}");
        presenter.WriteFailure(message, renderedDetails);
        throw new MetaCliExitException(exitCode);
    }

    private sealed class OrchestrationRuntimeProgressObserver(OrchestrationExecutionProgressRenderer progress) : IOrchestrationRuntimeObserver
    {
        public void PhaseChanged(string phase) => progress.SetPhase(phase);

        public void RunPlanReady(int totalTasks) => progress.RunPlanReady(totalTasks);

        public void RuntimeStateChanged(OrchestrationRuntimeProgressSnapshot snapshot) => progress.RuntimeStateChanged(snapshot);

        public void TaskStarted(string taskId, string taskName) => progress.TaskStarted(taskId, taskName);

        public void TaskCompleted(string taskId, bool succeeded) => progress.TaskCompleted(taskId, succeeded);

        public void TaskBlocked(string taskId) => progress.TaskBlocked(taskId);

        public void PipelineCompleted(string pipelineName) => progress.PipelineCompleted(pipelineName);
    }

    private sealed record ChildFailureSummary(
        string? Reason,
        string? Next,
        IReadOnlyList<string> OutputLines,
        IReadOnlyList<string> ErrorLines);

    private sealed record RunPlanGraphEdge(
        MO.TaskAccessProfile Predecessor,
        MO.TaskAccessProfile Successor,
        string Kind,
        string Condition,
        string? ObjectSqlIdentifier);

    private static string Pluralize(int count, string singular, string plural) =>
        count == 1 ? singular : plural;

    private static int ParseOrdinal(string value) =>
        int.TryParse(value, CultureInfo.InvariantCulture, out var ordinal) ? ordinal : int.MaxValue;
}
