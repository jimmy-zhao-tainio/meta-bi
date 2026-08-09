#nullable enable

namespace MetaTransformScript
{
    public sealed class NextValueForExpressionSequenceNameLink
    {
        public string Id { get; set; } = string.Empty;

        public NextValueForExpression NextValueForExpression { get; set; } = null!;

        public SchemaObjectName SchemaObjectName { get; set; } = null!;

    }
}
