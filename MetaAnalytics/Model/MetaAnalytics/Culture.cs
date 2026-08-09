#nullable enable

namespace MetaAnalytics
{
    public sealed class Culture
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public AnalyticsModel AnalyticsModel { get; set; } = null!;

    }
}
