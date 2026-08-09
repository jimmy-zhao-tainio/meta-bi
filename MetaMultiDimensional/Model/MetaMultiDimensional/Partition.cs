#nullable enable

namespace MetaMultiDimensional
{
    public sealed class Partition
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public string? ProcessingMode { get; set; }

        public string? SliceExpression { get; set; }

        public string? SourceExpression { get; set; }

        public string? StorageMode { get; set; }

        public MeasureGroup MeasureGroup { get; set; } = null!;

        public MultiDimensionalDataSource? MultiDimensionalDataSource { get; set; }

    }
}
