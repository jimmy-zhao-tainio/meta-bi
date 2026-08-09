#nullable enable

namespace MetaDataWarehouse
{
    public sealed class Type2DimensionAttribute
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DimensionAttribute DimensionAttribute { get; set; } = null!;

        public SlowlyChangingDimension SlowlyChangingDimension { get; set; } = null!;

    }
}
