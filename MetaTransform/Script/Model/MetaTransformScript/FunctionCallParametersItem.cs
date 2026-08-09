#nullable enable

namespace MetaTransformScript
{
    public sealed class FunctionCallParametersItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public FunctionCall FunctionCall { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
