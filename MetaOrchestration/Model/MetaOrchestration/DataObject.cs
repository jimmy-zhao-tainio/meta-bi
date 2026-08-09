#nullable enable

namespace MetaOrchestration
{
    public sealed class DataObject
    {
        public string Id { get; set; } = string.Empty;

        public string NormalizedKey { get; set; } = string.Empty;

        public string SqlIdentifier { get; set; } = string.Empty;

        public OrchestrationPlan OrchestrationPlan { get; set; } = null!;

    }
}
