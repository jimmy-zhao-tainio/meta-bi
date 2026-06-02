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

        using var progress = OrchestrationExecutionProgressRenderer.TryCreate();
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
                            : TimeSpan.FromSeconds(parse.WorkerEventTimeoutSeconds.Value)),
                    observer)
                .ConfigureAwait(false);

            progress?.Complete(failed: !result.Succeeded);
            if (!result.Succeeded)
            {
                return PrintExecutionIncomplete(result);
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
        var succeededCount = result.TaskResults.Count(static item => item.ExitCode == 0);
        var failedCount = result.TaskResults.Count(static item => item.ExitCode != 0);
        var details = new List<string>
        {
            $"{result.RunPlanName} stopped with unresolved paths.",
            $"  RunId: {result.RunId}",
            $"  RunArtifacts: {result.RunArtifactDirectoryPath}",
            $"  {succeededCount.ToString(CultureInfo.InvariantCulture)} succeeded",
            $"  {failedCount.ToString(CultureInfo.InvariantCulture)} failed",
            $"  {result.BlockedResults.Count.ToString(CultureInfo.InvariantCulture)} blocked",
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

        if (result.BlockedResults.Count > 0)
        {
            details.Add(string.Empty);
            details.Add("Blocked");
            foreach (var blocked in result.BlockedResults.Take(4))
            {
                details.Add($"  {blocked.PipelineName}.{blocked.StepName}");
            }

            if (result.BlockedResults.Count > 4)
            {
                details.Add($"  ... {result.BlockedResults.Count - 4} more");
            }
        }

        return Fail(
            "Cannot complete orchestration.",
            next ?? "inspect the failed pipeline run, fix the underlying source problem, then rerun execute.",
            4,
            details);
    }

    private static ChildFailureSummary SummarizeChildFailure(OrchestrationTaskWorkerResult failed)
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
                case "--run-artifacts-root":
                    runArtifactsRootPath = value;
                    break;
                case "--worker-event-timeout-seconds":
                    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedWorkerEventTimeoutSeconds) ||
                        parsedWorkerEventTimeoutSeconds <= 0)
                    {
                        return ParsedExecuteArgs.Fail("invalid value for --worker-event-timeout-seconds. Expected a positive integer.");
                    }

                    workerEventTimeoutSeconds = parsedWorkerEventTimeoutSeconds;
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
            runArtifactsRootPath,
            workerEventTimeoutSeconds,
            string.Empty);
    }

    private sealed class OrchestrationRuntimeProgressObserver(OrchestrationExecutionProgressRenderer progress) : IOrchestrationRuntimeObserver
    {
        public void PhaseChanged(string phase) => progress.SetPhase(phase);

        public void RunPlanReady(int totalTasks) => progress.RunPlanReady(totalTasks);

        public void TaskStarted(string taskId, string taskName) => progress.TaskStarted(taskId, taskName);

        public void TaskCompleted(string taskId, bool succeeded) => progress.TaskCompleted(taskId, succeeded);

        public void TaskBlocked(string taskId) => progress.TaskBlocked();
    }

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
        string RunArtifactsRootPath,
        int? WorkerEventTimeoutSeconds,
        string ErrorMessage)
    {
        public static ParsedExecuteArgs Fail(string errorMessage) =>
            new(false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 1, string.Empty, null, errorMessage);
    }
}
