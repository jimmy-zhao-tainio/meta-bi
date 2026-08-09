#nullable enable

namespace MetaTransformScript
{
    public sealed class DeleteStatementFromClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public DeleteStatement DeleteStatement { get; set; } = null!;

        public FromClause FromClause { get; set; } = null!;

    }
}
