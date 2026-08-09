#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeTraversal
    {
        public string Id { get; set; } = string.Empty;

        public BusinessBridge BusinessBridge { get; set; } = null!;

        public BusinessBridgeTraversal? PreviousTraversal { get; set; }

        public BusinessLinkRole SourceRole { get; set; } = null!;

        public BusinessLinkRole TargetRole { get; set; } = null!;

    }
}
