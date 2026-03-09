namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTimeStampDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessPointInTimeStampId { get; internal set; } = string.Empty;
        public BusinessPointInTimeStamp BusinessPointInTimeStamp { get; internal set; } = new BusinessPointInTimeStamp();
    }
}
