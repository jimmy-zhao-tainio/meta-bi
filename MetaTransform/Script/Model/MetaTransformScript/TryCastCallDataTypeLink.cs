#nullable enable

namespace MetaTransformScript
{
    public sealed class TryCastCallDataTypeLink
    {
        public string Id { get; set; } = string.Empty;

        public DataTypeReference DataTypeReference { get; set; } = null!;

        public TryCastCall TryCastCall { get; set; } = null!;

    }
}
