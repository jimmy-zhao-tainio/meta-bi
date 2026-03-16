namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTime
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string BusinessHubId { get; set; } = string.Empty;
        public BusinessHub BusinessHub { get; set; } = new BusinessHub();
    }
}
