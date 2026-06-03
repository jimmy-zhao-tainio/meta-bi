using System.Globalization;

namespace MetaOrchestration.Core.Runtime;

internal enum TaskRuntimeState
{
    None,
    Pending,
    Ready,
    GrantIssued,
    GrantAccepted,
    Running,
    Succeeded,
    Failed,
    Blocked,
    RetryScheduled
}

internal enum WorkerRuntimeState
{
    NotStarted,
    Starting,
    Online,
    Ready,
    StartPipelineSent,
    PipelineStarted,
    Closed
}

internal enum GrantRuntimeState
{
    None,
    Issued,
    Accepted,
    Running,
    Completed,
    Failed,
    Released
}

internal enum ExecutionTrigger
{
    WorkerOnline,
    WorkerReady,
    StartPipelineAcknowledged,
    PipelineStarted,
    TaskReady,
    GrantIssued,
    GrantDeliveryFailed,
    GrantAccepted,
    TaskStarted,
    TaskSucceeded,
    TaskFailed,
    SameWorkerRetryScheduled,
    SupervisorFailure,
    ReplacementRetryScheduled,
    ReplacementWorkerStarted,
    RetryDue,
    TaskBlocked,
    ReadyWorkerLost,
    WorkerClosed
}

internal readonly record struct ExecutionState(
    TaskRuntimeState Task,
    WorkerRuntimeState Worker,
    GrantRuntimeState Grant);

internal readonly record struct ExecutionFacts(
    string TaskId,
    string WorkerName,
    string GrantId,
    string CommandId,
    int AttemptNumber);

internal readonly record struct ExecutionTransitionResult(
    ExecutionState State,
    ExecutionTrigger Trigger);

internal sealed class ExecutionStateReducer
{
    private static readonly ExecutionTransition[] Transitions =
    [
        new(
            new ExecutionState(TaskRuntimeState.None, WorkerRuntimeState.Starting, GrantRuntimeState.None),
            ExecutionTrigger.WorkerOnline,
            new ExecutionState(TaskRuntimeState.None, WorkerRuntimeState.Online, GrantRuntimeState.None)),
        new(
            new ExecutionState(TaskRuntimeState.None, WorkerRuntimeState.Online, GrantRuntimeState.None),
            ExecutionTrigger.WorkerReady,
            new ExecutionState(TaskRuntimeState.None, WorkerRuntimeState.Ready, GrantRuntimeState.None)),
        new(
            new ExecutionState(TaskRuntimeState.None, WorkerRuntimeState.Ready, GrantRuntimeState.None),
            ExecutionTrigger.StartPipelineAcknowledged,
            new ExecutionState(TaskRuntimeState.None, WorkerRuntimeState.StartPipelineSent, GrantRuntimeState.None)),
        new(
            new ExecutionState(TaskRuntimeState.None, WorkerRuntimeState.StartPipelineSent, GrantRuntimeState.None),
            ExecutionTrigger.PipelineStarted,
            new ExecutionState(TaskRuntimeState.None, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None)),
        new(
            new ExecutionState(TaskRuntimeState.Pending, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None),
            ExecutionTrigger.TaskReady,
            new ExecutionState(TaskRuntimeState.Ready, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None)),
        new(
            new ExecutionState(TaskRuntimeState.Ready, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None),
            ExecutionTrigger.GrantIssued,
            new ExecutionState(TaskRuntimeState.GrantIssued, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Issued)),
        new(
            new ExecutionState(TaskRuntimeState.GrantIssued, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Issued),
            ExecutionTrigger.GrantDeliveryFailed,
            new ExecutionState(TaskRuntimeState.Pending, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Released)),
        new(
            new ExecutionState(TaskRuntimeState.GrantIssued, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Issued),
            ExecutionTrigger.GrantAccepted,
            new ExecutionState(TaskRuntimeState.GrantAccepted, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Accepted)),
        new(
            new ExecutionState(TaskRuntimeState.GrantIssued, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Issued),
            ExecutionTrigger.TaskStarted,
            new ExecutionState(TaskRuntimeState.Running, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Running)),
        new(
            new ExecutionState(TaskRuntimeState.GrantAccepted, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Accepted),
            ExecutionTrigger.TaskStarted,
            new ExecutionState(TaskRuntimeState.Running, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Running)),
        new(
            new ExecutionState(TaskRuntimeState.Running, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Running),
            ExecutionTrigger.TaskSucceeded,
            new ExecutionState(TaskRuntimeState.Succeeded, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Completed)),
        new(
            new ExecutionState(TaskRuntimeState.Running, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Running),
            ExecutionTrigger.TaskFailed,
            new ExecutionState(TaskRuntimeState.Failed, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Failed)),
        new(
            new ExecutionState(TaskRuntimeState.Running, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Running),
            ExecutionTrigger.SameWorkerRetryScheduled,
            new ExecutionState(TaskRuntimeState.Ready, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Released)),
        new(
            new ExecutionState(TaskRuntimeState.GrantIssued, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Issued),
            ExecutionTrigger.SupervisorFailure,
            new ExecutionState(TaskRuntimeState.Failed, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Failed)),
        new(
            new ExecutionState(TaskRuntimeState.GrantAccepted, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Accepted),
            ExecutionTrigger.SupervisorFailure,
            new ExecutionState(TaskRuntimeState.Failed, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Failed)),
        new(
            new ExecutionState(TaskRuntimeState.Running, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Running),
            ExecutionTrigger.SupervisorFailure,
            new ExecutionState(TaskRuntimeState.Failed, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Failed)),
        new(
            new ExecutionState(TaskRuntimeState.Running, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Running),
            ExecutionTrigger.ReplacementRetryScheduled,
            new ExecutionState(TaskRuntimeState.RetryScheduled, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Released)),
        new(
            new ExecutionState(TaskRuntimeState.RetryScheduled, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Released),
            ExecutionTrigger.ReplacementWorkerStarted,
            new ExecutionState(TaskRuntimeState.Pending, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None)),
        new(
            new ExecutionState(TaskRuntimeState.RetryScheduled, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.Released),
            ExecutionTrigger.RetryDue,
            new ExecutionState(TaskRuntimeState.Ready, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None)),
        new(
            new ExecutionState(TaskRuntimeState.Pending, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None),
            ExecutionTrigger.TaskBlocked,
            new ExecutionState(TaskRuntimeState.Blocked, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None)),
        new(
            new ExecutionState(TaskRuntimeState.Ready, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None),
            ExecutionTrigger.TaskBlocked,
            new ExecutionState(TaskRuntimeState.Blocked, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None)),
        new(
            new ExecutionState(TaskRuntimeState.Ready, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None),
            ExecutionTrigger.ReadyWorkerLost,
            new ExecutionState(TaskRuntimeState.Pending, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None)),
        new(
            new ExecutionState(TaskRuntimeState.Pending, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None),
            ExecutionTrigger.ReadyWorkerLost,
            new ExecutionState(TaskRuntimeState.Pending, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None))
    ];

    public ExecutionTransitionResult Apply(
        ExecutionState state,
        ExecutionTrigger trigger,
        ExecutionFacts facts)
    {
        _ = facts;
        if (trigger == ExecutionTrigger.WorkerClosed)
        {
            if (state.Worker == WorkerRuntimeState.Closed)
            {
                return new ExecutionTransitionResult(state, trigger);
            }

            return new ExecutionTransitionResult(state with { Worker = WorkerRuntimeState.Closed }, trigger);
        }

        var matches = Transitions
            .Where(item => item.From == state && item.Trigger == trigger)
            .ToArray();
        if (matches.Length == 1)
        {
            return new ExecutionTransitionResult(matches[0].To, trigger);
        }

        if (matches.Length > 1)
        {
            throw new InvalidOperationException(
                $"Ambiguous execution transition: {state} + {trigger} has {matches.Length.ToString(CultureInfo.InvariantCulture)} definitions.");
        }

        throw new InvalidOperationException(
            $"Illegal execution transition: {state.Task} + {trigger}; {state.Worker} + {trigger}; {state.Grant} + {trigger}. " +
            $"State: {state}.");
    }
}

internal readonly record struct ExecutionTransition(
    ExecutionState From,
    ExecutionTrigger Trigger,
    ExecutionState To);
