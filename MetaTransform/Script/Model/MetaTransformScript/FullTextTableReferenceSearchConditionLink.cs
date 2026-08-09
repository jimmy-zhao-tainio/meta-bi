#nullable enable

namespace MetaTransformScript
{
    public sealed class FullTextTableReferenceSearchConditionLink
    {
        public string Id { get; set; } = string.Empty;

        public FullTextTableReference FullTextTableReference { get; set; } = null!;

        public ValueExpression ValueExpression { get; set; } = null!;

    }
}
