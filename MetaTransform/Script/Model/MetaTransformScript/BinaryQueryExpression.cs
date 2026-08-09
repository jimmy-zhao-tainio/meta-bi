#nullable enable

namespace MetaTransformScript
{
    public sealed class BinaryQueryExpression
    {
        public string Id { get; set; } = string.Empty;

        public string? All { get; set; }

        public string? BinaryQueryExpressionType { get; set; }

        public QueryExpression QueryExpression { get; set; } = null!;

    }
}
