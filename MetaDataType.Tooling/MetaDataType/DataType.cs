namespace MetaDataType
{
    public sealed class DataType
    {
        public string Id { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IsCanonical { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DataTypeSystemId { get; set; } = string.Empty;
        public DataTypeSystem DataTypeSystem { get; set; } = new DataTypeSystem();
    }
}
