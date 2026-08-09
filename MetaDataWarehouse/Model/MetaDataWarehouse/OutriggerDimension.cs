#nullable enable

namespace MetaDataWarehouse
{
    public sealed class OutriggerDimension
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? IsRequired { get; set; }

        public string Ordinal { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public Dimension ChildDimension { get; set; } = null!;

        public Dimension ParentDimension { get; set; } = null!;

    }
}
