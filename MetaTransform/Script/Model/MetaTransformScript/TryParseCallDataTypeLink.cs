#nullable enable

namespace MetaTransformScript
{
    public sealed class TryParseCallDataTypeLink
    {
        public string Id { get; set; } = string.Empty;

        public DataTypeReference DataTypeReference { get; set; } = null!;

        public TryParseCall TryParseCall { get; set; } = null!;

    }
}
