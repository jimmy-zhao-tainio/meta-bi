#nullable enable

namespace MetaDataWarehouse
{
    public sealed class DimensionHierarchy
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public Dimension Dimension { get; set; } = null!;

    }
}
