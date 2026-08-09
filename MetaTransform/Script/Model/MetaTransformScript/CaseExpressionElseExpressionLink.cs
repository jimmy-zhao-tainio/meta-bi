#nullable enable

namespace MetaTransformScript
{
    public sealed class CaseExpressionElseExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public CaseExpression CaseExpression { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
