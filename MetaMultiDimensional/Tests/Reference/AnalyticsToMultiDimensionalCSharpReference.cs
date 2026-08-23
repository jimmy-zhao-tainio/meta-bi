using AnalyticsAttribute = MetaAnalytics.Attribute;
using AnalyticsCulture = MetaAnalytics.Culture;
using AnalyticsDataSource = MetaAnalytics.DataSource;
using AnalyticsHierarchy = MetaAnalytics.Hierarchy;
using AnalyticsMeasure = MetaAnalytics.Measure;
using AnalyticsModel = MetaAnalytics.AnalyticsModel;
using AnalyticsPerspective = MetaAnalytics.Perspective;
using AnalyticsSecurityRole = MetaAnalytics.SecurityRole;
using AnalyticsTable = MetaAnalytics.Table;
using MultiAttributeRelationship = MetaMultiDimensional.AttributeRelationship;
using MultiCulture = MetaMultiDimensional.Culture;
using MultiDimensionAttribute = MetaMultiDimensional.DimensionAttribute;
using MultiMeasure = MetaMultiDimensional.Measure;
using MultiPerspective = MetaMultiDimensional.Perspective;
using MultiRoleMember = MetaMultiDimensional.RoleMember;
using MultiSecurityRole = MetaMultiDimensional.SecurityRole;
using MultiMeasureTranslation = MetaMultiDimensional.MeasureTranslation;
using MetaMultiDimensional;

namespace MetaConvert.AnalyticsToMultiDimensional;

// Frozen imperative oracle retained only to verify sanctioned-weave equivalence.
internal static class AnalyticsToMultiDimensionalCSharpReference
{
    public static MetaMultiDimensionalModel Convert(MetaAnalytics.MetaAnalyticsModel source)
    {
        ArgumentNullException.ThrowIfNull(source);
        RejectUnsupportedSecurity(source);

        var target = MetaMultiDimensionalModel.CreateEmpty();
        var aggregateFunctions = new AggregateFunctionIndex(source);
        var databases = new Dictionary<AnalyticsModel, MultiDimensionalDatabase>();
        var cubes = new Dictionary<AnalyticsModel, Cube>();
        var dataSources = new Dictionary<AnalyticsDataSource, MultiDimensionalDataSource>();
        var dimensions = new Dictionary<AnalyticsTable, Dimension>();
        var cubeDimensions = new Dictionary<AnalyticsTable, CubeDimension>();
        var dimensionAttributes = new Dictionary<AnalyticsAttribute, MultiDimensionAttribute>();
        var hierarchies = new Dictionary<AnalyticsHierarchy, DimensionHierarchy>();
        var measureGroups = new Dictionary<AnalyticsTable, MeasureGroup>();
        var measures = new Dictionary<AnalyticsMeasure, MultiMeasure>();
        var perspectives = new Dictionary<AnalyticsPerspective, MultiPerspective>();
        var roles = new Dictionary<AnalyticsSecurityRole, MultiSecurityRole>();
        var cultures = new Dictionary<AnalyticsCulture, MultiCulture>();

        var factTables = source.MeasureList
            .Select(row => row.Table)
            .Distinct()
            .ToHashSet();

        foreach (var row in source.AnalyticsModelList)
        {
            var database = Add(target.MultiDimensionalDatabaseList, new MultiDimensionalDatabase
            {
                Id = row.Id,
                Name = row.Name,
                DefaultLanguage = row.DefaultCulture,
                Description = row.Description,
            });
            var cube = Add(target.CubeList, new Cube
            {
                Id = $"{row.Id}:cube",
                MultiDimensionalDatabase = database,
                Name = row.Name,
                Description = row.Description,
            });
            databases[row] = database;
            cubes[row] = cube;
        }

        foreach (var row in source.DataSourceList)
        {
            var converted = Add(target.MultiDimensionalDataSourceList, new MultiDimensionalDataSource
            {
                Id = row.Id,
                MultiDimensionalDatabase = databases[row.AnalyticsModel],
                Name = row.Name,
                Provider = row.Provider,
                ConnectionReference = row.ConnectionReference,
                SourceKind = row.SourceKind,
                Description = row.Description,
            });
            dataSources[row] = converted;
        }

        foreach (var row in source.TableList.Where(table => !factTables.Contains(table)))
        {
            var dimension = Add(target.DimensionList, new Dimension
            {
                Id = row.Id,
                MultiDimensionalDatabase = databases[row.AnalyticsModel],
                Name = row.Name,
                DimensionType = row.DataCategory,
                SourceName = row.Name,
                Description = row.Description,
            });
            var cubeDimension = Add(target.CubeDimensionList, new CubeDimension
            {
                Id = $"{row.Id}:cube-dimension",
                Cube = cubes[row.AnalyticsModel],
                Dimension = dimension,
                Name = row.Name,
                Description = row.Description,
            });
            dimensions[row] = dimension;
            cubeDimensions[row] = cubeDimension;
        }

        foreach (var row in source.AttributeList.Where(attribute => dimensions.ContainsKey(attribute.Table)))
        {
            var converted = Add(target.DimensionAttributeList, new MultiDimensionAttribute
            {
                Id = row.Id,
                Dimension = dimensions[row.Table],
                Name = row.Name,
                DataTypeId = row.DataTypeId,
                Ordinal = row.Ordinal,
                SourceName = row.SourceName ?? row.Name,
                Usage = row.IsKey == "true" ? "Key" : row.Kind,
                AttributeHierarchyEnabled = "true",
                AttributeHierarchyVisible = row.IsHidden == "true" ? "false" : "true",
                IsKey = row.IsKey,
                Description = row.Description,
            });
            dimensionAttributes[row] = converted;
        }

        foreach (var row in source.AttributeRelationshipList)
        {
            if (!dimensionAttributes.ContainsKey(row.ChildAttribute) ||
                !dimensionAttributes.ContainsKey(row.ParentAttribute))
            {
                continue;
            }

            Add(target.AttributeRelationshipList, new MultiAttributeRelationship
            {
                Id = row.Id,
                ChildAttribute = dimensionAttributes[row.ChildAttribute],
                ParentAttribute = dimensionAttributes[row.ParentAttribute],
                RelationshipType = row.RelationshipType,
                Description = row.Description,
            });
        }

        foreach (var row in source.HierarchyList.Where(hierarchy => dimensions.ContainsKey(hierarchy.Table)))
        {
            var converted = Add(target.DimensionHierarchyList, new DimensionHierarchy
            {
                Id = row.Id,
                Dimension = dimensions[row.Table],
                Name = row.Name,
                HierarchyType = row.Kind,
                Description = row.Description,
            });
            hierarchies[row] = converted;
        }

        foreach (var row in source.HierarchyLevelList.Where(level => hierarchies.ContainsKey(level.Hierarchy)))
        {
            Add(target.DimensionHierarchyLevelList, new DimensionHierarchyLevel
            {
                Id = row.Id,
                DimensionHierarchy = hierarchies[row.Hierarchy],
                DimensionAttribute = dimensionAttributes[row.Attribute],
                Name = row.Name,
                Ordinal = row.Ordinal,
            });
        }

        foreach (var row in factTables)
        {
            var converted = Add(target.MeasureGroupList, new MeasureGroup
            {
                Id = $"{row.Id}:measure-group",
                Cube = cubes[row.AnalyticsModel],
                Name = row.Name,
                SourceName = row.Name,
                Description = row.Description,
            });
            measureGroups[row] = converted;
        }

        foreach (var row in source.MeasureList)
        {
            var converted = Add(target.MeasureList, new MultiMeasure
            {
                Id = row.Id,
                MeasureGroup = measureGroups[row.Table],
                Name = row.Name,
                DataTypeId = row.DataTypeId ?? row.SourceAttribute.DataTypeId,
                SourceName = row.SourceAttribute.SourceName ?? row.SourceAttribute.Name,
                AggregateFunction = aggregateFunctions.ToMultiDimensionalFunction(row),
                FormatString = row.FormatString,
                DisplayFolder = row.DisplayFolder,
                Description = row.Description,
            });
            measures[row] = converted;
        }

        foreach (var row in source.RelationshipList)
        {
            if (!measureGroups.TryGetValue(row.FromTable, out var group) ||
                !cubeDimensions.TryGetValue(row.ToTable, out var cubeDimension))
            {
                continue;
            }

            Add(target.DimensionUsageList, new DimensionUsage
            {
                Id = row.Id,
                MeasureGroup = group,
                CubeDimension = cubeDimension,
                GranularityAttribute = dimensionAttributes.GetValueOrDefault(row.ToAttribute),
                UsageKind = row.RelationshipKind,
                RoleName = row.RoleName,
                IsRequired = row.IsRequired,
                Description = row.Description,
            });
        }

        foreach (var row in source.PerspectiveList)
        {
            var converted = Add(target.PerspectiveList, new MultiPerspective
            {
                Id = row.Id,
                Cube = cubes[row.AnalyticsModel],
                Name = row.Name,
                Description = row.Description,
            });
            perspectives[row] = converted;
        }

        foreach (var row in source.PerspectiveTableList)
        {
            if (cubeDimensions.TryGetValue(row.Table, out var dimension))
            {
                Add(target.PerspectiveDimensionList, new PerspectiveDimension { Id = row.Id, Perspective = perspectives[row.Perspective], CubeDimension = dimension });
            }
            else if (measureGroups.TryGetValue(row.Table, out var measureGroup))
            {
                Add(target.PerspectiveMeasureGroupList, new PerspectiveMeasureGroup { Id = row.Id, Perspective = perspectives[row.Perspective], MeasureGroup = measureGroup });
            }
        }

        foreach (var row in source.PerspectiveMeasureList) Add(target.PerspectiveMeasureList, new PerspectiveMeasure { Id = row.Id, Perspective = perspectives[row.Perspective], Measure = measures[row.Measure] });

        foreach (var row in source.SecurityRoleList)
        {
            var converted = Add(target.SecurityRoleList, new MultiSecurityRole
            {
                Id = row.Id,
                MultiDimensionalDatabase = databases[row.AnalyticsModel],
                Name = row.Name,
                Permission = row.Permission,
                Description = row.Description,
            });
            roles[row] = converted;
        }

        foreach (var row in source.RoleMemberList) Add(target.RoleMemberList, new MultiRoleMember { Id = row.Id, SecurityRole = roles[row.SecurityRole], MemberName = row.MemberName });

        foreach (var row in source.CultureList)
        {
            var converted = Add(target.CultureList, new MultiCulture
            {
                Id = row.Id,
                MultiDimensionalDatabase = databases[row.AnalyticsModel],
                Name = row.Name,
                Description = row.Description,
            });
            cultures[row] = converted;
        }

        foreach (var row in source.TableTranslationList)
        {
            if (dimensions.TryGetValue(row.Table, out var dimension))
            {
                Add(target.DimensionTranslationList, new DimensionTranslation { Id = row.Id, Culture = cultures[row.Culture], Dimension = dimension, Caption = row.Caption, Description = row.Description });
            }
        }

        foreach (var row in source.AttributeTranslationList)
        {
            if (dimensionAttributes.TryGetValue(row.Attribute, out var attribute))
            {
                Add(target.AttributeTranslationList, new AttributeTranslation { Id = row.Id, Culture = cultures[row.Culture], DimensionAttribute = attribute, Caption = row.Caption, Description = row.Description });
            }
        }

        foreach (var row in source.MeasureTranslationList) Add(target.MeasureTranslationList, new MultiMeasureTranslation { Id = row.Id, Culture = cultures[row.Culture], Measure = measures[row.Measure], Caption = row.Caption, Description = row.Description });
        foreach (var row in source.PerspectiveTranslationList) Add(target.PerspectiveTranslationList, new PerspectiveTranslation { Id = row.Id, Culture = cultures[row.Culture], Perspective = perspectives[row.Perspective], Caption = row.Caption, Description = row.Description });

        _ = dataSources;
        return target;
    }

    private static void RejectUnsupportedSecurity(MetaAnalytics.MetaAnalyticsModel source)
    {
        if (source.TablePermissionList.Count > 0 || source.AttributePermissionList.Count > 0)
        {
            throw new InvalidOperationException(
                "MetaMultiDimensional conversion does not translate tabular-style row/object security. Convert the shared model first, then add dimension/cell permissions in MetaMultiDimensional.");
        }
    }

    private static T Add<T>(ICollection<T> rows, T row)
    {
        rows.Add(row);
        return row;
    }

    private sealed class AggregateFunctionIndex
    {
        private readonly HashSet<MetaAnalytics.AggregateFunction> sums;
        private readonly HashSet<MetaAnalytics.AggregateFunction> averages;
        private readonly HashSet<MetaAnalytics.AggregateFunction> counts;
        private readonly HashSet<MetaAnalytics.AggregateFunction> distinctCounts;
        private readonly HashSet<MetaAnalytics.AggregateFunction> minimums;
        private readonly HashSet<MetaAnalytics.AggregateFunction> maximums;

        public AggregateFunctionIndex(MetaAnalytics.MetaAnalyticsModel model)
        {
            sums = new HashSet<MetaAnalytics.AggregateFunction>(model.SumAggregateFunctionList.Select(row => row.AggregateFunction), ReferenceEqualityComparer.Instance);
            averages = new HashSet<MetaAnalytics.AggregateFunction>(model.AverageAggregateFunctionList.Select(row => row.AggregateFunction), ReferenceEqualityComparer.Instance);
            counts = new HashSet<MetaAnalytics.AggregateFunction>(model.CountAggregateFunctionList.Select(row => row.AggregateFunction), ReferenceEqualityComparer.Instance);
            distinctCounts = new HashSet<MetaAnalytics.AggregateFunction>(model.DistinctCountAggregateFunctionList.Select(row => row.AggregateFunction), ReferenceEqualityComparer.Instance);
            minimums = new HashSet<MetaAnalytics.AggregateFunction>(model.MinimumAggregateFunctionList.Select(row => row.AggregateFunction), ReferenceEqualityComparer.Instance);
            maximums = new HashSet<MetaAnalytics.AggregateFunction>(model.MaximumAggregateFunctionList.Select(row => row.AggregateFunction), ReferenceEqualityComparer.Instance);
        }

        public string ToMultiDimensionalFunction(AnalyticsMeasure measure)
        {
            var matches = new List<string>(capacity: 1);
            AddIf(matches, sums, measure.AggregateFunction, "Sum");
            AddIf(matches, averages, measure.AggregateFunction, "Average");
            AddIf(matches, counts, measure.AggregateFunction, "Count");
            AddIf(matches, distinctCounts, measure.AggregateFunction, "DistinctCount");
            AddIf(matches, minimums, measure.AggregateFunction, "Min");
            AddIf(matches, maximums, measure.AggregateFunction, "Max");
            if (matches.Count != 1)
            {
                throw new InvalidOperationException(
                    $"Measure '{measure.Id}' must reference one concrete aggregate-function entity; found {matches.Count}.");
            }

            return matches[0];
        }

        private static void AddIf(
            ICollection<string> matches,
            IReadOnlySet<MetaAnalytics.AggregateFunction> functions,
            MetaAnalytics.AggregateFunction candidate,
            string targetName)
        {
            if (functions.Contains(candidate))
            {
                matches.Add(targetName);
            }
        }
    }
}
