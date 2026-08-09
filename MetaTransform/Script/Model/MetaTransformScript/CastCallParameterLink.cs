#nullable enable

namespace MetaTransformScript
{
    public sealed class CastCallParameterLink
    {
        public string Id { get; set; } = string.Empty;

        public CastCall CastCall { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
