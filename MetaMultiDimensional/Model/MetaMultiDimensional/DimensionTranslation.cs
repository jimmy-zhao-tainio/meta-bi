#nullable enable

namespace MetaMultiDimensional
{
    public sealed class DimensionTranslation
    {
        public string Id { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public string? Description { get; set; }

        public Culture Culture { get; set; } = null!;

        public Dimension Dimension { get; set; } = null!;

    }
}
