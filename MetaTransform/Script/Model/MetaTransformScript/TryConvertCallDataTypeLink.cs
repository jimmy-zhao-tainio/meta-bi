#nullable enable

namespace MetaTransformScript
{
    public sealed class TryConvertCallDataTypeLink
    {
        public string Id { get; set; } = string.Empty;

        public DataTypeReference DataTypeReference { get; set; } = null!;

        public TryConvertCall TryConvertCall { get; set; } = null!;

    }
}
