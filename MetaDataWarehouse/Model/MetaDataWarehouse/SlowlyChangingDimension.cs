#nullable enable

namespace MetaDataWarehouse
{
    public sealed class SlowlyChangingDimension
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Name { get; set; }

        public Dimension Dimension { get; set; } = null!;

    }
}
