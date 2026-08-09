#nullable enable

namespace MetaAnalytics
{
    public sealed class Table
    {
        public string Id { get; set; } = string.Empty;

        public string? DataCategory { get; set; }

        public string? Description { get; set; }

        public string? DisplayFolder { get; set; }

        public string? IsHidden { get; set; }

        public string Kind { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public AnalyticsModel AnalyticsModel { get; set; } = null!;

    }
}
