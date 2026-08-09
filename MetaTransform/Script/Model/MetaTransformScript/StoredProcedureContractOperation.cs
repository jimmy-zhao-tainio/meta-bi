#nullable enable

namespace MetaTransformScript
{
    public sealed class StoredProcedureContractOperation
    {
        public string Id { get; set; } = string.Empty;

        public string? AccessRole { get; set; }

        public string? Notes { get; set; }

        public string OperationKind { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public string SqlIdentifier { get; set; } = string.Empty;

        public StoredProcedureContract StoredProcedureContract { get; set; } = null!;

    }
}
