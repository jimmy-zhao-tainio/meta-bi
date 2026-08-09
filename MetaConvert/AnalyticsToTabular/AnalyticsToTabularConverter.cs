using AnalyticsAttribute = MetaAnalytics.Attribute;
using AnalyticsTable = MetaAnalytics.Table;
using MetaAnalytics;
using MetaTabular;

namespace MetaConvert.AnalyticsToTabular;

public sealed record AnalyticsToTabularResult(
    string SourceWorkspacePath,
    string OutputWorkspacePath,
    int TableCount,
    int ColumnCount,
    int MeasureCount);

public static class AnalyticsToTabularConverter
{
    public static Task<AnalyticsToTabularResult> ConvertAsync(
        string sourceWorkspacePath,
        string outputWorkspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputWorkspacePath);

        var sourcePath = Path.GetFullPath(sourceWorkspacePath);
        var outputPath = Path.GetFullPath(outputWorkspacePath);
        var source = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaAnalyticsModel>(sourcePath, searchUpward: false);
        var target = Convert(source);
        Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(target, outputPath);

        return Task.FromResult(new AnalyticsToTabularResult(
            sourcePath,
            outputPath,
            target.TabularTableList.Count,
            target.TabularColumnList.Count,
            target.TabularMeasureList.Count));
    }

    public static MetaTabularModel Convert(MetaAnalyticsModel source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var target = MetaTabularModel.CreateEmpty();
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
                Expression = RequireTabularExpression(row.ExpressionLanguage, row.Expression, $"Attribute '{row.Id}'"),
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

        var aggregationByMeasure = source.AggregationBehaviorList
            .GroupBy(row => row.Measure)
            .ToDictionary(group => group.Key, group => group.ToArray());
        foreach (var row in source.MeasureList)
        {
            var expression = BuildDaxMeasureExpression(row, aggregationByMeasure.GetValueOrDefault(row));
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
        foreach (var row in source.RoleFilterList) Add(target.TabularRoleFilterList, new TabularRoleFilter { Id = row.Id, TabularSecurityRole = roles[row.SecurityRole], TabularTable = tables[row.Table], Expression = RequireExpressionLanguage(row.ExpressionLanguage, "DAX", row.Expression, $"RoleFilter '{row.Id}'") });
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

    private static string BuildDaxMeasureExpression(Measure measure, IReadOnlyList<AggregationBehavior>? aggregationBehaviors)
    {
        var behavior = RequireSingleAggregationBehavior(measure, aggregationBehaviors);
        var functionName = ToDaxAggregateFunction(behavior.Function, measure.Id);
        return $"{functionName}({DaxIdentifier(measure.Table.Name)}[{DaxColumnIdentifier(measure.SourceAttribute.Name)}])";
    }

    private static AggregationBehavior RequireSingleAggregationBehavior(Measure measure, IReadOnlyList<AggregationBehavior>? aggregationBehaviors)
    {
        if (aggregationBehaviors == null || aggregationBehaviors.Count == 0)
        {
            throw new InvalidOperationException($"Measure '{measure.Id}' does not define an aggregation behavior.");
        }

        if (aggregationBehaviors.Count > 1)
        {
            throw new InvalidOperationException($"Measure '{measure.Id}' defines multiple aggregation behaviors.");
        }

        return aggregationBehaviors[0];
    }

    private static string? RequireTabularExpression(string? language, string? expression, string context)
    {
        return string.IsNullOrWhiteSpace(expression)
            ? null
            : RequireExpressionLanguage(language, "DAX", expression, context);
    }

    private static string RequireExpressionLanguage(string? actual, string expected, string expression, string context)
    {
        if (!string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"{context} uses expression language '{actual ?? "(none)"}', but MetaTabular conversion requires {expected}.");
        }

        return expression;
    }

    private static string ToDaxAggregateFunction(string function, string measureId)
    {
        return function.Trim().ToUpperInvariant() switch
        {
            "SUM" => "SUM",
            "COUNT" => "COUNT",
            "DISTINCTCOUNT" or "DISTINCT_COUNT" => "DISTINCTCOUNT",
            "MIN" => "MIN",
            "MAX" => "MAX",
            "AVERAGE" or "AVG" => "AVERAGE",
            _ => throw new InvalidOperationException($"Measure '{measureId}' uses aggregate function '{function}', which does not have a supported DAX base-measure projection."),
        };
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
}
