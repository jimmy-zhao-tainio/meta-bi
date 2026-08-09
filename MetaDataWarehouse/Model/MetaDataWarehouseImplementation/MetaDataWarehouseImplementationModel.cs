#nullable enable

using System.Collections.Generic;

namespace MetaDataWarehouseImplementation
{
    public sealed partial class MetaDataWarehouseImplementationModel
    {
        public static MetaDataWarehouseImplementationModel CreateEmpty() => new();

        public List<AccumulatingSnapshotFactTableImplementation> AccumulatingSnapshotFactTableImplementationList { get; set; } = new();
        public List<AggregateFactTableImplementation> AggregateFactTableImplementationList { get; set; } = new();
        public List<BridgeTableImplementation> BridgeTableImplementationList { get; set; } = new();
        public List<DimensionTableImplementation> DimensionTableImplementationList { get; set; } = new();
        public List<FactTableImplementation> FactTableImplementationList { get; set; } = new();
        public List<IndexImplementation> IndexImplementationList { get; set; } = new();
        public List<PeriodicSnapshotFactTableImplementation> PeriodicSnapshotFactTableImplementationList { get; set; } = new();
        public List<PlatformColumnImplementation> PlatformColumnImplementationList { get; set; } = new();
        public List<SlowlyChangingDimensionTableImplementation> SlowlyChangingDimensionTableImplementationList { get; set; } = new();
    }
}
