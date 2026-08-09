#nullable enable

namespace MetaRawDataVault
{
    public sealed class RawHubKeyPart
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Field Field { get; set; } = null!;

        public RawHub RawHub { get; set; } = null!;

    }
}
