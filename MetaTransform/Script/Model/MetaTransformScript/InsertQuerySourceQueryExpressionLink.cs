#nullable enable

namespace MetaTransformScript
{
    public sealed class InsertQuerySourceQueryExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public InsertQuerySource InsertQuerySource { get; set; } = null!;

        public QueryExpression QueryExpression { get; set; } = null!;

    }
}
