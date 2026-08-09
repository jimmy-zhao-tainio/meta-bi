#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceKeyPartDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public BusinessReferenceKeyPart BusinessReferenceKeyPart { get; set; } = null!;

    }
}
