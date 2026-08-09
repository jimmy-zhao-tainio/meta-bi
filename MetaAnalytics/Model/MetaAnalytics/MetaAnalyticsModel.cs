#nullable enable

using System.Collections.Generic;

namespace MetaAnalytics
{
    public sealed partial class MetaAnalyticsModel
    {
        public static MetaAnalyticsModel CreateEmpty() => new();

        public List<AggregationBehavior> AggregationBehaviorList { get; set; } = new();
        public List<AnalyticsModel> AnalyticsModelList { get; set; } = new();
        public List<Attribute> AttributeList { get; set; } = new();
        public List<AttributePermission> AttributePermissionList { get; set; } = new();
        public List<AttributeRelationship> AttributeRelationshipList { get; set; } = new();
        public List<AttributeTranslation> AttributeTranslationList { get; set; } = new();
        public List<Culture> CultureList { get; set; } = new();
        public List<DataSource> DataSourceList { get; set; } = new();
        public List<Hierarchy> HierarchyList { get; set; } = new();
        public List<HierarchyLevel> HierarchyLevelList { get; set; } = new();
        public List<HierarchyTranslation> HierarchyTranslationList { get; set; } = new();
        public List<Measure> MeasureList { get; set; } = new();
        public List<MeasureTranslation> MeasureTranslationList { get; set; } = new();
        public List<Perspective> PerspectiveList { get; set; } = new();
        public List<PerspectiveAttribute> PerspectiveAttributeList { get; set; } = new();
        public List<PerspectiveHierarchy> PerspectiveHierarchyList { get; set; } = new();
        public List<PerspectiveMeasure> PerspectiveMeasureList { get; set; } = new();
        public List<PerspectiveTable> PerspectiveTableList { get; set; } = new();
        public List<PerspectiveTranslation> PerspectiveTranslationList { get; set; } = new();
        public List<Relationship> RelationshipList { get; set; } = new();
        public List<RoleFilter> RoleFilterList { get; set; } = new();
        public List<RoleMember> RoleMemberList { get; set; } = new();
        public List<SecurityRole> SecurityRoleList { get; set; } = new();
        public List<SortByAttribute> SortByAttributeList { get; set; } = new();
        public List<Table> TableList { get; set; } = new();
        public List<TablePermission> TablePermissionList { get; set; } = new();
        public List<TableTranslation> TableTranslationList { get; set; } = new();
    }
}
