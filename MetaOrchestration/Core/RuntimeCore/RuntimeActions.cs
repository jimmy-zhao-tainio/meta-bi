namespace MetaOrchestration.Core.Runtime;

internal interface IRuntimeActionHandler
{
    Task HandleAsync(RuntimeAction action, CancellationToken cancellationToken);
}

internal abstract record RuntimeAction
{
    public sealed record StartWorker(
        string WorkerName,
        string PipelineId,
        string ResumeTaskId) : RuntimeAction;

    public sealed record SendStartPipeline(
        string WorkerName,
        string PipelineId,
        string ResumeTaskId) : RuntimeAction;

    public sealed record IssueGrant(
        string WorkerName,
        string TaskId,
        string TaskName,
        RuntimeGrant Grant) : RuntimeAction;

    public sealed record SendStopPipeline(
        string PipelineName,
        string PipelineId,
        string BlockingTaskId,
        string Reason) : RuntimeAction;

    public sealed record MarkPipelineFailed(
        string PipelineName,
        string PipelineId,
        string TaskId,
        string FailureClass,
        string Reason) : RuntimeAction;

    public sealed record ScheduleRetry(
        string TaskId,
        string WorkerName,
        int AttemptNumber,
        DateTimeOffset DueAtUtc,
        string PreviousGrantId) : RuntimeAction;

    public sealed record RecordTaskCompletion(RuntimeTaskCompletion Completion) : RuntimeAction;

    public sealed record RecordBlockedTasks(
        string PipelineName,
        string PipelineId,
        string BlockingTaskId,
        string Reason,
        IReadOnlyList<RuntimeBlockedTask> BlockedTasks) : RuntimeAction;

    public sealed record RecordPipelineCompletion(string PipelineName) : RuntimeAction;

    public sealed record WriteJournalEntry(
        string EventKind,
        string Subject,
        string Detail) : RuntimeAction;

    public sealed record NotifyObserver(
        string EventKind,
        string Subject) : RuntimeAction;

    public sealed record PublishSnapshot(RuntimeSnapshot Snapshot) : RuntimeAction;
}

internal sealed record KernelResult(
    IReadOnlyList<RuntimeAction> Actions,
    RuntimeSnapshot Snapshot);
