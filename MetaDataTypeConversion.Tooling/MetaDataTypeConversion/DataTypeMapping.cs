namespace MetaDataTypeConversion
{
    public sealed class DataTypeMapping
    {
        public string Id { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string SourceDataTypeId { get; set; } = string.Empty;
        public string TargetDataTypeId { get; set; } = string.Empty;
        public string ConversionImplementationId { get; set; } = string.Empty;
        public ConversionImplementation ConversionImplementation { get; set; } = new ConversionImplementation();
    }
}
