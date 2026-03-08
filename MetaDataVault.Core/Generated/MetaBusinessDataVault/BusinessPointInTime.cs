namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTime
    {
        public string Id { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string BusinessHubId { get; internal set; } = string.Empty;
        public BusinessHub BusinessHub { get; internal set; } = new BusinessHub();
    }
}
