#nullable enable

namespace MetaTransformBinding
{
    public sealed class WriteValueScalarExpression
    {
        public string Id { get; set; } = string.Empty;

        public string MetaTransformScriptScalarExpressionId { get; set; } = string.Empty;

        public WriteValue WriteValue { get; set; } = null!;

    }
}
