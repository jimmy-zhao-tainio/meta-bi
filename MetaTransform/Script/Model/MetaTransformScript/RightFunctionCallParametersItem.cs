#nullable enable

namespace MetaTransformScript
{
    public sealed class RightFunctionCallParametersItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public RightFunctionCall RightFunctionCall { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
