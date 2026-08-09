#nullable enable

namespace MetaMultiDimensional
{
    public sealed class PerspectiveNamedSet
    {
        public string Id { get; set; } = string.Empty;

        public NamedSet NamedSet { get; set; } = null!;

        public Perspective Perspective { get; set; } = null!;

    }
}
