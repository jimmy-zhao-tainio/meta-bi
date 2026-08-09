#nullable enable

namespace MetaTransformScript
{
    public sealed class InlineDerivedTableRowValuesItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public InlineDerivedTable InlineDerivedTable { get; set; } = null!;

        public RowValue RowValue { get; set; } = null!;

    }
}
