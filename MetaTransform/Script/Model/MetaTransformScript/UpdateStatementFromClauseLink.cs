#nullable enable

namespace MetaTransformScript
{
    public sealed class UpdateStatementFromClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public FromClause FromClause { get; set; } = null!;

        public UpdateStatement UpdateStatement { get; set; } = null!;

    }
}
