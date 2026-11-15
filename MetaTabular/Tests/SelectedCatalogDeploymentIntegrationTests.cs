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
            var database = BuildDatabase(source, catalog);
            var table = Assert.Single(database.Model.Tables);
            Assert.Equal(Tom.DataType.Int64, table.Columns["Number"].DataType);
            Assert.Equal(Tom.DataType.String, table.Columns["Text"].DataType);
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Theory]
    [InlineData("xml", "chosen-guid", "Guid")]
    [InlineData("csharp", "chosen-guid", "Guid")]
    [InlineData("xml", "chosen-datetime2", "DateTime2")]
    [InlineData("csharp", "chosen-datetime2", "DateTime2")]
    [InlineData("xml", "chosen-datetimeoffset", "DateTimeOffset")]
    [InlineData("csharp", "chosen-datetimeoffset", "DateTimeOffset")]
    [InlineData("xml", "chosen-byte", "Byte")]
    [InlineData("csharp", "chosen-byte", "Byte")]
    [InlineData("xml", "chosen-unmapped", "Unmapped")]
    [InlineData("csharp", "chosen-unmapped", "Unmapped")]
    [InlineData("xml", "meta:type:Int32", "Int32")]
    [InlineData("csharp", "meta:type:Int32", "Int32")]
    [InlineData("xml", "sqlserver:type:int", "Int32")]
    [InlineData("csharp", "sqlserver:type:int", "Int32")]
    [InlineData("xml", "", null)]
    [InlineData("csharp", "", null)]
    [InlineData("xml", " ", null)]
    [InlineData("csharp", " ", null)]
    public async Task UnmappedOrMissingTypesAreRejectedBeforeConnectingThroughApiAndCli(
        string representation, string typeId, string? typeName)
    {
        var root = NewRoot();
        try
        {
            var catalogPath = await SaveCatalog(root, representation);
            var sourcePath = await SaveModel(root, representation, typeId);
            var source = await TypedWorkspaceModelMapper.LoadAsync(sourcePath, new MetaTabularWorkspaceBinding());
            var catalog = MetaDataTypeWorkspaceProvider.LoadOrDefault(catalogPath);
            Tom.Database? accepted = null;
            var buildFailure = Record.Exception(() => { accepted = BuildDatabase(source, catalog); });
            Assert.True(buildFailure is not null,
                $"TabularColumn 'column-number' type ID '{typeId}' (name '{typeName ?? "missing"}') was accepted as TOM {accepted?.Model.Tables["Values"].Columns["Number"].DataType}; expected rejection.");
            var failure = Assert.IsType<InvalidOperationException>(Assert.IsType<TargetInvocationException>(buildFailure).InnerException);
            AssertDiagnostic(failure.Message, typeId, typeName);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => new MetaTabularDeployService().DeployAsync(new MetaTabularDeployRequest
            {
                WorkspacePath = sourcePath, Server = "must-not-connect", DataTypeWorkspacePath = catalogPath,
            }));
            Assert.Equal(failure.Message, exception.Message);
            var cli = CliTestRunner.RunStandardCli("meta-tabular", $"deploy --workspace \"{sourcePath}\" --server must-not-connect --data-type-workspace \"{catalogPath}\" --no-process");
            Assert.Equal(4, cli.ExitCode);
            Assert.Contains(failure.Message, cli.Output);
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
            new DataType { Id = "chosen-string", Name = "String", DataTypeSystem = meta },
            new DataType { Id = "chosen-guid", Name = "Guid", DataTypeSystem = meta },
            new DataType { Id = "chosen-datetime2", Name = "DateTime2", DataTypeSystem = meta },
            new DataType { Id = "chosen-datetimeoffset", Name = "DateTimeOffset", DataTypeSystem = meta },
            new DataType { Id = "chosen-byte", Name = "Byte", DataTypeSystem = meta },
            new DataType { Id = "chosen-unmapped", Name = "Unmapped", DataTypeSystem = meta },
            new DataType { Id = "meta:type:Int32", Name = "Int32", DataTypeSystem = foreign },
            new DataType { Id = "sqlserver:type:int", Name = "Int32", DataTypeSystem = foreign },
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
            new TabularColumn { Id = "column-text", Name = "Text", Ordinal = "2", DataTypeId = "chosen-string", TabularTable = table },
        ]);
        var path = Path.Combine(root, "source");
        await WorkspaceSurface.CreateAsync(new MetaTabularWorkspaceBinding().Write(model, TabularModels.LoadContract()), path, representation);
        return path;
    }

    private static Tom.Database BuildDatabase(MetaTabularModel source, MetaDataTypeModel catalog)
    {
        var method = typeof(MetaTabularDeployService).GetMethod("BuildDatabaseWithCatalog", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.NotNull(method);
        return Assert.IsType<Tom.Database>(method.Invoke(null, [source, Assert.Single(source.TabularModelList), "Selected", catalog]));
    }

    private static void AssertDiagnostic(string message, string typeId, string? typeName)
    {
        Assert.Contains("TabularColumn 'column-number'", message);
        Assert.Contains("Values.Number", message);
        Assert.Contains("DataTypeId", message);
        if (typeName is null)
            Assert.Contains("missing DataTypeId", message);
        else
        {
            Assert.Contains(typeId, message);
            Assert.Contains(typeName, message);
            Assert.Contains("SSAS Tabular", message);
            if (typeId is "meta:type:Int32" or "sqlserver:type:int")
            {
                Assert.Contains("DataTypeSystem 'Other'", message);
                Assert.Contains("ID 'Meta'", message);
            }
            else
            {
                Assert.Contains("DataTypeSystem 'Meta'", message);
                Assert.Contains("ID 'chosen-system'", message);
            }
        }
    }

    private static string NewRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "tabular-selected-catalog", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }
}
