#nullable enable
using System;
using System.Collections.Generic;

namespace MetaOrchestration;
public sealed partial class DataObject
{
    public string Id { get; set; } = null !;
    public string NormalizedKey { get; set; } = null !;
    public string SqlIdentifier { get; set; } = null !;
    public OrchestrationPlan OrchestrationPlan { get; set; } = null !;
}

public sealed partial class DependencyIssue
{
    public string Id { get; set; } = null !;
    public string BlocksAutomaticRunPlanning { get; set; } = null !;
    public string BlocksDag { get; set; } = null !;
    public string Code { get; set; } = null !;
    public string IssueDomain { get; set; } = null !;
    public string Message { get; set; } = null !;
    public string Severity { get; set; } = null !;
    public DataObject? DataObject { get; set; }
    public OrchestrationPlan OrchestrationPlan { get; set; } = null !;
}

public sealed partial class DependencyIssuePipeline
{
    public string Id { get; set; } = null !;
    public string Role { get; set; } = null !;
    public DependencyIssue DependencyIssue { get; set; } = null !;
    public PipelineReference PipelineReference { get; set; } = null !;
}

public sealed partial class LockCompatibilityPolicy
{
    public string Id { get; set; } = null !;
    public string LeftEffect { get; set; } = null !;
    public string LockBehavior { get; set; } = null !;
    public string PolicyKind { get; set; } = null !;
    public string? Reason { get; set; }
    public string RightEffect { get; set; } = null !;
    public string Status { get; set; } = null !;
    public DataObject DataObject { get; set; } = null !;
    public DependencyIssue? DependencyIssue { get; set; }
    public OrchestrationPlan OrchestrationPlan { get; set; } = null !;
}

public sealed partial class ObjectAccess
{
    public string Id { get; set; } = null !;
    public string AccessKind { get; set; } = null !;
    public string AccessRole { get; set; } = null !;
    public string? OperationKind { get; set; }
    public string Ordinal { get; set; } = null !;
    public string? Reason { get; set; }
    public DataObject DataObject { get; set; } = null !;
    public TaskAccessProfile TaskAccessProfile { get; set; } = null !;
}

public sealed partial class OrchestrationPlan
{
    public string Id { get; set; } = null !;
    public string DagStatus { get; set; } = null !;
    public string? Description { get; set; }
    public string DeterminismStatus { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string SynchronizationStatus { get; set; } = null !;
}

public sealed partial class PipelineDependency
{
    public string Id { get; set; } = null !;
    public string DependencyKind { get; set; } = null !;
    public string? Reason { get; set; }
    public OrchestrationPlan OrchestrationPlan { get; set; } = null !;
    public PipelineReference Predecessor { get; set; } = null !;
    public PipelineReference Successor { get; set; } = null !;
}

public sealed partial class PipelineObjectAccess
{
    public string Id { get; set; } = null !;
    public string AccessKind { get; set; } = null !;
    public string? Reason { get; set; }
    public DataObject DataObject { get; set; } = null !;
    public PipelineReference PipelineReference { get; set; } = null !;
}

public sealed partial class PipelineReference
{
    public string Id { get; set; } = null !;
    public string MetaPipelinePipelineId { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string? PipelineWorkspacePath { get; set; }
    public OrchestrationPlan OrchestrationPlan { get; set; } = null !;
}

public sealed partial class PlannedTask
{
    public string Id { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public string? Reason { get; set; }
    public PipelineReference PipelineReference { get; set; } = null !;
    public RunPlan RunPlan { get; set; } = null !;
    public TaskAccessProfile TaskAccessProfile { get; set; } = null !;
}

public sealed partial class PlannedTaskLock
{
    public string Id { get; set; } = null !;
    public string LockMode { get; set; } = null !;
    public string? Reason { get; set; }
    public DataObject DataObject { get; set; } = null !;
    public LockCompatibilityPolicy? LockCompatibilityPolicy { get; set; }
    public PlannedTask PlannedTask { get; set; } = null !;
    public TaskObjectEffect TaskObjectEffect { get; set; } = null !;
}

public sealed partial class RetryPolicy
{
    public string Id { get; set; } = null !;
    public string BackoffMultiplier { get; set; } = null !;
    public string InitialDelayMilliseconds { get; set; } = null !;
    public string MaxAttempts { get; set; } = null !;
    public string MaxDelayMilliseconds { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string PolicyKind { get; set; } = null !;
    public string? Reason { get; set; }
    public string RetryReadOnlyTasksByDefault { get; set; } = null !;
    public string RetryWriteTasksByDefault { get; set; } = null !;
    public string Status { get; set; } = null !;
    public OrchestrationPlan OrchestrationPlan { get; set; } = null !;
}

public sealed partial class RetryPolicyFailureClass
{
    public string Id { get; set; } = null !;
    public string FailureClass { get; set; } = null !;
    public string? Reason { get; set; }
    public string RetryBehavior { get; set; } = null !;
    public RetryPolicy RetryPolicy { get; set; } = null !;
}

public sealed partial class RunPlan
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string? Reason { get; set; }
    public string RunPlanStatus { get; set; } = null !;
    public OrchestrationPlan OrchestrationPlan { get; set; } = null !;
}

public sealed partial class RunPlanRetryPolicy
{
    public string Id { get; set; } = null !;
    public string PolicyRole { get; set; } = null !;
    public string? Reason { get; set; }
    public RetryPolicy RetryPolicy { get; set; } = null !;
    public RunPlan RunPlan { get; set; } = null !;
}

public sealed partial class TaskAccessProfile
{
    public string Id { get; set; } = null !;
    public string? BindingWorkspacePath { get; set; }
    public string MetaPipelinePipelineTaskId { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public string StatementKind { get; set; } = null !;
    public string TaskKind { get; set; } = null !;
    public string TaskName { get; set; } = null !;
    public string? TransformBindingId { get; set; }
    public string? TransformScriptId { get; set; }
    public string? TransformScriptName { get; set; }
    public string? TransformWorkspacePath { get; set; }
    public PipelineReference PipelineReference { get; set; } = null !;
}

public sealed partial class TaskDependency
{
    public string Id { get; set; } = null !;
    public string DependencyCondition { get; set; } = null !;
    public string DependencyKind { get; set; } = null !;
    public string? Reason { get; set; }
    public DataObject? DataObject { get; set; }
    public OrchestrationPlan OrchestrationPlan { get; set; } = null !;
    public TaskAccessProfile Predecessor { get; set; } = null !;
    public TaskAccessProfile Successor { get; set; } = null !;
}

public sealed partial class TaskObjectEffect
{
    public string Id { get; set; } = null !;
    public string AccessDirection { get; set; } = null !;
    public string AccessPurpose { get; set; } = null !;
    public string CreatesDataDependency { get; set; } = null !;
    public string IsPublishedProducer { get; set; } = null !;
    public string LockMode { get; set; } = null !;
    public string? Reason { get; set; }
    public string RequiresSynchronization { get; set; } = null !;
    public string WriteEffect { get; set; } = null !;
    public DataObject DataObject { get; set; } = null !;
    public TaskAccessProfile TaskAccessProfile { get; set; } = null !;
}

public sealed partial class TaskOrderingResolution
{
    public string Id { get; set; } = null !;
    public string DependencyCondition { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string? Reason { get; set; }
    public string ResolutionKind { get; set; } = null !;
    public string Status { get; set; } = null !;
    public DataObject? DataObject { get; set; }
    public DependencyIssue? DependencyIssue { get; set; }
    public OrchestrationPlan OrchestrationPlan { get; set; } = null !;
    public TaskAccessProfile Predecessor { get; set; } = null !;
    public TaskAccessProfile Successor { get; set; } = null !;
}

public sealed partial class MetaOrchestrationModel
{
    public static MetaOrchestrationModel CreateEmpty() => new();
    public List<DataObject> DataObjectList { get; set; } = new();
    public List<DependencyIssue> DependencyIssueList { get; set; } = new();
    public List<DependencyIssuePipeline> DependencyIssuePipelineList { get; set; } = new();
    public List<LockCompatibilityPolicy> LockCompatibilityPolicyList { get; set; } = new();
    public List<ObjectAccess> ObjectAccessList { get; set; } = new();
    public List<OrchestrationPlan> OrchestrationPlanList { get; set; } = new();
    public List<PipelineDependency> PipelineDependencyList { get; set; } = new();
    public List<PipelineObjectAccess> PipelineObjectAccessList { get; set; } = new();
    public List<PipelineReference> PipelineReferenceList { get; set; } = new();
    public List<PlannedTask> PlannedTaskList { get; set; } = new();
    public List<PlannedTaskLock> PlannedTaskLockList { get; set; } = new();
    public List<RetryPolicy> RetryPolicyList { get; set; } = new();
    public List<RetryPolicyFailureClass> RetryPolicyFailureClassList { get; set; } = new();
    public List<RunPlan> RunPlanList { get; set; } = new();
    public List<RunPlanRetryPolicy> RunPlanRetryPolicyList { get; set; } = new();
    public List<TaskAccessProfile> TaskAccessProfileList { get; set; } = new();
    public List<TaskDependency> TaskDependencyList { get; set; } = new();
    public List<TaskObjectEffect> TaskObjectEffectList { get; set; } = new();
    public List<TaskOrderingResolution> TaskOrderingResolutionList { get; set; } = new();
}

public static partial class MetaOrchestrationInstance
{
    private static readonly MetaOrchestrationModel _builtIn = CreateBuiltIn();
    public static MetaOrchestrationModel BuiltIn => _builtIn;

    public static MetaOrchestrationModel CreateBuiltIn()
    {
        var model = MetaOrchestrationModel.CreateEmpty();
        return model;
    }
}