#nullable enable

namespace MetaTransformScript
{
    public sealed class CommonTableExpressionExpressionNameLink
    {
        public string Id { get; set; } = string.Empty;

        public CommonTableExpression CommonTableExpression { get; set; } = null!;

        public Identifier Identifier { get; set; } = null!;

    }
}
