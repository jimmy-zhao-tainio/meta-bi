#nullable enable

namespace MetaAnalytics
{
    public sealed class Hierarchy
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? DisplayFolder { get; set; }

        public string? IsHidden { get; set; }

        public string? Kind { get; set; }

        public string Name { get; set; } = string.Empty;

        public Table Table { get; set; } = null!;

    }
}
