#nullable enable

namespace MetaTransformScript
{
    public sealed class FunctionCall
    {
        public string Id { get; set; } = string.Empty;

        public string? UniqueRowFilter { get; set; }

        public string? WithArrayWrapper { get; set; }

        public PrimaryExpression PrimaryExpression { get; set; } = null!;

    }
}
