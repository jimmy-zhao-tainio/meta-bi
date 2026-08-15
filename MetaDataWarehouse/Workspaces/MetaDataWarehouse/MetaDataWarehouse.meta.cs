#nullable enable
using System;
using System.Collections.Generic;

namespace MetaDataWarehouse;
public sealed partial class AccumulatingSnapshotFact
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public Fact Fact { get; set; } = null !;
}

public sealed partial class AccumulatingSnapshotMilestone
{
    public string Id { get; set; } = null !;
    public string DateRoleName { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public AccumulatingSnapshotFact AccumulatingSnapshotFact { get; set; } = null !;
}

public sealed partial class AggregateFact
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public Fact AggregatedFact { get; set; } = null !;
    public Fact SourceFact { get; set; } = null !;
}

public sealed partial class BridgeParticipant
{
    public string Id { get; set; } = null !;
    public string? IsRequired { get; set; }
    public string Ordinal { get; set; } = null !;
    public string RoleName { get; set; } = null !;
    public BridgeTable BridgeTable { get; set; } = null !;
    public Dimension Dimension { get; set; } = null !;
}

public sealed partial class BridgeTable
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public Warehouse Warehouse { get; set; } = null !;
}

public sealed partial class BridgeWeight
{
    public string Id { get; set; } = null !;
    public string DataTypeId { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public BridgeTable BridgeTable { get; set; } = null !;
}

public sealed partial class ConformedDimension
{
    public string Id { get; set; } = null !;
    public string ConformanceName { get; set; } = null !;
    public string? Description { get; set; }
    public Dimension Dimension { get; set; } = null !;
}

public sealed partial class DegenerateDimension
{
    public string Id { get; set; } = null !;
    public string DataTypeId { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public Fact Fact { get; set; } = null !;
}

public sealed partial class Dimension
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public Warehouse Warehouse { get; set; } = null !;
}

public sealed partial class DimensionAttribute
{
    public string Id { get; set; } = null !;
    public string DataTypeId { get; set; } = null !;
    public string? Description { get; set; }
    public string? IsNullable { get; set; }
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public Dimension Dimension { get; set; } = null !;
}

public sealed partial class DimensionBusinessKey
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public Dimension Dimension { get; set; } = null !;
}

public sealed partial class DimensionBusinessKeyPart
{
    public string Id { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public DimensionAttribute DimensionAttribute { get; set; } = null !;
    public DimensionBusinessKey DimensionBusinessKey { get; set; } = null !;
}

public sealed partial class DimensionHierarchy
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public Dimension Dimension { get; set; } = null !;
}

public sealed partial class DimensionHierarchyLevel
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public DimensionAttribute DimensionAttribute { get; set; } = null !;
    public DimensionHierarchy DimensionHierarchy { get; set; } = null !;
}

public sealed partial class Fact
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public Warehouse Warehouse { get; set; } = null !;
}

public sealed partial class FactBridge
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Ordinal { get; set; } = null !;
    public string RoleName { get; set; } = null !;
    public BridgeTable BridgeTable { get; set; } = null !;
    public Fact Fact { get; set; } = null !;
}

public sealed partial class FactDimension
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? IsRequired { get; set; }
    public string Ordinal { get; set; } = null !;
    public string RoleName { get; set; } = null !;
    public Dimension Dimension { get; set; } = null !;
    public Fact Fact { get; set; } = null !;
}

public sealed partial class FactGrain
{
    public string Id { get; set; } = null !;
    public string Description { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Fact Fact { get; set; } = null !;
}

public sealed partial class FactlessFact
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public Fact Fact { get; set; } = null !;
}

public sealed partial class FactMeasure
{
    public string Id { get; set; } = null !;
    public string DataTypeId { get; set; } = null !;
    public string? Description { get; set; }
    public string? IsNullable { get; set; }
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public Fact Fact { get; set; } = null !;
}

public sealed partial class JunkDimension
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public Dimension Dimension { get; set; } = null !;
}

public sealed partial class JunkDimensionComponent
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Ordinal { get; set; } = null !;
    public DimensionAttribute DimensionAttribute { get; set; } = null !;
    public JunkDimension JunkDimension { get; set; } = null !;
}

public sealed partial class MiniDimension
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? RoleName { get; set; }
    public Dimension ProfileDimension { get; set; } = null !;
    public Dimension SourceDimension { get; set; } = null !;
}

public sealed partial class OutriggerDimension
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? IsRequired { get; set; }
    public string Ordinal { get; set; } = null !;
    public string RoleName { get; set; } = null !;
    public Dimension ChildDimension { get; set; } = null !;
    public Dimension ParentDimension { get; set; } = null !;
}

public sealed partial class PeriodicSnapshotFact
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string PeriodName { get; set; } = null !;
    public Fact Fact { get; set; } = null !;
}

public sealed partial class SlowlyChangingDimension
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? Name { get; set; }
    public Dimension Dimension { get; set; } = null !;
}

public sealed partial class TransactionFact
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public Fact Fact { get; set; } = null !;
}

public sealed partial class Type1DimensionAttribute
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public DimensionAttribute DimensionAttribute { get; set; } = null !;
    public SlowlyChangingDimension SlowlyChangingDimension { get; set; } = null !;
}

public sealed partial class Type2DimensionAttribute
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public DimensionAttribute DimensionAttribute { get; set; } = null !;
    public SlowlyChangingDimension SlowlyChangingDimension { get; set; } = null !;
}

public sealed partial class Warehouse
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

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

public static partial class MetaDataWarehouseInstance
{
    private static readonly MetaDataWarehouseModel _builtIn = CreateBuiltIn();
    public static MetaDataWarehouseModel BuiltIn => _builtIn;

    public static MetaDataWarehouseModel CreateBuiltIn()
    {
        var model = MetaDataWarehouseModel.CreateEmpty();
        return model;
    }
}