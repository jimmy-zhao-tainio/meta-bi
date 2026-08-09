#nullable enable

namespace MetaTransformScript
{
    public sealed class CastCallDataTypeLink
    {
        public string Id { get; set; } = string.Empty;

        public CastCall CastCall { get; set; } = null!;

        public DataTypeReference DataTypeReference { get; set; } = null!;

    }
}
