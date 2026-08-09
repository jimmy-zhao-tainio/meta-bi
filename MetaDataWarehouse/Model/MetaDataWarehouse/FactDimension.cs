#nullable enable

namespace MetaDataWarehouse
{
    public sealed class FactDimension
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? IsRequired { get; set; }

        public string Ordinal { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public Dimension Dimension { get; set; } = null!;

        public Fact Fact { get; set; } = null!;

    }
}
