#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkRole
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public BusinessHub BusinessHub { get; set; } = null!;

        public BusinessLink BusinessLink { get; set; } = null!;

    }
}
