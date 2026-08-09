#nullable enable

namespace MetaMultiDimensional
{
    public sealed class Measure
    {
        public string Id { get; set; } = string.Empty;

        public string? AggregateFunction { get; set; }

        public string? DataTypeId { get; set; }

        public string? Description { get; set; }

        public string? DisplayFolder { get; set; }

        public string? FormatString { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? SourceName { get; set; }

        public MeasureGroup MeasureGroup { get; set; } = null!;

    }
}
