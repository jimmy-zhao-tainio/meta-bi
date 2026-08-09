#nullable enable

namespace MetaMultiDimensional
{
    public sealed class NamedSet
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? DisplayFolder { get; set; }

        public string Expression { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Cube Cube { get; set; } = null!;

    }
}
