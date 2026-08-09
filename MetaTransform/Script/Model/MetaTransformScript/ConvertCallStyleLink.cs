#nullable enable

namespace MetaTransformScript
{
    public sealed class ConvertCallStyleLink
    {
        public string Id { get; set; } = string.Empty;

        public ConvertCall ConvertCall { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
