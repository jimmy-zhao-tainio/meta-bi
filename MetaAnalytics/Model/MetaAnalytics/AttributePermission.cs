#nullable enable

namespace MetaAnalytics
{
    public sealed class AttributePermission
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string MetadataPermission { get; set; } = string.Empty;

        public Attribute Attribute { get; set; } = null!;

        public SecurityRole SecurityRole { get; set; } = null!;

    }
}
