#nullable enable

namespace MetaOrchestration
{
    public sealed class DependencyIssue
    {
        public string Id { get; set; } = string.Empty;

        public string BlocksAutomaticRunPlanning { get; set; } = string.Empty;

        public string BlocksDag { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string IssueDomain { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Severity { get; set; } = string.Empty;

        public DataObject? DataObject { get; set; }

        public OrchestrationPlan OrchestrationPlan { get; set; } = null!;

    }
}
