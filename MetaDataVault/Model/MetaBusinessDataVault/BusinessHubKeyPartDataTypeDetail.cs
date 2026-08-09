#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessHubKeyPartDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public BusinessHubKeyPart BusinessHubKeyPart { get; set; } = null!;

    }
}
