using System.Diagnostics;
using System.Globalization;
using Meta.Core.Presentation.Cli;

internal sealed class PipelineConsoleProgressRenderer : IProgress<MetaPipeline.BufferedPipelineExecutionProgress>, IDisposable
{
    private const int ProgressRailWidth = 20;
    private readonly object sync = new();
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();
    private readonly CliLiveLineRenderer liveLine;
    private readonly int totalSteps;
    private int completedSteps;
    private int currentStepIndex;
    private int baseBatchCount;
    private long baseRowCount;
    private long baseByteCount;
    private int batchCount;
    private long rowCount;
    private long byteCount;
    private string status = "running";
    private bool disposed;

    private PipelineConsoleProgressRenderer(int totalSteps)
    {
        this.totalSteps = Math.Max(totalSteps, 1);
        liveLine = CliLiveLineRenderer.TryStart(BuildReadout, TimeSpan.FromSeconds(1))
            ?? throw new InvalidOperationException("Console live-line renderer is not available.");
    }

    public static PipelineConsoleProgressRenderer? TryCreate(int totalSteps)
    {
        if (Console.IsErrorRedirected || Console.IsOutputRedirected)
        {
            return null;
        }

        return new PipelineConsoleProgressRenderer(totalSteps);
    }

    public void StartStep(int stepIndex, string stepName)
    {
        _ = stepName;
        lock (sync)
        {
            currentStepIndex = Math.Clamp(stepIndex, 1, totalSteps);
            status = "running";
            baseRowCount = rowCount;
            baseBatchCount = batchCount;
            baseByteCount = byteCount;
        }
    }

    public void CompleteStep(bool succeeded, long completedRowCount = 0, int completedBatchCount = 0)
    {
        lock (sync)
        {
            rowCount = Math.Max(rowCount, baseRowCount + Math.Max(0, completedRowCount));
            batchCount = Math.Max(batchCount, baseBatchCount + Math.Max(0, completedBatchCount));
            if (succeeded)
            {
                completedSteps = Math.Max(completedSteps, currentStepIndex);
                status = "running";
            }
            else
            {
                status = "failed";
            }
        }
    }

    public void Report(MetaPipeline.BufferedPipelineExecutionProgress value)
    {
        lock (sync)
        {
            rowCount = baseRowCount + value.RowCount;
            batchCount = baseBatchCount + value.BatchCount;
            byteCount = baseByteCount + value.EstimatedByteCount;
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
            var elapsed = stopwatch.Elapsed;
            var rate = elapsed.TotalSeconds <= 0
                ? 0
                : byteCount / elapsed.TotalSeconds;
            var stepText = $"{BuildProgressRail(completedSteps, totalSteps)} {completedSteps} of {totalSteps}";
            var statusText = string.Equals(status, "failed", StringComparison.Ordinal)
                ? "  failed"
                : string.Empty;

            return string.Create(
                CultureInfo.InvariantCulture,
                $"{stepText}  {rowCount:N0} rows  {FormatByteRate(rate)}{statusText}");
        }
    }

    private string BuildCompletionReadout()
    {
        lock (sync)
        {
            var elapsed = stopwatch.Elapsed;
            var rate = elapsed.TotalSeconds <= 0
                ? 0
                : byteCount / elapsed.TotalSeconds;

            return string.Create(
                CultureInfo.InvariantCulture,
                $"{BuildProgressRail(completedSteps, totalSteps)} {completedSteps} of {totalSteps}  {rowCount:N0} rows  {FormatByteRate(rate)}");
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

    private static string FormatByteRate(double bytesPerSecond)
    {
        string[] units = ["B/s", "KB/s", "MB/s", "GB/s"];
        var value = Math.Max(0, bytesPerSecond);
        var unitIndex = 0;
        while (value >= 1024 && unitIndex < units.Length - 1)
        {
            value /= 1024;
            unitIndex++;
        }

        return unitIndex == 0
            ? $"{value:0} {units[unitIndex]}"
            : $"{value:0.0} {units[unitIndex]}";
    }
}
