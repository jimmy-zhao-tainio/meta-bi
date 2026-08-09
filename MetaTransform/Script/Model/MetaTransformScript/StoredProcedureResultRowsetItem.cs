#nullable enable

namespace MetaTransformScript
{
    public sealed class StoredProcedureResultRowsetItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Name { get; set; }

        public string Ordinal { get; set; } = string.Empty;

        public StoredProcedureContract StoredProcedureContract { get; set; } = null!;

    }
}
