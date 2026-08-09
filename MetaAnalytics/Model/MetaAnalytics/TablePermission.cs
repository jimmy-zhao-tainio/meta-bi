#nullable enable

namespace MetaAnalytics
{
    public sealed class TablePermission
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string MetadataPermission { get; set; } = string.Empty;

        public SecurityRole SecurityRole { get; set; } = null!;

        public Table Table { get; set; } = null!;

    }
}
