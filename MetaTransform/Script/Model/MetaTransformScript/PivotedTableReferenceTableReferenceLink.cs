#nullable enable

namespace MetaTransformScript
{
    public sealed class PivotedTableReferenceTableReferenceLink
    {
        public string Id { get; set; } = string.Empty;

        public PivotedTableReference PivotedTableReference { get; set; } = null!;

        public TableReference TableReference { get; set; } = null!;

    }
}
