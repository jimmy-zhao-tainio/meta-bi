#nullable enable

namespace MetaTransformScript
{
    public sealed class NamedTableReferenceTableSampleClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public NamedTableReference NamedTableReference { get; set; } = null!;

        public TableSampleClause TableSampleClause { get; set; } = null!;

    }
}
