using MetaDataWarehouse;

namespace MetaDataWarehouse.Tests;

public sealed class GeneratedPocoContractTests
{
    [Fact]
    public void RelationshipsExposeObjectReferences_NotPublicTransportIds()
    {
        Assert.Null(typeof(FactDimension).GetProperty("FactId"));
        Assert.Null(typeof(FactDimension).GetProperty("DimensionId"));
        Assert.Equal(typeof(Fact), typeof(FactDimension).GetProperty(nameof(FactDimension.Fact))!.PropertyType);
        Assert.Equal(typeof(Dimension), typeof(FactDimension).GetProperty(nameof(FactDimension.Dimension))!.PropertyType);
    }

    [Fact]
    public void ScalarIdLikePropertiesRemainWhenTheyAreActualScalarProperties()
    {
        Assert.NotNull(typeof(DimensionAttribute).GetProperty(nameof(DimensionAttribute.DataTypeId)));
        Assert.Equal(typeof(string), typeof(DimensionAttribute).GetProperty(nameof(DimensionAttribute.DataTypeId))!.PropertyType);
    }

    [Fact]
    public void FactMeasureDoesNotOwnAnalyticsAggregationSemantics()
    {
        Assert.Null(typeof(FactMeasure).GetProperty("Additivity"));
        Assert.Null(typeof(FactMeasure).GetProperty("AggregationFunction"));
        Assert.NotNull(typeof(FactMeasure).GetProperty(nameof(FactMeasure.DataTypeId)));
    }
}
