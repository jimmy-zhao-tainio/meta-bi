#nullable enable

namespace MetaMultiDimensional
{
    public sealed class CubeDimension
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? RoleName { get; set; }

        public Cube Cube { get; set; } = null!;

        public Dimension Dimension { get; set; } = null!;

    }
}
