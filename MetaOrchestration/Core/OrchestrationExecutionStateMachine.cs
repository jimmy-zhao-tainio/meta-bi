using System.Globalization;
using MetaOrchestration.WorkerProtocol;

namespace MetaOrchestration.Core;

internal sealed class OrchestrationExecutionStateMachine
{
    private static readonly StateTransition<OrchestrationTaskRuntimeState, OrchestrationTaskRuntimeTrigger>[] TaskTransitionDefinitions =
    [
        new(OrchestrationTaskRuntimeState.Pending, OrchestrationTaskRuntimeTrigger.WorkerTaskReady, OrchestrationTaskRuntimeState.Ready),
        new(OrchestrationTaskRuntimeState.Pending, OrchestrationTaskRuntimeTrigger.Blocked, OrchestrationTaskRuntimeState.Blocked),
        new(OrchestrationTaskRuntimeState.Ready, OrchestrationTaskRuntimeTrigger.GrantIssued, OrchestrationTaskRuntimeState.GrantIssued),
        new(OrchestrationTaskRuntimeState.Ready, OrchestrationTaskRuntimeTrigger.ReadyWorkerLost, OrchestrationTaskRuntimeState.Pending),
        new(OrchestrationTaskRuntimeState.Ready, OrchestrationTaskRuntimeTrigger.Blocked, OrchestrationTaskRuntimeState.Blocked),
        new(OrchestrationTaskRuntimeState.GrantIssued, OrchestrationTaskRuntimeTrigger.GrantAccepted, OrchestrationTaskRuntimeState.GrantAccepted),
        new(OrchestrationTaskRuntimeState.GrantIssued, OrchestrationTaskRuntimeTrigger.TaskStarted, OrchestrationTaskRuntimeState.Running),
        new(OrchestrationTaskRuntimeState.GrantAccepted, OrchestrationTaskRuntimeTrigger.TaskStarted, OrchestrationTaskRuntimeState.Running),
        new(OrchestrationTaskRuntimeState.Running, OrchestrationTaskRuntimeTrigger.TaskSucceeded, OrchestrationTaskRuntimeState.Succeeded),
        new(OrchestrationTaskRuntimeState.Running, OrchestrationTaskRuntimeTrigger.TaskFailed, OrchestrationTaskRuntimeState.Failed),
        new(OrchestrationTaskRuntimeState.Running, OrchestrationTaskRuntimeTrigger.SameWorkerRetryScheduled, OrchestrationTaskRuntimeState.Ready),
        new(OrchestrationTaskRuntimeState.GrantIssued, OrchestrationTaskRuntimeTrigger.SupervisorFailed, OrchestrationTaskRuntimeState.Failed),
        new(OrchestrationTaskRuntimeState.GrantAccepted, OrchestrationTaskRuntimeTrigger.SupervisorFailed, OrchestrationTaskRuntimeState.Failed),
        new(OrchestrationTaskRuntimeState.Running, OrchestrationTaskRuntimeTrigger.SupervisorFailed, OrchestrationTaskRuntimeState.Failed),
        new(OrchestrationTaskRuntimeState.GrantIssued, OrchestrationTaskRuntimeTrigger.ReplacementRetryScheduled, OrchestrationTaskRuntimeState.RetryScheduled),
        new(OrchestrationTaskRuntimeState.GrantAccepted, OrchestrationTaskRuntimeTrigger.ReplacementRetryScheduled, OrchestrationTaskRuntimeState.RetryScheduled),
        new(OrchestrationTaskRuntimeState.Running, OrchestrationTaskRuntimeTrigger.ReplacementRetryScheduled, OrchestrationTaskRuntimeState.RetryScheduled),
        new(OrchestrationTaskRuntimeState.RetryScheduled, OrchestrationTaskRuntimeTrigger.ReplacementWorkerReady, OrchestrationTaskRuntimeState.Pending)
    ];

    private static readonly StateTransition<OrchestrationWorkerRuntimeState, OrchestrationWorkerRuntimeTrigger>[] WorkerTransitionDefinitions =
    [
        new(OrchestrationWorkerRuntimeState.Starting, OrchestrationWorkerRuntimeTrigger.WorkerOnline, OrchestrationWorkerRuntimeState.Online),
        new(OrchestrationWorkerRuntimeState.Online, OrchestrationWorkerRuntimeTrigger.WorkerReady, OrchestrationWorkerRuntimeState.Ready),
        new(OrchestrationWorkerRuntimeState.Ready, OrchestrationWorkerRuntimeTrigger.StartPipelineSent, OrchestrationWorkerRuntimeState.StartPipelineSent),
        new(OrchestrationWorkerRuntimeState.StartPipelineSent, OrchestrationWorkerRuntimeTrigger.PipelineStarted, OrchestrationWorkerRuntimeState.PipelineStarted),
        new(OrchestrationWorkerRuntimeState.Starting, OrchestrationWorkerRuntimeTrigger.WorkerClosed, OrchestrationWorkerRuntimeState.Closed),
        new(OrchestrationWorkerRuntimeState.Online, OrchestrationWorkerRuntimeTrigger.WorkerClosed, OrchestrationWorkerRuntimeState.Closed),
        new(OrchestrationWorkerRuntimeState.Ready, OrchestrationWorkerRuntimeTrigger.WorkerClosed, OrchestrationWorkerRuntimeState.Closed),
        new(OrchestrationWorkerRuntimeState.StartPipelineSent, OrchestrationWorkerRuntimeTrigger.WorkerClosed, OrchestrationWorkerRuntimeState.Closed),
        new(OrchestrationWorkerRuntimeState.PipelineStarted, OrchestrationWorkerRuntimeTrigger.WorkerClosed, OrchestrationWorkerRuntimeState.Closed)
    ];

    private readonly Dictionary<string, TaskState> tasksById;
    private readonly Dictionary<string, WorkerState> workersByName = new(StringComparer.OrdinalIgnoreCase);

    static OrchestrationExecutionStateMachine()
    {
        ValidateTransitionDefinitions(TaskTransitionDefinitions, "task");
        ValidateTransitionDefinitions(WorkerTransitionDefinitions, "worker");
    }

    public OrchestrationExecutionStateMachine(
        IEnumerable<string> taskIds,
        IReadOnlyDictionary<string, string>? pipelineNamesByTaskId = null)
    {
        ArgumentNullException.ThrowIfNull(taskIds);

        tasksById = taskIds
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.Ordinal)
            .ToDictionary(
                static item => item,
                item => new TaskState(
                    item,
                    ResolvePipelineName(item, pipelineNamesByTaskId),
                    OrchestrationTaskRuntimeState.Pending),
                StringComparer.Ordinal);
        if (tasksById.Count == 0)
        {
            throw new ArgumentException("At least one task id is required.", nameof(taskIds));
        }
    }

    public static IReadOnlyList<StateTransition<OrchestrationTaskRuntimeState, OrchestrationTaskRuntimeTrigger>> TaskTransitions =>
        TaskTransitionDefinitions;

    public static IReadOnlyList<StateTransition<OrchestrationWorkerRuntimeState, OrchestrationWorkerRuntimeTrigger>> WorkerTransitions =>
        WorkerTransitionDefinitions;

    public int PendingCount => tasksById.Values.Count(static item => item.State == OrchestrationTaskRuntimeState.Pending);

    public int ReadyCount => tasksById.Values.Count(static item => item.State == OrchestrationTaskRuntimeState.Ready);

    public int RunningCount => tasksById.Values.Count(static item => item.State == OrchestrationTaskRuntimeState.Running);

    public bool HasUnresolvedTasks => tasksById.Values.Any(static item =>
        item.State is OrchestrationTaskRuntimeState.Pending
            or OrchestrationTaskRuntimeState.Ready
            or OrchestrationTaskRuntimeState.GrantIssued
            or OrchestrationTaskRuntimeState.GrantAccepted
            or OrchestrationTaskRuntimeState.Running
            or OrchestrationTaskRuntimeState.RetryScheduled);

    public bool IsPending(string taskId) =>
        GetRequiredTask(taskId).State == OrchestrationTaskRuntimeState.Pending;

    public bool IsReady(string taskId) =>
        GetRequiredTask(taskId).State == OrchestrationTaskRuntimeState.Ready;

    public bool HasActiveGrant(string taskId) =>
        TaskHasActiveGrant(GetRequiredTask(taskId));

    public bool IsRunning(string taskId) =>
        GetRequiredTask(taskId).State == OrchestrationTaskRuntimeState.Running;

    public OrchestrationTaskRuntimeSnapshot GetTaskSnapshot(string taskId)
    {
        var task = GetRequiredTask(taskId);
        return new OrchestrationTaskRuntimeSnapshot(
            task.TaskId,
            task.State,
            task.WorkerName,
            task.GrantId,
            task.CommandId,
            task.PreviousGrantId,
            task.AttemptNumber);
    }

    public IReadOnlyList<OrchestrationTaskRuntimeSnapshot> GetTaskSnapshots() =>
        tasksById.Values
            .OrderBy(static item => item.TaskId, StringComparer.Ordinal)
            .Select(static item => new OrchestrationTaskRuntimeSnapshot(
                item.TaskId,
                item.State,
                item.WorkerName,
                item.GrantId,
                item.CommandId,
                item.PreviousGrantId,
                item.AttemptNumber))
            .ToArray();

    public IReadOnlyList<OrchestrationWorkerRuntimeSnapshot> GetWorkerSnapshots() =>
        workersByName.Values
            .OrderBy(static item => item.WorkerName, StringComparer.OrdinalIgnoreCase)
            .Select(static item => new OrchestrationWorkerRuntimeSnapshot(
                item.WorkerName,
                item.PipelineId,
                item.ResumeTaskId,
                item.State,
                item.TaskBoundaryReached))
            .ToArray();

    public OrchestrationWorkerRuntimeState GetWorkerState(string workerName) =>
        GetRequiredWorker(workerName).State;

    public bool WorkerHasReachedTaskBoundary(string workerName) =>
        GetRequiredWorker(workerName).TaskBoundaryReached;

    public bool WorkerIsOnline(string workerName) =>
        WorkerHasReachedState(GetRequiredWorker(workerName), OrchestrationWorkerRuntimeState.Online);

    public bool WorkerIsReady(string workerName) =>
        WorkerHasReachedState(GetRequiredWorker(workerName), OrchestrationWorkerRuntimeState.Ready);

    public bool WorkerStartPipelineSent(string workerName) =>
        WorkerHasReachedState(GetRequiredWorker(workerName), OrchestrationWorkerRuntimeState.StartPipelineSent);

    public bool WorkerPipelineStarted(string workerName) =>
        WorkerHasReachedState(GetRequiredWorker(workerName), OrchestrationWorkerRuntimeState.PipelineStarted);

    public string? ReadyTaskIdForWorker(string workerName) =>
        tasksById.Values
            .Where(item => item.State == OrchestrationTaskRuntimeState.Ready)
            .Where(item => string.Equals(item.WorkerName, workerName, StringComparison.OrdinalIgnoreCase))
            .Select(static item => item.TaskId)
            .FirstOrDefault();

    public string? ActiveGrantTaskIdForWorker(string workerName) =>
        tasksById.Values
            .Where(TaskHasActiveGrant)
            .Where(item => string.Equals(item.WorkerName, workerName, StringComparison.OrdinalIgnoreCase))
            .Select(static item => item.TaskId)
            .FirstOrDefault();

    public OrchestrationWorkerTimeoutDecision ResolveWorkerTimeout(string workerName)
    {
        var worker = GetRequiredWorker(workerName);
        return ResolveWorkerTimeoutDecision(worker);
    }

    public OrchestrationWorkerLossDecision ApplyWorkerLoss(
        string workerName,
        int exitCode = 0,
        bool stoppedByOrchestration = false)
    {
        var worker = GetRequiredWorker(workerName);
        var decision = ResolveWorkerLossDecision(worker, exitCode, stoppedByOrchestration);
        if (decision.Kind == OrchestrationWorkerLossDecisionKind.ReplaceAtReadyTaskBoundary)
        {
            MarkPendingAfterReadyWorkerLost(decision.TaskId, workerName);
        }

        MarkWorkerClosed(workerName);
        return decision;
    }

    public void RegisterWorker(
        string workerName,
        string pipelineId,
        string resumeTaskId,
        string expectedExecutableVersion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workerName);
        if (workersByName.TryGetValue(workerName, out var existing) &&
            existing.State != OrchestrationWorkerRuntimeState.Closed)
        {
            throw new InvalidOperationException($"Pipeline worker '{workerName}' is already registered.");
        }

        workersByName[workerName] = new WorkerState(
            workerName,
            string.IsNullOrWhiteSpace(pipelineId) ? $"pipeline:{workerName}" : pipelineId,
            string.IsNullOrWhiteSpace(resumeTaskId) ? string.Empty : resumeTaskId.Trim(),
            expectedExecutableVersion);
    }

    public void MarkWorkerOnline(string workerName, string executableVersion)
    {
        var worker = GetRequiredWorker(workerName);
        if (string.IsNullOrWhiteSpace(executableVersion))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{workerName}' did not report an executable version.");
        }

        if (!string.Equals(executableVersion, worker.ExpectedExecutableVersion, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{workerName}' executable version mismatch. Expected '{worker.ExpectedExecutableVersion}', got '{executableVersion}'.");
        }

        ApplyWorkerTransition(worker, OrchestrationWorkerRuntimeTrigger.WorkerOnline);
    }

    public void MarkWorkerReady(string workerName) =>
        ApplyWorkerTransition(GetRequiredWorker(workerName), OrchestrationWorkerRuntimeTrigger.WorkerReady);

    public void MarkStartPipelineSent(string workerName) =>
        ApplyWorkerTransition(GetRequiredWorker(workerName), OrchestrationWorkerRuntimeTrigger.StartPipelineSent);

    public void MarkPipelineStarted(string workerName) =>
        ApplyWorkerTransition(GetRequiredWorker(workerName), OrchestrationWorkerRuntimeTrigger.PipelineStarted);

    public void AcceptWorkerLifecycleEvent(string workerName, string eventKind)
    {
        var worker = GetRequiredWorker(workerName);
        EnsureWorkerCanEmitEvent(worker, eventKind);
        if (!WorkerHasReachedState(worker, OrchestrationWorkerRuntimeState.Online))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{workerName}' emitted {eventKind} before {WorkerEventKinds.WorkerOnline}.");
        }
    }

    public void MarkWorkerClosed(string workerName)
    {
        var worker = GetRequiredWorker(workerName);
        if (worker.State == OrchestrationWorkerRuntimeState.Closed)
        {
            return;
        }

        ApplyWorkerTransition(worker, OrchestrationWorkerRuntimeTrigger.WorkerClosed);
    }

    public void ValidateTaskEvent(string workerName, string eventKind, string taskId)
    {
        var worker = GetRequiredWorker(workerName);
        EnsureWorkerCanEmitEvent(worker, eventKind);
        if (!WorkerHasReachedState(worker, OrchestrationWorkerRuntimeState.Online))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{workerName}' emitted {eventKind} before {WorkerEventKinds.WorkerOnline}.");
        }

        switch (eventKind)
        {
            case WorkerEventKinds.TaskReady:
                ValidateTaskReady(worker, taskId);
                return;
            case WorkerEventKinds.GrantAccepted:
                ValidateActiveGrantEvent(worker, taskId, [OrchestrationTaskRuntimeState.GrantIssued], eventKind);
                return;
            case WorkerEventKinds.TaskStarted:
                ValidateActiveGrantEvent(
                    worker,
                    taskId,
                    [OrchestrationTaskRuntimeState.GrantIssued, OrchestrationTaskRuntimeState.GrantAccepted],
                    eventKind);
                return;
            case WorkerEventKinds.TaskSucceeded:
            case WorkerEventKinds.TaskFailed:
                ValidateActiveGrantEvent(worker, taskId, [OrchestrationTaskRuntimeState.Running], eventKind);
                return;
            default:
                throw new InvalidOperationException(
                    $"Pipeline worker '{workerName}' emitted unsupported event '{eventKind}'.");
        }
    }

    public void MarkReady(string taskId, string workerName)
    {
        var worker = GetRequiredWorker(workerName);
        ValidateTaskReady(worker, taskId);
        var task = GetRequiredTask(taskId);

        ApplyTaskTransition(task, OrchestrationTaskRuntimeTrigger.WorkerTaskReady);
        task.WorkerName = workerName;
        worker.TaskBoundaryReached = true;
    }

    public void MarkGrantIssued(
        string taskId,
        string workerName,
        string grantId,
        string commandId,
        int attemptNumber)
    {
        if (string.IsNullOrWhiteSpace(grantId))
        {
            throw new InvalidOperationException($"Cannot grant task '{taskId}' without a grant id.");
        }

        if (string.IsNullOrWhiteSpace(commandId))
        {
            throw new InvalidOperationException($"Cannot grant task '{taskId}' without a command id.");
        }

        if (attemptNumber <= 0)
        {
            throw new InvalidOperationException($"Cannot grant task '{taskId}' without a positive attempt number.");
        }

        var task = GetRequiredTask(taskId);
        if (!string.Equals(task.WorkerName, workerName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Cannot grant task '{taskId}' on worker '{workerName}' because it is ready on worker '{task.WorkerName}'.");
        }

        ApplyTaskTransition(task, OrchestrationTaskRuntimeTrigger.GrantIssued);
        task.GrantId = grantId;
        task.CommandId = commandId;
        task.AttemptNumber = attemptNumber;
    }

    public void MarkGrantAccepted(string taskId, string workerName, string grantId, string commandId, int attemptNumber)
    {
        var task = GetRequiredTask(taskId);
        ValidateActiveGrantEvent(
            GetRequiredWorker(workerName),
            taskId,
            [OrchestrationTaskRuntimeState.GrantIssued],
            WorkerEventKinds.GrantAccepted);
        ValidateGrantEvidence(task, grantId, commandId, attemptNumber, WorkerEventKinds.GrantAccepted);
        ApplyTaskTransition(task, OrchestrationTaskRuntimeTrigger.GrantAccepted);
    }

    public void MarkTaskStarted(string taskId, string workerName, string grantId, string commandId, int attemptNumber)
    {
        var task = GetRequiredTask(taskId);
        ValidateActiveGrantEvent(
            GetRequiredWorker(workerName),
            taskId,
            [OrchestrationTaskRuntimeState.GrantIssued, OrchestrationTaskRuntimeState.GrantAccepted],
            WorkerEventKinds.TaskStarted);
        ValidateGrantEvidence(task, grantId, commandId, attemptNumber, WorkerEventKinds.TaskStarted);
        ApplyTaskTransition(task, OrchestrationTaskRuntimeTrigger.TaskStarted);
    }

    public void MarkSucceeded(string taskId, string workerName, string grantId, string commandId, int attemptNumber)
    {
        var task = GetRequiredTask(taskId);
        ValidateActiveGrantEvent(
            GetRequiredWorker(workerName),
            taskId,
            [OrchestrationTaskRuntimeState.Running],
            WorkerEventKinds.TaskSucceeded);
        ValidateGrantEvidence(task, grantId, commandId, attemptNumber, WorkerEventKinds.TaskSucceeded);
        ApplyTaskTransition(task, OrchestrationTaskRuntimeTrigger.TaskSucceeded);
        task.ClearWorkerState();
    }

    public void MarkFailed(string taskId, string workerName, string grantId, string commandId, int attemptNumber)
    {
        var task = GetRequiredTask(taskId);
        ValidateActiveGrantEvent(
            GetRequiredWorker(workerName),
            taskId,
            [OrchestrationTaskRuntimeState.Running],
            WorkerEventKinds.TaskFailed);
        ValidateGrantEvidence(task, grantId, commandId, attemptNumber, WorkerEventKinds.TaskFailed);
        ApplyTaskTransition(task, OrchestrationTaskRuntimeTrigger.TaskFailed);
        task.ClearWorkerState();
    }

    public void MarkFailedFromSupervisor(string taskId)
    {
        var task = GetRequiredTask(taskId);
        ApplyTaskTransition(task, OrchestrationTaskRuntimeTrigger.SupervisorFailed);
        task.ClearWorkerState();
    }

    public void MarkReadyForSameWorkerRetry(
        string taskId,
        string workerName,
        string grantId,
        string commandId,
        int eventAttemptNumber,
        string previousGrantId,
        int nextAttemptNumber)
    {
        var task = GetRequiredTask(taskId);
        ValidateActiveGrantEvent(
            GetRequiredWorker(workerName),
            taskId,
            [OrchestrationTaskRuntimeState.Running],
            WorkerEventKinds.TaskFailed);
        ValidateGrantEvidence(task, grantId, commandId, eventAttemptNumber, WorkerEventKinds.TaskFailed);
        ApplyTaskTransition(task, OrchestrationTaskRuntimeTrigger.SameWorkerRetryScheduled);
        task.WorkerName = workerName;
        task.PreviousGrantId = string.IsNullOrWhiteSpace(previousGrantId) ? task.GrantId : previousGrantId;
        task.AttemptNumber = nextAttemptNumber;
        task.GrantId = string.Empty;
        task.CommandId = string.Empty;
    }

    public void MarkRetryScheduledForReplacement(string taskId, string previousGrantId, int attemptNumber)
    {
        var task = GetRequiredTask(taskId);
        ApplyTaskTransition(task, OrchestrationTaskRuntimeTrigger.ReplacementRetryScheduled);
        task.PreviousGrantId = previousGrantId;
        task.AttemptNumber = attemptNumber;
        task.ClearWorkerState(keepRetryEvidence: true);
    }

    public void MarkPendingForReplacement(string taskId)
    {
        var task = GetRequiredTask(taskId);
        ApplyTaskTransition(task, OrchestrationTaskRuntimeTrigger.ReplacementWorkerReady);
    }

    public void MarkPendingAfterReadyWorkerLost(string taskId, string workerName)
    {
        var task = GetRequiredTask(taskId);
        if (!string.Equals(task.WorkerName, workerName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Cannot return ready task '{taskId}' to pending because it is ready on worker '{task.WorkerName}', not '{workerName}'.");
        }

        ApplyTaskTransition(task, OrchestrationTaskRuntimeTrigger.ReadyWorkerLost);
        task.ClearWorkerState();
    }

    public void MarkBlocked(string taskId)
    {
        var task = GetRequiredTask(taskId);
        ApplyTaskTransition(task, OrchestrationTaskRuntimeTrigger.Blocked);
        task.ClearWorkerState();
    }

    private OrchestrationWorkerLossDecision ResolveWorkerLossDecision(
        WorkerState worker,
        int exitCode,
        bool stoppedByOrchestration)
    {
        if (worker.State == OrchestrationWorkerRuntimeState.Closed)
        {
            return OrchestrationWorkerLossDecision.CloseOnly(worker.WorkerName);
        }

        if (ActiveGrantTaskIdForWorker(worker.WorkerName) is { } activeTaskId)
        {
            return OrchestrationWorkerLossDecision.ActiveGrantLost(worker.WorkerName, activeTaskId);
        }

        if (ReadyTaskIdForWorker(worker.WorkerName) is { } readyTaskId)
        {
            return OrchestrationWorkerLossDecision.ReplaceAtReadyTaskBoundary(worker.WorkerName, readyTaskId);
        }

        var hasUnresolvedPipelineTasks = PipelineHasUnresolvedTasks(worker.WorkerName);
        if (!hasUnresolvedPipelineTasks)
        {
            if (exitCode != 0 &&
                !stoppedByOrchestration &&
                !PipelineHasFailedTask(worker.WorkerName))
            {
                return OrchestrationWorkerLossDecision.FailUnresolved(worker.WorkerName);
            }

            return OrchestrationWorkerLossDecision.CloseOnly(worker.WorkerName);
        }

        if (PipelineHasFailedTask(worker.WorkerName))
        {
            return OrchestrationWorkerLossDecision.BlockRemainingAfterFailure(worker.WorkerName);
        }

        if (WorkerCanRestartWithoutPipelineContext(worker))
        {
            return OrchestrationWorkerLossDecision.ReplaceFromBeginning(worker.WorkerName);
        }

        return OrchestrationWorkerLossDecision.FailUnresolved(worker.WorkerName);
    }

    private OrchestrationWorkerTimeoutDecision ResolveWorkerTimeoutDecision(WorkerState worker)
    {
        var hasUnresolvedPipelineTasks = PipelineHasUnresolvedTasks(worker.WorkerName);
        var hasReachedTaskBoundary = worker.TaskBoundaryReached;

        if (!WorkerHasReachedState(worker, OrchestrationWorkerRuntimeState.Online))
        {
            return OrchestrationWorkerTimeoutDecision.AwaitingWorkerOnline(
                worker.WorkerName,
                hasUnresolvedPipelineTasks,
                hasReachedTaskBoundary);
        }

        if (!WorkerHasReachedState(worker, OrchestrationWorkerRuntimeState.Ready))
        {
            return OrchestrationWorkerTimeoutDecision.AwaitingWorkerReady(
                worker.WorkerName,
                hasUnresolvedPipelineTasks,
                hasReachedTaskBoundary);
        }

        if (!WorkerHasReachedState(worker, OrchestrationWorkerRuntimeState.StartPipelineSent))
        {
            return OrchestrationWorkerTimeoutDecision.AwaitingStartPipelineCommand(
                worker.WorkerName,
                hasUnresolvedPipelineTasks,
                hasReachedTaskBoundary);
        }

        if (!WorkerHasReachedState(worker, OrchestrationWorkerRuntimeState.PipelineStarted))
        {
            return OrchestrationWorkerTimeoutDecision.AwaitingPipelineStarted(
                worker.WorkerName,
                hasUnresolvedPipelineTasks,
                hasReachedTaskBoundary);
        }

        if (ReadyTaskIdForWorker(worker.WorkerName) is { } readyTaskId)
        {
            return OrchestrationWorkerTimeoutDecision.WaitingForGrant(
                worker.WorkerName,
                readyTaskId,
                hasUnresolvedPipelineTasks,
                hasReachedTaskBoundary);
        }

        if (ActiveGrantTaskIdForWorker(worker.WorkerName) is { } activeTaskId)
        {
            return OrchestrationWorkerTimeoutDecision.ActiveGrantTimedOut(
                worker.WorkerName,
                activeTaskId,
                hasUnresolvedPipelineTasks,
                hasReachedTaskBoundary);
        }

        return hasUnresolvedPipelineTasks
            ? OrchestrationWorkerTimeoutDecision.AwaitingTaskBoundary(
                worker.WorkerName,
                hasUnresolvedPipelineTasks,
                hasReachedTaskBoundary)
            : OrchestrationWorkerTimeoutDecision.PipelineResolved(
                worker.WorkerName,
                hasReachedTaskBoundary);
    }

    private bool PipelineHasUnresolvedTasks(string pipelineName) =>
        tasksById.Values.Any(task =>
            string.Equals(task.PipelineName, pipelineName, StringComparison.OrdinalIgnoreCase) &&
            TaskIsUnresolved(task));

    private bool PipelineHasFailedTask(string pipelineName) =>
        tasksById.Values.Any(task =>
            string.Equals(task.PipelineName, pipelineName, StringComparison.OrdinalIgnoreCase) &&
            task.State == OrchestrationTaskRuntimeState.Failed);

    private static bool TaskIsUnresolved(TaskState task) =>
        task.State is OrchestrationTaskRuntimeState.Pending
            or OrchestrationTaskRuntimeState.Ready
            or OrchestrationTaskRuntimeState.GrantIssued
            or OrchestrationTaskRuntimeState.GrantAccepted
            or OrchestrationTaskRuntimeState.Running
            or OrchestrationTaskRuntimeState.RetryScheduled;

    private static bool WorkerCanRestartWithoutPipelineContext(WorkerState worker) =>
        worker.State is OrchestrationWorkerRuntimeState.Online or OrchestrationWorkerRuntimeState.Ready;

    private void ValidateTaskReady(WorkerState worker, string taskId)
    {
        EnsureWorkerCanEmitEvent(worker, WorkerEventKinds.TaskReady);
        if (worker.State != OrchestrationWorkerRuntimeState.PipelineStarted)
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.WorkerName}' emitted {WorkerEventKinds.TaskReady} before {WorkerEventKinds.PipelineStarted}.");
        }

        if (string.IsNullOrWhiteSpace(taskId))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.WorkerName}' emitted {WorkerEventKinds.TaskReady} without a task id.");
        }

        var task = GetRequiredTask(taskId);
        if (!CanApplyTaskTransition(task.State, OrchestrationTaskRuntimeTrigger.WorkerTaskReady))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.WorkerName}' emitted {WorkerEventKinds.TaskReady} for task '{taskId}', but that task is not pending. The worker is waiting for a command orchestration must not send.");
        }

        if (ReadyTaskIdForWorker(worker.WorkerName) is { } readyTaskId)
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.WorkerName}' emitted {WorkerEventKinds.TaskReady} for task '{taskId}' while it is already ready at task '{readyTaskId}'.");
        }

        if (ActiveGrantTaskIdForWorker(worker.WorkerName) is { } activeTaskId)
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.WorkerName}' emitted {WorkerEventKinds.TaskReady} for task '{taskId}' while grant '{activeTaskId}' is still active.");
        }
    }

    private void ValidateActiveGrantEvent(
        WorkerState worker,
        string taskId,
        OrchestrationTaskRuntimeState[] allowedStates,
        string eventKind)
    {
        EnsureWorkerCanEmitEvent(worker, eventKind);
        if (string.IsNullOrWhiteSpace(taskId))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.WorkerName}' emitted {eventKind} without a task id.");
        }

        var activeTaskId = ActiveGrantTaskIdForWorker(worker.WorkerName);
        if (string.IsNullOrWhiteSpace(activeTaskId))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.WorkerName}' emitted {eventKind} for task '{taskId}', but orchestration has no active grant for that worker.");
        }

        if (!string.Equals(activeTaskId, taskId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.WorkerName}' emitted {eventKind} for task '{taskId}', but the active grant is '{activeTaskId}'.");
        }

        var task = GetRequiredTask(taskId);
        if (!allowedStates.Contains(task.State))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.WorkerName}' emitted {eventKind} for task '{taskId}', but task state is {task.State}.");
        }
    }

    private static void ValidateGrantEvidence(
        TaskState task,
        string grantId,
        string commandId,
        int attemptNumber,
        string eventKind)
    {
        if (string.IsNullOrWhiteSpace(grantId))
        {
            throw new InvalidOperationException(
                $"Worker event {eventKind} for task '{task.TaskId}' did not report a grant id.");
        }

        if (!string.Equals(task.GrantId, grantId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Worker event {eventKind} for task '{task.TaskId}' reported grant id '{grantId}', but the active grant is '{task.GrantId}'.");
        }

        if (string.IsNullOrWhiteSpace(commandId))
        {
            throw new InvalidOperationException(
                $"Worker event {eventKind} for task '{task.TaskId}' did not report a command id.");
        }

        if (!string.Equals(task.CommandId, commandId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Worker event {eventKind} for task '{task.TaskId}' reported command id '{commandId}', but the active command is '{task.CommandId}'.");
        }

        if (attemptNumber <= 0)
        {
            throw new InvalidOperationException(
                $"Worker event {eventKind} for task '{task.TaskId}' did not report an attempt number.");
        }

        if (attemptNumber != task.AttemptNumber)
        {
            throw new InvalidOperationException(
                $"Worker event {eventKind} for task '{task.TaskId}' reported attempt {attemptNumber.ToString(CultureInfo.InvariantCulture)}, but the active attempt is {task.AttemptNumber.ToString(CultureInfo.InvariantCulture)}.");
        }
    }

    private static void ApplyTaskTransition(TaskState task, OrchestrationTaskRuntimeTrigger trigger)
    {
        var next = ApplyTransition(TaskTransitionDefinitions, task.State, trigger, $"task '{task.TaskId}'");
        task.State = next;
    }

    private static void ApplyWorkerTransition(WorkerState worker, OrchestrationWorkerRuntimeTrigger trigger)
    {
        var next = ApplyTransition(WorkerTransitionDefinitions, worker.State, trigger, $"worker '{worker.WorkerName}'");
        worker.State = next;
    }

    private static bool CanApplyTaskTransition(
        OrchestrationTaskRuntimeState state,
        OrchestrationTaskRuntimeTrigger trigger) =>
        TaskTransitionDefinitions.Any(item => item.From.Equals(state) && item.Trigger.Equals(trigger));

    private static TState ApplyTransition<TState, TTrigger>(
        IReadOnlyList<StateTransition<TState, TTrigger>> transitions,
        TState current,
        TTrigger trigger,
        string subject)
        where TState : struct, Enum
        where TTrigger : struct, Enum
    {
        var match = transitions
            .Where(item => item.From.Equals(current) && item.Trigger.Equals(trigger))
            .ToArray();
        if (match.Length == 1)
        {
            return match[0].To;
        }

        if (match.Length > 1)
        {
            throw new InvalidOperationException(
                $"Ambiguous orchestration transition for {subject}: {current} + {trigger} has {match.Length.ToString(CultureInfo.InvariantCulture)} definitions.");
        }

        throw new InvalidOperationException(
            $"Illegal orchestration transition for {subject}: {current} + {trigger}.");
    }

    private static void ValidateTransitionDefinitions<TState, TTrigger>(
        IReadOnlyList<StateTransition<TState, TTrigger>> transitions,
        string machineName)
        where TState : struct, Enum
        where TTrigger : struct, Enum
    {
        var duplicate = transitions
            .GroupBy(static item => (item.From, item.Trigger))
            .FirstOrDefault(static group => group.Count() > 1);
        if (duplicate is not null)
        {
            throw new InvalidOperationException(
                $"Duplicate {machineName} transition definition for {duplicate.Key.From} + {duplicate.Key.Trigger}.");
        }
    }

    private static string ResolvePipelineName(
        string taskId,
        IReadOnlyDictionary<string, string>? pipelineNamesByTaskId)
    {
        if (pipelineNamesByTaskId is not null &&
            pipelineNamesByTaskId.TryGetValue(taskId, out var pipelineName) &&
            !string.IsNullOrWhiteSpace(pipelineName))
        {
            return pipelineName;
        }

        const string prefix = "pipeline:";
        const string separator = ":task:";
        if (taskId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            var separatorIndex = taskId.IndexOf(separator, StringComparison.OrdinalIgnoreCase);
            if (separatorIndex > prefix.Length)
            {
                return taskId[prefix.Length..separatorIndex];
            }
        }

        return string.Empty;
    }

    private TaskState GetRequiredTask(string taskId)
    {
        if (string.IsNullOrWhiteSpace(taskId))
        {
            throw new InvalidOperationException("Task id is required for orchestration state transition.");
        }

        if (!tasksById.TryGetValue(taskId, out var task))
        {
            throw new InvalidOperationException($"Task '{taskId}' is not known to the orchestration state machine.");
        }

        return task;
    }

    private WorkerState GetRequiredWorker(string workerName)
    {
        if (string.IsNullOrWhiteSpace(workerName))
        {
            throw new InvalidOperationException("Worker name is required for orchestration state transition.");
        }

        if (!workersByName.TryGetValue(workerName, out var worker))
        {
            throw new InvalidOperationException($"Pipeline worker '{workerName}' is not known to the orchestration state machine.");
        }

        return worker;
    }

    private static bool TaskHasActiveGrant(TaskState task) =>
        task.State is OrchestrationTaskRuntimeState.GrantIssued
            or OrchestrationTaskRuntimeState.GrantAccepted
            or OrchestrationTaskRuntimeState.Running;

    private static bool WorkerHasReachedState(WorkerState worker, OrchestrationWorkerRuntimeState minimumState) =>
        worker.State != OrchestrationWorkerRuntimeState.Closed &&
        worker.State >= minimumState;

    private static void EnsureWorkerCanEmitEvent(WorkerState worker, string eventKind)
    {
        if (worker.State == OrchestrationWorkerRuntimeState.Closed)
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.WorkerName}' emitted {eventKind} after it was closed.");
        }
    }

    private sealed class TaskState
    {
        public TaskState(
            string taskId,
            string pipelineName,
            OrchestrationTaskRuntimeState state)
        {
            TaskId = taskId;
            PipelineName = pipelineName;
            State = state;
        }

        public string TaskId { get; }

        public string PipelineName { get; }

        public OrchestrationTaskRuntimeState State { get; set; }

        public string WorkerName { get; set; } = string.Empty;

        public string GrantId { get; set; } = string.Empty;

        public string CommandId { get; set; } = string.Empty;

        public string PreviousGrantId { get; set; } = string.Empty;

        public int AttemptNumber { get; set; }

        public void ClearWorkerState(bool keepRetryEvidence = false)
        {
            WorkerName = string.Empty;
            GrantId = string.Empty;
            CommandId = string.Empty;
            if (!keepRetryEvidence)
            {
                PreviousGrantId = string.Empty;
                AttemptNumber = 0;
            }
        }
    }

    private sealed class WorkerState
    {
        public WorkerState(
            string workerName,
            string pipelineId,
            string resumeTaskId,
            string expectedExecutableVersion)
        {
            WorkerName = workerName;
            PipelineId = pipelineId;
            ResumeTaskId = resumeTaskId;
            ExpectedExecutableVersion = expectedExecutableVersion;
        }

        public string WorkerName { get; }

        public string PipelineId { get; }

        public string ResumeTaskId { get; }

        public string ExpectedExecutableVersion { get; }

        public OrchestrationWorkerRuntimeState State { get; set; } = OrchestrationWorkerRuntimeState.Starting;

        public bool TaskBoundaryReached { get; set; }
    }
}

internal readonly record struct StateTransition<TState, TTrigger>(TState From, TTrigger Trigger, TState To)
    where TState : struct, Enum
    where TTrigger : struct, Enum;

internal readonly record struct OrchestrationTaskRuntimeSnapshot(
    string TaskId,
    OrchestrationTaskRuntimeState State,
    string WorkerName,
    string GrantId,
    string CommandId,
    string PreviousGrantId,
    int AttemptNumber);

internal readonly record struct OrchestrationWorkerRuntimeSnapshot(
    string WorkerName,
    string PipelineId,
    string ResumeTaskId,
    OrchestrationWorkerRuntimeState State,
    bool TaskBoundaryReached);

internal readonly record struct OrchestrationWorkerLossDecision(
    OrchestrationWorkerLossDecisionKind Kind,
    string WorkerName,
    string TaskId,
    string ResumeTaskId)
{
    public static OrchestrationWorkerLossDecision CloseOnly(string workerName) =>
        new(OrchestrationWorkerLossDecisionKind.CloseOnly, workerName, string.Empty, string.Empty);

    public static OrchestrationWorkerLossDecision ActiveGrantLost(string workerName, string taskId) =>
        new(OrchestrationWorkerLossDecisionKind.ActiveGrantLost, workerName, taskId, taskId);

    public static OrchestrationWorkerLossDecision ReplaceAtReadyTaskBoundary(string workerName, string taskId) =>
        new(OrchestrationWorkerLossDecisionKind.ReplaceAtReadyTaskBoundary, workerName, taskId, taskId);

    public static OrchestrationWorkerLossDecision ReplaceFromBeginning(string workerName) =>
        new(OrchestrationWorkerLossDecisionKind.ReplaceFromBeginning, workerName, string.Empty, string.Empty);

    public static OrchestrationWorkerLossDecision BlockRemainingAfterFailure(string workerName) =>
        new(OrchestrationWorkerLossDecisionKind.BlockRemainingAfterFailure, workerName, string.Empty, string.Empty);

    public static OrchestrationWorkerLossDecision FailUnresolved(string workerName) =>
        new(OrchestrationWorkerLossDecisionKind.FailUnresolved, workerName, string.Empty, string.Empty);
}

internal enum OrchestrationWorkerLossDecisionKind
{
    CloseOnly,
    ReplaceFromBeginning,
    ReplaceAtReadyTaskBoundary,
    ActiveGrantLost,
    BlockRemainingAfterFailure,
    FailUnresolved
}

internal readonly record struct OrchestrationWorkerTimeoutDecision(
    OrchestrationWorkerTimeoutDecisionKind Kind,
    string WorkerName,
    string TaskId,
    string ExpectedEventKind,
    string ExpectedCommandKind,
    bool HasUnresolvedPipelineTasks,
    bool WorkerHasReachedTaskBoundary,
    bool IsWaitingForOrchestrationCommand)
{
    public static OrchestrationWorkerTimeoutDecision AwaitingWorkerOnline(
        string workerName,
        bool hasUnresolvedPipelineTasks,
        bool workerHasReachedTaskBoundary) =>
        new(
            OrchestrationWorkerTimeoutDecisionKind.AwaitingWorkerOnline,
            workerName,
            string.Empty,
            WorkerEventKinds.WorkerOnline,
            string.Empty,
            hasUnresolvedPipelineTasks,
            workerHasReachedTaskBoundary,
            false);

    public static OrchestrationWorkerTimeoutDecision AwaitingWorkerReady(
        string workerName,
        bool hasUnresolvedPipelineTasks,
        bool workerHasReachedTaskBoundary) =>
        new(
            OrchestrationWorkerTimeoutDecisionKind.AwaitingWorkerReady,
            workerName,
            string.Empty,
            WorkerEventKinds.WorkerReady,
            string.Empty,
            hasUnresolvedPipelineTasks,
            workerHasReachedTaskBoundary,
            false);

    public static OrchestrationWorkerTimeoutDecision AwaitingStartPipelineCommand(
        string workerName,
        bool hasUnresolvedPipelineTasks,
        bool workerHasReachedTaskBoundary) =>
        new(
            OrchestrationWorkerTimeoutDecisionKind.AwaitingStartPipelineCommand,
            workerName,
            string.Empty,
            string.Empty,
            WorkerCommandKinds.StartPipeline,
            hasUnresolvedPipelineTasks,
            workerHasReachedTaskBoundary,
            false);

    public static OrchestrationWorkerTimeoutDecision AwaitingPipelineStarted(
        string workerName,
        bool hasUnresolvedPipelineTasks,
        bool workerHasReachedTaskBoundary) =>
        new(
            OrchestrationWorkerTimeoutDecisionKind.AwaitingPipelineStarted,
            workerName,
            string.Empty,
            WorkerEventKinds.PipelineStarted,
            WorkerCommandKinds.StartPipeline,
            hasUnresolvedPipelineTasks,
            workerHasReachedTaskBoundary,
            false);

    public static OrchestrationWorkerTimeoutDecision WaitingForGrant(
        string workerName,
        string taskId,
        bool hasUnresolvedPipelineTasks,
        bool workerHasReachedTaskBoundary) =>
        new(
            OrchestrationWorkerTimeoutDecisionKind.WaitingForGrant,
            workerName,
            taskId,
            string.Empty,
            WorkerCommandKinds.GrantTask,
            hasUnresolvedPipelineTasks,
            workerHasReachedTaskBoundary,
            true);

    public static OrchestrationWorkerTimeoutDecision ActiveGrantTimedOut(
        string workerName,
        string taskId,
        bool hasUnresolvedPipelineTasks,
        bool workerHasReachedTaskBoundary) =>
        new(
            OrchestrationWorkerTimeoutDecisionKind.ActiveGrantTimedOut,
            workerName,
            taskId,
            string.Empty,
            WorkerCommandKinds.GrantTask,
            hasUnresolvedPipelineTasks,
            workerHasReachedTaskBoundary,
            false);

    public static OrchestrationWorkerTimeoutDecision AwaitingTaskBoundary(
        string workerName,
        bool hasUnresolvedPipelineTasks,
        bool workerHasReachedTaskBoundary) =>
        new(
            OrchestrationWorkerTimeoutDecisionKind.AwaitingTaskBoundary,
            workerName,
            string.Empty,
            WorkerEventKinds.TaskReady,
            string.Empty,
            hasUnresolvedPipelineTasks,
            workerHasReachedTaskBoundary,
            false);

    public static OrchestrationWorkerTimeoutDecision PipelineResolved(
        string workerName,
        bool workerHasReachedTaskBoundary) =>
        new(
            OrchestrationWorkerTimeoutDecisionKind.PipelineResolved,
            workerName,
            string.Empty,
            string.Empty,
            string.Empty,
            false,
            workerHasReachedTaskBoundary,
            false);
}

internal enum OrchestrationWorkerTimeoutDecisionKind
{
    AwaitingWorkerOnline,
    AwaitingWorkerReady,
    AwaitingStartPipelineCommand,
    AwaitingPipelineStarted,
    WaitingForGrant,
    ActiveGrantTimedOut,
    AwaitingTaskBoundary,
    PipelineResolved
}

internal enum OrchestrationTaskRuntimeState
{
    Pending,
    Ready,
    GrantIssued,
    GrantAccepted,
    Running,
    RetryScheduled,
    Succeeded,
    Failed,
    Blocked
}

internal enum OrchestrationTaskRuntimeTrigger
{
    WorkerTaskReady,
    GrantIssued,
    GrantAccepted,
    TaskStarted,
    TaskSucceeded,
    TaskFailed,
    SupervisorFailed,
    SameWorkerRetryScheduled,
    ReplacementRetryScheduled,
    ReplacementWorkerReady,
    ReadyWorkerLost,
    Blocked
}

internal enum OrchestrationWorkerRuntimeState
{
    Starting,
    Online,
    Ready,
    StartPipelineSent,
    PipelineStarted,
    Closed
}

internal enum OrchestrationWorkerRuntimeTrigger
{
    WorkerOnline,
    WorkerReady,
    StartPipelineSent,
    PipelineStarted,
    WorkerClosed
}
