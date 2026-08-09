#nullable enable

namespace MetaTransformScript
{
    public sealed class LeftFunctionCallParametersItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public LeftFunctionCall LeftFunctionCall { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
