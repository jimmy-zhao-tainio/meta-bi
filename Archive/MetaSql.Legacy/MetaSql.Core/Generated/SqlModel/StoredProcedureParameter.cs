namespace SqlModel
{
    public sealed class StoredProcedureParameter
    {
        public string Id { get; internal set; } = string.Empty;
        public string IsNullable { get; internal set; } = string.Empty;
        public string Length { get; internal set; } = string.Empty;
        public string Mode { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string Precision { get; internal set; } = string.Empty;
        public string Scale { get; internal set; } = string.Empty;
        public string TypeName { get; internal set; } = string.Empty;
        public string StoredProcedureId { get; internal set; } = string.Empty;
        public StoredProcedure StoredProcedure { get; internal set; } = new StoredProcedure();
    }
}
