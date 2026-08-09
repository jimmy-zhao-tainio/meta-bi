#nullable enable

namespace MetaTransformScript
{
    public sealed class TryConvertCallParameterLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public TryConvertCall TryConvertCall { get; set; } = null!;

    }
}
