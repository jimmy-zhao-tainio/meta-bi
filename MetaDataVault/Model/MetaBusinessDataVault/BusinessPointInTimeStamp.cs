#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTimeStamp
    {
        public string Id { get; set; } = string.Empty;

        public string DataTypeId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public BusinessPointInTime BusinessPointInTime { get; set; } = null!;

    }
}
