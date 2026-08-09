#nullable enable

namespace MetaTransformBinding
{
    public sealed class InsertQueryWrite
    {
        public string Id { get; set; } = string.Empty;

        public string MetaTransformScriptQueryExpressionId { get; set; } = string.Empty;

        public Write Write { get; set; } = null!;

    }
}
