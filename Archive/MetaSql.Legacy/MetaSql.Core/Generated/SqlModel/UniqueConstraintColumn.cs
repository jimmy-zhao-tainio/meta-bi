namespace SqlModel
{
    public sealed class UniqueConstraintColumn
    {
        public string Id { get; internal set; } = string.Empty;
        public string IsDescending { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string TableColumnId { get; internal set; } = string.Empty;
        public TableColumn TableColumn { get; internal set; } = new TableColumn();
        public string UniqueConstraintId { get; internal set; } = string.Empty;
        public UniqueConstraint UniqueConstraint { get; internal set; } = new UniqueConstraint();
    }
}
