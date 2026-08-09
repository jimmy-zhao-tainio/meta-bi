#nullable enable

namespace MetaTransformScript
{
    public sealed class UnpivotedTableReferenceTableReferenceLink
    {
        public string Id { get; set; } = string.Empty;

        public TableReference TableReference { get; set; } = null!;

        public UnpivotedTableReference UnpivotedTableReference { get; set; } = null!;

    }
}
