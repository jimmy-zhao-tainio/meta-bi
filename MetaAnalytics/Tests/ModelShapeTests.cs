namespace MetaAnalytics.Tests;

public sealed class ModelShapeTests
{
    [Fact]
    public void MetaAnalyticsModel_UsesCleanNamesAndStaysConceptual()
    {
        var entityNames = typeof(MetaAnalyticsModel)
            .GetProperties()
            .Where(property =>
                property.PropertyType.IsGenericType &&
                property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            .Select(property => property.Name[..^"List".Length])
            .ToHashSet(StringComparer.Ordinal);

        Assert.DoesNotContain(entityNames, name => name.Contains("Semantic", StringComparison.OrdinalIgnoreCase));
        Assert.Contains("AnalyticsModel", entityNames);
        Assert.Contains("DataSource", entityNames);
        Assert.Contains("Table", entityNames);
        Assert.Contains("Attribute", entityNames);
        Assert.Contains("Hierarchy", entityNames);
        Assert.Contains("Relationship", entityNames);
        Assert.Contains("Measure", entityNames);
        Assert.Contains("AggregateFunction", entityNames);
        Assert.Contains("SumAggregateFunction", entityNames);
        Assert.Contains("AverageAggregateFunction", entityNames);
        Assert.Contains("CountAggregateFunction", entityNames);
        Assert.Contains("DistinctCountAggregateFunction", entityNames);
        Assert.Contains("MinimumAggregateFunction", entityNames);
        Assert.Contains("MaximumAggregateFunction", entityNames);
        Assert.Contains("TablePermission", entityNames);
        Assert.Contains("AttributePermission", entityNames);
        Assert.Contains("Perspective", entityNames);
        Assert.Contains("Culture", entityNames);
        Assert.DoesNotContain("MeasureGroup", entityNames);
        Assert.DoesNotContain("DimensionUsage", entityNames);
        Assert.DoesNotContain("CalculationGroup", entityNames);
        Assert.DoesNotContain("CalculationItem", entityNames);
        Assert.DoesNotContain("CellPermission", entityNames);
        Assert.DoesNotContain("Partition", entityNames);
        Assert.DoesNotContain("Action", entityNames);
        Assert.DoesNotContain("NamedSet", entityNames);
        Assert.DoesNotContain("MeasureExpression", entityNames);
        Assert.DoesNotContain("Kpi", entityNames);

        Assert.Equal(typeof(AggregateFunction), typeof(Measure).GetProperty(nameof(Measure.AggregateFunction))!.PropertyType);
        Assert.Null(typeof(Attribute).GetProperty("ExpressionLanguage"));
        Assert.Null(typeof(Attribute).GetProperty("Expression"));
        Assert.DoesNotContain("AggregationBehavior", entityNames);
        Assert.DoesNotContain("RoleFilter", entityNames);
    }
}
