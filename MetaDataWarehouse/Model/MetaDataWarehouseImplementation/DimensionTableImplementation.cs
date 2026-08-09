#nullable enable

namespace MetaDataWarehouseImplementation
{
    public sealed class DimensionTableImplementation
    {
        public string Id { get; set; } = string.Empty;

        public string AttributeColumnPattern { get; set; } = string.Empty;

        public string BusinessKeyColumnPattern { get; set; } = string.Empty;

        public string PrimaryKeyNamePattern { get; set; } = string.Empty;

        public string SchemaName { get; set; } = string.Empty;

        public string SurrogateKeyColumnName { get; set; } = string.Empty;

        public string SurrogateKeyDataTypeId { get; set; } = string.Empty;

        public string TableNamePattern { get; set; } = string.Empty;

    }
}
