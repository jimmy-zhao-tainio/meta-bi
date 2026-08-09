#nullable enable

namespace MetaTabular
{
    public sealed class TabularColumnTranslation
    {
        public string Id { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public string? Description { get; set; }

        public TabularColumn TabularColumn { get; set; } = null!;

        public TabularCulture TabularCulture { get; set; } = null!;

    }
}
