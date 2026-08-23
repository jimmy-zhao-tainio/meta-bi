using System.Reflection;
using MetaMultiDimensionalModel = MetaMultiDimensional.MetaMultiDimensionalModel;

namespace MetaMultiDimensional.Tests;

public sealed class ModelShapeTests
{
    [Fact]
    public void MetaMultiDimensionalModel_CarriesMultidimensionalImplementationConcepts()
    {
        var modelType = typeof(MetaMultiDimensionalModel);
        var entityNames = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.PropertyType.IsGenericType &&
                               property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            .Select(property => property.PropertyType.GetGenericArguments()[0].Name)
            .ToHashSet(StringComparer.Ordinal);

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

        var dimensionProperties = EntityProperties<Dimension>();
        Assert.Contains("StorageMode", dimensionProperties);
        Assert.Contains("ProcessingMode", dimensionProperties);
        Assert.Contains("ProcessingGroup", dimensionProperties);

        var cubeProperties = EntityProperties<Cube>();
        Assert.Contains("StorageMode", cubeProperties);
        Assert.Contains("ProcessingMode", cubeProperties);

        var measureGroupProperties = EntityProperties<MeasureGroup>();
        Assert.Contains("StorageMode", measureGroupProperties);
        Assert.Contains("ProcessingMode", measureGroupProperties);
    }

    private static HashSet<string> EntityProperties<T>() =>
        typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.CanRead && property.CanWrite)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
