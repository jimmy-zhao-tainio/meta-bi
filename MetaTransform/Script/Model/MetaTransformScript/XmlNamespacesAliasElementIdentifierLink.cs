#nullable enable

namespace MetaTransformScript
{
    public sealed class XmlNamespacesAliasElementIdentifierLink
    {
        public string Id { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public XmlNamespacesAliasElement XmlNamespacesAliasElement { get; set; } = null!;

    }
}
