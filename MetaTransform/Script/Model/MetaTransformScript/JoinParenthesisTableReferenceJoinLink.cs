#nullable enable

namespace MetaTransformScript
{
    public sealed class JoinParenthesisTableReferenceJoinLink
    {
        public string Id { get; set; } = string.Empty;

        public JoinParenthesisTableReference JoinParenthesisTableReference { get; set; } = null!;

        public TableReference TableReference { get; set; } = null!;

    }
}
