#nullable enable

namespace MetaTransformScript
{
    public sealed class ColumnReferenceExpressionMultiPartIdentifierLink
    {
        public string Id { get; set; } = string.Empty;

        public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null!;

        public MultiPartIdentifier MultiPartIdentifier { get; set; } = null!;

    }
}
