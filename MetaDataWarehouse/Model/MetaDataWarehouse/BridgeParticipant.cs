#nullable enable

namespace MetaDataWarehouse
{
    public sealed class BridgeParticipant
    {
        public string Id { get; set; } = string.Empty;

        public string? IsRequired { get; set; }

        public string Ordinal { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public BridgeTable BridgeTable { get; set; } = null!;

        public Dimension Dimension { get; set; } = null!;

    }
}
