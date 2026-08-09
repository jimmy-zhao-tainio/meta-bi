#nullable enable

namespace MetaTransformScript
{
    public sealed class XmlNodesTableReference
    {
        public string Id { get; set; } = string.Empty;

        public TableReferenceWithAliasAndColumns TableReferenceWithAliasAndColumns { get; set; } = null!;

    }
}
