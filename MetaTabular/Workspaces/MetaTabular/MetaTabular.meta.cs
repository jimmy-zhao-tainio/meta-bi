#nullable enable
using System;
using System.Collections.Generic;

namespace MetaTabular;
public sealed partial class TabularCalculationGroup
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string Precedence { get; set; } = null !;
    public TabularModel TabularModel { get; set; } = null !;
}

public sealed partial class TabularCalculationItem
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Expression { get; set; } = null !;
    public string? FormatStringExpression { get; set; }
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public TabularCalculationGroup TabularCalculationGroup { get; set; } = null !;
}

public sealed partial class TabularColumn
{
    public string Id { get; set; } = null !;
    public string? DataCategory { get; set; }
    public string DataTypeId { get; set; } = null !;
    public string? Description { get; set; }
    public string? Expression { get; set; }
    public string? FormatString { get; set; }
    public string? IsHidden { get; set; }
    public string? IsKey { get; set; }
    public string? IsNullable { get; set; }
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public string? SourceName { get; set; }
    public string? SummarizeBy { get; set; }
    public TabularTable TabularTable { get; set; } = null !;
}

public sealed partial class TabularColumnPermission
{
    public string Id { get; set; } = null !;
    public string MetadataPermission { get; set; } = null !;
    public TabularColumn TabularColumn { get; set; } = null !;
    public TabularSecurityRole TabularSecurityRole { get; set; } = null !;
}

public sealed partial class TabularColumnTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public TabularColumn TabularColumn { get; set; } = null !;
    public TabularCulture TabularCulture { get; set; } = null !;
}

public sealed partial class TabularCulture
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public TabularModel TabularModel { get; set; } = null !;
}

public sealed partial class TabularDataSource
{
    public string Id { get; set; } = null !;
    public string? ConnectionReference { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string? Provider { get; set; }
    public TabularModel TabularModel { get; set; } = null !;
}

public sealed partial class TabularHierarchy
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? DisplayFolder { get; set; }
    public string? IsHidden { get; set; }
    public string Name { get; set; } = null !;
    public TabularTable TabularTable { get; set; } = null !;
}

public sealed partial class TabularHierarchyLevel
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public TabularColumn TabularColumn { get; set; } = null !;
    public TabularHierarchy TabularHierarchy { get; set; } = null !;
}

public sealed partial class TabularHierarchyTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public TabularCulture TabularCulture { get; set; } = null !;
    public TabularHierarchy TabularHierarchy { get; set; } = null !;
}

public sealed partial class TabularKpi
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? StatusExpression { get; set; }
    public string? StatusGraphic { get; set; }
    public string? TargetExpression { get; set; }
    public string? TrendExpression { get; set; }
    public string? TrendGraphic { get; set; }
    public TabularMeasure BaseMeasure { get; set; } = null !;
    public TabularMeasure? TargetMeasure { get; set; }
}

public sealed partial class TabularKpiTranslation
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public TabularCulture TabularCulture { get; set; } = null !;
    public TabularKpi TabularKpi { get; set; } = null !;
}

public sealed partial class TabularMeasure
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? DisplayFolder { get; set; }
    public string? Expression { get; set; }
    public string? FormatString { get; set; }
    public string? IsHidden { get; set; }
    public string Name { get; set; } = null !;
    public TabularTable TabularTable { get; set; } = null !;
}

public sealed partial class TabularMeasureTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public TabularCulture TabularCulture { get; set; } = null !;
    public TabularMeasure TabularMeasure { get; set; } = null !;
}

public sealed partial class TabularModel
{
    public string Id { get; set; } = null !;
    public string? Collation { get; set; }
    public string? CompatibilityLevel { get; set; }
    public string? DefaultCulture { get; set; }
    public string? DefaultDataView { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class TabularPartition
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? Expression { get; set; }
    public string? Mode { get; set; }
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public TabularDataSource? TabularDataSource { get; set; }
    public TabularTable TabularTable { get; set; } = null !;
}

public sealed partial class TabularPerspective
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public TabularModel TabularModel { get; set; } = null !;
}

public sealed partial class TabularPerspectiveCalculationGroup
{
    public string Id { get; set; } = null !;
    public TabularCalculationGroup TabularCalculationGroup { get; set; } = null !;
    public TabularPerspective TabularPerspective { get; set; } = null !;
}

public sealed partial class TabularPerspectiveColumn
{
    public string Id { get; set; } = null !;
    public TabularColumn TabularColumn { get; set; } = null !;
    public TabularPerspective TabularPerspective { get; set; } = null !;
}

public sealed partial class TabularPerspectiveHierarchy
{
    public string Id { get; set; } = null !;
    public TabularHierarchy TabularHierarchy { get; set; } = null !;
    public TabularPerspective TabularPerspective { get; set; } = null !;
}

public sealed partial class TabularPerspectiveKpi
{
    public string Id { get; set; } = null !;
    public TabularKpi TabularKpi { get; set; } = null !;
    public TabularPerspective TabularPerspective { get; set; } = null !;
}

public sealed partial class TabularPerspectiveMeasure
{
    public string Id { get; set; } = null !;
    public TabularMeasure TabularMeasure { get; set; } = null !;
    public TabularPerspective TabularPerspective { get; set; } = null !;
}

public sealed partial class TabularPerspectiveTable
{
    public string Id { get; set; } = null !;
    public TabularPerspective TabularPerspective { get; set; } = null !;
    public TabularTable TabularTable { get; set; } = null !;
}

public sealed partial class TabularPerspectiveTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public TabularCulture TabularCulture { get; set; } = null !;
    public TabularPerspective TabularPerspective { get; set; } = null !;
}

public sealed partial class TabularRelationship
{
    public string Id { get; set; } = null !;
    public string Cardinality { get; set; } = null !;
    public string? CrossFilterDirection { get; set; }
    public string? IsActive { get; set; }
    public string? IsRequired { get; set; }
    public string Name { get; set; } = null !;
    public TabularColumn FromColumn { get; set; } = null !;
    public TabularTable FromTable { get; set; } = null !;
    public TabularColumn ToColumn { get; set; } = null !;
    public TabularTable ToTable { get; set; } = null !;
}

public sealed partial class TabularRoleFilter
{
    public string Id { get; set; } = null !;
    public string Expression { get; set; } = null !;
    public TabularSecurityRole TabularSecurityRole { get; set; } = null !;
    public TabularTable TabularTable { get; set; } = null !;
}

public sealed partial class TabularRoleMember
{
    public string Id { get; set; } = null !;
    public string? MemberId { get; set; }
    public string MemberName { get; set; } = null !;
    public TabularSecurityRole TabularSecurityRole { get; set; } = null !;
}

public sealed partial class TabularSecurityRole
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string Permission { get; set; } = null !;
    public TabularModel TabularModel { get; set; } = null !;
}

public sealed partial class TabularSortByColumn
{
    public string Id { get; set; } = null !;
    public TabularColumn SortColumn { get; set; } = null !;
    public TabularColumn SourceColumn { get; set; } = null !;
}

public sealed partial class TabularTable
{
    public string Id { get; set; } = null !;
    public string? DataCategory { get; set; }
    public string? Description { get; set; }
    public string? IsHidden { get; set; }
    public string Name { get; set; } = null !;
    public TabularModel TabularModel { get; set; } = null !;
}

public sealed partial class TabularTablePermission
{
    public string Id { get; set; } = null !;
    public string MetadataPermission { get; set; } = null !;
    public TabularSecurityRole TabularSecurityRole { get; set; } = null !;
    public TabularTable TabularTable { get; set; } = null !;
}

public sealed partial class TabularTableTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public TabularCulture TabularCulture { get; set; } = null !;
    public TabularTable TabularTable { get; set; } = null !;
}

public sealed partial class MetaTabularModel
{
    public static MetaTabularModel CreateEmpty() => new();
    public List<TabularCalculationGroup> TabularCalculationGroupList { get; set; } = new();
    public List<TabularCalculationItem> TabularCalculationItemList { get; set; } = new();
    public List<TabularColumn> TabularColumnList { get; set; } = new();
    public List<TabularColumnPermission> TabularColumnPermissionList { get; set; } = new();
    public List<TabularColumnTranslation> TabularColumnTranslationList { get; set; } = new();
    public List<TabularCulture> TabularCultureList { get; set; } = new();
    public List<TabularDataSource> TabularDataSourceList { get; set; } = new();
    public List<TabularHierarchy> TabularHierarchyList { get; set; } = new();
    public List<TabularHierarchyLevel> TabularHierarchyLevelList { get; set; } = new();
    public List<TabularHierarchyTranslation> TabularHierarchyTranslationList { get; set; } = new();
    public List<TabularKpi> TabularKpiList { get; set; } = new();
    public List<TabularKpiTranslation> TabularKpiTranslationList { get; set; } = new();
    public List<TabularMeasure> TabularMeasureList { get; set; } = new();
    public List<TabularMeasureTranslation> TabularMeasureTranslationList { get; set; } = new();
    public List<TabularModel> TabularModelList { get; set; } = new();
    public List<TabularPartition> TabularPartitionList { get; set; } = new();
    public List<TabularPerspective> TabularPerspectiveList { get; set; } = new();
    public List<TabularPerspectiveCalculationGroup> TabularPerspectiveCalculationGroupList { get; set; } = new();
    public List<TabularPerspectiveColumn> TabularPerspectiveColumnList { get; set; } = new();
    public List<TabularPerspectiveHierarchy> TabularPerspectiveHierarchyList { get; set; } = new();
    public List<TabularPerspectiveKpi> TabularPerspectiveKpiList { get; set; } = new();
    public List<TabularPerspectiveMeasure> TabularPerspectiveMeasureList { get; set; } = new();
    public List<TabularPerspectiveTable> TabularPerspectiveTableList { get; set; } = new();
    public List<TabularPerspectiveTranslation> TabularPerspectiveTranslationList { get; set; } = new();
    public List<TabularRelationship> TabularRelationshipList { get; set; } = new();
    public List<TabularRoleFilter> TabularRoleFilterList { get; set; } = new();
    public List<TabularRoleMember> TabularRoleMemberList { get; set; } = new();
    public List<TabularSecurityRole> TabularSecurityRoleList { get; set; } = new();
    public List<TabularSortByColumn> TabularSortByColumnList { get; set; } = new();
    public List<TabularTable> TabularTableList { get; set; } = new();
    public List<TabularTablePermission> TabularTablePermissionList { get; set; } = new();
    public List<TabularTableTranslation> TabularTableTranslationList { get; set; } = new();
}

public static partial class MetaTabularInstance
{
    private static readonly MetaTabularModel _builtIn = CreateBuiltIn();
    public static MetaTabularModel BuiltIn => _builtIn;

    public static MetaTabularModel CreateBuiltIn()
    {
        var model = MetaTabularModel.CreateEmpty();
        return model;
    }
}