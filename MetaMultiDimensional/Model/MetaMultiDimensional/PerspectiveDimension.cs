#nullable enable

namespace MetaMultiDimensional
{
    public sealed class PerspectiveDimension
    {
        public string Id { get; set; } = string.Empty;

        public CubeDimension CubeDimension { get; set; } = null!;

        public Perspective Perspective { get; set; } = null!;

    }
}
