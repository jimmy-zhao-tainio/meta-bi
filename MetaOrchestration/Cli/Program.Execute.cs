using System.Globalization;
using MetaOrchestration.Core;

internal static partial class Program
{
    private static async Task<int> RunExecuteAsync(string[] args, int startIndex)
    {
        var parse = ParseExecuteArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("execute"));
        }

        using var progress = OrchestrationExecutionProgressRenderer.TryCreate(parse.MaxDegreeOfParallelism);
        try
        {
            var observer = progress is null
                ? null
                : new OrchestrationRuntimeProgressObserver(progress);
            var result = await new MetaOrchestrationRuntimeService()
                .ExecuteAsync(
                    new OrchestrationRuntimeRequest(
                        parse.WorkspacePath,
                        parse.PipelineWorkspacePath,
                        parse.TransformWorkspacePath,
                        parse.BindingWorkspacePath,
                        parse.DataTypeConversionWorkspacePath,
                        parse.PipelineDbConnectionEnvironmentVariableName,
                        parse.MaxDegreeOfParallelism,
                        RunArtifactsRootPath: parse.RunArtifactsRootPath,
                        WorkerEventTimeout: parse.WorkerEventTimeoutSeconds is null
                            ? null
                            : TimeSpan.FromSeconds(parse.WorkerEventTimeoutSeconds.Value),
                        WorkerActivationTimeout: parse.WorkerActivationTimeoutSeconds is null
                            ? null
                            : TimeSpan.FromSeconds(parse.WorkerActivationTimeoutSeconds.Value),
                        WorkerControlPipeConnectTimeout: parse.WorkerControlPipeConnectTimeoutSeconds is null
                            ? null
                            : TimeSpan.FromSeconds(parse.WorkerControlPipeConnectTimeoutSeconds.Value)),
                    observer)
                .ConfigureAwait(false);

            progress?.Complete(failed: !result.Succeeded);
            if (!result.Succeeded)
            {
                return PrintExecutionIncomplete(result);
            }

            PrintExecutionComplete(result);

            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException)
        {
            progress?.Complete(failed: true);
            return Fail(
                "Cannot execute orchestration.",
                "check the orchestration workspace, pipeline workspace, child meta-pipeline worker availability, and retry.",
                4,
                [$"  {ex.Message}"]);
        }
        catch (OperationCanceledException ex)
        {
            progress?.Complete(failed: true);
            return Fail(
                "Orchestration execution was cancelled.",
                "inspect the run artifacts for the last supervisor state and worker logs before rerunning execute.",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static int PrintExecutionIncomplete(OrchestrationRuntimeResult result)
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

        return Fail(
            "Cannot complete orchestration.",
            next ?? "inspect the failed pipeline run, fix the underlying source problem, then rerun execute.",
            4,
            details);
    }

    private static void PrintExecutionComplete(OrchestrationRuntimeResult result)
    {
        var pipelineCount = result.PipelineCount;
        Presenter.WriteInfo($"{pipelineCount.ToString(CultureInfo.InvariantCulture)} {Pluralize(pipelineCount, "pipeline", "pipelines")} executed successfully.");
    }

    private static ChildFailureSummary SummarizeChildFailure(OrchestrationTaskWorkerResult failed)
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

    private static ParsedExecuteArgs ParseExecuteArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var pipelineWorkspacePath = string.Empty;
        var transformWorkspacePath = string.Empty;
        var bindingWorkspacePath = string.Empty;
        var dataTypeConversionWorkspacePath = string.Empty;
        var pipelineDbConnectionEnvironmentVariableName = string.Empty;
        var maxDegreeOfParallelism = 1;
        var runArtifactsRootPath = string.Empty;
        int? workerEventTimeoutSeconds = null;
        int? workerActivationTimeoutSeconds = null;
        int? workerControlPipeConnectTimeoutSeconds = null;
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
                case "--run-artifacts-root":
                    runArtifactsRootPath = value;
                    break;
                case "--worker-event-timeout-seconds":
                    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedWorkerEventTimeoutSeconds) ||
                        parsedWorkerEventTimeoutSeconds < 0)
                    {
                        return ParsedExecuteArgs.Fail("invalid value for --worker-event-timeout-seconds. Expected a non-negative integer; 0 means no timeout.");
                    }

                    workerEventTimeoutSeconds = parsedWorkerEventTimeoutSeconds;
                    break;
                case "--worker-activation-timeout-seconds":
                    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedWorkerActivationTimeoutSeconds) ||
                        parsedWorkerActivationTimeoutSeconds < 0)
                    {
                        return ParsedExecuteArgs.Fail("invalid value for --worker-activation-timeout-seconds. Expected a non-negative integer; 0 means no timeout.");
                    }

                    workerActivationTimeoutSeconds = parsedWorkerActivationTimeoutSeconds;
                    break;
                case "--worker-control-pipe-connect-timeout-seconds":
                    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedWorkerControlPipeConnectTimeoutSeconds) ||
                        parsedWorkerControlPipeConnectTimeoutSeconds < 0)
                    {
                        return ParsedExecuteArgs.Fail("invalid value for --worker-control-pipe-connect-timeout-seconds. Expected a non-negative integer; 0 means no timeout.");
                    }

                    workerControlPipeConnectTimeoutSeconds = parsedWorkerControlPipeConnectTimeoutSeconds;
                    break;
                default:
                    return ParsedExecuteArgs.Fail($"unknown option '{option}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return ParsedExecuteArgs.Fail("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(pipelineWorkspacePath)) return ParsedExecuteArgs.Fail("missing required option --pipeline-workspace <path>.");

        return new ParsedExecuteArgs(
            true,
            workspacePath,
            pipelineWorkspacePath,
            transformWorkspacePath,
            bindingWorkspacePath,
            dataTypeConversionWorkspacePath,
            pipelineDbConnectionEnvironmentVariableName,
            maxDegreeOfParallelism,
            runArtifactsRootPath,
            workerEventTimeoutSeconds,
            workerActivationTimeoutSeconds,
            workerControlPipeConnectTimeoutSeconds,
            string.Empty);
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

    private static string Pluralize(int count, string singular, string plural) =>
        count == 1 ? singular : plural;

    private sealed record ParsedExecuteArgs(
        bool Ok,
        string WorkspacePath,
        string PipelineWorkspacePath,
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        string DataTypeConversionWorkspacePath,
        string PipelineDbConnectionEnvironmentVariableName,
        int MaxDegreeOfParallelism,
        string RunArtifactsRootPath,
        int? WorkerEventTimeoutSeconds,
        int? WorkerActivationTimeoutSeconds,
        int? WorkerControlPipeConnectTimeoutSeconds,
        string ErrorMessage)
    {
        public static ParsedExecuteArgs Fail(string errorMessage) =>
            new(false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 1, string.Empty, null, null, null, errorMessage);
    }
}
