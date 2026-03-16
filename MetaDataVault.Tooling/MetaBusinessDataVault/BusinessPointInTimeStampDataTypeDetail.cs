namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTimeStampDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BusinessPointInTimeStampId { get; set; } = string.Empty;
        public BusinessPointInTimeStamp BusinessPointInTimeStamp { get; set; } = new BusinessPointInTimeStamp();
    }
}
