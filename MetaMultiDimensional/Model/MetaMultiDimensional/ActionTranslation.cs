#nullable enable

namespace MetaMultiDimensional
{
    public sealed class ActionTranslation
    {
        public string Id { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public string? Description { get; set; }

        public CubeAction CubeAction { get; set; } = null!;

        public Culture Culture { get; set; } = null!;

    }
}
