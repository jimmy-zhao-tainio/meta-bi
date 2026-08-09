#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLink
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public BusinessHub EquivalentHub { get; set; } = null!;

        public BusinessHub PrimaryHub { get; set; } = null!;

    }
}
