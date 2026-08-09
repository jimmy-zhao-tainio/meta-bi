#nullable enable

namespace MetaTransformScript
{
    public sealed class IIfCallThenExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public IIfCall IIfCall { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
