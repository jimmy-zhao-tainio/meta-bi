namespace SqlModel
{
    public sealed class CheckConstraint
    {
        public string Id { get; internal set; } = string.Empty;
        public string ExpressionSql { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string TableId { get; internal set; } = string.Empty;
        public Table Table { get; internal set; } = new Table();
    }
}
