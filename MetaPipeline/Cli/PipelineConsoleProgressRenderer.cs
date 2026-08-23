using System.Diagnostics;
using System.Globalization;
using MetaCli.Core;

internal sealed class PipelineConsoleProgressRenderer : MetaPipeline.IMetaPipelineExecutionProgress
{
    private readonly object sync = new();
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();
    private readonly MetaCliProgressMeter meter;
    private readonly int totalSteps;
    private int completedSteps;
    private int currentStepIndex;
    private long baseRowCount;
    private long baseByteCount;
    private long rowCount;
    private long byteCount;
    private bool failed;
    private bool disposed;

    private PipelineConsoleProgressRenderer(int totalSteps, MetaCliProgressMeter meter)
    {
        this.totalSteps = Math.Max(totalSteps, 1);
        this.meter = meter;
        meter.Report(0, this.totalSteps, BuildDetail());
    }

    public static PipelineConsoleProgressRenderer? TryCreate(int totalSteps)
    {
        var meter = MetaCliProgressMeter.TryStart(delay: TimeSpan.FromSeconds(1));
        return meter is null
            ? null
            : new PipelineConsoleProgressRenderer(totalSteps, meter);
    }

    public void StartStep(int stepIndex, string stepName)
    {
        _ = stepName;
        lock (sync)
        {
            currentStepIndex = Math.Clamp(stepIndex, 1, totalSteps);
            baseRowCount = rowCount;
            baseByteCount = byteCount;
            ReportMeter();
        }
    }

    public void CompleteStep(bool succeeded, long completedRowCount = 0, int completedBatchCount = 0)
    {
        _ = completedBatchCount;
        lock (sync)
        {
            rowCount = Math.Max(rowCount, baseRowCount + Math.Max(0, completedRowCount));
            if (succeeded)
            {
                completedSteps = Math.Max(completedSteps, currentStepIndex);
            }
            else
            {
                failed = true;
            }

            ReportMeter();
        }
    }

    public void Report(MetaPipeline.BufferedPipelineExecutionProgress value)
    {
        lock (sync)
        {
            rowCount = baseRowCount + value.RowCount;
            byteCount = baseByteCount + value.EstimatedByteCount;
            ReportMeter();
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
            this.failed |= failed;
            var detail = BuildDetail();
            if (this.failed)
            {
                meter.Fail(detail);
            }
            else
            {
                meter.Succeed(detail);
            }
        }
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
            meter.Dispose();
        }
    }

    private void ReportMeter() =>
        meter.Report(completedSteps, totalSteps, BuildDetail());

    private string BuildDetail()
    {
        var rate = stopwatch.Elapsed.TotalSeconds <= 0
            ? 0
            : byteCount / stopwatch.Elapsed.TotalSeconds;
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{rowCount:N0} rows  {FormatByteRate(rate)}");
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
