namespace MetaOrchestration.Core;

public enum OrchestrationObjectAccessKind
{
    Read,
    Write,
    ReadWrite,
    ResetWrite
}

public enum OrchestrationAccessDirection
{
    None,
    Read,
    Write,
    ReadWrite
}

public enum OrchestrationWriteEffect
{
    None,
    Append,
    Replace,
    ResetOnly,
    Mutation,
    KeyedUpsert,
    ConditionalKeyedUpsert,
    OperationalAppend,
    Unclassified
}

public enum OrchestrationAccessPurpose
{
    SourceInput,
    Lookup,
    TargetLoad,
    TargetMutation,
    InferredMemberRepair,
    DataQualityCheck,
    Audit,
    OperationalLog,
    Unclassified
}

public enum OrchestrationLockMode
{
    None,
    SharedRead,
    AppendWrite,
    KeyedUpsert,
    MutationWrite,
    ReplaceWrite,
    Exclusive
}

public enum OrchestrationIssueDomain
{
    Dependency,
    Determinism,
    Synchronization,
    ProfileResolution,
    Policy
}

public enum OrchestrationIssueCode
{
    MissingScriptOrBinding,
    NonExecutableTransformScript,
    UncoveredPipeline,
    DependencyCycle,
    WriteOrderAmbiguity,
    SharedAppendWritersRequirePolicy,
    SynchronizationRequired,
    SynchronizationPolicyMissing,
    UnsafeSharedReset,
    MultipleReplacementProducers,
    UnclassifiedWriteIntent,
    ContradictoryResolution
}

public sealed record OrchestrationAnalysisRequest(
    string PipelineWorkspacePath,
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    string PlanName,
    string? Description = null);

public sealed record PipelineDependencyProfile(
    string PipelineId,
    string PipelineName,
    IReadOnlyList<PipelineTaskAccessProfile> Tasks,
    IReadOnlyList<PipelineObjectAccessProfile> ObjectAccesses,
    IReadOnlyList<PipelineDependencyProfileIssue> Issues);

public sealed record PipelineTaskAccessProfile(
    string PipelineTaskId,
    string TaskName,
    int Ordinal,
    string TransformScriptId,
    string TransformScriptName,
    string TransformBindingId,
    string StatementKind,
    IReadOnlyList<PipelineObjectAccessProfile> ObjectAccesses);

public sealed record PipelineObjectAccessProfile(
    string SqlIdentifier,
    string ObjectKey,
    OrchestrationObjectAccessKind AccessKind,
    string AccessRole,
    int Ordinal,
    string? OperationKind,
    string Reason);

public sealed record TaskObjectEffectProfile(
    string PipelineTaskId,
    string TaskName,
    string PipelineId,
    string PipelineName,
    string SqlIdentifier,
    string ObjectKey,
    OrchestrationAccessDirection AccessDirection,
    OrchestrationWriteEffect WriteEffect,
    OrchestrationAccessPurpose AccessPurpose,
    bool CreatesDataDependency,
    bool IsPublishedProducer,
    bool RequiresSynchronization,
    OrchestrationLockMode LockMode,
    string Reason);

public sealed record TaskDependencyEdge(
    string PredecessorTaskId,
    string SuccessorTaskId,
    string PredecessorPipelineId,
    string SuccessorPipelineId,
    string ObjectKey,
    string DependencyKind,
    string Reason);

public sealed record PipelineDependencyProfileIssue(
    OrchestrationIssueCode Code,
    OrchestrationIssueDomain Domain,
    string Severity,
    bool BlocksDag,
    bool BlocksAutomaticRunPlanning,
    string Message,
    string? ObjectKey,
    IReadOnlyList<string> PipelineIds);

public sealed record PipelineDependencyEdge(
    string PredecessorPipelineId,
    string SuccessorPipelineId,
    string DependencyKind,
    string Reason);

public sealed record OrchestrationAnalysisResult(
    string PlanName,
    string? Description,
    IReadOnlyList<PipelineDependencyProfile> Pipelines,
    IReadOnlyList<TaskObjectEffectProfile> TaskObjectEffects,
    IReadOnlyList<TaskDependencyEdge> TaskDependencies,
    IReadOnlyList<PipelineDependencyEdge> Dependencies,
    IReadOnlyList<PipelineDependencyProfileIssue> Issues,
    string DagStatus,
    string DeterminismStatus,
    string SynchronizationStatus)
{
    public bool IsCompleteDag => string.Equals(DagStatus, "Complete", StringComparison.Ordinal);

    public bool BlocksAutomaticRunPlanning => Issues.Any(static item => item.BlocksAutomaticRunPlanning);
}
