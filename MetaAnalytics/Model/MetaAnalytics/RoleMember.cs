#nullable enable

namespace MetaAnalytics
{
    public sealed class RoleMember
    {
        public string Id { get; set; } = string.Empty;

        public string? MemberKind { get; set; }

        public string MemberName { get; set; } = string.Empty;

        public SecurityRole SecurityRole { get; set; } = null!;

    }
}
