#nullable enable

namespace MetaTransformBinding
{
    public sealed class ValidationSourceRowsetLink
    {
        public string Id { get; set; } = string.Empty;

        public string MetaSchemaTableId { get; set; } = string.Empty;

        public Rowset Rowset { get; set; } = null!;

        public Validation Validation { get; set; } = null!;

    }
}
