using AnalyticsAttribute = MetaAnalytics.Attribute;
using AnalyticsTable = MetaAnalytics.Table;
using MetaAnalytics;
using MetaTabular;

namespace MetaConvert.AnalyticsToTabular;

// Frozen imperative oracle retained only to verify sanctioned-weave equivalence.
internal static class AnalyticsToTabularCSharpReference
{
    public static MetaTabularModel Convert(MetaAnalyticsModel source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var target = MetaTabularModel.CreateEmpty();
        var aggregateFunctions = new AggregateFunctionIndex(source);
        var models = new Dictionary<AnalyticsModel, TabularModel>();
        var dataSources = new Dictionary<DataSource, TabularDataSource>();
        var tables = new Dictionary<AnalyticsTable, TabularTable>();
        var columns = new Dictionary<AnalyticsAttribute, TabularColumn>();
        var hierarchies = new Dictionary<Hierarchy, TabularHierarchy>();
        var measures = new Dictionary<Measure, TabularMeasure>();
        var perspectives = new Dictionary<Perspective, TabularPerspective>();
        var roles = new Dictionary<SecurityRole, TabularSecurityRole>();
        var cultures = new Dictionary<Culture, TabularCulture>();

        foreach (var row in source.AnalyticsModelList)
        {
            var converted = Add(target.TabularModelList, new TabularModel
            {
                Id = row.Id,
                Name = row.Name,
                DefaultCulture = row.DefaultCulture,
                CompatibilityLevel = "1500",
                Description = row.Description,
            });
            models[row] = converted;
        }

        foreach (var row in source.DataSourceList)
        {
            var converted = Add(target.TabularDataSourceList, new TabularDataSource
            {
                Id = row.Id,
                TabularModel = models[row.AnalyticsModel],
                Name = row.Name,
                Provider = row.Provider,
                ConnectionReference = row.ConnectionReference,
                Description = row.Description,
            });
            dataSources[row] = converted;
        }

        foreach (var row in source.TableList)
        {
            var converted = Add(target.TabularTableList, new TabularTable
            {
                Id = row.Id,
                TabularModel = models[row.AnalyticsModel],
                Name = row.Name,
                DataCategory = row.DataCategory,
                IsHidden = row.IsHidden,
                Description = row.Description,
            });
            tables[row] = converted;
        }

        foreach (var row in source.AttributeList)
        {
            var converted = Add(target.TabularColumnList, new TabularColumn
            {
                Id = row.Id,
                TabularTable = tables[row.Table],
                Name = row.Name,
                DataTypeId = row.DataTypeId,
                Ordinal = row.Ordinal,
                SourceName = row.SourceName,
                IsKey = row.IsKey,
                IsNullable = row.IsNullable,
                IsHidden = row.IsHidden,
                FormatString = row.FormatString,
                SummarizeBy = row.SummarizeBy,
                DataCategory = row.DataCategory,
                Description = row.Description,
            });
            columns[row] = converted;
        }

        foreach (var row in source.SortByAttributeList)
        {
            Add(target.TabularSortByColumnList, new TabularSortByColumn
            {
                Id = row.Id,
                SourceColumn = columns[row.SourceAttribute],
                SortColumn = columns[row.SortAttribute],
            });
        }

        foreach (var row in source.HierarchyList)
        {
            var converted = Add(target.TabularHierarchyList, new TabularHierarchy
            {
                Id = row.Id,
                TabularTable = tables[row.Table],
                Name = row.Name,
                IsHidden = row.IsHidden,
                DisplayFolder = row.DisplayFolder,
                Description = row.Description,
            });
            hierarchies[row] = converted;
        }

        foreach (var row in source.HierarchyLevelList)
        {
            Add(target.TabularHierarchyLevelList, new TabularHierarchyLevel
            {
                Id = row.Id,
                TabularHierarchy = hierarchies[row.Hierarchy],
                TabularColumn = columns[row.Attribute],
                Name = row.Name,
                Ordinal = row.Ordinal,
            });
        }

        foreach (var row in source.RelationshipList)
        {
            Add(target.TabularRelationshipList, new TabularRelationship
            {
                Id = row.Id,
                FromTable = tables[row.FromTable],
                FromColumn = columns[row.FromAttribute],
                ToTable = tables[row.ToTable],
                ToColumn = columns[row.ToAttribute],
                Name = row.Name,
                Cardinality = row.Cardinality,
                CrossFilterDirection = row.CrossFilterDirection,
                IsActive = row.IsActive,
                IsRequired = row.IsRequired,
            });
        }

        foreach (var row in source.MeasureList)
        {
            var expression = BuildDaxMeasureExpression(row, aggregateFunctions);
            var converted = Add(target.TabularMeasureList, new TabularMeasure
            {
                Id = row.Id,
                TabularTable = tables[row.Table],
                Name = row.Name,
                Expression = expression,
                FormatString = row.FormatString,
                DisplayFolder = row.DisplayFolder,
                IsHidden = row.IsHidden,
                Description = row.Description,
            });
            measures[row] = converted;
        }

        foreach (var row in source.PerspectiveList)
        {
            var converted = Add(target.TabularPerspectiveList, new TabularPerspective
            {
                Id = row.Id,
                TabularModel = models[row.AnalyticsModel],
                Name = row.Name,
                Description = row.Description,
            });
            perspectives[row] = converted;
        }

        foreach (var row in source.PerspectiveTableList) Add(target.TabularPerspectiveTableList, new TabularPerspectiveTable { Id = row.Id, TabularPerspective = perspectives[row.Perspective], TabularTable = tables[row.Table] });
        foreach (var row in source.PerspectiveAttributeList) Add(target.TabularPerspectiveColumnList, new TabularPerspectiveColumn { Id = row.Id, TabularPerspective = perspectives[row.Perspective], TabularColumn = columns[row.Attribute] });
        foreach (var row in source.PerspectiveHierarchyList) Add(target.TabularPerspectiveHierarchyList, new TabularPerspectiveHierarchy { Id = row.Id, TabularPerspective = perspectives[row.Perspective], TabularHierarchy = hierarchies[row.Hierarchy] });
        foreach (var row in source.PerspectiveMeasureList) Add(target.TabularPerspectiveMeasureList, new TabularPerspectiveMeasure { Id = row.Id, TabularPerspective = perspectives[row.Perspective], TabularMeasure = measures[row.Measure] });

        foreach (var row in source.SecurityRoleList)
        {
            var converted = Add(target.TabularSecurityRoleList, new TabularSecurityRole
            {
                Id = row.Id,
                TabularModel = models[row.AnalyticsModel],
                Name = row.Name,
                Permission = row.Permission,
                Description = row.Description,
            });
            roles[row] = converted;
        }

        foreach (var row in source.RoleMemberList) Add(target.TabularRoleMemberList, new TabularRoleMember { Id = row.Id, TabularSecurityRole = roles[row.SecurityRole], MemberName = row.MemberName });
        foreach (var row in source.TablePermissionList) Add(target.TabularTablePermissionList, new TabularTablePermission { Id = row.Id, TabularSecurityRole = roles[row.SecurityRole], TabularTable = tables[row.Table], MetadataPermission = row.MetadataPermission });
        foreach (var row in source.AttributePermissionList) Add(target.TabularColumnPermissionList, new TabularColumnPermission { Id = row.Id, TabularSecurityRole = roles[row.SecurityRole], TabularColumn = columns[row.Attribute], MetadataPermission = row.MetadataPermission });

        foreach (var row in source.CultureList)
        {
            var converted = Add(target.TabularCultureList, new TabularCulture
            {
                Id = row.Id,
                TabularModel = models[row.AnalyticsModel],
                Name = row.Name,
            });
            cultures[row] = converted;
        }

        foreach (var row in source.TableTranslationList) Add(target.TabularTableTranslationList, new TabularTableTranslation { Id = row.Id, TabularCulture = cultures[row.Culture], TabularTable = tables[row.Table], Caption = row.Caption, Description = row.Description });
        foreach (var row in source.AttributeTranslationList) Add(target.TabularColumnTranslationList, new TabularColumnTranslation { Id = row.Id, TabularCulture = cultures[row.Culture], TabularColumn = columns[row.Attribute], Caption = row.Caption, Description = row.Description });
        foreach (var row in source.HierarchyTranslationList) Add(target.TabularHierarchyTranslationList, new TabularHierarchyTranslation { Id = row.Id, TabularCulture = cultures[row.Culture], TabularHierarchy = hierarchies[row.Hierarchy], Caption = row.Caption, Description = row.Description });
        foreach (var row in source.MeasureTranslationList) Add(target.TabularMeasureTranslationList, new TabularMeasureTranslation { Id = row.Id, TabularCulture = cultures[row.Culture], TabularMeasure = measures[row.Measure], Caption = row.Caption, Description = row.Description });
        foreach (var row in source.PerspectiveTranslationList) Add(target.TabularPerspectiveTranslationList, new TabularPerspectiveTranslation { Id = row.Id, TabularCulture = cultures[row.Culture], TabularPerspective = perspectives[row.Perspective], Caption = row.Caption, Description = row.Description });

        _ = dataSources;
        return target;
    }

    private static string BuildDaxMeasureExpression(
        Measure measure,
        AggregateFunctionIndex aggregateFunctions)
    {
        var functionName = aggregateFunctions.ToDaxFunction(measure);
        return $"{functionName}({DaxIdentifier(measure.Table.Name)}[{DaxColumnIdentifier(measure.SourceAttribute.Name)}])";
    }

    private static string DaxIdentifier(string value)
    {
        return "'" + value.Replace("'", "''", StringComparison.Ordinal) + "'";
    }

    private static string DaxColumnIdentifier(string value)
    {
        return value.Replace("]", "]]", StringComparison.Ordinal);
    }

    private static T Add<T>(ICollection<T> rows, T row)
    {
        rows.Add(row);
        return row;
    }

    private sealed class AggregateFunctionIndex
    {
        private readonly HashSet<AggregateFunction> sums;
        private readonly HashSet<AggregateFunction> averages;
        private readonly HashSet<AggregateFunction> counts;
        private readonly HashSet<AggregateFunction> distinctCounts;
        private readonly HashSet<AggregateFunction> minimums;
        private readonly HashSet<AggregateFunction> maximums;

        public AggregateFunctionIndex(MetaAnalyticsModel model)
        {
            sums = new HashSet<AggregateFunction>(model.SumAggregateFunctionList.Select(row => row.AggregateFunction), ReferenceEqualityComparer.Instance);
            averages = new HashSet<AggregateFunction>(model.AverageAggregateFunctionList.Select(row => row.AggregateFunction), ReferenceEqualityComparer.Instance);
            counts = new HashSet<AggregateFunction>(model.CountAggregateFunctionList.Select(row => row.AggregateFunction), ReferenceEqualityComparer.Instance);
            distinctCounts = new HashSet<AggregateFunction>(model.DistinctCountAggregateFunctionList.Select(row => row.AggregateFunction), ReferenceEqualityComparer.Instance);
            minimums = new HashSet<AggregateFunction>(model.MinimumAggregateFunctionList.Select(row => row.AggregateFunction), ReferenceEqualityComparer.Instance);
            maximums = new HashSet<AggregateFunction>(model.MaximumAggregateFunctionList.Select(row => row.AggregateFunction), ReferenceEqualityComparer.Instance);
        }

        public string ToDaxFunction(Measure measure)
        {
            var matches = new List<string>(capacity: 1);
            AddIf(matches, sums, measure.AggregateFunction, "SUM");
            AddIf(matches, averages, measure.AggregateFunction, "AVERAGE");
            AddIf(matches, counts, measure.AggregateFunction, "COUNT");
            AddIf(matches, distinctCounts, measure.AggregateFunction, "DISTINCTCOUNT");
            AddIf(matches, minimums, measure.AggregateFunction, "MIN");
            AddIf(matches, maximums, measure.AggregateFunction, "MAX");
            if (matches.Count != 1)
            {
                throw new InvalidOperationException(
                    $"Measure '{measure.Id}' must reference one concrete aggregate-function entity; found {matches.Count}.");
            }

            return matches[0];
        }

        private static void AddIf(
            ICollection<string> matches,
            IReadOnlySet<AggregateFunction> functions,
            AggregateFunction candidate,
            string targetName)
        {
            if (functions.Contains(candidate))
            {
                matches.Add(targetName);
            }
        }
    }
}
