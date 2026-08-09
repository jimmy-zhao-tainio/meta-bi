#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceKeyPart
    {
        public string Id { get; set; } = string.Empty;

        public string DataTypeId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public BusinessReference BusinessReference { get; set; } = null!;

        public BusinessReferenceKeyPart? PreviousKeyPart { get; set; }

    }
}
