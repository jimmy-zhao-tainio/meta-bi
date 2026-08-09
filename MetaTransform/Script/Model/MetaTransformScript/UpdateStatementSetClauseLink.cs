#nullable enable

namespace MetaTransformScript
{
    public sealed class UpdateStatementSetClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public SetClause SetClause { get; set; } = null!;

        public UpdateStatement UpdateStatement { get; set; } = null!;

    }
}
