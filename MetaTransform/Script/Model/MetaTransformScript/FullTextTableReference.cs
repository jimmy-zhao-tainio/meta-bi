#nullable enable

namespace MetaTransformScript
{
    public sealed class FullTextTableReference
    {
        public string Id { get; set; } = string.Empty;

        public string? FullTextFunctionType { get; set; }

        public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null!;

    }
}
