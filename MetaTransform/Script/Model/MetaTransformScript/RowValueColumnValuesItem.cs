#nullable enable

namespace MetaTransformScript
{
    public sealed class RowValueColumnValuesItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public RowValue RowValue { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
