#nullable enable
using System;
using System.Collections.Generic;

namespace MetaAnalytics;
public sealed partial class AggregateFunction
{
    public string Id { get; set; } = null !;
}

public sealed partial class AnalyticsModel
{
    public string Id { get; set; } = null !;
    public string? DefaultCulture { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class Attribute
{
    public string Id { get; set; } = null !;
    public string? DataCategory { get; set; }
    public string DataTypeId { get; set; } = null !;
    public string? Description { get; set; }
    public string? FormatString { get; set; }
    public string? IsHidden { get; set; }
    public string? IsKey { get; set; }
    public string? IsNullable { get; set; }
    public string? Kind { get; set; }
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public string? SourceName { get; set; }
    public string? SummarizeBy { get; set; }
    public Table Table { get; set; } = null !;
}

public sealed partial class AttributePermission
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string MetadataPermission { get; set; } = null !;
    public Attribute Attribute { get; set; } = null !;
    public SecurityRole SecurityRole { get; set; } = null !;
}

public sealed partial class AttributeRelationship
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? RelationshipType { get; set; }
    public Attribute ChildAttribute { get; set; } = null !;
    public Attribute ParentAttribute { get; set; } = null !;
}

public sealed partial class AttributeTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public Attribute Attribute { get; set; } = null !;
    public Culture Culture { get; set; } = null !;
}

public sealed partial class AverageAggregateFunction
{
    public string Id { get; set; } = null !;
    public AggregateFunction AggregateFunction { get; set; } = null !;
}

public sealed partial class CountAggregateFunction
{
    public string Id { get; set; } = null !;
    public AggregateFunction AggregateFunction { get; set; } = null !;
}

public sealed partial class Culture
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public AnalyticsModel AnalyticsModel { get; set; } = null !;
}

public sealed partial class DataSource
{
    public string Id { get; set; } = null !;
    public string? ConnectionReference { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string? Provider { get; set; }
    public string? SourceKind { get; set; }
    public AnalyticsModel AnalyticsModel { get; set; } = null !;
}

public sealed partial class DistinctCountAggregateFunction
{
    public string Id { get; set; } = null !;
    public AggregateFunction AggregateFunction { get; set; } = null !;
}

public sealed partial class Hierarchy
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string? DisplayFolder { get; set; }
    public string? IsHidden { get; set; }
    public string? Kind { get; set; }
    public string Name { get; set; } = null !;
    public Table Table { get; set; } = null !;
}

public sealed partial class HierarchyLevel
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public Attribute Attribute { get; set; } = null !;
    public Hierarchy Hierarchy { get; set; } = null !;
}

public sealed partial class HierarchyTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public Culture Culture { get; set; } = null !;
    public Hierarchy Hierarchy { get; set; } = null !;
}

public sealed partial class MaximumAggregateFunction
{
    public string Id { get; set; } = null !;
    public AggregateFunction AggregateFunction { get; set; } = null !;
}

public sealed partial class Measure
{
    public string Id { get; set; } = null !;
    public string? DataTypeId { get; set; }
    public string? Description { get; set; }
    public string? DisplayFolder { get; set; }
    public string? FormatString { get; set; }
    public string? IsHidden { get; set; }
    public string Name { get; set; } = null !;
    public AggregateFunction AggregateFunction { get; set; } = null !;
    public Attribute SourceAttribute { get; set; } = null !;
    public Table Table { get; set; } = null !;
}

public sealed partial class MeasureTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public Culture Culture { get; set; } = null !;
    public Measure Measure { get; set; } = null !;
}

public sealed partial class MinimumAggregateFunction
{
    public string Id { get; set; } = null !;
    public AggregateFunction AggregateFunction { get; set; } = null !;
}

public sealed partial class Perspective
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public AnalyticsModel AnalyticsModel { get; set; } = null !;
}

public sealed partial class PerspectiveAttribute
{
    public string Id { get; set; } = null !;
    public Attribute Attribute { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
}

public sealed partial class PerspectiveHierarchy
{
    public string Id { get; set; } = null !;
    public Hierarchy Hierarchy { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
}

public sealed partial class PerspectiveMeasure
{
    public string Id { get; set; } = null !;
    public Measure Measure { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
}

public sealed partial class PerspectiveTable
{
    public string Id { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
    public Table Table { get; set; } = null !;
}

public sealed partial class PerspectiveTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public Culture Culture { get; set; } = null !;
    public Perspective Perspective { get; set; } = null !;
}

public sealed partial class Relationship
{
    public string Id { get; set; } = null !;
    public string Cardinality { get; set; } = null !;
    public string? CrossFilterDirection { get; set; }
    public string? Description { get; set; }
    public string? IsActive { get; set; }
    public string? IsRequired { get; set; }
    public string Name { get; set; } = null !;
    public string RelationshipKind { get; set; } = null !;
    public string? RoleName { get; set; }
    public Attribute FromAttribute { get; set; } = null !;
    public Table FromTable { get; set; } = null !;
    public Attribute? GranularityAttribute { get; set; }
    public Table? IntermediateTable { get; set; }
    public Attribute ToAttribute { get; set; } = null !;
    public Table ToTable { get; set; } = null !;
}

public sealed partial class RoleMember
{
    public string Id { get; set; } = null !;
    public string? MemberKind { get; set; }
    public string MemberName { get; set; } = null !;
    public SecurityRole SecurityRole { get; set; } = null !;
}

public sealed partial class SecurityRole
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public string Permission { get; set; } = null !;
    public AnalyticsModel AnalyticsModel { get; set; } = null !;
}

public sealed partial class SortByAttribute
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public Attribute SortAttribute { get; set; } = null !;
    public Attribute SourceAttribute { get; set; } = null !;
}

public sealed partial class SumAggregateFunction
{
    public string Id { get; set; } = null !;
    public AggregateFunction AggregateFunction { get; set; } = null !;
}

public sealed partial class Table
{
    public string Id { get; set; } = null !;
    public string? DataCategory { get; set; }
    public string? Description { get; set; }
    public string? DisplayFolder { get; set; }
    public string? IsHidden { get; set; }
    public string Kind { get; set; } = null !;
    public string Name { get; set; } = null !;
    public AnalyticsModel AnalyticsModel { get; set; } = null !;
}

public sealed partial class TablePermission
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string MetadataPermission { get; set; } = null !;
    public SecurityRole SecurityRole { get; set; } = null !;
    public Table Table { get; set; } = null !;
}

public sealed partial class TableTranslation
{
    public string Id { get; set; } = null !;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public Culture Culture { get; set; } = null !;
    public Table Table { get; set; } = null !;
}

public sealed partial class MetaAnalyticsModel
{
    public static MetaAnalyticsModel CreateEmpty() => new();
    public List<AggregateFunction> AggregateFunctionList { get; set; } = new();
    public List<AnalyticsModel> AnalyticsModelList { get; set; } = new();
    public List<Attribute> AttributeList { get; set; } = new();
    public List<AttributePermission> AttributePermissionList { get; set; } = new();
    public List<AttributeRelationship> AttributeRelationshipList { get; set; } = new();
    public List<AttributeTranslation> AttributeTranslationList { get; set; } = new();
    public List<AverageAggregateFunction> AverageAggregateFunctionList { get; set; } = new();
    public List<CountAggregateFunction> CountAggregateFunctionList { get; set; } = new();
    public List<Culture> CultureList { get; set; } = new();
    public List<DataSource> DataSourceList { get; set; } = new();
    public List<DistinctCountAggregateFunction> DistinctCountAggregateFunctionList { get; set; } = new();
    public List<Hierarchy> HierarchyList { get; set; } = new();
    public List<HierarchyLevel> HierarchyLevelList { get; set; } = new();
    public List<HierarchyTranslation> HierarchyTranslationList { get; set; } = new();
    public List<MaximumAggregateFunction> MaximumAggregateFunctionList { get; set; } = new();
    public List<Measure> MeasureList { get; set; } = new();
    public List<MeasureTranslation> MeasureTranslationList { get; set; } = new();
    public List<MinimumAggregateFunction> MinimumAggregateFunctionList { get; set; } = new();
    public List<Perspective> PerspectiveList { get; set; } = new();
    public List<PerspectiveAttribute> PerspectiveAttributeList { get; set; } = new();
    public List<PerspectiveHierarchy> PerspectiveHierarchyList { get; set; } = new();
    public List<PerspectiveMeasure> PerspectiveMeasureList { get; set; } = new();
    public List<PerspectiveTable> PerspectiveTableList { get; set; } = new();
    public List<PerspectiveTranslation> PerspectiveTranslationList { get; set; } = new();
    public List<Relationship> RelationshipList { get; set; } = new();
    public List<RoleMember> RoleMemberList { get; set; } = new();
    public List<SecurityRole> SecurityRoleList { get; set; } = new();
    public List<SortByAttribute> SortByAttributeList { get; set; } = new();
    public List<SumAggregateFunction> SumAggregateFunctionList { get; set; } = new();
    public List<Table> TableList { get; set; } = new();
    public List<TablePermission> TablePermissionList { get; set; } = new();
    public List<TableTranslation> TableTranslationList { get; set; } = new();
}

public static partial class MetaAnalyticsInstance
{
    private static readonly MetaAnalyticsModel _builtIn = CreateBuiltIn();
    public static MetaAnalyticsModel BuiltIn => _builtIn;

    public static MetaAnalyticsModel CreateBuiltIn()
    {
        var model = MetaAnalyticsModel.CreateEmpty();
        return model;
    }
}