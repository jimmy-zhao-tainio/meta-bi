#nullable enable

namespace MetaMultiDimensional
{
    public sealed class Cube
    {
        public string Id { get; set; } = string.Empty;

        public string? DefaultMeasureName { get; set; }

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ProcessingMode { get; set; }

        public string? StorageMode { get; set; }

        public MultiDimensionalDatabase MultiDimensionalDatabase { get; set; } = null!;

    }
}
