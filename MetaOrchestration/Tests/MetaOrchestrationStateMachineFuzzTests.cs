using MetaOrchestration.Core;
using MetaOrchestration.WorkerProtocol;

namespace MetaOrchestration.Tests;

public sealed class MetaOrchestrationStateMachineFuzzTests
{
    [Fact]
    public void StateMachineProtocolFuzzerSurvivesRandomCrashesAndIllegalEvents()
    {
        var iterations = ResolveIterationCount();

        var result = OrchestrationStateMachineProtocolFuzzer.Run(iterations, seed: 0x5EED_2026);

        Assert.Equal(iterations, result.Scenarios);
        Assert.True(result.WorkerCrashes > 0);
        Assert.True(result.WorkerCrashesBeforeOnline > 0);
        Assert.True(result.WorkerCrashesAfterOnline > 0);
        Assert.True(result.WorkerCrashesAfterReady > 0);
        Assert.True(result.WorkerCrashesAfterStartPipelineSent > 0);
        Assert.True(result.WorkerCrashesAfterPipelineStarted > 0);
        Assert.True(result.WorkerCrashesBeforeTaskStarted > 0);
        Assert.True(result.WorkerCrashesAfterTaskStarted > 0);
        Assert.True(result.ReplacementRetries > 0);
        Assert.True(result.SameWorkerRetries > 0);
        Assert.True(result.IllegalEventsRejected > 0);
        Assert.Equal(0, result.InvariantFailures);
    }

    [Fact]
    public void StateMachineTransitionTablesDeclareCrashBranchesForEveryActiveState()
    {
        var closeableWorkerStates = Enum
            .GetValues<OrchestrationWorkerRuntimeState>()
            .Where(static item => item != OrchestrationWorkerRuntimeState.Closed)
            .OrderBy(static item => item)
            .ToArray();
        var workerCloseSources = OrchestrationExecutionStateMachine.WorkerTransitions
            .Where(static item => item.Trigger == OrchestrationWorkerRuntimeTrigger.WorkerClosed)
            .Select(static item => item.From)
            .OrderBy(static item => item)
            .ToArray();

        Assert.Equal(closeableWorkerStates, workerCloseSources);

        var activeTaskStates = new[]
        {
            OrchestrationTaskRuntimeState.GrantIssued,
            OrchestrationTaskRuntimeState.GrantAccepted,
            OrchestrationTaskRuntimeState.Running
        };
        foreach (var taskState in activeTaskStates)
        {
            Assert.Contains(
                OrchestrationExecutionStateMachine.TaskTransitions,
                item => item.From == taskState &&
                        item.Trigger == OrchestrationTaskRuntimeTrigger.SupervisorFailed &&
                        item.To == OrchestrationTaskRuntimeState.Failed);
            Assert.Contains(
                OrchestrationExecutionStateMachine.TaskTransitions,
                item => item.From == taskState &&
                        item.Trigger == OrchestrationTaskRuntimeTrigger.ReplacementRetryScheduled &&
                        item.To == OrchestrationTaskRuntimeState.RetryScheduled);
        }

        Assert.DoesNotContain(
            OrchestrationExecutionStateMachine.TaskTransitions,
            item => (item.Trigger == OrchestrationTaskRuntimeTrigger.TaskSucceeded ||
                     item.Trigger == OrchestrationTaskRuntimeTrigger.TaskFailed) &&
                    item.From != OrchestrationTaskRuntimeState.Running);
    }

    [Fact]
    public void StateMachineWorkerLifecycleMatchesTransitionTableForEveryStateAndTrigger()
    {
        foreach (var stateValue in Enum.GetValues<OrchestrationWorkerRuntimeState>())
        {
            foreach (var trigger in Enum.GetValues<OrchestrationWorkerRuntimeTrigger>())
            {
                var state = new OrchestrationExecutionStateMachine(["pipeline:CustomerLoad:task:1"]);
                RegisterWorkerAtPhase(state, stateValue);
                var isDeclared = OrchestrationExecutionStateMachine.WorkerTransitions.Any(
                    item => item.From == stateValue && item.Trigger == trigger);

                if (stateValue == OrchestrationWorkerRuntimeState.Closed &&
                    trigger == OrchestrationWorkerRuntimeTrigger.WorkerClosed)
                {
                    state.MarkWorkerClosed("CustomerLoad");
                    continue;
                }

                if (isDeclared)
                {
                    ApplyWorkerTrigger(state, trigger);
                }
                else
                {
                    Assert.Throws<InvalidOperationException>(() => ApplyWorkerTrigger(state, trigger));
                }
            }
        }
    }

    [Fact]
    public void StateMachineTaskLifecycleMatchesTransitionTableForEveryStateAndTrigger()
    {
        foreach (var stateValue in Enum.GetValues<OrchestrationTaskRuntimeState>())
        {
            foreach (var trigger in Enum.GetValues<OrchestrationTaskRuntimeTrigger>())
            {
                var state = CreateTaskAtPhase(stateValue);
                var isDeclared = OrchestrationExecutionStateMachine.TaskTransitions.Any(
                    item => item.From == stateValue && item.Trigger == trigger);

                if (isDeclared)
                {
                    ApplyTaskTrigger(state, trigger);
                }
                else
                {
                    Assert.Throws<InvalidOperationException>(() => ApplyTaskTrigger(state, trigger));
                }
            }
        }
    }

    [Fact]
    public void StateMachineCrashMatrixCoversWorkerLifecyclePhases()
    {
        var phases = new[]
        {
            OrchestrationWorkerRuntimeState.Starting,
            OrchestrationWorkerRuntimeState.Online,
            OrchestrationWorkerRuntimeState.Ready,
            OrchestrationWorkerRuntimeState.StartPipelineSent,
            OrchestrationWorkerRuntimeState.PipelineStarted
        };

        foreach (var phase in phases)
        {
            var state = new OrchestrationExecutionStateMachine(["pipeline:CustomerLoad:task:1"]);
            RegisterWorkerAtPhase(state, phase);

            state.MarkWorkerClosed("CustomerLoad");
            state.RegisterWorker("CustomerLoad", "pipeline:CustomerLoad", string.Empty, "test-version");
            state.MarkWorkerOnline("CustomerLoad", "test-version");
            state.MarkWorkerReady("CustomerLoad");
            state.MarkStartPipelineSent("CustomerLoad");
            state.MarkPipelineStarted("CustomerLoad");
            state.MarkReady("pipeline:CustomerLoad:task:1", "CustomerLoad");

            Assert.True(state.IsReady("pipeline:CustomerLoad:task:1"));
        }
    }

    [Fact]
    public void StateMachineWorkerProgressPredicatesMatchExplicitLifecycleTruthTable()
    {
        var expected = new Dictionary<OrchestrationWorkerRuntimeState, bool[]>
        {
            [OrchestrationWorkerRuntimeState.Starting] = [false, false, false, false],
            [OrchestrationWorkerRuntimeState.Online] = [true, false, false, false],
            [OrchestrationWorkerRuntimeState.Ready] = [true, true, false, false],
            [OrchestrationWorkerRuntimeState.StartPipelineSent] = [true, true, true, false],
            [OrchestrationWorkerRuntimeState.PipelineStarted] = [true, true, true, true],
            [OrchestrationWorkerRuntimeState.Closed] = [false, false, false, false]
        };

        foreach (var (phase, expectedPredicates) in expected)
        {
            var state = new OrchestrationExecutionStateMachine(["pipeline:CustomerLoad:task:1"]);
            if (phase == OrchestrationWorkerRuntimeState.Closed)
            {
                RegisterWorkerAtPhase(state, OrchestrationWorkerRuntimeState.PipelineStarted);
                state.MarkWorkerClosed("CustomerLoad");
            }
            else
            {
                RegisterWorkerAtPhase(state, phase);
            }

            Assert.Equal(expectedPredicates[0], state.WorkerIsOnline("CustomerLoad"));
            Assert.Equal(expectedPredicates[1], state.WorkerIsReady("CustomerLoad"));
            Assert.Equal(expectedPredicates[2], state.WorkerStartPipelineSent("CustomerLoad"));
            Assert.Equal(expectedPredicates[3], state.WorkerPipelineStarted("CustomerLoad"));
        }
    }

    [Fact]
    public void StateMachineCrashMatrixCoversActiveGrantPhases()
    {
        var activeStates = new[]
        {
            OrchestrationTaskRuntimeState.GrantIssued,
            OrchestrationTaskRuntimeState.GrantAccepted,
            OrchestrationTaskRuntimeState.Running
        };

        foreach (var activeState in activeStates)
        {
            var failedState = CreateActiveGrantState(activeState);
            failedState.MarkFailedFromSupervisor("pipeline:CustomerLoad:task:1");
            failedState.MarkWorkerClosed("CustomerLoad");
            Assert.False(failedState.HasUnresolvedTasks);

            var retryState = CreateActiveGrantState(activeState);
            retryState.MarkRetryScheduledForReplacement("pipeline:CustomerLoad:task:1", "grant-1", 2);
            retryState.MarkWorkerClosed("CustomerLoad");
            retryState.MarkPendingForReplacement("pipeline:CustomerLoad:task:1");
            retryState.RegisterWorker("CustomerLoad", "pipeline:CustomerLoad", "pipeline:CustomerLoad:task:1", "test-version");
            retryState.MarkWorkerOnline("CustomerLoad", "test-version");
            retryState.MarkWorkerReady("CustomerLoad");
            retryState.MarkStartPipelineSent("CustomerLoad");
            retryState.MarkPipelineStarted("CustomerLoad");
            retryState.MarkReady("pipeline:CustomerLoad:task:1", "CustomerLoad");

            Assert.True(retryState.IsReady("pipeline:CustomerLoad:task:1"));
        }
    }

    private static int ResolveIterationCount()
    {
        var rawValue = Environment.GetEnvironmentVariable("META_ORCH_STATE_FUZZ_ITERATIONS");
        if (int.TryParse(rawValue, out var parsed) && parsed > 0)
        {
            return parsed;
        }

        return 20_000;
    }

    private static void RegisterWorkerAtPhase(
        OrchestrationExecutionStateMachine state,
        OrchestrationWorkerRuntimeState phase)
    {
        state.RegisterWorker("CustomerLoad", "pipeline:CustomerLoad", string.Empty, "test-version");
        if (phase == OrchestrationWorkerRuntimeState.Starting)
        {
            return;
        }

        state.MarkWorkerOnline("CustomerLoad", "test-version");
        if (phase == OrchestrationWorkerRuntimeState.Online)
        {
            return;
        }

        state.MarkWorkerReady("CustomerLoad");
        if (phase == OrchestrationWorkerRuntimeState.Ready)
        {
            return;
        }

        state.MarkStartPipelineSent("CustomerLoad");
        if (phase == OrchestrationWorkerRuntimeState.StartPipelineSent)
        {
            return;
        }

        state.MarkPipelineStarted("CustomerLoad");
        if (phase == OrchestrationWorkerRuntimeState.Closed)
        {
            state.MarkWorkerClosed("CustomerLoad");
        }
    }

    private static OrchestrationExecutionStateMachine CreateActiveGrantState(
        OrchestrationTaskRuntimeState activeState)
    {
        var state = new OrchestrationExecutionStateMachine(["pipeline:CustomerLoad:task:1"]);
        state.RegisterWorker("CustomerLoad", "pipeline:CustomerLoad", string.Empty, "test-version");
        state.MarkWorkerOnline("CustomerLoad", "test-version");
        state.MarkWorkerReady("CustomerLoad");
        state.MarkStartPipelineSent("CustomerLoad");
        state.MarkPipelineStarted("CustomerLoad");
        state.MarkReady("pipeline:CustomerLoad:task:1", "CustomerLoad");
        state.MarkGrantIssued("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-1", "command-1", 1);
        if (activeState == OrchestrationTaskRuntimeState.GrantIssued)
        {
            return state;
        }

        state.MarkGrantAccepted("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-1", "command-1", 1);
        if (activeState == OrchestrationTaskRuntimeState.GrantAccepted)
        {
            return state;
        }

        state.MarkTaskStarted("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-1", "command-1", 1);
        return state;
    }

    private static OrchestrationExecutionStateMachine CreateTaskAtPhase(
        OrchestrationTaskRuntimeState phase)
    {
        var state = new OrchestrationExecutionStateMachine(["pipeline:CustomerLoad:task:1"]);
        state.RegisterWorker("CustomerLoad", "pipeline:CustomerLoad", string.Empty, "test-version");
        state.MarkWorkerOnline("CustomerLoad", "test-version");
        state.MarkWorkerReady("CustomerLoad");
        state.MarkStartPipelineSent("CustomerLoad");
        state.MarkPipelineStarted("CustomerLoad");
        if (phase == OrchestrationTaskRuntimeState.Pending)
        {
            return state;
        }

        state.MarkReady("pipeline:CustomerLoad:task:1", "CustomerLoad");
        if (phase == OrchestrationTaskRuntimeState.Ready)
        {
            return state;
        }

        state.MarkGrantIssued("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-1", "command-1", 1);
        if (phase == OrchestrationTaskRuntimeState.GrantIssued)
        {
            return state;
        }

        if (phase == OrchestrationTaskRuntimeState.GrantAccepted)
        {
            state.MarkGrantAccepted("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-1", "command-1", 1);
            return state;
        }

        state.MarkTaskStarted("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-1", "command-1", 1);
        switch (phase)
        {
            case OrchestrationTaskRuntimeState.Running:
                return state;
            case OrchestrationTaskRuntimeState.RetryScheduled:
                state.MarkRetryScheduledForReplacement("pipeline:CustomerLoad:task:1", "grant-1", 2);
                return state;
            case OrchestrationTaskRuntimeState.Succeeded:
                state.MarkSucceeded("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-1", "command-1", 1);
                return state;
            case OrchestrationTaskRuntimeState.Failed:
                state.MarkFailed("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-1", "command-1", 1);
                return state;
            case OrchestrationTaskRuntimeState.Blocked:
                var blockedState = new OrchestrationExecutionStateMachine(["pipeline:CustomerLoad:task:1"]);
                blockedState.RegisterWorker("CustomerLoad", "pipeline:CustomerLoad", string.Empty, "test-version");
                blockedState.MarkWorkerOnline("CustomerLoad", "test-version");
                blockedState.MarkWorkerReady("CustomerLoad");
                blockedState.MarkStartPipelineSent("CustomerLoad");
                blockedState.MarkPipelineStarted("CustomerLoad");
                blockedState.MarkBlocked("pipeline:CustomerLoad:task:1");
                return blockedState;
            default:
                throw new ArgumentOutOfRangeException(nameof(phase), phase, "Unknown task phase.");
        }
    }

    private static void ApplyWorkerTrigger(
        OrchestrationExecutionStateMachine state,
        OrchestrationWorkerRuntimeTrigger trigger)
    {
        switch (trigger)
        {
            case OrchestrationWorkerRuntimeTrigger.WorkerOnline:
                state.MarkWorkerOnline("CustomerLoad", "test-version");
                break;
            case OrchestrationWorkerRuntimeTrigger.WorkerReady:
                state.MarkWorkerReady("CustomerLoad");
                break;
            case OrchestrationWorkerRuntimeTrigger.StartPipelineSent:
                state.MarkStartPipelineSent("CustomerLoad");
                break;
            case OrchestrationWorkerRuntimeTrigger.PipelineStarted:
                state.MarkPipelineStarted("CustomerLoad");
                break;
            case OrchestrationWorkerRuntimeTrigger.WorkerClosed:
                state.MarkWorkerClosed("CustomerLoad");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(trigger), trigger, "Unknown worker trigger.");
        }
    }

    private static void ApplyTaskTrigger(
        OrchestrationExecutionStateMachine state,
        OrchestrationTaskRuntimeTrigger trigger)
    {
        switch (trigger)
        {
            case OrchestrationTaskRuntimeTrigger.WorkerTaskReady:
                state.MarkReady("pipeline:CustomerLoad:task:1", "CustomerLoad");
                break;
            case OrchestrationTaskRuntimeTrigger.GrantIssued:
                state.MarkGrantIssued("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-2", "command-2", 2);
                break;
            case OrchestrationTaskRuntimeTrigger.GrantAccepted:
                state.MarkGrantAccepted("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-1", "command-1", 1);
                break;
            case OrchestrationTaskRuntimeTrigger.TaskStarted:
                state.MarkTaskStarted("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-1", "command-1", 1);
                break;
            case OrchestrationTaskRuntimeTrigger.TaskSucceeded:
                state.MarkSucceeded("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-1", "command-1", 1);
                break;
            case OrchestrationTaskRuntimeTrigger.TaskFailed:
                state.MarkFailed("pipeline:CustomerLoad:task:1", "CustomerLoad", "grant-1", "command-1", 1);
                break;
            case OrchestrationTaskRuntimeTrigger.SupervisorFailed:
                state.MarkFailedFromSupervisor("pipeline:CustomerLoad:task:1");
                break;
            case OrchestrationTaskRuntimeTrigger.SameWorkerRetryScheduled:
                state.MarkReadyForSameWorkerRetry(
                    "pipeline:CustomerLoad:task:1",
                    "CustomerLoad",
                    "grant-1",
                    "command-1",
                    1,
                    "grant-1",
                    2);
                break;
            case OrchestrationTaskRuntimeTrigger.ReplacementRetryScheduled:
                state.MarkRetryScheduledForReplacement("pipeline:CustomerLoad:task:1", "grant-1", 2);
                break;
            case OrchestrationTaskRuntimeTrigger.ReplacementWorkerReady:
                state.MarkPendingForReplacement("pipeline:CustomerLoad:task:1");
                break;
            case OrchestrationTaskRuntimeTrigger.ReadyWorkerLost:
                state.MarkPendingAfterReadyWorkerLost("pipeline:CustomerLoad:task:1", "CustomerLoad");
                break;
            case OrchestrationTaskRuntimeTrigger.Blocked:
                state.MarkBlocked("pipeline:CustomerLoad:task:1");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(trigger), trigger, "Unknown task trigger.");
        }
    }

    private static class OrchestrationStateMachineProtocolFuzzer
    {
        public static FuzzResult Run(int scenarios, uint seed)
        {
            var random = new XorShift32(seed);
            var result = new FuzzResult();
            for (var scenario = 0; scenario < scenarios; scenario++)
            {
                RunScenario(ref random, result, scenario);
                result.Scenarios++;
            }

            return result;
        }

        private static void RunScenario(ref XorShift32 random, FuzzResult result, int scenario)
        {
            var taskCount = 1 + random.Next(5);
            var taskIds = Enumerable
                .Range(1, taskCount)
                .Select(index => $"pipeline:Load:task:{index.ToString(System.Globalization.CultureInfo.InvariantCulture)}")
                .ToArray();
            var terminal = new bool[taskCount];
            var state = new OrchestrationExecutionStateMachine(taskIds);
            var worker = new SimulatedWorker("Load");

            worker = RegisterAndStartWorker(state, worker, ref random, result, allowLifecycleCrashes: true);

            for (var taskIndex = 0; taskIndex < taskIds.Length; taskIndex++)
            {
                if (terminal[taskIndex])
                {
                    continue;
                }

                var taskId = taskIds[taskIndex];
                if (random.Percent(3))
                {
                    state.MarkBlocked(taskId);
                    terminal[taskIndex] = true;
                    result.Blocks++;
                    BlockRemaining(state, taskIds, terminal, taskIndex + 1, result);
                    break;
                }

                ReadyTask(state, worker, taskId, ref random, result);
                var attempts = 0;
                while (!terminal[taskIndex])
                {
                    attempts++;
                    if (attempts > 12)
                    {
                        state.MarkBlocked(taskId);
                        terminal[taskIndex] = true;
                        result.Blocks++;
                        BlockRemaining(state, taskIds, terminal, taskIndex + 1, result);
                        break;
                    }

                    var command = RoundTripCommand(new WorkerProtocolCommand(
                        WorkerCommandKinds.GrantTask,
                        $"cmd-{scenario}-{taskIndex}-{attempts}",
                        $"grant-{scenario}-{taskIndex}-{attempts}",
                        attempts == 1 ? string.Empty : $"grant-{scenario}-{taskIndex}-{attempts - 1}",
                        attempts,
                        "pipeline:Load",
                        worker.Name,
                        taskId,
                        string.Empty));
                    state.MarkGrantIssued(
                        command.TaskId,
                        command.PipelineName,
                        command.GrantId,
                        command.CommandId,
                        command.AttemptNumber);
                    AssertInvariant(state.HasActiveGrant(taskId), result, "grant issue should make task active");

                    RejectIllegalTerminalBeforeStart(state, worker, taskId, command, result);

                    if (CrashDuringActiveGrant(state, worker, taskId, command, taskIds, terminal, taskIndex, ref random, result, beforeTaskStarted: true))
                    {
                        if (terminal[taskIndex])
                        {
                            break;
                        }

                        continue;
                    }

                    if (random.Percent(70))
                    {
                        var accepted = RoundTripEvent(
                            WorkerEventKinds.GrantAccepted,
                            worker,
                            taskId,
                            command.GrantId,
                            command.CommandId,
                            command.AttemptNumber);
                        state.ValidateTaskEvent(worker.Name, accepted.Kind, accepted.TaskId);
                        state.MarkGrantAccepted(
                            accepted.TaskId,
                            worker.Name,
                            accepted.GrantId,
                            accepted.CommandId,
                            accepted.AttemptNumber);
                    }

                    RejectWrongGrantEvidence(state, worker, taskId, command, result);

                    var started = RoundTripEvent(
                        WorkerEventKinds.TaskStarted,
                        worker,
                        taskId,
                        command.GrantId,
                        command.CommandId,
                        command.AttemptNumber);
                    state.ValidateTaskEvent(worker.Name, started.Kind, started.TaskId);
                    state.MarkTaskStarted(
                        started.TaskId,
                        worker.Name,
                        started.GrantId,
                        started.CommandId,
                        started.AttemptNumber);
                    AssertInvariant(state.IsRunning(taskId), result, "task should be running after TaskStarted");

                    if (CrashDuringActiveGrant(state, worker, taskId, command, taskIds, terminal, taskIndex, ref random, result, beforeTaskStarted: false))
                    {
                        if (terminal[taskIndex])
                        {
                            break;
                        }

                        continue;
                    }

                    var outcome = random.Next(100);
                    if (outcome < 58)
                    {
                        var succeeded = RoundTripEvent(
                            WorkerEventKinds.TaskSucceeded,
                            worker,
                            taskId,
                            command.GrantId,
                            command.CommandId,
                            command.AttemptNumber);
                        state.ValidateTaskEvent(worker.Name, succeeded.Kind, succeeded.TaskId);
                        state.MarkSucceeded(
                            succeeded.TaskId,
                            worker.Name,
                            succeeded.GrantId,
                            succeeded.CommandId,
                            succeeded.AttemptNumber);
                        terminal[taskIndex] = true;
                        result.Successes++;
                        AssertInvariant(!state.HasActiveGrant(taskId), result, "success should clear active grant");
                        break;
                    }

                    if (outcome < 74)
                    {
                        var failed = RoundTripEvent(
                            WorkerEventKinds.TaskFailed,
                            worker,
                            taskId,
                            command.GrantId,
                            command.CommandId,
                            command.AttemptNumber);
                        state.ValidateTaskEvent(worker.Name, failed.Kind, failed.TaskId);
                        state.MarkReadyForSameWorkerRetry(
                            failed.TaskId,
                            worker.Name,
                            failed.GrantId,
                            failed.CommandId,
                            failed.AttemptNumber,
                            failed.GrantId,
                            attempts + 1);
                        result.SameWorkerRetries++;
                        AssertInvariant(state.IsReady(taskId), result, "same-worker retry should return to ready");
                        continue;
                    }

                    if (outcome < 90)
                    {
                        ScheduleReplacement(state, ref worker, taskId, command.GrantId, attempts + 1, ref random, result);
                        ReadyTask(state, worker, taskId, ref random, result);
                        continue;
                    }

                    var terminalFailed = RoundTripEvent(
                        WorkerEventKinds.TaskFailed,
                        worker,
                        taskId,
                        command.GrantId,
                        command.CommandId,
                        command.AttemptNumber);
                    state.ValidateTaskEvent(worker.Name, terminalFailed.Kind, terminalFailed.TaskId);
                    state.MarkFailed(
                        terminalFailed.TaskId,
                        worker.Name,
                        terminalFailed.GrantId,
                        terminalFailed.CommandId,
                        terminalFailed.AttemptNumber);
                    terminal[taskIndex] = true;
                    result.Failures++;
                    BlockRemaining(state, taskIds, terminal, taskIndex + 1, result);
                    break;
                }
            }

            AssertInvariant(!state.HasUnresolvedTasks, result, "scenario ended with unresolved tasks");
        }

        private static SimulatedWorker RegisterAndStartWorker(
            OrchestrationExecutionStateMachine state,
            SimulatedWorker worker,
            ref XorShift32 random,
            FuzzResult result,
            bool allowLifecycleCrashes)
        {
            for (var restartCount = 0; restartCount < 100; restartCount++)
            {
                state.RegisterWorker(worker.Name, worker.PipelineId, worker.ResumeTaskId, worker.ExpectedVersion);
                if (MaybeCrashWorkerLifecycle(
                    state,
                    ref worker,
                    WorkerCrashPhase.BeforeOnline,
                    ref random,
                    result,
                    allowLifecycleCrashes))
                {
                    continue;
                }

                var online = RoundTripEvent(WorkerEventKinds.WorkerOnline, worker, string.Empty, string.Empty, string.Empty, 0);
                state.MarkWorkerOnline(worker.Name, online.ExecutableVersion);
                if (MaybeCrashWorkerLifecycle(
                    state,
                    ref worker,
                    WorkerCrashPhase.AfterOnline,
                    ref random,
                    result,
                    allowLifecycleCrashes))
                {
                    continue;
                }

                state.MarkWorkerReady(worker.Name);
                if (MaybeCrashWorkerLifecycle(
                    state,
                    ref worker,
                    WorkerCrashPhase.AfterReady,
                    ref random,
                    result,
                    allowLifecycleCrashes))
                {
                    continue;
                }

                state.MarkStartPipelineSent(worker.Name);
                if (MaybeCrashWorkerLifecycle(
                    state,
                    ref worker,
                    WorkerCrashPhase.AfterStartPipelineSent,
                    ref random,
                    result,
                    allowLifecycleCrashes))
                {
                    continue;
                }

                state.MarkPipelineStarted(worker.Name);
                if (MaybeCrashWorkerLifecycle(
                    state,
                    ref worker,
                    WorkerCrashPhase.AfterPipelineStarted,
                    ref random,
                    result,
                    allowLifecycleCrashes))
                {
                    continue;
                }

                return worker;
            }

            throw new InvalidOperationException("Fuzzer restarted the same pipeline worker too many times before activation.");
        }

        private static bool MaybeCrashWorkerLifecycle(
            OrchestrationExecutionStateMachine state,
            ref SimulatedWorker worker,
            WorkerCrashPhase phase,
            ref XorShift32 random,
            FuzzResult result,
            bool allowLifecycleCrashes)
        {
            if (!allowLifecycleCrashes)
            {
                return false;
            }

            var crashChance = phase == WorkerCrashPhase.BeforeOnline ? 5 : 3;
            if (!random.Percent(crashChance))
            {
                return false;
            }

            state.MarkWorkerClosed(worker.Name);
            result.RecordWorkerCrash(phase);
            worker = worker.NextReplacement(worker.ResumeTaskId);
            return true;
        }

        private static void ReadyTask(
            OrchestrationExecutionStateMachine state,
            SimulatedWorker worker,
            string taskId,
            ref XorShift32 random,
            FuzzResult result)
        {
            if (random.Percent(5))
            {
                ExpectInvalid(
                    () => state.ValidateTaskEvent(worker.Name, WorkerEventKinds.TaskReady, "pipeline:Load:missing"),
                    result);
            }

            var ready = RoundTripEvent(WorkerEventKinds.TaskReady, worker, taskId, string.Empty, string.Empty, 0);
            state.ValidateTaskEvent(worker.Name, ready.Kind, ready.TaskId);
            state.MarkReady(taskId, worker.Name);
            AssertInvariant(state.IsReady(taskId), result, "TaskReady should make task ready");

            if (random.Percent(5))
            {
                ExpectInvalid(() => state.ValidateTaskEvent(worker.Name, WorkerEventKinds.TaskReady, taskId), result);
            }
        }

        private static bool CrashDuringActiveGrant(
            OrchestrationExecutionStateMachine state,
            SimulatedWorker worker,
            string taskId,
            WorkerProtocolCommand command,
            IReadOnlyList<string> taskIds,
            bool[] terminal,
            int taskIndex,
            ref XorShift32 random,
            FuzzResult result,
            bool beforeTaskStarted)
        {
            var crashChance = beforeTaskStarted ? 6 : 10;
            if (!random.Percent(crashChance))
            {
                return false;
            }

            result.RecordWorkerCrash(beforeTaskStarted ? WorkerCrashPhase.BeforeTaskStarted : WorkerCrashPhase.AfterTaskStarted);
            if (random.Percent(72))
            {
                var replacement = worker.NextReplacement(taskId);
                state.MarkRetryScheduledForReplacement(taskId, command.GrantId, command.AttemptNumber + 1);
                state.MarkWorkerClosed(worker.Name);
                state.MarkPendingForReplacement(taskId);
                replacement = RegisterAndStartWorker(state, replacement, ref random, result, allowLifecycleCrashes: true);
                worker.CopyFrom(replacement);
                ReadyTask(state, worker, taskId, ref random, result);
                result.ReplacementRetries++;
                return true;
            }

            state.MarkFailedFromSupervisor(taskId);
            state.MarkWorkerClosed(worker.Name);
            terminal[taskIndex] = true;
            result.Failures++;
            BlockRemaining(state, taskIds, terminal, taskIndex + 1, result);
            return true;
        }

        private static void ScheduleReplacement(
            OrchestrationExecutionStateMachine state,
            ref SimulatedWorker worker,
            string taskId,
            string previousGrantId,
            int nextAttempt,
            ref XorShift32 random,
            FuzzResult result)
        {
            var replacement = worker.NextReplacement(taskId);
            state.MarkRetryScheduledForReplacement(taskId, previousGrantId, nextAttempt);
            state.MarkWorkerClosed(worker.Name);
            state.MarkPendingForReplacement(taskId);
            replacement = RegisterAndStartWorker(state, replacement, ref random, result, allowLifecycleCrashes: true);
            worker = replacement;
            result.ReplacementRetries++;
        }

        private static void RejectIllegalTerminalBeforeStart(
            OrchestrationExecutionStateMachine state,
            SimulatedWorker worker,
            string taskId,
            WorkerProtocolCommand command,
            FuzzResult result)
        {
            var succeeded = RoundTripEvent(
                WorkerEventKinds.TaskSucceeded,
                worker,
                taskId,
                command.GrantId,
                command.CommandId,
                command.AttemptNumber);
            ExpectInvalid(() => state.ValidateTaskEvent(worker.Name, succeeded.Kind, succeeded.TaskId), result);
        }

        private static void RejectWrongGrantEvidence(
            OrchestrationExecutionStateMachine state,
            SimulatedWorker worker,
            string taskId,
            WorkerProtocolCommand command,
            FuzzResult result)
        {
            var wrongGrant = RoundTripEvent(
                WorkerEventKinds.TaskStarted,
                worker,
                taskId,
                command.GrantId + "-wrong",
                command.CommandId,
                command.AttemptNumber);
            ExpectInvalid(
                () => state.MarkTaskStarted(
                    wrongGrant.TaskId,
                    worker.Name,
                    wrongGrant.GrantId,
                    wrongGrant.CommandId,
                    wrongGrant.AttemptNumber),
                result);
        }

        private static void BlockRemaining(
            OrchestrationExecutionStateMachine state,
            IReadOnlyList<string> taskIds,
            bool[] terminal,
            int startIndex,
            FuzzResult result)
        {
            for (var index = startIndex; index < taskIds.Count; index++)
            {
                if (terminal[index])
                {
                    continue;
                }

                state.MarkBlocked(taskIds[index]);
                terminal[index] = true;
                result.Blocks++;
            }
        }

        private static WorkerProtocolEvent RoundTripEvent(
            string kind,
            SimulatedWorker worker,
            string taskId,
            string grantId,
            string commandId,
            int attempt)
        {
            var source = new WorkerProtocolEvent(
                kind,
                worker.WorkerId,
                worker.PipelineId,
                worker.Name,
                taskId,
                string.IsNullOrWhiteSpace(taskId) ? string.Empty : taskId.Split(':').Last(),
                grantId,
                commandId,
                attempt,
                0,
                worker.ExpectedVersion,
                string.Empty,
                string.Empty);
            Assert.True(OrchestrationWorkerProtocol.TryDecodeEvent(OrchestrationWorkerProtocol.EncodeEvent(source), out var decoded));
            return decoded;
        }

        private static WorkerProtocolCommand RoundTripCommand(WorkerProtocolCommand source)
        {
            Assert.True(OrchestrationWorkerProtocol.TryDecodeCommand(OrchestrationWorkerProtocol.EncodeCommand(source), out var decoded));
            return decoded;
        }

        private static void ExpectInvalid(Action action, FuzzResult result)
        {
            Assert.Throws<InvalidOperationException>(action);
            result.IllegalEventsRejected++;
        }

        private static void AssertInvariant(bool condition, FuzzResult result, string message)
        {
            if (condition)
            {
                return;
            }

            result.InvariantFailures++;
            throw new InvalidOperationException(message);
        }
    }

    private sealed class FuzzResult
    {
        public int Scenarios { get; set; }

        public int Successes { get; set; }

        public int Failures { get; set; }

        public int Blocks { get; set; }

        public int WorkerCrashes { get; set; }

        public int WorkerCrashesBeforeOnline { get; set; }

        public int WorkerCrashesAfterOnline { get; set; }

        public int WorkerCrashesAfterReady { get; set; }

        public int WorkerCrashesAfterStartPipelineSent { get; set; }

        public int WorkerCrashesAfterPipelineStarted { get; set; }

        public int WorkerCrashesBeforeTaskStarted { get; set; }

        public int WorkerCrashesAfterTaskStarted { get; set; }

        public int ReplacementRetries { get; set; }

        public int SameWorkerRetries { get; set; }

        public int IllegalEventsRejected { get; set; }

        public int InvariantFailures { get; set; }

        public void RecordWorkerCrash(WorkerCrashPhase phase)
        {
            WorkerCrashes++;
            switch (phase)
            {
                case WorkerCrashPhase.BeforeOnline:
                    WorkerCrashesBeforeOnline++;
                    break;
                case WorkerCrashPhase.AfterOnline:
                    WorkerCrashesAfterOnline++;
                    break;
                case WorkerCrashPhase.AfterReady:
                    WorkerCrashesAfterReady++;
                    break;
                case WorkerCrashPhase.AfterStartPipelineSent:
                    WorkerCrashesAfterStartPipelineSent++;
                    break;
                case WorkerCrashPhase.AfterPipelineStarted:
                    WorkerCrashesAfterPipelineStarted++;
                    break;
                case WorkerCrashPhase.BeforeTaskStarted:
                    WorkerCrashesBeforeTaskStarted++;
                    break;
                case WorkerCrashPhase.AfterTaskStarted:
                    WorkerCrashesAfterTaskStarted++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, "Unknown worker crash phase.");
            }
        }
    }

    private enum WorkerCrashPhase
    {
        BeforeOnline,
        AfterOnline,
        AfterReady,
        AfterStartPipelineSent,
        AfterPipelineStarted,
        BeforeTaskStarted,
        AfterTaskStarted
    }

    private sealed class SimulatedWorker
    {
        private int generation;

        public SimulatedWorker(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public string PipelineId => $"pipeline:{Name}";

        public string ResumeTaskId { get; private set; } = string.Empty;

        public string WorkerId => $"{Name}-{generation.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

        public string ExpectedVersion => "test-version";

        public SimulatedWorker NextReplacement(string resumeTaskId = "")
        {
            return new SimulatedWorker(Name)
            {
                generation = generation + 1,
                ResumeTaskId = resumeTaskId
            };
        }

        public void CopyFrom(SimulatedWorker other)
        {
            generation = other.generation;
            ResumeTaskId = other.ResumeTaskId;
        }
    }

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
}
