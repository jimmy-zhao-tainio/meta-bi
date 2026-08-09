#nullable enable

using System.Collections.Generic;

namespace MetaOrchestration
{
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
}
