using System.Diagnostics;
using System.Globalization;
using System.Text;
using MetaOrchestration.Core;
using MO = MetaOrchestration;

internal static partial class Program
{
    private static async Task<int> RunExecuteAsync(string[] args, int startIndex)
    {
        var parse = ParseExecuteArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("execute"));
        }

        if (!string.IsNullOrWhiteSpace(parse.PipelineDbConnectionEnvironmentVariableName) &&
            string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(parse.PipelineDbConnectionEnvironmentVariableName)))
        {
            return Fail(
                "Cannot execute orchestration.",
                "set the named connection environment variable and retry.",
                4,
                [$"Connection environment variable '{parse.PipelineDbConnectionEnvironmentVariableName}' was not found."]);
        }

        var progress = OrchestrationExecutionProgressRenderer.TryCreate();
        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            progress?.SetPhase("Loading");
            var model = MO.MetaOrchestrationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            var runPlanningService = new MetaOrchestrationRunPlanningService();
            progress?.SetPhase("Building");
            runPlanningService.BuildRunPlan(model);
            progress?.SetPhase("Saving");
            model.SaveToXmlWorkspace(workspacePath);

            var runPlan = ResolveRunPlan(model);
            if (!string.Equals(runPlan.RunPlanStatus, "Ready", StringComparison.OrdinalIgnoreCase))
            {
                progress?.Complete(failed: true);
                progress?.Dispose();
                return Fail(
                    $"Run plan '{runPlan.Name}' is not ready.",
                    "resolve policy gaps, then retry execute or run meta-orchestration refresh-run-plan for preflight.",
                    4,
                    [$"  RunPlanStatus: {runPlan.RunPlanStatus}"]);
            }

            var plannedTasks = model.PlannedTaskList
                .Where(item => ReferenceEquals(item.RunPlan, runPlan))
                .OrderBy(static item => ParseOrdinal(item.Ordinal))
                .ThenBy(static item => item.Id, StringComparer.Ordinal)
                .ToArray();

            if (plannedTasks.Length == 0)
            {
                progress?.Complete(failed: true);
                progress?.Dispose();
                return Fail(
                    $"Run plan '{runPlan.Name}' has no planned tasks.",
                    "run meta-orchestration refresh-run-plan --workspace <path>, then inspect-run-plan.",
                    4);
            }

            progress?.RunPlanReady(plannedTasks.Length);
            var allResults = new List<PlannedTaskProcessResult>();
            var skippedResults = new List<PlannedTaskSkipResult>();
            var taskOutcomesByTaskProfileId = new Dictionary<string, string>(StringComparer.Ordinal);
            var dependenciesByTaskProfileId = OrchestrationExecutionContinuity.BuildDependencyMap(model);
            var plannedTasksByProfileId = plannedTasks
                .GroupBy(static item => item.TaskAccessProfile.Id, StringComparer.Ordinal)
                .ToDictionary(
                    static group => group.Key,
                    static group => group.OrderBy(static item => item.Id, StringComparer.Ordinal).First(),
                    StringComparer.Ordinal);

            var locksByPlannedTaskId = model.PlannedTaskLockList
                .Where(item => plannedTasks.Any(task => ReferenceEquals(item.PlannedTask, task)))
                .GroupBy(static item => item.PlannedTask.Id, StringComparer.Ordinal)
                .ToDictionary(
                    static group => group.Key,
                    static group => group.OrderBy(static item => item.DataObject.NormalizedKey, StringComparer.OrdinalIgnoreCase).ToArray(),
                    StringComparer.Ordinal);
            var activeLockPolicies = model.LockCompatibilityPolicyList
                .Where(static item => IsActive(item.Status))
                .ToArray();

            await ExecuteReadyGraphAsync(
                plannedTasks,
                locksByPlannedTaskId,
                activeLockPolicies,
                dependenciesByTaskProfileId,
                plannedTasksByProfileId,
                taskOutcomesByTaskProfileId,
                allResults,
                skippedResults,
                parse,
                progress).ConfigureAwait(false);

            var hasFailure = allResults.Any(static item => item.ExitCode != 0) ||
                             skippedResults.Any(static item => string.Equals(item.SkipOutcome, OrchestrationExecutionContinuity.SkippedBlocked, StringComparison.Ordinal));
            progress?.Complete(failed: hasFailure);
            progress?.Dispose();
            if (hasFailure)
            {
                return PrintExecutionIncomplete(runPlan, allResults, skippedResults);
            }

            if (progress is null)
            {
                Presenter.WriteOk();
            }

            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException)
        {
            progress?.Complete(failed: true);
            progress?.Dispose();
            return Fail(
                "Cannot execute orchestration.",
                "check the orchestration workspace, pipeline workspace, child meta-pipeline command availability, and retry.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static async Task ExecuteReadyGraphAsync(
        IReadOnlyList<MO.PlannedTask> plannedTasks,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]> locksByPlannedTaskId,
        IReadOnlyList<MO.LockCompatibilityPolicy> activeLockPolicies,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependenciesByTaskProfileId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId,
        Dictionary<string, string> taskOutcomesByTaskProfileId,
        ICollection<PlannedTaskProcessResult> allResults,
        ICollection<PlannedTaskSkipResult> skippedResults,
        ParsedExecuteArgs parse,
        OrchestrationExecutionProgressRenderer? progress)
    {
        var pending = plannedTasks
            .OrderBy(static item => ParseOrdinal(item.Ordinal))
            .ThenBy(static item => item.Id, StringComparer.Ordinal)
            .ToList();
        var running = new List<Task<PlannedTaskProcessResult>>();
        var runningLocksByTaskId = new Dictionary<string, MO.PlannedTaskLock[]>(StringComparer.Ordinal);

        while (pending.Count > 0 || running.Count > 0)
        {
            var madeProgress = false;
            foreach (var plannedTask in pending.ToArray())
            {
                if (running.Count >= parse.MaxDegreeOfParallelism)
                {
                    break;
                }

                var readiness = OrchestrationExecutionContinuity.EvaluateReadiness(
                    plannedTask,
                    dependenciesByTaskProfileId,
                    taskOutcomesByTaskProfileId,
                    out var dependency,
                    out var skipOutcome,
                    out var skipReason);

                if (readiness == OrchestrationTaskReadiness.Waiting)
                {
                    continue;
                }

                pending.Remove(plannedTask);
                madeProgress = true;

                if (readiness == OrchestrationTaskReadiness.Skip)
                {
                    skippedResults.Add(CreateSkipResult(
                        plannedTask,
                        dependency,
                        skipOutcome,
                        skipReason,
                        plannedTasksByProfileId));
                    taskOutcomesByTaskProfileId[plannedTask.TaskAccessProfile.Id] = skipOutcome;
                    progress?.TaskSkipped();
                    continue;
                }

                var plannedTaskLocks = locksByPlannedTaskId.TryGetValue(plannedTask.Id, out var locks)
                    ? locks
                    : [];
                if (!AreLocksCompatibleWithRunning(plannedTaskLocks, runningLocksByTaskId.Values.SelectMany(static item => item).ToArray(), activeLockPolicies))
                {
                    pending.Add(plannedTask);
                    madeProgress = false;
                    continue;
                }

                progress?.TaskStarted(plannedTask.Id, FormatTaskName(plannedTask));
                runningLocksByTaskId[plannedTask.Id] = plannedTaskLocks;
                running.Add(RunPlannedTaskProcessAsync(plannedTask, parse));
            }

            if (running.Count == 0)
            {
                if (!madeProgress && pending.Count > 0)
                {
                    throw new InvalidOperationException("Cannot execute run plan because remaining tasks are waiting on predecessors that are not in the run plan.");
                }

                continue;
            }

            var completed = await Task.WhenAny(running).ConfigureAwait(false);
            running.Remove(completed);
            var result = await completed.ConfigureAwait(false);
            allResults.Add(result);
            taskOutcomesByTaskProfileId[result.TaskAccessProfileId] = OrchestrationExecutionContinuity.OutcomeForExitCode(result.ExitCode);
            runningLocksByTaskId.Remove(result.PlannedTaskId);
            progress?.TaskCompleted(result.PlannedTaskId, result.ExitCode == 0);
        }
    }

    private static async Task<PlannedTaskProcessResult> RunPlannedTaskProcessAsync(
        MO.PlannedTask plannedTask,
        ParsedExecuteArgs parse)
    {
        var pipelineName = plannedTask.PipelineReference.Name;
        var displayStepName = plannedTask.TaskAccessProfile.TaskName;
        var stepSelector = string.IsNullOrWhiteSpace(plannedTask.TaskAccessProfile.MetaPipelinePipelineTaskId)
            ? displayStepName
            : plannedTask.TaskAccessProfile.MetaPipelinePipelineTaskId;
        var output = new StringBuilder();
        var error = new StringBuilder();
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "meta-pipeline",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            startInfo.ArgumentList.Add("execute-step");
            startInfo.ArgumentList.Add("--workspace");
            startInfo.ArgumentList.Add(parse.PipelineWorkspacePath);
            startInfo.ArgumentList.Add("--pipeline");
            startInfo.ArgumentList.Add(pipelineName);
            startInfo.ArgumentList.Add("--step-name");
            startInfo.ArgumentList.Add(stepSelector);
            startInfo.ArgumentList.Add("--transform-workspace");
            startInfo.ArgumentList.Add(parse.TransformWorkspacePath);
            startInfo.ArgumentList.Add("--binding-workspace");
            startInfo.ArgumentList.Add(parse.BindingWorkspacePath);

            if (!string.IsNullOrWhiteSpace(parse.DataTypeConversionWorkspacePath))
            {
                startInfo.ArgumentList.Add("--data-type-conversion-workspace");
                startInfo.ArgumentList.Add(parse.DataTypeConversionWorkspacePath);
            }

            if (!string.IsNullOrWhiteSpace(parse.PipelineDbConnectionEnvironmentVariableName))
            {
                startInfo.ArgumentList.Add("--pipeline-db-connection-env");
                startInfo.ArgumentList.Add(parse.PipelineDbConnectionEnvironmentVariableName);
            }

            using var process = new Process { StartInfo = startInfo };
            process.Start();
            var outputTask = ReadToBuilderAsync(process.StandardOutput, output);
            var errorTask = ReadToBuilderAsync(process.StandardError, error);
            await process.WaitForExitAsync().ConfigureAwait(false);
            await Task.WhenAll(outputTask, errorTask).ConfigureAwait(false);

            return new PlannedTaskProcessResult(
                plannedTask.TaskAccessProfile.Id,
                plannedTask.Id,
                pipelineName,
                displayStepName,
                process.ExitCode,
                output.ToString(),
                error.ToString());
        }
        catch (Exception ex)
        {
            return new PlannedTaskProcessResult(
                plannedTask.TaskAccessProfile.Id,
                plannedTask.Id,
                pipelineName,
                displayStepName,
                -1,
                output.ToString(),
                error.AppendLine(ex.Message).ToString());
        }
    }

    private static bool AreLocksCompatibleWithRunning(
        IReadOnlyList<MO.PlannedTaskLock> candidateLocks,
        IReadOnlyList<MO.PlannedTaskLock> runningLocks,
        IReadOnlyList<MO.LockCompatibilityPolicy> activeLockPolicies)
    {
        foreach (var runningLock in runningLocks)
        {
            foreach (var candidateLock in candidateLocks)
            {
                if (!string.Equals(runningLock.DataObject.Id, candidateLock.DataObject.Id, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!ArePlannedTaskLocksCompatible(runningLock, candidateLock, activeLockPolicies))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool ArePlannedTaskLocksCompatible(
        MO.PlannedTaskLock left,
        MO.PlannedTaskLock right,
        IReadOnlyList<MO.LockCompatibilityPolicy> activeLockPolicies)
    {
        if (string.Equals(left.LockMode, "SharedRead", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(right.LockMode, "SharedRead", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var policy = activeLockPolicies
            .Where(item => string.Equals(item.DataObject.Id, left.DataObject.Id, StringComparison.Ordinal))
            .Where(item => EffectsMatch(item, left.TaskObjectEffect.WriteEffect, right.TaskObjectEffect.WriteEffect))
            .OrderBy(static item => item.PolicyKind, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.Id, StringComparer.Ordinal)
            .FirstOrDefault();

        return policy is not null &&
               string.Equals(policy.LockBehavior, "AllowConcurrent", StringComparison.OrdinalIgnoreCase);
    }

    private static bool EffectsMatch(MO.LockCompatibilityPolicy policy, string leftEffect, string rightEffect)
    {
        return
            (string.Equals(policy.LeftEffect, leftEffect, StringComparison.OrdinalIgnoreCase) &&
             string.Equals(policy.RightEffect, rightEffect, StringComparison.OrdinalIgnoreCase)) ||
            (string.Equals(policy.LeftEffect, rightEffect, StringComparison.OrdinalIgnoreCase) &&
             string.Equals(policy.RightEffect, leftEffect, StringComparison.OrdinalIgnoreCase));
    }

    private static async Task ReadToBuilderAsync(StreamReader reader, StringBuilder builder)
    {
        while (await reader.ReadLineAsync().ConfigureAwait(false) is { } line)
        {
            builder.AppendLine(line);
        }
    }

    private static int PrintExecutionIncomplete(
        MO.RunPlan runPlan,
        IReadOnlyList<PlannedTaskProcessResult> results,
        IReadOnlyList<PlannedTaskSkipResult> skippedResults)
    {
        var failed = results.FirstOrDefault(static item => item.ExitCode != 0);
        var succeededCount = results.Count(static item => item.ExitCode == 0);
        var failedCount = results.Count(static item => item.ExitCode != 0);
        var details = new List<string>
        {
            $"{runPlan.Name} stopped with unresolved paths.",
            $"  {succeededCount.ToString(CultureInfo.InvariantCulture)} succeeded",
            $"  {failedCount.ToString(CultureInfo.InvariantCulture)} failed",
            $"  {skippedResults.Count.ToString(CultureInfo.InvariantCulture)} skipped",
        };

        string? next = null;
        if (failed is not null)
        {
            var childFailure = SummarizeChildFailure(failed);
            details.Add(string.Empty);
            details.Add("First failed");
            details.Add($"  {failed.PipelineName}.{failed.StepName}");
            if (!string.IsNullOrWhiteSpace(childFailure.Reason))
            {
                details.Add($"  {childFailure.Reason}");
            }
            else
            {
                details.Add($"  exit code {failed.ExitCode.ToString(CultureInfo.InvariantCulture)}");
            }

            AddTail(details, "Output", childFailure.OutputLines);
            AddTail(details, "Error", childFailure.ErrorLines);
            next = childFailure.Next;
        }

        if (skippedResults.Count > 0)
        {
            details.Add(string.Empty);
            details.Add("Skipped");
            foreach (var skipped in skippedResults.Take(4))
            {
                details.Add($"  {skipped.PipelineName}.{skipped.StepName}");
            }

            if (skippedResults.Count > 4)
            {
                details.Add($"  ... {skippedResults.Count - 4} more");
            }
        }

        return Fail(
            "Cannot complete orchestration.",
            next ?? "inspect the failed pipeline run, fix the underlying source problem, then rerun execute.",
            4,
            details);
    }

    private static PlannedTaskSkipResult CreateSkipResult(
        MO.PlannedTask plannedTask,
        OrchestrationExecutionDependency dependency,
        string skipOutcome,
        string skipReason,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId)
    {
        var blockingTaskProfileId = dependency.PredecessorTaskProfileId;
        var blockingTask = plannedTasksByProfileId.GetValueOrDefault(blockingTaskProfileId);
        return new PlannedTaskSkipResult(
            plannedTask.Id,
            plannedTask.TaskAccessProfile.Id,
            plannedTask.PipelineReference.Name,
            plannedTask.TaskAccessProfile.TaskName,
            blockingTaskProfileId,
            blockingTask?.PipelineReference.Name ?? "<unknown>",
            blockingTask?.TaskAccessProfile.TaskName ?? blockingTaskProfileId,
            dependency.Condition,
            skipOutcome,
            skipReason);
    }

    private static ChildFailureSummary SummarizeChildFailure(PlannedTaskProcessResult failed)
    {
        var outputLines = NormalizeChildLines(failed.StandardOutput);
        var errorLines = NormalizeChildLines(failed.StandardError);
        var reason = outputLines.Concat(errorLines)
            .FirstOrDefault(line =>
                !line.StartsWith("Next:", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(line, "Ok", StringComparison.OrdinalIgnoreCase));
        var next = outputLines.Concat(errorLines)
            .FirstOrDefault(line => line.StartsWith("Next:", StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(next) &&
            next.StartsWith("Next:", StringComparison.OrdinalIgnoreCase))
        {
            next = next["Next:".Length..].Trim();
        }

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

    private static string FormatTaskName(MO.PlannedTask plannedTask) =>
        $"{plannedTask.PipelineReference.Name}.{plannedTask.TaskAccessProfile.TaskName}";

    private static bool IsActive(string value) =>
        string.Equals(value, "Active", StringComparison.OrdinalIgnoreCase);

    private static MO.RunPlan ResolveRunPlan(MO.MetaOrchestrationModel model)
    {
        return model.RunPlanList.Count switch
        {
            1 => model.RunPlanList[0],
            0 => throw new InvalidOperationException("The orchestration workspace contains no RunPlan rows."),
            _ => throw new InvalidOperationException("The orchestration workspace contains multiple run plans.")
        };
    }

    private static ParsedExecuteArgs ParseExecuteArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var pipelineWorkspacePath = string.Empty;
        var transformWorkspacePath = string.Empty;
        var bindingWorkspacePath = string.Empty;
        var dataTypeConversionWorkspacePath = string.Empty;
        var pipelineDbConnectionEnvironmentVariableName = string.Empty;
        var maxDegreeOfParallelism = 1;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var option = args[i];
            if (i + 1 >= args.Length)
            {
                return ParsedExecuteArgs.Fail($"missing value for {option}.");
            }

            var value = args[++i];
            if (!seen.Add(option))
            {
                return ParsedExecuteArgs.Fail($"{option} can only be provided once.");
            }

            switch (option.ToLowerInvariant())
            {
                case "--workspace":
                    workspacePath = value;
                    break;
                case "--pipeline-workspace":
                    pipelineWorkspacePath = value;
                    break;
                case "--transform-workspace":
                    transformWorkspacePath = value;
                    break;
                case "--binding-workspace":
                    bindingWorkspacePath = value;
                    break;
                case "--data-type-conversion-workspace":
                    dataTypeConversionWorkspacePath = value;
                    break;
                case "--pipeline-db-connection-env":
                    pipelineDbConnectionEnvironmentVariableName = value;
                    break;
                case "--max-degree-of-parallelism":
                    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out maxDegreeOfParallelism) ||
                        maxDegreeOfParallelism <= 0)
                    {
                        return ParsedExecuteArgs.Fail("invalid value for --max-degree-of-parallelism. Expected a positive integer.");
                    }

                    break;
                default:
                    return ParsedExecuteArgs.Fail($"unknown option '{option}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return ParsedExecuteArgs.Fail("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(pipelineWorkspacePath)) return ParsedExecuteArgs.Fail("missing required option --pipeline-workspace <path>.");
        if (string.IsNullOrWhiteSpace(transformWorkspacePath)) return ParsedExecuteArgs.Fail("missing required option --transform-workspace <path>.");
        if (string.IsNullOrWhiteSpace(bindingWorkspacePath)) return ParsedExecuteArgs.Fail("missing required option --binding-workspace <path>.");

        return new ParsedExecuteArgs(
            true,
            workspacePath,
            pipelineWorkspacePath,
            transformWorkspacePath,
            bindingWorkspacePath,
            dataTypeConversionWorkspacePath,
            pipelineDbConnectionEnvironmentVariableName,
            maxDegreeOfParallelism,
            string.Empty);
    }

    private sealed record PlannedTaskProcessResult(
        string TaskAccessProfileId,
        string PlannedTaskId,
        string PipelineName,
        string StepName,
        int ExitCode,
        string StandardOutput,
        string StandardError);

    private sealed record PlannedTaskSkipResult(
        string PlannedTaskId,
        string TaskAccessProfileId,
        string PipelineName,
        string StepName,
        string BlockingTaskAccessProfileId,
        string BlockingPipelineName,
        string BlockingStepName,
        string DependencyCondition,
        string SkipOutcome,
        string Reason);

    private sealed record ChildFailureSummary(
        string? Reason,
        string? Next,
        IReadOnlyList<string> OutputLines,
        IReadOnlyList<string> ErrorLines);

    private sealed record ParsedExecuteArgs(
        bool Ok,
        string WorkspacePath,
        string PipelineWorkspacePath,
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        string DataTypeConversionWorkspacePath,
        string PipelineDbConnectionEnvironmentVariableName,
        int MaxDegreeOfParallelism,
        string ErrorMessage)
    {
        public static ParsedExecuteArgs Fail(string errorMessage) =>
            new(false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 1, errorMessage);
    }
}
