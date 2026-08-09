#nullable enable

namespace MetaTransformScript
{
    public sealed class PrimaryExpression
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
