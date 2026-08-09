#nullable enable

namespace MetaAnalytics
{
    public sealed class AnalyticsModel
    {
        public string Id { get; set; } = string.Empty;

        public string? DefaultCulture { get; set; }

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

    }
}
