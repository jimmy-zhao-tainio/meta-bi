namespace MetaDataWarehouse.Tests;

public sealed class WorkspaceRoundTripTests
{
    [Fact]
    public void SampleSales_RoundTripsWithReferenceCompleteFactRoles()
    {
        var path = CreateTempPath();
        try
        {
            Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(TestModels.LoadSampleSales(), path);

            var loaded = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaDataWarehouseModel>(path, searchUpward: false);
            var date = Assert.Single(loaded.DimensionList, row => row.Id == "dimension:date");
            var salesOrder = Assert.Single(loaded.FactList, row => row.Id == "fact:sales-order");
            var orderDate = Assert.Single(loaded.FactDimensionList, row => row.Fact.Id == salesOrder.Id && row.RoleName == "OrderDate");
            var shipDate = Assert.Single(loaded.FactDimensionList, row => row.Fact.Id == salesOrder.Id && row.RoleName == "ShipDate");

            Assert.Same(salesOrder, orderDate.Fact);
            Assert.Same(salesOrder, shipDate.Fact);
            Assert.Same(date, orderDate.Dimension);
            Assert.Same(date, shipDate.Dimension);
            Assert.NotEqual(orderDate.Id, shipDate.Id);
        }
        finally
        {
            DeleteDirectoryIfExists(path);
        }
    }

    [Fact]
    public void SCDAndBusinessKeysStayLogical()
    {
        var path = CreateTempPath();
        try
        {
            Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(TestModels.LoadSampleSales(), path);

            var loaded = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaDataWarehouseModel>(path, searchUpward: false);
            var scd = Assert.Single(loaded.SlowlyChangingDimensionList, row => row.Id == "scd:customer");
            var key = Assert.Single(loaded.DimensionBusinessKeyList, row => row.Dimension.Id == "dimension:customer");
            var keyPart = Assert.Single(loaded.DimensionBusinessKeyPartList, row => row.DimensionBusinessKey == key);

            Assert.Equal("dimension:customer", scd.Dimension.Id);
            Assert.Equal("CustomerNumber", keyPart.DimensionAttribute.Name);
            Assert.Equal("meta:type:String", keyPart.DimensionAttribute.DataTypeId);
        }
        finally
        {
            DeleteDirectoryIfExists(path);
        }
    }

    [Fact]
    public void DefaultImplementation_RoundTripsPlatformDefaults()
    {
        var path = CreateTempPath();
        try
        {
            Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(
                MetaDataWarehouseImplementation.MetaDataWarehouseImplementationInstance.BuiltIn,
                path);

            var loaded = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaDataWarehouseImplementation.MetaDataWarehouseImplementationModel>(path, searchUpward: false);
            Assert.Contains(loaded.PlatformColumnImplementationList, row =>
                row.ColumnName == "AuditId" &&
                row.DataTypeId == "meta:type:Int64" &&
                row.DefaultExpressionSql == "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))");
            Assert.Contains(loaded.PlatformColumnImplementationList, row =>
                row.ColumnName == "InsertDateTime2" &&
                row.DefaultExpressionSql == "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))");
        }
        finally
        {
            DeleteDirectoryIfExists(path);
        }
    }

    private static string CreateTempPath()
    {
        return Path.Combine(Path.GetTempPath(), "metadatawarehouse-tests", Guid.NewGuid().ToString("N"));
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
