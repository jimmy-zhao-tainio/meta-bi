#nullable enable

namespace MetaDataWarehouseImplementation
{
    public sealed class BridgeTableImplementation
    {
        public string Id { get; set; } = string.Empty;

        public string ParticipantForeignKeyNamePattern { get; set; } = string.Empty;

        public string ParticipantKeyColumnPattern { get; set; } = string.Empty;

        public string PrimaryKeyNamePattern { get; set; } = string.Empty;

        public string SchemaName { get; set; } = string.Empty;

        public string TableNamePattern { get; set; } = string.Empty;

        public string? WeightColumnName { get; set; }

        public string? WeightDataTypeId { get; set; }

    }
}
