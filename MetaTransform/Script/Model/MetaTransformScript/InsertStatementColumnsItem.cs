#nullable enable

namespace MetaTransformScript
{
    public sealed class InsertStatementColumnsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public Identifier Identifier { get; set; } = null!;

        public InsertStatement InsertStatement { get; set; } = null!;

    }
}
