#nullable enable

namespace MetaTransformScript
{
    public sealed class ConvertCallParameterLink
    {
        public string Id { get; set; } = string.Empty;

        public ConvertCall ConvertCall { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
