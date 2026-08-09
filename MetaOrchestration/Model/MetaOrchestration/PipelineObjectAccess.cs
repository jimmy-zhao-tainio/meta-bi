#nullable enable

namespace MetaOrchestration
{
    public sealed class PipelineObjectAccess
    {
        public string Id { get; set; } = string.Empty;

        public string AccessKind { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public DataObject DataObject { get; set; } = null!;

        public PipelineReference PipelineReference { get; set; } = null!;

    }
}
