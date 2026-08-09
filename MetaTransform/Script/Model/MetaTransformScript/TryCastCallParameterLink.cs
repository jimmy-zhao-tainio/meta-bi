#nullable enable

namespace MetaTransformScript
{
    public sealed class TryCastCallParameterLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public TryCastCall TryCastCall { get; set; } = null!;

    }
}
