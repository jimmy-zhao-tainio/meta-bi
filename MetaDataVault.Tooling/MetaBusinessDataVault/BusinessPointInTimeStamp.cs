namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTimeStamp
    {
        public string Id { get; set; } = string.Empty;
        public string DataTypeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessPointInTimeId { get; set; } = string.Empty;
        public BusinessPointInTime BusinessPointInTime { get; set; } = new BusinessPointInTime();
    }
}
