#nullable enable

namespace MetaTransformBinding
{
    public sealed class UpdateWrite
    {
        public string Id { get; set; } = string.Empty;

        public string MetaTransformScriptSetClauseId { get; set; } = string.Empty;

        public Write Write { get; set; } = null!;

    }
}
