#nullable enable
using System;
using System.Collections.Generic;

namespace MetaMultiDimensional;
public sealed partial class ActionTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public CubeAction CubeAction { get; set; } = null !;
    public Culture Culture { get; set; } = null !;
}

public sealed partial class AttributeRelationship
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? RelationshipType { get; set; }
    public DimensionAttribute ChildAttribute { get; set; } = null !;
    public DimensionAttribute ParentAttribute { get; set; } = null !;
}

public sealed partial class AttributeTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public Culture Culture { get; set; } = null !;
    public DimensionAttribute DimensionAttribute { get; set; } = null !;
}

public sealed partial class CellPermission
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Expression { get; set; } = null !;
    public Cube Cube { get; set; } = null !;
    public SecurityRole SecurityRole { get; set; } = null !;
}

public sealed partial class Cube
{
    public string Id { get; set; } = null !;
    public string? DefaultMeasureName { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string? ProcessingMode { get; set; }
    public string? StorageMode { get; set; }
    public MultiDimensionalDatabase MultiDimensionalDatabase { get; set; } = null !;
}

public sealed partial class CubeAction
{
    public string Id { get; set; } = null !;
    public string ActionType { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public string Expression { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string? Target { get; set; }
    public string TargetKind { get; set; } = null !;
    public Cube Cube { get; set; } = null !;
}

public sealed partial class CubeDimension
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string? RoleName { get; set; }
    public Cube Cube { get; set; } = null !;
    public Dimension Dimension { get; set; } = null !;
}

public sealed partial class CubeTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public Cube Cube { get; set; } = null !;
    public Culture Culture { get; set; } = null !;
}

public sealed partial class Culture
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? LanguageId { get; set; }
    public string Name { get; set; } = null !;
    public MultiDimensionalDatabase MultiDimensionalDatabase { get; set; } = null !;
}

public sealed partial class Dimension
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? DimensionType { get; set; }
    public string Name { get; set; } = null !;
    public string? ProcessingGroup { get; set; }
    public string? ProcessingMode { get; set; }
    public string? SourceName { get; set; }
    public string? StorageMode { get; set; }
    public MultiDimensionalDatabase MultiDimensionalDatabase { get; set; } = null !;
}

public sealed partial class DimensionAttribute
{
    public string Id { get; set; } = null !;
    public string? AttributeHierarchyEnabled { get; set; }
    public string? AttributeHierarchyVisible { get; set; }
    public string DataTypeId { get; set; } = null !;
    public string? Description { get; set; }
    public string? IsKey { get; set; }
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public string? SourceName { get; set; }
    public string? Usage { get; set; }
    public Dimension Dimension { get; set; } = null !;
}

public sealed partial class DimensionHierarchy
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? HierarchyType { get; set; }
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

public sealed partial class DimensionPermission
{
    public string Id { get; set; } = null !;
    public string? AllowedSetExpression { get; set; }
    public string? DefaultMemberExpression { get; set; }
    public string? DeniedSetExpression { get; set; }
    public string? Description { get; set; }
    public string? VisualTotals { get; set; }
    public DimensionAttribute DimensionAttribute { get; set; } = null !;
    public Dimension Dimension { get; set; } = null !;
    public SecurityRole SecurityRole { get; set; } = null !;
}

public sealed partial class DimensionTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public Culture Culture { get; set; } = null !;
    public Dimension Dimension { get; set; } = null !;
}

public sealed partial class DimensionUsage
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? IsRequired { get; set; }
    public string? RoleName { get; set; }
    public string UsageKind { get; set; } = null !;
    public CubeDimension CubeDimension { get; set; } = null !;
    public DimensionAttribute? GranularityAttribute { get; set; }
    public MeasureGroup? IntermediateMeasureGroup { get; set; }
    public MeasureGroup MeasureGroup { get; set; } = null !;
}

public sealed partial class Kpi
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? GoalExpression { get; set; }
    public string Name { get; set; } = null !;
    public string? StatusExpression { get; set; }
    public string? StatusGraphic { get; set; }
    public string? TrendExpression { get; set; }
    public string? TrendGraphic { get; set; }
    public string? ValueExpression { get; set; }
    public Measure? AssociatedMeasure { get; set; }
    public Cube Cube { get; set; } = null !;
}

public sealed partial class KpiTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public Culture Culture { get; set; } = null !;
    public Kpi Kpi { get; set; } = null !;
}

public sealed partial class MdxCalculation
{
    public string Id { get; set; } = null !;
    public string CalculationKind { get; set; } = null !;
    public string? Description { get; set; }
    public string? DisplayFolder { get; set; }
    public string Expression { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string? SolveOrder { get; set; }
    public Cube Cube { get; set; } = null !;
}

public sealed partial class Measure
{
    public string Id { get; set; } = null !;
    public string? AggregateFunction { get; set; }
    public string? DataTypeId { get; set; }
    public string? Description { get; set; }
    public string? DisplayFolder { get; set; }
    public string? FormatString { get; set; }
    public string Name { get; set; } = null !;
    public string? SourceName { get; set; }
    public MeasureGroup MeasureGroup { get; set; } = null !;
}

public sealed partial class MeasureGroup
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string? ProcessingMode { get; set; }
    public string? SourceName { get; set; }
    public string? StorageMode { get; set; }
    public Cube Cube { get; set; } = null !;
}

public sealed partial class MeasureTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public Culture Culture { get; set; } = null !;
    public Measure Measure { get; set; } = null !;
}

public sealed partial class MultiDimensionalDatabase
{
    public string Id { get; set; } = null !;
    public string? Collation { get; set; }
    public string? DefaultLanguage { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class MultiDimensionalDataSource
{
    public string Id { get; set; } = null !;
    public string? ConnectionReference { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string? Provider { get; set; }
    public string? SourceKind { get; set; }
    public MultiDimensionalDatabase MultiDimensionalDatabase { get; set; } = null !;
}

public sealed partial class NamedSet
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? DisplayFolder { get; set; }
    public string Expression { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Cube Cube { get; set; } = null !;
}

public sealed partial class NamedSetTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public Culture Culture { get; set; } = null !;
    public NamedSet NamedSet { get; set; } = null !;
}

public sealed partial class Partition
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public string? ProcessingMode { get; set; }
    public string? SliceExpression { get; set; }
    public string? SourceExpression { get; set; }
    public string? StorageMode { get; set; }
    public MeasureGroup MeasureGroup { get; set; } = null !;
    public MultiDimensionalDataSource? MultiDimensionalDataSource { get; set; }
}

public sealed partial class Perspective
{
    public string Id { get; set; } = null !;
    public string? DefaultMeasureName { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public Cube Cube { get; set; } = null !;
}

public sealed partial class PerspectiveAction
{
    public string Id { get; set; } = null !;
    public CubeAction CubeAction { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
}

public sealed partial class PerspectiveCalculation
{
    public string Id { get; set; } = null !;
    public MdxCalculation MdxCalculation { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
}

public sealed partial class PerspectiveDimension
{
    public string Id { get; set; } = null !;
    public CubeDimension CubeDimension { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
}

public sealed partial class PerspectiveKpi
{
    public string Id { get; set; } = null !;
    public Kpi Kpi { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
}

public sealed partial class PerspectiveMeasure
{
    public string Id { get; set; } = null !;
    public Measure Measure { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
}

public sealed partial class PerspectiveMeasureGroup
{
    public string Id { get; set; } = null !;
    public MeasureGroup MeasureGroup { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
}

public sealed partial class PerspectiveNamedSet
{
    public string Id { get; set; } = null !;
    public NamedSet NamedSet { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
}

public sealed partial class PerspectiveTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public Culture Culture { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
}

public sealed partial class RoleMember
{
    public string Id { get; set; } = null !;
    public string MemberName { get; set; } = null !;
    public string? MemberSid { get; set; }
    public SecurityRole SecurityRole { get; set; } = null !;
}

public sealed partial class SecurityRole
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string Permission { get; set; } = null !;
    public MultiDimensionalDatabase MultiDimensionalDatabase { get; set; } = null !;
}

public sealed partial class MetaMultiDimensionalModel
{
    public static MetaMultiDimensionalModel CreateEmpty() => new();
    public List<ActionTranslation> ActionTranslationList { get; set; } = new();
    public List<AttributeRelationship> AttributeRelationshipList { get; set; } = new();
    public List<AttributeTranslation> AttributeTranslationList { get; set; } = new();
    public List<CellPermission> CellPermissionList { get; set; } = new();
    public List<Cube> CubeList { get; set; } = new();
    public List<CubeAction> CubeActionList { get; set; } = new();
    public List<CubeDimension> CubeDimensionList { get; set; } = new();
    public List<CubeTranslation> CubeTranslationList { get; set; } = new();
    public List<Culture> CultureList { get; set; } = new();
    public List<Dimension> DimensionList { get; set; } = new();
    public List<DimensionAttribute> DimensionAttributeList { get; set; } = new();
    public List<DimensionHierarchy> DimensionHierarchyList { get; set; } = new();
    public List<DimensionHierarchyLevel> DimensionHierarchyLevelList { get; set; } = new();
    public List<DimensionPermission> DimensionPermissionList { get; set; } = new();
    public List<DimensionTranslation> DimensionTranslationList { get; set; } = new();
    public List<DimensionUsage> DimensionUsageList { get; set; } = new();
    public List<Kpi> KpiList { get; set; } = new();
    public List<KpiTranslation> KpiTranslationList { get; set; } = new();
    public List<MdxCalculation> MdxCalculationList { get; set; } = new();
    public List<Measure> MeasureList { get; set; } = new();
    public List<MeasureGroup> MeasureGroupList { get; set; } = new();
    public List<MeasureTranslation> MeasureTranslationList { get; set; } = new();
    public List<MultiDimensionalDatabase> MultiDimensionalDatabaseList { get; set; } = new();
    public List<MultiDimensionalDataSource> MultiDimensionalDataSourceList { get; set; } = new();
    public List<NamedSet> NamedSetList { get; set; } = new();
    public List<NamedSetTranslation> NamedSetTranslationList { get; set; } = new();
    public List<Partition> PartitionList { get; set; } = new();
    public List<Perspective> PerspectiveList { get; set; } = new();
    public List<PerspectiveAction> PerspectiveActionList { get; set; } = new();
    public List<PerspectiveCalculation> PerspectiveCalculationList { get; set; } = new();
    public List<PerspectiveDimension> PerspectiveDimensionList { get; set; } = new();
    public List<PerspectiveKpi> PerspectiveKpiList { get; set; } = new();
    public List<PerspectiveMeasure> PerspectiveMeasureList { get; set; } = new();
    public List<PerspectiveMeasureGroup> PerspectiveMeasureGroupList { get; set; } = new();
    public List<PerspectiveNamedSet> PerspectiveNamedSetList { get; set; } = new();
    public List<PerspectiveTranslation> PerspectiveTranslationList { get; set; } = new();
    public List<RoleMember> RoleMemberList { get; set; } = new();
    public List<SecurityRole> SecurityRoleList { get; set; } = new();
}

public static partial class MetaMultiDimensionalInstance
{
    private static readonly MetaMultiDimensionalModel _builtIn = CreateBuiltIn();
    public static MetaMultiDimensionalModel BuiltIn => _builtIn;

    public static MetaMultiDimensionalModel CreateBuiltIn()
    {
        var model = MetaMultiDimensionalModel.CreateEmpty();
        return model;
    }
}