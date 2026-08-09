#nullable enable

namespace MetaTransformScript
{
    public sealed class TryParseCallCultureLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public TryParseCall TryParseCall { get; set; } = null!;

    }
}
