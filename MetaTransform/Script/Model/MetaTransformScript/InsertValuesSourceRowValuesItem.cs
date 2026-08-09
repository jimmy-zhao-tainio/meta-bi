#nullable enable

namespace MetaTransformScript
{
    public sealed class InsertValuesSourceRowValuesItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public InsertValuesSource InsertValuesSource { get; set; } = null!;

        public RowValue RowValue { get; set; } = null!;

    }
}
