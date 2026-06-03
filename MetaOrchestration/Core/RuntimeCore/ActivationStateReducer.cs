using System.Globalization;

namespace MetaOrchestration.Core.Runtime;

internal enum PipelineActivationState
{
    Inactive,
    StartRequested,
    Active,
    CapacityDeferralRequested,
    Parked,
    Completed,
    Stopped
}

internal enum ActivationTrigger
{
    SchedulerTick,
    WorkerRegistered,
    CapacityDeferralRequested,
    CapacityDeferralCancelled,
    CapacityDeferredWorkerClosed,
    WorkerReplacementFromBeginning,
    WorkerReplacementAtResumeBoundary,
    PipelineCompleted,
    NoRemainingPipelineWork,
    PipelineStopped
}

internal enum ActivationTransitionEffect
{
    None,
    StartWorker
}

internal readonly record struct ActivationFacts(
    bool HasRemainingPipelineWork,
    bool HasWorkerCapacity,
    DateTimeOffset Now);

internal readonly record struct ActivationTransitionResult(
    PipelineActivationState State,
    ActivationTrigger Trigger,
    ActivationTransitionEffect Effect);

internal sealed class ActivationStateReducer
{
    private static readonly ActivationTransition[] Transitions =
    [
        new(
            PipelineActivationState.Inactive,
            ActivationTrigger.SchedulerTick,
            PipelineActivationState.StartRequested,
            ActivationTransitionEffect.StartWorker,
            RequiresRemainingWork: true,
            RequiresWorkerCapacity: true),
        new(
            PipelineActivationState.Parked,
            ActivationTrigger.SchedulerTick,
            PipelineActivationState.StartRequested,
            ActivationTransitionEffect.StartWorker,
            RequiresRemainingWork: true,
            RequiresWorkerCapacity: true),
        new(
            PipelineActivationState.StartRequested,
            ActivationTrigger.WorkerRegistered,
            PipelineActivationState.Active),
        new(
            PipelineActivationState.Active,
            ActivationTrigger.CapacityDeferralRequested,
            PipelineActivationState.CapacityDeferralRequested),
        new(
            PipelineActivationState.CapacityDeferralRequested,
            ActivationTrigger.CapacityDeferralCancelled,
            PipelineActivationState.Active),
        new(
            PipelineActivationState.CapacityDeferralRequested,
            ActivationTrigger.CapacityDeferredWorkerClosed,
            PipelineActivationState.Parked),
        new(
            PipelineActivationState.Active,
            ActivationTrigger.WorkerReplacementFromBeginning,
            PipelineActivationState.Inactive),
        new(
            PipelineActivationState.Active,
            ActivationTrigger.WorkerReplacementAtResumeBoundary,
            PipelineActivationState.Parked),
        new(
            PipelineActivationState.Active,
            ActivationTrigger.PipelineCompleted,
            PipelineActivationState.Completed),
        new(
            PipelineActivationState.Inactive,
            ActivationTrigger.NoRemainingPipelineWork,
            PipelineActivationState.Completed),
        new(
            PipelineActivationState.Parked,
            ActivationTrigger.NoRemainingPipelineWork,
            PipelineActivationState.Completed),
        new(
            PipelineActivationState.Inactive,
            ActivationTrigger.PipelineStopped,
            PipelineActivationState.Stopped),
        new(
            PipelineActivationState.StartRequested,
            ActivationTrigger.PipelineStopped,
            PipelineActivationState.Stopped),
        new(
            PipelineActivationState.Active,
            ActivationTrigger.PipelineStopped,
            PipelineActivationState.Stopped),
        new(
            PipelineActivationState.Parked,
            ActivationTrigger.PipelineStopped,
            PipelineActivationState.Stopped)
    ];

    public ActivationTransitionResult Apply(
        PipelineActivationState state,
        ActivationTrigger trigger,
        ActivationFacts facts)
    {
        var matches = Transitions
            .Where(item => item.From == state && item.Trigger == trigger)
            .Where(item => !item.RequiresRemainingWork || facts.HasRemainingPipelineWork)
            .Where(item => !item.RequiresWorkerCapacity || facts.HasWorkerCapacity)
            .ToArray();
        if (matches.Length == 1)
        {
            var match = matches[0];
            return new ActivationTransitionResult(match.To, trigger, match.Effect);
        }

        if (matches.Length > 1)
        {
            throw new InvalidOperationException(
                $"Ambiguous activation transition: {state} + {trigger} has {matches.Length.ToString(CultureInfo.InvariantCulture)} definitions.");
        }

        throw new InvalidOperationException($"Illegal activation transition: {state} + {trigger}.");
    }
}

internal readonly record struct ActivationTransition(
    PipelineActivationState From,
    ActivationTrigger Trigger,
    PipelineActivationState To,
    ActivationTransitionEffect Effect = ActivationTransitionEffect.None,
    bool RequiresRemainingWork = false,
    bool RequiresWorkerCapacity = false);
