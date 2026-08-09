#nullable enable

namespace MetaTransformScript
{
    public sealed class ParseCallDataTypeLink
    {
        public string Id { get; set; } = string.Empty;

        public DataTypeReference DataTypeReference { get; set; } = null!;

        public ParseCall ParseCall { get; set; } = null!;

    }
}
