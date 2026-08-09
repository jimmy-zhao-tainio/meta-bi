#nullable enable

namespace MetaTransformScript
{
    public sealed class PivotedTableReferenceInColumnsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public Identifier Identifier { get; set; } = null!;

        public PivotedTableReference PivotedTableReference { get; set; } = null!;

    }
}
