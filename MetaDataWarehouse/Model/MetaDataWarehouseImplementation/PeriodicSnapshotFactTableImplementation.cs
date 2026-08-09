#nullable enable

namespace MetaDataWarehouseImplementation
{
    public sealed class PeriodicSnapshotFactTableImplementation
    {
        public string Id { get; set; } = string.Empty;

        public string PeriodDataTypeId { get; set; } = string.Empty;

        public string? PeriodEndColumnName { get; set; }

        public string PeriodStartColumnName { get; set; } = string.Empty;

    }
}
