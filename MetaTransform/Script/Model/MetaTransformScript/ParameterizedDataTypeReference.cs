#nullable enable

namespace MetaTransformScript
{
    public sealed class ParameterizedDataTypeReference
    {
        public string Id { get; set; } = string.Empty;

        public DataTypeReference DataTypeReference { get; set; } = null!;

    }
}
