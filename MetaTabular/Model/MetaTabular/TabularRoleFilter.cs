#nullable enable

namespace MetaTabular
{
    public sealed class TabularRoleFilter
    {
        public string Id { get; set; } = string.Empty;

        public string Expression { get; set; } = string.Empty;

        public TabularSecurityRole TabularSecurityRole { get; set; } = null!;

        public TabularTable TabularTable { get; set; } = null!;

    }
}
