#nullable enable

namespace MetaMultiDimensional
{
    public sealed class PerspectiveMeasure
    {
        public string Id { get; set; } = string.Empty;

        public Measure Measure { get; set; } = null!;

        public Perspective Perspective { get; set; } = null!;

    }
}
