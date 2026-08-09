#nullable enable

namespace MetaTransformScript
{
    public sealed class CommonTableExpressionColumnsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public CommonTableExpression CommonTableExpression { get; set; } = null!;

        public Identifier Identifier { get; set; } = null!;

    }
}
