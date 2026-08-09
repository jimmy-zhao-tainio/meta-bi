#nullable enable

namespace MetaTransformScript
{
    public sealed class NullIfExpressionFirstExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public NullIfExpression NullIfExpression { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
