namespace MetaTabular.Tests;

public sealed class ModelShapeTests
{
    [Fact]
    public void MetaTabularModel_CarriesTabularImplementationConcepts()
    {
        var entityNames = typeof(MetaTabularModel).GetProperties()
            .Where(property => property.PropertyType.IsGenericType &&
                               property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            .Select(property => property.PropertyType.GetGenericArguments()[0].Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains("TabularModel", entityNames);
        Assert.Contains("TabularTable", entityNames);
        Assert.Contains("TabularColumn", entityNames);
        Assert.Contains("TabularRelationship", entityNames);
        Assert.Contains("TabularMeasure", entityNames);
        Assert.Contains("TabularCalculationGroup", entityNames);
        Assert.Contains("TabularCalculationItem", entityNames);
        Assert.Contains("TabularPartition", entityNames);
        Assert.Contains("TabularRoleFilter", entityNames);
        Assert.Contains("TabularTablePermission", entityNames);
        Assert.Contains("TabularColumnPermission", entityNames);
        Assert.Contains("TabularCulture", entityNames);
    }
}
