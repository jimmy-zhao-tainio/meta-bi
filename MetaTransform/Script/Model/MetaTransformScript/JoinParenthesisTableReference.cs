#nullable enable

namespace MetaTransformScript
{
    public sealed class JoinParenthesisTableReference
    {
        public string Id { get; set; } = string.Empty;

        public TableReference TableReference { get; set; } = null!;

    }
}
