#nullable enable

namespace MetaTransformScript
{
    public sealed class UpdateStatementWhereClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public UpdateStatement UpdateStatement { get; set; } = null!;

        public WhereClause WhereClause { get; set; } = null!;

    }
}
