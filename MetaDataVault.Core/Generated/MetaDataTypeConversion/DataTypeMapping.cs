namespace MetaDataTypeConversion
{
    public sealed class DataTypeMapping
    {
        public string Id { get; internal set; } = string.Empty;
        public string Notes { get; internal set; } = string.Empty;
        public string SourceDataTypeId { get; internal set; } = string.Empty;
        public string TargetDataTypeId { get; internal set; } = string.Empty;
        public string ConversionImplementationId { get; internal set; } = string.Empty;
        public ConversionImplementation ConversionImplementation { get; internal set; } = new ConversionImplementation();
    }
}
