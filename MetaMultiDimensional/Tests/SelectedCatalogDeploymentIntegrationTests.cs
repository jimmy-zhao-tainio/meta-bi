using System.Reflection;
using Meta.Integration;
using Meta.Operations.Domain;
using Meta.Surfaces;
using MetaBi.Tests.Common;
using MetaDataType;
using MetaDataType.Core;
using MetaMultiDimensional.Core;
using MetaMultiDimensional.Core.Deploy;
using Amo = Microsoft.AnalysisServices;

namespace MetaMultiDimensional.Tests;

[Trait("Category", "Integration")]
public sealed class SelectedCatalogDeploymentIntegrationTests
{
    // Type validation must reject these workspaces before attempting to use this server.
    private const string InvalidServer = "must-not-connect;invalid-connection-token";

    [Theory]
    [InlineData("xml", "chosen-double")]
    [InlineData("csharp", "chosen-double")]
    [InlineData("xml", "chosen-decimal")]
    [InlineData("csharp", "chosen-decimal")]
    [InlineData("xml", "chosen-numeric")]
    [InlineData("csharp", "chosen-numeric")]
    public async Task PersistedArbitraryTypeIdsProduceTheExistingNativeTypes(string representation, string measureTypeId)
    {
        var root = NewRoot();
        try
        {
            var catalogPath = await SaveCatalog(root, representation);
            var sourcePath = await SaveModel(root, representation, "chosen-integer", includeMeasure: true, measureTypeId: measureTypeId);
            var source = await TypedWorkspaceModelMapper.LoadAsync(sourcePath, new MetaMultiDimensionalWorkspaceBinding());
            var catalog = MetaDataTypeWorkspaceProvider.LoadOrDefault(catalogPath);
            Assert.Same(Assert.Single(source.DimensionList), source.DimensionAttributeList[0].Dimension);
            var method = typeof(MetaMultiDimensionalDeployService).GetMethod("BuildDatabaseWithCatalog", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(method);
            var database = Assert.IsType<Amo.Database>(method.Invoke(null, [source, Assert.Single(source.MultiDimensionalDatabaseList), "Selected", catalog]));
            var dimension = Assert.Single(database.Dimensions.Cast<Amo.Dimension>());
            Assert.Equal(new Amo.DataItem("Values", "Number", Amo.DataType.Int32).Type, Assert.Single(dimension.Attributes["attribute_number"].KeyColumns.Cast<Amo.DataItem>()).Type);
            Assert.Equal(new Amo.DataItem("Values", "Text", Amo.DataType.String).Type, Assert.Single(dimension.Attributes["attribute_text"].KeyColumns.Cast<Amo.DataItem>()).Type);
            Assert.Equal(new Amo.DataItem("Values", "Bytes", Amo.DataType.Binary).Type, Assert.Single(dimension.Attributes["attribute_binary"].KeyColumns.Cast<Amo.DataItem>()).Type);
            var measure = Assert.Single(Assert.Single(database.Cubes.Cast<Amo.Cube>()).MeasureGroups.Cast<Amo.MeasureGroup>()).Measures.Cast<Amo.Measure>().Single();
            Assert.Equal(Amo.MeasureDataType.Double, measure.DataType);
            Assert.Equal(new Amo.DataItem("Facts", "Value", Amo.DataType.Double).Type, measure.Source.Type);
            Assert.Equal(typeof(double), Assert.Single(database.DataSourceViews.Cast<Amo.DataSourceView>()).Schema.Tables["Facts"]!.Columns["Value"]!.DataType);
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    public static IEnumerable<object?[]> UnsupportedUsages()
    {
        foreach (var representation in new[] { "xml", "csharp" })
        {
            yield return [representation, "dimension attribute", "chosen-unmapped", "Unmapped", "Meta"];
            yield return [representation, "dimension attribute", "chosen-guid", "Guid", "Meta"];
            yield return [representation, "dimension attribute", "chosen-temporal", "DateTime2", "Meta"];
            yield return [representation, "dimension attribute", "meta:type:Int32", "Int32", "Other"];
            yield return [representation, "dimension attribute", "", null, null];
            yield return [representation, "measure", "chosen-string", "String", "Meta"];
            yield return [representation, "measure", "chosen-binary", "Binary", "Meta"];
            yield return [representation, "measure", "chosen-unmapped", "Unmapped", "Meta"];
            yield return [representation, "measure", "chosen-guid", "Guid", "Meta"];
            yield return [representation, "measure", "chosen-temporal", "DateTime2", "Meta"];
            yield return [representation, "measure", "meta:type:Int32", "Int32", "Other"];
            yield return [representation, "measure", null, null, null];
        }
    }

    [Theory]
    [MemberData(nameof(UnsupportedUsages))]
    public async Task PersistedUnsupportedUsageIsRejectedBeforeConnectingThroughApiAndCli(
        string representation, string usage, string? typeId, string? typeName, string? systemName)
    {
        var root = NewRoot();
        try
        {
            var catalogPath = await SaveCatalog(root, representation);
            var isMeasure = usage == "measure";
            var attributeTypeId = isMeasure ? "chosen-integer" : typeId
                ?? throw new InvalidOperationException("Required attribute fixtures use an empty value to represent a missing type.");
            var sourcePath = await SaveModel(root, representation,
                attributeTypeId, includeMeasure: isMeasure, measureTypeId: typeId);
            var originalSource = await TypedWorkspaceModelMapper.LoadStateAsync(sourcePath);
            var originalCatalog = await TypedWorkspaceModelMapper.LoadStateAsync(catalogPath);
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => new MetaMultiDimensionalDeployService().DeployAsync(new MetaMultiDimensionalDeployRequest
            {
                WorkspacePath = sourcePath, Server = InvalidServer, DataTypeWorkspacePath = catalogPath, Process = false,
            }));
            var field = isMeasure ? "Measure 'measure-value'" : "DimensionAttribute 'attribute-number'";
            var name = isMeasure ? "Value" : "Number";
            AssertDiagnostic(exception.Message);
            var cli = CliTestRunner.RunStandardCli("meta-multi-dimensional", $"deploy --workspace \"{sourcePath}\" --server {InvalidServer} --data-type-workspace \"{catalogPath}\" --no-process");
            Assert.Equal(4, cli.ExitCode);
            AssertDiagnostic(cli.Output);
            Assert.Null(InMemoryWorkspaceComparer.FindDifference(originalSource, await TypedWorkspaceModelMapper.LoadStateAsync(sourcePath)));
            Assert.Null(InMemoryWorkspaceComparer.FindDifference(originalCatalog, await TypedWorkspaceModelMapper.LoadStateAsync(catalogPath)));

            void AssertDiagnostic(string diagnostic)
            {
                Assert.Contains(field, diagnostic);
                Assert.Contains("'" + name + "'", diagnostic);
                Assert.Contains("SSAS " + usage + " mapping", diagnostic);
                if (string.IsNullOrWhiteSpace(typeId))
                    Assert.Contains("missing DataTypeId", diagnostic);
                else
                {
                    Assert.Contains("'" + typeId + "'", diagnostic);
                    Assert.Contains("'" + typeName + "'", diagnostic);
                    Assert.Contains("'" + systemName + "'", diagnostic);
                }
            }
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
            new DataType { Id = "chosen-string", Name = "String", DataTypeSystem = meta },
            new DataType { Id = "chosen-binary", Name = "Binary", DataTypeSystem = meta },
            new DataType { Id = "chosen-double", Name = "Double", DataTypeSystem = meta },
            new DataType { Id = "chosen-decimal", Name = "Decimal", DataTypeSystem = meta },
            new DataType { Id = "chosen-numeric", Name = "Numeric", DataTypeSystem = meta },
            new DataType { Id = "chosen-guid", Name = "Guid", DataTypeSystem = meta },
            new DataType { Id = "chosen-temporal", Name = "DateTime2", DataTypeSystem = meta },
            new DataType { Id = "chosen-unmapped", Name = "Unmapped", DataTypeSystem = meta },
            new DataType { Id = "meta:type:Int32", Name = "Int32", DataTypeSystem = foreign },
        ]);
        var path = Path.Combine(root, "catalog");
        await WorkspaceSurface.CreateAsync(new MetaDataTypeWorkspaceBinding().Write(catalog, defaults.Model), path, representation);
        return path;
    }

    private static async Task<string> SaveModel(string root, string representation, string integerTypeId,
        bool includeMeasure = false, string? measureTypeId = null)
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
            new DimensionAttribute { Id = "attribute-text", Name = "Text", Ordinal = "2", DataTypeId = "chosen-string", Dimension = dimension },
            new DimensionAttribute { Id = "attribute-binary", Name = "Bytes", Ordinal = "3", DataTypeId = "chosen-binary", Dimension = dimension },
        ]);
        if (includeMeasure)
        {
            var cube = new Cube { Id = "cube", Name = "Selected", MultiDimensionalDatabase = database };
            var group = new MeasureGroup { Id = "group", Name = "Facts", Cube = cube };
            model.CubeList.Add(cube);
            model.MeasureGroupList.Add(group);
            model.MeasureList.Add(new Measure
            {
                Id = "measure-value", Name = "Value", DataTypeId = measureTypeId, AggregateFunction = "Sum", MeasureGroup = group,
            });
        }
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
