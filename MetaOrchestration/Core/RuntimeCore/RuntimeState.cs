using System.Globalization;
using MetaOrchestration.Core;

namespace MetaOrchestration.Core.Runtime;

internal enum RetryRuntimeState
{
    Scheduled,
    Due,
    Consumed,
    Cancelled
}

internal enum LockRuntimeState
{
    Acquired,
    Released
}

internal sealed record RuntimeRetryPolicy(
    int MaxAttempts,
    TimeSpan Delay,
    bool RetryReadOnlyTasksByDefault,
    bool RetryWriteTasksByDefault,
    IReadOnlySet<string> RetryableFailureClasses)
{
    public RuntimeRetryPolicy(int MaxAttempts, TimeSpan Delay)
        : this(MaxAttempts, Delay, RetryReadOnlyTasksByDefault: true, RetryWriteTasksByDefault: true, new HashSet<string>(StringComparer.OrdinalIgnoreCase))
    {
    }

    public static RuntimeRetryPolicy NoRetry { get; } = new(
        1,
        TimeSpan.Zero,
        RetryReadOnlyTasksByDefault: false,
        RetryWriteTasksByDefault: false,
        new HashSet<string>(StringComparer.OrdinalIgnoreCase));

    public bool ShouldRetry(int attemptNumber) =>
        attemptNumber > 0 && attemptNumber < MaxAttempts;

    public RuntimeRetryDecision Evaluate(
        int attemptNumber,
        string failureClass,
        bool hasWriteEffect)
    {
        if (MaxAttempts <= 1 || attemptNumber >= MaxAttempts)
        {
            return RuntimeRetryDecision.DoNotRetry("retry budget exhausted");
        }

        if (RetryableFailureClasses.Count > 0 && !RetryableFailureClasses.Contains(failureClass))
        {
            return RuntimeRetryDecision.DoNotRetry($"failure class '{failureClass}' is not retryable");
        }

        var isRetrySafe = hasWriteEffect ? RetryWriteTasksByDefault : RetryReadOnlyTasksByDefault;
        if (!isRetrySafe)
        {
            return RuntimeRetryDecision.DoNotRetry("task is not retry-safe");
        }

        return RuntimeRetryDecision.Retry(attemptNumber + 1, Delay, $"failure class '{failureClass}' is retryable");
    }
}

internal sealed record RuntimeRetryDecision(
    bool ShouldRetry,
    int NextAttemptNumber,
    TimeSpan Delay,
    string Reason)
{
    public static RuntimeRetryDecision Retry(int nextAttemptNumber, TimeSpan delay, string reason) =>
        new(true, nextAttemptNumber, delay, reason);

    public static RuntimeRetryDecision DoNotRetry(string reason) =>
        new(false, 0, TimeSpan.Zero, reason);
}

internal sealed record RuntimeDefinition(
    IReadOnlyList<RuntimePipelineDefinition> Pipelines,
    RuntimeRetryPolicy RetryPolicy,
    IReadOnlyList<RuntimeDependency> Dependencies,
    IReadOnlyList<RuntimeLockCompatibilityPolicy> LockCompatibilityPolicies,
    string ExpectedWorkerExecutableVersion = "")
{
    public RuntimeDefinition(
        IReadOnlyList<RuntimePipelineDefinition> Pipelines,
        RuntimeRetryPolicy RetryPolicy)
        : this(Pipelines, RetryPolicy, [], [], string.Empty)
    {
    }

    public RuntimeTaskDefinition RequireTask(string taskId)
    {
        var task = Pipelines
            .SelectMany(static item => item.Tasks)
            .FirstOrDefault(item => string.Equals(item.TaskId, taskId, StringComparison.Ordinal));
        return task ?? throw new InvalidOperationException($"Task '{taskId}' is not part of the runtime definition.");
    }

    public RuntimePipelineDefinition RequirePipeline(string pipelineName)
    {
        var pipeline = Pipelines.FirstOrDefault(item =>
            string.Equals(item.PipelineName, pipelineName, StringComparison.OrdinalIgnoreCase));
        return pipeline ?? throw new InvalidOperationException($"Pipeline '{pipelineName}' is not part of the runtime definition.");
    }
}

internal sealed record RuntimePipelineDefinition(
    string PipelineName,
    string PipelineId,
    IReadOnlyList<RuntimeTaskDefinition> Tasks);

internal sealed record RuntimeTaskDefinition(
    string TaskId,
    string TaskName,
    string PipelineName,
    string PipelineId,
    string PlannedTaskId,
    string TaskAccessProfileId,
    IReadOnlyList<RuntimeLockRequest> LockRequests)
{
    public RuntimeTaskDefinition(
        string taskId,
        string taskName,
        string pipelineName,
        string pipelineId)
        : this(taskId, taskName, pipelineName, pipelineId, taskId, taskId, [])
    {
    }
}

internal sealed record RuntimeLockRequest(
    string ResourceId,
    string Mode,
    string PlannedTaskId = "");

internal sealed record RuntimeLockCompatibilityPolicy(
    string LeftMode,
    string RightMode);

internal sealed record RuntimeDependency(
    string TaskId,
    string TaskAccessProfileId,
    string PredecessorTaskAccessProfileId,
    string PredecessorPipelineName,
    string PredecessorStepName,
    string Condition,
    string BlockedOutcome,
    string BlockedReason);

internal sealed record RuntimeGrant(
    string GrantId,
    string CommandId,
    string PipelineId,
    string TaskId,
    string WorkerName,
    int AttemptNumber,
    string PreviousGrantId);

internal sealed record RuntimeReadyWork(
    string TaskId,
    string TaskName,
    string WorkerName,
    string PipelineName,
    string PipelineId,
    int AttemptNumber,
    string PreviousGrantId,
    DateTimeOffset NotBeforeUtc);

internal sealed record RuntimeRunningGrant(
    RuntimeGrant Grant,
    string TaskName,
    string PipelineName,
    string PlannedTaskId,
    string TaskAccessProfileId,
    GrantRuntimeState State);

internal sealed record RuntimeRetryEntry(
    string TaskId,
    string WorkerName,
    int AttemptNumber,
    DateTimeOffset DueAtUtc,
    string PreviousGrantId,
    RetryRuntimeState State);

internal sealed record TaskRuntimeSnapshot(
    string TaskId,
    string TaskName,
    string PipelineName,
    TaskRuntimeState State);

internal sealed record WorkerRuntimeSnapshot(
    string WorkerName,
    string PipelineId,
    string ResumeTaskId,
    WorkerRuntimeState State);

internal sealed record PipelineActivationSnapshot(
    string PipelineName,
    string PipelineId,
    PipelineActivationState State);

internal sealed record GrantRuntimeSnapshot(
    string GrantId,
    string CommandId,
    string TaskId,
    string WorkerName,
    int AttemptNumber,
    GrantRuntimeState State);

internal sealed record RetryRuntimeSnapshot(
    string TaskId,
    string WorkerName,
    int AttemptNumber,
    DateTimeOffset DueAtUtc,
    string PreviousGrantId,
    RetryRuntimeState State);

internal sealed record LockRuntimeSnapshot(
    string GrantId,
    string ResourceId,
    string Mode,
    LockRuntimeState State);

internal sealed record OutcomeRuntimeSnapshot(
    string TaskId,
    string TaskAccessProfileId,
    string Outcome,
    int ExitCode);

internal sealed record RuntimeSnapshot(
    int PendingCount,
    int ReadyCount,
    int RunningGrantCount,
    int RetryCount,
    int LockCount,
    IReadOnlyList<TaskRuntimeSnapshot> Tasks,
    IReadOnlyList<WorkerRuntimeSnapshot> Workers,
    IReadOnlyList<PipelineActivationSnapshot> Pipelines,
    IReadOnlyList<GrantRuntimeSnapshot> RunningGrants,
    IReadOnlyList<RetryRuntimeSnapshot> Retries,
    IReadOnlyList<LockRuntimeSnapshot> Locks,
    IReadOnlyList<OutcomeRuntimeSnapshot> Outcomes);

internal sealed record RuntimeTaskCompletion(
    string TaskId,
    string TaskAccessProfileId,
    string PlannedTaskId,
    string PipelineName,
    string StepName,
    int ExitCode,
    string GrantId,
    string CommandId,
    int AttemptNumber,
    bool RecordTerminalOutcome,
    string FailureMessage,
    string JournalEventKind);

internal sealed record RuntimeBlockedTask(
    string PlannedTaskId,
    string TaskAccessProfileId,
    string PipelineName,
    string StepName,
    string BlockingTaskAccessProfileId,
    string BlockingPipelineName,
    string BlockingStepName,
    string DependencyCondition,
    string Outcome,
    string Reason);

internal sealed record RuntimePreWorkReplacementReservation(
    string PipelineName,
    string ResumeTaskId,
    int Attempt,
    int Limit);

internal sealed class RuntimeState
{
    private int nextGrantSequence = 1;
    private readonly Dictionary<string, RuntimeRetryEntry> replacementRetryByTaskId = new(StringComparer.Ordinal);

    public RuntimeState(RuntimeDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        if (definition.Pipelines.Count == 0)
        {
            throw new ArgumentException("At least one pipeline is required.", nameof(definition));
        }

        Definition = definition;
        var tasks = definition.Pipelines.SelectMany(static item => item.Tasks).ToArray();
        if (tasks.Length == 0)
        {
            throw new ArgumentException("At least one task is required.", nameof(definition));
        }

        var duplicateTaskId = tasks
            .GroupBy(static item => item.TaskId, StringComparer.Ordinal)
            .FirstOrDefault(static item => item.Count() > 1);
        if (duplicateTaskId is not null)
        {
            throw new ArgumentException($"Duplicate task id '{duplicateTaskId.Key}'.", nameof(definition));
        }

        TaskLifecycles = new TaskLifecycles(tasks);
        WorkerRegistry = new WorkerRegistry();
        PipelineActivations = new PipelineActivations(definition.Pipelines);
        PendingTasks = new PendingTasks(tasks.Select(static item => item.TaskId));
        ReadyQueue = new ReadyQueue();
        RunningGrants = new RunningGrants();
        RuntimeLocks = new RuntimeLocks(definition.LockCompatibilityPolicies);
        RetrySchedule = new RetrySchedule();
        PipelineOutcomes = new PipelineOutcomes();
        StoppedPipelines = new StoppedPipelines();
        WorkerReplacementAttempts = new WorkerReplacementAttempts();
        CapacityDeferrals = new CapacityDeferrals();
    }

    internal RuntimeDefinition Definition { get; }

    internal TaskLifecycles TaskLifecycles { get; }

    internal WorkerRegistry WorkerRegistry { get; }

    internal PipelineActivations PipelineActivations { get; }

    internal PendingTasks PendingTasks { get; }

    internal ReadyQueue ReadyQueue { get; }

    internal RunningGrants RunningGrants { get; }

    internal RuntimeLocks RuntimeLocks { get; }

    internal RetrySchedule RetrySchedule { get; }

    internal PipelineOutcomes PipelineOutcomes { get; }

    internal StoppedPipelines StoppedPipelines { get; }

    internal WorkerReplacementAttempts WorkerReplacementAttempts { get; }

    internal CapacityDeferrals CapacityDeferrals { get; }

    public RuntimeSnapshot CreateSnapshot() =>
        new(
            PendingTasks.Count,
            ReadyQueue.Count,
            RunningGrants.Count,
            RetrySchedule.Count,
            RuntimeLocks.Count,
            TaskLifecycles.CreateSnapshot(),
            WorkerRegistry.CreateSnapshot(),
            PipelineActivations.CreateSnapshot(),
            RunningGrants.CreateSnapshot(),
            RetrySchedule.CreateSnapshot(),
            RuntimeLocks.CreateSnapshot(),
            PipelineOutcomes.CreateSnapshot());

    internal RuntimePipelineDefinition? SelectStartablePipeline(int maxActiveWorkerProcesses)
    {
        if (WorkerRegistry.LiveCount >= Math.Max(1, maxActiveWorkerProcesses))
        {
            return null;
        }

        return SelectStartablePipelineIgnoringCapacity() ?? SelectWaitingPipeline();
    }

    internal RuntimePipelineDefinition? SelectStartablePipelineIgnoringCapacity()
    {
        return Definition.Pipelines
            .Where(item => !StoppedPipelines.Contains(item.PipelineName))
            .Where(item => PipelineActivations.GetState(item.PipelineName) is PipelineActivationState.Inactive or PipelineActivationState.Parked)
            .FirstOrDefault(PipelineCanMakeProgress);
    }

    private RuntimePipelineDefinition? SelectWaitingPipeline() =>
        Definition.Pipelines
            .Where(item => !StoppedPipelines.Contains(item.PipelineName))
            .Where(item => PipelineActivations.GetState(item.PipelineName) is PipelineActivationState.Inactive or PipelineActivationState.Parked)
            .FirstOrDefault(PipelineHasBoundaryWork);

    internal void RequestWorkerStart(
        RuntimePipelineDefinition pipeline,
        ActivationTransitionResult transition,
        string resumeTaskId = "")
    {
        if (transition.State != PipelineActivationState.StartRequested ||
            transition.Effect != ActivationTransitionEffect.StartWorker)
        {
            throw new InvalidOperationException(
                $"Activation transition {transition.Trigger} for pipeline '{pipeline.PipelineName}' did not request a worker start.");
        }

        EnsurePipelineCanReceiveWork(pipeline.PipelineName);
        PipelineActivations.ApplyTransition(pipeline.PipelineName, transition.State);
        WorkerRegistry.RequestStart(pipeline.PipelineName, pipeline.PipelineId, resumeTaskId);
    }

    internal string ConsumeParkedResumeTask(string pipelineName) =>
        CapacityDeferrals.ConsumeParkedResumeTask(pipelineName);

    internal void ActivateWorker(
        string workerName,
        string pipelineId,
        ExecutionTransitionResult workerTransition,
        ActivationTransitionResult activationTransition)
    {
        var pipeline = Definition.RequirePipeline(workerName);
        if (!string.Equals(pipeline.PipelineId, pipelineId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{workerName}' reported pipeline id '{pipelineId}', expected '{pipeline.PipelineId}'.");
        }

        WorkerRegistry.ApplyLifecycleTransition(workerName, workerTransition.State.Worker);
        PipelineActivations.ApplyTransition(workerName, activationTransition.State);
    }

    internal void ApplyWorkerLifecycle(
        string workerName,
        ExecutionTransitionResult transition) =>
        WorkerRegistry.ApplyLifecycleTransition(workerName, transition.State.Worker);

    internal void ApplyPipelineActivation(
        string pipelineName,
        ActivationTransitionResult transition) =>
        PipelineActivations.ApplyTransition(pipelineName, transition.State);

    internal void AcceptTaskReady(
        RuntimeTaskDefinition task,
        string workerName,
        ExecutionTransitionResult transition)
    {
        EnsurePipelineCanReceiveWork(task.PipelineName);
        if (transition.State.Task != TaskRuntimeState.Ready)
        {
            throw new InvalidOperationException($"Task '{task.TaskId}' transition did not produce Ready state.");
        }

        TaskLifecycles.ApplyTransition(task.TaskId, transition.State.Task);
        PendingTasks.RemoveForReady(task.TaskId);
        var attemptNumber = 1;
        var previousGrantId = string.Empty;
        if (replacementRetryByTaskId.Remove(task.TaskId, out var replacementRetry))
        {
            attemptNumber = replacementRetry.AttemptNumber;
            previousGrantId = replacementRetry.PreviousGrantId;
        }

        ReadyQueue.MarkReady(new RuntimeReadyWork(
            task.TaskId,
            task.TaskName,
            workerName,
            task.PipelineName,
            task.PipelineId,
            AttemptNumber: attemptNumber,
            PreviousGrantId: previousGrantId,
            NotBeforeUtc: DateTimeOffset.MinValue));
        AssertTaskHasSingleRuntimeLocation(task.TaskId);
    }

    internal RuntimeGrant CreateGrant(RuntimeReadyWork ready)
    {
        var sequence = nextGrantSequence++;
        return new RuntimeGrant(
            $"grant-{sequence.ToString(CultureInfo.InvariantCulture)}",
            $"command-{sequence.ToString(CultureInfo.InvariantCulture)}",
            ready.PipelineId,
            ready.TaskId,
            ready.WorkerName,
            ready.AttemptNumber <= 0 ? 1 : ready.AttemptNumber,
            ready.PreviousGrantId);
    }

    internal RuntimeRunningGrant IssueGrantFromReady(
        RuntimeReadyWork ready,
        RuntimeTaskDefinition task,
        RuntimeGrant grant,
        ExecutionTransitionResult transition)
    {
        EnsurePipelineCanReceiveWork(task.PipelineName);
        if (transition.State.Task != TaskRuntimeState.GrantIssued ||
            transition.State.Grant != GrantRuntimeState.Issued)
        {
            throw new InvalidOperationException($"Task '{task.TaskId}' transition did not produce a grant-issued state.");
        }

        if (!RuntimeLocks.CanAcquire(task.LockRequests))
        {
            throw new InvalidOperationException($"Locks for task '{task.TaskId}' are not compatible with currently running grants.");
        }

        var removedReady = ReadyQueue.RemoveForGrant(ready.TaskId);
        if (!string.Equals(removedReady.WorkerName, ready.WorkerName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Ready task '{ready.TaskId}' was owned by '{removedReady.WorkerName}', not '{ready.WorkerName}'.");
        }

        TaskLifecycles.ApplyTransition(task.TaskId, transition.State.Task);
        RuntimeLocks.AcquireForGrant(grant.GrantId, task.LockRequests);
        var running = RunningGrants.IssueGrant(
            grant,
            task.TaskName,
            task.PipelineName,
            task.PlannedTaskId,
            task.TaskAccessProfileId);
        AssertTaskHasSingleRuntimeLocation(task.TaskId);
        return running;
    }

    internal void AcceptGrant(
        RuntimeGrant grant,
        ExecutionTransitionResult transition)
    {
        TaskLifecycles.ApplyTransition(grant.TaskId, transition.State.Task);
        RunningGrants.AcceptGrant(grant.TaskId, grant.GrantId);
        AssertTaskHasSingleRuntimeLocation(grant.TaskId);
    }

    internal void StartGrant(
        RuntimeGrant grant,
        ExecutionTransitionResult transition)
    {
        TaskLifecycles.ApplyTransition(grant.TaskId, transition.State.Task);
        RunningGrants.StartGrant(grant.TaskId, grant.GrantId);
        AssertTaskHasSingleRuntimeLocation(grant.TaskId);
    }

    internal RuntimeTaskCompletion CompleteGrantSucceeded(
        RuntimeGrant grant,
        RuntimeTaskDefinition task,
        int exitCode,
        ExecutionTransitionResult transition)
    {
        TaskLifecycles.ApplyTransition(grant.TaskId, transition.State.Task);
        RunningGrants.CompleteGrant(grant.TaskId, grant.GrantId);
        RuntimeLocks.ReleaseForGrant(grant.GrantId);
        PipelineOutcomes.RecordOutcome(grant.TaskId, task.TaskAccessProfileId, "Succeeded", exitCode);
        AssertTaskHasSingleRuntimeLocation(grant.TaskId);
        return CreateCompletion(task, grant, exitCode, recordTerminalOutcome: true, failureMessage: string.Empty, journalEventKind: "TaskSucceeded");
    }

    internal (RuntimeTaskCompletion Completion, RuntimeReadyWork Ready) CompleteGrantWithSameWorkerRetry(
        RuntimeGrant grant,
        RuntimeTaskDefinition task,
        int nextAttemptNumber,
        DateTimeOffset dueAtUtc,
        ExecutionTransitionResult transition)
    {
        if (transition.State.Task != TaskRuntimeState.Ready)
        {
            throw new InvalidOperationException($"Task '{grant.TaskId}' transition did not produce Ready retry state.");
        }

        TaskLifecycles.ApplyTransition(grant.TaskId, transition.State.Task);
        RunningGrants.FailGrant(grant.TaskId, grant.GrantId);
        RuntimeLocks.ReleaseForGrant(grant.GrantId);
        var ready = new RuntimeReadyWork(
            grant.TaskId,
            task.TaskName,
            grant.WorkerName,
            task.PipelineName,
            task.PipelineId,
            nextAttemptNumber,
            grant.GrantId,
            dueAtUtc);
        ReadyQueue.MarkReady(ready);
        AssertTaskHasSingleRuntimeLocation(grant.TaskId);
        return (CreateCompletion(task, grant, exitCode: 4, recordTerminalOutcome: false, failureMessage: string.Empty, journalEventKind: "TaskAttemptFailed"), ready);
    }

    internal (RuntimeTaskCompletion Completion, RuntimeRetryEntry Retry) CompleteGrantWithReplacementRetry(
        RuntimeGrant grant,
        RuntimeTaskDefinition task,
        int exitCode,
        int nextAttemptNumber,
        DateTimeOffset dueAtUtc,
        ExecutionTransitionResult transition,
        string failureMessage)
    {
        if (transition.State.Task != TaskRuntimeState.RetryScheduled)
        {
            throw new InvalidOperationException($"Task '{grant.TaskId}' transition did not produce RetryScheduled state.");
        }

        TaskLifecycles.ApplyTransition(grant.TaskId, transition.State.Task);
        RunningGrants.FailGrant(grant.TaskId, grant.GrantId);
        RuntimeLocks.ReleaseForGrant(grant.GrantId);
        var retry = RetrySchedule.ScheduleReplacementRetry(
            grant.TaskId,
            grant.WorkerName,
            nextAttemptNumber,
            dueAtUtc,
            grant.GrantId);
        AssertTaskHasSingleRuntimeLocation(grant.TaskId);
        return (CreateCompletion(task, grant, exitCode, recordTerminalOutcome: false, failureMessage, journalEventKind: "TaskAttemptFailed"), retry);
    }

    internal RuntimeTaskCompletion CompleteGrantFailed(
        RuntimeGrant grant,
        RuntimeTaskDefinition task,
        int exitCode,
        ExecutionTransitionResult transition,
        string failureMessage)
    {
        TaskLifecycles.ApplyTransition(grant.TaskId, transition.State.Task);
        RunningGrants.FailGrant(grant.TaskId, grant.GrantId);
        RuntimeLocks.ReleaseForGrant(grant.GrantId);
        PipelineOutcomes.RecordOutcome(grant.TaskId, task.TaskAccessProfileId, "Failed", exitCode);
        AssertTaskHasSingleRuntimeLocation(grant.TaskId);
        return CreateCompletion(task, grant, exitCode, recordTerminalOutcome: true, failureMessage, journalEventKind: "TaskFailed");
    }

    internal RuntimeTaskCompletion FailActiveGrantAfterWorkerLoss(
        RuntimeRunningGrant runningGrant,
        RuntimeTaskDefinition task,
        int exitCode,
        ExecutionTransitionResult transition,
        string failureMessage)
    {
        TaskLifecycles.ApplyTransition(runningGrant.Grant.TaskId, transition.State.Task);
        RunningGrants.FailGrant(runningGrant.Grant.TaskId, runningGrant.Grant.GrantId);
        RuntimeLocks.ReleaseForGrant(runningGrant.Grant.GrantId);
        PipelineOutcomes.RecordOutcome(runningGrant.Grant.TaskId, task.TaskAccessProfileId, "Failed", exitCode);
        AssertTaskHasSingleRuntimeLocation(runningGrant.Grant.TaskId);
        return CreateCompletion(task, runningGrant.Grant, exitCode, recordTerminalOutcome: true, failureMessage, journalEventKind: "TaskFailed");
    }

    internal void StopPipeline(
        string pipelineName,
        ActivationTransitionResult transition)
    {
        StoppedPipelines.Stop(pipelineName);
        PipelineActivations.ApplyTransition(pipelineName, transition.State);
    }

    internal void PrepareReplacementRetry(
        RuntimeRetryEntry retry,
        ExecutionTransitionResult transition)
    {
        if (transition.State.Task != TaskRuntimeState.Pending)
        {
            throw new InvalidOperationException($"Task '{retry.TaskId}' transition did not produce Pending replacement state.");
        }

        RetrySchedule.Consume(retry.TaskId);
        replacementRetryByTaskId[retry.TaskId] = retry;
        TaskLifecycles.ApplyTransition(retry.TaskId, transition.State.Task);
        PendingTasks.AddForReplacement(retry.TaskId);
        AssertTaskHasSingleRuntimeLocation(retry.TaskId);
    }

    internal RuntimeReadyWork? FindReadyWorkByWorker(string workerName) =>
        ReadyQueue.CreateSnapshot().FirstOrDefault(item =>
            string.Equals(item.WorkerName, workerName, StringComparison.OrdinalIgnoreCase));

    internal bool PipelineHasUnresolvedWork(string pipelineName) =>
        Definition.Pipelines
            .Where(item => string.Equals(item.PipelineName, pipelineName, StringComparison.OrdinalIgnoreCase))
            .SelectMany(static item => item.Tasks)
            .Any(item =>
                TaskLifecycles.GetState(item.TaskId) is
                    TaskRuntimeState.Pending or
                    TaskRuntimeState.Ready or
                    TaskRuntimeState.GrantIssued or
                    TaskRuntimeState.GrantAccepted or
                    TaskRuntimeState.Running or
                    TaskRuntimeState.RetryScheduled);

    internal bool PipelineHasFailedTask(string pipelineName) =>
        Definition.Pipelines
            .Where(item => string.Equals(item.PipelineName, pipelineName, StringComparison.OrdinalIgnoreCase))
            .SelectMany(static item => item.Tasks)
            .Any(item => TaskLifecycles.GetState(item.TaskId) == TaskRuntimeState.Failed);

    private bool PipelineCanMakeProgress(RuntimePipelineDefinition pipeline) =>
        pipeline.Tasks
            .Where(task => PendingTasks.Contains(task.TaskId) || RetrySchedule.Contains(task.TaskId))
            .Any(TaskCanReachBoundaryNow);

    private bool PipelineHasBoundaryWork(RuntimePipelineDefinition pipeline) =>
        pipeline.Tasks.Any(task => PendingTasks.Contains(task.TaskId) || RetrySchedule.Contains(task.TaskId));

    private bool TaskCanReachBoundaryNow(RuntimeTaskDefinition task)
    {
        foreach (var dependency in Definition.Dependencies.Where(item =>
                     string.Equals(item.TaskId, task.TaskId, StringComparison.Ordinal)))
        {
            if (!PipelineOutcomes.TryGetOutcomeByTaskAccessProfileId(
                    dependency.PredecessorTaskAccessProfileId,
                    out _))
            {
                return false;
            }
        }

        return true;
    }

    internal void PrepareReadyWorkerReplacement(
        RuntimeReadyWork ready,
        ExecutionTransitionResult transition)
    {
        if (transition.State.Task != TaskRuntimeState.Pending)
        {
            throw new InvalidOperationException($"Task '{ready.TaskId}' transition did not produce Pending replacement state.");
        }

        ReadyQueue.RemoveForGrant(ready.TaskId);
        TaskLifecycles.ApplyTransition(ready.TaskId, transition.State.Task);
        PendingTasks.AddForReplacement(ready.TaskId);
        if (ready.AttemptNumber > 1 || !string.IsNullOrWhiteSpace(ready.PreviousGrantId))
        {
            replacementRetryByTaskId[ready.TaskId] = new RuntimeRetryEntry(
                ready.TaskId,
                ready.WorkerName,
                ready.AttemptNumber,
                DateTimeOffset.MinValue,
                ready.PreviousGrantId,
                RetryRuntimeState.Consumed);
        }

        AssertTaskHasSingleRuntimeLocation(ready.TaskId);
    }

    internal void RequestCapacityDeferral(
        RuntimeReadyWork ready,
        ExecutionTransitionResult taskTransition,
        ActivationTransitionResult activationTransition)
    {
        if (activationTransition.State != PipelineActivationState.CapacityDeferralRequested)
        {
            throw new InvalidOperationException(
                $"Activation transition {activationTransition.Trigger} for pipeline '{ready.PipelineName}' did not request capacity deferral.");
        }

        PrepareReadyWorkerReplacement(ready, taskTransition);
        PipelineActivations.ApplyTransition(ready.PipelineName, activationTransition.State);
        CapacityDeferrals.Request(ready.WorkerName, ready.TaskId);
    }

    internal bool TryCompleteCapacityDeferral(
        string workerName,
        ActivationTransitionResult transition,
        out string resumeTaskId)
    {
        if (!CapacityDeferrals.TryGetPendingResumeTask(workerName, out resumeTaskId))
        {
            return false;
        }

        if (transition.State != PipelineActivationState.Parked)
        {
            throw new InvalidOperationException(
                $"Activation transition {transition.Trigger} for pipeline '{workerName}' did not park a capacity-deferred worker.");
        }

        PipelineActivations.ApplyTransition(workerName, transition.State);
        CapacityDeferrals.Complete(workerName, resumeTaskId);
        return true;
    }

    internal void PrepareGrantDeliveryReplacement(
        RuntimeGrant grant,
        ExecutionTransitionResult transition)
    {
        if (transition.State.Task != TaskRuntimeState.Pending)
        {
            throw new InvalidOperationException($"Task '{grant.TaskId}' transition did not produce Pending replacement state.");
        }

        TaskLifecycles.ApplyTransition(grant.TaskId, transition.State.Task);
        RunningGrants.FailGrant(grant.TaskId, grant.GrantId);
        RuntimeLocks.ReleaseForGrant(grant.GrantId);
        PendingTasks.AddForReplacement(grant.TaskId);
        AssertTaskHasSingleRuntimeLocation(grant.TaskId);
    }

    internal RuntimePreWorkReplacementReservation ReservePreWorkReplacementAttempt(
        string pipelineName,
        string resumeTaskId,
        string reason) =>
        WorkerReplacementAttempts.Reserve(pipelineName, resumeTaskId, reason);

    internal void AssertTaskHasSingleRuntimeLocation(string taskId)
    {
        var locations = 0;
        if (PendingTasks.Contains(taskId))
        {
            locations++;
        }

        if (ReadyQueue.Contains(taskId))
        {
            locations++;
        }

        if (RunningGrants.ContainsTask(taskId))
        {
            locations++;
        }

        if (RetrySchedule.Contains(taskId))
        {
            locations++;
        }

        if (TaskLifecycles.IsTerminal(taskId))
        {
            locations++;
        }

        if (locations != 1)
        {
            throw new InvalidOperationException(
                $"Task '{taskId}' has {locations.ToString(CultureInfo.InvariantCulture)} runtime locations.");
        }
    }

    internal RuntimeBlockedTask[] BlockPipelineTasks(
        RuntimeTaskDefinition blockedTask,
        RuntimeDependency dependency,
        string reason)
    {
        var blocked = new List<RuntimeBlockedTask>();
        StoppedPipelines.Stop(blockedTask.PipelineName);
        foreach (var task in Definition.Pipelines
                     .Where(item => string.Equals(item.PipelineName, blockedTask.PipelineName, StringComparison.OrdinalIgnoreCase))
                     .SelectMany(static item => item.Tasks)
                     .Where(item => !TaskLifecycles.IsTerminal(item.TaskId))
                     .ToArray())
        {
            if (RunningGrants.ContainsTask(task.TaskId))
            {
                continue;
            }

            PendingTasks.RemoveIfPresent(task.TaskId);
            ReadyQueue.RemoveIfPresent(task.TaskId);
            RetrySchedule.RemoveIfPresent(task.TaskId);
            var currentState = TaskLifecycles.GetState(task.TaskId);
            if (currentState is TaskRuntimeState.Pending or TaskRuntimeState.Ready or TaskRuntimeState.RetryScheduled)
            {
                TaskLifecycles.ApplyTransition(task.TaskId, TaskRuntimeState.Blocked);
            }

            var isBlockingTask = string.Equals(task.TaskId, blockedTask.TaskId, StringComparison.Ordinal);
            var outcome = isBlockingTask
                ? dependency.BlockedOutcome
                : OrchestrationExecutionContinuity.SkippedBlocked;
            var taskReason = isBlockingTask
                ? reason
                : $"pipeline stopped after blocked task {blockedTask.TaskName}";
            PipelineOutcomes.RecordOutcome(task.TaskId, task.TaskAccessProfileId, outcome, exitCode: 4);
            blocked.Add(new RuntimeBlockedTask(
                task.PlannedTaskId,
                task.TaskAccessProfileId,
                task.PipelineName,
                task.TaskName,
                isBlockingTask ? dependency.PredecessorTaskAccessProfileId : string.Empty,
                isBlockingTask ? dependency.PredecessorPipelineName : string.Empty,
                isBlockingTask ? dependency.PredecessorStepName : string.Empty,
                isBlockingTask ? dependency.Condition : string.Empty,
                outcome,
                taskReason));
            AssertTaskHasSingleRuntimeLocation(task.TaskId);
        }

        return blocked.ToArray();
    }

    private void EnsurePipelineCanReceiveWork(string pipelineName)
    {
        if (StoppedPipelines.Contains(pipelineName))
        {
            throw new InvalidOperationException($"Pipeline '{pipelineName}' is stopped and cannot receive new work.");
        }
    }

    private static RuntimeTaskCompletion CreateCompletion(
        RuntimeTaskDefinition task,
        RuntimeGrant grant,
        int exitCode,
        bool recordTerminalOutcome,
        string failureMessage,
        string journalEventKind) =>
        new(
            task.TaskId,
            task.TaskAccessProfileId,
            task.PlannedTaskId,
            task.PipelineName,
            task.TaskName,
            exitCode,
            grant.GrantId,
            grant.CommandId,
            grant.AttemptNumber,
            recordTerminalOutcome,
            failureMessage,
            journalEventKind);
}

internal sealed class TaskLifecycles
{
    private readonly Dictionary<string, TaskLifecycleEntry> entries;

    public TaskLifecycles(IEnumerable<RuntimeTaskDefinition> tasks)
    {
        entries = tasks.ToDictionary(
            static item => item.TaskId,
            static item => new TaskLifecycleEntry(item, TaskRuntimeState.Pending),
            StringComparer.Ordinal);
    }

    internal TaskRuntimeState GetState(string taskId) =>
        Require(taskId).State;

    internal void ApplyTransition(string taskId, TaskRuntimeState state) =>
        Require(taskId).State = state;

    internal bool IsTerminal(string taskId) =>
        GetState(taskId) is TaskRuntimeState.Succeeded or TaskRuntimeState.Failed or TaskRuntimeState.Blocked;

    internal IReadOnlyList<TaskRuntimeSnapshot> CreateSnapshot() =>
        entries.Values
            .OrderBy(static item => item.Task.TaskId, StringComparer.Ordinal)
            .Select(static item => new TaskRuntimeSnapshot(
                item.Task.TaskId,
                item.Task.TaskName,
                item.Task.PipelineName,
                item.State))
            .ToArray();

    private TaskLifecycleEntry Require(string taskId) =>
        entries.TryGetValue(taskId, out var entry)
            ? entry
            : throw new InvalidOperationException($"Task '{taskId}' is not tracked.");

    private sealed class TaskLifecycleEntry(RuntimeTaskDefinition task, TaskRuntimeState state)
    {
        public RuntimeTaskDefinition Task { get; } = task;

        public TaskRuntimeState State { get; set; } = state;
    }
}

internal sealed class WorkerRegistry
{
    private readonly Dictionary<string, WorkerEntry> workersByName = new(StringComparer.OrdinalIgnoreCase);

    internal int LiveCount => workersByName.Values.Count(static item => item.State != WorkerRuntimeState.Closed);

    internal void RequestStart(
        string workerName,
        string pipelineId,
        string resumeTaskId)
    {
        if (workersByName.TryGetValue(workerName, out var existing) &&
            existing.State != WorkerRuntimeState.Closed)
        {
            throw new InvalidOperationException($"Worker '{workerName}' is already live.");
        }

        workersByName[workerName] = new WorkerEntry(
            workerName,
            pipelineId,
            resumeTaskId,
            WorkerRuntimeState.Starting);
    }

    internal WorkerRuntimeState GetState(string workerName) =>
        Require(workerName).State;

    internal void ApplyLifecycleTransition(string workerName, WorkerRuntimeState state) =>
        Require(workerName).State = state;

    internal bool TryFindRunningWorker(string workerName, out WorkerRuntimeSnapshot worker)
    {
        if (workersByName.TryGetValue(workerName, out var entry) &&
            entry.State != WorkerRuntimeState.Closed)
        {
            worker = entry.ToSnapshot();
            return true;
        }

        worker = default!;
        return false;
    }

    internal IReadOnlyList<WorkerRuntimeSnapshot> CreateSnapshot() =>
        workersByName.Values
            .OrderBy(static item => item.WorkerName, StringComparer.OrdinalIgnoreCase)
            .Select(static item => item.ToSnapshot())
            .ToArray();

    private WorkerEntry Require(string workerName) =>
        workersByName.TryGetValue(workerName, out var entry)
            ? entry
            : throw new InvalidOperationException($"Worker '{workerName}' is not tracked.");

    private sealed class WorkerEntry(
        string workerName,
        string pipelineId,
        string resumeTaskId,
        WorkerRuntimeState state)
    {
        public string WorkerName { get; } = workerName;

        public string PipelineId { get; } = pipelineId;

        public string ResumeTaskId { get; } = resumeTaskId;

        public WorkerRuntimeState State { get; set; } = state;

        public WorkerRuntimeSnapshot ToSnapshot() =>
            new(WorkerName, PipelineId, ResumeTaskId, State);
    }
}

internal sealed class PipelineActivations
{
    private readonly Dictionary<string, PipelineActivationEntry> pipelinesByName;

    public PipelineActivations(IEnumerable<RuntimePipelineDefinition> pipelines)
    {
        pipelinesByName = pipelines.ToDictionary(
            static item => item.PipelineName,
            static item => new PipelineActivationEntry(item.PipelineName, item.PipelineId, PipelineActivationState.Inactive),
            StringComparer.OrdinalIgnoreCase);
    }

    internal PipelineActivationState GetState(string pipelineName) =>
        Require(pipelineName).State;

    internal void ApplyTransition(string pipelineName, PipelineActivationState state) =>
        Require(pipelineName).State = state;

    internal IReadOnlyList<PipelineActivationSnapshot> CreateSnapshot() =>
        pipelinesByName.Values
            .OrderBy(static item => item.PipelineName, StringComparer.OrdinalIgnoreCase)
            .Select(static item => new PipelineActivationSnapshot(item.PipelineName, item.PipelineId, item.State))
            .ToArray();

    private PipelineActivationEntry Require(string pipelineName) =>
        pipelinesByName.TryGetValue(pipelineName, out var entry)
            ? entry
            : throw new InvalidOperationException($"Pipeline '{pipelineName}' is not tracked.");

    private sealed class PipelineActivationEntry(
        string pipelineName,
        string pipelineId,
        PipelineActivationState state)
    {
        public string PipelineName { get; } = pipelineName;

        public string PipelineId { get; } = pipelineId;

        public PipelineActivationState State { get; set; } = state;
    }
}

internal sealed class PendingTasks
{
    private readonly HashSet<string> taskIds;

    public PendingTasks(IEnumerable<string> taskIds)
    {
        this.taskIds = taskIds.ToHashSet(StringComparer.Ordinal);
    }

    internal int Count => taskIds.Count;

    internal bool Contains(string taskId) =>
        taskIds.Contains(taskId);

    internal void RemoveForReady(string taskId)
    {
        if (!taskIds.Remove(taskId))
        {
            throw new InvalidOperationException($"Task '{taskId}' is not pending.");
        }
    }

    internal bool RemoveIfPresent(string taskId) =>
        taskIds.Remove(taskId);

    internal void AddForReplacement(string taskId)
    {
        if (!taskIds.Add(taskId))
        {
            throw new InvalidOperationException($"Task '{taskId}' is already pending.");
        }
    }
}

internal sealed class ReadyQueue
{
    private readonly Dictionary<string, RuntimeReadyWork> readyByTaskId = new(StringComparer.Ordinal);

    internal int Count => readyByTaskId.Count;

    internal bool Contains(string taskId) =>
        readyByTaskId.ContainsKey(taskId);

    internal void MarkReady(RuntimeReadyWork ready)
    {
        if (readyByTaskId.ContainsKey(ready.TaskId))
        {
            throw new InvalidOperationException($"Task '{ready.TaskId}' is already ready.");
        }

        readyByTaskId[ready.TaskId] = ready;
    }

    internal bool TryGet(string taskId, out RuntimeReadyWork ready) =>
        readyByTaskId.TryGetValue(taskId, out ready!);

    internal RuntimeReadyWork RemoveForGrant(string taskId)
    {
        if (!readyByTaskId.Remove(taskId, out var ready))
        {
            throw new InvalidOperationException($"Task '{taskId}' is not ready.");
        }

        return ready;
    }

    internal bool RemoveIfPresent(string taskId) =>
        readyByTaskId.Remove(taskId);

    internal IReadOnlyList<RuntimeReadyWork> CreateSnapshot() =>
        readyByTaskId.Values
            .OrderBy(static item => item.PipelineName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.TaskName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
}

internal sealed class RunningGrants
{
    private readonly Dictionary<string, RuntimeRunningGrant> runningByTaskId = new(StringComparer.Ordinal);

    internal int Count => runningByTaskId.Count;

    internal bool ContainsTask(string taskId) =>
        runningByTaskId.ContainsKey(taskId);

    internal RuntimeRunningGrant IssueGrant(
        RuntimeGrant grant,
        string taskName,
        string pipelineName,
        string plannedTaskId,
        string taskAccessProfileId)
    {
        if (runningByTaskId.ContainsKey(grant.TaskId))
        {
            throw new InvalidOperationException($"Task '{grant.TaskId}' already has an active grant.");
        }

        var running = new RuntimeRunningGrant(
            grant,
            taskName,
            pipelineName,
            plannedTaskId,
            taskAccessProfileId,
            GrantRuntimeState.Issued);
        runningByTaskId[grant.TaskId] = running;
        return running;
    }

    internal RuntimeGrant RequireGrant(
        string taskId,
        string grantId,
        string commandId,
        int attemptNumber)
    {
        var running = Require(taskId);
        if (!string.Equals(running.Grant.GrantId, grantId, StringComparison.Ordinal) ||
            !string.Equals(running.Grant.CommandId, commandId, StringComparison.Ordinal) ||
            running.Grant.AttemptNumber != attemptNumber)
        {
            throw new InvalidOperationException(
                $"Grant evidence for task '{taskId}' does not match the active grant id, active command, or active attempt. " +
                $"ActiveGrantId={running.Grant.GrantId}; EventGrantId={grantId}; " +
                $"ActiveCommandId={running.Grant.CommandId}; EventCommandId={commandId}; " +
                $"ActiveAttempt={running.Grant.AttemptNumber.ToString(CultureInfo.InvariantCulture)}; EventAttempt={attemptNumber.ToString(CultureInfo.InvariantCulture)}.");
        }

        return running.Grant;
    }

    internal RuntimeRunningGrant? FindByWorker(string workerName) =>
        runningByTaskId.Values.FirstOrDefault(item =>
            string.Equals(item.Grant.WorkerName, workerName, StringComparison.OrdinalIgnoreCase));

    internal GrantRuntimeState GetGrantState(string taskId) =>
        Require(taskId).State;

    internal void AcceptGrant(string taskId, string grantId) =>
        UpdateGrantState(taskId, grantId, GrantRuntimeState.Accepted);

    internal void StartGrant(string taskId, string grantId) =>
        UpdateGrantState(taskId, grantId, GrantRuntimeState.Running);

    internal void CompleteGrant(string taskId, string grantId)
    {
        UpdateGrantState(taskId, grantId, GrantRuntimeState.Completed);
        runningByTaskId.Remove(taskId);
    }

    internal void FailGrant(string taskId, string grantId)
    {
        UpdateGrantState(taskId, grantId, GrantRuntimeState.Failed);
        runningByTaskId.Remove(taskId);
    }

    internal IReadOnlyList<GrantRuntimeSnapshot> CreateSnapshot() =>
        runningByTaskId.Values
            .OrderBy(static item => item.Grant.TaskId, StringComparer.Ordinal)
            .Select(static item => new GrantRuntimeSnapshot(
                item.Grant.GrantId,
                item.Grant.CommandId,
                item.Grant.TaskId,
                item.Grant.WorkerName,
                item.Grant.AttemptNumber,
                item.State))
            .ToArray();

    private RuntimeRunningGrant Require(string taskId) =>
        runningByTaskId.TryGetValue(taskId, out var running)
            ? running
            : throw new InvalidOperationException($"Task '{taskId}' has no active grant.");

    private void UpdateGrantState(
        string taskId,
        string grantId,
        GrantRuntimeState state)
    {
        var running = Require(taskId);
        if (!string.Equals(running.Grant.GrantId, grantId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Grant '{grantId}' does not own task '{taskId}'.");
        }

        runningByTaskId[taskId] = running with { State = state };
    }
}

internal sealed class RuntimeLocks
{
    private readonly IReadOnlyList<RuntimeLockCompatibilityPolicy> compatibilityPolicies;
    private readonly Dictionary<string, RuntimeLockRequest[]> locksByGrantId = new(StringComparer.Ordinal);

    public RuntimeLocks(IReadOnlyList<RuntimeLockCompatibilityPolicy> compatibilityPolicies)
    {
        this.compatibilityPolicies = compatibilityPolicies;
    }

    internal int Count => locksByGrantId.Values.Sum(static item => item.Length);

    internal bool CanAcquire(IReadOnlyList<RuntimeLockRequest> requestedLocks)
    {
        foreach (var requested in requestedLocks)
        {
            foreach (var held in locksByGrantId.Values.SelectMany(static item => item))
            {
                if (string.Equals(held.ResourceId, requested.ResourceId, StringComparison.OrdinalIgnoreCase) &&
                    !LocksAreCompatible(held, requested))
                {
                    return false;
                }
            }
        }

        return true;
    }

    internal void AcquireForGrant(
        string grantId,
        IReadOnlyList<RuntimeLockRequest> requestedLocks)
    {
        if (locksByGrantId.ContainsKey(grantId))
        {
            throw new InvalidOperationException($"Grant '{grantId}' already owns locks.");
        }

        if (!CanAcquire(requestedLocks))
        {
            throw new InvalidOperationException($"Requested locks for grant '{grantId}' are not compatible.");
        }

        locksByGrantId[grantId] = requestedLocks.ToArray();
    }

    internal void ReleaseForGrant(string grantId)
    {
        if (!locksByGrantId.Remove(grantId))
        {
            throw new InvalidOperationException($"Grant '{grantId}' has no locks to release.");
        }
    }

    internal IReadOnlyList<LockRuntimeSnapshot> CreateSnapshot() =>
        locksByGrantId
            .OrderBy(static item => item.Key, StringComparer.Ordinal)
            .SelectMany(static item => item.Value.Select(lockRequest => new LockRuntimeSnapshot(
                item.Key,
                lockRequest.ResourceId,
                lockRequest.Mode,
                LockRuntimeState.Acquired)))
            .ToArray();

    private bool LocksAreCompatible(
        RuntimeLockRequest left,
        RuntimeLockRequest right)
    {
        if (string.Equals(left.Mode, "SharedRead", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(right.Mode, "SharedRead", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return compatibilityPolicies.Any(policy =>
            (string.Equals(policy.LeftMode, left.Mode, StringComparison.OrdinalIgnoreCase) &&
             string.Equals(policy.RightMode, right.Mode, StringComparison.OrdinalIgnoreCase)) ||
            (string.Equals(policy.LeftMode, right.Mode, StringComparison.OrdinalIgnoreCase) &&
             string.Equals(policy.RightMode, left.Mode, StringComparison.OrdinalIgnoreCase)));
    }
}

internal sealed class RetrySchedule
{
    private readonly Dictionary<string, RuntimeRetryEntry> retriesByTaskId = new(StringComparer.Ordinal);

    internal int Count => retriesByTaskId.Count;

    internal bool Contains(string taskId) =>
        retriesByTaskId.ContainsKey(taskId);

    internal RuntimeRetryEntry? FirstDue(DateTimeOffset now) =>
        retriesByTaskId.Values
            .Where(item => item.DueAtUtc <= now)
            .OrderBy(static item => item.DueAtUtc)
            .ThenBy(static item => item.TaskId, StringComparer.Ordinal)
            .FirstOrDefault();

    internal RuntimeRetryEntry Consume(string taskId)
    {
        if (!retriesByTaskId.Remove(taskId, out var retry))
        {
            throw new InvalidOperationException($"Task '{taskId}' has no scheduled retry.");
        }

        return retry with { State = RetryRuntimeState.Consumed };
    }

    internal RuntimeRetryEntry ScheduleReplacementRetry(
        string taskId,
        string workerName,
        int attemptNumber,
        DateTimeOffset dueAtUtc,
        string previousGrantId)
    {
        if (retriesByTaskId.ContainsKey(taskId))
        {
            throw new InvalidOperationException($"Task '{taskId}' already has a scheduled retry.");
        }

        var retry = new RuntimeRetryEntry(
            taskId,
            workerName,
            attemptNumber,
            dueAtUtc,
            previousGrantId,
            RetryRuntimeState.Scheduled);
        retriesByTaskId[taskId] = retry;
        return retry;
    }

    internal IReadOnlyList<RetryRuntimeSnapshot> CreateSnapshot() =>
        retriesByTaskId.Values
            .OrderBy(static item => item.TaskId, StringComparer.Ordinal)
            .Select(static item => new RetryRuntimeSnapshot(
                item.TaskId,
                item.WorkerName,
                item.AttemptNumber,
                item.DueAtUtc,
                item.PreviousGrantId,
                item.State))
            .ToArray();

    internal bool RemoveIfPresent(string taskId) =>
        retriesByTaskId.Remove(taskId);
}

internal sealed class PipelineOutcomes
{
    private readonly Dictionary<string, OutcomeRuntimeSnapshot> outcomesByTaskId = new(StringComparer.Ordinal);

    internal void RecordOutcome(
        string taskId,
        string taskAccessProfileId,
        string outcome,
        int exitCode)
    {
        if (outcomesByTaskId.ContainsKey(taskId))
        {
            throw new InvalidOperationException($"Task '{taskId}' already has an outcome.");
        }

        outcomesByTaskId[taskId] = new OutcomeRuntimeSnapshot(taskId, taskAccessProfileId, outcome, exitCode);
    }

    internal bool TryGetOutcomeByTaskAccessProfileId(
        string taskAccessProfileId,
        out OutcomeRuntimeSnapshot outcome)
    {
        outcome = outcomesByTaskId.Values.FirstOrDefault(item =>
            string.Equals(item.TaskAccessProfileId, taskAccessProfileId, StringComparison.Ordinal))!;
        return outcome is not null;
    }

    internal IReadOnlyList<OutcomeRuntimeSnapshot> CreateSnapshot() =>
        outcomesByTaskId.Values
            .OrderBy(static item => item.TaskId, StringComparer.Ordinal)
            .ToArray();
}

internal sealed class StoppedPipelines
{
    private readonly HashSet<string> pipelineNames = new(StringComparer.OrdinalIgnoreCase);

    internal bool Contains(string pipelineName) =>
        pipelineNames.Contains(pipelineName);

    internal void Stop(string pipelineName)
    {
        if (!pipelineNames.Add(pipelineName))
        {
            return;
        }
    }
}

internal sealed class WorkerReplacementAttempts
{
    private const int MaxPreWorkWorkerReplacementAttempts = 3;
    private readonly Dictionary<string, int> attemptsByBoundary = new(StringComparer.OrdinalIgnoreCase);

    internal RuntimePreWorkReplacementReservation Reserve(
        string pipelineName,
        string resumeTaskId,
        string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pipelineName);
        var normalizedResumeTaskId = string.IsNullOrWhiteSpace(resumeTaskId) ? string.Empty : resumeTaskId.Trim();
        var key = string.IsNullOrWhiteSpace(normalizedResumeTaskId)
            ? pipelineName.Trim()
            : $"{pipelineName.Trim()}\u001f{normalizedResumeTaskId}";
        attemptsByBoundary.TryGetValue(key, out var currentAttempt);
        var nextAttempt = currentAttempt + 1;
        if (nextAttempt > MaxPreWorkWorkerReplacementAttempts)
        {
            var boundary = string.IsNullOrWhiteSpace(normalizedResumeTaskId)
                ? "before pipeline activation"
                : $"before granting task '{normalizedResumeTaskId}'";
            throw new InvalidOperationException(
                $"Pipeline worker '{pipelineName}' exceeded the pre-work worker replacement limit {boundary}. " +
                $"Limit: {MaxPreWorkWorkerReplacementAttempts.ToString(CultureInfo.InvariantCulture)}. Last reason: {reason}");
        }

        attemptsByBoundary[key] = nextAttempt;
        return new RuntimePreWorkReplacementReservation(
            pipelineName,
            normalizedResumeTaskId,
            nextAttempt,
            MaxPreWorkWorkerReplacementAttempts);
    }
}

internal sealed class CapacityDeferrals
{
    private readonly Dictionary<string, string> pendingResumeTaskByWorker = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> parkedResumeTaskByPipeline = new(StringComparer.OrdinalIgnoreCase);

    internal void Request(
        string workerName,
        string resumeTaskId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(resumeTaskId);
        if (pendingResumeTaskByWorker.ContainsKey(workerName))
        {
            throw new InvalidOperationException($"Worker '{workerName}' already has a pending capacity deferral.");
        }

        pendingResumeTaskByWorker[workerName] = resumeTaskId;
    }

    internal bool TryGetPendingResumeTask(
        string workerName,
        out string resumeTaskId) =>
        pendingResumeTaskByWorker.TryGetValue(workerName, out resumeTaskId!);

    internal void Complete(
        string workerName,
        string resumeTaskId)
    {
        if (!pendingResumeTaskByWorker.Remove(workerName, out var pendingResumeTaskId))
        {
            throw new InvalidOperationException($"Worker '{workerName}' has no pending capacity deferral.");
        }

        if (!string.Equals(pendingResumeTaskId, resumeTaskId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Worker '{workerName}' capacity deferral resume task '{pendingResumeTaskId}' does not match '{resumeTaskId}'.");
        }

        parkedResumeTaskByPipeline[workerName] = resumeTaskId;
    }

    internal string ConsumeParkedResumeTask(string pipelineName)
    {
        if (!parkedResumeTaskByPipeline.Remove(pipelineName, out var resumeTaskId))
        {
            return string.Empty;
        }

        return resumeTaskId;
    }
}
