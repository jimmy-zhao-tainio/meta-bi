#nullable enable

namespace MetaDataWarehouseImplementation
{
    public sealed class AggregateFactTableImplementation
    {
        public string Id { get; set; } = string.Empty;

        public string SchemaName { get; set; } = string.Empty;

        public string? SourceFactForeignKeyNamePattern { get; set; }

        public string TableNamePattern { get; set; } = string.Empty;

    }
}
