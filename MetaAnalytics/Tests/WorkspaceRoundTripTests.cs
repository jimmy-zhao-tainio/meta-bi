namespace MetaAnalytics.Tests;

public sealed class WorkspaceRoundTripTests
{
    [Fact]
    public void SampleCommerce_RoundTripsWithReferenceCompleteAnalyticsObjects()
    {
        var path = CreateTempPath();
        try
        {
            Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(TestModels.LoadSampleCommerce(), path);

            var loaded = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaAnalyticsModel>(path, searchUpward: false);
            var sales = Assert.Single(loaded.TableList, row => row.Id == "table:sales");
            var date = Assert.Single(loaded.TableList, row => row.Id == "table:date");
            var salesAmount = Assert.Single(loaded.MeasureList, row => row.Id == "measure:sales-amount");
            var relationship = Assert.Single(loaded.RelationshipList, row => row.Id == "relationship:sales:order-date");
            var perspectiveMeasure = Assert.Single(loaded.PerspectiveMeasureList, row => row.Measure.Id == salesAmount.Id);

            Assert.Same(sales, salesAmount.Table);
            Assert.Equal("Sales Amount", salesAmount.SourceAttribute.Name);
            Assert.Same(sales, relationship.FromTable);
            Assert.Same(date, relationship.ToTable);
            Assert.Same(salesAmount, perspectiveMeasure.Measure);
            Assert.Contains(
                loaded.SumAggregateFunctionList,
                row => ReferenceEquals(row.AggregateFunction, salesAmount.AggregateFunction));
            Assert.Single(loaded.SecurityRoleList);
            Assert.Contains(loaded.MeasureTranslationList, row => row.Measure == salesAmount && row.Culture.Name == "sv-SE");
        }
        finally
        {
            DeleteDirectoryIfExists(path);
        }
    }

    [Fact]
    public void GeneratedSaveRejectsReferenceOutsideCanonicalCollection()
    {
        var model = TestModels.LoadSampleCommerce();
        var measure = model.MeasureList.Single(row => row.Id == "measure:sales-amount");
        var perspective = model.PerspectiveList.Single(row => row.Id == "perspective:sales");

        model.PerspectiveMeasureList.Add(new PerspectiveMeasure
        {
            Id = "bad-perspective-measure",
            Perspective = perspective,
            Measure = new Measure
            {
                Id = measure.Id,
                Table = measure.Table,
                SourceAttribute = measure.SourceAttribute,
                Name = measure.Name,
                AggregateFunction = measure.AggregateFunction,
            },
        });

        var path = CreateTempPath();
        try
        {
            Assert.Throws<InvalidOperationException>(() => Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(model, path));
        }
        finally
        {
            model.PerspectiveMeasureList.RemoveAll(row => row.Id == "bad-perspective-measure");
            DeleteDirectoryIfExists(path);
        }
    }

    private static string CreateTempPath()
    {
        return Path.Combine(Path.GetTempPath(), "metaanalytics-tests", Guid.NewGuid().ToString("N"));
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
