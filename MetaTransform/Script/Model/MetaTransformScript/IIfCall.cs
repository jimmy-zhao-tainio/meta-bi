#nullable enable

namespace MetaTransformScript
{
    public sealed class IIfCall
    {
        public string Id { get; set; } = string.Empty;

        public PrimaryExpression PrimaryExpression { get; set; } = null!;

    }
}
