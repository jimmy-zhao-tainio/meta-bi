#nullable enable

namespace MetaTransformScript
{
    public sealed class DeleteStatementWhereClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public DeleteStatement DeleteStatement { get; set; } = null!;

        public WhereClause WhereClause { get; set; } = null!;

    }
}
