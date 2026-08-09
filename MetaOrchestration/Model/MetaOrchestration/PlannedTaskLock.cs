#nullable enable

namespace MetaOrchestration
{
    public sealed class PlannedTaskLock
    {
        public string Id { get; set; } = string.Empty;

        public string LockMode { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public DataObject DataObject { get; set; } = null!;

        public LockCompatibilityPolicy? LockCompatibilityPolicy { get; set; }

        public PlannedTask PlannedTask { get; set; } = null!;

        public TaskObjectEffect TaskObjectEffect { get; set; } = null!;

    }
}
