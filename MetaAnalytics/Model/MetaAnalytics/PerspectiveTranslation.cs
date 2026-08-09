#nullable enable

namespace MetaAnalytics
{
    public sealed class PerspectiveTranslation
    {
        public string Id { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public string? Description { get; set; }

        public Culture Culture { get; set; } = null!;

        public Perspective Perspective { get; set; } = null!;

    }
}
