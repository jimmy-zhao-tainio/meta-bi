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

        try
        {
            using var lease = OrchestrationWorkspaceExecutionLease.Acquire(workspacePath, runId, request.RunArtifactsRootPath);
            journal.WriteEvent("LeaseAcquired", workspacePath, lease.LeaseRecordPath);

            observer?.PhaseChanged("Loading");
            journal.WriteEvent("Phase", "Loading", workspacePath);
            var model = MO.MetaOrchestrationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            observer?.PhaseChanged("Building");
            journal.WriteEvent("Phase", "Building", workspacePath);
            new MetaOrchestrationRunPlanningService().BuildRunPlan(model);
            observer?.PhaseChanged("Saving");
            journal.WriteEvent("Phase", "Saving", workspacePath);
            model.SaveToXmlWorkspace(workspacePath);

            var runPlan = ResolveRunPlan(model);
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

            observer?.RunPlanReady(plannedTasks.Length);
            journal.WriteEvent("RunPlanReady", runPlan.Name, plannedTasks.Length.ToString(CultureInfo.InvariantCulture));
            var taskResults = new List<OrchestrationTaskWorkerResult>();
            var blockedResults = new List<OrchestrationTaskBlockedResult>();
            var taskOutcomesByTaskProfileId = new Dictionary<string, string>(StringComparer.Ordinal);
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

            await ExecuteWorkerGraphAsync(
                plannedTasks,
                locksByPlannedTaskId,
                activeLockPolicies,
                dependenciesByTaskProfileId,
                plannedTasksByProfileId,
                taskOutcomesByTaskProfileId,
                taskResults,
                blockedResults,
                retryPolicy,
                request,
                observer,
                journal,
                cancellationToken).ConfigureAwait(false);

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
            return result;
        }
        catch (Exception ex)
        {
            journal.WriteEvent("RunFailed", ex.GetType().Name, ex.Message);
            throw;
        }
    }

    private static async Task ExecuteWorkerGraphAsync(
        IReadOnlyList<MO.PlannedTask> plannedTasks,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]> locksByPlannedTaskId,
        IReadOnlyList<MO.LockCompatibilityPolicy> activeLockPolicies,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependenciesByTaskProfileId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId,
        Dictionary<string, string> taskOutcomesByTaskProfileId,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        ResolvedOrchestrationRetryPolicy retryPolicy,
        OrchestrationRuntimeRequest request,
        IOrchestrationRuntimeObserver? observer,
        OrchestrationRunJournal journal,
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

        var pendingTaskIds = plannedTasksByPipelineTaskId.Keys.ToHashSet(StringComparer.Ordinal);
        var readyByTaskId = new Dictionary<string, PipelineWorkerReadyState>(StringComparer.Ordinal);
        var runningByTaskId = new Dictionary<string, PipelineWorkerProcess>(StringComparer.Ordinal);
        var runningGrantsByTaskId = new Dictionary<string, PipelineWorkerGrantState>(StringComparer.Ordinal);
        var runningLocksByTaskId = new Dictionary<string, MO.PlannedTaskLock[]>(StringComparer.Ordinal);
        var stoppedPipelineNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var workers = new List<PipelineWorkerProcess>();
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
                    request,
                    journal.RunDirectoryPath,
                    cancellationToken).ConfigureAwait(false);
                journal.WriteEvent("WorkerStarted", pipelineName, worker.ProcessId.ToString(CultureInfo.InvariantCulture));
                journal.WriteEvent("WorkerControlPipe", pipelineName, worker.ControlPipeName);
                journal.WriteEvent("WorkerLog", pipelineName, $"stdout={worker.StandardOutputArtifactPath}; stderr={worker.StandardErrorArtifactPath}");
                workers.Add(worker);
            }

            var eventTasksByWorker = workers.ToDictionary(
                static worker => worker,
                worker => worker.ReadEventAsync(cancellationToken));

            while (pendingTaskIds.Count > 0 || runningByTaskId.Count > 0 || readyByTaskId.Count > 0 || eventTasksByWorker.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (await TryGrantReadyWorkerTaskAsync(
                    readyByTaskId,
                    runningByTaskId,
                    runningGrantsByTaskId,
                    runningLocksByTaskId,
                    stoppedPipelineNames,
                    pendingTaskIds,
                    plannedTasksByPipelineTaskId,
                    plannedTasksByProfileId,
                    locksByPlannedTaskId,
                    activeLockPolicies,
                    dependenciesByTaskProfileId,
                    taskOutcomesByTaskProfileId,
                    blockedResults,
                    request,
                    observer,
                    journal).ConfigureAwait(false))
                {
                    continue;
                }

                if (eventTasksByWorker.Count == 0)
                {
                    if (pendingTaskIds.Count > 0 || runningByTaskId.Count > 0 || readyByTaskId.Count > 0)
                    {
                        throw new InvalidOperationException("Cannot execute run plan because no pipeline worker can produce the remaining task events.");
                    }

                    break;
                }

                if (runningByTaskId.Count == 0 &&
                    eventTasksByWorker.Values.All(static task => !task.IsCompleted) &&
                    eventTasksByWorker.Keys.All(worker => !worker.HasExited && readyByTaskId.Values.Any(ready => ReferenceEquals(ready.Worker, worker))) &&
                    !readyByTaskId.Values.Any(static ready => ready.NotBeforeUtc > DateTimeOffset.UtcNow))
                {
                    var detail = DescribeAllReadyNoProgress(
                        readyByTaskId,
                        pendingTaskIds,
                        runningByTaskId,
                        plannedTasksByPipelineTaskId,
                        plannedTasksByProfileId,
                        dependenciesByTaskProfileId,
                        taskOutcomesByTaskProfileId);
                    journal.WriteEvent("NoProgress", "all-workers-ready", detail);
                    throw new InvalidOperationException(
                        "Cannot execute run plan because every live pipeline worker is waiting for an orchestration command, but no ready task can be granted. " + detail);
                }

                var timedOutWorker = FindTimedOutWorker(
                    eventTasksByWorker,
                    readyByTaskId,
                    runningByTaskId,
                    pendingTaskIds,
                    plannedTasksByPipelineTaskId,
                    workerEventTimeout,
                    DateTimeOffset.UtcNow);
                if (timedOutWorker is not null)
                {
                    HandleTimedOutWorker(
                        timedOutWorker,
                        workerEventTimeout,
                        stoppedPipelineNames,
                        pendingTaskIds,
                        readyByTaskId,
                        runningByTaskId,
                        runningGrantsByTaskId,
                        runningLocksByTaskId,
                        plannedTasksByPipelineTaskId,
                        plannedTasksByProfileId,
                        taskOutcomesByTaskProfileId,
                        taskResults,
                        blockedResults,
                        journal,
                        observer);
                    continue;
                }

                var retryWakeTask = CreateRetryWakeTask(readyByTaskId.Values, cancellationToken);
                var timeoutWakeTask = CreateWorkerEventTimeoutWakeTask(
                    eventTasksByWorker,
                    readyByTaskId,
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
                    continue;
                }
                if (ReferenceEquals(completedEventTask, timeoutWakeTask))
                {
                    continue;
                }

                var worker = eventTasksByWorker.First(item => ReferenceEquals(item.Value, completedEventTask)).Key;
                eventTasksByWorker.Remove(worker);
                var workerEvent = await ((Task<WorkerProtocolEvent>)completedEventTask).ConfigureAwait(false);
                journal.WriteEvent(
                    "WorkerEvent",
                    string.IsNullOrWhiteSpace(workerEvent.TaskName)
                        ? worker.PipelineName
                        : $"{worker.PipelineName}.{workerEvent.TaskName}",
                    $"{workerEvent.Kind}; ExitCode={workerEvent.ExitCode.ToString(CultureInfo.InvariantCulture)}; {workerEvent.Message}");

                if (string.Equals(workerEvent.Kind, WorkerEventKinds.Closed, StringComparison.OrdinalIgnoreCase))
                {
                    HandleClosedWorker(
                        worker,
                        workerEvent,
                        stoppedPipelineNames,
                        pendingTaskIds,
                        readyByTaskId,
                        runningByTaskId,
                        runningGrantsByTaskId,
                        runningLocksByTaskId,
                        plannedTasksByPipelineTaskId,
                        plannedTasksByProfileId,
                        taskOutcomesByTaskProfileId,
                        taskResults,
                        blockedResults,
                        journal,
                        observer);
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
                        journal,
                        cancellationToken).ConfigureAwait(false))
                {
                    continue;
                }

                ValidateWorkerEventAgainstState(
                    worker,
                    workerEvent,
                    pendingTaskIds,
                    readyByTaskId,
                    runningByTaskId);

                if (!plannedTasksByPipelineTaskId.TryGetValue(workerEvent.TaskId, out var plannedTask))
                {
                    throw new InvalidOperationException(
                        $"Pipeline worker '{worker.PipelineName}' emitted task id '{workerEvent.TaskId}' that is not present in the run plan.");
                }

                switch (workerEvent.Kind)
                {
                    case WorkerEventKinds.TaskReady:
                        worker.MarkTaskBoundaryReached();
                        readyByTaskId[workerEvent.TaskId] = new PipelineWorkerReadyState(worker, workerEvent);
                        eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
                        break;
                    case WorkerEventKinds.GrantAccepted:
                    case WorkerEventKinds.TaskStarted:
                        eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
                        break;
                    case WorkerEventKinds.TaskSucceeded:
                        CompleteRunningTask(
                            worker,
                            workerEvent,
                            plannedTask,
                            exitCode: 0,
                            taskOutcomesByTaskProfileId,
                            taskResults,
                            runningByTaskId,
                            runningGrantsByTaskId,
                            runningLocksByTaskId,
                            journal,
                            observer);
                        eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
                        break;
                    case WorkerEventKinds.TaskFailed:
                        await HandleTaskFailedAsync(
                            worker,
                            workerEvent,
                            plannedTask,
                            retryPolicy,
                            locksByPlannedTaskId,
                            readyByTaskId,
                            runningByTaskId,
                            runningGrantsByTaskId,
                            runningLocksByTaskId,
                            stoppedPipelineNames,
                            taskOutcomesByTaskProfileId,
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
        IReadOnlyDictionary<string, PipelineWorkerReadyState> readyByTaskId,
        TimeSpan workerEventTimeout,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var nextTimeoutAt = eventTasksByWorker
            .Where(static item => !item.Value.IsCompleted)
            .Select(static item => item.Key)
            .Where(worker => !WorkerIsWaitingAtTaskReady(worker, readyByTaskId))
            .Select(worker => worker.LastProtocolActivityUtc + ResolveWorkerProtocolTimeout(worker, workerEventTimeout))
            .Where(item => item > now)
            .OrderBy(static item => item)
            .FirstOrDefault();
        if (nextTimeoutAt <= DateTimeOffset.MinValue)
        {
            return null;
        }

        var delay = nextTimeoutAt - now;
        return delay <= TimeSpan.Zero
            ? Task.CompletedTask
            : Task.Delay(delay, cancellationToken);
    }

    private static PipelineWorkerProcess? FindTimedOutWorker(
        IReadOnlyDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        IReadOnlyDictionary<string, PipelineWorkerReadyState> readyByTaskId,
        IReadOnlyDictionary<string, PipelineWorkerProcess> runningByTaskId,
        ISet<string> pendingTaskIds,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId,
        TimeSpan workerEventTimeout,
        DateTimeOffset now)
    {
        foreach (var worker in eventTasksByWorker.Keys.OrderBy(static item => item.PipelineName, StringComparer.OrdinalIgnoreCase))
        {
            if (eventTasksByWorker[worker].IsCompleted ||
                WorkerIsWaitingAtTaskReady(worker, readyByTaskId))
            {
                continue;
            }

            var timeout = ResolveWorkerProtocolTimeout(worker, workerEventTimeout);
            var elapsed = now - worker.LastProtocolActivityUtc;
            if (elapsed < timeout)
            {
                continue;
            }

            var hasRunningTask = runningByTaskId.Values.Any(candidate => ReferenceEquals(candidate, worker));
            var hasPendingTask = pendingTaskIds.Any(taskId => plannedTasksByPipelineTaskId.TryGetValue(taskId, out var task) &&
                                                             string.Equals(task.PipelineReference.Name, worker.PipelineName, StringComparison.OrdinalIgnoreCase));
            if (hasRunningTask || hasPendingTask || !worker.HasExited)
            {
                return worker;
            }
        }

        return null;
    }

    private static bool WorkerIsWaitingAtTaskReady(
        PipelineWorkerProcess worker,
        IReadOnlyDictionary<string, PipelineWorkerReadyState> readyByTaskId) =>
        readyByTaskId.Values.Any(ready => ReferenceEquals(ready.Worker, worker));

    private static TimeSpan ResolveWorkerProtocolTimeout(
        PipelineWorkerProcess worker,
        TimeSpan workerEventTimeout) =>
        worker.TaskBoundaryReached || workerEventTimeout <= DefaultWorkerActivationTimeout
            ? workerEventTimeout
            : DefaultWorkerActivationTimeout;

    private static async Task<bool> HandleWorkerLifecycleEventAsync(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        IDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        OrchestrationRunJournal journal,
        CancellationToken cancellationToken)
    {
        if (string.Equals(workerEvent.Kind, WorkerEventKinds.WorkerOnline, StringComparison.OrdinalIgnoreCase))
        {
            worker.MarkOnline(workerEvent.ExecutableVersion);
            eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
            return true;
        }

        if (string.Equals(workerEvent.Kind, WorkerEventKinds.WorkerReady, StringComparison.OrdinalIgnoreCase))
        {
            if (!worker.IsOnline)
            {
                throw new InvalidOperationException(
                    $"Pipeline worker '{worker.PipelineName}' emitted {workerEvent.Kind} before {WorkerEventKinds.WorkerOnline}.");
            }

            await worker.SendStartPipelineAsync().ConfigureAwait(false);
            journal.WriteEvent("StartPipeline", worker.PipelineName, worker.PipelineId);
            eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
            return true;
        }

        if (string.Equals(workerEvent.Kind, WorkerEventKinds.PipelineStarted, StringComparison.OrdinalIgnoreCase))
        {
            if (!worker.IsOnline)
            {
                throw new InvalidOperationException(
                    $"Pipeline worker '{worker.PipelineName}' emitted {workerEvent.Kind} before {WorkerEventKinds.WorkerOnline}.");
            }

            if (!worker.StartPipelineSent)
            {
                throw new InvalidOperationException(
                    $"Pipeline worker '{worker.PipelineName}' emitted {workerEvent.Kind} before {WorkerCommandKinds.StartPipeline} was sent.");
            }

            worker.MarkPipelineStarted();
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
            if (!worker.IsOnline)
            {
                throw new InvalidOperationException(
                    $"Pipeline worker '{worker.PipelineName}' emitted {workerEvent.Kind} before {WorkerEventKinds.WorkerOnline}.");
            }

            eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
            return true;
        }

        return false;
    }

    private static void ValidateWorkerEventAgainstState(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        ISet<string> pendingTaskIds,
        IReadOnlyDictionary<string, PipelineWorkerReadyState> readyByTaskId,
        IReadOnlyDictionary<string, PipelineWorkerProcess> runningByTaskId)
    {
        if (!worker.IsOnline)
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.PipelineName}' emitted {workerEvent.Kind} before {WorkerEventKinds.WorkerOnline}.");
        }

        var readyTaskIdByWorkerName = readyByTaskId.Values
            .GroupBy(static item => item.Worker.PipelineName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                static group => group.Key,
                static group => group.Select(static item => item.Event.TaskId).First(),
                StringComparer.OrdinalIgnoreCase);
        var runningTaskIdByWorkerName = runningByTaskId
            .GroupBy(static item => item.Value.PipelineName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                static group => group.Key,
                static group => group.Select(static item => item.Key).First(),
                StringComparer.OrdinalIgnoreCase);
        OrchestrationRuntimeLiveness.ValidateWorkerEvent(
            worker.PipelineName,
            workerEvent.Kind,
            workerEvent.TaskId,
            pendingTaskIds,
            readyTaskIdByWorkerName,
            runningTaskIdByWorkerName);
    }

    private static async Task<bool> TryGrantReadyWorkerTaskAsync(
        IDictionary<string, PipelineWorkerReadyState> readyByTaskId,
        IDictionary<string, PipelineWorkerProcess> runningByTaskId,
        IDictionary<string, PipelineWorkerGrantState> runningGrantsByTaskId,
        IDictionary<string, MO.PlannedTaskLock[]> runningLocksByTaskId,
        ISet<string> stoppedPipelineNames,
        ISet<string> pendingTaskIds,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]> locksByPlannedTaskId,
        IReadOnlyList<MO.LockCompatibilityPolicy> activeLockPolicies,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependenciesByTaskProfileId,
        Dictionary<string, string> taskOutcomesByTaskProfileId,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        OrchestrationRuntimeRequest request,
        IOrchestrationRuntimeObserver? observer,
        OrchestrationRunJournal journal)
    {
        foreach (var ready in readyByTaskId.Values
                     .OrderBy(static item => item.Event.PipelineName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(static item => item.Event.TaskName, StringComparer.OrdinalIgnoreCase)
                     .ToArray())
        {
            if (ready.NotBeforeUtc > DateTimeOffset.UtcNow)
            {
                continue;
            }

            if (ready.Worker.HasExited)
            {
                journal.WriteEvent("GrantDeferred", ready.Event.PipelineName, $"worker exited before task {ready.Event.TaskName} could be granted");
                continue;
            }

            if (runningByTaskId.Count >= request.MaxDegreeOfParallelism)
            {
                return false;
            }

            var plannedTask = plannedTasksByPipelineTaskId[ready.Event.TaskId];
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
                await StopPipelineAtBlockedTaskAsync(
                    ready,
                    plannedTask,
                    dependency,
                    blockedOutcome,
                    blockedReason,
                    stoppedPipelineNames,
                    pendingTaskIds,
                    readyByTaskId,
                    plannedTasksByPipelineTaskId,
                    plannedTasksByProfileId,
                    taskOutcomesByTaskProfileId,
                    blockedResults,
                    observer,
                    journal).ConfigureAwait(false);
                return true;
            }

            var plannedTaskLocks = locksByPlannedTaskId.TryGetValue(plannedTask.Id, out var locks)
                ? locks
                : [];
            if (!AreLocksCompatibleWithRunning(
                    plannedTaskLocks,
                    runningLocksByTaskId.Values.SelectMany(static item => item).ToArray(),
                    activeLockPolicies))
            {
                continue;
            }

            var grant = PipelineWorkerGrantState.Create(
                ready.Event.PipelineId,
                ready.Event.TaskId,
                ready.PreviousGrantId,
                ready.AttemptNumber <= 0 ? 1 : ready.AttemptNumber);
            await ready.Worker.SendGrantTaskAsync(grant).ConfigureAwait(false);
            runningByTaskId[ready.Event.TaskId] = ready.Worker;
            runningGrantsByTaskId[ready.Event.TaskId] = grant;
            runningLocksByTaskId[plannedTask.Id] = plannedTaskLocks;
            readyByTaskId.Remove(ready.Event.TaskId);
            pendingTaskIds.Remove(ready.Event.TaskId);
            observer?.TaskStarted(plannedTask.Id, FormatTaskName(plannedTask));
            journal.WriteEvent(
                "GrantTask",
                FormatTaskName(plannedTask),
                string.IsNullOrWhiteSpace(grant.PreviousGrantId)
                    ? $"{ready.Event.TaskId}; GrantId={grant.GrantId}; Attempt={grant.AttemptNumber.ToString(CultureInfo.InvariantCulture)}"
                    : $"{ready.Event.TaskId}; GrantId={grant.GrantId}; PreviousGrantId={grant.PreviousGrantId}; Attempt={grant.AttemptNumber.ToString(CultureInfo.InvariantCulture)}");
            return true;
        }

        return false;
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
        PipelineWorkerReadyState ready,
        MO.PlannedTask blockedTask,
        OrchestrationExecutionDependency dependency,
        string blockedOutcome,
        string blockedReason,
        ISet<string> stoppedPipelineNames,
        ISet<string> pendingTaskIds,
        IDictionary<string, PipelineWorkerReadyState> readyByTaskId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId,
        Dictionary<string, string> taskOutcomesByTaskProfileId,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        IOrchestrationRuntimeObserver? observer,
        OrchestrationRunJournal journal)
    {
        readyByTaskId.Remove(ready.Event.TaskId);
        stoppedPipelineNames.Add(blockedTask.PipelineReference.Name);
        foreach (var taskId in pendingTaskIds
                     .Where(taskId => plannedTasksByPipelineTaskId.TryGetValue(taskId, out var candidate) &&
                                      string.Equals(candidate.PipelineReference.Name, blockedTask.PipelineReference.Name, StringComparison.OrdinalIgnoreCase))
                     .ToArray())
        {
            pendingTaskIds.Remove(taskId);
            readyByTaskId.Remove(taskId);
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
            blockedResults.Add(CreateBlockedResult(
                plannedTask,
                taskDependency,
                outcome,
                reason,
                plannedTasksByProfileId));
            taskOutcomesByTaskProfileId[plannedTask.TaskAccessProfile.Id] = outcome;
            observer?.TaskBlocked(plannedTask.Id);
            journal.WriteEvent("TaskBlocked", FormatTaskName(plannedTask), reason);
        }

        journal.WriteEvent("StopPipeline", blockedTask.PipelineReference.Name, blockedReason);
        await ready.Worker.SendStopPipelineAsync(ready.Event.PipelineId, ready.Event.TaskId, blockedReason).ConfigureAwait(false);
    }

    private static async Task HandleTaskFailedAsync(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        MO.PlannedTask plannedTask,
        ResolvedOrchestrationRetryPolicy retryPolicy,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]> locksByPlannedTaskId,
        IDictionary<string, PipelineWorkerReadyState> readyByTaskId,
        IDictionary<string, PipelineWorkerProcess> runningByTaskId,
        IDictionary<string, PipelineWorkerGrantState> runningGrantsByTaskId,
        IDictionary<string, MO.PlannedTaskLock[]> runningLocksByTaskId,
        ISet<string> stoppedPipelineNames,
        Dictionary<string, string> taskOutcomesByTaskProfileId,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer,
        CancellationToken cancellationToken)
    {
        var exitCode = workerEvent.ExitCode == 0 ? 4 : workerEvent.ExitCode;
        var attemptNumber = ResolveAttemptNumber(workerEvent, runningGrantsByTaskId);
        var failureClass = ResolveFailureClass(workerEvent, WorkerFailureClasses.WorkerReportedRetryable);
        var plannedTaskLocks = locksByPlannedTaskId.TryGetValue(plannedTask.Id, out var locks)
            ? locks
            : [];
        var isTaskRetrySafe = retryPolicy.IsTaskRetrySafe(HasWriteEffect(plannedTaskLocks));
        var retryDecision = retryPolicy.Evaluate(new OrchestrationRetryEvaluationContext(
            workerEvent.TaskId,
            attemptNumber,
            failureClass,
            isTaskRetrySafe,
            exitCode,
            workerEvent.Message));

        var attemptResult = CompleteRunningTask(
            worker,
            workerEvent,
            plannedTask,
            exitCode,
            taskOutcomesByTaskProfileId,
            taskResults,
            runningByTaskId,
            runningGrantsByTaskId,
            runningLocksByTaskId,
            journal,
            observer,
            recordTerminalOutcome: !retryDecision.ShouldRetry);

        if (!retryDecision.ShouldRetry)
        {
            stoppedPipelineNames.Add(worker.PipelineName);
            journal.WriteEvent(
                "RetryExhausted",
                FormatTaskName(plannedTask),
                $"{failureClass}; Attempt={attemptResult.AttemptNumber.ToString(CultureInfo.InvariantCulture)}; {retryDecision.Reason}");
            await worker.SendFailPipelineAsync(workerEvent.PipelineId, workerEvent.TaskId, retryDecision.Reason).ConfigureAwait(false);
            return;
        }

        journal.WriteEvent(
            WorkerEventKinds.RetryScheduled,
            FormatTaskName(plannedTask),
            retryDecision.Delay > TimeSpan.Zero
                ? $"{failureClass}; NextAttempt={retryDecision.NextAttemptNumber.ToString(CultureInfo.InvariantCulture)}; Delay={retryDecision.Delay.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)}ms; {retryDecision.Reason}"
                : $"{failureClass}; NextAttempt={retryDecision.NextAttemptNumber.ToString(CultureInfo.InvariantCulture)}; {retryDecision.Reason}");
        readyByTaskId[workerEvent.TaskId] = new PipelineWorkerReadyState(
            worker,
            workerEvent with { Kind = WorkerEventKinds.TaskReady, GrantId = string.Empty, CommandId = string.Empty, AttemptNumber = 0 },
            DateTimeOffset.UtcNow + retryDecision.Delay,
            attemptResult.GrantId,
            retryDecision.NextAttemptNumber);
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

    private static void HandleClosedWorker(
        PipelineWorkerProcess worker,
        WorkerProtocolEvent workerEvent,
        ISet<string> stoppedPipelineNames,
        ISet<string> pendingTaskIds,
        IDictionary<string, PipelineWorkerReadyState> readyByTaskId,
        IDictionary<string, PipelineWorkerProcess> runningByTaskId,
        IDictionary<string, PipelineWorkerGrantState> runningGrantsByTaskId,
        IDictionary<string, MO.PlannedTaskLock[]> runningLocksByTaskId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId,
        Dictionary<string, string> taskOutcomesByTaskProfileId,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer)
    {
        var runningTaskId = runningByTaskId
            .Where(item => ReferenceEquals(item.Value, worker))
            .Select(static item => item.Key)
            .FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(runningTaskId) &&
            plannedTasksByPipelineTaskId.TryGetValue(runningTaskId, out var runningPlannedTask))
        {
            CompleteRunningTask(
                worker,
                workerEvent with { TaskId = runningTaskId, TaskName = runningPlannedTask.TaskAccessProfile.TaskName, ExitCode = workerEvent.ExitCode == 0 ? 4 : workerEvent.ExitCode },
                runningPlannedTask,
                workerEvent.ExitCode == 0 ? 4 : workerEvent.ExitCode,
                taskOutcomesByTaskProfileId,
                taskResults,
                runningByTaskId,
                runningGrantsByTaskId,
                runningLocksByTaskId,
                journal,
                observer);
            BlockRemainingPipelineTasks(
                worker.PipelineName,
                $"pipeline worker exited while task {runningPlannedTask.TaskAccessProfile.TaskName} was running",
                pendingTaskIds,
                readyByTaskId,
                plannedTasksByPipelineTaskId,
                plannedTasksByProfileId,
                taskOutcomesByTaskProfileId,
                blockedResults,
                journal,
                observer);
            return;
        }

        var stoppedByOrchestration = stoppedPipelineNames.Contains(worker.PipelineName);
        var workerHasUnresolvedTasks =
            pendingTaskIds.Any(taskId => plannedTasksByPipelineTaskId.TryGetValue(taskId, out var task) &&
                                         string.Equals(task.PipelineReference.Name, worker.PipelineName, StringComparison.OrdinalIgnoreCase)) ||
            readyByTaskId.Values.Any(item => ReferenceEquals(item.Worker, worker));
        var workerHasRecordedFailure = taskResults.Any(result =>
            string.Equals(result.PipelineName, worker.PipelineName, StringComparison.OrdinalIgnoreCase) &&
            result.ExitCode != 0);
        if (workerHasRecordedFailure && workerHasUnresolvedTasks)
        {
            var failedTask = taskResults
                .Where(result => string.Equals(result.PipelineName, worker.PipelineName, StringComparison.OrdinalIgnoreCase) && result.ExitCode != 0)
                .LastOrDefault();
            BlockRemainingPipelineTasks(
                worker.PipelineName,
                failedTask is null
                    ? "pipeline stopped after failed task"
                    : $"pipeline stopped after failed task {failedTask.StepName}",
                pendingTaskIds,
                readyByTaskId,
                plannedTasksByPipelineTaskId,
                plannedTasksByProfileId,
                taskOutcomesByTaskProfileId,
                blockedResults,
                journal,
                observer);
            return;
        }

        if (workerHasUnresolvedTasks || (!stoppedByOrchestration && workerEvent.ExitCode != 0 && !workerHasRecordedFailure))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.PipelineName}' exited before all of its run-plan tasks were resolved. ExitCode: {workerEvent.ExitCode.ToString(CultureInfo.InvariantCulture)}.");
        }
    }

    private static void HandleTimedOutWorker(
        PipelineWorkerProcess worker,
        TimeSpan workerEventTimeout,
        ISet<string> stoppedPipelineNames,
        ISet<string> pendingTaskIds,
        IDictionary<string, PipelineWorkerReadyState> readyByTaskId,
        IDictionary<string, PipelineWorkerProcess> runningByTaskId,
        IDictionary<string, PipelineWorkerGrantState> runningGrantsByTaskId,
        IDictionary<string, MO.PlannedTaskLock[]> runningLocksByTaskId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByPipelineTaskId,
        IReadOnlyDictionary<string, MO.PlannedTask> plannedTasksByProfileId,
        Dictionary<string, string> taskOutcomesByTaskProfileId,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer)
    {
        var reason = DescribeWorkerEventTimeout(worker, ResolveWorkerProtocolTimeout(worker, workerEventTimeout));
        journal.WriteEvent("WorkerProtocolTimeout", worker.PipelineName, reason);
        worker.Terminate(reason);

        var runningTaskId = runningByTaskId
            .Where(item => ReferenceEquals(item.Value, worker))
            .Select(static item => item.Key)
            .FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(runningTaskId) &&
            plannedTasksByPipelineTaskId.TryGetValue(runningTaskId, out var runningPlannedTask))
        {
            runningGrantsByTaskId.TryGetValue(runningTaskId, out var grant);
            CompleteRunningTask(
                worker,
                new WorkerProtocolEvent(
                    WorkerEventKinds.TaskFailed,
                    worker.ProcessId.ToString(CultureInfo.InvariantCulture),
                    worker.PipelineId,
                    worker.PipelineName,
                    runningTaskId,
                    runningPlannedTask.TaskAccessProfile.TaskName,
                    grant?.GrantId ?? string.Empty,
                    grant?.CommandId ?? string.Empty,
                    grant?.AttemptNumber ?? 0,
                    4,
                    string.Empty,
                    reason,
                    WorkerFailureClasses.TaskTimeout),
                runningPlannedTask,
                exitCode: 4,
                taskOutcomesByTaskProfileId,
                taskResults,
                runningByTaskId,
                runningGrantsByTaskId,
                runningLocksByTaskId,
                journal,
                observer);
            stoppedPipelineNames.Add(worker.PipelineName);
            BlockRemainingPipelineTasks(
                worker.PipelineName,
                reason,
                pendingTaskIds,
                readyByTaskId,
                plannedTasksByPipelineTaskId,
                plannedTasksByProfileId,
                taskOutcomesByTaskProfileId,
                blockedResults,
                journal,
                observer);
            return;
        }

        var workerHasUnresolvedTasks =
            pendingTaskIds.Any(taskId => plannedTasksByPipelineTaskId.TryGetValue(taskId, out var task) &&
                                         string.Equals(task.PipelineReference.Name, worker.PipelineName, StringComparison.OrdinalIgnoreCase)) ||
            readyByTaskId.Values.Any(item => ReferenceEquals(item.Worker, worker));
        if (workerHasUnresolvedTasks)
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{worker.PipelineName}' stopped responding before all of its run-plan tasks were resolved. {reason}");
        }
    }

    private static string DescribeWorkerEventTimeout(
        PipelineWorkerProcess worker,
        TimeSpan workerEventTimeout)
    {
        var timeoutText = FormatTimeout(workerEventTimeout);
        if (!worker.IsOnline)
        {
            return $"No worker protocol event was received within {timeoutText}; the worker did not emit {WorkerEventKinds.WorkerOnline}.";
        }

        if (!worker.StartPipelineSent)
        {
            return $"No worker protocol event was received within {timeoutText}; the worker did not emit {WorkerEventKinds.WorkerReady}.";
        }

        if (!worker.PipelineStarted)
        {
            return $"No worker protocol event was received within {timeoutText}; the worker did not emit {WorkerEventKinds.PipelineStarted} after {WorkerCommandKinds.StartPipeline}.";
        }

        return $"No worker protocol event was received within {timeoutText} after the last command or event; the active task outcome is unknown.";
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
        private bool online;
        private bool startPipelineSent;
        private bool pipelineStarted;
        private bool taskBoundaryReached;

        private PipelineWorkerProcess(
            string pipelineName,
            string pipelineId,
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

        public string ControlPipeName { get; }

        public int ProcessId => process.Id;

        public bool IsOnline => online;

        public bool StartPipelineSent => startPipelineSent;

        public bool PipelineStarted => pipelineStarted;

        public bool TaskBoundaryReached => taskBoundaryReached;

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
                process,
                OrchestrationWorkerProtocolChannel.FromConnectedStream(serverPipe),
                expectedVersion,
                controlPipeName,
                standardOutput,
                standardError,
                standardOutputPump,
                standardErrorPump);
        }

        public void MarkOnline(string executableVersion)
        {
            if (online)
            {
                throw new InvalidOperationException(
                    $"Pipeline worker '{PipelineName}' emitted WorkerOnline more than once.");
            }

            if (string.IsNullOrWhiteSpace(executableVersion))
            {
                throw new InvalidOperationException(
                    $"Pipeline worker '{PipelineName}' did not report an executable version.");
            }

            if (!string.Equals(executableVersion, expectedExecutableVersion, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Pipeline worker '{PipelineName}' executable version mismatch. Expected '{expectedExecutableVersion}', got '{executableVersion}'.");
            }

            online = true;
        }

        public void MarkPipelineStarted()
        {
            if (pipelineStarted)
            {
                throw new InvalidOperationException(
                    $"Pipeline worker '{PipelineName}' emitted {WorkerEventKinds.PipelineStarted} more than once.");
            }

            pipelineStarted = true;
        }

        public void MarkTaskBoundaryReached()
        {
            taskBoundaryReached = true;
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
            if (startPipelineSent)
            {
                throw new InvalidOperationException(
                    $"Pipeline worker '{PipelineName}' has already received {WorkerCommandKinds.StartPipeline}.");
            }

            await SendCommandAsync(new WorkerProtocolCommand(
                WorkerCommandKinds.StartPipeline,
                Guid.NewGuid().ToString("N"),
                string.Empty,
                string.Empty,
                0,
                PipelineId,
                PipelineName,
                string.Empty,
                "activate pipeline")).ConfigureAwait(false);
            startPipelineSent = true;
        }

        public Task SendGrantTaskAsync(PipelineWorkerGrantState grant) =>
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
