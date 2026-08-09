#nullable enable

namespace MetaDataWarehouse
{
    public sealed class DimensionBusinessKeyPart
    {
        public string Id { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public DimensionAttribute DimensionAttribute { get; set; } = null!;

        public DimensionBusinessKey DimensionBusinessKey { get; set; } = null!;

    }
}
