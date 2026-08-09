#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeStatementSearchConditionLink
    {
        public string Id { get; set; } = string.Empty;

        public BooleanExpression BooleanExpression { get; set; } = null!;

        public MergeStatement MergeStatement { get; set; } = null!;

    }
}
