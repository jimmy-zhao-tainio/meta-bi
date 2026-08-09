#nullable enable

namespace MetaTabular
{
    public sealed class TabularPerspectiveTranslation
    {
        public string Id { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public string? Description { get; set; }

        public TabularCulture TabularCulture { get; set; } = null!;

        public TabularPerspective TabularPerspective { get; set; } = null!;

    }
}
