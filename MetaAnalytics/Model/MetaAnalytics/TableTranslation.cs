#nullable enable

namespace MetaAnalytics
{
    public sealed class TableTranslation
    {
        public string Id { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public string? Description { get; set; }

        public Culture Culture { get; set; } = null!;

        public Table Table { get; set; } = null!;

    }
}
