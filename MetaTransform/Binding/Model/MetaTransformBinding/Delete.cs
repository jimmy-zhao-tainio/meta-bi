#nullable enable

namespace MetaTransformBinding
{
    public sealed class Delete
    {
        public string Id { get; set; } = string.Empty;

        public string MetaTransformScriptDeleteStatementId { get; set; } = string.Empty;

        public ValidationTargetRowsetLink ValidationTargetRowsetLink { get; set; } = null!;

    }
}
