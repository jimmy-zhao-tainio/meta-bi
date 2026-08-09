#nullable enable

namespace MetaMultiDimensional
{
    public sealed class PerspectiveAction
    {
        public string Id { get; set; } = string.Empty;

        public CubeAction CubeAction { get; set; } = null!;

        public Perspective Perspective { get; set; } = null!;

    }
}
