#nullable enable

namespace MetaTransformScript
{
    public sealed class TableReferenceWithAlias
    {
        public string Id { get; set; } = string.Empty;

        public string? ForPath { get; set; }

        public TableReference TableReference { get; set; } = null!;

    }
}
