#nullable enable

namespace MetaAnalytics
{
    public sealed class SortByAttribute
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Attribute SortAttribute { get; set; } = null!;

        public Attribute SourceAttribute { get; set; } = null!;

    }
}
