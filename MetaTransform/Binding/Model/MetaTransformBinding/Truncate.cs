#nullable enable

namespace MetaTransformBinding
{
    public sealed class Truncate
    {
        public string Id { get; set; } = string.Empty;

        public string MetaTransformScriptTruncateStatementId { get; set; } = string.Empty;

        public ValidationTargetRowsetLink ValidationTargetRowsetLink { get; set; } = null!;

    }
}
