#nullable enable

namespace MetaTransformScript
{
    public sealed class FunctionCallCallTargetLink
    {
        public string Id { get; set; } = string.Empty;

        public CallTarget CallTarget { get; set; } = null!;

        public FunctionCall FunctionCall { get; set; } = null!;

    }
}
