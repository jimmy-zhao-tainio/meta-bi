using MetaTabular.Core;

namespace MetaTabular.Tests;

public sealed class ModelShapeTests
{
    [Fact]
    public void MetaTabularModel_CarriesTabularImplementationConcepts()
    {
        var model = MetaTabularModels.CreateMetaTabularModel();
        var entityNames = model.Entities.Select(entity => entity.Name).ToHashSet(StringComparer.Ordinal);

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
