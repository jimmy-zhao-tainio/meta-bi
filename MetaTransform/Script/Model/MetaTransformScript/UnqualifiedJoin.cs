#nullable enable

namespace MetaTransformScript
{
    public sealed class UnqualifiedJoin
    {
        public string Id { get; set; } = string.Empty;

        public string? UnqualifiedJoinType { get; set; }

        public JoinTableReference JoinTableReference { get; set; } = null!;

    }
}
