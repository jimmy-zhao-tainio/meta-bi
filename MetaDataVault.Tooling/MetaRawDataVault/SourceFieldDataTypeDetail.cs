namespace MetaRawDataVault
{
    public sealed class SourceFieldDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string SourceFieldId { get; set; } = string.Empty;
        public SourceField SourceField { get; set; } = new SourceField();
    }
}
