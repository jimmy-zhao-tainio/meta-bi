using System.Reflection;
using Meta.Integration;
using Meta.Surfaces;
using MetaBi.Tests.Common;
using MetaDataType;
using MetaDataType.Core;
using MetaMultiDimensional.Core;
using MetaMultiDimensional.Core.Deploy;
using Amo = Microsoft.AnalysisServices;

namespace MetaMultiDimensional.Tests;

public sealed class SelectedCatalogDeploymentIntegrationTests
{
    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task PersistedArbitraryTypeIdsProduceTheExistingNativeTypes(string representation)
    {
        var root = NewRoot();
        try
        {
            var catalogPath = await SaveCatalog(root, representation);
            var sourcePath = await SaveModel(root, representation, "chosen-integer");
            var source = await TypedWorkspaceModelMapper.LoadAsync(sourcePath, new MetaMultiDimensionalWorkspaceBinding());
            var catalog = MetaDataTypeWorkspaceProvider.LoadOrDefault(catalogPath);
            Assert.Same(Assert.Single(source.DimensionList), source.DimensionAttributeList[0].Dimension);
            var method = typeof(MetaMultiDimensionalDeployService).GetMethod("BuildDatabaseWithCatalog", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(method);
            var database = Assert.IsType<Amo.Database>(method.Invoke(null, [source, Assert.Single(source.MultiDimensionalDatabaseList), "Selected", catalog]));
            var dimension = Assert.Single(database.Dimensions.Cast<Amo.Dimension>());
            Assert.Equal(new Amo.DataItem("Values", "Number", Amo.DataType.Int32).Type, Assert.Single(dimension.Attributes["attribute_number"].KeyColumns.Cast<Amo.DataItem>()).Type);
            Assert.Equal(new Amo.DataItem("Values", "Text", Amo.DataType.String).Type, Assert.Single(dimension.Attributes["attribute_unmapped"].KeyColumns.Cast<Amo.DataItem>()).Type);
            Assert.Equal(new Amo.DataItem("Values", "Text", Amo.DataType.String).Type, Assert.Single(dimension.Attributes["attribute_foreign"].KeyColumns.Cast<Amo.DataItem>()).Type);
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task SelectedCatalogRejectsUnknownIdsBeforeConnectingThroughApiAndCli(string representation)
    {
        var root = NewRoot();
        try
        {
            var catalogPath = await SaveCatalog(root, representation);
            var sourcePath = await SaveModel(root, representation, "unknown-type");
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => new MetaMultiDimensionalDeployService().DeployAsync(new MetaMultiDimensionalDeployRequest
            {
                WorkspacePath = sourcePath, Server = "must-not-connect", DataTypeWorkspacePath = catalogPath,
            }));
            Assert.Contains("DimensionAttribute 'attribute-number'", exception.Message);
            Assert.Contains("'unknown-type', which resolves to 0", exception.Message);
            var cli = CliTestRunner.RunStandardCli("meta-multi-dimensional", $"deploy --workspace \"{sourcePath}\" --server must-not-connect --data-type-workspace \"{catalogPath}\" --no-process");
            Assert.Equal(4, cli.ExitCode);
            Assert.Contains("'unknown-type', which resolves to 0", cli.Output);
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    private static async Task<string> SaveCatalog(string root, string representation)
    {
        var defaults = await MetaDataTypeWorkspaceProvider.LoadDefaultStateAsync();
        var catalog = new MetaDataTypeModel();
        var meta = new DataTypeSystem { Id = "chosen-system", Name = "Meta" };
        var foreign = new DataTypeSystem { Id = "Meta", Name = "Other" };
        catalog.DataTypeSystemList.AddRange([meta, foreign]);
        catalog.DataTypeList.AddRange([
            new DataType { Id = "chosen-integer", Name = "Int32", DataTypeSystem = meta },
            new DataType { Id = "chosen-unmapped", Name = "Unmapped", DataTypeSystem = meta },
            new DataType { Id = "meta:type:Int32", Name = "Int32", DataTypeSystem = foreign },
        ]);
        var path = Path.Combine(root, "catalog");
        await WorkspaceSurface.CreateAsync(new MetaDataTypeWorkspaceBinding().Write(catalog, defaults.Model), path, representation);
        return path;
    }

    private static async Task<string> SaveModel(string root, string representation, string integerTypeId)
    {
        var model = new MetaMultiDimensionalModel();
        var database = new MultiDimensionalDatabase { Id = "root", Name = "Selected" };
        var dimension = new Dimension { Id = "dimension", Name = "Values", MultiDimensionalDatabase = database };
        model.MultiDimensionalDatabaseList.Add(database);
        model.MultiDimensionalDataSourceList.Add(new MultiDimensionalDataSource
        {
            Id = "source", Name = "Source", Provider = "SqlServer", MultiDimensionalDatabase = database,
        });
        model.DimensionList.Add(dimension);
        model.DimensionAttributeList.AddRange([
            new DimensionAttribute { Id = "attribute-number", Name = "Number", Ordinal = "1", IsKey = "true", DataTypeId = integerTypeId, Dimension = dimension },
            new DimensionAttribute { Id = "attribute-unmapped", Name = "Unmapped", Ordinal = "2", DataTypeId = "chosen-unmapped", Dimension = dimension },
            new DimensionAttribute { Id = "attribute-foreign", Name = "Foreign", Ordinal = "3", DataTypeId = "meta:type:Int32", Dimension = dimension },
        ]);
        var path = Path.Combine(root, "source");
        await WorkspaceSurface.CreateAsync(new MetaMultiDimensionalWorkspaceBinding().Write(model, MultiDimensionalModels.LoadContract()), path, representation);
        return path;
    }

    private static string NewRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "multidimensional-selected-catalog", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }
}
