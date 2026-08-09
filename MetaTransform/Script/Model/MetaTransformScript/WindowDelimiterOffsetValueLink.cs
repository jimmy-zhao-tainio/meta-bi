#nullable enable

namespace MetaTransformScript
{
    public sealed class WindowDelimiterOffsetValueLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public WindowDelimiter WindowDelimiter { get; set; } = null!;

    }
}
