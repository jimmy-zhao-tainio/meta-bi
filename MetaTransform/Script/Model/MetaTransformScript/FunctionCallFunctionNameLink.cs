#nullable enable

namespace MetaTransformScript
{
    public sealed class FunctionCallFunctionNameLink
    {
        public string Id { get; set; } = string.Empty;

        public FunctionCall FunctionCall { get; set; } = null!;

        public Identifier Identifier { get; set; } = null!;

    }
}
