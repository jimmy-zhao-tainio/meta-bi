using System.Diagnostics;
using System.Globalization;
using System.IO.Pipes;
using MetaOrchestration.WorkerProtocol;
using MO = MetaOrchestration;

namespace MetaOrchestration.Core;

public sealed class MetaOrchestrationRuntimeService
{
    public async Task<OrchestrationRuntimeResult> ExecuteAsync(
        OrchestrationRuntimeRequest request,
        IOrchestrationRuntimeObserver? observer = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TransformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.BindingWorkspacePath);
        if (request.MaxDegreeOfParallelism <= 0)
        {
            throw new ArgumentException("MaxDegreeOfParallelism must be a positive integer.", nameof(request));
        }
        if (request.WorkerEventTimeout is { } workerEventTimeout && workerEventTimeout < TimeSpan.Zero)
        {
            throw new ArgumentException("WorkerEventTimeout must be non-negative when provided. Use zero for no timeout.", nameof(request));
        }
        if (request.WorkerActivationTimeout is { } workerActivationTimeout && workerActivationTimeout < TimeSpan.Zero)
        {
            throw new ArgumentException("WorkerActivationTimeout must be non-negative when provided. Use zero for no timeout.", nameof(request));
        }
        if (request.WorkerControlPipeConnectTimeout is { } workerControlPipeConnectTimeout && workerControlPipeConnectTimeout < TimeSpan.Zero)
        {
            throw new ArgumentException("WorkerControlPipeConnectTimeout must be non-negative when provided. Use zero for no timeout.", nameof(request));
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PipelineExecutableName);

        if (!string.IsNullOrWhiteSpace(request.PipelineDbConnectionEnvironmentVariableName) &&
            string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(request.PipelineDbConnectionEnvironmentVariableName)))
        {
            throw new InvalidOperationException(
                $"Connection environment variable '{request.PipelineDbConnectionEnvironmentVariableName}' was not found.");
        }

        request = NormalizeRequestPaths(request);
        var workspacePath = request.WorkspacePath;
        if (!string.IsNullOrWhiteSpace(request.RunArtifactsRootPath) &&
            IsSameOrChildPath(request.RunArtifactsRootPath, workspacePath))
        {
            throw new ArgumentException("RunArtifactsRootPath must not be the orchestration workspace or a child of it.", nameof(request));
        }

        var runId = Guid.NewGuid();
        var journal = OrchestrationRunJournal.Start(runId, request, workspacePath);
        var supervisorState = new OrchestrationSupervisorRunState(runId, workspacePath);
        using var signalScope = OrchestrationSupervisorSignalScope.Register(journal, supervisorState);
        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            signalScope.CancellationToken);
        var executionCancellationToken = linkedCancellation.Token;

        try
        {
            using var lease = OrchestrationWorkspaceExecutionLease.Acquire(workspacePath, runId, request.RunArtifactsRootPath);
            journal.WriteEvent("LeaseAcquired", workspacePath, lease.LeaseRecordPath);

            observer?.PhaseChanged("Loading");
            supervisorState.SetPhase("Loading");
            journal.WriteEvent("Phase", "Loading", workspacePath);
            var model = MO.MetaOrchestrationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            observer?.PhaseChanged("Building");
            supervisorState.SetPhase("Building");
            journal.WriteEvent("Phase", "Building", workspacePath);
            new MetaOrchestrationRunPlanningService().BuildRunPlan(model);
            observer?.PhaseChanged("Saving");
            supervisorState.SetPhase("Saving");
            journal.WriteEvent("Phase", "Saving", workspacePath);
            model.SaveToXmlWorkspace(workspacePath);

            var runPlan = ResolveRunPlan(model);
            supervisorState.SetRunPlan(runPlan.Name, 0);
            if (!string.Equals(runPlan.RunPlanStatus, "Ready", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Run plan '{runPlan.Name}' is not ready. RunPlanStatus: {runPlan.RunPlanStatus}");
            }

            var retryPolicy = ResolvedOrchestrationRetryPolicy.FromRunPlan(model, runPlan);
            journal.WriteEvent(
                "RetryPolicyResolved",
                retryPolicy.Name,
                $"MaxAttempts={retryPolicy.MaxAttempts.ToString(CultureInfo.InvariantCulture)}; RetryableFailureClasses={retryPolicy.RetryableFailureClasses.Count.ToString(CultureInfo.InvariantCulture)}");

            var plannedTasks = model.PlannedTaskList
                .Where(item => ReferenceEquals(item.RunPlan, runPlan))
                .OrderBy(static item => ParseOrdinal(item.Ordinal))
                .ThenBy(static item => item.Id, StringComparer.Ordinal)
                .ToArray();

            if (plannedTasks.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Run plan '{runPlan.Name}' has no planned tasks.");
            }
            supervisorState.SetRunPlan(runPlan.Name, plannedTasks.Length);

            observer?.RunPlanReady(plannedTasks.Length);
            journal.WriteEvent("RunPlanReady", runPlan.Name, plannedTasks.Length.ToString(CultureInfo.InvariantCulture));
            var taskResults = new List<OrchestrationTaskWorkerResult>();
            var blockedResults = new List<OrchestrationTaskBlockedResult>();
            var dependenciesByTaskProfileId = OrchestrationExecutionContinuity.BuildDependencyMap(model);
            var plannedTasksByProfileId = plannedTasks
                .GroupBy(static item => item.TaskAccessProfile.Id, StringComparer.Ordinal)
                .ToDictionary(
                    static group => group.Key,
                    static group => group.OrderBy(static item => item.Id, StringComparer.Ordinal).First(),
                    StringComparer.Ordinal);

            var locksByPlannedTaskId = model.PlannedTaskLockList
                .Where(item => plannedTasks.Any(task => ReferenceEquals(item.PlannedTask, task)))
                .GroupBy(static item => item.PlannedTask.Id, StringComparer.Ordinal)
                .ToDictionary(
                    static group => group.Key,
                    static group => group.OrderBy(static item => item.DataObject.NormalizedKey, StringComparer.OrdinalIgnoreCase).ToArray(),
                    StringComparer.Ordinal);
            var activeLockPolicies = model.LockCompatibilityPolicyList
                .Where(static item => IsActive(item.Status))
                .ToArray();

            supervisorState.SetPhase("Executing");
            var taskOutcomesByTaskProfileId = await ExecuteWorkerGraphAsync(
                plannedTasks,
                locksByPlannedTaskId,
                activeLockPolicies,
                dependenciesByTaskProfileId,
                plannedTasksByProfileId,
                taskResults,
                blockedResults,
                retryPolicy,
                request,
                observer,
                journal,
                supervisorState,
                executionCancellationToken).ConfigureAwait(false);

            var hasFailure = taskOutcomesByTaskProfileId.Values.Any(OrchestrationExecutionContinuity.IsFailureOutcome) ||
                             blockedResults.Any(static item => string.Equals(item.Outcome, OrchestrationExecutionContinuity.SkippedBlocked, StringComparison.Ordinal));
            var result = new OrchestrationRuntimeResult(
                runPlan.Name,
                !hasFailure,
                taskResults,
                blockedResults,
                runId,
                journal.RunDirectoryPath);
            journal.WriteEvent("RunCompleted", runPlan.Name, result.Succeeded ? "Succeeded" : "Failed");
            signalScope.MarkCompleted();
            return result;
        }
        catch (Exception ex)
        {
            supervisorState.SetPhase(ex is OperationCanceledException ? "Cancelled" : "Failed");
            journal.WriteException("SupervisorException", ex);
            journal.WriteEvent("SupervisorState", "exception", supervisorState.Describe());
            journal.WriteEvent("RunFailed", ex.GetType().Name, ex.Message);
            signalScope.MarkCompleted();
            throw;
        }
    }

    private static async Task<IReadOnlyDictionary<string, string>> ExecuteWorkerGraphAsync(
        IReadOnlyList<MO.PlannedTask> plannedTasks,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]> locksByPlannedTaskId,
        IReadOnlyList<MO.LockCompatibilityPolicy> activeLockPolicies,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependenciesByTaskProfileId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        ResolvedOrchestrationRetryPolicy retryPolicy,
        OrchestrationRuntimeRequest request,
        IOrchestrationRuntimeObserver? observer,
        OrchestrationRunJournal journal,
        OrchestrationSupervisorRunState supervisorState,
        CancellationToken cancellationToken)
    {
        var workerEventTimeout = NormalizeOptionalTimeout(request.WorkerEventTimeout);
        var workerActivationTimeout = ResolveWorkerActivationTimeout(request.WorkerActivationTimeout, workerEventTimeout);
        var workerControlPipeConnectTimeout = NormalizeOptionalTimeout(request.WorkerControlPipeConnectTimeout);
        var plannedTasksByPipelineTaskId = plannedTasks
            .Where(static item => !string.IsNullOrWhiteSpace(item.TaskAccessProfile.MetaPipelinePipelineTaskId))
            .GroupBy(static item => item.TaskAccessProfile.MetaPipelinePipelineTaskId, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group.OrderBy(static item => item.Id, StringComparer.Ordinal).First(),
                StringComparer.Ordinal);
        if (plannedTasksByPipelineTaskId.Count != plannedTasks.Count)
        {
            throw new InvalidOperationException("Cannot execute run plan because at least one planned task has no MetaPipelinePipelineTaskId.");
        }

        var kernel = new OrchestrationRuntimeKernel(
            plannedTasksByPipelineTaskId,
            plannedTasksByProfileId,
            locksByPlannedTaskId,
            activeLockPolicies,
            dependenciesByTaskProfileId);
        supervisorState.SetRuntimeCounts(
            kernel.PendingCount,
            kernel.ReadyCount,
            kernel.RunningCount,
            kernel.ScheduledRetryByTaskId.Count,
            taskResults.Count,
            blockedResults.Count);
        var workers = new List<PipelineWorkerProcess>();
        var workersByName = new Dictionary<string, PipelineWorkerProcess>(StringComparer.OrdinalIgnoreCase);
        var eventTasksByWorker = new Dictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>>();
        try
        {
            void UpdateSupervisorState(string eventKind, string subject = "")
            {
                supervisorState.SetRuntimeCounts(
                    kernel.PendingCount,
                    kernel.ReadyCount,
                    kernel.RunningCount,
                    kernel.ScheduledRetryByTaskId.Count,
                    taskResults.Count,
                    blockedResults.Count);
                supervisorState.SetLiveWorkers(eventTasksByWorker.Keys.Select(static worker => worker.PipelineName));
                supervisorState.NoteEvent(eventKind, subject);
            }

            void AssertProjection(string stage) => kernel.AssertProjection(
                stage,
                ResolveLiveWorkerNames(eventTasksByWorker, kernel));

            IReadOnlySet<string> ResolveLiveWorkerProcessNames() =>
                eventTasksByWorker.Keys
                    .Where(static worker => !worker.HasExited)
                    .Select(static worker => worker.PipelineName)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

            async Task StartPipelineWorkerAsync(OrchestrationRuntimeWorkerActivationDecision activationDecision)
            {
                var pipelineName = activationDecision.PipelineName;
                var resumeTaskId = activationDecision.ResumeTaskId ?? string.Empty;
                journal.WriteEvent(
                    string.IsNullOrWhiteSpace(resumeTaskId) ? "WorkerStarting" : "WorkerResuming",
                    pipelineName,
                    string.IsNullOrWhiteSpace(resumeTaskId)
                        ? request.PipelineExecutableName
                        : $"ResumeTaskId={resumeTaskId}; Executable={request.PipelineExecutableName}");
                var worker = await PipelineWorkerProcess.StartAsync(
                    pipelineName,
                    activationDecision.PipelineId,
                    resumeTaskId,
                    request,
                    journal.RunDirectoryPath,
                    workerControlPipeConnectTimeout,
                    cancellationToken).ConfigureAwait(false);
                kernel.RegisterWorker(
                    worker.PipelineName,
                    worker.PipelineId,
                    worker.ResumeTaskId,
                    worker.ExpectedExecutableVersion);
                journal.WriteEvent("WorkerStarted", pipelineName, worker.ProcessId.ToString(CultureInfo.InvariantCulture));
                journal.WriteEvent("WorkerControlPipe", pipelineName, worker.ControlPipeName);
                journal.WriteEvent("WorkerLog", pipelineName, $"stdout={worker.StandardOutputArtifactPath}; stderr={worker.StandardErrorArtifactPath}");
                workers.Add(worker);
                workersByName[worker.PipelineName] = worker;
                eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
            }

            async Task<bool> TryPerformWorkerActivationActionAsync()
            {
                var acted = false;
                while (true)
                {
                    var activationDecision = kernel.ChooseWorkerActivationAction(
                        ResolveLiveWorkerProcessNames(),
                        request.MaxDegreeOfParallelism,
                        DateTimeOffset.UtcNow);
                    switch (activationDecision.Kind)
                    {
                        case OrchestrationRuntimeWorkerActivationDecisionKind.None:
                            return acted;
                        case OrchestrationRuntimeWorkerActivationDecisionKind.StartWorker:
                            await StartPipelineWorkerAsync(activationDecision).ConfigureAwait(false);
                            acted = true;
                            UpdateSupervisorState("WorkerActivated", activationDecision.PipelineName);
                            AssertProjection("after worker activation");
                            continue;
                        case OrchestrationRuntimeWorkerActivationDecisionKind.DeferWorkerForCapacity:
                            if (!workersByName.TryGetValue(activationDecision.WorkerName, out var worker) || worker.HasExited)
                            {
                                journal.WriteEvent(
                                    "WorkerDeferCommandLost",
                                    activationDecision.WorkerName,
                                    "worker process was no longer live");
                                kernel.CancelWorkerCapacityDeferralRequested(activationDecision);
                                return acted;
                            }

                            journal.WriteEvent(
                                "WorkerDeferredForCapacity",
                                activationDecision.WorkerName,
                                activationDecision.Reason);
                            try
                            {
                                await worker.SendStopPipelineAsync(
                                    activationDecision.PipelineId,
                                    activationDecision.TaskId,
                                    "Deferred by orchestration to honor max active worker process capacity.").ConfigureAwait(false);
                            }
                            catch (InvalidOperationException ex)
                            {
                                journal.WriteEvent("WorkerDeferCommandLost", activationDecision.WorkerName, ex.Message);
                                kernel.CancelWorkerCapacityDeferralRequested(activationDecision);
                                return acted;
                            }

                            kernel.CommitWorkerCapacityDeferralRequested(activationDecision);
                            return true;
                        default:
                            throw new InvalidOperationException(
                                $"Unsupported worker activation decision '{activationDecision.Kind}'.");
                    }
                }
            }

            async Task ProcessWorkerEventTaskAsync(Task completedEventTask)
            {
                var worker = eventTasksByWorker.First(item => ReferenceEquals(item.Value, completedEventTask)).Key;
                eventTasksByWorker.Remove(worker);
                var workerEvent = await ((Task<WorkerProtocolEvent>)completedEventTask).ConfigureAwait(false);
                worker.MarkProtocolEventObservedBySupervisor();
                UpdateSupervisorState(workerEvent.Kind, worker.PipelineName);
                journal.WriteEvent(
                    "WorkerEvent",
                    string.IsNullOrWhiteSpace(workerEvent.TaskName)
                        ? worker.PipelineName
                        : $"{worker.PipelineName}.{workerEvent.TaskName}",
                    $"{workerEvent.Kind}; ExitCode={workerEvent.ExitCode.ToString(CultureInfo.InvariantCulture)}; {workerEvent.Message}");

                if (string.Equals(workerEvent.Kind, WorkerEventKinds.Closed, StringComparison.OrdinalIgnoreCase))
                {
                    if (kernel.TryApplyCapacityDeferredWorkerClosed(
                            worker.PipelineName,
                            workerEvent.ExitCode,
                            out var deferredWorker))
                    {
                        journal.WriteEvent(
                            "WorkerDeferred",
                            deferredWorker.WorkerName,
                            $"ResumeTaskId={deferredWorker.ResumeTaskId}; Task={deferredWorker.TaskName}");
                        UpdateSupervisorState("WorkerDeferred", deferredWorker.WorkerName);
                        AssertProjection("after worker deferred");
                        return;
                    }

                    await HandleClosedWorkerAsync(
                        worker,
                        workerEvent,
                        eventTasksByWorker,
                        workers,
                        workersByName,
                        retryPolicy,
                        kernel,
                        plannedTasksByPipelineTaskId,
                        taskResults,
                        blockedResults,
                        request,
                        journal,
                        observer,
                        cancellationToken).ConfigureAwait(false);
                    UpdateSupervisorState("WorkerClosedHandled", worker.PipelineName);
                    AssertProjection("after worker closed");
                    return;
                }

                if (string.Equals(workerEvent.Kind, WorkerEventKinds.ProtocolFault, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"Pipeline worker '{worker.PipelineName}' emitted a malformed protocol event. {workerEvent.Message}");
                }

                if (await HandleWorkerLifecycleEventAsync(
                        worker,
                        workerEvent,
                        eventTasksByWorker,
                        workers,
                        workersByName,
                        kernel,
                        request,
                        journal,
                        cancellationToken).ConfigureAwait(false))
                {
                    UpdateSupervisorState("WorkerLifecycleHandled", worker.PipelineName);
                    AssertProjection("after worker lifecycle event");
                    return;
                }

                if (!plannedTasksByPipelineTaskId.TryGetValue(workerEvent.TaskId, out var plannedTask))
                {
                    throw new InvalidOperationException(
                        $"Pipeline worker '{worker.PipelineName}' emitted task id '{workerEvent.TaskId}' that is not present in the run plan.");
                }

                switch (workerEvent.Kind)
                {
                    case WorkerEventKinds.TaskReady:
                        kernel.AddReady(workerEvent, worker.PipelineName);
                        eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
                        break;
                    case WorkerEventKinds.GrantAccepted:
                        kernel.MarkGrantAccepted(workerEvent, worker.PipelineName);
                        eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
                        break;
                    case WorkerEventKinds.TaskStarted:
                        kernel.MarkTaskStarted(workerEvent, worker.PipelineName);
                        eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
                        break;
                    case WorkerEventKinds.TaskSucceeded:
                        RecordTaskCompletion(
                            worker,
                            workerEvent,
                            kernel.CompleteSucceeded(workerEvent, worker.PipelineName),
                            taskResults,
                            journal,
                            observer);
                        eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
                        break;
                    case WorkerEventKinds.TaskFailed:
                        await HandleTaskFailedAsync(
                            worker,
                            workerEvent,
                            retryPolicy,
                            kernel,
                            taskResults,
                            journal,
                            observer,
                            cancellationToken).ConfigureAwait(false);
                        eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Pipeline worker '{worker.PipelineName}' emitted unsupported event '{workerEvent.Kind}'.");
                }

                UpdateSupervisorState($"TaskEventHandled:{workerEvent.Kind}", worker.PipelineName);
                AssertProjection($"after task event {workerEvent.Kind}");
            }

            await TryPerformWorkerActivationActionAsync().ConfigureAwait(false);
            UpdateSupervisorState("WorkersActivated");
            AssertProjection("after initial worker activation");

            while (kernel.HasRuntimeWork || eventTasksByWorker.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                UpdateSupervisorState("LoopStart");
                AssertProjection("loop start");
                var completedWorkerEventTask = eventTasksByWorker.Values.FirstOrDefault(static task => task.IsCompleted);
                if (completedWorkerEventTask is not null)
                {
                    await ProcessWorkerEventTaskAsync(completedWorkerEventTask).ConfigureAwait(false);
                    continue;
                }

                if (await TryGrantReadyWorkerTaskAsync(
                    kernel,
                    eventTasksByWorker,
                    workers,
                    workersByName,
                    blockedResults,
                    request,
                    observer,
                    journal,
                    cancellationToken).ConfigureAwait(false))
                {
                    UpdateSupervisorState("GrantOrBlock");
                    AssertProjection("after grant or block");
                    continue;
                }

                if (await TryPerformWorkerActivationActionAsync().ConfigureAwait(false))
                {
                    UpdateSupervisorState("WorkerActivated");
                    AssertProjection("after worker activation");
                    continue;
                }

                if (eventTasksByWorker.Count == 0)
                {
                    if (kernel.HasRuntimeWork)
                    {
                        UpdateSupervisorState("NoWorkersWithRuntimeWork");
                        throw new InvalidOperationException("Cannot execute run plan because no pipeline worker can produce the remaining task events.");
                    }

                    break;
                }

                if (kernel.RunningCount == 0 &&
                    eventTasksByWorker.Values.All(static task => !task.IsCompleted) &&
                    eventTasksByWorker.Keys.All(worker => !worker.HasExited && kernel.ReadyByTaskId.Values.Any(ready => string.Equals(ready.WorkerName, worker.PipelineName, StringComparison.OrdinalIgnoreCase))) &&
                    !kernel.HasInactivePipelineThatCanProgress(DateTimeOffset.UtcNow) &&
                    !kernel.ReadyByTaskId.Values.Any(static ready => ready.NotBeforeUtc > DateTimeOffset.UtcNow))
                {
                    var detail = kernel.DescribeAllReadyNoProgress();
                    UpdateSupervisorState("NoProgress", "all-workers-ready");
                    journal.WriteEvent("NoProgress", "all-workers-ready", detail);
                    throw new InvalidOperationException(
                        "Cannot execute run plan because every live pipeline worker is waiting for an orchestration command, but no ready task can be granted. " + detail);
                }

                var timedOutWorker = FindTimedOutWorker(
                    eventTasksByWorker,
                    kernel,
                    workerEventTimeout,
                    workerActivationTimeout,
                    DateTimeOffset.UtcNow);
                if (timedOutWorker is not null)
                {
                    eventTasksByWorker.Remove(timedOutWorker);
                    UpdateSupervisorState("WorkerTimeout", timedOutWorker.PipelineName);
                    await HandleTimedOutWorkerAsync(
                        timedOutWorker,
                        workerEventTimeout,
                        workerActivationTimeout,
                        eventTasksByWorker,
                        workers,
                        workersByName,
                        retryPolicy,
                        kernel,
                        plannedTasksByPipelineTaskId,
                        taskResults,
                        blockedResults,
                        request,
                        journal,
                        observer,
                        cancellationToken).ConfigureAwait(false);
                    UpdateSupervisorState("WorkerTimeoutHandled", timedOutWorker.PipelineName);
                    AssertProjection("after worker timeout");
                    continue;
                }

                var retryWakeTask = kernel.CreateRetryWakeTask(cancellationToken);
                var timeoutWakeTask = CreateWorkerEventTimeoutWakeTask(
                    eventTasksByWorker,
                    kernel,
                    workerEventTimeout,
                    workerActivationTimeout,
                    cancellationToken);
                var waitTasks = eventTasksByWorker.Values.Cast<Task>();
                if (retryWakeTask is not null)
                {
                    waitTasks = waitTasks.Append(retryWakeTask);
                }
                if (timeoutWakeTask is not null)
                {
                    waitTasks = waitTasks.Append(timeoutWakeTask);
                }

                Task completedEventTask = await Task.WhenAny(waitTasks).ConfigureAwait(false);
                if (ReferenceEquals(completedEventTask, retryWakeTask))
                {
                    UpdateSupervisorState("RetryWake");
                    continue;
                }
                if (ReferenceEquals(completedEventTask, timeoutWakeTask))
                {
                    UpdateSupervisorState("TimeoutWake");
                    continue;
                }

                await ProcessWorkerEventTaskAsync(completedEventTask).ConfigureAwait(false);
            }
        }
        finally
        {
            foreach (var worker in workers)
            {
                journal.WriteEvent(
                    "WorkerLogSummary",
                    worker.PipelineName,
                    $"stdout={worker.StandardOutputArtifactPath}; stdoutBytes={worker.StandardOutputCapturedBytes.ToString(CultureInfo.InvariantCulture)}; stdoutTruncated={worker.StandardOutputWasTruncated}; stdoutDroppedBytes={worker.StandardOutputDroppedBytes.ToString(CultureInfo.InvariantCulture)}; stderr={worker.StandardErrorArtifactPath}; stderrBytes={worker.StandardErrorCapturedBytes.ToString(CultureInfo.InvariantCulture)}; stderrTruncated={worker.StandardErrorWasTruncated}; stderrDroppedBytes={worker.StandardErrorDroppedBytes.ToString(CultureInfo.InvariantCulture)}");
                worker.Dispose();
            }
        }

        return kernel.TaskOutcomesByTaskProfileId;
    }

    private static Task? CreateWorkerEventTimeoutWakeTask(
        IReadOnlyDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        OrchestrationRuntimeKernel kernel,
        TimeSpan? workerEventTimeout,
        TimeSpan? workerActivationTimeout,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var nextTimeoutAt = eventTasksByWorker
            .Where(static item => !item.Value.IsCompleted)
            .Select(static item => item.Key)
            .Select(worker => new
            {
                Worker = worker,
                Decision = kernel.ResolveWorkerTimeout(worker.PipelineName)
            })
            .Where(static item => !item.Decision.IsWaitingForOrchestrationCommand)
            .Select(item => ResolveWorkerProtocolTimeout(item.Decision, workerEventTimeout, workerActivationTimeout) is { } timeout
                ? item.Worker.LastProtocolActivityUtc + timeout
                : (DateTimeOffset?)null)
            .Where(static item => item is not null)
            .OrderBy(static item => item)
            .FirstOrDefault();
        if (nextTimeoutAt is null)
        {
            return null;
        }

        var delay = nextTimeoutAt.Value - now;
        return delay <= TimeSpan.Zero
            ? Task.CompletedTask
            : Task.Delay(delay, cancellationToken);
    }

    private static PipelineWorkerProcess? FindTimedOutWorker(
        IReadOnlyDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        OrchestrationRuntimeKernel kernel,
        TimeSpan? workerEventTimeout,
        TimeSpan? workerActivationTimeout,
        DateTimeOffset now)
    {
        foreach (var worker in eventTasksByWorker.Keys.OrderBy(static item => item.PipelineName, StringComparer.OrdinalIgnoreCase))
        {
            if (eventTasksByWorker[worker].IsCompleted)
            {
                continue;
            }

            var decision = kernel.ResolveWorkerTimeout(worker.PipelineName);
            if (decision.IsWaitingForOrchestrationCommand)
            {
                continue;
            }

            var timeout = ResolveWorkerProtocolTimeout(decision, workerEventTimeout, workerActivationTimeout);
            if (timeout is not { } protocolTimeout)
            {
                continue;
            }

            var elapsed = now - worker.LastProtocolActivityUtc;
            if (elapsed < protocolTimeout)
            {
                continue;
            }

            if (decision.HasUnresolvedPipelineTasks || !worker.HasExited)
            {
                return worker;
            }
        }

        return null;
    }

    private static IReadOnlySet<string> ResolveLiveWorkerNames(
        IReadOnlyDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        OrchestrationRuntimeKernel kernel) =>
        eventTasksByWorker.Keys
            .Select(static item => item.PipelineName)
            .Concat(kernel.ReadyByTaskId.Values.Select(static item => item.WorkerName))
            .Concat(kernel.RunningWorkerNamesByTaskId.Values)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

    private static TimeSpan? ResolveWorkerProtocolTimeout(
        OrchestrationWorkerTimeoutDecision decision,
        TimeSpan? workerEventTimeout,
        TimeSpan? workerActivationTimeout) =>
        decision.Kind is OrchestrationWorkerTimeoutDecisionKind.AwaitingWorkerOnline
            or OrchestrationWorkerTimeoutDecisionKind.AwaitingWorkerReady
            or OrchestrationWorkerTimeoutDecisionKind.AwaitingPipelineStarted
                ? workerActivationTimeout
                : workerEventTimeout;

    private static TimeSpan? NormalizeOptionalTimeout(TimeSpan? timeout) =>
        timeout is null || timeout.Value == TimeSpan.Zero
            ? null
            : timeout;

    private static TimeSpan? ResolveWorkerActivationTimeout(
        TimeSpan? configuredActivationTimeout,
        TimeSpan? workerEventTimeout) =>
        configuredActivationTimeout is null
            ? workerEventTimeout
            : NormalizeOptionalTimeout(configuredActivationTimeout);

    private static async Task<bool> HandleWorkerLifecycleEventAsync(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        IDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        ICollection<PipelineWorkerProcess> workers,
        IDictionary<string, PipelineWorkerProcess> workersByName,
        OrchestrationRuntimeKernel kernel,
        OrchestrationRuntimeRequest request,
        OrchestrationRunJournal journal,
        CancellationToken cancellationToken)
    {
        if (string.Equals(workerEvent.Kind, WorkerEventKinds.WorkerOnline, StringComparison.OrdinalIgnoreCase))
        {
            kernel.MarkWorkerOnline(worker.PipelineName, workerEvent.ExecutableVersion);
            eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
            return true;
        }

        if (string.Equals(workerEvent.Kind, WorkerEventKinds.WorkerReady, StringComparison.OrdinalIgnoreCase))
        {
            kernel.MarkWorkerReady(worker.PipelineName);
            try
            {
                await worker.SendStartPipelineAsync().ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                var decision = kernel.ApplyWorkerLoss(worker.PipelineName);
                await ExecutePreWorkReplacementDecisionAsync(
                    worker,
                    decision,
                    "StartPipelineCommandLost",
                    worker.PipelineName,
                    ex.Message,
                    eventTasksByWorker,
                    workers,
                    workersByName,
                    kernel,
                    request,
                    journal,
                    cancellationToken).ConfigureAwait(false);
                return true;
            }

            kernel.MarkStartPipelineSent(worker.PipelineName);
            journal.WriteEvent("StartPipeline", worker.PipelineName, worker.PipelineId);
            eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
            return true;
        }

        if (string.Equals(workerEvent.Kind, WorkerEventKinds.PipelineStarted, StringComparison.OrdinalIgnoreCase))
        {
            kernel.MarkPipelineStarted(worker.PipelineName);
            eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
            return true;
        }

        if (string.Equals(workerEvent.Kind, WorkerEventKinds.PipelineCatalog, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(workerEvent.Kind, WorkerEventKinds.PipelineCompleted, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(workerEvent.Kind, WorkerEventKinds.PipelineStopped, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(workerEvent.Kind, WorkerEventKinds.PipelineFailed, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(workerEvent.Kind, WorkerEventKinds.WorkerDrained, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(workerEvent.Kind, WorkerEventKinds.Heartbeat, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(workerEvent.Kind, WorkerEventKinds.Diagnostic, StringComparison.OrdinalIgnoreCase))
        {
            kernel.AcceptWorkerLifecycleEvent(worker.PipelineName, workerEvent.Kind);
            eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
            return true;
        }

        return false;
    }

    private static async Task ExecutePreWorkReplacementDecisionAsync(
        PipelineWorkerProcess worker,
        OrchestrationWorkerLossDecision decision,
        string journalEventKind,
        string journalSubject,
        string reason,
        IDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        ICollection<PipelineWorkerProcess> workers,
        IDictionary<string, PipelineWorkerProcess> workersByName,
        OrchestrationRuntimeKernel kernel,
        OrchestrationRuntimeRequest request,
        OrchestrationRunJournal journal,
        CancellationToken cancellationToken)
    {
        if (decision.Kind is not (OrchestrationWorkerLossDecisionKind.ReplaceFromBeginning or OrchestrationWorkerLossDecisionKind.ReplaceAtReadyTaskBoundary))
        {
            throw new InvalidOperationException(
                $"Worker loss for '{worker.PipelineName}' produced decision {decision.Kind}, but a pre-work replacement was required.");
        }

        eventTasksByWorker.Remove(worker);
        journal.WriteEvent(
            journalEventKind,
            journalSubject,
            reason);
        worker.Terminate(reason);

        var replacementReservation = kernel.ReservePreWorkReplacementAttempt(
            worker.PipelineName,
            decision.ResumeTaskId,
            reason);
        journal.WriteEvent(
            "WorkerReplacementReserved",
            worker.PipelineName,
            $"ResumeTaskId={replacementReservation.ResumeTaskId}; Attempt={replacementReservation.Attempt.ToString(CultureInfo.InvariantCulture)}; Limit={replacementReservation.Limit.ToString(CultureInfo.InvariantCulture)}");

        var replacement = await StartReplacementWorkerAsync(
            worker.PipelineName,
            worker.PipelineId,
            decision.ResumeTaskId,
            request,
            journal,
            cancellationToken).ConfigureAwait(false);
        kernel.RegisterWorker(
            replacement.PipelineName,
            replacement.PipelineId,
            replacement.ResumeTaskId,
            replacement.ExpectedExecutableVersion);
        workers.Add(replacement);
        workersByName[replacement.PipelineName] = replacement;
        eventTasksByWorker[replacement] = replacement.ReadEventAsync(cancellationToken);
    }

    private static async Task<bool> TryGrantReadyWorkerTaskAsync(
        OrchestrationRuntimeKernel kernel,
        IDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        ICollection<PipelineWorkerProcess> workers,
        IDictionary<string, PipelineWorkerProcess> workersByName,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        OrchestrationRuntimeRequest request,
        IOrchestrationRuntimeObserver? observer,
        OrchestrationRunJournal journal,
        CancellationToken cancellationToken)
    {
        var exitedWorkerNames = workersByName.Values
            .Where(static item => item.HasExited)
            .Select(static item => item.PipelineName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var decision = kernel.ChooseReadyAction(
            exitedWorkerNames,
            DateTimeOffset.UtcNow,
            request.MaxDegreeOfParallelism);
        switch (decision.Kind)
        {
            case OrchestrationRuntimeReadyDecisionKind.None:
                return false;
            case OrchestrationRuntimeReadyDecisionKind.ReplaceWorker:
                if (!workersByName.TryGetValue(decision.ReadyTask.WorkerName, out var lostWorker))
                {
                    throw new InvalidOperationException(
                        $"Cannot replace ready worker '{decision.ReadyTask.WorkerName}' because no worker process is registered.");
                }

                await ExecutePreWorkReplacementDecisionAsync(
                    lostWorker,
                    decision.WorkerLossDecision,
                    "ReadyWorkerClosedBeforeGrant",
                    FormatTaskName(decision.PlannedTask!),
                    $"Pipeline worker '{lostWorker.PipelineName}' exited before task '{decision.ReadyTask.TaskName}' could be granted.",
                    eventTasksByWorker,
                    workers,
                    workersByName,
                    kernel,
                    request,
                    journal,
                    cancellationToken).ConfigureAwait(false);
                return true;
            case OrchestrationRuntimeReadyDecisionKind.Block:
                await StopPipelineAtBlockedTaskAsync(
                    decision.BlockedPipeline,
                    workersByName,
                    blockedResults,
                    observer,
                    journal).ConfigureAwait(false);
                return true;
            case OrchestrationRuntimeReadyDecisionKind.Grant:
                if (!workersByName.TryGetValue(decision.ReadyTask.WorkerName, out var readyWorker))
                {
                    throw new InvalidOperationException(
                        $"Cannot grant task '{decision.ReadyTask.TaskId}' because ready worker '{decision.ReadyTask.WorkerName}' is not registered.");
                }

                try
                {
                    await readyWorker.SendGrantTaskAsync(decision.Grant).ConfigureAwait(false);
                }
                catch (InvalidOperationException ex)
                {
                    var lossDecision = kernel.ApplyWorkerLoss(readyWorker.PipelineName);
                    await ExecutePreWorkReplacementDecisionAsync(
                        readyWorker,
                        lossDecision,
                        "GrantTaskCommandLost",
                        FormatTaskName(decision.PlannedTask!),
                        ex.Message,
                        eventTasksByWorker,
                        workers,
                        workersByName,
                        kernel,
                        request,
                        journal,
                        cancellationToken).ConfigureAwait(false);
                    return true;
                }

                var issued = kernel.CommitGrantIssued(
                    decision.ReadyTask,
                    decision.PlannedTask!,
                    decision.PlannedTaskLocks,
                    decision.Grant);
                observer?.TaskStarted(issued.PlannedTask.Id, FormatTaskName(issued.PlannedTask));
                journal.WriteEvent(
                    "GrantTask",
                    FormatTaskName(issued.PlannedTask),
                    string.IsNullOrWhiteSpace(issued.Grant.PreviousGrantId)
                        ? $"{issued.Grant.TaskId}; GrantId={issued.Grant.GrantId}; Attempt={issued.Grant.AttemptNumber.ToString(CultureInfo.InvariantCulture)}"
                        : $"{issued.Grant.TaskId}; GrantId={issued.Grant.GrantId}; PreviousGrantId={issued.Grant.PreviousGrantId}; Attempt={issued.Grant.AttemptNumber.ToString(CultureInfo.InvariantCulture)}");
                return true;
            default:
                throw new InvalidOperationException(
                    $"Unsupported runtime ready decision {decision.Kind}.");
        }
    }

    private static async Task StopPipelineAtBlockedTaskAsync(
        OrchestrationRuntimeBlockedPipeline blockedPipeline,
        IDictionary<string, PipelineWorkerProcess> workersByName,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        IOrchestrationRuntimeObserver? observer,
        OrchestrationRunJournal journal)
    {
        RecordBlockedPipeline(blockedPipeline, blockedResults, observer, journal);
        journal.WriteEvent("StopPipeline", blockedPipeline.PipelineName, blockedPipeline.Reason);
        if (!workersByName.TryGetValue(blockedPipeline.PipelineName, out var worker))
        {
            journal.WriteEvent(
                "StopPipelineCommandLost",
                blockedPipeline.PipelineName,
                "worker process was no longer registered");
            return;
        }

        try
        {
            await worker.SendStopPipelineAsync(
                blockedPipeline.PipelineId,
                blockedPipeline.BlockingTaskId,
                blockedPipeline.Reason).ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            journal.WriteEvent(
                "StopPipelineCommandLost",
                blockedPipeline.PipelineName,
                ex.Message);
        }
    }

    private static void RecordBlockedPipeline(
        OrchestrationRuntimeBlockedPipeline blockedPipeline,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        IOrchestrationRuntimeObserver? observer,
        OrchestrationRunJournal journal)
    {
        foreach (var blocked in blockedPipeline.BlockedTasks)
        {
            blockedResults.Add(new OrchestrationTaskBlockedResult(
                blocked.PlannedTask.Id,
                blocked.PlannedTask.TaskAccessProfile.Id,
                blocked.PlannedTask.PipelineReference.Name,
                blocked.PlannedTask.TaskAccessProfile.TaskName,
                blocked.BlockingTaskProfileId,
                blocked.BlockingPipelineName,
                blocked.BlockingStepName,
                blocked.DependencyCondition,
                blocked.Outcome,
                blocked.Reason));
            observer?.TaskBlocked(blocked.PlannedTask.Id);
            journal.WriteEvent("TaskBlocked", FormatTaskName(blocked.PlannedTask), blocked.Reason);
        }
    }

    private static OrchestrationTaskWorkerResult RecordTaskCompletion(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        OrchestrationRuntimeTaskCompletion completion,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer)
    {
        var standardError = string.IsNullOrWhiteSpace(workerEvent.Message)
            ? worker.StandardErrorText
            : string.Concat(worker.StandardErrorText, workerEvent.Message, Environment.NewLine);
        var result = new OrchestrationTaskWorkerResult(
            completion.PlannedTask.TaskAccessProfile.Id,
            completion.PlannedTask.Id,
            completion.PlannedTask.PipelineReference.Name,
            completion.PlannedTask.TaskAccessProfile.TaskName,
            completion.ExitCode,
            worker.StandardOutputText,
            standardError,
            string.IsNullOrWhiteSpace(completion.GrantId) ? workerEvent.GrantId : completion.GrantId,
            string.IsNullOrWhiteSpace(completion.CommandId) ? workerEvent.CommandId : completion.CommandId,
            completion.AttemptNumber);
        taskResults.Add(result);
        if (completion.RecordTerminalOutcome)
        {
            observer?.TaskCompleted(result.PlannedTaskId, result.ExitCode == 0);
        }

        journal.WriteEvent(
            completion.JournalEventKind,
            FormatTaskName(completion.PlannedTask),
            result.ExitCode.ToString(CultureInfo.InvariantCulture));
        return result;
    }

    private static async Task HandleTaskFailedAsync(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        ResolvedOrchestrationRetryPolicy retryPolicy,
        OrchestrationRuntimeKernel kernel,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer,
        CancellationToken cancellationToken)
    {
        var failure = kernel.ResolveWorkerReportedFailure(workerEvent, worker.PipelineName, retryPolicy);
        var attemptResult = RecordTaskCompletion(
            worker,
            workerEvent with { ExitCode = failure.Completion.ExitCode },
            failure.Completion,
            taskResults,
            journal,
            observer);

        if (!failure.RetryDecision.ShouldRetry)
        {
            journal.WriteEvent(
                "RetryExhausted",
                FormatTaskName(failure.Completion.PlannedTask),
                $"{failure.FailureClass}; Attempt={attemptResult.AttemptNumber.ToString(CultureInfo.InvariantCulture)}; {failure.RetryDecision.Reason}");
            try
            {
                await worker.SendFailPipelineAsync(workerEvent.PipelineId, workerEvent.TaskId, failure.RetryDecision.Reason).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                journal.WriteEvent(
                    "FailPipelineCommandLost",
                    worker.PipelineName,
                    ex.Message);
            }

            return;
        }

        journal.WriteEvent(
            WorkerEventKinds.RetryScheduled,
            FormatTaskName(failure.Completion.PlannedTask),
            failure.RetryDecision.Delay > TimeSpan.Zero
                ? $"{failure.FailureClass}; NextAttempt={failure.RetryDecision.NextAttemptNumber.ToString(CultureInfo.InvariantCulture)}; Delay={failure.RetryDecision.Delay.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)}ms; {failure.RetryDecision.Reason}"
                : $"{failure.FailureClass}; NextAttempt={failure.RetryDecision.NextAttemptNumber.ToString(CultureInfo.InvariantCulture)}; {failure.RetryDecision.Reason}");
    }

    private static async Task HandleClosedWorkerAsync(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        IDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        ICollection<PipelineWorkerProcess> workers,
        IDictionary<string, PipelineWorkerProcess> workersByName,
        ResolvedOrchestrationRetryPolicy retryPolicy,
        OrchestrationRuntimeKernel kernel,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        OrchestrationRuntimeRequest request,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer,
        CancellationToken cancellationToken)
    {
        var stoppedByOrchestration = kernel.IsPipelineStopped(worker.PipelineName);
        var decision = kernel.ApplyWorkerLoss(
            worker.PipelineName,
            workerEvent.ExitCode,
            stoppedByOrchestration);
        await ExecuteWorkerLossDecisionAsync(
            worker,
            workerEvent,
            decision,
            eventTasksByWorker,
            workers,
            workersByName,
            retryPolicy,
            kernel,
            plannedTasksByPipelineTaskId,
            taskResults,
            blockedResults,
            request,
            journal,
            observer,
            cancellationToken).ConfigureAwait(false);
    }

    private static async Task ExecuteWorkerLossDecisionAsync(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        OrchestrationWorkerLossDecision decision,
        IDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        ICollection<PipelineWorkerProcess> workers,
        IDictionary<string, PipelineWorkerProcess> workersByName,
        ResolvedOrchestrationRetryPolicy retryPolicy,
        OrchestrationRuntimeKernel kernel,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        OrchestrationRuntimeRequest request,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer,
        CancellationToken cancellationToken)
    {
        switch (decision.Kind)
        {
            case OrchestrationWorkerLossDecisionKind.CloseOnly:
                return;

            case OrchestrationWorkerLossDecisionKind.ReplaceFromBeginning:
                await ExecutePreWorkReplacementDecisionAsync(
                    worker,
                    decision,
                    "WorkerClosedBeforePipelineContext",
                    worker.PipelineName,
                    $"Pipeline worker '{worker.PipelineName}' exited before pipeline context was established. ExitCode: {workerEvent.ExitCode.ToString(CultureInfo.InvariantCulture)}.",
                    eventTasksByWorker,
                    workers,
                    workersByName,
                    kernel,
                    request,
                    journal,
                    cancellationToken).ConfigureAwait(false);
                return;

            case OrchestrationWorkerLossDecisionKind.ReplaceAtReadyTaskBoundary:
                if (!plannedTasksByPipelineTaskId.TryGetValue(decision.TaskId, out var readyPlannedTask))
                {
                    throw new InvalidOperationException(
                        $"Pipeline worker '{worker.PipelineName}' lost ready task '{decision.TaskId}', but the task is not present in the run plan.");
                }

                await ExecutePreWorkReplacementDecisionAsync(
                    worker,
                    decision,
                    "ReadyWorkerClosedBeforeGrant",
                    FormatTaskName(readyPlannedTask),
                    $"Pipeline worker '{worker.PipelineName}' exited before task '{readyPlannedTask.TaskAccessProfile.TaskName}' could be granted.",
                    eventTasksByWorker,
                    workers,
                    workersByName,
                    kernel,
                    request,
                    journal,
                    cancellationToken).ConfigureAwait(false);
                return;

            case OrchestrationWorkerLossDecisionKind.ActiveGrantLost:
                if (!plannedTasksByPipelineTaskId.TryGetValue(decision.TaskId, out var runningPlannedTask))
                {
                    throw new InvalidOperationException(
                        $"Pipeline worker '{worker.PipelineName}' lost active task '{decision.TaskId}', but the task is not present in the run plan.");
                }

                var activeReason = $"pipeline worker exited while task {runningPlannedTask.TaskAccessProfile.TaskName} was running";
                await HandleSupervisorObservedRunningTaskFailureAsync(
                    worker,
                    workerEvent with
                    {
                        TaskId = decision.TaskId,
                        TaskName = runningPlannedTask.TaskAccessProfile.TaskName,
                        ExitCode = workerEvent.ExitCode == 0 ? 4 : workerEvent.ExitCode,
                        Message = string.IsNullOrWhiteSpace(workerEvent.Message) ? activeReason : workerEvent.Message,
                        FailureClass = WorkerFailureClasses.WorkerCrashBeforeTerminalEvent
                    },
                    WorkerFailureClasses.WorkerCrashBeforeTerminalEvent,
                    activeReason,
                    eventTasksByWorker,
                    workers,
                    workersByName,
                    retryPolicy,
                    kernel,
                    taskResults,
                    blockedResults,
                    request,
                    journal,
                    observer,
                    cancellationToken).ConfigureAwait(false);
                return;

            case OrchestrationWorkerLossDecisionKind.BlockRemainingAfterFailure:
                var failedTask = taskResults
                    .Where(result => string.Equals(result.PipelineName, worker.PipelineName, StringComparison.OrdinalIgnoreCase) && result.ExitCode != 0)
                    .LastOrDefault();
                var blockedPipeline = kernel.BlockRemainingPipelineTasks(
                    worker.PipelineName,
                    failedTask is null
                        ? "pipeline stopped after failed task"
                        : $"pipeline stopped after failed task {failedTask.StepName}");
                RecordBlockedPipeline(blockedPipeline, blockedResults, observer, journal);
                return;

            case OrchestrationWorkerLossDecisionKind.FailUnresolved:
                throw new InvalidOperationException(
                    $"Pipeline worker '{worker.PipelineName}' exited before all of its run-plan tasks were resolved or exited unexpectedly after resolution. ExitCode: {workerEvent.ExitCode.ToString(CultureInfo.InvariantCulture)}.");

            default:
                throw new InvalidOperationException(
                    $"Pipeline worker '{worker.PipelineName}' produced unsupported worker-loss decision {decision.Kind}.");
        }
    }

    private static async Task HandleSupervisorObservedRunningTaskFailureAsync(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        string failureClass,
        string reason,
        IDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        ICollection<PipelineWorkerProcess> workers,
        IDictionary<string, PipelineWorkerProcess> workersByName,
        ResolvedOrchestrationRetryPolicy retryPolicy,
        OrchestrationRuntimeKernel kernel,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        OrchestrationRuntimeRequest request,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer,
        CancellationToken cancellationToken)
    {
        var supervisorFailure = kernel.ResolveSupervisorObservedFailure(
            workerEvent,
            worker.PipelineName,
            failureClass,
            reason,
            retryPolicy);
        var attemptResult = RecordTaskCompletion(
            worker,
            workerEvent with { ExitCode = supervisorFailure.Completion.ExitCode, Message = reason, FailureClass = failureClass },
            supervisorFailure.Completion,
            taskResults,
            journal,
            observer);

        if (!supervisorFailure.RetryDecision.ShouldRetry)
        {
            journal.WriteEvent(
                "RetryExhausted",
                FormatTaskName(supervisorFailure.Completion.PlannedTask),
                $"{failureClass}; Attempt={attemptResult.AttemptNumber.ToString(CultureInfo.InvariantCulture)}; {supervisorFailure.RetryDecision.Reason}");
            RecordBlockedPipeline(supervisorFailure.BlockedPipeline, blockedResults, observer, journal);
            return;
        }

        journal.WriteEvent(
            WorkerEventKinds.RetryScheduled,
            FormatTaskName(supervisorFailure.Completion.PlannedTask),
            supervisorFailure.RetryDecision.Delay > TimeSpan.Zero
                ? $"{failureClass}; ReplacementWorker=True; NextAttempt={supervisorFailure.RetryDecision.NextAttemptNumber.ToString(CultureInfo.InvariantCulture)}; Delay={supervisorFailure.RetryDecision.Delay.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)}ms; {supervisorFailure.RetryDecision.Reason}"
                : $"{failureClass}; ReplacementWorker=True; NextAttempt={supervisorFailure.RetryDecision.NextAttemptNumber.ToString(CultureInfo.InvariantCulture)}; {supervisorFailure.RetryDecision.Reason}");

        var replacement = await StartReplacementWorkerAsync(
            worker.PipelineName,
            worker.PipelineId,
            supervisorFailure.ResumeTaskId,
            request,
            journal,
            cancellationToken).ConfigureAwait(false);
        kernel.RegisterWorker(
            replacement.PipelineName,
            replacement.PipelineId,
            replacement.ResumeTaskId,
            replacement.ExpectedExecutableVersion);
        workers.Add(replacement);
        workersByName[replacement.PipelineName] = replacement;
        eventTasksByWorker[replacement] = replacement.ReadEventAsync(cancellationToken);
    }

    private static async Task<PipelineWorkerProcess> StartReplacementWorkerAsync(
        string pipelineName,
        string pipelineId,
        string resumeTaskId,
        OrchestrationRuntimeRequest request,
        OrchestrationRunJournal journal,
        CancellationToken cancellationToken)
    {
        journal.WriteEvent("WorkerRestarting", pipelineName, $"ResumeTaskId={resumeTaskId}");
        var replacement = await PipelineWorkerProcess.StartAsync(
            pipelineName,
            pipelineId,
            resumeTaskId,
            request,
            journal.RunDirectoryPath,
            NormalizeOptionalTimeout(request.WorkerControlPipeConnectTimeout),
            cancellationToken).ConfigureAwait(false);
        journal.WriteEvent("WorkerRestarted", pipelineName, replacement.ProcessId.ToString(CultureInfo.InvariantCulture));
        journal.WriteEvent("WorkerControlPipe", pipelineName, replacement.ControlPipeName);
        journal.WriteEvent("WorkerLog", pipelineName, $"stdout={replacement.StandardOutputArtifactPath}; stderr={replacement.StandardErrorArtifactPath}");
        return replacement;
    }

    private static async Task HandleTimedOutWorkerAsync(
        PipelineWorkerProcess worker,
        TimeSpan? workerEventTimeout,
        TimeSpan? workerActivationTimeout,
        IDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        ICollection<PipelineWorkerProcess> workers,
        IDictionary<string, PipelineWorkerProcess> workersByName,
        ResolvedOrchestrationRetryPolicy retryPolicy,
        OrchestrationRuntimeKernel kernel,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        OrchestrationRuntimeRequest request,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer,
        CancellationToken cancellationToken)
    {
        var decision = kernel.ResolveWorkerTimeout(worker.PipelineName);
        var protocolTimeout = ResolveWorkerProtocolTimeout(decision, workerEventTimeout, workerActivationTimeout);
        if (protocolTimeout is null)
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.PipelineName}' was selected for timeout handling, but no timeout is configured for state '{decision.Kind}'.");
        }

        var reason = DescribeWorkerEventTimeout(decision, protocolTimeout.Value);
        journal.WriteEvent("WorkerProtocolTimeout", worker.PipelineName, reason);
        worker.Terminate(reason);

        if (decision.Kind == OrchestrationWorkerTimeoutDecisionKind.ActiveGrantTimedOut &&
            plannedTasksByPipelineTaskId.TryGetValue(decision.TaskId, out var runningPlannedTask))
        {
            kernel.MarkWorkerClosed(worker.PipelineName);
            kernel.RunningGrantsByTaskId.TryGetValue(decision.TaskId, out var grant);
            await HandleSupervisorObservedRunningTaskFailureAsync(
                worker,
                new WorkerProtocolEvent(
                    WorkerEventKinds.TaskFailed,
                    worker.ProcessId.ToString(CultureInfo.InvariantCulture),
                    worker.PipelineId,
                    worker.PipelineName,
                    decision.TaskId,
                    runningPlannedTask.TaskAccessProfile.TaskName,
                    grant.GrantId,
                    grant.CommandId,
                    grant.AttemptNumber,
                    4,
                    string.Empty,
                    reason,
                    WorkerFailureClasses.TaskTimeout),
                WorkerFailureClasses.TaskTimeout,
                reason,
                eventTasksByWorker,
                workers,
                workersByName,
                retryPolicy,
                kernel,
                taskResults,
                blockedResults,
                request,
                journal,
                observer,
                cancellationToken).ConfigureAwait(false);
            return;
        }

        if (decision.HasUnresolvedPipelineTasks)
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.PipelineName}' stopped responding before all of its run-plan tasks were resolved. {reason}");
        }

        kernel.MarkWorkerClosed(worker.PipelineName);
    }

    private static string DescribeWorkerEventTimeout(
        OrchestrationWorkerTimeoutDecision decision,
        TimeSpan workerEventTimeout)
    {
        var timeoutText = FormatTimeout(workerEventTimeout);
        return decision.Kind switch
        {
            OrchestrationWorkerTimeoutDecisionKind.AwaitingWorkerOnline =>
                $"No worker protocol event was received within {timeoutText}; the worker did not emit {WorkerEventKinds.WorkerOnline}.",
            OrchestrationWorkerTimeoutDecisionKind.AwaitingWorkerReady =>
                $"No worker protocol event was received within {timeoutText}; the worker did not emit {WorkerEventKinds.WorkerReady}.",
            OrchestrationWorkerTimeoutDecisionKind.AwaitingStartPipelineCommand =>
                $"No worker protocol event was received within {timeoutText}; orchestration did not send {WorkerCommandKinds.StartPipeline} before the worker timeout boundary.",
            OrchestrationWorkerTimeoutDecisionKind.AwaitingPipelineStarted =>
                $"No worker protocol event was received within {timeoutText}; the worker did not emit {WorkerEventKinds.PipelineStarted} after {WorkerCommandKinds.StartPipeline}.",
            OrchestrationWorkerTimeoutDecisionKind.WaitingForGrant =>
                $"No worker protocol event was received within {timeoutText}; the worker is waiting for {WorkerCommandKinds.GrantTask} for task '{decision.TaskId}'.",
            OrchestrationWorkerTimeoutDecisionKind.ActiveGrantTimedOut =>
                $"No worker protocol event was received within {timeoutText} after {WorkerCommandKinds.GrantTask} for task '{decision.TaskId}'; the active task outcome is unknown.",
            OrchestrationWorkerTimeoutDecisionKind.AwaitingTaskBoundary =>
                $"No worker protocol event was received within {timeoutText}; the worker did not emit {WorkerEventKinds.TaskReady} or a terminal pipeline event after {WorkerEventKinds.PipelineStarted}.",
            OrchestrationWorkerTimeoutDecisionKind.PipelineResolved =>
                $"No worker protocol event was received within {timeoutText}; the pipeline has no unresolved run-plan tasks.",
            _ =>
                $"No worker protocol event was received within {timeoutText} after the last command or event."
        };
    }

    private static string FormatTimeout(TimeSpan timeout)
    {
        if (timeout.TotalSeconds < 1)
        {
            return $"{timeout.TotalMilliseconds.ToString("0", CultureInfo.InvariantCulture)}ms";
        }

        if (timeout.TotalMinutes < 1)
        {
            return $"{timeout.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture)}s";
        }

        return $"{timeout.TotalMinutes.ToString("0.###", CultureInfo.InvariantCulture)}m";
    }

    private static string FormatTaskName(MO.PlannedTask plannedTask) =>
        $"{plannedTask.PipelineReference.Name}.{plannedTask.TaskAccessProfile.TaskName}";

    private static OrchestrationRuntimeRequest NormalizeRequestPaths(OrchestrationRuntimeRequest request) =>
        request with
        {
            WorkspacePath = Path.GetFullPath(request.WorkspacePath),
            PipelineWorkspacePath = Path.GetFullPath(request.PipelineWorkspacePath),
            TransformWorkspacePath = Path.GetFullPath(request.TransformWorkspacePath),
            BindingWorkspacePath = Path.GetFullPath(request.BindingWorkspacePath),
            DataTypeConversionWorkspacePath = string.IsNullOrWhiteSpace(request.DataTypeConversionWorkspacePath)
                ? string.Empty
                : Path.GetFullPath(request.DataTypeConversionWorkspacePath),
            RunArtifactsRootPath = string.IsNullOrWhiteSpace(request.RunArtifactsRootPath)
                ? string.Empty
                : Path.GetFullPath(request.RunArtifactsRootPath),
        };

    private static bool IsSameOrChildPath(string candidatePath, string parentPath)
    {
        var normalizedCandidate = NormalizeDirectoryComparisonPath(candidatePath);
        var normalizedParent = NormalizeDirectoryComparisonPath(parentPath);
        return string.Equals(normalizedCandidate, normalizedParent, StringComparison.OrdinalIgnoreCase) ||
               normalizedCandidate.StartsWith(normalizedParent + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeDirectoryComparisonPath(string path) =>
        Path.GetFullPath(path)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    private static bool IsActive(string value) =>
        string.Equals(value, "Active", StringComparison.OrdinalIgnoreCase);

    private static MO.RunPlan ResolveRunPlan(MO.MetaOrchestrationModel model)
    {
        return model.RunPlanList.Count switch
        {
            1 => model.RunPlanList[0],
            0 => throw new InvalidOperationException("The orchestration workspace contains no RunPlan rows."),
            _ => throw new InvalidOperationException("The orchestration workspace contains multiple run plans.")
        };
    }

    private static decimal ParseOrdinal(string value) =>
        decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var ordinal)
            ? ordinal
            : decimal.MaxValue;

    private sealed class PipelineWorkerProcess : IDisposable
    {
        private readonly Process process;
        private readonly OrchestrationWorkerProtocolChannel channel;
        private readonly Task standardOutputPump;
        private readonly Task standardErrorPump;
        private readonly string expectedExecutableVersion;
        private readonly OrchestrationDiagnosticLogBuffer standardOutput;
        private readonly OrchestrationDiagnosticLogBuffer standardError;
        private long lastProtocolActivityUnixMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        private bool disposed;

        private PipelineWorkerProcess(
            string pipelineName,
            string pipelineId,
            string resumeTaskId,
            Process process,
            OrchestrationWorkerProtocolChannel channel,
            string expectedExecutableVersion,
            string controlPipeName,
            OrchestrationDiagnosticLogBuffer standardOutput,
            OrchestrationDiagnosticLogBuffer standardError,
            Task standardOutputPump,
            Task standardErrorPump)
        {
            PipelineName = pipelineName;
            PipelineId = string.IsNullOrWhiteSpace(pipelineId) ? $"pipeline:{pipelineName}" : pipelineId;
            ResumeTaskId = string.IsNullOrWhiteSpace(resumeTaskId) ? string.Empty : resumeTaskId.Trim();
            this.process = process;
            this.channel = channel;
            this.expectedExecutableVersion = expectedExecutableVersion;
            ControlPipeName = controlPipeName;
            this.standardOutput = standardOutput;
            this.standardError = standardError;
            this.standardOutputPump = standardOutputPump;
            this.standardErrorPump = standardErrorPump;
        }

        public string PipelineName { get; }

        public string PipelineId { get; }

        public string ResumeTaskId { get; }

        public string ExpectedExecutableVersion => expectedExecutableVersion;

        public string ControlPipeName { get; }

        public int ProcessId => process.Id;

        public DateTimeOffset LastProtocolActivityUtc =>
            DateTimeOffset.FromUnixTimeMilliseconds(Interlocked.Read(ref lastProtocolActivityUnixMilliseconds));

        public bool HasExited
        {
            get
            {
                try
                {
                    return process.HasExited;
                }
                catch (InvalidOperationException)
                {
                    return true;
                }
            }
        }

        public string StandardOutputText => standardOutput.ToString();

        public string StandardErrorText => standardError.ToString();

        public string? StandardOutputArtifactPath => standardOutput.ArtifactPath;

        public string? StandardErrorArtifactPath => standardError.ArtifactPath;

        public bool StandardOutputWasTruncated => standardOutput.WasTruncated;

        public bool StandardErrorWasTruncated => standardError.WasTruncated;

        public long StandardOutputDroppedBytes => standardOutput.DroppedBytes;

        public long StandardErrorDroppedBytes => standardError.DroppedBytes;

        public int StandardOutputCapturedBytes => standardOutput.CapturedBytes;

        public int StandardErrorCapturedBytes => standardError.CapturedBytes;

        public static async Task<PipelineWorkerProcess> StartAsync(
            string pipelineName,
            string pipelineId,
            string resumeTaskId,
            OrchestrationRuntimeRequest request,
            string runDirectoryPath,
            TimeSpan? controlPipeConnectTimeout,
            CancellationToken cancellationToken)
        {
            var controlPipeName = CreateControlPipeName();
            var serverPipe = OrchestrationWorkerProtocolChannel.CreateServerPipe(controlPipeName);
            var startInfo = new ProcessStartInfo
            {
                FileName = request.PipelineExecutableName,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            startInfo.ArgumentList.Add("execute-worker");
            startInfo.ArgumentList.Add("--control-pipe");
            startInfo.ArgumentList.Add(controlPipeName);
            if (controlPipeConnectTimeout is { } connectTimeout)
            {
                startInfo.ArgumentList.Add("--control-pipe-connect-timeout-seconds");
                startInfo.ArgumentList.Add(((int)Math.Ceiling(connectTimeout.TotalSeconds)).ToString(CultureInfo.InvariantCulture));
            }

            startInfo.ArgumentList.Add("--workspace");
            startInfo.ArgumentList.Add(request.PipelineWorkspacePath);
            startInfo.ArgumentList.Add("--pipeline");
            startInfo.ArgumentList.Add(pipelineName);
            startInfo.ArgumentList.Add("--transform-workspace");
            startInfo.ArgumentList.Add(request.TransformWorkspacePath);
            startInfo.ArgumentList.Add("--binding-workspace");
            startInfo.ArgumentList.Add(request.BindingWorkspacePath);

            if (!string.IsNullOrWhiteSpace(request.DataTypeConversionWorkspacePath))
            {
                startInfo.ArgumentList.Add("--data-type-conversion-workspace");
                startInfo.ArgumentList.Add(request.DataTypeConversionWorkspacePath);
            }

            if (!string.IsNullOrWhiteSpace(request.PipelineDbConnectionEnvironmentVariableName))
            {
                startInfo.ArgumentList.Add("--pipeline-db-connection-env");
                startInfo.ArgumentList.Add(request.PipelineDbConnectionEnvironmentVariableName);
            }

            var process = new Process { StartInfo = startInfo };
            if (!process.Start())
            {
                serverPipe.Dispose();
                process.Dispose();
                throw new InvalidOperationException($"Cannot start pipeline worker process for pipeline '{pipelineName}'.");
            }

            var expectedVersion = string.IsNullOrWhiteSpace(request.ExpectedWorkerExecutableVersion)
                ? OrchestrationWorkerProtocol.ResolveExecutableVersion(typeof(MetaOrchestrationRuntimeService).Assembly)
                : request.ExpectedWorkerExecutableVersion;
            var logFileSegment = $"{SanitizeFileSegment(pipelineName)}-{process.Id.ToString(CultureInfo.InvariantCulture)}";
            var logCapturePolicy = request.LogCapturePolicy ?? OrchestrationLogCapturePolicy.Default;
            var standardOutput = new OrchestrationDiagnosticLogBuffer(
                logCapturePolicy,
                Path.Combine(runDirectoryPath, "logs", $"{logFileSegment}.stdout.log"));
            var standardError = new OrchestrationDiagnosticLogBuffer(
                logCapturePolicy,
                Path.Combine(runDirectoryPath, "logs", $"{logFileSegment}.stderr.log"));
            var standardOutputPump = ReadTextStreamAsync(process.StandardOutput, standardOutput);
            var standardErrorPump = ReadTextStreamAsync(process.StandardError, standardError);

            try
            {
                await WaitForControlPipeConnectionAsync(
                    serverPipe,
                    process,
                    pipelineName,
                    standardError,
                    controlPipeConnectTimeout,
                    cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await DisposeFailedStartAsync(
                    process,
                    serverPipe,
                    standardOutputPump,
                    standardErrorPump).ConfigureAwait(false);
                throw;
            }

            return new PipelineWorkerProcess(
                pipelineName,
                pipelineId,
                resumeTaskId,
                process,
                OrchestrationWorkerProtocolChannel.FromConnectedStream(serverPipe),
                expectedVersion,
                controlPipeName,
                standardOutput,
                standardError,
                standardOutputPump,
                standardErrorPump);
        }

        public async Task<WorkerProtocolEvent> ReadEventAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (await channel.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
                {
                    try
                    {
                        if (OrchestrationWorkerProtocol.TryDecodeEvent(line, out var workerEvent))
                        {
                            RecordProtocolActivity();
                            if (string.Equals(workerEvent.Kind, WorkerEventKinds.Closed, StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(workerEvent.Kind, WorkerEventKinds.ProtocolFault, StringComparison.OrdinalIgnoreCase))
                            {
                                return new WorkerProtocolEvent(
                                    WorkerEventKinds.ProtocolFault,
                                    string.Empty,
                                    string.Empty,
                                    PipelineName,
                                    string.Empty,
                                    string.Empty,
                                    string.Empty,
                                    string.Empty,
                                    0,
                                    -1,
                                    string.Empty,
                                    $"Worker emitted reserved event kind '{workerEvent.Kind}'.");
                            }

                            return workerEvent;
                        }

                        RecordProtocolActivity();
                        return new WorkerProtocolEvent(
                            WorkerEventKinds.ProtocolFault,
                            string.Empty,
                            string.Empty,
                            PipelineName,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            0,
                            -1,
                            string.Empty,
                            $"Unsupported worker control line '{line}'.");
                    }
                    catch (InvalidOperationException ex)
                    {
                        RecordProtocolActivity();
                        return new WorkerProtocolEvent(
                            WorkerEventKinds.ProtocolFault,
                            string.Empty,
                            string.Empty,
                            PipelineName,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            0,
                            -1,
                            string.Empty,
                            ex.Message);
                    }
                }

                await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
                await standardOutputPump.ConfigureAwait(false);
                await standardErrorPump.ConfigureAwait(false);
                RecordProtocolActivity();
                return new WorkerProtocolEvent(
                    WorkerEventKinds.Closed,
                    string.Empty,
                    string.Empty,
                    PipelineName,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    0,
                    process.ExitCode,
                    string.Empty,
                    string.Empty);
            }
            catch (Exception ex) when (ex is InvalidOperationException or IOException or ObjectDisposedException)
            {
                standardError.AppendLine(ex.Message);
                RecordProtocolActivity();
                return new WorkerProtocolEvent(
                    ex is InvalidOperationException ? WorkerEventKinds.ProtocolFault : WorkerEventKinds.Closed,
                    string.Empty,
                    string.Empty,
                    PipelineName,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    0,
                    -1,
                    string.Empty,
                    ex.Message);
            }
        }

        public async Task SendStartPipelineAsync()
        {
            await SendCommandAsync(new WorkerProtocolCommand(
                WorkerCommandKinds.StartPipeline,
                Guid.NewGuid().ToString("N"),
                string.Empty,
                string.Empty,
                0,
                PipelineId,
                PipelineName,
                ResumeTaskId,
                "activate pipeline")).ConfigureAwait(false);
        }

        public Task SendGrantTaskAsync(OrchestrationRuntimeGrant grant) =>
            SendCommandAsync(new WorkerProtocolCommand(
                WorkerCommandKinds.GrantTask,
                grant.CommandId,
                grant.GrantId,
                grant.PreviousGrantId,
                grant.AttemptNumber,
                grant.PipelineId,
                PipelineName,
                grant.TaskId,
                string.Empty));

        public Task SendStopPipelineAsync(string pipelineId, string taskId, string reason) =>
            SendCommandAsync(new WorkerProtocolCommand(
                WorkerCommandKinds.StopPipeline,
                Guid.NewGuid().ToString("N"),
                string.Empty,
                string.Empty,
                0,
                pipelineId,
                PipelineName,
                taskId,
                reason));

        public Task SendFailPipelineAsync(string pipelineId, string taskId, string reason) =>
            SendCommandAsync(new WorkerProtocolCommand(
                WorkerCommandKinds.FailPipeline,
                Guid.NewGuid().ToString("N"),
                string.Empty,
                string.Empty,
                0,
                pipelineId,
                PipelineName,
                taskId,
                reason));

        public void MarkProtocolEventObservedBySupervisor() =>
            RecordProtocolActivity();

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            try
            {
                channel.Dispose();
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (Exception)
            {
                // Best-effort cleanup. Runtime result is determined before disposal.
            }
            finally
            {
                process.Dispose();
            }
        }

        private async Task SendCommandAsync(WorkerProtocolCommand command)
        {
            try
            {
                if (HasExited)
                {
                    throw new InvalidOperationException(
                        $"process has already exited with code {process.ExitCode.ToString(CultureInfo.InvariantCulture)}");
                }

                await channel.WriteCommandAsync(command).ConfigureAwait(false);
                RecordProtocolActivity();
            }
            catch (Exception ex) when (ex is InvalidOperationException or IOException or ObjectDisposedException)
            {
                throw new InvalidOperationException(
                    $"Cannot send {command.Kind} to pipeline worker '{PipelineName}' for task '{command.TaskId}' because the worker control channel is not available. {ex.Message}",
                    ex);
            }
        }

        public void Terminate(string reason)
        {
            standardError.AppendLine(reason);
            try
            {
                channel.Dispose();
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (Exception)
            {
                // Best-effort termination after supervisor timeout.
            }
        }

        private void RecordProtocolActivity() =>
            Interlocked.Exchange(
                ref lastProtocolActivityUnixMilliseconds,
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

        private static async Task ReadTextStreamAsync(
            TextReader reader,
            OrchestrationDiagnosticLogBuffer buffer)
        {
            while (await reader.ReadLineAsync().ConfigureAwait(false) is { } line)
            {
                buffer.AppendLine(line);
            }
        }

        private static async Task WaitForControlPipeConnectionAsync(
            NamedPipeServerStream serverPipe,
            Process process,
            string pipelineName,
            OrchestrationDiagnosticLogBuffer standardError,
            TimeSpan? controlPipeConnectTimeout,
            CancellationToken cancellationToken)
        {
            var connectTask = serverPipe.WaitForConnectionAsync(cancellationToken);
            var exitTask = process.WaitForExitAsync(cancellationToken);
            var waitTasks = new List<Task> { connectTask, exitTask };
            if (controlPipeConnectTimeout is { } timeout)
            {
                waitTasks.Add(Task.Delay(timeout, cancellationToken));
            }

            var completedTask = await Task.WhenAny(waitTasks).ConfigureAwait(false);
            if (ReferenceEquals(completedTask, connectTask))
            {
                await connectTask.ConfigureAwait(false);
                return;
            }

            if (ReferenceEquals(completedTask, exitTask))
            {
                await exitTask.ConfigureAwait(false);
                throw new InvalidOperationException(
                    $"Pipeline worker '{pipelineName}' exited before connecting to the orchestration control pipe. ExitCode: {process.ExitCode.ToString(CultureInfo.InvariantCulture)}. {standardError.ToString().Trim()}");
            }

            throw new InvalidOperationException(
                $"Pipeline worker '{pipelineName}' did not connect to the orchestration control pipe within {FormatTimeout(controlPipeConnectTimeout!.Value)}.");
        }

        private static async Task DisposeFailedStartAsync(
            Process process,
            NamedPipeServerStream serverPipe,
            Task standardOutputPump,
            Task standardErrorPump)
        {
            try
            {
                serverPipe.Dispose();
            }
            catch (Exception)
            {
                // Best-effort failed-start cleanup.
            }

            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }

                await Task.WhenAll(standardOutputPump, standardErrorPump).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // Best-effort failed-start cleanup.
            }
            finally
            {
                process.Dispose();
            }
        }

        private static string CreateControlPipeName() =>
            $"meta-orchestration-{Guid.NewGuid():N}";

        private static string SanitizeFileSegment(string value)
        {
            var invalidCharacters = Path.GetInvalidFileNameChars();
            var sanitized = string.Concat(
                value.Trim().Select(character =>
                    invalidCharacters.Contains(character) ||
                    character == Path.DirectorySeparatorChar ||
                    character == Path.AltDirectorySeparatorChar ||
                    char.IsControl(character)
                        ? '_'
                        : character));
            sanitized = sanitized.Trim('.', ' ');
            if (string.IsNullOrWhiteSpace(sanitized))
            {
                return "pipeline";
            }

            return sanitized.Length <= 96
                ? sanitized
                : sanitized[..96];
        }
    }

}

public sealed record OrchestrationRuntimeRequest(
    string WorkspacePath,
    string PipelineWorkspacePath,
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    string DataTypeConversionWorkspacePath,
    string PipelineDbConnectionEnvironmentVariableName,
    int MaxDegreeOfParallelism,
    string PipelineExecutableName = "meta-pipeline",
    string RunArtifactsRootPath = "",
    string ExpectedWorkerExecutableVersion = "",
    OrchestrationLogCapturePolicy? LogCapturePolicy = null,
    TimeSpan? WorkerEventTimeout = null,
    TimeSpan? WorkerActivationTimeout = null,
    TimeSpan? WorkerControlPipeConnectTimeout = null);

public sealed record OrchestrationRuntimeResult(
    string RunPlanName,
    bool Succeeded,
    IReadOnlyList<OrchestrationTaskWorkerResult> TaskResults,
    IReadOnlyList<OrchestrationTaskBlockedResult> BlockedResults,
    Guid RunId,
    string RunArtifactDirectoryPath);

public sealed record OrchestrationTaskWorkerResult(
    string TaskAccessProfileId,
    string PlannedTaskId,
    string PipelineName,
    string StepName,
    int ExitCode,
    string StandardOutput,
    string StandardError,
    string GrantId = "",
    string CommandId = "",
    int AttemptNumber = 0);

public sealed record OrchestrationTaskBlockedResult(
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

public interface IOrchestrationRuntimeObserver
{
    void PhaseChanged(string phase);

    void RunPlanReady(int totalTasks);

    void TaskStarted(string taskId, string taskName);

    void TaskCompleted(string taskId, bool succeeded);

    void TaskBlocked(string taskId);
}
