#nullable enable

namespace MetaMultiDimensional
{
    public sealed class Perspective
    {
        public string Id { get; set; } = string.Empty;

        public string? DefaultMeasureName { get; set; }

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public Cube Cube { get; set; } = null!;

    }
}
