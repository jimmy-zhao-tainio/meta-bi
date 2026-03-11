namespace MetaRawDataVault
{
    public sealed class SourceFieldDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string SourceFieldId { get; internal set; } = string.Empty;
        public SourceField SourceField { get; internal set; } = new SourceField();
    }
}
