using MetaOrchestration.Core;
using MetaOrchestration.WorkerProtocol;

namespace MetaOrchestration.Core.Runtime;

internal sealed class MetaOrchestrationRuntimeKernel : IRuntimeEventSink
{
    private readonly RuntimeState state;
    private readonly ExecutionStateReducer executionReducer;
    private readonly ActivationStateReducer activationReducer;

    public MetaOrchestrationRuntimeKernel(
        RuntimeState state,
        ExecutionStateReducer? executionReducer = null,
        ActivationStateReducer? activationReducer = null)
    {
        this.state = state ?? throw new ArgumentNullException(nameof(state));
        this.executionReducer = executionReducer ?? new ExecutionStateReducer();
        this.activationReducer = activationReducer ?? new ActivationStateReducer();
    }

    public RuntimeSnapshot Snapshot => state.CreateSnapshot();

    public KernelResult RegisterEvent(RuntimeEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var actions = new List<RuntimeAction>();
        switch (@event)
        {
            case RuntimeEvent.SchedulerTick e:
                HandleSchedulerTick(e, actions);
                break;
            case RuntimeEvent.WorkerOnline e:
                HandleWorkerOnline(e, actions);
                break;
            case RuntimeEvent.WorkerReady e:
                HandleWorkerReady(e, actions);
                break;
            case RuntimeEvent.StartPipelineAcknowledged e:
                HandleStartPipelineAcknowledged(e);
                break;
            case RuntimeEvent.PipelineStarted e:
                HandlePipelineStarted(e);
                break;
            case RuntimeEvent.TaskReady e:
                HandleTaskReady(e, actions);
                break;
            case RuntimeEvent.GrantAccepted e:
                HandleGrantAccepted(e);
                break;
            case RuntimeEvent.GrantDeliveryFailed e:
                HandleGrantDeliveryFailed(e, actions);
                break;
            case RuntimeEvent.TaskStarted e:
                HandleTaskStarted(e);
                break;
            case RuntimeEvent.TaskSucceeded e:
                HandleTaskSucceeded(e, actions);
                break;
            case RuntimeEvent.TaskFailed e:
                HandleTaskFailed(e, actions);
                break;
            case RuntimeEvent.WorkerClosed e:
                HandleWorkerClosed(e, actions);
                break;
            case RuntimeEvent.WorkerTimedOut e:
                HandleWorkerTimedOut(e, actions);
                break;
            case RuntimeEvent.SupervisorFailureObserved e:
                HandleSupervisorFailureObserved(e, actions);
                break;
            case RuntimeEvent.PipelineStopRequested e:
                HandlePipelineStopRequested(e, actions);
                break;
            default:
                throw new InvalidOperationException($"Unsupported runtime event '{@event.GetType().Name}'.");
        }

        var snapshot = state.CreateSnapshot();
        actions.Add(new RuntimeAction.PublishSnapshot(snapshot));
        return new KernelResult(actions, snapshot);
    }

    private void HandleSchedulerTick(
        RuntimeEvent.SchedulerTick e,
        ICollection<RuntimeAction> actions)
    {
        TryPromoteDueRetry(e.Now);
        TryIssueNextGrant(e.Now, e.MaxActiveWorkerProcesses, actions);
        if (TryRequestCapacityDeferral(e.MaxActiveWorkerProcesses, actions))
        {
            return;
        }

        var pipeline = state.SelectStartablePipeline(e.MaxActiveWorkerProcesses);
        if (pipeline is null)
        {
            return;
        }

        var transition = activationReducer.Apply(
            state.PipelineActivations.GetState(pipeline.PipelineName),
            ActivationTrigger.SchedulerTick,
            new ActivationFacts(
                HasRemainingPipelineWork: true,
                HasWorkerCapacity: true,
                e.Now));
        var resumeTaskId = state.ConsumeParkedResumeTask(pipeline.PipelineName);
        state.RequestWorkerStart(pipeline, transition, resumeTaskId);
        actions.Add(new RuntimeAction.StartWorker(pipeline.PipelineName, pipeline.PipelineId, resumeTaskId));
    }

    private void HandleWorkerOnline(
        RuntimeEvent.WorkerOnline e,
        ICollection<RuntimeAction> actions)
    {
        _ = actions;
        if (!string.IsNullOrWhiteSpace(state.Definition.ExpectedWorkerExecutableVersion) &&
            !string.Equals(e.ExecutableVersion, state.Definition.ExpectedWorkerExecutableVersion, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{e.WorkerName}' executable version mismatch. Expected '{state.Definition.ExpectedWorkerExecutableVersion}', got '{e.ExecutableVersion}'.");
        }

        var workerTransition = executionReducer.Apply(
            new ExecutionState(
                TaskRuntimeState.None,
                state.WorkerRegistry.GetState(e.WorkerName),
                GrantRuntimeState.None),
            ExecutionTrigger.WorkerOnline,
            CreateFacts(e.WorkerName));
        var activationTransition = activationReducer.Apply(
            state.PipelineActivations.GetState(e.WorkerName),
            ActivationTrigger.WorkerRegistered,
            new ActivationFacts(
                HasRemainingPipelineWork: true,
                HasWorkerCapacity: true,
                DateTimeOffset.UtcNow));
        state.ActivateWorker(e.WorkerName, e.PipelineId, workerTransition, activationTransition);
    }

    private void HandleWorkerReady(
        RuntimeEvent.WorkerReady e,
        ICollection<RuntimeAction> actions)
    {
        var workerTransition = executionReducer.Apply(
            new ExecutionState(
                TaskRuntimeState.None,
                state.WorkerRegistry.GetState(e.WorkerName),
                GrantRuntimeState.None),
            ExecutionTrigger.WorkerReady,
            CreateFacts(e.WorkerName));
        state.ApplyWorkerLifecycle(e.WorkerName, workerTransition);
        var pipeline = state.Definition.RequirePipeline(e.WorkerName);
        actions.Add(new RuntimeAction.SendStartPipeline(e.WorkerName, pipeline.PipelineId, ResumeTaskId: string.Empty));
    }

    private void HandleStartPipelineAcknowledged(RuntimeEvent.StartPipelineAcknowledged e)
    {
        var workerTransition = executionReducer.Apply(
            new ExecutionState(
                TaskRuntimeState.None,
                state.WorkerRegistry.GetState(e.WorkerName),
                GrantRuntimeState.None),
            ExecutionTrigger.StartPipelineAcknowledged,
            CreateFacts(e.WorkerName));
        state.ApplyWorkerLifecycle(e.WorkerName, workerTransition);
    }

    private void HandlePipelineStarted(RuntimeEvent.PipelineStarted e)
    {
        var workerTransition = executionReducer.Apply(
            new ExecutionState(
                TaskRuntimeState.None,
                state.WorkerRegistry.GetState(e.WorkerName),
                GrantRuntimeState.None),
            ExecutionTrigger.PipelineStarted,
            CreateFacts(e.WorkerName));
        state.ApplyWorkerLifecycle(e.WorkerName, workerTransition);
    }

    private void HandleTaskReady(
        RuntimeEvent.TaskReady e,
        ICollection<RuntimeAction> actions)
    {
        _ = actions;
        var task = state.Definition.RequireTask(e.TaskId);
        var workerState = state.WorkerRegistry.GetState(e.WorkerName);
        if (workerState != WorkerRuntimeState.PipelineStarted)
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{e.WorkerName}' emitted {WorkerEventKinds.TaskReady} before {WorkerEventKinds.PipelineStarted}.");
        }

        var taskState = state.TaskLifecycles.GetState(e.TaskId);
        if (taskState != TaskRuntimeState.Pending)
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{e.WorkerName}' emitted {WorkerEventKinds.TaskReady} for task '{e.TaskId}', but the task is not pending. Current state: {taskState}.");
        }

        var transition = executionReducer.Apply(
            new ExecutionState(
                taskState,
                workerState,
                GrantRuntimeState.None),
            ExecutionTrigger.TaskReady,
            CreateFacts(e.WorkerName, e.TaskId));
        state.AcceptTaskReady(task, e.WorkerName, transition);
    }

    private void HandleGrantAccepted(RuntimeEvent.GrantAccepted e)
    {
        var grant = state.RunningGrants.RequireGrant(e.TaskId, e.GrantId, e.CommandId, e.AttemptNumber);
        var task = state.Definition.RequireTask(e.TaskId);
        var transition = executionReducer.Apply(
            new ExecutionState(
                state.TaskLifecycles.GetState(e.TaskId),
                state.WorkerRegistry.GetState(e.WorkerName),
                state.RunningGrants.GetGrantState(e.TaskId)),
            ExecutionTrigger.GrantAccepted,
            CreateFacts(e.WorkerName, e.TaskId, e.GrantId, e.CommandId, e.AttemptNumber));
        state.AcceptGrant(grant, transition);
    }

    private void HandleGrantDeliveryFailed(
        RuntimeEvent.GrantDeliveryFailed e,
        ICollection<RuntimeAction> actions)
    {
        var grant = state.RunningGrants.RequireGrant(e.TaskId, e.GrantId, e.CommandId, e.AttemptNumber);
        var task = state.Definition.RequireTask(e.TaskId);
        var workerTransition = executionReducer.Apply(
            new ExecutionState(
                TaskRuntimeState.None,
                state.WorkerRegistry.GetState(e.WorkerName),
                GrantRuntimeState.None),
            ExecutionTrigger.WorkerClosed,
            CreateFacts(e.WorkerName));
        state.ApplyWorkerLifecycle(e.WorkerName, workerTransition);

        var replacementTransition = executionReducer.Apply(
            new ExecutionState(
                state.TaskLifecycles.GetState(e.TaskId),
                WorkerRuntimeState.PipelineStarted,
                state.RunningGrants.GetGrantState(e.TaskId)),
            ExecutionTrigger.GrantDeliveryFailed,
            CreateFacts(e.WorkerName, e.TaskId, e.GrantId, e.CommandId, e.AttemptNumber));
        state.PrepareGrantDeliveryReplacement(grant, replacementTransition);

        var parked = activationReducer.Apply(
            state.PipelineActivations.GetState(e.WorkerName),
            ActivationTrigger.WorkerReplacementAtResumeBoundary,
            new ActivationFacts(true, true, DateTimeOffset.UtcNow));
        state.ApplyPipelineActivation(e.WorkerName, parked);
        var reservation = state.ReservePreWorkReplacementAttempt(e.WorkerName, e.TaskId, e.Reason);
        actions.Add(new RuntimeAction.WriteJournalEntry(
            "WorkerReplacementReserved",
            e.WorkerName,
            $"ResumeTaskId={reservation.ResumeTaskId}; Attempt={reservation.Attempt}; Limit={reservation.Limit}"));

        var start = activationReducer.Apply(
            state.PipelineActivations.GetState(e.WorkerName),
            ActivationTrigger.SchedulerTick,
            new ActivationFacts(true, true, DateTimeOffset.UtcNow));
        state.RequestWorkerStart(state.Definition.RequirePipeline(e.WorkerName), start, task.TaskId);
        actions.Add(new RuntimeAction.StartWorker(e.WorkerName, task.PipelineId, task.TaskId));
    }

    private void HandleTaskStarted(RuntimeEvent.TaskStarted e)
    {
        var grant = state.RunningGrants.RequireGrant(e.TaskId, e.GrantId, e.CommandId, e.AttemptNumber);
        var transition = executionReducer.Apply(
            new ExecutionState(
                state.TaskLifecycles.GetState(e.TaskId),
                state.WorkerRegistry.GetState(e.WorkerName),
                state.RunningGrants.GetGrantState(e.TaskId)),
            ExecutionTrigger.TaskStarted,
            CreateFacts(e.WorkerName, e.TaskId, e.GrantId, e.CommandId, e.AttemptNumber));
        state.StartGrant(grant, transition);
    }

    private void HandleTaskSucceeded(
        RuntimeEvent.TaskSucceeded e,
        ICollection<RuntimeAction> actions)
    {
        var grant = state.RunningGrants.RequireGrant(e.TaskId, e.GrantId, e.CommandId, e.AttemptNumber);
        var task = state.Definition.RequireTask(e.TaskId);
        var transition = executionReducer.Apply(
            new ExecutionState(
                state.TaskLifecycles.GetState(e.TaskId),
                state.WorkerRegistry.GetState(e.WorkerName),
                state.RunningGrants.GetGrantState(e.TaskId)),
            ExecutionTrigger.TaskSucceeded,
            CreateFacts(e.WorkerName, e.TaskId, e.GrantId, e.CommandId, e.AttemptNumber));
        var completion = state.CompleteGrantSucceeded(grant, task, e.ExitCode, transition);
        actions.Add(new RuntimeAction.RecordTaskCompletion(completion));
    }

    private void HandleTaskFailed(
        RuntimeEvent.TaskFailed e,
        ICollection<RuntimeAction> actions)
    {
        var grant = state.RunningGrants.RequireGrant(e.TaskId, e.GrantId, e.CommandId, e.AttemptNumber);
        var task = state.Definition.RequireTask(e.TaskId);
        var hasWriteEffect = task.LockRequests.Any(static item =>
            !string.Equals(item.Mode, "SharedRead", StringComparison.OrdinalIgnoreCase));
        var retryDecision = state.Definition.RetryPolicy.Evaluate(e.AttemptNumber, e.FailureClass, hasWriteEffect);
        if (retryDecision.ShouldRetry)
        {
            var retryTransition = executionReducer.Apply(
                new ExecutionState(
                    state.TaskLifecycles.GetState(e.TaskId),
                    state.WorkerRegistry.GetState(e.WorkerName),
                    state.RunningGrants.GetGrantState(e.TaskId)),
                ExecutionTrigger.ReplacementRetryScheduled,
                CreateFacts(e.WorkerName, e.TaskId, e.GrantId, e.CommandId, e.AttemptNumber));
            var dueAt = DateTimeOffset.UtcNow + retryDecision.Delay;
            var retry = state.CompleteGrantWithReplacementRetry(
                grant,
                task,
                e.ExitCode == 0 ? 4 : e.ExitCode,
                retryDecision.NextAttemptNumber,
                dueAt,
                retryTransition);
            actions.Add(new RuntimeAction.RecordTaskCompletion(retry.Completion));
            actions.Add(new RuntimeAction.ScheduleRetry(
                retry.Retry.TaskId,
                retry.Retry.WorkerName,
                retry.Retry.AttemptNumber,
                retry.Retry.DueAtUtc,
                retry.Retry.PreviousGrantId));
            return;
        }

        var failedTransition = executionReducer.Apply(
            new ExecutionState(
                state.TaskLifecycles.GetState(e.TaskId),
                state.WorkerRegistry.GetState(e.WorkerName),
                state.RunningGrants.GetGrantState(e.TaskId)),
            ExecutionTrigger.TaskFailed,
            CreateFacts(e.WorkerName, e.TaskId, e.GrantId, e.CommandId, e.AttemptNumber));
        var completion = state.CompleteGrantFailed(grant, task, e.ExitCode, failedTransition);
        actions.Add(new RuntimeAction.RecordTaskCompletion(completion));
        actions.Add(new RuntimeAction.MarkPipelineFailed(
            grant.WorkerName,
            grant.PipelineId,
            grant.TaskId,
            e.FailureClass,
            retryDecision.Reason));
    }

    private void HandleWorkerClosed(
        RuntimeEvent.WorkerClosed e,
        ICollection<RuntimeAction> actions)
    {
        var runningGrant = state.RunningGrants.FindByWorker(e.WorkerName);
        var workerState = state.WorkerRegistry.GetState(e.WorkerName);
        var workerTransition = executionReducer.Apply(
            new ExecutionState(
                TaskRuntimeState.None,
                workerState,
                GrantRuntimeState.None),
            ExecutionTrigger.WorkerClosed,
            CreateFacts(e.WorkerName));
        state.ApplyWorkerLifecycle(e.WorkerName, workerTransition);
        if (runningGrant is null)
        {
            if (TryCompleteCapacityDeferral(e.WorkerName, actions))
            {
                return;
            }

            HandleClosedWorkerWithoutActiveGrant(e.WorkerName, workerState, e.ExitCode, e.Reason, actions);
            return;
        }

        var task = state.Definition.RequireTask(runningGrant.Grant.TaskId);
        var hasWriteEffect = task.LockRequests.Any(static item =>
            !string.Equals(item.Mode, "SharedRead", StringComparison.OrdinalIgnoreCase));
        var retryDecision = state.Definition.RetryPolicy.Evaluate(
            runningGrant.Grant.AttemptNumber,
            WorkerFailureClasses.WorkerCrashBeforeTerminalEvent,
            hasWriteEffect);
        if (retryDecision.ShouldRetry)
        {
            var retryTransition = executionReducer.Apply(
                new ExecutionState(
                    state.TaskLifecycles.GetState(runningGrant.Grant.TaskId),
                    WorkerRuntimeState.PipelineStarted,
                    runningGrant.State),
                ExecutionTrigger.ReplacementRetryScheduled,
                CreateFacts(
                    e.WorkerName,
                    runningGrant.Grant.TaskId,
                    runningGrant.Grant.GrantId,
                    runningGrant.Grant.CommandId,
                    runningGrant.Grant.AttemptNumber));
            var retry = state.CompleteGrantWithReplacementRetry(
                runningGrant.Grant,
                task,
                e.ExitCode == 0 ? 4 : e.ExitCode,
                retryDecision.NextAttemptNumber,
                DateTimeOffset.UtcNow + retryDecision.Delay,
                retryTransition);
            actions.Add(new RuntimeAction.RecordTaskCompletion(retry.Completion));
            actions.Add(new RuntimeAction.ScheduleRetry(
                retry.Retry.TaskId,
                retry.Retry.WorkerName,
                retry.Retry.AttemptNumber,
                retry.Retry.DueAtUtc,
                retry.Retry.PreviousGrantId));

            var parked = activationReducer.Apply(
                state.PipelineActivations.GetState(e.WorkerName),
                ActivationTrigger.WorkerReplacementAtResumeBoundary,
                new ActivationFacts(true, true, DateTimeOffset.UtcNow));
            state.PipelineActivations.ApplyTransition(e.WorkerName, parked.State);
            var replacementTransition = executionReducer.Apply(
                new ExecutionState(
                    state.TaskLifecycles.GetState(runningGrant.Grant.TaskId),
                    WorkerRuntimeState.PipelineStarted,
                    GrantRuntimeState.Released),
                ExecutionTrigger.ReplacementWorkerStarted,
                CreateFacts(e.WorkerName, runningGrant.Grant.TaskId));
            state.PrepareReplacementRetry(retry.Retry, replacementTransition);
            var pipeline = state.Definition.RequirePipeline(e.WorkerName);
            var start = activationReducer.Apply(
                state.PipelineActivations.GetState(e.WorkerName),
                ActivationTrigger.SchedulerTick,
                new ActivationFacts(true, true, DateTimeOffset.UtcNow));
            state.RequestWorkerStart(pipeline, start, runningGrant.Grant.TaskId);
            actions.Add(new RuntimeAction.StartWorker(e.WorkerName, pipeline.PipelineId, runningGrant.Grant.TaskId));
            return;
        }

        var failureTransition = executionReducer.Apply(
            new ExecutionState(
                state.TaskLifecycles.GetState(runningGrant.Grant.TaskId),
                WorkerRuntimeState.PipelineStarted,
                runningGrant.State),
            ExecutionTrigger.SupervisorFailure,
            CreateFacts(
                e.WorkerName,
                runningGrant.Grant.TaskId,
                runningGrant.Grant.GrantId,
                runningGrant.Grant.CommandId,
                runningGrant.Grant.AttemptNumber));
        var completion = state.FailActiveGrantAfterWorkerLoss(runningGrant, task, e.ExitCode == 0 ? 4 : e.ExitCode, failureTransition);
        actions.Add(new RuntimeAction.RecordTaskCompletion(completion));
        actions.Add(new RuntimeAction.MarkPipelineFailed(
            e.WorkerName,
            runningGrant.Grant.PipelineId,
            runningGrant.Grant.TaskId,
            "WorkerClosed",
            e.Reason));
    }

    private void HandleWorkerTimedOut(
        RuntimeEvent.WorkerTimedOut e,
        ICollection<RuntimeAction> actions)
    {
        if (state.RunningGrants.FindByWorker(e.WorkerName) is null)
        {
            var workerState = state.WorkerRegistry.GetState(e.WorkerName);
            throw new InvalidOperationException(
                $"Pipeline worker '{e.WorkerName}' stopped responding before all of its run-plan tasks were resolved. " +
                $"{e.Reason} Expected {DescribeExpectedWorkerEvent(workerState)}.");
        }

        HandleWorkerClosed(new RuntimeEvent.WorkerClosed(e.WorkerName, 4, e.Reason), actions);
    }

    private void HandleClosedWorkerWithoutActiveGrant(
        string workerName,
        WorkerRuntimeState workerState,
        int exitCode,
        string reason,
        ICollection<RuntimeAction> actions)
    {
        if (state.FindReadyWorkByWorker(workerName) is { } ready)
        {
            StartReplacementAtReadyBoundary(workerName, ready, reason, actions);
            return;
        }

        if (!state.PipelineHasUnresolvedWork(workerName))
        {
            return;
        }

        if (state.PipelineHasFailedTask(workerName))
        {
            return;
        }

        if (workerState is WorkerRuntimeState.Online or WorkerRuntimeState.Ready)
        {
            StartReplacementFromBeginning(workerName, reason, actions);
            return;
        }

        throw new InvalidOperationException(
            $"Pipeline worker '{workerName}' exited before all of its run-plan tasks were resolved or exited unexpectedly after resolution. " +
            $"ExitCode: {exitCode.ToString(System.Globalization.CultureInfo.InvariantCulture)}. {reason}");
    }

    private bool TryCompleteCapacityDeferral(
        string workerName,
        ICollection<RuntimeAction> actions)
    {
        if (!state.CapacityDeferrals.TryGetPendingResumeTask(workerName, out _))
        {
            return false;
        }

        var parked = activationReducer.Apply(
            state.PipelineActivations.GetState(workerName),
            ActivationTrigger.CapacityDeferredWorkerClosed,
            new ActivationFacts(true, true, DateTimeOffset.UtcNow));
        state.TryCompleteCapacityDeferral(workerName, parked, out var resumeTaskId);
        actions.Add(new RuntimeAction.WriteJournalEntry(
            "WorkerDeferred",
            workerName,
            $"ResumeTaskId={resumeTaskId}"));
        return true;
    }

    private void StartReplacementFromBeginning(
        string workerName,
        string reason,
        ICollection<RuntimeAction> actions)
    {
        var reset = activationReducer.Apply(
            state.PipelineActivations.GetState(workerName),
            ActivationTrigger.WorkerReplacementFromBeginning,
            new ActivationFacts(true, true, DateTimeOffset.UtcNow));
        state.ApplyPipelineActivation(workerName, reset);
        var reservation = state.ReservePreWorkReplacementAttempt(workerName, string.Empty, reason);
        actions.Add(new RuntimeAction.WriteJournalEntry(
            "WorkerReplacementReserved",
            workerName,
            $"ResumeTaskId={reservation.ResumeTaskId}; Attempt={reservation.Attempt}; Limit={reservation.Limit}"));

        var pipeline = state.Definition.RequirePipeline(workerName);
        var start = activationReducer.Apply(
            state.PipelineActivations.GetState(workerName),
            ActivationTrigger.SchedulerTick,
            new ActivationFacts(true, true, DateTimeOffset.UtcNow));
        state.RequestWorkerStart(pipeline, start);
        actions.Add(new RuntimeAction.StartWorker(workerName, pipeline.PipelineId, ResumeTaskId: string.Empty));
    }

    private void StartReplacementAtReadyBoundary(
        string workerName,
        RuntimeReadyWork ready,
        string reason,
        ICollection<RuntimeAction> actions)
    {
        var task = state.Definition.RequireTask(ready.TaskId);
        var replacementTransition = executionReducer.Apply(
            new ExecutionState(
                state.TaskLifecycles.GetState(ready.TaskId),
                WorkerRuntimeState.PipelineStarted,
                GrantRuntimeState.None),
            ExecutionTrigger.ReadyWorkerLost,
            CreateFacts(workerName, ready.TaskId));
        state.PrepareReadyWorkerReplacement(ready, replacementTransition);

        var parked = activationReducer.Apply(
            state.PipelineActivations.GetState(workerName),
            ActivationTrigger.WorkerReplacementAtResumeBoundary,
            new ActivationFacts(true, true, DateTimeOffset.UtcNow));
        state.ApplyPipelineActivation(workerName, parked);
        var reservation = state.ReservePreWorkReplacementAttempt(workerName, ready.TaskId, reason);
        actions.Add(new RuntimeAction.WriteJournalEntry(
            "WorkerReplacementReserved",
            workerName,
            $"ResumeTaskId={reservation.ResumeTaskId}; Attempt={reservation.Attempt}; Limit={reservation.Limit}"));

        var start = activationReducer.Apply(
            state.PipelineActivations.GetState(workerName),
            ActivationTrigger.SchedulerTick,
            new ActivationFacts(true, true, DateTimeOffset.UtcNow));
        state.RequestWorkerStart(state.Definition.RequirePipeline(workerName), start, task.TaskId);
        actions.Add(new RuntimeAction.StartWorker(workerName, task.PipelineId, task.TaskId));
    }

    private static string DescribeExpectedWorkerEvent(WorkerRuntimeState workerState) =>
        workerState switch
        {
            WorkerRuntimeState.Starting => WorkerEventKinds.WorkerOnline,
            WorkerRuntimeState.Online => WorkerEventKinds.WorkerReady,
            WorkerRuntimeState.Ready => WorkerCommandKinds.StartPipeline,
            WorkerRuntimeState.StartPipelineSent => WorkerEventKinds.PipelineStarted,
            WorkerRuntimeState.PipelineStarted => $"{WorkerEventKinds.TaskReady} or a terminal pipeline event",
            _ => "the next legal worker protocol event"
        };

    private void HandleSupervisorFailureObserved(
        RuntimeEvent.SupervisorFailureObserved e,
        ICollection<RuntimeAction> actions)
    {
        var runningGrant = state.RunningGrants.FindByWorker(e.WorkerName);
        if (runningGrant is null || !string.Equals(runningGrant.Grant.TaskId, e.TaskId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Supervisor failure for task '{e.TaskId}' has no matching active grant.");
        }

        var task = state.Definition.RequireTask(e.TaskId);
        var failureTransition = executionReducer.Apply(
            new ExecutionState(
                state.TaskLifecycles.GetState(e.TaskId),
                state.WorkerRegistry.GetState(e.WorkerName),
                runningGrant.State),
            ExecutionTrigger.SupervisorFailure,
            CreateFacts(
                e.WorkerName,
                e.TaskId,
                runningGrant.Grant.GrantId,
                runningGrant.Grant.CommandId,
                runningGrant.Grant.AttemptNumber));
        var completion = state.FailActiveGrantAfterWorkerLoss(runningGrant, task, e.ExitCode == 0 ? 4 : e.ExitCode, failureTransition);
        actions.Add(new RuntimeAction.RecordTaskCompletion(completion));
        actions.Add(new RuntimeAction.MarkPipelineFailed(
            e.WorkerName,
            runningGrant.Grant.PipelineId,
            runningGrant.Grant.TaskId,
            e.FailureClass,
            e.Reason));
    }

    private void HandlePipelineStopRequested(
        RuntimeEvent.PipelineStopRequested e,
        ICollection<RuntimeAction> actions)
    {
        var pipeline = state.Definition.RequirePipeline(e.PipelineName);
        var transition = activationReducer.Apply(
            state.PipelineActivations.GetState(e.PipelineName),
            ActivationTrigger.PipelineStopped,
            new ActivationFacts(
                HasRemainingPipelineWork: true,
                HasWorkerCapacity: true,
                DateTimeOffset.UtcNow));
        state.StopPipeline(e.PipelineName, transition);
        actions.Add(new RuntimeAction.SendStopPipeline(e.PipelineName, pipeline.PipelineId, string.Empty, e.Reason));
    }

    private void TryPromoteDueRetry(DateTimeOffset now)
    {
        var retry = state.RetrySchedule.FirstDue(now);
        if (retry is null)
        {
            return;
        }

        var task = state.Definition.RequireTask(retry.TaskId);
        var transition = executionReducer.Apply(
            new ExecutionState(
                state.TaskLifecycles.GetState(retry.TaskId),
                state.WorkerRegistry.GetState(retry.WorkerName),
                GrantRuntimeState.Released),
            ExecutionTrigger.RetryDue,
            CreateFacts(retry.WorkerName, retry.TaskId, retry.PreviousGrantId, string.Empty, retry.AttemptNumber));
        state.RetrySchedule.Consume(retry.TaskId);
        state.TaskLifecycles.ApplyTransition(retry.TaskId, transition.State.Task);
        state.ReadyQueue.MarkReady(new RuntimeReadyWork(
            retry.TaskId,
            task.TaskName,
            retry.WorkerName,
            task.PipelineName,
            task.PipelineId,
            retry.AttemptNumber,
            retry.PreviousGrantId,
            DateTimeOffset.MinValue));
        state.AssertTaskHasSingleRuntimeLocation(retry.TaskId);
    }

    private void TryIssueNextGrant(
        DateTimeOffset now,
        int maxDegreeOfParallelism,
        ICollection<RuntimeAction> actions)
    {
        if (state.RunningGrants.Count >= Math.Max(1, maxDegreeOfParallelism))
        {
            return;
        }

        foreach (var ready in state.ReadyQueue.CreateSnapshot())
        {
            if (ready.NotBeforeUtc > now)
            {
                continue;
            }

            var task = state.Definition.RequireTask(ready.TaskId);
            var readiness = EvaluateReadiness(task, out var dependency, out var blockedOutcome, out var blockedReason);
            if (readiness == OrchestrationTaskReadiness.Waiting)
            {
                continue;
            }

            if (readiness == OrchestrationTaskReadiness.Skip)
            {
                var blocked = state.BlockPipelineTasks(
                    task,
                    dependency with
                    {
                        BlockedOutcome = blockedOutcome,
                        BlockedReason = blockedReason
                    },
                    blockedReason);
                actions.Add(new RuntimeAction.RecordBlockedTasks(
                    task.PipelineName,
                    task.PipelineId,
                    task.TaskId,
                    blockedReason,
                    blocked));
                actions.Add(new RuntimeAction.SendStopPipeline(task.PipelineName, task.PipelineId, task.TaskId, blockedReason));
                return;
            }

            if (!state.RuntimeLocks.CanAcquire(task.LockRequests))
            {
                continue;
            }

            var grant = state.CreateGrant(ready);
            var transition = executionReducer.Apply(
                new ExecutionState(
                    state.TaskLifecycles.GetState(ready.TaskId),
                    state.WorkerRegistry.GetState(ready.WorkerName),
                    GrantRuntimeState.None),
                ExecutionTrigger.GrantIssued,
                CreateFacts(ready.WorkerName, ready.TaskId, grant.GrantId, grant.CommandId, grant.AttemptNumber));
            state.IssueGrantFromReady(ready, task, grant, transition);
            actions.Add(new RuntimeAction.IssueGrant(ready.WorkerName, ready.TaskId, task.TaskName, grant));
            return;
        }
    }

    private bool TryRequestCapacityDeferral(
        int maxActiveWorkerProcesses,
        ICollection<RuntimeAction> actions)
    {
        if (state.WorkerRegistry.LiveCount < Math.Max(1, maxActiveWorkerProcesses) ||
            state.SelectStartablePipelineIgnoringCapacity() is null)
        {
            return false;
        }

        foreach (var ready in state.ReadyQueue.CreateSnapshot())
        {
            var task = state.Definition.RequireTask(ready.TaskId);
            if (EvaluateReadiness(task, out _, out _, out _) != OrchestrationTaskReadiness.Waiting)
            {
                continue;
            }

            var taskTransition = executionReducer.Apply(
                new ExecutionState(
                    state.TaskLifecycles.GetState(ready.TaskId),
                    WorkerRuntimeState.PipelineStarted,
                    GrantRuntimeState.None),
                ExecutionTrigger.ReadyWorkerLost,
                CreateFacts(ready.WorkerName, ready.TaskId));
            var activationTransition = activationReducer.Apply(
                state.PipelineActivations.GetState(ready.PipelineName),
                ActivationTrigger.CapacityDeferralRequested,
                new ActivationFacts(true, true, DateTimeOffset.UtcNow));
            state.RequestCapacityDeferral(ready, taskTransition, activationTransition);

            var reason = "Deferred by orchestration to honor max active worker process capacity.";
            actions.Add(new RuntimeAction.WriteJournalEntry(
                "WorkerDeferredForCapacity",
                ready.WorkerName,
                reason));
            actions.Add(new RuntimeAction.SendStopPipeline(
                ready.PipelineName,
                ready.PipelineId,
                ready.TaskId,
                reason));
            return true;
        }

        return false;
    }

    private OrchestrationTaskReadiness EvaluateReadiness(
        RuntimeTaskDefinition task,
        out RuntimeDependency dependency,
        out string blockedOutcome,
        out string reason)
    {
        dependency = new RuntimeDependency(task.TaskId, task.TaskAccessProfileId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        blockedOutcome = string.Empty;
        reason = string.Empty;
        foreach (var candidate in state.Definition.Dependencies.Where(item =>
                     string.Equals(item.TaskId, task.TaskId, StringComparison.Ordinal)))
        {
            if (!state.PipelineOutcomes.TryGetOutcomeByTaskAccessProfileId(candidate.PredecessorTaskAccessProfileId, out var predecessorOutcome))
            {
                dependency = candidate;
                reason = "predecessor has not completed";
                return OrchestrationTaskReadiness.Waiting;
            }

            if (IsDependencySatisfied(candidate.Condition, predecessorOutcome.Outcome))
            {
                continue;
            }

            dependency = candidate;
            blockedOutcome = ResolveBlockedOutcome(candidate.Condition, predecessorOutcome.Outcome);
            reason = $"{candidate.Condition} dependency was not satisfied by predecessor outcome {predecessorOutcome.Outcome}";
            return OrchestrationTaskReadiness.Skip;
        }

        return OrchestrationTaskReadiness.Ready;
    }

    private static bool IsDependencySatisfied(
        string condition,
        string predecessorOutcome) =>
        string.Equals(condition, OrchestrationExecutionContinuity.OnSuccess, StringComparison.OrdinalIgnoreCase)
            ? string.Equals(predecessorOutcome, OrchestrationExecutionContinuity.Succeeded, StringComparison.OrdinalIgnoreCase)
            : string.Equals(condition, OrchestrationExecutionContinuity.OnFailure, StringComparison.OrdinalIgnoreCase) &&
              string.Equals(predecessorOutcome, OrchestrationExecutionContinuity.Failed, StringComparison.OrdinalIgnoreCase);

    private static string ResolveBlockedOutcome(
        string condition,
        string predecessorOutcome)
    {
        if (string.Equals(predecessorOutcome, OrchestrationExecutionContinuity.SkippedBlocked, StringComparison.OrdinalIgnoreCase))
        {
            return OrchestrationExecutionContinuity.SkippedBlocked;
        }

        if (string.Equals(predecessorOutcome, OrchestrationExecutionContinuity.SkippedConditionNotMet, StringComparison.OrdinalIgnoreCase))
        {
            return OrchestrationExecutionContinuity.SkippedConditionNotMet;
        }

        return string.Equals(condition, OrchestrationExecutionContinuity.OnFailure, StringComparison.OrdinalIgnoreCase)
            ? OrchestrationExecutionContinuity.SkippedConditionNotMet
            : OrchestrationExecutionContinuity.SkippedBlocked;
    }

    private static ExecutionFacts CreateFacts(
        string workerName,
        string taskId = "",
        string grantId = "",
        string commandId = "",
        int attemptNumber = 0) =>
        new(taskId, workerName, grantId, commandId, attemptNumber);
}
