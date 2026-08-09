#nullable enable

namespace MetaTransformScript
{
    public sealed class SqlDataTypeReference
    {
        public string Id { get; set; } = string.Empty;

        public string? SqlDataTypeOption { get; set; }

        public ParameterizedDataTypeReference ParameterizedDataTypeReference { get; set; } = null!;

    }
}
