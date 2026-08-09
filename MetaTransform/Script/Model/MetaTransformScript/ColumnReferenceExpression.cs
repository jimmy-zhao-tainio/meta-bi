#nullable enable

namespace MetaTransformScript
{
    public sealed class ColumnReferenceExpression
    {
        public string Id { get; set; } = string.Empty;

        public string? ColumnType { get; set; }

        public PrimaryExpression PrimaryExpression { get; set; } = null!;

    }
}
