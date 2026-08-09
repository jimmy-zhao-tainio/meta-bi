#nullable enable

namespace MetaOrchestration
{
    public sealed class ObjectAccess
    {
        public string Id { get; set; } = string.Empty;

        public string AccessKind { get; set; } = string.Empty;

        public string AccessRole { get; set; } = string.Empty;

        public string? OperationKind { get; set; }

        public string Ordinal { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public DataObject DataObject { get; set; } = null!;

        public TaskAccessProfile TaskAccessProfile { get; set; } = null!;

    }
}
