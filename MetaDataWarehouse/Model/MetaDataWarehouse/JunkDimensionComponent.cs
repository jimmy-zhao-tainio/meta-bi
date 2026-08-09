#nullable enable

namespace MetaDataWarehouse
{
    public sealed class JunkDimensionComponent
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Ordinal { get; set; } = string.Empty;

        public DimensionAttribute DimensionAttribute { get; set; } = null!;

        public JunkDimension JunkDimension { get; set; } = null!;

    }
}
