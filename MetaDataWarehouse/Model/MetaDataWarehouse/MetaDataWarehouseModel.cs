#nullable enable

using System.Collections.Generic;

namespace MetaDataWarehouse
{
    public sealed partial class MetaDataWarehouseModel
    {
        public static MetaDataWarehouseModel CreateEmpty() => new();

        public List<AccumulatingSnapshotFact> AccumulatingSnapshotFactList { get; set; } = new();
        public List<AccumulatingSnapshotMilestone> AccumulatingSnapshotMilestoneList { get; set; } = new();
        public List<AggregateFact> AggregateFactList { get; set; } = new();
        public List<BridgeParticipant> BridgeParticipantList { get; set; } = new();
        public List<BridgeTable> BridgeTableList { get; set; } = new();
        public List<BridgeWeight> BridgeWeightList { get; set; } = new();
        public List<ConformedDimension> ConformedDimensionList { get; set; } = new();
        public List<DegenerateDimension> DegenerateDimensionList { get; set; } = new();
        public List<Dimension> DimensionList { get; set; } = new();
        public List<DimensionAttribute> DimensionAttributeList { get; set; } = new();
        public List<DimensionBusinessKey> DimensionBusinessKeyList { get; set; } = new();
        public List<DimensionBusinessKeyPart> DimensionBusinessKeyPartList { get; set; } = new();
        public List<DimensionHierarchy> DimensionHierarchyList { get; set; } = new();
        public List<DimensionHierarchyLevel> DimensionHierarchyLevelList { get; set; } = new();
        public List<Fact> FactList { get; set; } = new();
        public List<FactBridge> FactBridgeList { get; set; } = new();
        public List<FactDimension> FactDimensionList { get; set; } = new();
        public List<FactGrain> FactGrainList { get; set; } = new();
        public List<FactlessFact> FactlessFactList { get; set; } = new();
        public List<FactMeasure> FactMeasureList { get; set; } = new();
        public List<JunkDimension> JunkDimensionList { get; set; } = new();
        public List<JunkDimensionComponent> JunkDimensionComponentList { get; set; } = new();
        public List<MiniDimension> MiniDimensionList { get; set; } = new();
        public List<OutriggerDimension> OutriggerDimensionList { get; set; } = new();
        public List<PeriodicSnapshotFact> PeriodicSnapshotFactList { get; set; } = new();
        public List<SlowlyChangingDimension> SlowlyChangingDimensionList { get; set; } = new();
        public List<TransactionFact> TransactionFactList { get; set; } = new();
        public List<Type1DimensionAttribute> Type1DimensionAttributeList { get; set; } = new();
        public List<Type2DimensionAttribute> Type2DimensionAttributeList { get; set; } = new();
        public List<Warehouse> WarehouseList { get; set; } = new();
    }
}
