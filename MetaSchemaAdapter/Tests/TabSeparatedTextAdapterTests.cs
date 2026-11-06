using ExampleFileProvider;
using MetaDataType;
using Meta.Integration;
using MetaPipeline;
using MetaSchema;
using MetaTransform.Binding;
using MetaTransformScript.Sql;
using MetaTransformScript.Sql.Parsing;

namespace MetaSchemaAdapter.Tests;

[Trait("Category", "Integration")]
public sealed class TabSeparatedTextAdapterTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task TextLiteralProjectionBindsAndExecutesAgainstDiscoveredMaxTextContract(bool selectedCatalog)
    {
        var root = Path.Combine(Path.GetTempPath(), "MetaSchemaAdapterTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            await File.WriteAllTextAsync(Path.Combine(root, "Text.tsv"), "Value\nsource\n");
            await File.WriteAllTextAsync(Path.Combine(root, "Literal.tsv"), "Value\n");
            var catalogPath = Path.Combine(root, "Catalog");
            var catalog = CreateCatalogWorkspace(catalogPath, selectedCatalog);
            var adapter = new TabSeparatedTextAdapter(new Dictionary<string, string> { ["FILES"] = root },
                selectedCatalog ? catalog : null);
            var schema = Path.Combine(root, "Schema");
            var transform = Path.Combine(root, "Transform");
            var bindingPath = Path.Combine(root, "Binding");
            await new MetaSchemaAdapterWorkspaceService().DiscoverToWorkspaceAsync(adapter,
                new MetaSchemaAdapterDiscoveryWorkspaceRequest("FILES", "TextFiles", schema));
            var imported = await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "SELECT N'constant' AS Value FROM files.Text AS src;", "files.Literal", transform, "LiteralProjection");
            var bound = new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(
                transform, [schema], schema, "TextFiles", "files", bindingPath,
                dataTypeWorkspacePath: selectedCatalog ? catalogPath : null);
            Assert.Equal(0, bound.ErrorCount);
            var assessment = Assert.Single(bound.Model.ValidationTargetColumnTypeExactList);
            Assert.Equal(selectedCatalog ? "adapter-user-text" : "sqlserver:type:nvarchar", assessment.SourceMetaDataTypeId);
            var result = Assert.IsType<MetaSchemaAdapterRowStreamExecutionResult>(
                await new MetaSchemaAdapterExecutionService().ExecuteAsync(adapter, adapter,
                    new MetaSchemaAdapterExecutionRequest(schema, schema, transform, bindingPath, "FILES", "FILES",
                        Assert.Single(imported.Model.TransformScriptList).Id,
                        Assert.Single(bound.Model.TransformBindingList).Id)));
            Assert.True(result.PipelineResult.Succeeded, result.PipelineResult.FailureMessage);
            Assert.Equal(1, result.PipelineResult.RowCount);
            Assert.Equal(new[] { "Value", "constant" }, await File.ReadAllLinesAsync(Path.Combine(root, "Literal.tsv")));
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Theory]
    [InlineData("null")]
    [InlineData("dbnull")]
    [InlineData("number")]
    [InlineData("tab")]
    [InlineData("newline")]
    public async Task TargetRejectsValuesTheTsvContractCannotRepresent(string invalidKind)
    {
        var root = Path.Combine(Path.GetTempPath(), "MetaSchemaAdapterTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var file = Path.Combine(root, "Text.tsv");
            await File.WriteAllTextAsync(file, "Value\n");
            var adapter = new TabSeparatedTextAdapter(new Dictionary<string, string> { ["FILES"] = root });
            var schemaPath = Path.Combine(root, "Schema");
            await new MetaSchemaAdapterWorkspaceService().DiscoverToWorkspaceAsync(adapter,
                new MetaSchemaAdapterDiscoveryWorkspaceRequest("FILES", "TextFiles", schemaPath));
            var schema = TypedWorkspaceModelMapper.Load<MetaSchemaModel>(schemaPath, searchUpward: false);
            var shape = new PipelineRowStreamShape([new PipelineColumn("Value", 0)]);
            object? invalid = invalidKind switch
            {
                "null" => null, "dbnull" => DBNull.Value, "number" => 42, "tab" => "a\tb", _ => "a\nb"
            };
            await using (var writer = await adapter.CreateInsertRowsOperationAsync(
                new MetaSchemaInsertRowsRequest("FILES", schema, "files.Text", shape)))
            {
                await writer.BeginAsync();
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => writer.WriteBatchAsync(
                    new PipelineDataBatch(shape, [new object?[] { "valid" }, new object?[] { invalid }])));
                Assert.Contains("non-null text", ex.Message);
                await writer.CompleteAsync();
            }
            Assert.Equal(new[] { "Value" }, await File.ReadAllLinesAsync(file));
        }
        finally { Directory.Delete(root, recursive: true); }
    }

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

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ExternalAdapterDiscoversBindsAndExecutesThroughOrdinaryWorkspaces(bool selectedCatalog)
    {
        var root = Path.Combine(Path.GetTempPath(), "MetaSchemaAdapterTests", Guid.NewGuid().ToString("N"));
        var dataRoot = Path.Combine(root, "Data");
        var schemaWorkspacePath = Path.Combine(root, "SchemaWorkspace");
        var transformWorkspacePath = Path.Combine(root, "TransformWorkspace");
        var bindingWorkspacePath = Path.Combine(root, "BindingWorkspace");
        Directory.CreateDirectory(dataRoot);

        var catalogPath = Path.Combine(root, "Catalog");
        try
        {
            var catalog = CreateCatalogWorkspace(catalogPath, selectedCatalog);
            await File.WriteAllTextAsync(
                Path.Combine(dataRoot, "Sales.tsv"),
                "Product\tChannel\tAmount\n" +
                "Bike\tOnline\t125.00\n" +
                "Helmet\tStore\t30.00\n" +
                "Jersey\tOnline\t45.00\n" +
                "\u00c5sa \u00e4ter r\u00e4ksm\u00f6rg\u00e5s.\tOnline\ttext\n" +
                "Empty\tOnline\t\n");
            await File.WriteAllTextAsync(
                Path.Combine(dataRoot, "OnlineSales.tsv"),
                "Item\tAmount\n");

            var adapter = new TabSeparatedTextAdapter(
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["SALES_FILES"] = dataRoot
                }, selectedCatalog ? catalog : null);

            var discovery = await new MetaSchemaAdapterWorkspaceService().DiscoverToWorkspaceAsync(
                adapter,
                new MetaSchemaAdapterDiscoveryWorkspaceRequest(
                    "SALES_FILES",
                    "SalesFiles",
                    schemaWorkspacePath));
            Assert.Equal("tab-separated-text", discovery.AdapterId);
            Assert.Equal(2, discovery.TableCount);
            Assert.Equal(5, discovery.FieldCount);

            var discovered = TypedWorkspaceModelMapper.Load<MetaSchemaModel>(schemaWorkspacePath, searchUpward: false);
            Assert.All(discovered.FieldList, field =>
            {
                Assert.Equal(selectedCatalog ? "adapter-user-text" : "sqlserver:type:nvarchar", field.MetaDataTypeId);
                Assert.Equal("false", field.IsNullable);
                Assert.Equal("-1", Assert.Single(discovered.FieldDataTypeDetailList, detail => detail.Field.Id == field.Id && detail.Name == "Length").Value);
            });
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
                newWorkspacePath: bindingWorkspacePath,
                dataTypeWorkspacePath: selectedCatalog ? catalogPath : null);
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
            Assert.Equal(4, result.PipelineResult.RowCount);
            Assert.Equal(4, result.PipelineResult.BatchCount);
            Assert.Equal(
                new[]
                {
                    "Item\tAmount",
                    "Bike\t125.00",
                    "Jersey\t45.00",
                    "\u00c5sa \u00e4ter r\u00e4ksm\u00f6rg\u00e5s.\ttext",
                    "Empty\t"
                },
                await File.ReadAllLinesAsync(Path.Combine(dataRoot, "OnlineSales.tsv")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Theory]
    [InlineData("SELECT src.Amount + ' extra' AS Amount FROM files.Sales AS src;")]
    [InlineData("SELECT TOP (1) src.Amount FROM files.Sales AS src;")]
    [InlineData("SELECT DISTINCT src.Amount FROM files.Sales AS src;")]
    [InlineData("SELECT src.Amount FROM files.Sales AS src ORDER BY src.Amount;")]
    public async Task ExternalAdapterRejectsUnsupportedSemanticShapeClearly(string sql)
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
            await File.WriteAllTextAsync(Path.Combine(root, "Result.tsv"), "Amount\n");
            var schemaPath = Path.Combine(root, "Schema");
            var transformPath = Path.Combine(root, "Transform");
            var bindingPath = Path.Combine(root, "Binding");
            await new MetaSchemaAdapterWorkspaceService().DiscoverToWorkspaceAsync(adapter,
                new MetaSchemaAdapterDiscoveryWorkspaceRequest("SALES_FILES", "SalesFiles", schemaPath));
            var imported = await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(sql,
                "files.Result", transformPath, "UnsupportedExpression");
            var bound = new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(transformPath,
                [schemaPath], schemaPath, "SalesFiles", "files", bindingPath);
            Assert.Equal(0, bound.ErrorCount);
            var schema = TypedWorkspaceModelMapper.Load<MetaSchemaModel>(schemaPath, searchUpward: false);
            var transforms = TypedWorkspaceModelMapper.Load<MetaTransformScript.MetaTransformScriptModel>(transformPath, searchUpward: false);
            var binding = TypedWorkspaceModelMapper.Load<MetaTransformBinding.MetaTransformBindingModel>(bindingPath, searchUpward: false);
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

            Assert.Contains("supports", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static MetaDataTypeModel CreateCatalogWorkspace(string path, bool arbitraryIds)
    {
        var catalog = MetaDataTypeInstance.CreateBuiltIn();
        if (arbitraryIds)
        {
            catalog.DataTypeList.Single(type => type.Name == "nvarchar" && type.DataTypeSystem.Name == "SqlServer").Id = "adapter-user-text";
            catalog.DataTypeSystemList.Single(system => system.Name == "SqlServer").Id = "adapter-user-sqlsystem";
        }
        TypedWorkspaceModelMapper.Create(catalog, path, "xml");
        return TypedWorkspaceModelMapper.Load<MetaDataTypeModel>(path, searchUpward: false);
    }
}
