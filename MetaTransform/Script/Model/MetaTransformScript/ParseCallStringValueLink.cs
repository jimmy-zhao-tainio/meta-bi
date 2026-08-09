#nullable enable

namespace MetaTransformScript
{
    public sealed class ParseCallStringValueLink
    {
        public string Id { get; set; } = string.Empty;

        public ParseCall ParseCall { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
