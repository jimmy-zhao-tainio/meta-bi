#nullable enable

namespace MetaTransformScript
{
    public sealed class FullTextTableReferenceTableNameLink
    {
        public string Id { get; set; } = string.Empty;

        public FullTextTableReference FullTextTableReference { get; set; } = null!;

        public SchemaObjectName SchemaObjectName { get; set; } = null!;

    }
}
