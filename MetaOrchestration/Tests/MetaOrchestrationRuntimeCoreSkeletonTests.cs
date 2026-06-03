using System.Collections;
using System.Reflection;
using MetaBi.Tests.Common;
using MetaOrchestration.Core;
using MetaOrchestration.Core.Runtime;
using MetaOrchestration.WorkerProtocol;

namespace MetaOrchestration.Tests;

public sealed class MetaOrchestrationRuntimeCoreSkeletonTests
{
    [Fact]
    public void ReducersExposeOnlySingleApplyBoundary()
    {
        AssertSingleReducerBoundary(typeof(ExecutionStateReducer));
        AssertSingleReducerBoundary(typeof(ActivationStateReducer));
    }

    [Fact]
    public void RuntimeStateDoesNotExposeRawMutableCollections()
    {
        var types = new[]
        {
            typeof(RuntimeState),
            typeof(PendingTasks),
            typeof(ReadyQueue),
            typeof(RunningGrants),
            typeof(RuntimeLocks),
            typeof(RetrySchedule),
            typeof(PipelineOutcomes),
            typeof(WorkerRegistry),
            typeof(PipelineActivations)
        };

        foreach (var type in types)
        {
            var exposedMembers = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(static item => item.GetMethod is { IsPrivate: false })
                .Select(item => (Member: item.Name, Type: item.PropertyType))
                .Concat(type
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .Where(static item => !item.IsPrivate)
                    .Select(item => (Member: item.Name, Type: item.FieldType)));

            foreach (var member in exposedMembers)
            {
                Assert.False(
                    IsRawMutableCollection(member.Type),
                    $"{type.Name}.{member.Member} exposes raw mutable collection type {member.Type.Name}.");
            }
        }
    }

    [Fact]
    public void ExistingRuntimeServiceDoesNotDependOnNewReducersByType()
    {
        var forbidden = new HashSet<Type>
        {
            typeof(ExecutionStateReducer),
            typeof(ActivationStateReducer),
            typeof(RuntimeState)
        };

        var serviceType = typeof(MetaOrchestrationRuntimeService);
        var exposedTypes = serviceType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .SelectMany(static item => item.GetParameters().Select(static parameter => parameter.ParameterType))
            .Concat(serviceType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(static item => item.FieldType))
            .Concat(serviceType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(static item => item.PropertyType))
            .Concat(serviceType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Select(static item => item.ReturnType))
            .Concat(serviceType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .SelectMany(static item => item.GetParameters().Select(static parameter => parameter.ParameterType)));

        Assert.DoesNotContain(exposedTypes, forbidden.Contains);
    }

    [Fact]
    public void RuntimeServiceSourceUsesRuntimeCoreBoundaryWithoutOldRuntimeTypes()
    {
        var repoRoot = CliTestRunner.FindRepositoryRoot("MetaOrchestration");
        var source = File.ReadAllText(Path.Combine(
            repoRoot,
            "MetaOrchestration",
            "Core",
            "MetaOrchestrationRuntimeService.cs"));

        Assert.Contains("RuntimeEvent.", source, StringComparison.Ordinal);
        Assert.Contains("RuntimeAction.", source, StringComparison.Ordinal);
        Assert.Contains("MetaOrchestrationRuntimeKernel", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ExecutionStateReducer", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ActivationStateReducer", source, StringComparison.Ordinal);
        Assert.DoesNotContain("OrchestrationRuntimeKernelEvent", source, StringComparison.Ordinal);
        Assert.DoesNotContain("OrchestrationRuntimeKernelAction", source, StringComparison.Ordinal);
        Assert.DoesNotContain("OrchestrationExecutionStateMachine", source, StringComparison.Ordinal);
        Assert.DoesNotContain("OrchestrationWorkerActivationStateMachine", source, StringComparison.Ordinal);
    }

    [Fact]
    public void InvalidExecutionTransitionFailsThroughReducerBoundary()
    {
        var reducer = new ExecutionStateReducer();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            reducer.Apply(
                new ExecutionState(TaskRuntimeState.Pending, WorkerRuntimeState.Starting, GrantRuntimeState.None),
                ExecutionTrigger.TaskSucceeded,
                new ExecutionFacts("task-1", "WorkerA", string.Empty, string.Empty, 0)));

        Assert.Contains("Illegal execution transition", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidExecutionTransitionProducesExplicitResult()
    {
        var reducer = new ExecutionStateReducer();

        var result = reducer.Apply(
            new ExecutionState(TaskRuntimeState.Pending, WorkerRuntimeState.PipelineStarted, GrantRuntimeState.None),
            ExecutionTrigger.TaskReady,
            new ExecutionFacts("task-1", "WorkerA", string.Empty, string.Empty, 0));

        Assert.Equal(TaskRuntimeState.Ready, result.State.Task);
        Assert.Equal(WorkerRuntimeState.PipelineStarted, result.State.Worker);
        Assert.Equal(GrantRuntimeState.None, result.State.Grant);
    }

    [Fact]
    public void KernelVerticalSliceStartsWorkerAndIssuesGrantAfterCoherentBookkeeping()
    {
        var kernel = CreateKernel();

        var tick = kernel.RegisterEvent(new RuntimeEvent.SchedulerTick(DateTimeOffset.UtcNow, MaxActiveWorkerProcesses: 1));
        var startWorker = Assert.Single(DomainActions(tick));
        var start = Assert.IsType<RuntimeAction.StartWorker>(startWorker);
        Assert.Equal("CustomerLoad", start.WorkerName);

        Assert.Empty(DomainActions(kernel.RegisterEvent(new RuntimeEvent.WorkerOnline("CustomerLoad", "pipeline:CustomerLoad", "test-version"))));

        var ready = kernel.RegisterEvent(new RuntimeEvent.WorkerReady("CustomerLoad"));
        var startPipeline = Assert.IsType<RuntimeAction.SendStartPipeline>(Assert.Single(DomainActions(ready)));
        Assert.Equal("CustomerLoad", startPipeline.WorkerName);

        Assert.Empty(DomainActions(kernel.RegisterEvent(new RuntimeEvent.StartPipelineAcknowledged("CustomerLoad"))));
        Assert.Empty(DomainActions(kernel.RegisterEvent(new RuntimeEvent.PipelineStarted("CustomerLoad"))));

        var taskReady = kernel.RegisterEvent(new RuntimeEvent.TaskReady("CustomerLoad", "task:CustomerLoad:load", "load"));
        Assert.Empty(DomainActions(taskReady));
        Assert.Equal(0, taskReady.Snapshot.PendingCount);
        Assert.Equal(1, taskReady.Snapshot.ReadyCount);
        Assert.Equal(0, taskReady.Snapshot.RunningGrantCount);

        var grantResult = kernel.RegisterEvent(new RuntimeEvent.SchedulerTick(DateTimeOffset.UtcNow, MaxActiveWorkerProcesses: 1));
        var grantAction = Assert.IsType<RuntimeAction.IssueGrant>(Assert.Single(DomainActions(grantResult)));

        Assert.Equal("task:CustomerLoad:load", grantAction.TaskId);
        Assert.Equal(0, grantResult.Snapshot.PendingCount);
        Assert.Equal(0, grantResult.Snapshot.ReadyCount);
        Assert.Equal(1, grantResult.Snapshot.RunningGrantCount);
        Assert.Contains(grantResult.Snapshot.Tasks, static item =>
            item.TaskId == "task:CustomerLoad:load" &&
            item.State == TaskRuntimeState.GrantIssued);
    }

    [Fact]
    public void TaskSuccessRemovesRunningGrantReleasesLocksAndRecordsOutcome()
    {
        var (kernel, grant) = RunToStartedGrant(CreateDefinition(withLock: true));

        var success = kernel.RegisterEvent(new RuntimeEvent.TaskSucceeded(
            "CustomerLoad",
            grant.TaskId,
            grant.GrantId,
            grant.CommandId,
            grant.AttemptNumber,
            ExitCode: 0));

        Assert.Equal(0, success.Snapshot.RunningGrantCount);
        Assert.Equal(0, success.Snapshot.LockCount);
        Assert.Empty(success.Snapshot.Retries);
        Assert.Contains(success.Snapshot.Outcomes, static item =>
            item.TaskId == "task:CustomerLoad:load" &&
            item.Outcome == "Succeeded");
        Assert.Contains(success.Snapshot.Tasks, static item =>
            item.TaskId == "task:CustomerLoad:load" &&
            item.State == TaskRuntimeState.Succeeded);
    }

    [Fact]
    public void FailedTaskWithRetryProducesRetryScheduleAndNoActiveGrant()
    {
        var retryPolicy = new RuntimeRetryPolicy(MaxAttempts: 2, Delay: TimeSpan.FromSeconds(5));
        var (kernel, grant) = RunToStartedGrant(CreateDefinition(retryPolicy: retryPolicy, withLock: true));

        var failed = kernel.RegisterEvent(new RuntimeEvent.TaskFailed(
            "CustomerLoad",
            grant.TaskId,
            grant.GrantId,
            grant.CommandId,
            grant.AttemptNumber,
            ExitCode: 4,
            FailureClass: "WorkerReportedRetryable",
            Reason: "synthetic failure"));

        var actions = DomainActions(failed);
        Assert.Contains(actions, static item => item is RuntimeAction.RecordTaskCompletion);
        var retryAction = Assert.IsType<RuntimeAction.ScheduleRetry>(Assert.Single(actions.OfType<RuntimeAction.ScheduleRetry>()));
        Assert.Equal("task:CustomerLoad:load", retryAction.TaskId);
        Assert.Equal(2, retryAction.AttemptNumber);
        Assert.Equal(0, failed.Snapshot.RunningGrantCount);
        Assert.Equal(0, failed.Snapshot.LockCount);
        Assert.Equal(1, failed.Snapshot.RetryCount);
        Assert.Empty(failed.Snapshot.Outcomes);
        Assert.Contains(failed.Snapshot.Tasks, static item =>
            item.TaskId == "task:CustomerLoad:load" &&
            item.State == TaskRuntimeState.RetryScheduled);
    }

    [Fact]
    public void WorkerLossDoesNotLeaveDanglingRunningGrant()
    {
        var (kernel, _) = RunToStartedGrant(CreateDefinition(withLock: true));

        var closed = kernel.RegisterEvent(new RuntimeEvent.WorkerClosed("CustomerLoad", ExitCode: 9, Reason: "worker exited"));

        Assert.Equal(0, closed.Snapshot.RunningGrantCount);
        Assert.Equal(0, closed.Snapshot.LockCount);
        Assert.Contains(closed.Snapshot.Outcomes, static item =>
            item.TaskId == "task:CustomerLoad:load" &&
            item.Outcome == "Failed");
        Assert.Contains(closed.Snapshot.Workers, static item =>
            item.WorkerName == "CustomerLoad" &&
            item.State == WorkerRuntimeState.Closed);
    }

    [Fact]
    public void StoppedPipelineCannotIssueNewGrant()
    {
        var kernel = CreateKernel();
        RunToPipelineStarted(kernel);

        var stopped = kernel.RegisterEvent(new RuntimeEvent.PipelineStopRequested("CustomerLoad", "test stop"));
        Assert.IsType<RuntimeAction.SendStopPipeline>(Assert.Single(DomainActions(stopped)));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            kernel.RegisterEvent(new RuntimeEvent.TaskReady("CustomerLoad", "task:CustomerLoad:load", "load")));
        Assert.Contains("stopped", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void LockConflictDoesNotIssueSecondGrantUntilFirstGrantReleasesLocks()
    {
        var kernel = CreateKernel(CreateTwoLockedPipelineDefinition());
        RunToPipelineStarted(kernel, "CustomerA", "pipeline:CustomerA", MaxActiveWorkerProcesses: 2);
        RunToPipelineStarted(kernel, "CustomerB", "pipeline:CustomerB", MaxActiveWorkerProcesses: 2);
        kernel.RegisterEvent(new RuntimeEvent.TaskReady("CustomerA", "task:CustomerA:load", "load-a"));
        kernel.RegisterEvent(new RuntimeEvent.TaskReady("CustomerB", "task:CustomerB:load", "load-b"));

        var firstGrant = Assert.IsType<RuntimeAction.IssueGrant>(Assert.Single(DomainActions(
            kernel.RegisterEvent(new RuntimeEvent.SchedulerTick(DateTimeOffset.UtcNow, MaxActiveWorkerProcesses: 2)))));
        Assert.Equal("task:CustomerA:load", firstGrant.TaskId);

        var conflicted = kernel.RegisterEvent(new RuntimeEvent.SchedulerTick(DateTimeOffset.UtcNow, MaxActiveWorkerProcesses: 2));
        Assert.Empty(DomainActions(conflicted));
        Assert.Equal(1, conflicted.Snapshot.ReadyCount);
        Assert.Equal(1, conflicted.Snapshot.RunningGrantCount);
        Assert.Equal(1, conflicted.Snapshot.LockCount);

        kernel.RegisterEvent(new RuntimeEvent.GrantAccepted(
            "CustomerA",
            firstGrant.Grant.TaskId,
            firstGrant.Grant.GrantId,
            firstGrant.Grant.CommandId,
            firstGrant.Grant.AttemptNumber));
        kernel.RegisterEvent(new RuntimeEvent.TaskStarted(
            "CustomerA",
            firstGrant.Grant.TaskId,
            firstGrant.Grant.GrantId,
            firstGrant.Grant.CommandId,
            firstGrant.Grant.AttemptNumber));
        var completed = kernel.RegisterEvent(new RuntimeEvent.TaskSucceeded(
            "CustomerA",
            firstGrant.Grant.TaskId,
            firstGrant.Grant.GrantId,
            firstGrant.Grant.CommandId,
            firstGrant.Grant.AttemptNumber,
            ExitCode: 0));

        Assert.Equal(0, completed.Snapshot.LockCount);
        Assert.Equal(1, completed.Snapshot.ReadyCount);

        var secondGrant = Assert.IsType<RuntimeAction.IssueGrant>(Assert.Single(DomainActions(
            kernel.RegisterEvent(new RuntimeEvent.SchedulerTick(DateTimeOffset.UtcNow, MaxActiveWorkerProcesses: 2)))));
        Assert.Equal("task:CustomerB:load", secondGrant.TaskId);
    }

    [Fact]
    public void DependencyFailureBlocksDependentReadyTaskWithoutIssuingGrant()
    {
        var kernel = CreateKernel(CreateProducerConsumerDefinition());
        var producerGrant = RunTaskToStartedGrant(
            kernel,
            "Producer",
            "pipeline:Producer",
            "task:Producer:produce",
            "produce",
            MaxActiveWorkerProcesses: 2);
        kernel.RegisterEvent(new RuntimeEvent.TaskFailed(
            "Producer",
            producerGrant.TaskId,
            producerGrant.GrantId,
            producerGrant.CommandId,
            producerGrant.AttemptNumber,
            ExitCode: 4,
            FailureClass: WorkerFailureClasses.DeterministicModelError,
            Reason: "synthetic producer failure"));

        kernel.RegisterEvent(new RuntimeEvent.WorkerOnline("Consumer", "pipeline:Consumer", "test-version"));
        kernel.RegisterEvent(new RuntimeEvent.WorkerReady("Consumer"));
        kernel.RegisterEvent(new RuntimeEvent.StartPipelineAcknowledged("Consumer"));
        kernel.RegisterEvent(new RuntimeEvent.PipelineStarted("Consumer"));
        kernel.RegisterEvent(new RuntimeEvent.TaskReady("Consumer", "task:Consumer:consume", "consume"));

        var blocked = kernel.RegisterEvent(new RuntimeEvent.SchedulerTick(DateTimeOffset.UtcNow, MaxActiveWorkerProcesses: 2));
        var actions = DomainActions(blocked);

        Assert.Contains(actions, static item => item is RuntimeAction.RecordBlockedTasks);
        Assert.Contains(actions, static item => item is RuntimeAction.SendStopPipeline);
        Assert.DoesNotContain(actions, static item => item is RuntimeAction.IssueGrant);
        Assert.Equal(0, blocked.Snapshot.ReadyCount);
        Assert.Equal(0, blocked.Snapshot.RunningGrantCount);
        Assert.Contains(blocked.Snapshot.Outcomes, static item =>
            item.TaskId == "task:Consumer:consume" &&
            item.Outcome == OrchestrationExecutionContinuity.SkippedBlocked);
    }

    private static void AssertSingleReducerBoundary(Type reducerType)
    {
        var methods = reducerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Select(static item => item.Name)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(["Apply"], methods);
        Assert.DoesNotContain(methods, static item => item.StartsWith("Mark", StringComparison.Ordinal));
        Assert.DoesNotContain(methods, static item => item.Contains("GrantIssued", StringComparison.Ordinal));
        Assert.DoesNotContain(methods, static item => item.Contains("WorkerReplacement", StringComparison.Ordinal));
    }

    private static bool IsRawMutableCollection(Type type)
    {
        if (!type.IsGenericType)
        {
            return typeof(IDictionary).IsAssignableFrom(type) ||
                   typeof(IList).IsAssignableFrom(type);
        }

        var genericType = type.GetGenericTypeDefinition();
        return genericType == typeof(Dictionary<,>) ||
               genericType == typeof(HashSet<>) ||
               genericType == typeof(List<>);
    }

    private static MetaOrchestrationRuntimeKernel CreateKernel(RuntimeDefinition? definition = null) =>
        new(new RuntimeState(definition ?? CreateDefinition()));

    private static (MetaOrchestrationRuntimeKernel Kernel, RuntimeGrant Grant) RunToStartedGrant(RuntimeDefinition definition)
    {
        var kernel = CreateKernel(definition);
        RunToPipelineStarted(kernel);
        var taskReady = kernel.RegisterEvent(new RuntimeEvent.TaskReady("CustomerLoad", "task:CustomerLoad:load", "load"));
        Assert.Empty(DomainActions(taskReady));
        var issueGrant = Assert.IsType<RuntimeAction.IssueGrant>(Assert.Single(DomainActions(
            kernel.RegisterEvent(new RuntimeEvent.SchedulerTick(DateTimeOffset.UtcNow, MaxActiveWorkerProcesses: 1)))));
        kernel.RegisterEvent(new RuntimeEvent.GrantAccepted(
            "CustomerLoad",
            issueGrant.Grant.TaskId,
            issueGrant.Grant.GrantId,
            issueGrant.Grant.CommandId,
            issueGrant.Grant.AttemptNumber));
        kernel.RegisterEvent(new RuntimeEvent.TaskStarted(
            "CustomerLoad",
            issueGrant.Grant.TaskId,
            issueGrant.Grant.GrantId,
            issueGrant.Grant.CommandId,
            issueGrant.Grant.AttemptNumber));
        return (kernel, issueGrant.Grant);
    }

    private static void RunToPipelineStarted(MetaOrchestrationRuntimeKernel kernel)
    {
        kernel.RegisterEvent(new RuntimeEvent.SchedulerTick(DateTimeOffset.UtcNow, MaxActiveWorkerProcesses: 1));
        kernel.RegisterEvent(new RuntimeEvent.WorkerOnline("CustomerLoad", "pipeline:CustomerLoad", "test-version"));
        kernel.RegisterEvent(new RuntimeEvent.WorkerReady("CustomerLoad"));
        kernel.RegisterEvent(new RuntimeEvent.StartPipelineAcknowledged("CustomerLoad"));
        kernel.RegisterEvent(new RuntimeEvent.PipelineStarted("CustomerLoad"));
    }

    private static void RunToPipelineStarted(
        MetaOrchestrationRuntimeKernel kernel,
        string pipelineName,
        string pipelineId,
        int MaxActiveWorkerProcesses)
    {
        var start = Assert.IsType<RuntimeAction.StartWorker>(Assert.Single(DomainActions(
            kernel.RegisterEvent(new RuntimeEvent.SchedulerTick(DateTimeOffset.UtcNow, MaxActiveWorkerProcesses)))));
        Assert.Equal(pipelineName, start.WorkerName);
        Assert.Equal(pipelineId, start.PipelineId);
        kernel.RegisterEvent(new RuntimeEvent.WorkerOnline(pipelineName, pipelineId, "test-version"));
        kernel.RegisterEvent(new RuntimeEvent.WorkerReady(pipelineName));
        kernel.RegisterEvent(new RuntimeEvent.StartPipelineAcknowledged(pipelineName));
        kernel.RegisterEvent(new RuntimeEvent.PipelineStarted(pipelineName));
    }

    private static RuntimeGrant RunTaskToStartedGrant(
        MetaOrchestrationRuntimeKernel kernel,
        string pipelineName,
        string pipelineId,
        string taskId,
        string taskName,
        int MaxActiveWorkerProcesses)
    {
        RunToPipelineStarted(kernel, pipelineName, pipelineId, MaxActiveWorkerProcesses);
        kernel.RegisterEvent(new RuntimeEvent.TaskReady(pipelineName, taskId, taskName));
        var issueGrant = Assert.Single(DomainActions(
                kernel.RegisterEvent(new RuntimeEvent.SchedulerTick(DateTimeOffset.UtcNow, MaxActiveWorkerProcesses)))
            .OfType<RuntimeAction.IssueGrant>());
        kernel.RegisterEvent(new RuntimeEvent.GrantAccepted(
            pipelineName,
            issueGrant.Grant.TaskId,
            issueGrant.Grant.GrantId,
            issueGrant.Grant.CommandId,
            issueGrant.Grant.AttemptNumber));
        kernel.RegisterEvent(new RuntimeEvent.TaskStarted(
            pipelineName,
            issueGrant.Grant.TaskId,
            issueGrant.Grant.GrantId,
            issueGrant.Grant.CommandId,
            issueGrant.Grant.AttemptNumber));
        return issueGrant.Grant;
    }

    private static IReadOnlyList<RuntimeAction> DomainActions(KernelResult result) =>
        result.Actions
            .Where(static item => item is not RuntimeAction.PublishSnapshot)
            .ToArray();

    private static RuntimeDefinition CreateDefinition(
        RuntimeRetryPolicy? retryPolicy = null,
        bool withLock = false)
    {
        RuntimeLockRequest[] locks = withLock
            ? [new RuntimeLockRequest("dbo.Customer", "ExclusiveWrite")]
            : [];
        var task = new RuntimeTaskDefinition(
            "task:CustomerLoad:load",
            "load",
            "CustomerLoad",
            "pipeline:CustomerLoad",
            "planned:CustomerLoad:load",
            "profile:CustomerLoad:load",
            locks);
        var pipeline = new RuntimePipelineDefinition(
            "CustomerLoad",
            "pipeline:CustomerLoad",
            [task]);
        return new RuntimeDefinition([pipeline], retryPolicy ?? RuntimeRetryPolicy.NoRetry);
    }

    private static RuntimeDefinition CreateTwoLockedPipelineDefinition()
    {
        var taskA = new RuntimeTaskDefinition(
            "task:CustomerA:load",
            "load-a",
            "CustomerA",
            "pipeline:CustomerA",
            "planned:CustomerA:load",
            "profile:CustomerA:load",
            [new RuntimeLockRequest("dbo.Customer", "ExclusiveWrite")]);
        var taskB = new RuntimeTaskDefinition(
            "task:CustomerB:load",
            "load-b",
            "CustomerB",
            "pipeline:CustomerB",
            "planned:CustomerB:load",
            "profile:CustomerB:load",
            [new RuntimeLockRequest("dbo.Customer", "ExclusiveWrite")]);
        return new RuntimeDefinition(
            [
                new RuntimePipelineDefinition("CustomerA", "pipeline:CustomerA", [taskA]),
                new RuntimePipelineDefinition("CustomerB", "pipeline:CustomerB", [taskB])
            ],
            RuntimeRetryPolicy.NoRetry);
    }

    private static RuntimeDefinition CreateProducerConsumerDefinition()
    {
        var producer = new RuntimeTaskDefinition(
            "task:Producer:produce",
            "produce",
            "Producer",
            "pipeline:Producer",
            "planned:Producer:produce",
            "profile:Producer:produce",
            []);
        var consumer = new RuntimeTaskDefinition(
            "task:Consumer:consume",
            "consume",
            "Consumer",
            "pipeline:Consumer",
            "planned:Consumer:consume",
            "profile:Consumer:consume",
            []);
        return new RuntimeDefinition(
            [
                new RuntimePipelineDefinition("Producer", "pipeline:Producer", [producer]),
                new RuntimePipelineDefinition("Consumer", "pipeline:Consumer", [consumer])
            ],
            RuntimeRetryPolicy.NoRetry,
            [
                new RuntimeDependency(
                    consumer.TaskId,
                    consumer.TaskAccessProfileId,
                    producer.TaskAccessProfileId,
                    producer.PipelineName,
                    producer.TaskName,
                    OrchestrationExecutionContinuity.OnSuccess,
                    string.Empty,
                    string.Empty)
            ],
            []);
    }
}
