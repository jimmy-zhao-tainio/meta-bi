#nullable enable

using System.Collections.Generic;

namespace MetaMultiDimensional;

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
