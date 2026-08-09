#nullable enable

namespace MetaTransformScript
{
    public sealed class StoredProcedureResultColumnItem
    {
        public string Id { get; set; } = string.Empty;

        public string? IsNullable { get; set; }

        public string? MetaDataTypeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public StoredProcedureResultRowsetItem StoredProcedureResultRowsetItem { get; set; } = null!;

    }
}
