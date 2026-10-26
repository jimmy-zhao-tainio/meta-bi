using System.Globalization;
using ExampleFileProvider;
using Meta.Integration;
using MetaDataType;
using MetaDataTypeConversion;
using MetaDataTypeConversion.Core;
using MetaPipeline;
using MetaSchema;
using MetaTransform.Binding;
using MetaTransformScript.Sql;
using Microsoft.Data.SqlClient;
using Xunit.Abstractions;

namespace MetaSchemaAdapter.Tests;

// This opt-in diagnostic records a known integration gap, not the desired execution contract.
// Supply ordinary, CLI-authored workspaces and rerun the same compiled test after editing data.
public sealed class CustomConversionBindingProbeTests(ITestOutputHelper output)
{
    [CustomConversionProbeFact]
    public async Task ManualProbe_CustomConversionIsAcceptedByBindingButNotExecutedByBulkCopy()
    {
        var root = Path.GetFullPath(Environment.GetEnvironmentVariable(CustomConversionProbeFactAttribute.RootVariable)!);
        var conversionPath = Path.Combine(root, "ConversionWorkspace");
        var sourceSchemaPath = Path.Combine(root, "SourceSchema");
        var targetSchemaPath = Path.Combine(root, "TargetSchema");
        var aliasTargetSchemaPath = Path.Combine(root, "AliasTargetSchema");
        var dataPath = Path.Combine(root, "Data");
        var runPath = Path.Combine(root, "Run-" + Guid.NewGuid().ToString("N"));
        var transformPath = Path.Combine(runPath, "TransformWorkspace");
        var bindingPath = Path.Combine(runPath, "BindingWorkspace");
        var aliasTransformPath = Path.Combine(runPath, "AliasTransformWorkspace");
        var aliasBindingPath = Path.Combine(runPath, "AliasBindingWorkspace");
        var expected = int.Parse(await File.ReadAllTextAsync(Path.Combine(root, "Expected.txt")), CultureInfo.InvariantCulture);
        var sourceLines = await File.ReadAllLinesAsync(Path.Combine(dataPath, "Wacky.tsv"));
        Assert.Equal("WackyValue", sourceLines[0]);
        var rawValue = Assert.Single(sourceLines.Skip(1));
        var conversion = await TypedWorkspaceModelMapper.LoadAsync<MetaDataTypeConversionModel>(conversionPath, searchUpward: false);
        var sourceSchema = await TypedWorkspaceModelMapper.LoadAsync<MetaSchemaModel>(sourceSchemaPath, searchUpward: false);
        var mapping = conversion.DataTypeMappingList.Single(row => row.Id == "probe:mapping:reversed");
        var implementation = Assert.IsType<ConversionImplementation>(mapping.ConversionImplementation);
        var snippet = Assert.IsType<SqlConversionSnippet>(implementation.SqlConversionSnippet);
        Assert.Equal(mapping.SourceDataTypeId, Assert.Single(sourceSchema.FieldList).MetaDataTypeId);
        Assert.Equal("sqlserver:type:int", mapping.TargetDataTypeId);
        var conversionService = new MetaDataTypeConversionService();
        var selected = Assert.Single(conversionService.ResolveCompatibility(
            conversion, mapping.SourceDataTypeId, mapping.TargetDataTypeId).Path);
        Assert.Equal(mapping.Id, selected.MappingId);
        Assert.Equal(implementation.Id, selected.ConversionImplementationId);

        var syntax = new MetaTransformScriptSqlService();
        var aliasImported = await syntax.ImportFromSqlCodeToXmlWorkspaceAsync(
            "SELECT src.WackyValue AS Value FROM files.Wacky AS src;",
            "dbo.Numbers", aliasTransformPath, "AliasedWackyConversionProjection");
        var aliasTransform = Assert.Single(aliasImported.Model.TransformScriptList);
        var aliasBinding = new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(
            aliasTransformPath, [sourceSchemaPath], aliasTargetSchemaPath,
            Assert.Single(sourceSchema.SystemList).Name, "files", aliasBindingPath,
            dataTypeConversionWorkspacePath: conversionPath);
        Assert.Equal(0, aliasBinding.ErrorCount);
        Assert.Single(aliasBinding.Model.ValidationTargetColumnLinkList);
        Assert.Empty(aliasBinding.Model.ValidationTargetColumnTypeSanctionedConversionList);
        Assert.Empty(aliasBinding.Model.ValidationTargetColumnTypeExactList);
        var aliasBound = Assert.Single(aliasBinding.Model.TransformBindingList);
        var aliasDefinition = new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
            aliasTransformPath, aliasBindingPath, aliasTransform.Id, aliasBound.Id);
        var aliasColumn = Assert.Single(aliasDefinition.RowStreamShape!.Columns);
        Assert.Null(aliasColumn.SourceMetaDataTypeId);
        Assert.Null(aliasColumn.TargetMetaDataTypeId);
        var aliasEvidence = $"""
            Aliased projection: {aliasDefinition.SourceSql}
            Alias binding errors: {aliasBinding.ErrorCount}
            Alias target column links: {aliasBinding.Model.ValidationTargetColumnLinkList.Count}
            Alias sanctioned/exact assessments: {aliasBinding.Model.ValidationTargetColumnTypeSanctionedConversionList.Count}/{aliasBinding.Model.ValidationTargetColumnTypeExactList.Count}
            Alias pipeline source/target type IDs: {aliasColumn.SourceMetaDataTypeId ?? "<null>"}/{aliasColumn.TargetMetaDataTypeId ?? "<null>"}
            Alias transform workspace: {aliasTransformPath}
            Alias binding workspace: {aliasBindingPath}
            """;
        output.WriteLine(aliasEvidence);
        await File.WriteAllTextAsync(Path.Combine(runPath, "AliasProbeResult.txt"), aliasEvidence);

        var imported = await syntax.ImportFromSqlCodeToXmlWorkspaceAsync(
            "SELECT src.WackyValue FROM files.Wacky AS src;",
            "dbo.Numbers", transformPath, "WackyConversionProjection");
        var transform = Assert.Single(imported.Model.TransformScriptList);
        var authoredSql = syntax.ExportToSqlCode(imported.Model, transform.Name);
        var binding = new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(
            transformPath, [sourceSchemaPath], targetSchemaPath,
            Assert.Single(sourceSchema.SystemList).Name, "files", bindingPath,
            dataTypeConversionWorkspacePath: conversionPath);
        Assert.Equal(0, binding.ErrorCount);
        var assessment = Assert.Single(binding.Model.ValidationTargetColumnTypeSanctionedConversionList);
        Assert.Equal(mapping.SourceDataTypeId, assessment.SourceMetaDataTypeId);
        Assert.Equal(mapping.TargetDataTypeId, assessment.TargetMetaDataTypeId);
        var bound = Assert.Single(binding.Model.TransformBindingList);
        var definition = new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
            transformPath, bindingPath, transform.Id, bound.Id);
        Assert.Equal(authoredSql, definition.SourceSql);
        var column = Assert.Single(definition.RowStreamShape!.Columns);
        Assert.Equal(mapping.SourceDataTypeId, column.SourceMetaDataTypeId);
        Assert.Equal(mapping.TargetDataTypeId, column.TargetMetaDataTypeId);

        var sourceAdapter = new TabSeparatedTextAdapter(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["WACKY_FILES"] = dataPath
        });
        var source = await sourceAdapter.CreateRowStreamSourceAsync(new MetaSchemaRowStreamRequest(
            "WACKY_FILES", sourceSchema, imported.Model, binding.Model, transform.Id, bound.Id,
            definition.RowStreamShape, BatchSize: 1));
        await using (var batches = source.ReadBatchesAsync().GetAsyncEnumerator())
        {
            Assert.True(await batches.MoveNextAsync());
            Assert.Equal(rawValue, Assert.Single(batches.Current.Rows)[0]);
        }

        var generated = conversionService.GenerateSqlExpression(conversion, MetaDataTypeInstance.BuiltIn,
            new(mapping.SourceDataTypeId, "SqlServer", "@raw", new()));
        var expressionSql = syntax.ExportScalarExpression(generated.Expression);
        var databaseName = "MetaBiCustomConversionProbe_" + Guid.NewGuid().ToString("N");
        const string masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false;Pooling=false";
        var connectionString = new SqlConnectionStringBuilder(masterConnectionString) { InitialCatalog = databaseName }.ConnectionString;
        await using var master = new SqlConnection(masterConnectionString);
        await master.OpenAsync();
        var created = false;
        try
        {
            using (var create = new SqlCommand($"CREATE DATABASE [{databaseName}]", master))
                await create.ExecuteNonQueryAsync();
            created = true;
            await using var database = new SqlConnection(connectionString);
            await database.OpenAsync();
            using (var createTable = new SqlCommand("CREATE TABLE dbo.Numbers ([WackyValue] int NOT NULL)", database))
                await createTable.ExecuteNonQueryAsync();
            int helperResult;
            using (var command = new SqlCommand("SELECT " + expressionSql, database))
            {
                command.Parameters.AddWithValue("@raw", rawValue);
                helperResult = Convert.ToInt32(await command.ExecuteScalarAsync(), CultureInfo.InvariantCulture);
            }
            Assert.Equal(expected, helperResult);

            var targetAdapter = new ProbeSqlServerTargetAdapter(connectionString, conversionPath);
            var execution = Assert.IsType<MetaSchemaAdapterRowStreamExecutionResult>(
                await new MetaSchemaAdapterExecutionService().ExecuteAsync(sourceAdapter, targetAdapter,
                    new MetaSchemaAdapterExecutionRequest(
                        sourceSchemaPath, targetSchemaPath, transformPath, bindingPath,
                        "WACKY_FILES", "PROBE_SQLSERVER", transform.Id, bound.Id, BatchSize: 1)));
            int insertedRows;
            using (var count = new SqlCommand("SELECT COUNT(*) FROM dbo.Numbers", database))
                insertedRows = Convert.ToInt32(await count.ExecuteScalarAsync(), CultureInfo.InvariantCulture);
            var evidence = $"""
                Custom conversion integration gap probe
                Source value: {rawValue}
                Source type: {mapping.SourceDataTypeId}
                Destination type: {mapping.TargetDataTypeId}
                Mapping: {mapping.Id}
                Implementation: {implementation.Id}
                Snippet: {snippet.Id}
                Authored snippet: {snippet.SqlText}
                Binding errors: {binding.ErrorCount}
                Sanctioned conversion assessments: {binding.Model.ValidationTargetColumnTypeSanctionedConversionList.Count}
                Bound SQL (unchanged): {definition.SourceSql}
                Pipeline shape carries source/target IDs, but no selected implementation or snippet.
                Helper-only SQL control: {expressionSql}
                Helper-only SQL expected/result: {expected}/{helperResult}
                Adapter execution succeeded: {execution.PipelineResult.Succeeded}
                Adapter failure stage: {execution.PipelineResult.FailureStage}
                Adapter failure: {execution.PipelineResult.FailureMessage}
                Inserted rows: {insertedRows}
                Generated transform workspace: {transformPath}
                Generated binding workspace: {bindingPath}
                {aliasEvidence}
                This records a known gap; helper success is not adapter conversion integration.
                """;
            output.WriteLine(evidence);
            await File.WriteAllTextAsync(Path.Combine(runPath, "ProbeResult.txt"), evidence);
            await File.WriteAllTextAsync(Path.Combine(root, "ProbeResult.txt"), evidence);
            Assert.False(execution.PipelineResult.Succeeded);
            Assert.Equal(PipelineExecutionFailureStage.TargetWrite, execution.PipelineResult.FailureStage);
            Assert.Contains("System.Int32", execution.PipelineResult.FailureMessage, StringComparison.Ordinal);
            Assert.Contains("could not be coerced", execution.PipelineResult.FailureMessage, StringComparison.Ordinal);
            Assert.Equal(0, insertedRows);
        }
        finally
        {
            if (created)
            {
                using var drop = new SqlCommand($"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}]", master);
                await drop.ExecuteNonQueryAsync();
            }
        }
    }

    // Transport-only fixture: no conversion logic or evaluation is implemented here.
    private sealed class ProbeSqlServerTargetAdapter(string connectionString, string conversionPath) : IMetaSchemaTargetWriteAdapter
    {
        public string Id => "probe-sqlserver-bulk-copy";

        public ValueTask<IPipelineTargetWriteOperation> CreateInsertRowsOperationAsync(
            MetaSchemaInsertRowsRequest request, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult<IPipelineTargetWriteOperation>(new SqlServerBulkInsertTargetWriteOperation(
                connectionString, request.TargetIdentifier, request.Shape,
                request.ExecutionContext, dataTypeConversionWorkspacePath: conversionPath,
                timeoutSeconds: request.TimeoutSeconds));
        }
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class CustomConversionProbeFactAttribute : FactAttribute
{
    public const string RootVariable = "META_BI_CUSTOM_CONVERSION_PROBE_ROOT";

    public CustomConversionProbeFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(RootVariable)))
            Skip = $"Manual integration-gap probe; set {RootVariable} to CLI-authored fixture workspaces.";
    }
}
