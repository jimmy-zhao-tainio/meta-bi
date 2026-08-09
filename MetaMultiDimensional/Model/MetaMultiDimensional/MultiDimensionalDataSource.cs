#nullable enable

namespace MetaMultiDimensional
{
    public sealed class MultiDimensionalDataSource
    {
        public string Id { get; set; } = string.Empty;

        public string? ConnectionReference { get; set; }

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Provider { get; set; }

        public string? SourceKind { get; set; }

        public MultiDimensionalDatabase MultiDimensionalDatabase { get; set; } = null!;

    }
}
