#nullable enable

namespace MetaDataWarehouse
{
    public sealed class BridgeWeight
    {
        public string Id { get; set; } = string.Empty;

        public string DataTypeId { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public BridgeTable BridgeTable { get; set; } = null!;

    }
}
