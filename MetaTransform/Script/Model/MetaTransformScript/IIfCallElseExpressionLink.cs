#nullable enable

namespace MetaTransformScript
{
    public sealed class IIfCallElseExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public IIfCall IIfCall { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
