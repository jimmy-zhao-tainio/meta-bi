namespace SqlModel
{
    public sealed class TableColumn
    {
        public string Id { get; set; } = string.Empty;
        public string ExpressionSql { get; set; } = string.Empty;
        public string IdentityIncrement { get; set; } = string.Empty;
        public string IdentitySeed { get; set; } = string.Empty;
        public string IsIdentity { get; set; } = string.Empty;
        public string IsNullable { get; set; } = string.Empty;
        public string MetaDataTypeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string TableId { get; set; } = string.Empty;
        public Table Table { get; set; } = new Table();
    }
}
