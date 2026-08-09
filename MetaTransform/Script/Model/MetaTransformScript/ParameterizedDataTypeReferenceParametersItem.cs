#nullable enable

namespace MetaTransformScript
{
    public sealed class ParameterizedDataTypeReferenceParametersItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public Literal Literal { get; set; } = null!;

        public ParameterizedDataTypeReference ParameterizedDataTypeReference { get; set; } = null!;

    }
}
