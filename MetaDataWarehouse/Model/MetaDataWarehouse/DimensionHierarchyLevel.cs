#nullable enable

namespace MetaDataWarehouse
{
    public sealed class DimensionHierarchyLevel
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public DimensionAttribute DimensionAttribute { get; set; } = null!;

        public DimensionHierarchy DimensionHierarchy { get; set; } = null!;

    }
}
