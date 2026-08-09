#nullable enable

namespace MetaTransformScript
{
    public sealed class ParseCall
    {
        public string Id { get; set; } = string.Empty;

        public PrimaryExpression PrimaryExpression { get; set; } = null!;

    }
}
