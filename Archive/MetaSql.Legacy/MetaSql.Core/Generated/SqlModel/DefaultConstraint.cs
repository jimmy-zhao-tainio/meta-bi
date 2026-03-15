namespace SqlModel
{
    public sealed class DefaultConstraint
    {
        public string Id { get; internal set; } = string.Empty;
        public string ExpressionSql { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string TableColumnId { get; internal set; } = string.Empty;
        public TableColumn TableColumn { get; internal set; } = new TableColumn();
    }
}
