using MetaOrchestration.Core;
using MetaOrchestration.WorkerProtocol;
using MO = MetaOrchestration;

namespace MetaOrchestration.Tests;

public sealed class MetaOrchestrationRuntimeKernelTests
{
    [Fact]
    public void RuntimeKernelFuzzerSurvivesRandomCrashesRetriesAndDependencyBlocks()
    {
        var iterations = ResolveKernelFuzzIterations();
        var random = new XorShift32(0xC0FFEE42);
        var sawWorkerReportedRetry = false;
        var sawSupervisorRetry = false;
        var sawDependencyBlock = false;
        var sawSuccess = false;

        for (var scenario = 0; scenario < iterations; scenario++)
        {
            var taskCount = 1 + random.Next(5);
            var tasks = Enumerable
                .Range(0, taskCount)
                .Select(index => CreateTask(
                    $"Pipe{index.ToString(System.Globalization.CultureInfo.InvariantCulture)}",
                    $"task-{index.ToString(System.Globalization.CultureInfo.InvariantCulture)}",
                    $"pipeline:Pipe{index.ToString(System.Globalization.CultureInfo.InvariantCulture)}:task:{index.ToString(System.Globalization.CultureInfo.InvariantCulture)}"))
                .ToArray();
            var dependencies = tasks.ToDictionary(
                static item => item.TaskAccessProfile.Id,
                static _ => Array.Empty<OrchestrationExecutionDependency>(),
                StringComparer.Ordinal);
            for (var index = 1; index < tasks.Length; index++)
            {
                dependencies[tasks[index].TaskAccessProfile.Id] =
                [
                    new OrchestrationExecutionDependency(
                        tasks[index - 1].TaskAccessProfile.Id,
                        tasks[index].TaskAccessProfile.Id,
                        OrchestrationExecutionContinuity.OnSuccess,
                        "GeneratedDependency",
                        "previous task must succeed")
                ];
            }

            var kernel = CreateKernel(tasks, dependencies);
            var previousFailed = false;
            foreach (var task in tasks)
            {
                StartWorker(kernel, task);
                kernel.AddReady(ReadyEvent(task), task.PipelineReference.Name);
                var liveWorkers = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { task.PipelineReference.Name };

                if (previousFailed)
                {
                    var blockedDecision = kernel.ChooseReadyAction(EmptyWorkerSet(), DateTimeOffset.UtcNow, maxDegreeOfParallelism: 1);
                    Assert.Equal(OrchestrationRuntimeReadyDecisionKind.Block, blockedDecision.Kind);
                    Assert.NotEmpty(blockedDecision.BlockedPipeline.BlockedTasks);
                    sawDependencyBlock = true;
                    kernel.MarkWorkerClosed(task.PipelineReference.Name);
                    kernel.AssertProjection("fuzz dependency block", EmptyWorkerSet());
                    continue;
                }

                var terminal = false;
                while (!terminal)
                {
                    var grantDecision = kernel.ChooseReadyAction(EmptyWorkerSet(), DateTimeOffset.UtcNow, maxDegreeOfParallelism: 1);
                    Assert.Equal(OrchestrationRuntimeReadyDecisionKind.Grant, grantDecision.Kind);
                    var grant = kernel.CommitGrantIssued(
                        grantDecision.ReadyTask,
                        grantDecision.PlannedTask!,
                        grantDecision.PlannedTaskLocks,
                        grantDecision.Grant).Grant;
                    kernel.MarkTaskStarted(GrantEvent(WorkerEventKinds.TaskStarted, task, grant), task.PipelineReference.Name);

                    switch (random.Next(5))
                    {
                        case 0:
                        case 1:
                            kernel.CompleteSucceeded(GrantEvent(WorkerEventKinds.TaskSucceeded, task, grant), task.PipelineReference.Name);
                            kernel.MarkWorkerClosed(task.PipelineReference.Name);
                            sawSuccess = true;
                            terminal = true;
                            break;
                        case 2:
                            var workerFailure = kernel.ResolveWorkerReportedFailure(
                                GrantEvent(WorkerEventKinds.TaskFailed, task, grant, exitCode: 5),
                                task.PipelineReference.Name,
                                RetryWorkerReportedPolicy());
                            if (workerFailure.RetryDecision.ShouldRetry)
                            {
                                sawWorkerReportedRetry = true;
                                kernel.AssertProjection("fuzz worker retry", liveWorkers);
                                continue;
                            }

                            kernel.MarkWorkerClosed(task.PipelineReference.Name);
                            previousFailed = true;
                            terminal = true;
                            break;
                        case 3:
                            var loss = kernel.ApplyWorkerLoss(task.PipelineReference.Name, exitCode: -1);
                            Assert.Equal(OrchestrationWorkerLossDecisionKind.ActiveGrantLost, loss.Kind);
                            liveWorkers.Clear();
                            var supervisorFailure = kernel.ResolveSupervisorObservedFailure(
                                GrantEvent(WorkerEventKinds.TaskFailed, task, grant, exitCode: -1),
                                task.PipelineReference.Name,
                                WorkerFailureClasses.WorkerCrashBeforeTerminalEvent,
                                "fuzz worker crash",
                                RetryCrashPolicy());
                            if (supervisorFailure.ShouldStartReplacementWorker)
                            {
                                sawSupervisorRetry = true;
                                kernel.AssertProjection("fuzz supervisor retry scheduled", liveWorkers);
                                StartWorker(kernel, task, supervisorFailure.ResumeTaskId);
                                kernel.AddReady(ReadyEvent(task), task.PipelineReference.Name);
                                liveWorkers.Add(task.PipelineReference.Name);
                                continue;
                            }

                            previousFailed = true;
                            terminal = true;
                            break;
                        default:
                            var noRetryFailure = kernel.ResolveWorkerReportedFailure(
                                GrantEvent(WorkerEventKinds.TaskFailed, task, grant, exitCode: 7),
                                task.PipelineReference.Name,
                                NoRetryPolicy());
                            Assert.False(noRetryFailure.RetryDecision.ShouldRetry);
                            kernel.MarkWorkerClosed(task.PipelineReference.Name);
                            previousFailed = true;
                            terminal = true;
                            break;
                    }

                    kernel.AssertProjection("fuzz terminal step", liveWorkers);
                }
            }

            Assert.False(kernel.HasUnresolvedWork);
        }

        Assert.True(sawWorkerReportedRetry);
        Assert.True(sawSupervisorRetry);
        Assert.True(sawDependencyBlock);
        Assert.True(sawSuccess);
    }

    [Fact]
    public void RuntimeKernelGraphFuzzerMatchesIndependentReadinessAndLockOracle()
    {
        var iterations = ResolveKernelGraphFuzzIterations();
        var random = new XorShift32(0xBADC0DEu);
        var sawDependencyWait = false;
        var sawDependencyBlock = false;
        var sawLockDeferral = false;
        var sawConcurrentGrant = false;
        var sawSuccess = false;
        var sawFailure = false;

        for (var scenario = 0; scenario < iterations; scenario++)
        {
            var taskCount = 5 + random.Next(5);
            var tasks = Enumerable
                .Range(0, taskCount)
                .Select(index => CreateTask(
                    $"Pipe{index.ToString("D2", System.Globalization.CultureInfo.InvariantCulture)}",
                    $"task-{index.ToString("D2", System.Globalization.CultureInfo.InvariantCulture)}",
                    $"pipeline:Pipe{index.ToString("D2", System.Globalization.CultureInfo.InvariantCulture)}:task:{index.ToString("D2", System.Globalization.CultureInfo.InvariantCulture)}"))
                .ToArray();
            var dependencies = CreateRandomDagDependencies(tasks, ref random);
            var locksByPlannedTaskId = CreateRandomLocks(tasks, ref random);
            var kernel = CreateKernel(tasks, dependencies, locksByPlannedTaskId);
            var terminalByTaskProfileId = new Dictionary<string, string>(StringComparer.Ordinal);
            var readyByTaskId = new HashSet<string>(StringComparer.Ordinal);
            var activeByTaskId = new Dictionary<string, ActiveGraphGrant>(StringComparer.Ordinal);
            var closedWorkers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var liveWorkers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var task in tasks)
            {
                StartWorker(kernel, task);
                kernel.AddReady(ReadyEvent(task), task.PipelineReference.Name);
                readyByTaskId.Add(task.TaskAccessProfile.MetaPipelinePipelineTaskId);
                liveWorkers.Add(task.PipelineReference.Name);
            }

            var maxDegreeOfParallelism = 2 + random.Next(3);
            var guard = 0;
            while (terminalByTaskProfileId.Count < taskCount)
            {
                if (++guard > taskCount * 30)
                {
                    throw new InvalidOperationException("Kernel graph fuzzer stopped making progress.");
                }

                var expected = ChooseExpectedGraphAction(
                    tasks,
                    dependencies,
                    locksByPlannedTaskId,
                    readyByTaskId,
                    activeByTaskId,
                    terminalByTaskProfileId,
                    maxDegreeOfParallelism,
                    ref sawDependencyWait,
                    ref sawLockDeferral);
                var actual = kernel.ChooseReadyAction(ClosedWorkerSet(closedWorkers), DateTimeOffset.UtcNow, maxDegreeOfParallelism);

                switch (expected.Kind)
                {
                    case ExpectedGraphActionKind.Grant:
                        Assert.Equal(OrchestrationRuntimeReadyDecisionKind.Grant, actual.Kind);
                        Assert.Equal(expected.TaskId, actual.ReadyTask.TaskId);
                        var grantIssue = kernel.CommitGrantIssued(
                            actual.ReadyTask,
                            actual.PlannedTask!,
                            actual.PlannedTaskLocks,
                            actual.Grant);
                        kernel.MarkTaskStarted(GrantEvent(WorkerEventKinds.TaskStarted, actual.PlannedTask!, grantIssue.Grant), actual.ReadyTask.WorkerName);
                        readyByTaskId.Remove(expected.TaskId);
                        activeByTaskId[expected.TaskId] = new ActiveGraphGrant(actual.PlannedTask!, grantIssue.Grant);
                        if (activeByTaskId.Count > 1)
                        {
                            sawConcurrentGrant = true;
                        }

                        break;
                    case ExpectedGraphActionKind.Block:
                        Assert.Equal(OrchestrationRuntimeReadyDecisionKind.Block, actual.Kind);
                        Assert.Contains(
                            actual.BlockedPipeline.BlockedTasks,
                            item => string.Equals(item.PlannedTask.TaskAccessProfile.MetaPipelinePipelineTaskId, expected.TaskId, StringComparison.Ordinal));
                        foreach (var blocked in actual.BlockedPipeline.BlockedTasks)
                        {
                            var taskId = blocked.PlannedTask.TaskAccessProfile.MetaPipelinePipelineTaskId;
                            readyByTaskId.Remove(taskId);
                            terminalByTaskProfileId[blocked.PlannedTask.TaskAccessProfile.Id] = blocked.Outcome;
                            kernel.MarkWorkerClosed(blocked.PlannedTask.PipelineReference.Name);
                            liveWorkers.Remove(blocked.PlannedTask.PipelineReference.Name);
                            closedWorkers.Add(blocked.PlannedTask.PipelineReference.Name);
                        }

                        sawDependencyBlock = true;
                        break;
                    case ExpectedGraphActionKind.None:
                        if (actual.Kind != OrchestrationRuntimeReadyDecisionKind.None)
                        {
                            throw new InvalidOperationException(
                                "Kernel graph oracle expected no grant, but runtime returned " +
                                $"{actual.Kind} for task '{actual.ReadyTask.TaskId}'. " +
                                DescribeGraphOracleState(
                                    tasks,
                                    dependencies,
                                    locksByPlannedTaskId,
                                    readyByTaskId,
                                    activeByTaskId,
                                    terminalByTaskProfileId,
                                    maxDegreeOfParallelism));
                        }

                        CompleteOneActiveGraphGrant(
                            kernel,
                            activeByTaskId,
                            terminalByTaskProfileId,
                            liveWorkers,
                            closedWorkers,
                            ref random,
                            ref sawSuccess,
                            ref sawFailure);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(expected.Kind), expected.Kind, "Unknown expected graph action.");
                }

                kernel.AssertProjection("graph fuzz iteration", liveWorkers);
            }

            Assert.Empty(activeByTaskId);
            Assert.False(kernel.HasUnresolvedWork);
        }

        Assert.True(sawDependencyWait);
        Assert.True(sawDependencyBlock);
        Assert.True(sawLockDeferral);
        Assert.True(sawConcurrentGrant);
        Assert.True(sawSuccess);
        Assert.True(sawFailure);
    }

    [Fact]
    public void RuntimeKernelBlocksDependentReadyTaskAfterPredecessorFailure()
    {
        var seed = CreateTask("Seed", "seed", "pipeline:Seed:task:seed");
        var consume = CreateTask("Consume", "consume", "pipeline:Consume:task:consume");
        var kernel = CreateKernel(
            [seed, consume],
            new Dictionary<string, OrchestrationExecutionDependency[]>
            {
                [seed.TaskAccessProfile.Id] = [],
                [consume.TaskAccessProfile.Id] =
                [
                    new OrchestrationExecutionDependency(
                        seed.TaskAccessProfile.Id,
                        consume.TaskAccessProfile.Id,
                        OrchestrationExecutionContinuity.OnSuccess,
                        "DataDependency",
                        "consume waits for seed")
                ]
            });

        StartWorker(kernel, seed);
        kernel.AddReady(ReadyEvent(seed), seed.PipelineReference.Name);
        var seedGrantDecision = kernel.ChooseReadyAction(EmptyWorkerSet(), DateTimeOffset.UtcNow, maxDegreeOfParallelism: 4);
        Assert.Equal(OrchestrationRuntimeReadyDecisionKind.Grant, seedGrantDecision.Kind);
        var seedGrant = kernel.CommitGrantIssued(
            seedGrantDecision.ReadyTask,
            seedGrantDecision.PlannedTask!,
            seedGrantDecision.PlannedTaskLocks,
            seedGrantDecision.Grant).Grant;
        kernel.MarkTaskStarted(GrantEvent(WorkerEventKinds.TaskStarted, seed, seedGrant), seed.PipelineReference.Name);

        var failure = kernel.ResolveWorkerReportedFailure(
            GrantEvent(WorkerEventKinds.TaskFailed, seed, seedGrant, exitCode: 5),
            seed.PipelineReference.Name,
            NoRetryPolicy());

        Assert.False(failure.RetryDecision.ShouldRetry);
        Assert.True(failure.Completion.RecordTerminalOutcome);
        Assert.Equal(OrchestrationExecutionContinuity.Failed, kernel.TaskOutcomesByTaskProfileId[seed.TaskAccessProfile.Id]);

        StartWorker(kernel, consume);
        kernel.AddReady(ReadyEvent(consume), consume.PipelineReference.Name);
        var consumeDecision = kernel.ChooseReadyAction(EmptyWorkerSet(), DateTimeOffset.UtcNow, maxDegreeOfParallelism: 4);

        Assert.Equal(OrchestrationRuntimeReadyDecisionKind.Block, consumeDecision.Kind);
        var blocked = Assert.Single(consumeDecision.BlockedPipeline.BlockedTasks);
        Assert.Equal(consume.Id, blocked.PlannedTask.Id);
        Assert.Equal(seed.TaskAccessProfile.Id, blocked.BlockingTaskProfileId);
        Assert.Equal(OrchestrationExecutionContinuity.SkippedBlocked, blocked.Outcome);
        Assert.Equal(OrchestrationExecutionContinuity.SkippedBlocked, kernel.TaskOutcomesByTaskProfileId[consume.TaskAccessProfile.Id]);
        kernel.AssertProjection("blocked dependency test", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Seed", "Consume" });
    }

    [Fact]
    public void RuntimeKernelSchedulesSupervisorFailureRetryAtResumeBoundary()
    {
        var task = CreateTask("Load", "load", "pipeline:Load:task:load");
        var kernel = CreateKernel([task]);
        StartWorker(kernel, task);
        kernel.AddReady(ReadyEvent(task), task.PipelineReference.Name);
        var grantDecision = kernel.ChooseReadyAction(EmptyWorkerSet(), DateTimeOffset.UtcNow, maxDegreeOfParallelism: 1);
        var grant = kernel.CommitGrantIssued(
            grantDecision.ReadyTask,
            grantDecision.PlannedTask!,
            grantDecision.PlannedTaskLocks,
            grantDecision.Grant).Grant;
        kernel.MarkTaskStarted(GrantEvent(WorkerEventKinds.TaskStarted, task, grant), task.PipelineReference.Name);

        var loss = kernel.ApplyWorkerLoss(task.PipelineReference.Name, exitCode: -1);
        Assert.Equal(OrchestrationWorkerLossDecisionKind.ActiveGrantLost, loss.Kind);
        Assert.Equal(task.TaskAccessProfile.MetaPipelinePipelineTaskId, loss.ResumeTaskId);

        var supervisorFailure = kernel.ResolveSupervisorObservedFailure(
            GrantEvent(WorkerEventKinds.TaskFailed, task, grant, exitCode: -1),
            task.PipelineReference.Name,
            WorkerFailureClasses.WorkerCrashBeforeTerminalEvent,
            "worker crashed before terminal task evidence",
            RetryCrashPolicy());

        Assert.True(supervisorFailure.ShouldStartReplacementWorker);
        Assert.Equal(task.TaskAccessProfile.MetaPipelinePipelineTaskId, supervisorFailure.ResumeTaskId);
        Assert.Equal(2, supervisorFailure.NextAttemptNumber);
        Assert.True(kernel.PendingTaskIds.Contains(task.TaskAccessProfile.MetaPipelinePipelineTaskId));
        Assert.True(kernel.ScheduledRetryByTaskId.ContainsKey(task.TaskAccessProfile.MetaPipelinePipelineTaskId));
        kernel.AssertProjection("retry scheduled", EmptyWorkerSet());

        StartWorker(kernel, task, resumeTaskId: supervisorFailure.ResumeTaskId);
        kernel.AddReady(ReadyEvent(task), task.PipelineReference.Name);
        var retryGrantDecision = kernel.ChooseReadyAction(EmptyWorkerSet(), DateTimeOffset.UtcNow, maxDegreeOfParallelism: 1);

        Assert.Equal(OrchestrationRuntimeReadyDecisionKind.Grant, retryGrantDecision.Kind);
        Assert.Equal(supervisorFailure.PreviousGrantId, retryGrantDecision.Grant.PreviousGrantId);
        Assert.Equal(2, retryGrantDecision.Grant.AttemptNumber);
        kernel.CommitGrantIssued(
            retryGrantDecision.ReadyTask,
            retryGrantDecision.PlannedTask!,
            retryGrantDecision.PlannedTaskLocks,
            retryGrantDecision.Grant);
        kernel.AssertProjection("retry grant issued", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Load" });
    }

    [Fact]
    public void RuntimeKernelProjectionInvariantRejectsLogicalWorkerWithoutLiveTransportProjection()
    {
        var task = CreateTask("Load", "load", "pipeline:Load:task:load");
        var kernel = CreateKernel([task]);
        StartWorker(kernel, task);

        var ex = Assert.Throws<InvalidOperationException>(() => kernel.AssertProjection("missing live worker", EmptyWorkerSet()));

        Assert.Contains("no live event, ready, or running projection", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeKernelBoundsPreWorkReplacementAttemptsAtPipelineActivationBoundary()
    {
        var task = CreateTask("Load", "load", "pipeline:Load:task:load");
        var kernel = CreateKernel([task]);

        var first = kernel.ReservePreWorkReplacementAttempt("Load", string.Empty, "first lost worker");
        var second = kernel.ReservePreWorkReplacementAttempt("Load", string.Empty, "second lost worker");
        var third = kernel.ReservePreWorkReplacementAttempt("Load", string.Empty, "third lost worker");
        var ex = Assert.Throws<InvalidOperationException>(() =>
            kernel.ReservePreWorkReplacementAttempt("Load", string.Empty, "fourth lost worker"));

        Assert.Equal("Load", first.PipelineName);
        Assert.Equal(string.Empty, first.ResumeTaskId);
        Assert.Equal(1, first.Attempt);
        Assert.Equal(2, second.Attempt);
        Assert.Equal(3, third.Attempt);
        Assert.Equal(3, third.Limit);
        Assert.Contains("pre-work worker replacement limit before pipeline activation", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Limit: 3", ex.Message, StringComparison.Ordinal);
        Assert.Contains("fourth lost worker", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeKernelTracksPreWorkReplacementAttemptsPerResumeBoundary()
    {
        var task = CreateTask("Load", "load", "pipeline:Load:task:load");
        var kernel = CreateKernel([task]);

        kernel.ReservePreWorkReplacementAttempt("Load", string.Empty, "activation one");
        kernel.ReservePreWorkReplacementAttempt("Load", string.Empty, "activation two");
        kernel.ReservePreWorkReplacementAttempt("Load", string.Empty, "activation three");

        var firstAtTaskBoundary = kernel.ReservePreWorkReplacementAttempt(
            "Load",
            task.TaskAccessProfile.MetaPipelinePipelineTaskId,
            "grant lost at task boundary");
        var secondAtTaskBoundary = kernel.ReservePreWorkReplacementAttempt(
            "Load",
            task.TaskAccessProfile.MetaPipelinePipelineTaskId,
            "grant lost again");

        Assert.Equal(1, firstAtTaskBoundary.Attempt);
        Assert.Equal(2, secondAtTaskBoundary.Attempt);
        Assert.Equal(task.TaskAccessProfile.MetaPipelinePipelineTaskId, firstAtTaskBoundary.ResumeTaskId);
    }

    [Fact]
    public void RuntimeWorkerActivationStateMachineDeclaresParkingAndResumeTransitions()
    {
        Assert.Contains(
            OrchestrationWorkerActivationStateMachine.Transitions,
            item => item.From == OrchestrationWorkerActivationState.Active &&
                    item.Trigger == OrchestrationWorkerActivationTrigger.CapacityDeferralRequested &&
                    item.To == OrchestrationWorkerActivationState.CapacityDeferralRequested);
        Assert.Contains(
            OrchestrationWorkerActivationStateMachine.Transitions,
            item => item.From == OrchestrationWorkerActivationState.CapacityDeferralRequested &&
                    item.Trigger == OrchestrationWorkerActivationTrigger.CapacityDeferredWorkerClosed &&
                    item.To == OrchestrationWorkerActivationState.Parked);
        Assert.Contains(
            OrchestrationWorkerActivationStateMachine.Transitions,
            item => item.From == OrchestrationWorkerActivationState.Parked &&
                    item.Trigger == OrchestrationWorkerActivationTrigger.StartRequested &&
                    item.To == OrchestrationWorkerActivationState.StartRequested);
        Assert.Contains(
            OrchestrationWorkerActivationStateMachine.Transitions,
            item => item.From == OrchestrationWorkerActivationState.StartRequested &&
                    item.Trigger == OrchestrationWorkerActivationTrigger.WorkerRegistered &&
                    item.To == OrchestrationWorkerActivationState.Active);
    }

    [Fact]
    public void RuntimeWorkerActivationStateMachineParksAndResumesThroughEvents()
    {
        var consumerStage = CreateTask("AConsumer", "stage", "pipeline:AConsumer:task:1");
        var consumerWait = CreateTask("AConsumer", "consume-b", "pipeline:AConsumer:task:2");
        var producer = CreateTask("BProducer", "produce-b", "pipeline:BProducer:task:1");
        var machine = CreateActivationStateMachine([consumerStage, consumerWait, producer]);

        var consumerStart = machine.ApplySchedulerTick(new OrchestrationWorkerActivationFacts(
            EmptyWorkerSet(),
            MaxActiveWorkerProcesses: 1,
            DateTimeOffset.UtcNow,
            (definition, _) => definition.PipelineName == "AConsumer"
                ? ActivationCandidate(definition, consumerStage, OrchestrationTaskReadiness.Ready)
                : ActivationCandidate(definition, producer, OrchestrationTaskReadiness.Ready),
            () => null));
        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.StartWorker, consumerStart.Kind);
        machine.RegisterWorker("AConsumer", "pipeline:AConsumer", string.Empty);

        var defer = machine.ApplySchedulerTick(new OrchestrationWorkerActivationFacts(
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "AConsumer" },
            MaxActiveWorkerProcesses: 1,
            DateTimeOffset.UtcNow,
            (definition, _) => definition.PipelineName == "AConsumer"
                ? ActivationCandidate(definition, consumerWait, OrchestrationTaskReadiness.Waiting)
                : ActivationCandidate(definition, producer, OrchestrationTaskReadiness.Ready),
            () => new OrchestrationRuntimeWorkerCapacityDeferralCandidate(
                "AConsumer",
                "pipeline:AConsumer",
                "AConsumer",
                consumerWait.TaskAccessProfile.MetaPipelinePipelineTaskId,
                consumerWait.TaskAccessProfile.TaskName,
                "consumer waits for producer")));
        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.DeferWorkerForCapacity, defer.Kind);
        machine.CommitCapacityDeferralRequested(defer);
        Assert.True(machine.IsCapacityDeferralPending("AConsumer"));

        Assert.True(machine.TryApplyCapacityDeferredWorkerClosed(
            "AConsumer",
            consumerWait.TaskAccessProfile.MetaPipelinePipelineTaskId,
            consumerWait.TaskAccessProfile.TaskName,
            out var parked));
        Assert.Equal(consumerWait.TaskAccessProfile.MetaPipelinePipelineTaskId, parked.ResumeTaskId);
        Assert.Equal(OrchestrationWorkerActivationState.Parked, Snapshot(machine, "AConsumer").State);

        var producerStart = machine.ApplySchedulerTick(new OrchestrationWorkerActivationFacts(
            EmptyWorkerSet(),
            MaxActiveWorkerProcesses: 1,
            DateTimeOffset.UtcNow,
            (definition, _) => definition.PipelineName == "AConsumer"
                ? ActivationCandidate(definition, consumerWait, OrchestrationTaskReadiness.Waiting)
                : ActivationCandidate(definition, producer, OrchestrationTaskReadiness.Ready),
            () => null));
        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.StartWorker, producerStart.Kind);
        Assert.Equal("BProducer", producerStart.PipelineName);

        machine.RegisterWorker("BProducer", "pipeline:BProducer", string.Empty);
        machine.MarkPipelineCompleted("BProducer");
        var consumerResume = machine.ApplySchedulerTick(new OrchestrationWorkerActivationFacts(
            EmptyWorkerSet(),
            MaxActiveWorkerProcesses: 1,
            DateTimeOffset.UtcNow,
            (definition, _) => definition.PipelineName == "AConsumer"
                ? ActivationCandidate(definition, consumerWait, OrchestrationTaskReadiness.Ready)
                : null,
            () => null));
        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.StartWorker, consumerResume.Kind);
        Assert.Equal("AConsumer", consumerResume.PipelineName);
        Assert.Equal(consumerWait.TaskAccessProfile.MetaPipelinePipelineTaskId, consumerResume.ResumeTaskId);
    }

    [Fact]
    public void RuntimeWorkerActivationStateMachineCancelsLostCapacityDeferralCommand()
    {
        var consumer = CreateTask("AConsumer", "consume", "pipeline:AConsumer:task:1");
        var producer = CreateTask("BProducer", "produce", "pipeline:BProducer:task:1");
        var machine = CreateActivationStateMachine([consumer, producer]);
        machine.RegisterWorker("AConsumer", "pipeline:AConsumer", string.Empty);

        var defer = machine.ApplySchedulerTick(new OrchestrationWorkerActivationFacts(
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "AConsumer" },
            MaxActiveWorkerProcesses: 1,
            DateTimeOffset.UtcNow,
            (definition, _) => definition.PipelineName == "BProducer"
                ? ActivationCandidate(definition, producer, OrchestrationTaskReadiness.Ready)
                : ActivationCandidate(definition, consumer, OrchestrationTaskReadiness.Waiting),
            () => new OrchestrationRuntimeWorkerCapacityDeferralCandidate(
                "AConsumer",
                "pipeline:AConsumer",
                "AConsumer",
                consumer.TaskAccessProfile.MetaPipelinePipelineTaskId,
                consumer.TaskAccessProfile.TaskName,
                "consumer waits")));

        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.DeferWorkerForCapacity, defer.Kind);
        machine.CancelCapacityDeferralRequested(defer);

        Assert.False(machine.IsCapacityDeferralPending("AConsumer"));
        Assert.Equal(OrchestrationWorkerActivationState.Active, Snapshot(machine, "AConsumer").State);
    }

    [Fact]
    public void RuntimeKernelChoosesWorkerActivationOnlyWithinLiveProcessCapacity()
    {
        var loadA = CreateTask("LoadA", "load", "pipeline:LoadA:task:1");
        var loadB = CreateTask("LoadB", "load", "pipeline:LoadB:task:1");
        var loadC = CreateTask("LoadC", "load", "pipeline:LoadC:task:1");
        var kernel = CreateKernel([loadA, loadB, loadC]);

        var first = kernel.ChooseWorkerActivationAction(EmptyWorkerSet(), maxActiveWorkerProcesses: 2, DateTimeOffset.UtcNow);
        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.StartWorker, first.Kind);
        Assert.Equal("LoadA", first.PipelineName);
        StartWorker(kernel, loadA);

        var second = kernel.ChooseWorkerActivationAction(
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "LoadA" },
            maxActiveWorkerProcesses: 2,
            DateTimeOffset.UtcNow);
        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.StartWorker, second.Kind);
        Assert.Equal("LoadB", second.PipelineName);
        StartWorker(kernel, loadB);

        var capped = kernel.ChooseWorkerActivationAction(
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "LoadA", "LoadB" },
            maxActiveWorkerProcesses: 2,
            DateTimeOffset.UtcNow);
        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.None, capped.Kind);
    }

    [Fact]
    public void RuntimeKernelCanActivateWaitingPipelineWhenCapacityIsSpare()
    {
        var producer = CreateTask("Producer", "produce", "pipeline:Producer:task:1");
        var consumer = CreateTask("Consumer", "consume", "pipeline:Consumer:task:1");
        var dependencies = new Dictionary<string, OrchestrationExecutionDependency[]>(StringComparer.Ordinal)
        {
            [producer.TaskAccessProfile.Id] = [],
            [consumer.TaskAccessProfile.Id] = [CreateDependency(producer, consumer, "Data")]
        };
        var kernel = CreateKernel([producer, consumer], dependencies);

        var producerActivation = kernel.ChooseWorkerActivationAction(EmptyWorkerSet(), maxActiveWorkerProcesses: 2, DateTimeOffset.UtcNow);
        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.StartWorker, producerActivation.Kind);
        Assert.Equal("Producer", producerActivation.PipelineName);
        StartWorker(kernel, producer);

        var consumerActivation = kernel.ChooseWorkerActivationAction(
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Producer" },
            maxActiveWorkerProcesses: 2,
            DateTimeOffset.UtcNow);
        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.StartWorker, consumerActivation.Kind);
        Assert.Equal("Consumer", consumerActivation.PipelineName);
        Assert.Equal(OrchestrationTaskReadiness.Waiting, consumerActivation.Readiness);

        var capped = kernel.ChooseWorkerActivationAction(
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Producer", "Consumer" },
            maxActiveWorkerProcesses: 2,
            DateTimeOffset.UtcNow);
        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.None, capped.Kind);
    }

    [Fact]
    public void RuntimeKernelDefersWaitingReadyWorkerWhenInactiveProducerNeedsCapacity()
    {
        var consumerStage = CreateTask("AConsumer", "stage", "pipeline:AConsumer:task:1");
        var consumerWait = CreateTask("AConsumer", "consume-b", "pipeline:AConsumer:task:2");
        var producer = CreateTask("BProducer", "produce-b", "pipeline:BProducer:task:1");
        var dependencies = new Dictionary<string, OrchestrationExecutionDependency[]>(StringComparer.Ordinal)
        {
            [consumerStage.TaskAccessProfile.Id] = [],
            [consumerWait.TaskAccessProfile.Id] = [CreateDependency(producer, consumerWait, "Data")],
            [producer.TaskAccessProfile.Id] = []
        };
        var kernel = CreateKernel([consumerStage, consumerWait, producer], dependencies);

        StartWorker(kernel, consumerStage);
        kernel.AddReady(ReadyEvent(consumerStage), "AConsumer");
        var grantDecision = kernel.ChooseReadyAction(EmptyWorkerSet(), DateTimeOffset.UtcNow, maxDegreeOfParallelism: 1);
        Assert.Equal(OrchestrationRuntimeReadyDecisionKind.Grant, grantDecision.Kind);
        var grant = kernel.CommitGrantIssued(
            grantDecision.ReadyTask,
            grantDecision.PlannedTask!,
            grantDecision.PlannedTaskLocks,
            grantDecision.Grant).Grant;
        kernel.MarkTaskStarted(GrantEvent(WorkerEventKinds.TaskStarted, consumerStage, grant), "AConsumer");
        kernel.CompleteSucceeded(GrantEvent(WorkerEventKinds.TaskSucceeded, consumerStage, grant), "AConsumer");
        kernel.AddReady(ReadyEvent(consumerWait), "AConsumer");

        var defer = kernel.ChooseWorkerActivationAction(
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "AConsumer" },
            maxActiveWorkerProcesses: 1,
            DateTimeOffset.UtcNow);
        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.DeferWorkerForCapacity, defer.Kind);
        Assert.Equal("AConsumer", defer.WorkerName);
        Assert.Equal(consumerWait.TaskAccessProfile.MetaPipelinePipelineTaskId, defer.TaskId);

        kernel.CommitWorkerCapacityDeferralRequested(defer);
        var notReplacedBeforeDeferredClose = kernel.ChooseReadyAction(
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "AConsumer" },
            DateTimeOffset.UtcNow,
            maxDegreeOfParallelism: 1);
        Assert.Equal(OrchestrationRuntimeReadyDecisionKind.None, notReplacedBeforeDeferredClose.Kind);
        Assert.True(kernel.TryApplyCapacityDeferredWorkerClosed("AConsumer", exitCode: 0, out var deferred));
        Assert.Equal(consumerWait.TaskAccessProfile.MetaPipelinePipelineTaskId, deferred.ResumeTaskId);

        var startProducer = kernel.ChooseWorkerActivationAction(EmptyWorkerSet(), maxActiveWorkerProcesses: 1, DateTimeOffset.UtcNow);
        Assert.Equal(OrchestrationRuntimeWorkerActivationDecisionKind.StartWorker, startProducer.Kind);
        Assert.Equal("BProducer", startProducer.PipelineName);
    }

    private static OrchestrationRuntimeKernel CreateKernel(
        IReadOnlyList<MO.PlannedTask> plannedTasks,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]>? dependenciesByTaskProfileId = null,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]>? locksByPlannedTaskId = null)
    {
        var byPipelineTaskId = plannedTasks.ToDictionary(
            static item => item.TaskAccessProfile.MetaPipelinePipelineTaskId,
            static item => item,
            StringComparer.Ordinal);
        var byProfileId = plannedTasks.ToDictionary(
            static item => item.TaskAccessProfile.Id,
            static item => item,
            StringComparer.Ordinal);
        dependenciesByTaskProfileId ??= plannedTasks.ToDictionary(
            static item => item.TaskAccessProfile.Id,
            static _ => Array.Empty<OrchestrationExecutionDependency>(),
            StringComparer.Ordinal);

        return new OrchestrationRuntimeKernel(
            byPipelineTaskId,
            byProfileId,
            locksByPlannedTaskId ?? new Dictionary<string, MO.PlannedTaskLock[]>(StringComparer.Ordinal),
            [],
            dependenciesByTaskProfileId);
    }

    private static OrchestrationWorkerActivationStateMachine CreateActivationStateMachine(
        IReadOnlyList<MO.PlannedTask> plannedTasks)
    {
        var definitions = plannedTasks
            .GroupBy(static item => item.PipelineReference.Name, StringComparer.OrdinalIgnoreCase)
            .Select(static group =>
            {
                var first = group.First();
                return new OrchestrationWorkerActivationPipelineDefinition(
                    first.PipelineReference.Name,
                    first.PipelineReference.MetaPipelinePipelineId,
                    group
                        .OrderBy(static item => item.Ordinal, StringComparer.Ordinal)
                        .ThenBy(static item => item.Id, StringComparer.Ordinal)
                        .ToArray());
            })
            .ToArray();
        return new OrchestrationWorkerActivationStateMachine(definitions);
    }

    private static OrchestrationRuntimePipelineActivationCandidate ActivationCandidate(
        OrchestrationWorkerActivationPipelineDefinition definition,
        MO.PlannedTask plannedTask,
        OrchestrationTaskReadiness readiness) =>
        new(
            definition.PipelineName,
            definition.PipelineId,
            plannedTask.TaskAccessProfile.MetaPipelinePipelineTaskId,
            plannedTask.TaskAccessProfile.TaskName,
            readiness);

    private static OrchestrationWorkerActivationSnapshot Snapshot(
        OrchestrationWorkerActivationStateMachine machine,
        string pipelineName) =>
        machine.GetSnapshots().Single(item => string.Equals(item.PipelineName, pipelineName, StringComparison.OrdinalIgnoreCase));

    private static IReadOnlySet<string> EmptyWorkerSet() =>
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private static int ResolveKernelFuzzIterations()
    {
        var raw = Environment.GetEnvironmentVariable("META_ORCH_RUNTIME_KERNEL_FUZZ_ITERATIONS");
        return int.TryParse(raw, out var parsed) && parsed > 0
            ? parsed
            : 5_000;
    }

    private static int ResolveKernelGraphFuzzIterations()
    {
        var raw = Environment.GetEnvironmentVariable("META_ORCH_RUNTIME_KERNEL_GRAPH_FUZZ_ITERATIONS");
        return int.TryParse(raw, out var parsed) && parsed > 0
            ? parsed
            : 2_000;
    }

    private static Dictionary<string, OrchestrationExecutionDependency[]> CreateRandomDagDependencies(
        IReadOnlyList<MO.PlannedTask> tasks,
        ref XorShift32 random)
    {
        var dependencies = tasks.ToDictionary(
            static item => item.TaskAccessProfile.Id,
            static _ => Array.Empty<OrchestrationExecutionDependency>(),
            StringComparer.Ordinal);
        for (var index = 2; index < tasks.Count; index++)
        {
            var predecessors = new List<OrchestrationExecutionDependency>();
            if (index == 2 || random.Percent(70))
            {
                var predecessor = tasks[random.Next(index)];
                predecessors.Add(CreateDependency(predecessor, tasks[index], "GeneratedDependency"));
            }

            if (index > 3 && random.Percent(35))
            {
                var predecessor = tasks[random.Next(index)];
                if (!predecessors.Any(item => string.Equals(item.PredecessorTaskProfileId, predecessor.TaskAccessProfile.Id, StringComparison.Ordinal)))
                {
                    predecessors.Add(CreateDependency(predecessor, tasks[index], "GeneratedSecondDependency"));
                }
            }

            dependencies[tasks[index].TaskAccessProfile.Id] = predecessors.ToArray();
        }

        return dependencies;
    }

    private static OrchestrationExecutionDependency CreateDependency(
        MO.PlannedTask predecessor,
        MO.PlannedTask successor,
        string sourceKind) =>
        new(
            predecessor.TaskAccessProfile.Id,
            successor.TaskAccessProfile.Id,
            OrchestrationExecutionContinuity.OnSuccess,
            sourceKind,
            $"{successor.TaskAccessProfile.TaskName} waits for {predecessor.TaskAccessProfile.TaskName}");

    private static Dictionary<string, MO.PlannedTaskLock[]> CreateRandomLocks(
        IReadOnlyList<MO.PlannedTask> tasks,
        ref XorShift32 random)
    {
        var dataObjects = Enumerable
            .Range(0, 4)
            .Select(index => new MO.DataObject
            {
                Id = $"data-object:{index.ToString(System.Globalization.CultureInfo.InvariantCulture)}",
                NormalizedKey = $"dbo.Object{index.ToString(System.Globalization.CultureInfo.InvariantCulture)}",
                SqlIdentifier = $"dbo.Object{index.ToString(System.Globalization.CultureInfo.InvariantCulture)}"
            })
            .ToArray();
        var result = new Dictionary<string, MO.PlannedTaskLock[]>(StringComparer.Ordinal);
        for (var index = 0; index < tasks.Count; index++)
        {
            var task = tasks[index];
            var lockCount = 1 + random.Next(2);
            var locks = new List<MO.PlannedTaskLock>();
            for (var lockIndex = 0; lockIndex < lockCount; lockIndex++)
            {
                var dataObject = index < 2
                    ? dataObjects[0]
                    : dataObjects[random.Next(dataObjects.Length)];
                var lockMode = index < 2 || random.Percent(55)
                    ? "ExclusiveWrite"
                    : "SharedRead";
                locks.Add(CreateLock(task, dataObject, lockMode, lockIndex));
            }

            result[task.Id] = locks.ToArray();
        }

        return result;
    }

    private static MO.PlannedTaskLock CreateLock(
        MO.PlannedTask task,
        MO.DataObject dataObject,
        string lockMode,
        int lockIndex)
    {
        var effect = new MO.TaskObjectEffect
        {
            Id = $"effect:{task.Id}:{lockIndex.ToString(System.Globalization.CultureInfo.InvariantCulture)}",
            AccessDirection = string.Equals(lockMode, "SharedRead", StringComparison.OrdinalIgnoreCase) ? "Read" : "Write",
            AccessPurpose = "Fuzz",
            CreatesDataDependency = "No",
            DataObject = dataObject,
            IsPublishedProducer = "No",
            LockMode = lockMode,
            RequiresSynchronization = "Yes",
            TaskAccessProfile = task.TaskAccessProfile,
            WriteEffect = string.Equals(lockMode, "SharedRead", StringComparison.OrdinalIgnoreCase) ? "None" : "Append"
        };
        return new MO.PlannedTaskLock
        {
            Id = $"lock:{task.Id}:{lockIndex.ToString(System.Globalization.CultureInfo.InvariantCulture)}",
            DataObject = dataObject,
            LockMode = lockMode,
            PlannedTask = task,
            Reason = "runtime kernel graph fuzz",
            TaskObjectEffect = effect
        };
    }

    private static ExpectedGraphAction ChooseExpectedGraphAction(
        IReadOnlyList<MO.PlannedTask> tasks,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependencies,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]> locksByPlannedTaskId,
        IReadOnlySet<string> readyByTaskId,
        IReadOnlyDictionary<string, ActiveGraphGrant> activeByTaskId,
        IReadOnlyDictionary<string, string> terminalByTaskId,
        int maxDegreeOfParallelism,
        ref bool sawDependencyWait,
        ref bool sawLockDeferral)
    {
        foreach (var task in tasks
                     .Where(item => readyByTaskId.Contains(item.TaskAccessProfile.MetaPipelinePipelineTaskId))
                     .OrderBy(static item => item.PipelineReference.Name, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(static item => item.TaskAccessProfile.TaskName, StringComparer.OrdinalIgnoreCase))
        {
            if (activeByTaskId.Count >= maxDegreeOfParallelism)
            {
                return ExpectedGraphAction.None;
            }

            var dependencyState = EvaluateExpectedDependencies(task, dependencies, terminalByTaskId);
            if (dependencyState == ExpectedDependencyState.Waiting)
            {
                sawDependencyWait = true;
                continue;
            }

            if (dependencyState == ExpectedDependencyState.Block)
            {
                return ExpectedGraphAction.Block(task.TaskAccessProfile.MetaPipelinePipelineTaskId);
            }

            var candidateLocks = locksByPlannedTaskId.TryGetValue(task.Id, out var locks)
                ? locks
                : [];
            var activeLocks = activeByTaskId.Values
                .SelectMany(item => locksByPlannedTaskId.TryGetValue(item.Task.Id, out var locks) ? locks : [])
                .ToArray();
            if (!ExpectedLocksCompatible(candidateLocks, activeLocks))
            {
                sawLockDeferral = true;
                continue;
            }

            return ExpectedGraphAction.Grant(task.TaskAccessProfile.MetaPipelinePipelineTaskId);
        }

        return ExpectedGraphAction.None;
    }

    private static ExpectedDependencyState EvaluateExpectedDependencies(
        MO.PlannedTask task,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependencies,
        IReadOnlyDictionary<string, string> terminalByTaskId)
    {
        if (!dependencies.TryGetValue(task.TaskAccessProfile.Id, out var taskDependencies) ||
            taskDependencies.Length == 0)
        {
            return ExpectedDependencyState.Ready;
        }

        foreach (var dependency in taskDependencies)
        {
            if (!terminalByTaskId.TryGetValue(dependency.PredecessorTaskProfileId, out var outcome))
            {
                return ExpectedDependencyState.Waiting;
            }

            if (!string.Equals(outcome, OrchestrationExecutionContinuity.Succeeded, StringComparison.OrdinalIgnoreCase))
            {
                return ExpectedDependencyState.Block;
            }
        }

        return ExpectedDependencyState.Ready;
    }

    private static bool ExpectedLocksCompatible(
        IReadOnlyList<MO.PlannedTaskLock> candidateLocks,
        IReadOnlyList<MO.PlannedTaskLock> activeLocks)
    {
        foreach (var activeLock in activeLocks)
        {
            foreach (var candidateLock in candidateLocks)
            {
                if (!string.Equals(activeLock.DataObject.Id, candidateLock.DataObject.Id, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!string.Equals(activeLock.LockMode, "SharedRead", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(candidateLock.LockMode, "SharedRead", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static string DescribeGraphOracleState(
        IReadOnlyList<MO.PlannedTask> tasks,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependencies,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]> locksByPlannedTaskId,
        IReadOnlySet<string> readyByTaskId,
        IReadOnlyDictionary<string, ActiveGraphGrant> activeByTaskId,
        IReadOnlyDictionary<string, string> terminalByTaskProfileId,
        int maxDegreeOfParallelism)
    {
        var ready = string.Join(
            ", ",
            tasks
                .Where(item => readyByTaskId.Contains(item.TaskAccessProfile.MetaPipelinePipelineTaskId))
                .Select(item => item.TaskAccessProfile.MetaPipelinePipelineTaskId));
        var active = string.Join(", ", activeByTaskId.Keys);
        var terminal = string.Join(
            ", ",
            terminalByTaskProfileId.Select(item => $"{item.Key}:{item.Value}"));
        var dependencyText = string.Join(
            ", ",
            dependencies
                .Where(item => item.Value.Length > 0)
                .Select(item => $"{item.Key}<-[{string.Join("|", item.Value.Select(static dep => dep.PredecessorTaskProfileId))}]"));
        var activeLocks = string.Join(
            ", ",
            activeByTaskId.Values.Select(item => DescribeTaskLocks(item.Task, locksByPlannedTaskId)));
        var readyLocks = string.Join(
            ", ",
            tasks
                .Where(item => readyByTaskId.Contains(item.TaskAccessProfile.MetaPipelinePipelineTaskId))
                .Select(item => DescribeTaskLocks(item, locksByPlannedTaskId)));
        return $"Active={activeByTaskId.Count.ToString(System.Globalization.CultureInfo.InvariantCulture)}/{maxDegreeOfParallelism.ToString(System.Globalization.CultureInfo.InvariantCulture)} [{active}]; Ready=[{ready}]; Terminal=[{terminal}]; Dependencies=[{dependencyText}]; ActiveLocks=[{activeLocks}]; ReadyLocks=[{readyLocks}]";
    }

    private static string DescribeTaskLocks(
        MO.PlannedTask task,
        IReadOnlyDictionary<string, MO.PlannedTaskLock[]> locksByPlannedTaskId)
    {
        var taskId = task.TaskAccessProfile.MetaPipelinePipelineTaskId;
        if (!locksByPlannedTaskId.TryGetValue(task.Id, out var locks) ||
            locks.Length == 0)
        {
            return $"{taskId}:<none>";
        }

        return $"{taskId}:{string.Join("|", locks.Select(static item => $"{item.DataObject.Id}/{item.LockMode}"))}";
    }

    private static void CompleteOneActiveGraphGrant(
        OrchestrationRuntimeKernel kernel,
        IDictionary<string, ActiveGraphGrant> activeByTaskId,
        IDictionary<string, string> terminalByTaskId,
        ISet<string> liveWorkers,
        ISet<string> closedWorkers,
        ref XorShift32 random,
        ref bool sawSuccess,
        ref bool sawFailure)
    {
        Assert.NotEmpty(activeByTaskId);
        var activeItems = activeByTaskId.Values.ToArray();
        var active = activeItems[random.Next(activeItems.Length)];
        if (random.Percent(72))
        {
            kernel.CompleteSucceeded(GrantEvent(WorkerEventKinds.TaskSucceeded, active.Task, active.Grant), active.Task.PipelineReference.Name);
            terminalByTaskId[active.Task.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Succeeded;
            sawSuccess = true;
        }
        else
        {
            var failure = kernel.ResolveWorkerReportedFailure(
                GrantEvent(WorkerEventKinds.TaskFailed, active.Task, active.Grant, exitCode: 11),
                active.Task.PipelineReference.Name,
                NoRetryPolicy());
            Assert.False(failure.RetryDecision.ShouldRetry);
            terminalByTaskId[active.Task.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Failed;
            sawFailure = true;
        }

        activeByTaskId.Remove(active.Task.TaskAccessProfile.MetaPipelinePipelineTaskId);
        kernel.MarkWorkerClosed(active.Task.PipelineReference.Name);
        liveWorkers.Remove(active.Task.PipelineReference.Name);
        closedWorkers.Add(active.Task.PipelineReference.Name);
    }

    private static IReadOnlySet<string> ClosedWorkerSet(IReadOnlySet<string> closedWorkers) =>
        new HashSet<string>(closedWorkers, StringComparer.OrdinalIgnoreCase);

    private static MO.PlannedTask CreateTask(string pipelineName, string taskName, string taskId)
    {
        var pipeline = new MO.PipelineReference
        {
            Id = $"pipeline-reference:{pipelineName}",
            MetaPipelinePipelineId = $"pipeline:{pipelineName}",
            Name = pipelineName
        };
        var profile = new MO.TaskAccessProfile
        {
            Id = $"task-profile:{pipelineName}:{taskName}",
            MetaPipelinePipelineTaskId = taskId,
            TaskName = taskName,
            PipelineReference = pipeline
        };
        return new MO.PlannedTask
        {
            Id = $"planned-task:{pipelineName}:{taskName}",
            Ordinal = "1",
            PipelineReference = pipeline,
            TaskAccessProfile = profile
        };
    }

    private static void StartWorker(
        OrchestrationRuntimeKernel kernel,
        MO.PlannedTask task,
        string resumeTaskId = "")
    {
        var pipelineName = task.PipelineReference.Name;
        kernel.RegisterWorker(pipelineName, task.PipelineReference.MetaPipelinePipelineId, resumeTaskId, "test-version");
        kernel.MarkWorkerOnline(pipelineName, "test-version");
        kernel.MarkWorkerReady(pipelineName);
        kernel.MarkStartPipelineSent(pipelineName);
        kernel.MarkPipelineStarted(pipelineName);
    }

    private static WorkerProtocolEvent ReadyEvent(MO.PlannedTask task) =>
        new(
            WorkerEventKinds.TaskReady,
            $"{task.PipelineReference.Name}-worker",
            task.PipelineReference.MetaPipelinePipelineId,
            task.PipelineReference.Name,
            task.TaskAccessProfile.MetaPipelinePipelineTaskId,
            task.TaskAccessProfile.TaskName,
            string.Empty,
            string.Empty,
            0,
            0,
            "test-version",
            "ready",
            string.Empty);

    private static WorkerProtocolEvent GrantEvent(
        string eventKind,
        MO.PlannedTask task,
        OrchestrationRuntimeGrant grant,
        int exitCode = 0) =>
        new(
            eventKind,
            $"{task.PipelineReference.Name}-worker",
            task.PipelineReference.MetaPipelinePipelineId,
            task.PipelineReference.Name,
            task.TaskAccessProfile.MetaPipelinePipelineTaskId,
            task.TaskAccessProfile.TaskName,
            grant.GrantId,
            grant.CommandId,
            grant.AttemptNumber,
            exitCode,
            "test-version",
            eventKind,
            eventKind == WorkerEventKinds.TaskFailed ? WorkerFailureClasses.WorkerReportedRetryable : string.Empty);

    private static ResolvedOrchestrationRetryPolicy NoRetryPolicy() =>
        new(
            "retry-policy:none",
            "NoRetry",
            MaxAttempts: 1,
            InitialDelayMilliseconds: 0,
            MaxDelayMilliseconds: 0,
            BackoffMultiplier: 1,
            RetryReadOnlyTasksByDefault: true,
            RetryWriteTasksByDefault: true,
            [WorkerFailureClasses.WorkerReportedRetryable]);

    private static ResolvedOrchestrationRetryPolicy RetryCrashPolicy() =>
        new(
            "retry-policy:crash",
            "RetryCrash",
            MaxAttempts: 2,
            InitialDelayMilliseconds: 0,
            MaxDelayMilliseconds: 0,
            BackoffMultiplier: 1,
            RetryReadOnlyTasksByDefault: true,
            RetryWriteTasksByDefault: true,
            [WorkerFailureClasses.WorkerCrashBeforeTerminalEvent]);

    private static ResolvedOrchestrationRetryPolicy RetryWorkerReportedPolicy() =>
        new(
            "retry-policy:worker-reported",
            "RetryWorkerReported",
            MaxAttempts: 2,
            InitialDelayMilliseconds: 0,
            MaxDelayMilliseconds: 0,
            BackoffMultiplier: 1,
            RetryReadOnlyTasksByDefault: true,
            RetryWriteTasksByDefault: true,
            [WorkerFailureClasses.WorkerReportedRetryable]);

    private struct XorShift32
    {
        private uint state;

        public XorShift32(uint seed)
        {
            state = seed == 0 ? 0xA341_316Cu : seed;
        }

        public int Next(int exclusiveMax)
        {
            if (exclusiveMax <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exclusiveMax));
            }

            return (int)(NextUInt32() % (uint)exclusiveMax);
        }

        public bool Percent(int percent) =>
            Next(100) < percent;

        private uint NextUInt32()
        {
            var value = state;
            value ^= value << 13;
            value ^= value >> 17;
            value ^= value << 5;
            state = value;
            return value;
        }
    }

    private readonly record struct ActiveGraphGrant(
        MO.PlannedTask Task,
        OrchestrationRuntimeGrant Grant);

    private readonly record struct ExpectedGraphAction(
        ExpectedGraphActionKind Kind,
        string TaskId)
    {
        public static ExpectedGraphAction None { get; } =
            new(ExpectedGraphActionKind.None, string.Empty);

        public static ExpectedGraphAction Grant(string taskId) =>
            new(ExpectedGraphActionKind.Grant, taskId);

        public static ExpectedGraphAction Block(string taskId) =>
            new(ExpectedGraphActionKind.Block, taskId);
    }

    private enum ExpectedGraphActionKind
    {
        None,
        Grant,
        Block
    }

    private enum ExpectedDependencyState
    {
        Ready,
        Waiting,
        Block
    }
}
