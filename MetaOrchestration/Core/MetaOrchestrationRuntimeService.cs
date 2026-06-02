using System.Diagnostics;
using System.Globalization;
using System.IO.Pipes;
using MetaOrchestration.WorkerProtocol;
using MO = MetaOrchestration;

namespace MetaOrchestration.Core;

public sealed class MetaOrchestrationRuntimeService
{
    private static readonly TimeSpan DefaultWorkerEventTimeout = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan DefaultWorkerActivationTimeout = TimeSpan.FromSeconds(30);
    private const int MaxPreWorkWorkerReplacementAttempts = 3;

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
        if (request.WorkerEventTimeout is { } workerEventTimeout && workerEventTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("WorkerEventTimeout must be positive when provided.", nameof(request));
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
        var workerEventTimeout = request.WorkerEventTimeout ?? DefaultWorkerEventTimeout;
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
        var replacementCoordinator = new PipelineWorkerReplacementCoordinator();
        var workers = new List<PipelineWorkerProcess>();
        var workersByName = new Dictionary<string, PipelineWorkerProcess>(StringComparer.OrdinalIgnoreCase);
        try
        {
            foreach (var group in plannedTasks.GroupBy(static item => item.PipelineReference.Name, StringComparer.OrdinalIgnoreCase))
            {
                var pipelineReference = group.First().PipelineReference;
                var pipelineName = pipelineReference.Name;
                journal.WriteEvent("WorkerStarting", pipelineName, request.PipelineExecutableName);
                var worker = await PipelineWorkerProcess.StartAsync(
                    pipelineName,
                    pipelineReference.MetaPipelinePipelineId,
                    string.Empty,
                    request,
                    journal.RunDirectoryPath,
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
            }

            var eventTasksByWorker = workers.ToDictionary(
                static worker => worker,
                worker => worker.ReadEventAsync(cancellationToken));
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

            UpdateSupervisorState("WorkersStarted");
            AssertProjection("after workers started");

            while (kernel.HasRuntimeWork || eventTasksByWorker.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                UpdateSupervisorState("LoopStart");
                AssertProjection("loop start");
                if (await TryGrantReadyWorkerTaskAsync(
                    kernel,
                    eventTasksByWorker,
                    workers,
                    workersByName,
                    replacementCoordinator,
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
                    DateTimeOffset.UtcNow);
                if (timedOutWorker is not null)
                {
                    eventTasksByWorker.Remove(timedOutWorker);
                    UpdateSupervisorState("WorkerTimeout", timedOutWorker.PipelineName);
                    await HandleTimedOutWorkerAsync(
                        timedOutWorker,
                        workerEventTimeout,
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

                var worker = eventTasksByWorker.First(item => ReferenceEquals(item.Value, completedEventTask)).Key;
                eventTasksByWorker.Remove(worker);
                var workerEvent = await ((Task<WorkerProtocolEvent>)completedEventTask).ConfigureAwait(false);
                UpdateSupervisorState(workerEvent.Kind, worker.PipelineName);
                journal.WriteEvent(
                    "WorkerEvent",
                    string.IsNullOrWhiteSpace(workerEvent.TaskName)
                        ? worker.PipelineName
                        : $"{worker.PipelineName}.{workerEvent.TaskName}",
                    $"{workerEvent.Kind}; ExitCode={workerEvent.ExitCode.ToString(CultureInfo.InvariantCulture)}; {workerEvent.Message}");

                if (string.Equals(workerEvent.Kind, WorkerEventKinds.Closed, StringComparison.OrdinalIgnoreCase))
                {
                    await HandleClosedWorkerAsync(
                        worker,
                        workerEvent,
                        eventTasksByWorker,
                        workers,
                        workersByName,
                        replacementCoordinator,
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
                    continue;
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
                        replacementCoordinator,
                        kernel,
                        request,
                        journal,
                        cancellationToken).ConfigureAwait(false))
                {
                    UpdateSupervisorState("WorkerLifecycleHandled", worker.PipelineName);
                    AssertProjection("after worker lifecycle event");
                    continue;
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

    private static Task? CreateRetryWakeTask(
        IEnumerable<PipelineWorkerReadyState> readyStates,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var nextRetryAt = readyStates
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

    private static Task? CreateWorkerEventTimeoutWakeTask(
        IReadOnlyDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        OrchestrationRuntimeKernel kernel,
        TimeSpan workerEventTimeout,
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
            .Select(item => item.Worker.LastProtocolActivityUtc + ResolveWorkerProtocolTimeout(item.Decision, workerEventTimeout))
            .OrderBy(static item => item)
            .Cast<DateTimeOffset?>()
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
        TimeSpan workerEventTimeout,
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

            var timeout = ResolveWorkerProtocolTimeout(decision, workerEventTimeout);
            var elapsed = now - worker.LastProtocolActivityUtc;
            if (elapsed < timeout)
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

    private static void AssertRuntimeProjection(
        string stage,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId,
        IReadOnlyDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        IReadOnlySet<string> pendingTaskIds,
        IReadOnlyDictionary<string, PipelineWorkerReadyState> readyByTaskId,
        IReadOnlyDictionary<string, PipelineWorkerRetryState> scheduledRetryByTaskId,
        IReadOnlyDictionary<string, PipelineWorkerProcess> runningByTaskId,
        IReadOnlyDictionary<string, PipelineWorkerGrantState> runningGrantsByTaskId,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]> runningLocksByTaskId,
        OrchestrationExecutionStateMachine stateMachine)
    {
        void Fail(string message) =>
            throw new InvalidOperationException(
                $"Orchestration runtime projection invariant failed at {stage}: {message}");

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

        foreach (var taskId in runningByTaskId.Keys)
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

        var runningPlannedTaskIds = runningByTaskId.Keys
            .Select(taskId => plannedTasksByPipelineTaskId[taskId].Id)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var plannedTaskId in runningLocksByTaskId.Keys)
        {
            if (!runningPlannedTaskIds.Contains(plannedTaskId))
            {
                Fail($"running locks exist for planned task '{plannedTaskId}', but no active grant owns that planned task.");
            }
        }

        foreach (var plannedTask in plannedTasksByPipelineTaskId.Values)
        {
            if (runningByTaskId.ContainsKey(plannedTask.TaskAccessProfile.MetaPipelinePipelineTaskId) &&
                !runningLocksByTaskId.ContainsKey(plannedTask.Id))
            {
                Fail($"active task '{plannedTask.TaskAccessProfile.MetaPipelinePipelineTaskId}' has no running lock projection for planned task '{plannedTask.Id}'.");
            }
        }

        var runningOnly = runningByTaskId.Keys
            .Except(runningGrantsByTaskId.Keys, StringComparer.Ordinal)
            .FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(runningOnly))
        {
            Fail($"task '{runningOnly}' is running without an active grant projection.");
        }

        var grantOnly = runningGrantsByTaskId.Keys
            .Except(runningByTaskId.Keys, StringComparer.Ordinal)
            .FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(grantOnly))
        {
            Fail($"task '{grantOnly}' has an active grant projection without a running worker projection.");
        }

        var liveWorkerNames = eventTasksByWorker.Keys
            .Select(static item => item.PipelineName)
            .Concat(readyByTaskId.Values.Select(static item => item.Worker.PipelineName))
            .Concat(runningByTaskId.Values.Select(static item => item.PipelineName))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
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
            var inRunning = runningByTaskId.TryGetValue(snapshot.TaskId, out var runningWorker);
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

                    AssertActiveGrantProjection(snapshot, runningWorker!, runningGrant!, Fail);
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

    private static void AssertReadyProjection(
        OrchestrationTaskRuntimeSnapshot snapshot,
        PipelineWorkerReadyState ready,
        Action<string> fail)
    {
        if (!string.Equals(ready.Event.TaskId, snapshot.TaskId, StringComparison.Ordinal))
        {
            fail($"ready projection key '{snapshot.TaskId}' contains event task id '{ready.Event.TaskId}'.");
        }

        if (!string.Equals(ready.Worker.PipelineName, snapshot.WorkerName, StringComparison.OrdinalIgnoreCase))
        {
            fail($"ready projection for task '{snapshot.TaskId}' is owned by worker '{ready.Worker.PipelineName}', but kernel owner is '{snapshot.WorkerName}'.");
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
        PipelineWorkerRetryState scheduledRetry,
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
        PipelineWorkerProcess runningWorker,
        PipelineWorkerGrantState runningGrant,
        Action<string> fail)
    {
        if (!string.Equals(runningWorker.PipelineName, snapshot.WorkerName, StringComparison.OrdinalIgnoreCase))
        {
            fail($"active task '{snapshot.TaskId}' is running on worker '{runningWorker.PipelineName}', but kernel owner is '{snapshot.WorkerName}'.");
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

    private static TimeSpan ResolveWorkerProtocolTimeout(
        OrchestrationWorkerTimeoutDecision decision,
        TimeSpan workerEventTimeout) =>
        decision.WorkerHasReachedTaskBoundary || workerEventTimeout <= DefaultWorkerActivationTimeout
            ? workerEventTimeout
            : DefaultWorkerActivationTimeout;

    private static async Task<bool> HandleWorkerLifecycleEventAsync(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        IDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        ICollection<PipelineWorkerProcess> workers,
        IDictionary<string, PipelineWorkerProcess> workersByName,
        PipelineWorkerReplacementCoordinator replacementCoordinator,
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
                    replacementCoordinator,
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
        PipelineWorkerReplacementCoordinator replacementCoordinator,
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

        var replacement = await replacementCoordinator.StartPreWorkReplacementAsync(
            worker,
            decision,
            reason,
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
        PipelineWorkerReplacementCoordinator replacementCoordinator,
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
                    replacementCoordinator,
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
                        replacementCoordinator,
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

    private static string DescribeAllReadyNoProgress(
        IReadOnlyDictionary<string, PipelineWorkerReadyState> readyByTaskId,
        ISet<string> pendingTaskIds,
        IReadOnlyDictionary<string, PipelineWorkerProcess> runningByTaskId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependenciesByTaskProfileId,
        IReadOnlyDictionary<string, string> taskOutcomesByTaskProfileId)
    {
        var descriptions = new List<string>();
        foreach (var ready in readyByTaskId.Values
                     .OrderBy(static item => item.Event.PipelineName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(static item => item.Event.TaskName, StringComparer.OrdinalIgnoreCase))
        {
            if (!plannedTasksByPipelineTaskId.TryGetValue(ready.Event.TaskId, out var plannedTask))
            {
                descriptions.Add($"{ready.Event.PipelineName}.{ready.Event.TaskName} is TaskReady for task '{ready.Event.TaskId}', which is not in the run plan");
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
                    $"{FormatTaskName(plannedTask)} waits for {DescribeDependencyState(dependency, pendingTaskIds, readyByTaskId, runningByTaskId, plannedTasksByProfileId, taskOutcomesByTaskProfileId)}");
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

    private static string DescribeDependencyState(
        OrchestrationExecutionDependency dependency,
        ISet<string> pendingTaskIds,
        IReadOnlyDictionary<string, PipelineWorkerReadyState> readyByTaskId,
        IReadOnlyDictionary<string, PipelineWorkerProcess> runningByTaskId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId,
        IReadOnlyDictionary<string, string> taskOutcomesByTaskProfileId)
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

        if (runningByTaskId.ContainsKey(predecessorPipelineTaskId))
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

    private static OrchestrationTaskWorkerResult CompleteRunningTask(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        MO.PlannedTask plannedTask,
        int exitCode,
        Dictionary<string, string> taskOutcomesByTaskProfileId,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        IDictionary<string, PipelineWorkerProcess> runningByTaskId,
        IDictionary<string, PipelineWorkerGrantState> runningGrantsByTaskId,
        IDictionary<string, MO.PlannedTaskLock[]> runningLocksByTaskId,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer,
        bool recordTerminalOutcome = true)
    {
        runningByTaskId.Remove(workerEvent.TaskId);
        runningGrantsByTaskId.Remove(workerEvent.TaskId, out var grant);
        runningLocksByTaskId.Remove(plannedTask.Id);
        var standardError = string.IsNullOrWhiteSpace(workerEvent.Message)
            ? worker.StandardErrorText
            : string.Concat(worker.StandardErrorText, workerEvent.Message, Environment.NewLine);
        var result = new OrchestrationTaskWorkerResult(
            plannedTask.TaskAccessProfile.Id,
            plannedTask.Id,
            plannedTask.PipelineReference.Name,
            plannedTask.TaskAccessProfile.TaskName,
            exitCode,
            worker.StandardOutputText,
            standardError,
            workerEvent.GrantId,
            workerEvent.CommandId,
            workerEvent.AttemptNumber == 0 ? grant?.AttemptNumber ?? 0 : workerEvent.AttemptNumber);
        taskResults.Add(result);
        if (recordTerminalOutcome)
        {
            taskOutcomesByTaskProfileId[result.TaskAccessProfileId] = OrchestrationExecutionContinuity.OutcomeForExitCode(result.ExitCode);
            observer?.TaskCompleted(result.PlannedTaskId, result.ExitCode == 0);
        }

        journal.WriteEvent(
            result.ExitCode == 0
                ? "TaskSucceeded"
                : recordTerminalOutcome ? "TaskFailed" : "TaskAttemptFailed",
            FormatTaskName(plannedTask),
            result.ExitCode.ToString(CultureInfo.InvariantCulture));
        return result;
    }

    private static async Task HandleClosedWorkerAsync(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        IDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        ICollection<PipelineWorkerProcess> workers,
        IDictionary<string, PipelineWorkerProcess> workersByName,
        PipelineWorkerReplacementCoordinator replacementCoordinator,
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
            replacementCoordinator,
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
        PipelineWorkerReplacementCoordinator replacementCoordinator,
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
                    replacementCoordinator,
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
                    replacementCoordinator,
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

    private sealed class PipelineWorkerReplacementCoordinator
    {
        private readonly Dictionary<string, int> preWorkReplacementAttemptsByKey = new(StringComparer.OrdinalIgnoreCase);

        public async Task<PipelineWorkerProcess> StartPreWorkReplacementAsync(
            PipelineWorkerProcess worker,
            OrchestrationWorkerLossDecision decision,
            string reason,
            OrchestrationRuntimeRequest request,
            OrchestrationRunJournal journal,
            CancellationToken cancellationToken)
        {
            var replacementAttempt = ReservePreWorkReplacementAttempt(
                worker.PipelineName,
                decision.ResumeTaskId,
                reason);
            journal.WriteEvent(
                "WorkerReplacementReserved",
                worker.PipelineName,
                $"ResumeTaskId={decision.ResumeTaskId}; Attempt={replacementAttempt.ToString(CultureInfo.InvariantCulture)}; Limit={MaxPreWorkWorkerReplacementAttempts.ToString(CultureInfo.InvariantCulture)}");

            return await StartReplacementWorkerAsync(
                worker.PipelineName,
                worker.PipelineId,
                decision.ResumeTaskId,
                request,
                journal,
                cancellationToken).ConfigureAwait(false);
        }

        private int ReservePreWorkReplacementAttempt(
            string pipelineName,
            string resumeTaskId,
            string reason)
        {
            var key = CreatePreWorkReplacementAttemptKey(pipelineName, resumeTaskId);
            preWorkReplacementAttemptsByKey.TryGetValue(key, out var currentAttempt);
            var nextAttempt = currentAttempt + 1;
            if (nextAttempt > MaxPreWorkWorkerReplacementAttempts)
            {
                var boundary = string.IsNullOrWhiteSpace(resumeTaskId)
                    ? "before pipeline activation"
                    : $"before granting task '{resumeTaskId}'";
                throw new InvalidOperationException(
                    $"Pipeline worker '{pipelineName}' exceeded the pre-work worker replacement limit {boundary}. " +
                    $"Limit: {MaxPreWorkWorkerReplacementAttempts.ToString(CultureInfo.InvariantCulture)}. Last reason: {reason}");
            }

            preWorkReplacementAttemptsByKey[key] = nextAttempt;
            return nextAttempt;
        }

        private static string CreatePreWorkReplacementAttemptKey(string pipelineName, string resumeTaskId) =>
            string.Concat(pipelineName, "\u001f", resumeTaskId ?? string.Empty);
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
            cancellationToken).ConfigureAwait(false);
        journal.WriteEvent("WorkerRestarted", pipelineName, replacement.ProcessId.ToString(CultureInfo.InvariantCulture));
        journal.WriteEvent("WorkerControlPipe", pipelineName, replacement.ControlPipeName);
        journal.WriteEvent("WorkerLog", pipelineName, $"stdout={replacement.StandardOutputArtifactPath}; stderr={replacement.StandardErrorArtifactPath}");
        return replacement;
    }

    private static async Task HandleTimedOutWorkerAsync(
        PipelineWorkerProcess worker,
        TimeSpan workerEventTimeout,
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
        var reason = DescribeWorkerEventTimeout(decision, ResolveWorkerProtocolTimeout(decision, workerEventTimeout));
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

    private static void BlockRemainingPipelineTasks(
        string pipelineName,
        string reason,
        ISet<string> pendingTaskIds,
        IDictionary<string, PipelineWorkerReadyState> readyByTaskId,
        OrchestrationExecutionStateMachine stateMachine,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId,
        Dictionary<string, string> taskOutcomesByTaskProfileId,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer)
    {
        foreach (var taskId in pendingTaskIds
                     .Where(taskId => plannedTasksByPipelineTaskId.TryGetValue(taskId, out var candidate) &&
                                      string.Equals(candidate.PipelineReference.Name, pipelineName, StringComparison.OrdinalIgnoreCase))
                     .ToArray())
        {
            pendingTaskIds.Remove(taskId);
            readyByTaskId.Remove(taskId);
            stateMachine.MarkBlocked(taskId);
            var plannedTask = plannedTasksByPipelineTaskId[taskId];
            blockedResults.Add(CreateBlockedResult(
                plannedTask,
                OrchestrationExecutionDependency.Empty,
                OrchestrationExecutionContinuity.SkippedBlocked,
                reason,
                plannedTasksByProfileId));
            taskOutcomesByTaskProfileId[plannedTask.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.SkippedBlocked;
            observer?.TaskBlocked(plannedTask.Id);
            journal.WriteEvent("TaskBlocked", FormatTaskName(plannedTask), reason);
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

    private static int ResolveAttemptNumber(
        WorkerProtocolEvent workerEvent,
        IDictionary<string, PipelineWorkerGrantState> runningGrantsByTaskId)
    {
        if (workerEvent.AttemptNumber > 0)
        {
            return workerEvent.AttemptNumber;
        }

        return runningGrantsByTaskId.TryGetValue(workerEvent.TaskId, out var grant)
            ? Math.Max(1, grant.AttemptNumber)
            : 1;
    }

    private static string ResolveFailureClass(WorkerProtocolEvent workerEvent, string fallbackFailureClass) =>
        string.IsNullOrWhiteSpace(workerEvent.FailureClass)
            ? fallbackFailureClass
            : workerEvent.FailureClass.Trim();

    private static bool HasWriteEffect(IReadOnlyList<MO.PlannedTaskLock> plannedTaskLocks)
    {
        return plannedTaskLocks.Any(static item =>
            !string.Equals(item.LockMode, "SharedRead", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(item.TaskObjectEffect.WriteEffect, "None", StringComparison.OrdinalIgnoreCase));
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

    private static OrchestrationTaskBlockedResult CreateBlockedResult(
        MO.PlannedTask plannedTask,
        OrchestrationExecutionDependency dependency,
        string outcome,
        string reason,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId)
    {
        var blockingTaskProfileId = dependency.PredecessorTaskProfileId;
        var blockingTask = plannedTasksByProfileId.GetValueOrDefault(blockingTaskProfileId);
        return new OrchestrationTaskBlockedResult(
            plannedTask.Id,
            plannedTask.TaskAccessProfile.Id,
            plannedTask.PipelineReference.Name,
            plannedTask.TaskAccessProfile.TaskName,
            blockingTaskProfileId,
            blockingTask?.PipelineReference.Name ?? "<unknown>",
            blockingTask?.TaskAccessProfile.TaskName ?? blockingTaskProfileId,
            dependency.Condition,
            outcome,
            reason);
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
            CancellationToken cancellationToken)
        {
            var connectTask = serverPipe.WaitForConnectionAsync(cancellationToken);
            var exitTask = process.WaitForExitAsync(cancellationToken);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            var completedTask = await Task.WhenAny(connectTask, exitTask, timeoutTask).ConfigureAwait(false);
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
                $"Pipeline worker '{pipelineName}' did not connect to the orchestration control pipe within 10 seconds.");
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

    private sealed record PipelineWorkerReadyState(
        PipelineWorkerProcess Worker,
        WorkerProtocolEvent Event,
        DateTimeOffset NotBeforeUtc = default,
        string PreviousGrantId = "",
        int AttemptNumber = 1);

    private sealed record PipelineWorkerRetryState(
        DateTimeOffset NotBeforeUtc,
        string PreviousGrantId,
        int AttemptNumber);

    private sealed record PipelineWorkerGrantState(
        string PipelineId,
        string TaskId,
        string CommandId,
        string GrantId,
        string PreviousGrantId,
        int AttemptNumber)
    {
        public static PipelineWorkerGrantState Create(string pipelineId, string taskId, string previousGrantId = "", int attemptNumber = 1) =>
            new(
                pipelineId,
                taskId,
                Guid.NewGuid().ToString("N"),
                Guid.NewGuid().ToString("N"),
                previousGrantId,
                attemptNumber);
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
    TimeSpan? WorkerEventTimeout = null);

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
