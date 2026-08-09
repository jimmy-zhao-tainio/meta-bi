#nullable enable

namespace MetaMultiDimensional
{
    public sealed class Dimension
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? DimensionType { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ProcessingGroup { get; set; }

        public string? ProcessingMode { get; set; }

        public string? SourceName { get; set; }

        public string? StorageMode { get; set; }

        public MultiDimensionalDatabase MultiDimensionalDatabase { get; set; } = null!;

    }
}
