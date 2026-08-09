#nullable enable

namespace MetaTabular
{
    public sealed class TabularModel
    {
        public string Id { get; set; } = string.Empty;

        public string? Collation { get; set; }

        public string? CompatibilityLevel { get; set; }

        public string? DefaultCulture { get; set; }

        public string? DefaultDataView { get; set; }

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

    }
}
