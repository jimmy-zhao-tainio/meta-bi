#nullable enable

namespace MetaTabular
{
    public sealed class TabularTableTranslation
    {
        public string Id { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public string? Description { get; set; }

        public TabularCulture TabularCulture { get; set; } = null!;

        public TabularTable TabularTable { get; set; } = null!;

    }
}
