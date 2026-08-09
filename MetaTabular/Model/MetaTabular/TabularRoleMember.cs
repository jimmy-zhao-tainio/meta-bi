#nullable enable

namespace MetaTabular
{
    public sealed class TabularRoleMember
    {
        public string Id { get; set; } = string.Empty;

        public string? MemberId { get; set; }

        public string MemberName { get; set; } = string.Empty;

        public TabularSecurityRole TabularSecurityRole { get; set; } = null!;

    }
}
