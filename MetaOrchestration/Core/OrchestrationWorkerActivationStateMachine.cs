using System.Globalization;
using MO = MetaOrchestration;

namespace MetaOrchestration.Core;

internal sealed class OrchestrationWorkerActivationStateMachine
{
    private static readonly StateTransition<OrchestrationWorkerActivationState, OrchestrationWorkerActivationTrigger>[] TransitionDefinitions =
    [
        new(OrchestrationWorkerActivationState.Inactive, OrchestrationWorkerActivationTrigger.StartRequested, OrchestrationWorkerActivationState.StartRequested),
        new(OrchestrationWorkerActivationState.Parked, OrchestrationWorkerActivationTrigger.StartRequested, OrchestrationWorkerActivationState.StartRequested),
        new(OrchestrationWorkerActivationState.StartRequested, OrchestrationWorkerActivationTrigger.WorkerRegistered, OrchestrationWorkerActivationState.Active),
        new(OrchestrationWorkerActivationState.Active, OrchestrationWorkerActivationTrigger.CapacityDeferralRequested, OrchestrationWorkerActivationState.CapacityDeferralRequested),
        new(OrchestrationWorkerActivationState.CapacityDeferralRequested, OrchestrationWorkerActivationTrigger.CapacityDeferralCancelled, OrchestrationWorkerActivationState.Active),
        new(OrchestrationWorkerActivationState.CapacityDeferralRequested, OrchestrationWorkerActivationTrigger.CapacityDeferredWorkerClosed, OrchestrationWorkerActivationState.Parked),
        new(OrchestrationWorkerActivationState.Active, OrchestrationWorkerActivationTrigger.WorkerReplacementFromBeginning, OrchestrationWorkerActivationState.Inactive),
        new(OrchestrationWorkerActivationState.Active, OrchestrationWorkerActivationTrigger.WorkerReplacementAtResumeBoundary, OrchestrationWorkerActivationState.Parked),
        new(OrchestrationWorkerActivationState.Inactive, OrchestrationWorkerActivationTrigger.NoRemainingPipelineWork, OrchestrationWorkerActivationState.Completed),
        new(OrchestrationWorkerActivationState.Parked, OrchestrationWorkerActivationTrigger.NoRemainingPipelineWork, OrchestrationWorkerActivationState.Completed),
        new(OrchestrationWorkerActivationState.Active, OrchestrationWorkerActivationTrigger.PipelineCompleted, OrchestrationWorkerActivationState.Completed)
    ];

    private readonly Dictionary<string, PipelineActivationState> pipelinesByName;

    static OrchestrationWorkerActivationStateMachine()
    {
        ValidateTransitionDefinitions(TransitionDefinitions);
    }

    public OrchestrationWorkerActivationStateMachine(
        IEnumerable<OrchestrationWorkerActivationPipelineDefinition> pipelineDefinitions)
    {
        ArgumentNullException.ThrowIfNull(pipelineDefinitions);

        pipelinesByName = pipelineDefinitions
            .Where(static item => !string.IsNullOrWhiteSpace(item.PipelineName))
            .GroupBy(static item => item.PipelineName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                static group => group.Key,
                static group => new PipelineActivationState(group.Single()),
                StringComparer.OrdinalIgnoreCase);
        if (pipelinesByName.Count == 0)
        {
            throw new ArgumentException("At least one pipeline activation definition is required.", nameof(pipelineDefinitions));
        }
    }

    public static IReadOnlyList<StateTransition<OrchestrationWorkerActivationState, OrchestrationWorkerActivationTrigger>> Transitions =>
        TransitionDefinitions;

    public IReadOnlyList<OrchestrationWorkerActivationSnapshot> GetSnapshots() =>
        pipelinesByName.Values
            .OrderBy(static item => item.Definition.PipelineName, StringComparer.OrdinalIgnoreCase)
            .Select(static item => new OrchestrationWorkerActivationSnapshot(
                item.Definition.PipelineName,
                item.Definition.PipelineId,
                item.State,
                item.ResumeTaskId,
                item.LastTaskId,
                item.LastTaskName))
            .ToArray();

    public OrchestrationRuntimeWorkerActivationDecision ApplySchedulerTick(
        OrchestrationWorkerActivationFacts facts)
    {
        ArgumentNullException.ThrowIfNull(facts.LiveWorkerNames);
        ArgumentNullException.ThrowIfNull(facts.ResolvePipelineCandidate);
        ArgumentNullException.ThrowIfNull(facts.SelectCapacityDeferralCandidate);

        var activeWorkerLimit = Math.Max(1, facts.MaxActiveWorkerProcesses);
        if (facts.LiveWorkerNames.Count < activeWorkerLimit)
        {
            var startCandidate = SelectPipelineActivationCandidate(facts);
            if (startCandidate is not null)
            {
                var pipeline = GetPipeline(startCandidate.Value.PipelineName);
                ApplyTransition(pipeline, OrchestrationWorkerActivationTrigger.StartRequested);
                pipeline.LastTaskId = startCandidate.Value.NextTaskId;
                pipeline.LastTaskName = startCandidate.Value.NextTaskName;
                return OrchestrationRuntimeWorkerActivationDecision.StartWorker(
                    pipeline.Definition.PipelineName,
                    pipeline.Definition.PipelineId,
                    pipeline.ResumeTaskId,
                    startCandidate.Value.NextTaskId,
                    startCandidate.Value.NextTaskName,
                    startCandidate.Value.Readiness);
            }
        }

        if (facts.LiveWorkerNames.Count >= activeWorkerLimit &&
            HasInactivePipelineThatCanProgress(facts))
        {
            var deferCandidate = facts.SelectCapacityDeferralCandidate();
            if (deferCandidate is not null)
            {
                var pipeline = GetPipeline(deferCandidate.Value.WorkerName);
                if (pipeline.State != OrchestrationWorkerActivationState.Active)
                {
                    return OrchestrationRuntimeWorkerActivationDecision.None;
                }

                ApplyTransition(pipeline, OrchestrationWorkerActivationTrigger.CapacityDeferralRequested);
                pipeline.LastTaskId = deferCandidate.Value.TaskId;
                pipeline.LastTaskName = deferCandidate.Value.TaskName;
                return OrchestrationRuntimeWorkerActivationDecision.DeferWorkerForCapacity(
                    deferCandidate.Value.WorkerName,
                    deferCandidate.Value.PipelineId,
                    deferCandidate.Value.PipelineName,
                    deferCandidate.Value.TaskId,
                    deferCandidate.Value.TaskName,
                    deferCandidate.Value.Reason);
            }
        }

        return OrchestrationRuntimeWorkerActivationDecision.None;
    }

    public void RegisterWorker(
        string workerName,
        string pipelineId,
        string resumeTaskId)
    {
        var pipeline = GetPipeline(workerName);
        var normalizedResumeTaskId = string.IsNullOrWhiteSpace(resumeTaskId) ? string.Empty : resumeTaskId.Trim();
        if (!string.IsNullOrWhiteSpace(pipelineId) &&
            !string.Equals(pipeline.Definition.PipelineId, pipelineId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{workerName}' registered pipeline id '{pipelineId}', but activation expected '{pipeline.Definition.PipelineId}'.");
        }

        if (pipeline.State is OrchestrationWorkerActivationState.Inactive or OrchestrationWorkerActivationState.Parked)
        {
            ApplyTransition(pipeline, OrchestrationWorkerActivationTrigger.StartRequested);
        }

        if (pipeline.State != OrchestrationWorkerActivationState.StartRequested)
        {
            throw new InvalidOperationException(
                $"Cannot register pipeline worker '{workerName}' while activation state is {pipeline.State}.");
        }

        if (!string.IsNullOrWhiteSpace(pipeline.ResumeTaskId) &&
            !string.Equals(pipeline.ResumeTaskId, normalizedResumeTaskId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{workerName}' registered resume task '{normalizedResumeTaskId}', but activation expected '{pipeline.ResumeTaskId}'.");
        }

        pipeline.ResumeTaskId = normalizedResumeTaskId;
        ApplyTransition(pipeline, OrchestrationWorkerActivationTrigger.WorkerRegistered);
    }

    public void CancelCapacityDeferralRequested(OrchestrationRuntimeWorkerActivationDecision decision)
    {
        if (decision.Kind != OrchestrationRuntimeWorkerActivationDecisionKind.DeferWorkerForCapacity)
        {
            throw new InvalidOperationException(
                $"Cannot cancel capacity deferral for activation decision {decision.Kind}.");
        }

        var pipeline = GetPipeline(decision.WorkerName);
        ApplyTransition(pipeline, OrchestrationWorkerActivationTrigger.CapacityDeferralCancelled);
    }

    public void CommitCapacityDeferralRequested(OrchestrationRuntimeWorkerActivationDecision decision)
    {
        if (decision.Kind != OrchestrationRuntimeWorkerActivationDecisionKind.DeferWorkerForCapacity)
        {
            throw new InvalidOperationException(
                $"Cannot commit capacity deferral for activation decision {decision.Kind}.");
        }

        var pipeline = GetPipeline(decision.WorkerName);
        if (pipeline.State != OrchestrationWorkerActivationState.CapacityDeferralRequested)
        {
            throw new InvalidOperationException(
                $"Cannot commit capacity deferral for pipeline worker '{decision.WorkerName}' while activation state is {pipeline.State}.");
        }
    }

    public bool IsCapacityDeferralPending(string workerName)
    {
        if (!pipelinesByName.TryGetValue(workerName, out var pipeline))
        {
            return false;
        }

        return pipeline.State == OrchestrationWorkerActivationState.CapacityDeferralRequested;
    }

    public bool TryApplyCapacityDeferredWorkerClosed(
        string workerName,
        string resumeTaskId,
        string taskName,
        out OrchestrationRuntimeCapacityDeferredWorkerClosed deferredWorker)
    {
        if (!pipelinesByName.TryGetValue(workerName, out var pipeline) ||
            pipeline.State != OrchestrationWorkerActivationState.CapacityDeferralRequested)
        {
            deferredWorker = default;
            return false;
        }

        pipeline.ResumeTaskId = RequireResumeTaskId(workerName, resumeTaskId);
        pipeline.LastTaskId = pipeline.ResumeTaskId;
        pipeline.LastTaskName = string.IsNullOrWhiteSpace(taskName) ? pipeline.LastTaskName : taskName;
        ApplyTransition(pipeline, OrchestrationWorkerActivationTrigger.CapacityDeferredWorkerClosed);
        deferredWorker = new OrchestrationRuntimeCapacityDeferredWorkerClosed(
            workerName,
            pipeline.ResumeTaskId,
            pipeline.LastTaskName);
        return true;
    }

    public void ApplyWorkerReplacementFromBeginning(string workerName)
    {
        var pipeline = GetPipeline(workerName);
        pipeline.ResumeTaskId = string.Empty;
        pipeline.LastTaskId = string.Empty;
        pipeline.LastTaskName = string.Empty;
        ApplyTransition(pipeline, OrchestrationWorkerActivationTrigger.WorkerReplacementFromBeginning);
    }

    public void ApplyWorkerReplacementAtResumeBoundary(
        string workerName,
        string resumeTaskId,
        string taskName)
    {
        var pipeline = GetPipeline(workerName);
        pipeline.ResumeTaskId = RequireResumeTaskId(workerName, resumeTaskId);
        pipeline.LastTaskId = pipeline.ResumeTaskId;
        pipeline.LastTaskName = string.IsNullOrWhiteSpace(taskName) ? pipeline.LastTaskName : taskName;
        ApplyTransition(pipeline, OrchestrationWorkerActivationTrigger.WorkerReplacementAtResumeBoundary);
    }

    public void MarkPipelineCompleted(string pipelineName)
    {
        var pipeline = GetPipeline(pipelineName);
        if (pipeline.State == OrchestrationWorkerActivationState.Completed)
        {
            return;
        }

        if (pipeline.State == OrchestrationWorkerActivationState.Active)
        {
            ApplyTransition(pipeline, OrchestrationWorkerActivationTrigger.PipelineCompleted);
            return;
        }

        if (pipeline.State is OrchestrationWorkerActivationState.Inactive or OrchestrationWorkerActivationState.Parked)
        {
            ApplyTransition(pipeline, OrchestrationWorkerActivationTrigger.NoRemainingPipelineWork);
            return;
        }

        throw new InvalidOperationException(
            $"Cannot complete pipeline '{pipelineName}' while activation state is {pipeline.State}.");
    }

    public bool HasInactivePipelineThatCanProgress(OrchestrationWorkerActivationFacts facts)
    {
        foreach (var pipeline in SelectStartablePipelines())
        {
            var candidate = facts.ResolvePipelineCandidate(pipeline.Definition, facts.Now);
            if (candidate is null)
            {
                ApplyTransition(pipeline, OrchestrationWorkerActivationTrigger.NoRemainingPipelineWork);
                continue;
            }

            if (candidate.Value.Readiness is OrchestrationTaskReadiness.Ready or OrchestrationTaskReadiness.Skip)
            {
                return true;
            }
        }

        return false;
    }

    public void AssertInvariants(Action<string> fail)
    {
        ArgumentNullException.ThrowIfNull(fail);
        foreach (var pipeline in pipelinesByName.Values)
        {
            if (pipeline.State == OrchestrationWorkerActivationState.Parked &&
                string.IsNullOrWhiteSpace(pipeline.ResumeTaskId))
            {
                fail($"pipeline '{pipeline.Definition.PipelineName}' is parked without a resume task id.");
            }

            if (pipeline.State == OrchestrationWorkerActivationState.CapacityDeferralRequested &&
                string.IsNullOrWhiteSpace(pipeline.LastTaskId))
            {
                fail($"pipeline '{pipeline.Definition.PipelineName}' has capacity deferral requested without a task boundary.");
            }
        }
    }

    private OrchestrationRuntimePipelineActivationCandidate? SelectPipelineActivationCandidate(
        OrchestrationWorkerActivationFacts facts)
    {
        OrchestrationRuntimePipelineActivationCandidate? skippedCandidate = null;
        OrchestrationRuntimePipelineActivationCandidate? waitingCandidate = null;
        foreach (var pipeline in SelectStartablePipelines())
        {
            var candidate = facts.ResolvePipelineCandidate(pipeline.Definition, facts.Now);
            if (candidate is null)
            {
                ApplyTransition(pipeline, OrchestrationWorkerActivationTrigger.NoRemainingPipelineWork);
                continue;
            }

            if (candidate.Value.Readiness == OrchestrationTaskReadiness.Ready)
            {
                return candidate;
            }

            if (candidate.Value.Readiness == OrchestrationTaskReadiness.Skip && skippedCandidate is null)
            {
                skippedCandidate = candidate;
            }

            if (candidate.Value.Readiness == OrchestrationTaskReadiness.Waiting && waitingCandidate is null)
            {
                waitingCandidate = candidate;
            }
        }

        return skippedCandidate ?? waitingCandidate;
    }

    private IEnumerable<PipelineActivationState> SelectStartablePipelines() =>
        pipelinesByName.Values
            .Where(static item => item.State is OrchestrationWorkerActivationState.Inactive or OrchestrationWorkerActivationState.Parked)
            .OrderBy(static item => item.Definition.PipelineName, StringComparer.OrdinalIgnoreCase);

    private PipelineActivationState GetPipeline(string pipelineName)
    {
        if (string.IsNullOrWhiteSpace(pipelineName))
        {
            throw new InvalidOperationException("Pipeline name is required for activation state.");
        }

        if (!pipelinesByName.TryGetValue(pipelineName, out var pipeline))
        {
            throw new InvalidOperationException($"Pipeline '{pipelineName}' is not known to the activation state machine.");
        }

        return pipeline;
    }

    private static void ApplyTransition(
        PipelineActivationState pipeline,
        OrchestrationWorkerActivationTrigger trigger)
    {
        var match = TransitionDefinitions
            .Where(item => item.From == pipeline.State && item.Trigger == trigger)
            .ToArray();
        if (match.Length == 1)
        {
            pipeline.State = match[0].To;
            if (pipeline.State is OrchestrationWorkerActivationState.Inactive
                or OrchestrationWorkerActivationState.Active
                or OrchestrationWorkerActivationState.Completed)
            {
                pipeline.ResumeTaskId = string.Empty;
            }

            return;
        }

        if (match.Length > 1)
        {
            throw new InvalidOperationException(
                $"Ambiguous worker activation transition for pipeline '{pipeline.Definition.PipelineName}': {pipeline.State} + {trigger} has {match.Length.ToString(CultureInfo.InvariantCulture)} definitions.");
        }

        throw new InvalidOperationException(
            $"Illegal worker activation transition for pipeline '{pipeline.Definition.PipelineName}': {pipeline.State} + {trigger}.");
    }

    private static void ValidateTransitionDefinitions(
        IReadOnlyList<StateTransition<OrchestrationWorkerActivationState, OrchestrationWorkerActivationTrigger>> transitions)
    {
        var duplicate = transitions
            .GroupBy(static item => (item.From, item.Trigger))
            .FirstOrDefault(static group => group.Count() > 1);
        if (duplicate is not null)
        {
            throw new InvalidOperationException(
                $"Duplicate worker activation transition definition for {duplicate.Key.From} + {duplicate.Key.Trigger}.");
        }
    }

    private static string RequireResumeTaskId(string workerName, string resumeTaskId)
    {
        if (string.IsNullOrWhiteSpace(resumeTaskId))
        {
            throw new InvalidOperationException(
                $"Pipeline worker '{workerName}' requires a resume task id for this activation transition.");
        }

        return resumeTaskId.Trim();
    }

    private sealed class PipelineActivationState
    {
        public PipelineActivationState(OrchestrationWorkerActivationPipelineDefinition definition)
        {
            Definition = definition;
        }

        public OrchestrationWorkerActivationPipelineDefinition Definition { get; }

        public OrchestrationWorkerActivationState State { get; set; } = OrchestrationWorkerActivationState.Inactive;

        public string ResumeTaskId { get; set; } = string.Empty;

        public string LastTaskId { get; set; } = string.Empty;

        public string LastTaskName { get; set; } = string.Empty;
    }
}

internal readonly record struct OrchestrationWorkerActivationPipelineDefinition(
    string PipelineName,
    string PipelineId,
    IReadOnlyList<MO.PlannedTask> PlannedTasks);

internal readonly record struct OrchestrationWorkerActivationFacts(
    IReadOnlySet<string> LiveWorkerNames,
    int MaxActiveWorkerProcesses,
    DateTimeOffset Now,
    Func<OrchestrationWorkerActivationPipelineDefinition, DateTimeOffset, OrchestrationRuntimePipelineActivationCandidate?> ResolvePipelineCandidate,
    Func<OrchestrationRuntimeWorkerCapacityDeferralCandidate?> SelectCapacityDeferralCandidate);

internal readonly record struct OrchestrationWorkerActivationSnapshot(
    string PipelineName,
    string PipelineId,
    OrchestrationWorkerActivationState State,
    string ResumeTaskId,
    string LastTaskId,
    string LastTaskName);

internal enum OrchestrationWorkerActivationState
{
    Inactive,
    StartRequested,
    Active,
    CapacityDeferralRequested,
    Parked,
    Completed
}

internal enum OrchestrationWorkerActivationTrigger
{
    StartRequested,
    WorkerRegistered,
    CapacityDeferralRequested,
    CapacityDeferralCancelled,
    CapacityDeferredWorkerClosed,
    WorkerReplacementFromBeginning,
    WorkerReplacementAtResumeBoundary,
    NoRemainingPipelineWork,
    PipelineCompleted
}
