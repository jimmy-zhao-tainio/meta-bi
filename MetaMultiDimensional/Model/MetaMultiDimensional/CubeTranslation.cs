#nullable enable

namespace MetaMultiDimensional
{
    public sealed class CubeTranslation
    {
        public string Id { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public string? Description { get; set; }

        public Cube Cube { get; set; } = null!;

        public Culture Culture { get; set; } = null!;

    }
}
