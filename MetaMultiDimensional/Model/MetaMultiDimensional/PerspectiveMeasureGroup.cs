#nullable enable

namespace MetaMultiDimensional
{
    public sealed class PerspectiveMeasureGroup
    {
        public string Id { get; set; } = string.Empty;

        public MeasureGroup MeasureGroup { get; set; } = null!;

        public Perspective Perspective { get; set; } = null!;

    }
}
