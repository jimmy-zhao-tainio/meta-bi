namespace SqlModel
{
    public sealed class ForeignKey
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string SourceTableId { get; internal set; } = string.Empty;
        public Table SourceTable { get; internal set; } = new Table();
        public string TargetTableId { get; internal set; } = string.Empty;
        public Table TargetTable { get; internal set; } = new Table();
    }
}
