#nullable enable

namespace MetaTransformScript
{
    public sealed class ConvertCallDataTypeLink
    {
        public string Id { get; set; } = string.Empty;

        public ConvertCall ConvertCall { get; set; } = null!;

        public DataTypeReference DataTypeReference { get; set; } = null!;

    }
}
