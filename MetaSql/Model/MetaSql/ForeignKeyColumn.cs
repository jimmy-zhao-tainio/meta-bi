#nullable enable

namespace MetaSql
{
    public sealed class ForeignKeyColumn
    {
        public string Id { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public ForeignKey ForeignKey { get; set; } = null!;

        public TableColumn SourceColumn { get; set; } = null!;

        public TableColumn TargetColumn { get; set; } = null!;

    }
}
