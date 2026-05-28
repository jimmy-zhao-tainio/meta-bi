using MetaAnalytics.Core;

namespace MetaAnalytics.Tests;

public sealed class ModelShapeTests
{
    [Fact]
    public void MetaAnalyticsModel_UsesCleanNamesAndStaysConceptual()
    {
        var model = MetaAnalyticsModels.CreateMetaAnalyticsModel();
        var entityNames = model.Entities.Select(entity => entity.Name).ToHashSet(StringComparer.Ordinal);

        Assert.DoesNotContain(entityNames, name => name.Contains("Semantic", StringComparison.OrdinalIgnoreCase));
        Assert.Contains("AnalyticsModel", entityNames);
        Assert.Contains("DataSource", entityNames);
        Assert.Contains("Table", entityNames);
        Assert.Contains("Attribute", entityNames);
        Assert.Contains("Hierarchy", entityNames);
        Assert.Contains("Relationship", entityNames);
        Assert.Contains("Measure", entityNames);
        Assert.Contains("AggregationBehavior", entityNames);
        Assert.Contains("RoleFilter", entityNames);
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
    }
}
