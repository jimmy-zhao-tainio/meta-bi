#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTime
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public BusinessHub BusinessHub { get; set; } = null!;

    }
}
