#nullable enable

namespace MetaTransformScript
{
    public sealed class FromClauseTableReferencesItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public FromClause FromClause { get; set; } = null!;

        public TableReference TableReference { get; set; } = null!;

    }
}
