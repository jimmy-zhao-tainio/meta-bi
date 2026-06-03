using System.Diagnostics;
using System.Globalization;
using MetaOrchestration.Core;

internal sealed class OrchestrationExecutionProgressRenderer : IDisposable
{
    private const int WorkRailWidth = 20;
    private const int WorkerRailWidth = 8;
    private readonly object sync = new();
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();
    private readonly OrchestrationLiveLineRenderer liveLine;
    private readonly HashSet<string> completedPipelineNames = new(StringComparer.OrdinalIgnoreCase);
    private readonly int maxParallelism;
    private string currentPhase = "starting";
    private int totalPipelines;
    private int liveWorkers;
    private bool disposed;

    private OrchestrationExecutionProgressRenderer(int maxParallelism)
    {
        this.maxParallelism = Math.Max(1, maxParallelism);
        liveLine = OrchestrationLiveLineRenderer.TryStart(
                BuildReadout,
                delay: TimeSpan.FromMilliseconds(180))
            ?? throw new InvalidOperationException("Console live-line renderer is not available.");
    }

    public static OrchestrationExecutionProgressRenderer? TryCreate(int maxParallelism)
    {
        if (Console.IsErrorRedirected || Console.IsOutputRedirected)
        {
            return null;
        }

        return new OrchestrationExecutionProgressRenderer(maxParallelism);
    }

    public void SetPhase(string phase)
    {
        lock (sync)
        {
            currentPhase = string.IsNullOrWhiteSpace(phase)
                ? "working"
                : phase.Trim().ToLowerInvariant();
        }
    }

    public void RunPlanReady(int totalPipelines)
    {
        lock (sync)
        {
            this.totalPipelines = Math.Max(totalPipelines, 1);
        }
    }

    public void RuntimeStateChanged(OrchestrationRuntimeProgressSnapshot snapshot)
    {
        lock (sync)
        {
            liveWorkers = Math.Max(0, snapshot.LiveWorkerCount);
        }
    }

    public void TaskStarted(string taskId, string taskName)
    {
        _ = taskId;
        _ = taskName;
    }

    public void TaskCompleted(string taskId, bool succeeded)
    {
        _ = taskId;
        _ = succeeded;
    }

    public void TaskBlocked(string taskId)
    {
        _ = taskId;
    }

    public void PipelineCompleted(string pipelineName)
    {
        lock (sync)
        {
            if (!string.IsNullOrWhiteSpace(pipelineName))
            {
                completedPipelineNames.Add(pipelineName);
            }
        }
    }

    public void Complete(bool failed)
    {
        lock (sync)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
        }

        liveLine.Complete(failed ? BuildFailureReadout() : BuildCompletionReadout());
    }

    public void Dispose()
    {
        lock (sync)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
        }

        liveLine.Dispose();
    }

    private string BuildReadout(char spinnerFrame)
    {
        lock (sync)
        {
            if (totalPipelines <= 0)
            {
                return currentPhase;
            }

            var completedPipelines = completedPipelineNames.Count;
            var countWidth = CountWidth(totalPipelines);
            var workerCountWidth = CountWidth(maxParallelism);
            var segments = new List<string>
            {
                $"Progress {BuildProgressRail(completedPipelines, totalPipelines, WorkRailWidth, spinnerFrame)} {FormatCount(completedPipelines, countWidth)}/{FormatCount(totalPipelines, countWidth)}",
                $"Workers {BuildProgressRail(liveWorkers, maxParallelism, WorkerRailWidth)} {FormatCount(liveWorkers, workerCountWidth)}/{FormatCount(maxParallelism, workerCountWidth)}"
            };

            return string.Join("  ", segments);
        }
    }

    private string BuildCompletionReadout()
    {
        lock (sync)
        {
            var completedPipelines = completedPipelineNames.Count;
            var countWidth = CountWidth(totalPipelines);
            var segments = new List<string>
            {
                $"Progress {BuildProgressRail(completedPipelines, totalPipelines, WorkRailWidth)} {FormatCount(completedPipelines, countWidth)}/{FormatCount(totalPipelines, countWidth)}",
                "OK",
                FormatElapsed(stopwatch.Elapsed)
            };

            return string.Join("  ", segments);
        }
    }

    private string BuildFailureReadout()
    {
        lock (sync)
        {
            if (totalPipelines <= 0)
            {
                return $"FAIL {currentPhase} {FormatElapsed(stopwatch.Elapsed)}";
            }

            var completedPipelines = completedPipelineNames.Count;
            var countWidth = CountWidth(totalPipelines);
            var segments = new List<string>
            {
                $"Progress {BuildProgressRail(completedPipelines, totalPipelines, WorkRailWidth)} {FormatCount(completedPipelines, countWidth)}/{FormatCount(totalPipelines, countWidth)}",
                "FAIL",
                FormatElapsed(stopwatch.Elapsed)
            };

            return string.Join("  ", segments);
        }
    }

    private static string BuildProgressRail(int completed, int total, int width)
    {
        var safeTotal = Math.Max(1, total);
        var safeWidth = Math.Max(1, width);
        var safeCompleted = Math.Clamp(completed, 0, safeTotal);
        var filled = safeCompleted >= safeTotal
            ? safeWidth
            : (int)Math.Floor(safeCompleted * safeWidth / (double)safeTotal);
        return $"[{new string('#', filled)}{new string('.', safeWidth - filled)}]";
    }

    private static string BuildProgressRail(int completed, int total, int width, char spinnerFrame)
    {
        var safeTotal = Math.Max(1, total);
        var safeWidth = Math.Max(1, width);
        var safeCompleted = Math.Clamp(completed, 0, safeTotal);
        if (safeCompleted >= safeTotal)
        {
            return BuildProgressRail(safeCompleted, safeTotal, safeWidth);
        }

        var spinnerIndex = (int)Math.Floor(safeCompleted * safeWidth / (double)safeTotal);
        spinnerIndex = Math.Clamp(spinnerIndex, 0, safeWidth - 1);
        return $"[{new string('#', spinnerIndex)}{spinnerFrame}{new string('.', safeWidth - spinnerIndex - 1)}]";
    }

    private static string FormatElapsed(TimeSpan elapsed) =>
        elapsed.TotalHours >= 1
            ? elapsed.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture)
            : elapsed.ToString(@"mm\:ss", CultureInfo.InvariantCulture);

    private static int CountWidth(int maxValue) =>
        Math.Max(2, Math.Max(1, maxValue).ToString(CultureInfo.InvariantCulture).Length);

    private static string FormatCount(int value, int width) =>
        Math.Max(0, value).ToString(CultureInfo.InvariantCulture).PadLeft(Math.Max(1, width));
}
