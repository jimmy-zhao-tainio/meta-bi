#nullable enable

namespace MetaTabular
{
    public sealed class TabularColumnPermission
    {
        public string Id { get; set; } = string.Empty;

        public string MetadataPermission { get; set; } = string.Empty;

        public TabularColumn TabularColumn { get; set; } = null!;

        public TabularSecurityRole TabularSecurityRole { get; set; } = null!;

    }
}
