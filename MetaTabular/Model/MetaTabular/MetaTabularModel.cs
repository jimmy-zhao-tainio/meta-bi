#nullable enable

using System.Collections.Generic;

namespace MetaTabular
{
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
}
