using System.Diagnostics;
using System.Globalization;
using System.IO.Pipes;
using MetaOrchestration.Core.Runtime;
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
            var runtimeDefinition = RuntimeDefinitionFactory.Create(
                model,
                runPlan,
                plannedTasks,
                locksByPlannedTaskId,
                dependenciesByTaskProfileId,
                retryPolicy);
            var taskOutcomesByTaskProfileId = await ExecuteWorkerGraphAsync(
                runtimeDefinition,
                taskResults,
                blockedResults,
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
        RuntimeDefinition runtimeDefinition,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        OrchestrationRuntimeRequest request,
        IOrchestrationRuntimeObserver? observer,
        OrchestrationRunJournal journal,
        OrchestrationSupervisorRunState supervisorState,
        CancellationToken cancellationToken)
    {
        var workerEventTimeout = NormalizeOptionalTimeout(request.WorkerEventTimeout);
        var workerActivationTimeout = ResolveWorkerActivationTimeout(request.WorkerActivationTimeout, workerEventTimeout);
        var workerControlPipeConnectTimeout = NormalizeOptionalTimeout(request.WorkerControlPipeConnectTimeout);
        var expectedVersion = string.IsNullOrWhiteSpace(request.ExpectedWorkerExecutableVersion)
            ? OrchestrationWorkerProtocol.ResolveExecutableVersion(typeof(MetaOrchestrationRuntimeService).Assembly)
            : request.ExpectedWorkerExecutableVersion;
        runtimeDefinition = runtimeDefinition with { ExpectedWorkerExecutableVersion = expectedVersion };
        var kernel = new MetaOrchestrationRuntimeKernel(new RuntimeState(runtimeDefinition));
        var workers = new List<PipelineWorkerProcess>();
        var workersByName = new Dictionary<string, PipelineWorkerProcess>(StringComparer.OrdinalIgnoreCase);
        var eventTasksByWorker = new Dictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>>();
        var snapshot = kernel.Snapshot;

        async Task<KernelResult> SubmitAsync(RuntimeEvent runtimeEvent)
        {
            var result = kernel.RegisterEvent(runtimeEvent);
            snapshot = result.Snapshot;
            supervisorState.SetRuntimeCounts(
                snapshot.PendingCount,
                snapshot.ReadyCount,
                snapshot.RunningGrantCount,
                snapshot.RetryCount,
                taskResults.Count,
                blockedResults.Count);
            supervisorState.SetLiveWorkers(eventTasksByWorker.Keys.Select(static worker => worker.PipelineName));

            foreach (var action in result.Actions)
            {
                await ExecuteRuntimeActionAsync(action).ConfigureAwait(false);
            }

            return result;
        }

        async Task ExecuteRuntimeActionAsync(RuntimeAction action)
        {
            switch (action)
            {
                case RuntimeAction.PublishSnapshot publish:
                    snapshot = publish.Snapshot;
                    return;
                case RuntimeAction.StartWorker start:
                    if (workersByName.TryGetValue(start.WorkerName, out var existingWorker))
                    {
                        eventTasksByWorker.Remove(existingWorker);
                        if (!existingWorker.HasExited)
                        {
                            existingWorker.Terminate("worker replaced by orchestration");
                        }
                    }

                    journal.WriteEvent(
                        string.IsNullOrWhiteSpace(start.ResumeTaskId) ? "WorkerStarting" : "WorkerResuming",
                        start.WorkerName,
                        string.IsNullOrWhiteSpace(start.ResumeTaskId)
                            ? request.PipelineExecutableName
                            : $"ResumeTaskId={start.ResumeTaskId}; Executable={request.PipelineExecutableName}");
                    var worker = await PipelineWorkerProcess.StartAsync(
                        start.WorkerName,
                        start.PipelineId,
                        start.ResumeTaskId,
                        request,
                        journal.RunDirectoryPath,
                        workerControlPipeConnectTimeout,
                        cancellationToken).ConfigureAwait(false);
                    journal.WriteEvent("WorkerStarted", worker.PipelineName, worker.ProcessId.ToString(CultureInfo.InvariantCulture));
                    journal.WriteEvent("WorkerControlPipe", worker.PipelineName, worker.ControlPipeName);
                    journal.WriteEvent("WorkerLog", worker.PipelineName, $"stdout={worker.StandardOutputArtifactPath}; stderr={worker.StandardErrorArtifactPath}");
                    workers.Add(worker);
                    workersByName[worker.PipelineName] = worker;
                    eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
                    return;
                case RuntimeAction.SendStartPipeline startPipeline:
                    if (!workersByName.TryGetValue(startPipeline.WorkerName, out var startWorker) || startWorker.HasExited)
                    {
                        journal.WriteEvent("StartPipelineCommandLost", startPipeline.WorkerName, "worker process was no longer live");
                        await SubmitAsync(new RuntimeEvent.WorkerClosed(startPipeline.WorkerName, -1, "StartPipeline command lost")).ConfigureAwait(false);
                        return;
                    }

                    try
                    {
                        await startWorker.SendStartPipelineAsync().ConfigureAwait(false);
                    }
                    catch (InvalidOperationException ex)
                    {
                        journal.WriteEvent("StartPipelineCommandLost", startPipeline.WorkerName, ex.Message);
                        await SubmitAsync(new RuntimeEvent.WorkerClosed(startPipeline.WorkerName, -1, ex.Message)).ConfigureAwait(false);
                        return;
                    }

                    journal.WriteEvent("StartPipeline", startPipeline.WorkerName, startPipeline.PipelineId);
                    await SubmitAsync(new RuntimeEvent.StartPipelineAcknowledged(startPipeline.WorkerName)).ConfigureAwait(false);
                    return;
                case RuntimeAction.IssueGrant issue:
                    if (!workersByName.TryGetValue(issue.WorkerName, out var grantWorker) || grantWorker.HasExited)
                    {
                        journal.WriteEvent("GrantTaskCommandLost", issue.WorkerName, "worker process was no longer live");
                        await SubmitAsync(new RuntimeEvent.GrantDeliveryFailed(
                            issue.WorkerName,
                            issue.TaskId,
                            issue.Grant.GrantId,
                            issue.Grant.CommandId,
                            issue.Grant.AttemptNumber,
                            "GrantTask command lost")).ConfigureAwait(false);
                        return;
                    }

                    try
                    {
                        await grantWorker.SendGrantTaskAsync(issue.Grant).ConfigureAwait(false);
                    }
                    catch (InvalidOperationException ex)
                    {
                        journal.WriteEvent("GrantTaskCommandLost", issue.WorkerName, ex.Message);
                        await SubmitAsync(new RuntimeEvent.GrantDeliveryFailed(
                            issue.WorkerName,
                            issue.TaskId,
                            issue.Grant.GrantId,
                            issue.Grant.CommandId,
                            issue.Grant.AttemptNumber,
                            ex.Message)).ConfigureAwait(false);
                        return;
                    }

                    observer?.TaskStarted(issue.TaskId, $"{issue.WorkerName}.{issue.TaskName}");
                    journal.WriteEvent(
                        "GrantTask",
                        $"{issue.WorkerName}.{issue.TaskName}",
                        string.IsNullOrWhiteSpace(issue.Grant.PreviousGrantId)
                            ? $"{issue.Grant.TaskId}; GrantId={issue.Grant.GrantId}; Attempt={issue.Grant.AttemptNumber.ToString(CultureInfo.InvariantCulture)}"
                            : $"{issue.Grant.TaskId}; GrantId={issue.Grant.GrantId}; PreviousGrantId={issue.Grant.PreviousGrantId}; Attempt={issue.Grant.AttemptNumber.ToString(CultureInfo.InvariantCulture)}");
                    return;
                case RuntimeAction.SendStopPipeline stop:
                    journal.WriteEvent("StopPipeline", stop.PipelineName, stop.Reason);
                    if (!workersByName.TryGetValue(stop.PipelineName, out var stopWorker) || stopWorker.HasExited)
                    {
                        journal.WriteEvent("StopPipelineCommandLost", stop.PipelineName, "worker process was no longer live");
                        return;
                    }

                    try
                    {
                        await stopWorker.SendStopPipelineAsync(stop.PipelineId, stop.BlockingTaskId, stop.Reason).ConfigureAwait(false);
                    }
                    catch (InvalidOperationException ex)
                    {
                        journal.WriteEvent("StopPipelineCommandLost", stop.PipelineName, ex.Message);
                    }

                    return;
                case RuntimeAction.MarkPipelineFailed failed:
                    journal.WriteEvent("PipelineFailed", failed.PipelineName, $"{failed.FailureClass}; {failed.Reason}");
                    if (workersByName.TryGetValue(failed.PipelineName, out var failedWorker) && !failedWorker.HasExited)
                    {
                        try
                        {
                            await failedWorker.SendFailPipelineAsync(failed.PipelineId, failed.TaskId, failed.Reason).ConfigureAwait(false);
                        }
                        catch (InvalidOperationException ex)
                        {
                            journal.WriteEvent("FailPipelineCommandLost", failed.PipelineName, ex.Message);
                        }
                    }

                    return;
                case RuntimeAction.ScheduleRetry retry:
                    journal.WriteEvent(
                        WorkerEventKinds.RetryScheduled,
                        retry.TaskId,
                        retry.DueAtUtc > DateTimeOffset.UtcNow
                            ? $"NextAttempt={retry.AttemptNumber.ToString(CultureInfo.InvariantCulture)}; Delay={(retry.DueAtUtc - DateTimeOffset.UtcNow).TotalMilliseconds.ToString(CultureInfo.InvariantCulture)}ms"
                            : $"NextAttempt={retry.AttemptNumber.ToString(CultureInfo.InvariantCulture)}");
                    return;
                case RuntimeAction.RecordTaskCompletion completion:
                    RecordRuntimeTaskCompletion(completion.Completion, workersByName, taskResults, journal, observer);
                    return;
                case RuntimeAction.RecordBlockedTasks blocked:
                    RecordRuntimeBlockedTasks(blocked.BlockedTasks, blockedResults, journal, observer);
                    return;
                case RuntimeAction.WriteJournalEntry entry:
                    journal.WriteEvent(entry.EventKind, entry.Subject, entry.Detail);
                    return;
                case RuntimeAction.NotifyObserver notify:
                    if (string.Equals(notify.EventKind, "TaskBlocked", StringComparison.OrdinalIgnoreCase))
                    {
                        observer?.TaskBlocked(notify.Subject);
                    }

                    return;
                default:
                    throw new InvalidOperationException($"Unsupported runtime action '{action.GetType().Name}'.");
            }
        }

        async Task ProcessWorkerEventAsync(Task<WorkerProtocolEvent> completedEventTask)
        {
            var worker = eventTasksByWorker.First(item => ReferenceEquals(item.Value, completedEventTask)).Key;
            eventTasksByWorker.Remove(worker);
            var workerEvent = await completedEventTask.ConfigureAwait(false);
            worker.MarkProtocolEventObservedBySupervisor();
            supervisorState.NoteEvent(workerEvent.Kind, worker.PipelineName);
            journal.WriteEvent(
                "WorkerEvent",
                string.IsNullOrWhiteSpace(workerEvent.TaskName)
                    ? worker.PipelineName
                    : $"{worker.PipelineName}.{workerEvent.TaskName}",
                $"{workerEvent.Kind}; ExitCode={workerEvent.ExitCode.ToString(CultureInfo.InvariantCulture)}; {workerEvent.Message}");

            if (string.Equals(workerEvent.Kind, WorkerEventKinds.ProtocolFault, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Pipeline worker '{worker.PipelineName}' emitted a malformed protocol event. {workerEvent.Message}");
            }

            if (string.Equals(workerEvent.Kind, WorkerEventKinds.Closed, StringComparison.OrdinalIgnoreCase))
            {
                await SubmitAsync(new RuntimeEvent.WorkerClosed(worker.PipelineName, workerEvent.ExitCode, workerEvent.Message)).ConfigureAwait(false);
                return;
            }

            RuntimeEvent? runtimeEvent = workerEvent.Kind switch
            {
                WorkerEventKinds.WorkerOnline => new RuntimeEvent.WorkerOnline(worker.PipelineName, worker.PipelineId, workerEvent.ExecutableVersion),
                WorkerEventKinds.WorkerReady => new RuntimeEvent.WorkerReady(worker.PipelineName),
                WorkerEventKinds.PipelineStarted => new RuntimeEvent.PipelineStarted(worker.PipelineName),
                WorkerEventKinds.TaskReady => new RuntimeEvent.TaskReady(worker.PipelineName, workerEvent.TaskId, workerEvent.TaskName),
                WorkerEventKinds.GrantAccepted => new RuntimeEvent.GrantAccepted(worker.PipelineName, workerEvent.TaskId, workerEvent.GrantId, workerEvent.CommandId, workerEvent.AttemptNumber),
                WorkerEventKinds.TaskStarted => new RuntimeEvent.TaskStarted(worker.PipelineName, workerEvent.TaskId, workerEvent.GrantId, workerEvent.CommandId, workerEvent.AttemptNumber),
                WorkerEventKinds.TaskSucceeded => new RuntimeEvent.TaskSucceeded(worker.PipelineName, workerEvent.TaskId, workerEvent.GrantId, workerEvent.CommandId, workerEvent.AttemptNumber, workerEvent.ExitCode),
                WorkerEventKinds.TaskFailed => new RuntimeEvent.TaskFailed(
                    worker.PipelineName,
                    workerEvent.TaskId,
                    workerEvent.GrantId,
                    workerEvent.CommandId,
                    workerEvent.AttemptNumber,
                    workerEvent.ExitCode,
                    string.IsNullOrWhiteSpace(workerEvent.FailureClass) ? WorkerFailureClasses.WorkerReportedRetryable : workerEvent.FailureClass,
                    workerEvent.Message),
                WorkerEventKinds.PipelineCatalog or
                    WorkerEventKinds.PipelineCompleted or
                    WorkerEventKinds.PipelineStopped or
                    WorkerEventKinds.PipelineFailed or
                    WorkerEventKinds.WorkerDrained or
                    WorkerEventKinds.Heartbeat or
                    WorkerEventKinds.Diagnostic => null,
                _ => throw new InvalidOperationException(
                    $"Pipeline worker '{worker.PipelineName}' emitted unsupported event '{workerEvent.Kind}'.")
            };

            if (runtimeEvent is not null)
            {
                await SubmitAsync(runtimeEvent).ConfigureAwait(false);
            }

            if (!worker.HasExited &&
                workersByName.TryGetValue(worker.PipelineName, out var currentWorker) &&
                ReferenceEquals(currentWorker, worker))
            {
                eventTasksByWorker[worker] = worker.ReadEventAsync(cancellationToken);
            }
        }

        try
        {
            await SubmitAsync(new RuntimeEvent.SchedulerTick(DateTimeOffset.UtcNow, request.MaxDegreeOfParallelism)).ConfigureAwait(false);
            while (HasRuntimeWork(snapshot) || eventTasksByWorker.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var completed = eventTasksByWorker.Values.FirstOrDefault(static item => item.IsCompleted);
                if (completed is not null)
                {
                    await ProcessWorkerEventAsync(completed).ConfigureAwait(false);
                    continue;
                }

                var beforeActionCount = eventTasksByWorker.Count;
                var schedulerResult = await SubmitAsync(new RuntimeEvent.SchedulerTick(DateTimeOffset.UtcNow, request.MaxDegreeOfParallelism)).ConfigureAwait(false);
                if (schedulerResult.Actions.Any(action => action is not RuntimeAction.PublishSnapshot) ||
                    eventTasksByWorker.Count != beforeActionCount)
                {
                    continue;
                }

                var timedOutWorker = FindRuntimeTimedOutWorker(
                    eventTasksByWorker,
                    workerEventTimeout,
                    workerActivationTimeout,
                    DateTimeOffset.UtcNow);
                if (timedOutWorker is not null)
                {
                    eventTasksByWorker.Remove(timedOutWorker);
                    var timeout = ResolveRuntimeWorkerTimeout(timedOutWorker, workerEventTimeout, workerActivationTimeout);
                    var reason = timeout is null
                        ? "worker stopped responding"
                        : $"No worker protocol event was received within {FormatTimeout(timeout.Value)}.";
                    journal.WriteEvent("WorkerProtocolTimeout", timedOutWorker.PipelineName, reason);
                    timedOutWorker.Terminate(reason);
                    await SubmitAsync(new RuntimeEvent.WorkerTimedOut(
                        timedOutWorker.PipelineName,
                        WorkerFailureClasses.TaskTimeout,
                        reason)).ConfigureAwait(false);
                    continue;
                }

                if (eventTasksByWorker.Count == 0)
                {
                    if (HasRuntimeWork(snapshot))
                    {
                        throw new InvalidOperationException("Cannot execute run plan because no pipeline worker can produce the remaining task events.");
                    }

                    break;
                }

                var timeoutWakeTask = CreateRuntimeTimeoutWakeTask(
                    eventTasksByWorker,
                    workerEventTimeout,
                    workerActivationTimeout,
                    cancellationToken);
                var waitTasks = eventTasksByWorker.Values.Cast<Task>();
                if (timeoutWakeTask is not null)
                {
                    waitTasks = waitTasks.Append(timeoutWakeTask);
                }

                var completedTask = await Task.WhenAny(waitTasks).ConfigureAwait(false);
                if (ReferenceEquals(completedTask, timeoutWakeTask))
                {
                    continue;
                }

                await ProcessWorkerEventAsync((Task<WorkerProtocolEvent>)completedTask).ConfigureAwait(false);
            }

            return snapshot.Outcomes.ToDictionary(
                static item => item.TaskAccessProfileId,
                static item => item.Outcome,
                StringComparer.Ordinal);
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

    private static bool HasRuntimeWork(RuntimeSnapshot snapshot) =>
        snapshot.PendingCount > 0 ||
        snapshot.ReadyCount > 0 ||
        snapshot.RunningGrantCount > 0 ||
        snapshot.RetryCount > 0;

    private static void RecordRuntimeTaskCompletion(
        RuntimeTaskCompletion completion,
        IReadOnlyDictionary<string, PipelineWorkerProcess> workersByName,
        ICollection<OrchestrationTaskWorkerResult> taskResults,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer)
    {
        workersByName.TryGetValue(completion.PipelineName, out var worker);
        var result = new OrchestrationTaskWorkerResult(
            completion.TaskAccessProfileId,
            completion.PlannedTaskId,
            completion.PipelineName,
            completion.StepName,
            completion.ExitCode,
            worker?.StandardOutputText ?? string.Empty,
            worker?.StandardErrorText ?? string.Empty,
            completion.GrantId,
            completion.CommandId,
            completion.AttemptNumber);
        taskResults.Add(result);
        if (completion.RecordTerminalOutcome)
        {
            observer?.TaskCompleted(result.PlannedTaskId, result.ExitCode == 0);
        }

        journal.WriteEvent(
            completion.JournalEventKind,
            $"{completion.PipelineName}.{completion.StepName}",
            result.ExitCode.ToString(CultureInfo.InvariantCulture));
    }

    private static void RecordRuntimeBlockedTasks(
        IReadOnlyList<RuntimeBlockedTask> blockedTasks,
        ICollection<OrchestrationTaskBlockedResult> blockedResults,
        OrchestrationRunJournal journal,
        IOrchestrationRuntimeObserver? observer)
    {
        foreach (var blocked in blockedTasks)
        {
            blockedResults.Add(new OrchestrationTaskBlockedResult(
                blocked.PlannedTaskId,
                blocked.TaskAccessProfileId,
                blocked.PipelineName,
                blocked.StepName,
                blocked.BlockingTaskAccessProfileId,
                blocked.BlockingPipelineName,
                blocked.BlockingStepName,
                blocked.DependencyCondition,
                blocked.Outcome,
                blocked.Reason));
            observer?.TaskBlocked(blocked.PlannedTaskId);
            journal.WriteEvent("TaskBlocked", $"{blocked.PipelineName}.{blocked.StepName}", blocked.Reason);
        }
    }

    private static PipelineWorkerProcess? FindRuntimeTimedOutWorker(
        IReadOnlyDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
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

            var timeout = ResolveRuntimeWorkerTimeout(worker, workerEventTimeout, workerActivationTimeout);
            if (timeout is null)
            {
                continue;
            }

            if (now - worker.LastProtocolActivityUtc >= timeout.Value)
            {
                return worker;
            }
        }

        return null;
    }

    private static Task? CreateRuntimeTimeoutWakeTask(
        IReadOnlyDictionary<PipelineWorkerProcess, Task<WorkerProtocolEvent>> eventTasksByWorker,
        TimeSpan? workerEventTimeout,
        TimeSpan? workerActivationTimeout,
        CancellationToken cancellationToken)
    {
        var nextTimeoutAt = eventTasksByWorker
            .Where(static item => !item.Value.IsCompleted)
            .Select(item => ResolveRuntimeWorkerTimeout(item.Key, workerEventTimeout, workerActivationTimeout) is { } timeout
                ? item.Key.LastProtocolActivityUtc + timeout
                : (DateTimeOffset?)null)
            .Where(static item => item is not null)
            .OrderBy(static item => item)
            .FirstOrDefault();
        if (nextTimeoutAt is null)
        {
            return null;
        }

        var delay = nextTimeoutAt.Value - DateTimeOffset.UtcNow;
        return delay <= TimeSpan.Zero
            ? Task.CompletedTask
            : Task.Delay(delay, cancellationToken);
    }

    private static TimeSpan? ResolveRuntimeWorkerTimeout(
        PipelineWorkerProcess worker,
        TimeSpan? workerEventTimeout,
        TimeSpan? workerActivationTimeout)
    {
        if (worker.ResumeTaskId.Length == 0 && worker.LastProtocolActivityUtc == default)
        {
            return workerActivationTimeout ?? workerEventTimeout;
        }

        return workerEventTimeout;
    }

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

        public Task SendGrantTaskAsync(RuntimeGrant grant) =>
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
