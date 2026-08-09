#nullable enable

namespace MetaMultiDimensional
{
    public sealed class MeasureGroup
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ProcessingMode { get; set; }

        public string? SourceName { get; set; }

        public string? StorageMode { get; set; }

        public Cube Cube { get; set; } = null!;

    }
}
