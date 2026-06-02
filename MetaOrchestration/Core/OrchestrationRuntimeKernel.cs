using System.Globalization;
using MetaOrchestration.WorkerProtocol;
using MO = MetaOrchestration;

namespace MetaOrchestration.Core;

internal sealed class OrchestrationRuntimeKernel
{
    private readonly IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId;
    private readonly IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId;
    private readonly IReadOnlyDictionary<string, MO.PlannedTaskLock[]> locksByPlannedTaskId;
    private readonly IReadOnlyList<MO.LockCompatibilityPolicy> activeLockPolicies;
    private readonly IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependenciesByTaskProfileId;
    private readonly OrchestrationExecutionStateMachine stateMachine;
    private readonly HashSet<string> pendingTaskIds;
    private readonly Dictionary<string, OrchestrationRuntimeReadyTask> readyByTaskId = new(StringComparer.Ordinal);
    private readonly Dictionary<string, OrchestrationRuntimeRetryState> scheduledRetryByTaskId = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> runningWorkerNamesByTaskId = new(StringComparer.Ordinal);
    private readonly Dictionary<string, OrchestrationRuntimeGrant> runningGrantsByTaskId = new(StringComparer.Ordinal);
    private readonly Dictionary<string, MO.PlannedTaskLock[]> runningLocksByPlannedTaskId = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> taskOutcomesByTaskProfileId = new(StringComparer.Ordinal);
    private readonly HashSet<string> stoppedPipelineNames = new(StringComparer.OrdinalIgnoreCase);

    public OrchestrationRuntimeKernel(
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]> locksByPlannedTaskId,
        IReadOnlyList<MO.LockCompatibilityPolicy> activeLockPolicies,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependenciesByTaskProfileId)
    {
        ArgumentNullException.ThrowIfNull(plannedTasksByPipelineTaskId);
        ArgumentNullException.ThrowIfNull(plannedTasksByProfileId);
        ArgumentNullException.ThrowIfNull(locksByPlannedTaskId);
        ArgumentNullException.ThrowIfNull(activeLockPolicies);
        ArgumentNullException.ThrowIfNull(dependenciesByTaskProfileId);
        if (plannedTasksByPipelineTaskId.Count == 0)
        {
            throw new ArgumentException("At least one planned task is required.", nameof(plannedTasksByPipelineTaskId));
        }

        this.plannedTasksByPipelineTaskId = plannedTasksByPipelineTaskId;
        this.plannedTasksByProfileId = plannedTasksByProfileId;
        this.locksByPlannedTaskId = locksByPlannedTaskId;
        this.activeLockPolicies = activeLockPolicies;
        this.dependenciesByTaskProfileId = dependenciesByTaskProfileId;
        pendingTaskIds = plannedTasksByPipelineTaskId.Keys.ToHashSet(StringComparer.Ordinal);
        var pipelineNamesByTaskId = plannedTasksByPipelineTaskId.ToDictionary(
            static item => item.Key,
            static item => item.Value.PipelineReference.Name,
            StringComparer.Ordinal);
        stateMachine = new OrchestrationExecutionStateMachine(plannedTasksByPipelineTaskId.Keys, pipelineNamesByTaskId);
    }

    public IReadOnlySet<string> PendingTaskIds => pendingTaskIds;

    public IReadOnlyDictionary<string, string> TaskOutcomesByTaskProfileId => taskOutcomesByTaskProfileId;

    public IReadOnlyDictionary<string, OrchestrationRuntimeReadyTask> ReadyByTaskId => readyByTaskId;

    public IReadOnlyDictionary<string, OrchestrationRuntimeRetryState> ScheduledRetryByTaskId => scheduledRetryByTaskId;

    public IReadOnlyDictionary<string, string> RunningWorkerNamesByTaskId => runningWorkerNamesByTaskId;

    public IReadOnlyDictionary<string, OrchestrationRuntimeGrant> RunningGrantsByTaskId => runningGrantsByTaskId;

    public IReadOnlyDictionary<string, MO.PlannedTaskLock[]> RunningLocksByPlannedTaskId => runningLocksByPlannedTaskId;

    public int PendingCount => pendingTaskIds.Count;

    public int ReadyCount => readyByTaskId.Count;

    public int RunningCount => runningWorkerNamesByTaskId.Count;

    public bool HasUnresolvedWork => stateMachine.HasUnresolvedTasks;

    public bool HasRuntimeWork => pendingTaskIds.Count > 0 || readyByTaskId.Count > 0 || runningWorkerNamesByTaskId.Count > 0;

    public void RegisterWorker(
        string workerName,
        string pipelineId,
        string resumeTaskId,
        string expectedExecutableVersion) =>
        stateMachine.RegisterWorker(workerName, pipelineId, resumeTaskId, expectedExecutableVersion);

    public void MarkWorkerOnline(string workerName, string executableVersion) =>
        stateMachine.MarkWorkerOnline(workerName, executableVersion);

    public void MarkWorkerReady(string workerName) =>
        stateMachine.MarkWorkerReady(workerName);

    public void MarkStartPipelineSent(string workerName) =>
        stateMachine.MarkStartPipelineSent(workerName);

    public void MarkPipelineStarted(string workerName) =>
        stateMachine.MarkPipelineStarted(workerName);

    public void AcceptWorkerLifecycleEvent(string workerName, string eventKind) =>
        stateMachine.AcceptWorkerLifecycleEvent(workerName, eventKind);

    public void MarkWorkerClosed(string workerName) =>
        stateMachine.MarkWorkerClosed(workerName);

    public OrchestrationWorkerTimeoutDecision ResolveWorkerTimeout(string workerName) =>
        stateMachine.ResolveWorkerTimeout(workerName);

    public OrchestrationWorkerLossDecision ApplyWorkerLoss(
        string workerName,
        int exitCode = 0,
        bool stoppedByOrchestration = false)
    {
        var decision = stateMachine.ApplyWorkerLoss(workerName, exitCode, stoppedByOrchestration);
        if (decision.Kind == OrchestrationWorkerLossDecisionKind.ReplaceAtReadyTaskBoundary)
        {
            readyByTaskId.Remove(decision.TaskId);
            pendingTaskIds.Add(decision.TaskId);
        }

        return decision;
    }

    public bool IsPipelineStopped(string pipelineName) =>
        stoppedPipelineNames.Contains(pipelineName);

    public void AddReady(WorkerProtocolEvent workerEvent, string workerName)
    {
        RequirePlannedTask(workerEvent.TaskId);
        stateMachine.MarkReady(workerEvent.TaskId, workerName);
        if (scheduledRetryByTaskId.Remove(workerEvent.TaskId, out var retry))
        {
            readyByTaskId[workerEvent.TaskId] = new OrchestrationRuntimeReadyTask(
                workerName,
                workerEvent.PipelineId,
                workerEvent.PipelineName,
                workerEvent.TaskId,
                workerEvent.TaskName,
                retry.NotBeforeUtc,
                retry.PreviousGrantId,
                retry.AttemptNumber);
            return;
        }

        readyByTaskId[workerEvent.TaskId] = new OrchestrationRuntimeReadyTask(
            workerName,
            workerEvent.PipelineId,
            workerEvent.PipelineName,
            workerEvent.TaskId,
            workerEvent.TaskName);
    }

    public void MarkGrantAccepted(WorkerProtocolEvent workerEvent, string workerName) =>
        stateMachine.MarkGrantAccepted(
            workerEvent.TaskId,
            workerName,
            workerEvent.GrantId,
            workerEvent.CommandId,
            workerEvent.AttemptNumber);

    public void MarkTaskStarted(WorkerProtocolEvent workerEvent, string workerName) =>
        stateMachine.MarkTaskStarted(
            workerEvent.TaskId,
            workerName,
            workerEvent.GrantId,
            workerEvent.CommandId,
            workerEvent.AttemptNumber);

    public OrchestrationRuntimeReadyDecision ChooseReadyAction(
        IReadOnlySet<string> exitedWorkerNames,
        DateTimeOffset now,
        int maxDegreeOfParallelism)
    {
        foreach (var ready in readyByTaskId.Values
                     .OrderBy(static item => item.PipelineName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(static item => item.TaskName, StringComparer.OrdinalIgnoreCase)
                     .ToArray())
        {
            if (ready.NotBeforeUtc > now)
            {
                continue;
            }

            var plannedTask = RequirePlannedTask(ready.TaskId);
            if (exitedWorkerNames.Contains(ready.WorkerName))
            {
                var decision = ApplyWorkerLoss(ready.WorkerName);
                return OrchestrationRuntimeReadyDecision.ReplaceWorker(decision, ready, plannedTask);
            }

            if (runningWorkerNamesByTaskId.Count >= maxDegreeOfParallelism)
            {
                return OrchestrationRuntimeReadyDecision.None;
            }

            var readiness = OrchestrationExecutionContinuity.EvaluateReadiness(
                plannedTask,
                dependenciesByTaskProfileId,
                taskOutcomesByTaskProfileId,
                out var dependency,
                out var blockedOutcome,
                out var blockedReason);

            if (readiness == OrchestrationTaskReadiness.Waiting)
            {
                continue;
            }

            if (readiness == OrchestrationTaskReadiness.Skip)
            {
                var blocked = BlockReadyPipeline(
                    ready,
                    plannedTask,
                    dependency,
                    blockedOutcome,
                    blockedReason);
                return OrchestrationRuntimeReadyDecision.Block(blocked);
            }

            var plannedTaskLocks = locksByPlannedTaskId.TryGetValue(plannedTask.Id, out var locks)
                ? locks
                : [];
            if (!AreLocksCompatibleWithRunning(
                    plannedTaskLocks,
                    runningLocksByPlannedTaskId.Values.SelectMany(static item => item).ToArray(),
                    activeLockPolicies))
            {
                continue;
            }

            return OrchestrationRuntimeReadyDecision.IssueGrant(
                ready,
                plannedTask,
                plannedTaskLocks,
                OrchestrationRuntimeGrant.Create(
                    ready.PipelineId,
                    ready.TaskId,
                    ready.PreviousGrantId,
                    ready.AttemptNumber <= 0 ? 1 : ready.AttemptNumber));
        }

        return OrchestrationRuntimeReadyDecision.None;
    }

    public OrchestrationRuntimeGrantIssue CommitGrantIssued(
        OrchestrationRuntimeReadyTask ready,
        MO.PlannedTask plannedTask,
        IReadOnlyList<MO.PlannedTaskLock> plannedTaskLocks,
        OrchestrationRuntimeGrant grant)
    {
        stateMachine.MarkGrantIssued(
            ready.TaskId,
            ready.WorkerName,
            grant.GrantId,
            grant.CommandId,
            grant.AttemptNumber);
        runningWorkerNamesByTaskId[ready.TaskId] = ready.WorkerName;
        runningGrantsByTaskId[ready.TaskId] = grant;
        runningLocksByPlannedTaskId[plannedTask.Id] = plannedTaskLocks.ToArray();
        readyByTaskId.Remove(ready.TaskId);
        pendingTaskIds.Remove(ready.TaskId);
        return new OrchestrationRuntimeGrantIssue(ready, plannedTask, grant);
    }

    public OrchestrationRuntimeTaskCompletion CompleteSucceeded(WorkerProtocolEvent workerEvent, string workerName)
    {
        var plannedTask = RequirePlannedTask(workerEvent.TaskId);
        stateMachine.MarkSucceeded(
            workerEvent.TaskId,
            workerName,
            workerEvent.GrantId,
            workerEvent.CommandId,
            workerEvent.AttemptNumber);
        return ReleaseRunningTask(
            workerEvent,
            plannedTask,
            workerEvent.ExitCode,
            recordTerminalOutcome: true,
            journalEventKind: "TaskSucceeded");
    }

    public OrchestrationRuntimeWorkerReportedFailure ResolveWorkerReportedFailure(
        WorkerProtocolEvent workerEvent,
        string workerName,
        ResolvedOrchestrationRetryPolicy retryPolicy)
    {
        var plannedTask = RequirePlannedTask(workerEvent.TaskId);
        var grant = GetRunningGrant(workerEvent.TaskId);
        var exitCode = workerEvent.ExitCode == 0 ? 4 : workerEvent.ExitCode;
        var attemptNumber = workerEvent.AttemptNumber > 0
            ? workerEvent.AttemptNumber
            : Math.Max(1, grant.AttemptNumber);
        var plannedTaskLocks = locksByPlannedTaskId.TryGetValue(plannedTask.Id, out var locks)
            ? locks
            : [];
        var failureClass = ResolveFailureClass(workerEvent, WorkerFailureClasses.WorkerReportedRetryable);
        var retryDecision = retryPolicy.Evaluate(new OrchestrationRetryEvaluationContext(
            workerEvent.TaskId,
            attemptNumber,
            failureClass,
            retryPolicy.IsTaskRetrySafe(HasWriteEffect(plannedTaskLocks)),
            exitCode,
            workerEvent.Message));

        if (retryDecision.ShouldRetry)
        {
            stateMachine.MarkReadyForSameWorkerRetry(
                workerEvent.TaskId,
                workerName,
                workerEvent.GrantId,
                workerEvent.CommandId,
                workerEvent.AttemptNumber,
                workerEvent.GrantId,
                retryDecision.NextAttemptNumber);
        }
        else
        {
            stateMachine.MarkFailed(
                workerEvent.TaskId,
                workerName,
                workerEvent.GrantId,
                workerEvent.CommandId,
                workerEvent.AttemptNumber);
            stoppedPipelineNames.Add(workerName);
        }

        var completion = ReleaseRunningTask(
            workerEvent with
            {
                ExitCode = exitCode,
                AttemptNumber = attemptNumber
            },
            plannedTask,
            exitCode,
            recordTerminalOutcome: !retryDecision.ShouldRetry,
            journalEventKind: retryDecision.ShouldRetry ? "TaskAttemptFailed" : "TaskFailed");

        if (retryDecision.ShouldRetry)
        {
            readyByTaskId[workerEvent.TaskId] = new OrchestrationRuntimeReadyTask(
                workerName,
                workerEvent.PipelineId,
                workerEvent.PipelineName,
                workerEvent.TaskId,
                workerEvent.TaskName,
                DateTimeOffset.UtcNow + retryDecision.Delay,
                completion.GrantId,
                retryDecision.NextAttemptNumber);
        }

        return new OrchestrationRuntimeWorkerReportedFailure(
            completion,
            retryDecision,
            failureClass);
    }

    public OrchestrationRuntimeSupervisorFailure ResolveSupervisorObservedFailure(
        WorkerProtocolEvent workerEvent,
        string workerName,
        string failureClass,
        string reason,
        ResolvedOrchestrationRetryPolicy retryPolicy)
    {
        var plannedTask = RequirePlannedTask(workerEvent.TaskId);
        var hasGrant = runningGrantsByTaskId.TryGetValue(workerEvent.TaskId, out var grant);
        var attemptNumber = workerEvent.AttemptNumber > 0
            ? workerEvent.AttemptNumber
            : hasGrant ? grant.AttemptNumber : 1;
        var exitCode = workerEvent.ExitCode == 0 ? 4 : workerEvent.ExitCode;
        var plannedTaskLocks = locksByPlannedTaskId.TryGetValue(plannedTask.Id, out var locks)
            ? locks
            : [];
        var retryDecision = retryPolicy.Evaluate(new OrchestrationRetryEvaluationContext(
            workerEvent.TaskId,
            attemptNumber,
            failureClass,
            retryPolicy.IsTaskRetrySafe(HasWriteEffect(plannedTaskLocks)),
            exitCode,
            reason));

        var completion = ReleaseRunningTask(
            workerEvent with
            {
                ExitCode = exitCode,
                GrantId = string.IsNullOrWhiteSpace(workerEvent.GrantId) ? hasGrant ? grant.GrantId : string.Empty : workerEvent.GrantId,
                CommandId = string.IsNullOrWhiteSpace(workerEvent.CommandId) ? hasGrant ? grant.CommandId : string.Empty : workerEvent.CommandId,
                AttemptNumber = attemptNumber,
                Message = reason,
                FailureClass = failureClass
            },
            plannedTask,
            exitCode,
            recordTerminalOutcome: !retryDecision.ShouldRetry,
            journalEventKind: retryDecision.ShouldRetry ? "TaskAttemptFailed" : "TaskFailed");

        if (!retryDecision.ShouldRetry)
        {
            stateMachine.MarkFailedFromSupervisor(workerEvent.TaskId);
            stoppedPipelineNames.Add(workerName);
            var blocked = BlockRemainingPipelineTasks(workerName, reason);
            return OrchestrationRuntimeSupervisorFailure.NoRetry(
                completion,
                retryDecision,
                failureClass,
                blocked);
        }

        stateMachine.MarkRetryScheduledForReplacement(
            workerEvent.TaskId,
            completion.GrantId,
            retryDecision.NextAttemptNumber);
        stateMachine.MarkWorkerClosed(workerName);
        pendingTaskIds.Add(workerEvent.TaskId);
        stateMachine.MarkPendingForReplacement(workerEvent.TaskId);
        scheduledRetryByTaskId[workerEvent.TaskId] = new OrchestrationRuntimeRetryState(
            DateTimeOffset.UtcNow + retryDecision.Delay,
            completion.GrantId,
            retryDecision.NextAttemptNumber);

        return OrchestrationRuntimeSupervisorFailure.Retry(
            completion,
            retryDecision,
            failureClass,
            workerEvent.TaskId,
            completion.GrantId,
            retryDecision.NextAttemptNumber);
    }

    public OrchestrationRuntimeBlockedPipeline BlockRemainingPipelineTasks(
        string pipelineName,
        string reason)
    {
        var blocked = new List<OrchestrationRuntimeBlockedTask>();
        foreach (var taskId in pendingTaskIds
                     .Where(taskId => plannedTasksByPipelineTaskId.TryGetValue(taskId, out var candidate) &&
                                      string.Equals(candidate.PipelineReference.Name, pipelineName, StringComparison.OrdinalIgnoreCase))
                     .ToArray())
        {
            pendingTaskIds.Remove(taskId);
            readyByTaskId.Remove(taskId);
            stateMachine.MarkBlocked(taskId);
            var plannedTask = plannedTasksByPipelineTaskId[taskId];
            blocked.Add(CreateBlockedTask(
                plannedTask,
                OrchestrationExecutionDependency.Empty,
                OrchestrationExecutionContinuity.SkippedBlocked,
                reason));
            taskOutcomesByTaskProfileId[plannedTask.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.SkippedBlocked;
        }

        return new OrchestrationRuntimeBlockedPipeline(pipelineName, string.Empty, string.Empty, reason, blocked);
    }

    public string DescribeAllReadyNoProgress()
    {
        var descriptions = new List<string>();
        foreach (var ready in readyByTaskId.Values
                     .OrderBy(static item => item.PipelineName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(static item => item.TaskName, StringComparer.OrdinalIgnoreCase))
        {
            if (!plannedTasksByPipelineTaskId.TryGetValue(ready.TaskId, out var plannedTask))
            {
                descriptions.Add($"{ready.PipelineName}.{ready.TaskName} is TaskReady for task '{ready.TaskId}', which is not in the run plan");
                continue;
            }

            if (ready.NotBeforeUtc > DateTimeOffset.UtcNow)
            {
                descriptions.Add(
                    $"{FormatTaskName(plannedTask)} waits for retry delay until {ready.NotBeforeUtc:O}");
                continue;
            }

            var readiness = OrchestrationExecutionContinuity.EvaluateReadiness(
                plannedTask,
                dependenciesByTaskProfileId,
                taskOutcomesByTaskProfileId,
                out var dependency,
                out var blockedOutcome,
                out var blockedReason);

            if (readiness == OrchestrationTaskReadiness.Waiting)
            {
                descriptions.Add(
                    $"{FormatTaskName(plannedTask)} waits for {DescribeDependencyState(dependency)}");
                continue;
            }

            if (readiness == OrchestrationTaskReadiness.Skip)
            {
                descriptions.Add(
                    $"{FormatTaskName(plannedTask)} should be blocked as {blockedOutcome}: {blockedReason}");
                continue;
            }

            descriptions.Add($"{FormatTaskName(plannedTask)} is ready but no grant was issued");
        }

        return descriptions.Count == 0
            ? "No TaskReady details were available."
            : "Ready waits: " + string.Join("; ", descriptions.Take(6)) + (descriptions.Count > 6 ? $"; ... {descriptions.Count - 6} more" : string.Empty);
    }

    public Task? CreateRetryWakeTask(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var nextRetryAt = readyByTaskId.Values
            .Where(static item => item.NotBeforeUtc > DateTimeOffset.MinValue)
            .Select(static item => item.NotBeforeUtc)
            .Where(item => item > now)
            .OrderBy(static item => item)
            .FirstOrDefault();
        if (nextRetryAt <= DateTimeOffset.MinValue)
        {
            return null;
        }

        var delay = nextRetryAt - now;
        return delay <= TimeSpan.Zero
            ? Task.CompletedTask
            : Task.Delay(delay, cancellationToken);
    }

    public void AssertProjection(
        string stage,
        IReadOnlySet<string> liveWorkerNames)
    {
        void Fail(string message) =>
            throw new InvalidOperationException(
                $"Orchestration runtime kernel invariant failed at {stage}: {message}");

        foreach (var taskId in pendingTaskIds)
        {
            if (!plannedTasksByPipelineTaskId.ContainsKey(taskId))
            {
                Fail($"pending task '{taskId}' is not present in the run plan.");
            }
        }

        foreach (var taskId in readyByTaskId.Keys)
        {
            if (!plannedTasksByPipelineTaskId.ContainsKey(taskId))
            {
                Fail($"ready task '{taskId}' is not present in the run plan.");
            }
        }

        foreach (var taskId in scheduledRetryByTaskId.Keys)
        {
            if (!plannedTasksByPipelineTaskId.ContainsKey(taskId))
            {
                Fail($"scheduled retry task '{taskId}' is not present in the run plan.");
            }
        }

        foreach (var taskId in runningWorkerNamesByTaskId.Keys)
        {
            if (!plannedTasksByPipelineTaskId.ContainsKey(taskId))
            {
                Fail($"running task '{taskId}' is not present in the run plan.");
            }
        }

        foreach (var taskId in runningGrantsByTaskId.Keys)
        {
            if (!plannedTasksByPipelineTaskId.ContainsKey(taskId))
            {
                Fail($"running grant task '{taskId}' is not present in the run plan.");
            }
        }

        var runningPlannedTaskIds = runningWorkerNamesByTaskId.Keys
            .Select(taskId => plannedTasksByPipelineTaskId[taskId].Id)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var plannedTaskId in runningLocksByPlannedTaskId.Keys)
        {
            if (!runningPlannedTaskIds.Contains(plannedTaskId))
            {
                Fail($"running locks exist for planned task '{plannedTaskId}', but no active grant owns that planned task.");
            }
        }

        foreach (var plannedTask in plannedTasksByPipelineTaskId.Values)
        {
            if (runningWorkerNamesByTaskId.ContainsKey(plannedTask.TaskAccessProfile.MetaPipelinePipelineTaskId) &&
                !runningLocksByPlannedTaskId.ContainsKey(plannedTask.Id))
            {
                Fail($"active task '{plannedTask.TaskAccessProfile.MetaPipelinePipelineTaskId}' has no running lock projection for planned task '{plannedTask.Id}'.");
            }
        }

        var runningOnly = runningWorkerNamesByTaskId.Keys
            .Except(runningGrantsByTaskId.Keys, StringComparer.Ordinal)
            .FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(runningOnly))
        {
            Fail($"task '{runningOnly}' is running without an active grant projection.");
        }

        var grantOnly = runningGrantsByTaskId.Keys
            .Except(runningWorkerNamesByTaskId.Keys, StringComparer.Ordinal)
            .FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(grantOnly))
        {
            Fail($"task '{grantOnly}' has an active grant projection without a running worker projection.");
        }

        foreach (var workerSnapshot in stateMachine.GetWorkerSnapshots())
        {
            if (workerSnapshot.State != OrchestrationWorkerRuntimeState.Closed &&
                !liveWorkerNames.Contains(workerSnapshot.WorkerName))
            {
                Fail($"kernel worker '{workerSnapshot.WorkerName}' is {workerSnapshot.State} but has no live event, ready, or running projection.");
            }
        }

        foreach (var snapshot in stateMachine.GetTaskSnapshots())
        {
            var inPending = pendingTaskIds.Contains(snapshot.TaskId);
            var inReady = readyByTaskId.TryGetValue(snapshot.TaskId, out var ready);
            var inScheduledRetry = scheduledRetryByTaskId.TryGetValue(snapshot.TaskId, out var scheduledRetry);
            var inRunning = runningWorkerNamesByTaskId.TryGetValue(snapshot.TaskId, out var runningWorkerName);
            var inRunningGrant = runningGrantsByTaskId.TryGetValue(snapshot.TaskId, out var runningGrant);

            switch (snapshot.State)
            {
                case OrchestrationTaskRuntimeState.Pending:
                    if (!inPending)
                    {
                        Fail($"kernel task '{snapshot.TaskId}' is Pending but pendingTaskIds does not contain it.");
                    }

                    if (inReady || inRunning || inRunningGrant)
                    {
                        Fail($"kernel task '{snapshot.TaskId}' is Pending but has ready/running projection state.");
                    }

                    if (inScheduledRetry)
                    {
                        AssertScheduledRetryProjection(snapshot, scheduledRetry!, Fail);
                    }

                    break;
                case OrchestrationTaskRuntimeState.Ready:
                    if (!inReady)
                    {
                        Fail($"kernel task '{snapshot.TaskId}' is Ready but readyByTaskId does not contain it.");
                    }

                    if (inRunning || inRunningGrant || inScheduledRetry)
                    {
                        Fail($"kernel task '{snapshot.TaskId}' is Ready but has running or scheduled-retry projection state.");
                    }

                    AssertReadyProjection(snapshot, ready!, Fail);
                    break;
                case OrchestrationTaskRuntimeState.GrantIssued:
                case OrchestrationTaskRuntimeState.GrantAccepted:
                case OrchestrationTaskRuntimeState.Running:
                    if (inPending || inReady || inScheduledRetry)
                    {
                        Fail($"kernel task '{snapshot.TaskId}' has an active grant but is still pending/ready/scheduled in runtime projections.");
                    }

                    if (!inRunning || !inRunningGrant)
                    {
                        Fail($"kernel task '{snapshot.TaskId}' has an active grant but does not have both running and grant projections.");
                    }

                    AssertActiveGrantProjection(snapshot, runningWorkerName!, runningGrant!, Fail);
                    break;
                case OrchestrationTaskRuntimeState.RetryScheduled:
                    if (!inScheduledRetry)
                    {
                        Fail($"kernel task '{snapshot.TaskId}' is RetryScheduled but scheduledRetryByTaskId does not contain it.");
                    }

                    if (inPending || inReady || inRunning || inRunningGrant)
                    {
                        Fail($"kernel task '{snapshot.TaskId}' is RetryScheduled but has pending/ready/running projection state.");
                    }

                    AssertScheduledRetryProjection(snapshot, scheduledRetry!, Fail);
                    break;
                case OrchestrationTaskRuntimeState.Succeeded:
                case OrchestrationTaskRuntimeState.Failed:
                case OrchestrationTaskRuntimeState.Blocked:
                    if (inPending || inReady || inScheduledRetry || inRunning || inRunningGrant)
                    {
                        Fail($"terminal kernel task '{snapshot.TaskId}' still has pending/ready/retry/running projection state.");
                    }

                    break;
                default:
                    Fail($"kernel task '{snapshot.TaskId}' has unknown state {snapshot.State}.");
                    break;
            }
        }
    }

    private OrchestrationRuntimeBlockedPipeline BlockReadyPipeline(
        OrchestrationRuntimeReadyTask ready,
        MO.PlannedTask blockedTask,
        OrchestrationExecutionDependency dependency,
        string blockedOutcome,
        string blockedReason)
    {
        readyByTaskId.Remove(ready.TaskId);
        stoppedPipelineNames.Add(blockedTask.PipelineReference.Name);
        var blocked = new List<OrchestrationRuntimeBlockedTask>();
        foreach (var taskId in pendingTaskIds
                     .Where(taskId => plannedTasksByPipelineTaskId.TryGetValue(taskId, out var candidate) &&
                                      string.Equals(candidate.PipelineReference.Name, blockedTask.PipelineReference.Name, StringComparison.OrdinalIgnoreCase))
                     .ToArray())
        {
            pendingTaskIds.Remove(taskId);
            readyByTaskId.Remove(taskId);
            stateMachine.MarkBlocked(taskId);
            var plannedTask = plannedTasksByPipelineTaskId[taskId];
            var taskDependency = ReferenceEquals(plannedTask, blockedTask)
                ? dependency
                : OrchestrationExecutionDependency.Empty;
            var outcome = ReferenceEquals(plannedTask, blockedTask)
                ? blockedOutcome
                : OrchestrationExecutionContinuity.SkippedBlocked;
            var reason = ReferenceEquals(plannedTask, blockedTask)
                ? blockedReason
                : $"pipeline stopped after blocked task {blockedTask.TaskAccessProfile.TaskName}";
            blocked.Add(CreateBlockedTask(plannedTask, taskDependency, outcome, reason));
            taskOutcomesByTaskProfileId[plannedTask.TaskAccessProfile.Id] = outcome;
        }

        return new OrchestrationRuntimeBlockedPipeline(
            blockedTask.PipelineReference.Name,
            ready.PipelineId,
            ready.TaskId,
            blockedReason,
            blocked);
    }

    private OrchestrationRuntimeTaskCompletion ReleaseRunningTask(
        WorkerProtocolEvent workerEvent,
        MO.PlannedTask plannedTask,
        int exitCode,
        bool recordTerminalOutcome,
        string journalEventKind)
    {
        runningWorkerNamesByTaskId.Remove(workerEvent.TaskId);
        var hadGrant = runningGrantsByTaskId.Remove(workerEvent.TaskId, out var grant);
        runningLocksByPlannedTaskId.Remove(plannedTask.Id);
        if (recordTerminalOutcome)
        {
            taskOutcomesByTaskProfileId[plannedTask.TaskAccessProfile.Id] =
                OrchestrationExecutionContinuity.OutcomeForExitCode(exitCode);
        }

        return new OrchestrationRuntimeTaskCompletion(
            plannedTask,
            workerEvent.TaskId,
            exitCode,
            workerEvent.GrantId,
            workerEvent.CommandId,
            workerEvent.AttemptNumber == 0 ? (hadGrant ? grant.AttemptNumber : 0) : workerEvent.AttemptNumber,
            recordTerminalOutcome,
            journalEventKind);
    }

    private string DescribeDependencyState(OrchestrationExecutionDependency dependency)
    {
        if (string.IsNullOrWhiteSpace(dependency.PredecessorTaskProfileId))
        {
            return "an unknown predecessor";
        }

        if (taskOutcomesByTaskProfileId.TryGetValue(dependency.PredecessorTaskProfileId, out var outcome))
        {
            return $"{dependency.PredecessorTaskProfileId} ({outcome})";
        }

        if (!plannedTasksByProfileId.TryGetValue(dependency.PredecessorTaskProfileId, out var predecessorTask))
        {
            return $"{dependency.PredecessorTaskProfileId} (not present in the run plan)";
        }

        var predecessorPipelineTaskId = predecessorTask.TaskAccessProfile.MetaPipelinePipelineTaskId;
        if (string.IsNullOrWhiteSpace(predecessorPipelineTaskId))
        {
            return $"{FormatTaskName(predecessorTask)} (has no MetaPipeline task id)";
        }

        if (runningWorkerNamesByTaskId.ContainsKey(predecessorPipelineTaskId))
        {
            return $"{FormatTaskName(predecessorTask)} (running)";
        }

        if (readyByTaskId.ContainsKey(predecessorPipelineTaskId))
        {
            return $"{FormatTaskName(predecessorTask)} (also TaskReady and waiting for a command)";
        }

        if (pendingTaskIds.Contains(predecessorPipelineTaskId))
        {
            return $"{FormatTaskName(predecessorTask)} (pending behind a worker boundary)";
        }

        return $"{FormatTaskName(predecessorTask)} (no active worker state can produce it)";
    }

    private MO.PlannedTask RequirePlannedTask(string taskId)
    {
        if (string.IsNullOrWhiteSpace(taskId))
        {
            throw new InvalidOperationException("Worker event did not include a task id.");
        }

        if (!plannedTasksByPipelineTaskId.TryGetValue(taskId, out var plannedTask))
        {
            throw new InvalidOperationException(
                $"Pipeline worker emitted task id '{taskId}' that is not present in the run plan.");
        }

        return plannedTask;
    }

    private OrchestrationRuntimeBlockedTask CreateBlockedTask(
        MO.PlannedTask plannedTask,
        OrchestrationExecutionDependency dependency,
        string outcome,
        string reason)
    {
        var blockingTaskProfileId = dependency.PredecessorTaskProfileId;
        var blockingTask = plannedTasksByProfileId.GetValueOrDefault(blockingTaskProfileId);
        return new OrchestrationRuntimeBlockedTask(
            plannedTask,
            blockingTaskProfileId,
            blockingTask?.PipelineReference.Name ?? "<unknown>",
            blockingTask?.TaskAccessProfile.TaskName ?? blockingTaskProfileId,
            dependency.Condition,
            outcome,
            reason);
    }

    private OrchestrationRuntimeGrant GetRunningGrant(string taskId)
    {
        if (!runningGrantsByTaskId.TryGetValue(taskId, out var grant))
        {
            throw new InvalidOperationException($"Task '{taskId}' has no active grant in the runtime kernel.");
        }

        return grant;
    }

    private static void AssertReadyProjection(
        OrchestrationTaskRuntimeSnapshot snapshot,
        OrchestrationRuntimeReadyTask ready,
        Action<string> fail)
    {
        if (!string.Equals(ready.TaskId, snapshot.TaskId, StringComparison.Ordinal))
        {
            fail($"ready projection key '{snapshot.TaskId}' contains ready task id '{ready.TaskId}'.");
        }

        if (!string.Equals(ready.WorkerName, snapshot.WorkerName, StringComparison.OrdinalIgnoreCase))
        {
            fail($"ready projection for task '{snapshot.TaskId}' is owned by worker '{ready.WorkerName}', but kernel owner is '{snapshot.WorkerName}'.");
        }

        if (snapshot.AttemptNumber > 0 && ready.AttemptNumber != snapshot.AttemptNumber)
        {
            fail($"ready projection for task '{snapshot.TaskId}' has attempt {ready.AttemptNumber.ToString(CultureInfo.InvariantCulture)}, but kernel attempt is {snapshot.AttemptNumber.ToString(CultureInfo.InvariantCulture)}.");
        }

        if (!string.IsNullOrWhiteSpace(snapshot.PreviousGrantId) &&
            !string.Equals(ready.PreviousGrantId, snapshot.PreviousGrantId, StringComparison.Ordinal))
        {
            fail($"ready projection for task '{snapshot.TaskId}' has previous grant '{ready.PreviousGrantId}', but kernel previous grant is '{snapshot.PreviousGrantId}'.");
        }
    }

    private static void AssertScheduledRetryProjection(
        OrchestrationTaskRuntimeSnapshot snapshot,
        OrchestrationRuntimeRetryState scheduledRetry,
        Action<string> fail)
    {
        if (scheduledRetry.AttemptNumber != snapshot.AttemptNumber)
        {
            fail($"scheduled retry for task '{snapshot.TaskId}' has attempt {scheduledRetry.AttemptNumber.ToString(CultureInfo.InvariantCulture)}, but kernel attempt is {snapshot.AttemptNumber.ToString(CultureInfo.InvariantCulture)}.");
        }

        if (!string.Equals(scheduledRetry.PreviousGrantId, snapshot.PreviousGrantId, StringComparison.Ordinal))
        {
            fail($"scheduled retry for task '{snapshot.TaskId}' has previous grant '{scheduledRetry.PreviousGrantId}', but kernel previous grant is '{snapshot.PreviousGrantId}'.");
        }
    }

    private static void AssertActiveGrantProjection(
        OrchestrationTaskRuntimeSnapshot snapshot,
        string runningWorkerName,
        OrchestrationRuntimeGrant runningGrant,
        Action<string> fail)
    {
        if (!string.Equals(runningWorkerName, snapshot.WorkerName, StringComparison.OrdinalIgnoreCase))
        {
            fail($"active task '{snapshot.TaskId}' is running on worker '{runningWorkerName}', but kernel owner is '{snapshot.WorkerName}'.");
        }

        if (!string.Equals(runningGrant.TaskId, snapshot.TaskId, StringComparison.Ordinal))
        {
            fail($"active grant projection key '{snapshot.TaskId}' contains grant task id '{runningGrant.TaskId}'.");
        }

        if (!string.Equals(runningGrant.GrantId, snapshot.GrantId, StringComparison.Ordinal))
        {
            fail($"active grant projection for task '{snapshot.TaskId}' has grant id '{runningGrant.GrantId}', but kernel grant id is '{snapshot.GrantId}'.");
        }

        if (!string.Equals(runningGrant.CommandId, snapshot.CommandId, StringComparison.Ordinal))
        {
            fail($"active grant projection for task '{snapshot.TaskId}' has command id '{runningGrant.CommandId}', but kernel command id is '{snapshot.CommandId}'.");
        }

        if (runningGrant.AttemptNumber != snapshot.AttemptNumber)
        {
            fail($"active grant projection for task '{snapshot.TaskId}' has attempt {runningGrant.AttemptNumber.ToString(CultureInfo.InvariantCulture)}, but kernel attempt is {snapshot.AttemptNumber.ToString(CultureInfo.InvariantCulture)}.");
        }
    }

    private static bool AreLocksCompatibleWithRunning(
        IReadOnlyList<MO.PlannedTaskLock> candidateLocks,
        IReadOnlyList<MO.PlannedTaskLock> runningLocks,
        IReadOnlyList<MO.LockCompatibilityPolicy> activeLockPolicies)
    {
        foreach (var runningLock in runningLocks)
        {
            foreach (var candidateLock in candidateLocks)
            {
                if (!string.Equals(runningLock.DataObject.Id, candidateLock.DataObject.Id, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!ArePlannedTaskLocksCompatible(runningLock, candidateLock, activeLockPolicies))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool ArePlannedTaskLocksCompatible(
        MO.PlannedTaskLock left,
        MO.PlannedTaskLock right,
        IReadOnlyList<MO.LockCompatibilityPolicy> activeLockPolicies)
    {
        if (string.Equals(left.LockMode, "SharedRead", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(right.LockMode, "SharedRead", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var policy = activeLockPolicies
            .Where(item => string.Equals(item.DataObject.Id, left.DataObject.Id, StringComparison.Ordinal))
            .Where(item => EffectsMatch(item, left.TaskObjectEffect.WriteEffect, right.TaskObjectEffect.WriteEffect))
            .OrderBy(static item => item.PolicyKind, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.Id, StringComparer.Ordinal)
            .FirstOrDefault();

        return policy is not null &&
               string.Equals(policy.LockBehavior, "AllowConcurrent", StringComparison.OrdinalIgnoreCase);
    }

    private static bool EffectsMatch(MO.LockCompatibilityPolicy policy, string leftEffect, string rightEffect)
    {
        return
            (string.Equals(policy.LeftEffect, leftEffect, StringComparison.OrdinalIgnoreCase) &&
             string.Equals(policy.RightEffect, rightEffect, StringComparison.OrdinalIgnoreCase)) ||
            (string.Equals(policy.LeftEffect, rightEffect, StringComparison.OrdinalIgnoreCase) &&
             string.Equals(policy.RightEffect, leftEffect, StringComparison.OrdinalIgnoreCase));
    }

    private static bool HasWriteEffect(IReadOnlyList<MO.PlannedTaskLock> plannedTaskLocks)
    {
        return plannedTaskLocks.Any(static item =>
            !string.Equals(item.LockMode, "SharedRead", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(item.TaskObjectEffect.WriteEffect, "None", StringComparison.OrdinalIgnoreCase));
    }

    private static string ResolveFailureClass(WorkerProtocolEvent workerEvent, string fallbackFailureClass) =>
        string.IsNullOrWhiteSpace(workerEvent.FailureClass)
            ? fallbackFailureClass
            : workerEvent.FailureClass.Trim();

    private static string FormatTaskName(MO.PlannedTask plannedTask) =>
        $"{plannedTask.PipelineReference.Name}.{plannedTask.TaskAccessProfile.TaskName}";
}

internal readonly record struct OrchestrationRuntimeReadyTask(
    string WorkerName,
    string PipelineId,
    string PipelineName,
    string TaskId,
    string TaskName,
    DateTimeOffset NotBeforeUtc = default,
    string PreviousGrantId = "",
    int AttemptNumber = 1);

internal readonly record struct OrchestrationRuntimeRetryState(
    DateTimeOffset NotBeforeUtc,
    string PreviousGrantId,
    int AttemptNumber);

internal readonly record struct OrchestrationRuntimeGrant(
    string PipelineId,
    string TaskId,
    string CommandId,
    string GrantId,
    string PreviousGrantId,
    int AttemptNumber)
{
    public static OrchestrationRuntimeGrant Create(
        string pipelineId,
        string taskId,
        string previousGrantId = "",
        int attemptNumber = 1) =>
        new(
            pipelineId,
            taskId,
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid().ToString("N"),
            previousGrantId,
            attemptNumber);
}

internal readonly record struct OrchestrationRuntimeReadyDecision(
    OrchestrationRuntimeReadyDecisionKind Kind,
    OrchestrationRuntimeReadyTask ReadyTask,
    MO.PlannedTask? PlannedTask,
    IReadOnlyList<MO.PlannedTaskLock> PlannedTaskLocks,
    OrchestrationRuntimeGrant Grant,
    OrchestrationWorkerLossDecision WorkerLossDecision,
    OrchestrationRuntimeBlockedPipeline BlockedPipeline)
{
    public static OrchestrationRuntimeReadyDecision None { get; } =
        new(OrchestrationRuntimeReadyDecisionKind.None, default, null, [], default, default, default);

    public static OrchestrationRuntimeReadyDecision IssueGrant(
        OrchestrationRuntimeReadyTask readyTask,
        MO.PlannedTask plannedTask,
        IReadOnlyList<MO.PlannedTaskLock> plannedTaskLocks,
        OrchestrationRuntimeGrant grant) =>
        new(OrchestrationRuntimeReadyDecisionKind.Grant, readyTask, plannedTask, plannedTaskLocks, grant, default, default);

    public static OrchestrationRuntimeReadyDecision Block(OrchestrationRuntimeBlockedPipeline blockedPipeline) =>
        new(OrchestrationRuntimeReadyDecisionKind.Block, default, null, [], default, default, blockedPipeline);

    public static OrchestrationRuntimeReadyDecision ReplaceWorker(
        OrchestrationWorkerLossDecision workerLossDecision,
        OrchestrationRuntimeReadyTask readyTask,
        MO.PlannedTask plannedTask) =>
        new(OrchestrationRuntimeReadyDecisionKind.ReplaceWorker, readyTask, plannedTask, [], default, workerLossDecision, default);
}

internal enum OrchestrationRuntimeReadyDecisionKind
{
    None,
    Grant,
    Block,
    ReplaceWorker
}

internal readonly record struct OrchestrationRuntimeGrantIssue(
    OrchestrationRuntimeReadyTask ReadyTask,
    MO.PlannedTask PlannedTask,
    OrchestrationRuntimeGrant Grant);

internal readonly record struct OrchestrationRuntimeTaskCompletion(
    MO.PlannedTask PlannedTask,
    string TaskId,
    int ExitCode,
    string GrantId,
    string CommandId,
    int AttemptNumber,
    bool RecordTerminalOutcome,
    string JournalEventKind);

internal readonly record struct OrchestrationRuntimeWorkerReportedFailure(
    OrchestrationRuntimeTaskCompletion Completion,
    OrchestrationRetryDecision RetryDecision,
    string FailureClass);

internal readonly record struct OrchestrationRuntimeSupervisorFailure(
    OrchestrationRuntimeTaskCompletion Completion,
    OrchestrationRetryDecision RetryDecision,
    string FailureClass,
    bool ShouldStartReplacementWorker,
    string ResumeTaskId,
    string PreviousGrantId,
    int NextAttemptNumber,
    OrchestrationRuntimeBlockedPipeline BlockedPipeline)
{
    public static OrchestrationRuntimeSupervisorFailure Retry(
        OrchestrationRuntimeTaskCompletion completion,
        OrchestrationRetryDecision retryDecision,
        string failureClass,
        string resumeTaskId,
        string previousGrantId,
        int nextAttemptNumber) =>
        new(completion, retryDecision, failureClass, true, resumeTaskId, previousGrantId, nextAttemptNumber, default);

    public static OrchestrationRuntimeSupervisorFailure NoRetry(
        OrchestrationRuntimeTaskCompletion completion,
        OrchestrationRetryDecision retryDecision,
        string failureClass,
        OrchestrationRuntimeBlockedPipeline blockedPipeline) =>
        new(completion, retryDecision, failureClass, false, string.Empty, string.Empty, 0, blockedPipeline);
}

internal readonly record struct OrchestrationRuntimeBlockedPipeline(
    string PipelineName,
    string PipelineId,
    string BlockingTaskId,
    string Reason,
    IReadOnlyList<OrchestrationRuntimeBlockedTask> BlockedTasks);

internal readonly record struct OrchestrationRuntimeBlockedTask(
    MO.PlannedTask PlannedTask,
    string BlockingTaskProfileId,
    string BlockingPipelineName,
    string BlockingStepName,
    string DependencyCondition,
    string Outcome,
    string Reason);
