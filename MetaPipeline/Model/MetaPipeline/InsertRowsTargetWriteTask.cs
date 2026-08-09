#nullable enable

namespace MetaPipeline
{
    public sealed class InsertRowsTargetWriteTask
    {
        public string Id { get; set; } = string.Empty;

        public string? BatchSize { get; set; }

        public string? TargetDataTypeSystemName { get; set; }

        public string TargetSqlIdentifier { get; set; } = string.Empty;

        public TargetWriteTask TargetWriteTask { get; set; } = null!;

    }
}
