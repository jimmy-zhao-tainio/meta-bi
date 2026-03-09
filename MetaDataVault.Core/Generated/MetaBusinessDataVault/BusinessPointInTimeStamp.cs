namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTimeStamp
    {
        public string Id { get; internal set; } = string.Empty;
        public string DataTypeId { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessPointInTimeId { get; internal set; } = string.Empty;
        public BusinessPointInTime BusinessPointInTime { get; internal set; } = new BusinessPointInTime();
    }
}
