#nullable enable

namespace MetaTabular
{
    public sealed class TabularTablePermission
    {
        public string Id { get; set; } = string.Empty;

        public string MetadataPermission { get; set; } = string.Empty;

        public TabularSecurityRole TabularSecurityRole { get; set; } = null!;

        public TabularTable TabularTable { get; set; } = null!;

    }
}
