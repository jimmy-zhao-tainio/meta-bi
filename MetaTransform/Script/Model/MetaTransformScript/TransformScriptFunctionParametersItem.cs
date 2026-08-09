#nullable enable

namespace MetaTransformScript
{
    public sealed class TransformScriptFunctionParametersItem
    {
        public string Id { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public DataTypeReference DataTypeReference { get; set; } = null!;

        public Identifier Identifier { get; set; } = null!;

        public TransformScript TransformScript { get; set; } = null!;

    }
}
