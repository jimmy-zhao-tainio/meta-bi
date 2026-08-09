#nullable enable

namespace MetaDataWarehouse
{
    public sealed class ConformedDimension
    {
        public string Id { get; set; } = string.Empty;

        public string ConformanceName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Dimension Dimension { get; set; } = null!;

    }
}
