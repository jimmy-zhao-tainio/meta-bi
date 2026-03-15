namespace SqlModel
{
    public sealed class TableColumn
    {
        public string Id { get; internal set; } = string.Empty;
        public string Collation { get; internal set; } = string.Empty;
        public string ExpressionSql { get; internal set; } = string.Empty;
        public string IdentityIncrement { get; internal set; } = string.Empty;
        public string IdentitySeed { get; internal set; } = string.Empty;
        public string IsIdentity { get; internal set; } = string.Empty;
        public string IsNullable { get; internal set; } = string.Empty;
        public string Length { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string Precision { get; internal set; } = string.Empty;
        public string Scale { get; internal set; } = string.Empty;
        public string TypeName { get; internal set; } = string.Empty;
        public string TableId { get; internal set; } = string.Empty;
        public Table Table { get; internal set; } = new Table();
    }
}
