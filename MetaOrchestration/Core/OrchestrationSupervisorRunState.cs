using System.Globalization;

namespace MetaOrchestration.Core;

internal sealed class OrchestrationSupervisorRunState
{
    private readonly object gate = new();
    private readonly Guid runId;
    private readonly string workspacePath;
    private string phase = "Starting";
    private string runPlanName = string.Empty;
    private int plannedTasks;
    private int pendingTasks;
    private int readyTasks;
    private int runningTasks;
    private int retryScheduledTasks;
    private int completedTasks;
    private int blockedTasks;
    private string liveWorkers = string.Empty;
    private string lastEvent = string.Empty;
    private DateTimeOffset updatedAtUtc = DateTimeOffset.UtcNow;

    public OrchestrationSupervisorRunState(Guid runId, string workspacePath)
    {
        this.runId = runId;
        this.workspacePath = workspacePath;
    }

    public void SetPhase(string value)
    {
        lock (gate)
        {
            phase = string.IsNullOrWhiteSpace(value) ? phase : value.Trim();
            updatedAtUtc = DateTimeOffset.UtcNow;
        }
    }

    public void SetRunPlan(string name, int taskCount)
    {
        lock (gate)
        {
            runPlanName = name;
            plannedTasks = taskCount;
            updatedAtUtc = DateTimeOffset.UtcNow;
        }
    }

    public void SetRuntimeCounts(
        int pending,
        int ready,
        int running,
        int retryScheduled,
        int completed,
        int blocked)
    {
        lock (gate)
        {
            pendingTasks = pending;
            readyTasks = ready;
            runningTasks = running;
            retryScheduledTasks = retryScheduled;
            completedTasks = completed;
            blockedTasks = blocked;
            updatedAtUtc = DateTimeOffset.UtcNow;
        }
    }

    public void SetLiveWorkers(IEnumerable<string> workerNames)
    {
        lock (gate)
        {
            liveWorkers = string.Join(
                ",",
                workerNames
                    .Where(static item => !string.IsNullOrWhiteSpace(item))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
                    .Take(12));
            updatedAtUtc = DateTimeOffset.UtcNow;
        }
    }

    public void NoteEvent(string eventKind, string subject)
    {
        lock (gate)
        {
            lastEvent = string.IsNullOrWhiteSpace(subject)
                ? eventKind
                : $"{eventKind}:{subject}";
            updatedAtUtc = DateTimeOffset.UtcNow;
        }
    }

    public string Describe()
    {
        lock (gate)
        {
            return string.Join(
                "; ",
                $"RunId={runId:N}",
                $"Phase={phase}",
                $"Workspace={workspacePath}",
                string.IsNullOrWhiteSpace(runPlanName) ? "RunPlan=<unknown>" : $"RunPlan={runPlanName}",
                $"Planned={plannedTasks.ToString(CultureInfo.InvariantCulture)}",
                $"Pending={pendingTasks.ToString(CultureInfo.InvariantCulture)}",
                $"Ready={readyTasks.ToString(CultureInfo.InvariantCulture)}",
                $"Running={runningTasks.ToString(CultureInfo.InvariantCulture)}",
                $"RetryScheduled={retryScheduledTasks.ToString(CultureInfo.InvariantCulture)}",
                $"Completed={completedTasks.ToString(CultureInfo.InvariantCulture)}",
                $"Blocked={blockedTasks.ToString(CultureInfo.InvariantCulture)}",
                string.IsNullOrWhiteSpace(liveWorkers) ? "LiveWorkers=<none>" : $"LiveWorkers={liveWorkers}",
                string.IsNullOrWhiteSpace(lastEvent) ? "LastEvent=<none>" : $"LastEvent={lastEvent}",
                $"UpdatedAtUtc={updatedAtUtc:O}");
        }
    }
}
