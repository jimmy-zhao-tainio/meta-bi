namespace MetaAnalytics.Tests;

public sealed class GeneratedPocoContractTests
{
    [Fact]
    public void RelationshipsExposeObjectReferences_NotPublicTransportIds()
    {
        Assert.Null(typeof(Relationship).GetProperty("FromTableId"));
        Assert.Null(typeof(Relationship).GetProperty("FromAttributeId"));
        Assert.Null(typeof(Relationship).GetProperty("ToTableId"));
        Assert.Null(typeof(Relationship).GetProperty("ToAttributeId"));
        Assert.Equal(typeof(Table), typeof(Relationship).GetProperty(nameof(Relationship.FromTable))!.PropertyType);
        Assert.Equal(typeof(MetaAnalytics.Attribute), typeof(Relationship).GetProperty(nameof(Relationship.FromAttribute))!.PropertyType);
        Assert.Equal(typeof(Table), typeof(Relationship).GetProperty(nameof(Relationship.ToTable))!.PropertyType);
        Assert.Equal(typeof(MetaAnalytics.Attribute), typeof(Relationship).GetProperty(nameof(Relationship.ToAttribute))!.PropertyType);
    }

    [Fact]
    public void NullableRelationshipsStayReferencesOnly()
    {
        Assert.Null(typeof(Relationship).GetProperty("GranularityAttributeId"));
        Assert.Equal(typeof(MetaAnalytics.Attribute), typeof(Relationship).GetProperty(nameof(Relationship.GranularityAttribute))!.PropertyType);
    }

    [Fact]
    public void CrossCuttingMembershipUsesTypedRows()
    {
        Assert.Null(typeof(PerspectiveMeasure).GetProperty("ObjectId"));
        Assert.Null(typeof(PerspectiveMeasure).GetProperty("ObjectKind"));
        Assert.Equal(typeof(Perspective), typeof(PerspectiveMeasure).GetProperty(nameof(PerspectiveMeasure.Perspective))!.PropertyType);
        Assert.Equal(typeof(Measure), typeof(PerspectiveMeasure).GetProperty(nameof(PerspectiveMeasure.Measure))!.PropertyType);
    }
}
