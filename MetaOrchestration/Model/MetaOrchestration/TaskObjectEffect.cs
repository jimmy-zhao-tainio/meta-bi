#nullable enable

namespace MetaOrchestration
{
    public sealed class TaskObjectEffect
    {
        public string Id { get; set; } = string.Empty;

        public string AccessDirection { get; set; } = string.Empty;

        public string AccessPurpose { get; set; } = string.Empty;

        public string CreatesDataDependency { get; set; } = string.Empty;

        public string IsPublishedProducer { get; set; } = string.Empty;

        public string LockMode { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public string RequiresSynchronization { get; set; } = string.Empty;

        public string WriteEffect { get; set; } = string.Empty;

        public DataObject DataObject { get; set; } = null!;

        public TaskAccessProfile TaskAccessProfile { get; set; } = null!;

    }
}
