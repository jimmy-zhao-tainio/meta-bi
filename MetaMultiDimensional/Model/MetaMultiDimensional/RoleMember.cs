#nullable enable

namespace MetaMultiDimensional
{
    public sealed class RoleMember
    {
        public string Id { get; set; } = string.Empty;

        public string MemberName { get; set; } = string.Empty;

        public string? MemberSid { get; set; }

        public SecurityRole SecurityRole { get; set; } = null!;

    }
}
