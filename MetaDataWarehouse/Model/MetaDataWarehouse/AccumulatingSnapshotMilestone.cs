#nullable enable

namespace MetaDataWarehouse
{
    public sealed class AccumulatingSnapshotMilestone
    {
        public string Id { get; set; } = string.Empty;

        public string DateRoleName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public AccumulatingSnapshotFact AccumulatingSnapshotFact { get; set; } = null!;

    }
}
