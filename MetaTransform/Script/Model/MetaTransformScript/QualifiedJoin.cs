#nullable enable

namespace MetaTransformScript
{
    public sealed class QualifiedJoin
    {
        public string Id { get; set; } = string.Empty;

        public string? JoinHint { get; set; }

        public string? QualifiedJoinType { get; set; }

        public JoinTableReference JoinTableReference { get; set; } = null!;

    }
}
