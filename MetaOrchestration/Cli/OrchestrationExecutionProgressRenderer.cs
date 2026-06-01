using Meta.Core.Presentation.Cli;

internal sealed class OrchestrationExecutionProgressRenderer : IDisposable
{
    private const int ProgressRailWidth = 20;
    private readonly object sync = new();
    private readonly CliLiveLineRenderer liveLine;
    private readonly Dictionary<string, string> runningTaskNamesById = new(StringComparer.Ordinal);
    private string currentPhase = "starting";
    private int totalTasks;
    private int completedTasks;
    private int failedTasks;
    private int blockedTasks;
    private bool disposed;

    private OrchestrationExecutionProgressRenderer()
    {
        liveLine = CliLiveLineRenderer.TryStart(BuildReadout, TimeSpan.FromMilliseconds(180))
            ?? throw new InvalidOperationException("Console live-line renderer is not available.");
    }

    public static OrchestrationExecutionProgressRenderer? TryCreate()
    {
        if (Console.IsErrorRedirected || Console.IsOutputRedirected)
        {
            return null;
        }

        return new OrchestrationExecutionProgressRenderer();
    }

    public void SetPhase(string phase)
    {
        lock (sync)
        {
            currentPhase = string.IsNullOrWhiteSpace(phase)
                ? "working"
                : phase.Trim();
        }
    }

    public void RunPlanReady(int totalTasks)
    {
        lock (sync)
        {
            this.totalTasks = Math.Max(totalTasks, 1);
        }
    }

    public void TaskStarted(string taskId, string taskName)
    {
        lock (sync)
        {
            runningTaskNamesById[taskId] = string.IsNullOrWhiteSpace(taskName)
                ? taskId
                : taskName.Trim();
        }
    }

    public void TaskCompleted(string taskId, bool succeeded)
    {
        lock (sync)
        {
            runningTaskNamesById.Remove(taskId);
            completedTasks++;
            if (!succeeded)
            {
                failedTasks++;
            }
        }
    }

    public void TaskBlocked()
    {
        lock (sync)
        {
            completedTasks++;
            blockedTasks++;
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

        if (failed)
        {
            liveLine.Clear();
            return;
        }

        liveLine.Complete(BuildCompletionReadout());
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

    private string BuildReadout()
    {
        lock (sync)
        {
            if (totalTasks <= 0)
            {
                return currentPhase;
            }

            var readout = $"{BuildProgressRail(completedTasks, totalTasks)} {completedTasks} of {totalTasks}";
            if (runningTaskNamesById.Count == 1)
            {
                readout += $"  {Shorten(runningTaskNamesById.Values.First(), 28)}";
            }
            else if (runningTaskNamesById.Count > 1)
            {
                readout += $"  {runningTaskNamesById.Count} running";
            }

            if (failedTasks > 0)
            {
                readout += $"  {failedTasks} failed";
            }

            if (blockedTasks > 0)
            {
                readout += $"  {blockedTasks} blocked";
            }

            return readout;
        }
    }

    private string BuildCompletionReadout()
    {
        lock (sync)
        {
            if (totalTasks <= 0)
            {
                return "Complete";
            }

            var readout = $"{BuildProgressRail(completedTasks, totalTasks)} {completedTasks} of {totalTasks}";
            if (blockedTasks > 0)
            {
                readout += $"  {blockedTasks} blocked";
            }

            return readout;
        }
    }

    private static string BuildProgressRail(int completed, int total)
    {
        var safeTotal = Math.Max(1, total);
        const int width = ProgressRailWidth;
        var safeCompleted = Math.Clamp(completed, 0, safeTotal);
        var filled = safeCompleted >= safeTotal
            ? width
            : (int)Math.Floor(safeCompleted * width / (double)safeTotal);
        return $"[{new string('=', filled)}{new string('-', width - filled)}]";
    }

    private static string Shorten(string value, int maxLength)
    {
        var normalized = value.Trim();
        if (normalized.Length <= maxLength)
        {
            return normalized;
        }

        return normalized[..Math.Max(0, maxLength - 3)] + "...";
    }
}
