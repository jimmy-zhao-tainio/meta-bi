#nullable enable

namespace MetaAnalytics
{
    public sealed class AttributeTranslation
    {
        public string Id { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public string? Description { get; set; }

        public Attribute Attribute { get; set; } = null!;

        public Culture Culture { get; set; } = null!;

    }
}
