#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLink
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public BusinessHub ChildHub { get; set; } = null!;

        public BusinessHub ParentHub { get; set; } = null!;

    }
}
