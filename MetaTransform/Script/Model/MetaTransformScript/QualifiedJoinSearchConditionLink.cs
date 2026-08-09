#nullable enable

namespace MetaTransformScript
{
    public sealed class QualifiedJoinSearchConditionLink
    {
        public string Id { get; set; } = string.Empty;

        public BooleanExpression BooleanExpression { get; set; } = null!;

        public QualifiedJoin QualifiedJoin { get; set; } = null!;

    }
}
