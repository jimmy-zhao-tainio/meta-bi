using MetaCli.Core;
using MetaOrchestration.Core;

internal sealed class OrchestrationExecutionProgressRenderer : IDisposable
{
    private readonly object sync = new();
    private readonly MetaCliProgressMeter meter;
    private readonly HashSet<string> completedPipelineNames = new(StringComparer.OrdinalIgnoreCase);
    private readonly int maxParallelism;
    private string currentPhase = "starting";
    private int totalPipelines;
    private int liveWorkers;
    private bool disposed;

    private OrchestrationExecutionProgressRenderer(
        int maxParallelism,
        MetaCliProgressMeter meter)
    {
        this.maxParallelism = Math.Max(1, maxParallelism);
        this.meter = meter;
        meter.Report(0, 0, currentPhase);
    }

    public static OrchestrationExecutionProgressRenderer? TryCreate(int maxParallelism)
    {
        var meter = MetaCliProgressMeter.TryStart(initialDetail: "starting");
        return meter is null
            ? null
            : new OrchestrationExecutionProgressRenderer(maxParallelism, meter);
    }

    public void SetPhase(string phase)
    {
        lock (sync)
        {
            currentPhase = string.IsNullOrWhiteSpace(phase)
                ? "working"
                : phase.Trim().ToLowerInvariant();
            ReportMeter();
        }
    }

    public void RunPlanReady(int totalPipelines)
    {
        lock (sync)
        {
            this.totalPipelines = Math.Max(totalPipelines, 1);
            ReportMeter();
        }
    }

    public void RuntimeStateChanged(OrchestrationRuntimeProgressSnapshot snapshot)
    {
        lock (sync)
        {
            liveWorkers = Math.Max(0, snapshot.LiveWorkerCount);
            ReportMeter();
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
            if (failed)
            {
                meter.Fail(currentPhase);
            }
            else
            {
                meter.Succeed();
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

    private void ReportMeter()
    {
        var detail = totalPipelines <= 0
            ? currentPhase
            : $"{Math.Min(liveWorkers, maxParallelism)}/{maxParallelism} running";
        meter.Report(completedPipelineNames.Count, totalPipelines, detail);
    }
}
