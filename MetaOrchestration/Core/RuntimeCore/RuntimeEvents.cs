namespace MetaOrchestration.Core.Runtime;

internal interface IRuntimeEventSink
{
    RuntimeSnapshot Snapshot { get; }

    KernelResult RegisterEvent(RuntimeEvent @event);
}

internal abstract record RuntimeEvent
{
    public sealed record SchedulerTick(DateTimeOffset Now, int MaxActiveWorkerProcesses) : RuntimeEvent;

    public sealed record WorkerOnline(
        string WorkerName,
        string PipelineId,
        string ExecutableVersion) : RuntimeEvent;

    public sealed record WorkerReady(string WorkerName) : RuntimeEvent;

    public sealed record StartPipelineAcknowledged(string WorkerName) : RuntimeEvent;

    public sealed record PipelineStarted(string WorkerName) : RuntimeEvent;

    public sealed record TaskReady(
        string WorkerName,
        string TaskId,
        string TaskName) : RuntimeEvent;

    public sealed record GrantAccepted(
        string WorkerName,
        string TaskId,
        string GrantId,
        string CommandId,
        int AttemptNumber) : RuntimeEvent;

    public sealed record GrantDeliveryFailed(
        string WorkerName,
        string TaskId,
        string GrantId,
        string CommandId,
        int AttemptNumber,
        string Reason) : RuntimeEvent;

    public sealed record TaskStarted(
        string WorkerName,
        string TaskId,
        string GrantId,
        string CommandId,
        int AttemptNumber) : RuntimeEvent;

    public sealed record TaskSucceeded(
        string WorkerName,
        string TaskId,
        string GrantId,
        string CommandId,
        int AttemptNumber,
        int ExitCode) : RuntimeEvent;

    public sealed record TaskFailed(
        string WorkerName,
        string TaskId,
        string GrantId,
        string CommandId,
        int AttemptNumber,
        int ExitCode,
        string FailureClass,
        string Reason) : RuntimeEvent;

    public sealed record WorkerClosed(
        string WorkerName,
        int ExitCode,
        string Reason) : RuntimeEvent;

    public sealed record WorkerTimedOut(
        string WorkerName,
        string FailureClass,
        string Reason) : RuntimeEvent;

    public sealed record SupervisorFailureObserved(
        string WorkerName,
        string TaskId,
        int ExitCode,
        string FailureClass,
        string Reason) : RuntimeEvent;

    public sealed record SupervisorStopRequested(string Reason) : RuntimeEvent;

    public sealed record PipelineStopRequested(
        string PipelineName,
        string Reason) : RuntimeEvent;
}
