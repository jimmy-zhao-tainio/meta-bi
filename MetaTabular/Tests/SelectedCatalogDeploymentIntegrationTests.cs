using System.Reflection;
using Meta.Integration;
using Meta.Surfaces;
using MetaBi.Tests.Common;
using MetaDataType;
using MetaDataType.Core;
using MetaTabular.Core;
using MetaTabular.Core.Deploy;
using Tom = Microsoft.AnalysisServices.Tabular;

namespace MetaTabular.Tests;

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
            var source = await TypedWorkspaceModelMapper.LoadAsync(sourcePath, new MetaTabularWorkspaceBinding());
            var catalog = MetaDataTypeWorkspaceProvider.LoadOrDefault(catalogPath);
            Assert.Same(Assert.Single(source.TabularTableList), source.TabularColumnList[0].TabularTable);
            var method = typeof(MetaTabularDeployService).GetMethod("BuildDatabaseWithCatalog", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(method);
            var database = Assert.IsType<Tom.Database>(method.Invoke(null, [source, Assert.Single(source.TabularModelList), "Selected", catalog]));
            var table = Assert.Single(database.Model.Tables);
            Assert.Equal(Tom.DataType.Int64, table.Columns["Number"].DataType);
            Assert.Equal(Tom.DataType.String, table.Columns["Unmapped"].DataType);
            Assert.Equal(Tom.DataType.String, table.Columns["Foreign"].DataType);
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
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => new MetaTabularDeployService().DeployAsync(new MetaTabularDeployRequest
            {
                WorkspacePath = sourcePath, Server = "must-not-connect", DataTypeWorkspacePath = catalogPath,
            }));
            Assert.Contains("TabularColumn 'column-number'", exception.Message);
            Assert.Contains("'unknown-type', which resolves to 0", exception.Message);
            var cli = CliTestRunner.RunStandardCli("meta-tabular", $"deploy --workspace \"{sourcePath}\" --server must-not-connect --data-type-workspace \"{catalogPath}\" --no-process");
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
        var model = new MetaTabularModel();
        var tabular = new TabularModel { Id = "root", Name = "Selected", CompatibilityLevel = "1500" };
        var table = new TabularTable { Id = "table", Name = "Values", TabularModel = tabular };
        model.TabularModelList.Add(tabular);
        model.TabularTableList.Add(table);
        model.TabularColumnList.AddRange([
            new TabularColumn { Id = "column-number", Name = "Number", Ordinal = "1", DataTypeId = integerTypeId, TabularTable = table },
            new TabularColumn { Id = "column-unmapped", Name = "Unmapped", Ordinal = "2", DataTypeId = "chosen-unmapped", TabularTable = table },
            new TabularColumn { Id = "column-foreign", Name = "Foreign", Ordinal = "3", DataTypeId = "meta:type:Int32", TabularTable = table },
        ]);
        var path = Path.Combine(root, "source");
        await WorkspaceSurface.CreateAsync(new MetaTabularWorkspaceBinding().Write(model, TabularModels.LoadContract()), path, representation);
        return path;
    }

    private static string NewRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "tabular-selected-catalog", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }
}
