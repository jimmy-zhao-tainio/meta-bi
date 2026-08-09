#nullable enable

namespace MetaDataWarehouse
{
    public sealed class PeriodicSnapshotFact
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string PeriodName { get; set; } = string.Empty;

        public Fact Fact { get; set; } = null!;

    }
}
