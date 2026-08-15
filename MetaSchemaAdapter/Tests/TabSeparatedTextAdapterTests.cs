using ExampleFileProvider;
using MetaPipeline;
using MetaSchema;
using MetaTransform.Binding;
using MetaTransformScript.Sql;
using MetaTransformScript.Sql.Parsing;

namespace MetaSchemaAdapter.Tests;

public sealed class TabSeparatedTextAdapterTests
{
    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task DiscoveryCreatesARepresentationNeutralMetaSchemaWorkspace(string representation)
    {
        var root = Path.Combine(Path.GetTempPath(), "MetaSchemaAdapterTests", Guid.NewGuid().ToString("N"));
        var dataRoot = Path.Combine(root, "Data");
        var workspacePath = Path.Combine(root, "SchemaWorkspace");
        Directory.CreateDirectory(dataRoot);

        try
        {
            await File.WriteAllTextAsync(
                Path.Combine(dataRoot, "Products.tsv"),
                "ProductId\tProductName\n" +
                "1\tBike\n");
            var adapter = new TabSeparatedTextAdapter(
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["PRODUCT_FILES"] = dataRoot
                });

            var result = await new MetaSchemaAdapterWorkspaceService().DiscoverToWorkspaceAsync(
                adapter,
                new MetaSchemaAdapterDiscoveryWorkspaceRequest(
                    "PRODUCT_FILES",
                    "ProductFiles",
                    workspacePath,
                    representation));
            var reloaded = await Meta.Integration.TypedWorkspaceModelMapper.LoadAsync<MetaSchemaModel>(
                workspacePath,
                searchUpward: false);

            Assert.Equal(representation, result.Representation);
            Assert.Single(reloaded.SystemList);
            Assert.Single(reloaded.TableList);
            Assert.Equal(2, reloaded.FieldList.Count);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task ExternalAdapterDiscoversBindsAndExecutesThroughOrdinaryWorkspaces()
    {
        var root = Path.Combine(Path.GetTempPath(), "MetaSchemaAdapterTests", Guid.NewGuid().ToString("N"));
        var dataRoot = Path.Combine(root, "Data");
        var schemaWorkspacePath = Path.Combine(root, "SchemaWorkspace");
        var transformWorkspacePath = Path.Combine(root, "TransformWorkspace");
        var bindingWorkspacePath = Path.Combine(root, "BindingWorkspace");
        Directory.CreateDirectory(dataRoot);

        try
        {
            await File.WriteAllTextAsync(
                Path.Combine(dataRoot, "Sales.tsv"),
                "Product\tChannel\tAmount\n" +
                "Bike\tOnline\t125.00\n" +
                "Helmet\tStore\t30.00\n" +
                "Jersey\tOnline\t45.00\n");
            await File.WriteAllTextAsync(
                Path.Combine(dataRoot, "OnlineSales.tsv"),
                "Item\tAmount\n");

            var adapter = new TabSeparatedTextAdapter(
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["SALES_FILES"] = dataRoot
                });

            var discovery = await new MetaSchemaAdapterWorkspaceService().DiscoverToWorkspaceAsync(
                adapter,
                new MetaSchemaAdapterDiscoveryWorkspaceRequest(
                    "SALES_FILES",
                    "SalesFiles",
                    schemaWorkspacePath));
            Assert.Equal("tab-separated-text", discovery.AdapterId);
            Assert.Equal(2, discovery.TableCount);
            Assert.Equal(5, discovery.FieldCount);

            var transformImport = await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                """
                SELECT src.Product AS Item, src.Amount
                FROM files.Sales AS src
                WHERE src.Channel = 'Online';
                """,
                "files.OnlineSales",
                transformWorkspacePath,
                "OnlineSalesProjection");
            var transform = Assert.Single(transformImport.Model.TransformScriptList);

            var binding = new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(
                transformWorkspacePath,
                [schemaWorkspacePath],
                schemaWorkspacePath,
                executeSystemName: "SalesFiles",
                executeSystemDefaultSchemaName: "files",
                newWorkspacePath: bindingWorkspacePath);
            Assert.Equal(0, binding.ErrorCount);
            var transformBinding = Assert.Single(binding.Model.TransformBindingList);

            var result = Assert.IsType<MetaSchemaAdapterRowStreamExecutionResult>(
                await new MetaSchemaAdapterExecutionService().ExecuteAsync(
                    adapter,
                    adapter,
                    new MetaSchemaAdapterExecutionRequest(
                        schemaWorkspacePath,
                        schemaWorkspacePath,
                        transformWorkspacePath,
                        bindingWorkspacePath,
                        "SALES_FILES",
                        "SALES_FILES",
                        transform.Id,
                        transformBinding.Id,
                        BatchSize: 1)));

            Assert.True(result.PipelineResult.Succeeded, result.PipelineResult.FailureMessage);
            Assert.Equal(2, result.PipelineResult.RowCount);
            Assert.Equal(2, result.PipelineResult.BatchCount);
            Assert.Equal(
                new[]
                {
                    "Item\tAmount",
                    "Bike\t125.00",
                    "Jersey\t45.00"
                },
                await File.ReadAllLinesAsync(Path.Combine(dataRoot, "OnlineSales.tsv")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task ExternalAdapterRejectsUnsupportedSemanticShapeClearly()
    {
        var root = Path.Combine(Path.GetTempPath(), "MetaSchemaAdapterTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        try
        {
            await File.WriteAllTextAsync(
                Path.Combine(root, "Sales.tsv"),
                "Product\tAmount\n" +
                "Bike\t125.00\n");

            var adapter = new TabSeparatedTextAdapter(
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["SALES_FILES"] = root
                });
            var schema = await adapter.DiscoverSchemaAsync(
                new MetaSchemaDiscoveryRequest("SALES_FILES", "SalesFiles"));
            var transforms = new MetaTransformScriptSqlParser().ParseSqlCode(
                "SELECT src.Amount + ' extra' AS Amount FROM files.Sales AS src;",
                bareSelectName: "UnsupportedExpression");
            var binding = new TransformBindingService().BindSingleTransformModel(transforms, schema);
            var transform = Assert.Single(transforms.TransformScriptList);
            var transformBinding = Assert.Single(binding.TransformBindingList);

            var exception = await Assert.ThrowsAsync<NotSupportedException>(async () =>
                await adapter.CreateRowStreamSourceAsync(
                    new MetaSchemaRowStreamRequest(
                        "SALES_FILES",
                        schema,
                        transforms,
                        binding,
                        transform.Id,
                        transformBinding.Id,
                        new PipelineRowStreamShape([new PipelineColumn("Amount", 0)]))));

            Assert.Contains("column and literal scalar expressions", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
