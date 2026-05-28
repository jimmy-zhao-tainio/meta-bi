using MetaMultiDimensional.Core;

namespace MetaMultiDimensional.Tests;

public sealed class ModelShapeTests
{
    [Fact]
    public void MetaMultiDimensionalModel_CarriesMultidimensionalImplementationConcepts()
    {
        var model = MetaMultiDimensionalModels.CreateMetaMultiDimensionalModel();
        var entityNames = model.Entities.Select(entity => entity.Name).ToHashSet(StringComparer.Ordinal);

        Assert.Contains("MultiDimensionalDatabase", entityNames);
        Assert.Contains("Cube", entityNames);
        Assert.Contains("Dimension", entityNames);
        Assert.Contains("DimensionAttribute", entityNames);
        Assert.Contains("CubeDimension", entityNames);
        Assert.Contains("MeasureGroup", entityNames);
        Assert.Contains("Measure", entityNames);
        Assert.Contains("DimensionUsage", entityNames);
        Assert.Contains("MdxCalculation", entityNames);
        Assert.Contains("NamedSet", entityNames);
        Assert.Contains("CubeAction", entityNames);
        Assert.Contains("CellPermission", entityNames);

        var dimension = Assert.Single(model.Entities, entity => entity.Name == "Dimension");
        var dimensionProperties = dimension.Properties.Select(property => property.Name).ToHashSet(StringComparer.Ordinal);
        Assert.Contains("StorageMode", dimensionProperties);
        Assert.Contains("ProcessingMode", dimensionProperties);
        Assert.Contains("ProcessingGroup", dimensionProperties);

        var cube = Assert.Single(model.Entities, entity => entity.Name == "Cube");
        var cubeProperties = cube.Properties.Select(property => property.Name).ToHashSet(StringComparer.Ordinal);
        Assert.Contains("StorageMode", cubeProperties);
        Assert.Contains("ProcessingMode", cubeProperties);

        var measureGroup = Assert.Single(model.Entities, entity => entity.Name == "MeasureGroup");
        var measureGroupProperties = measureGroup.Properties.Select(property => property.Name).ToHashSet(StringComparer.Ordinal);
        Assert.Contains("StorageMode", measureGroupProperties);
        Assert.Contains("ProcessingMode", measureGroupProperties);
    }
}
