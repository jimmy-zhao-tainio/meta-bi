using Meta.Integration;
using MetaBi.Tests.Common;
using MetaDataType;
using Microsoft.Data.SqlClient;

namespace MetaPipeline.Tests;

[Trait("Category", "Integration")]
public sealed class SqlServerContractExecutionIntegrationTests
{
    [Theory]
    [InlineData("int", false)]
    [InlineData("int", true)]
    [InlineData("nvarchar", false)]
    [InlineData("nvarchar", true)]
    public async Task BulkCopyPreservesContractValues_AndRejectsCoercionBeforeWriting(string type, bool arbitraryIds)
    {
        var root = Path.Combine(Path.GetTempPath(), "MetaBiBulkContract", Guid.NewGuid().ToString("N"));
        var database = "MetaBiBulkContract_" + Guid.NewGuid().ToString("N");
        await using var master = new SqlConnection("Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false;Pooling=false");
        await master.OpenAsync();
        await using (var create = new SqlCommand($"CREATE DATABASE [{database}]", master)) await create.ExecuteNonQueryAsync();
        try
        {
            var catalog = Catalog(arbitraryIds);
            var path = Path.Combine(root, "Catalog");
            TypedWorkspaceModelMapper.Create(catalog, path, "xml");
            var id = catalog.DataTypeList.Single(row => row.Name == type && row.DataTypeSystem.Name == "SqlServer").Id;
            var shape = new PipelineRowStreamShape([new PipelineColumn("Value", 0, id, id)]);
            var cs = new SqlConnectionStringBuilder(master.ConnectionString) { InitialCatalog = database }.ConnectionString;
            await using var db = new SqlConnection(cs);
            await db.OpenAsync();
            await using (var setup = new SqlCommand($"CREATE TABLE dbo.TargetRows (Value {(type == "int" ? "int" : "nvarchar(50)")} NULL)", db)) await setup.ExecuteNonQueryAsync();
            await using var writer = new SqlServerBulkCopyRowStreamWriter(cs, "dbo.TargetRows", shape,
                dataTypeWorkspacePath: arbitraryIds ? path : null);
            object?[] expected = type == "int" ? [12, null, -7] : ["ordinary text", "Åsa äter räksmörgås.", null, ""];
            object[] invalid = type == "int" ? ["12", 12L] : [12];
            foreach (var value in invalid)
            {
                var error = await Assert.ThrowsAsync<MetaPipelineConfigurationException>(() => writer.WriteBatchAsync(
                    new PipelineDataBatch(shape, [new object?[] { expected[0] }, new object?[] { value }])));
                Assert.Contains("CLR type", error.Message);
                Assert.Contains("TransformScript", error.Message);
                await using var count = new SqlCommand("SELECT COUNT(*) FROM dbo.TargetRows", db);
                Assert.Equal(0, (int)(await count.ExecuteScalarAsync())!);
            }
            await writer.WriteBatchAsync(new PipelineDataBatch(shape, expected.Select(value => new object?[] { value }).ToArray()));
            await using var read = new SqlCommand("SELECT Value FROM dbo.TargetRows", db);
            await using var reader = await read.ExecuteReaderAsync();
            var actual = new List<object?>();
            while (await reader.ReadAsync()) actual.Add(reader.IsDBNull(0) ? null : reader.GetValue(0));
            // Compare multisets: heaps do not promise row order.
            Assert.Equal(expected.Length, actual.Count);
            foreach (var value in expected) Assert.True(actual.Remove(value), $"Missing exact value '{value ?? "SQL NULL"}'.");
            Assert.Empty(actual);
        }
        finally
        {
            await using var drop = new SqlCommand($"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{database}]", master);
            await drop.ExecuteNonQueryAsync();
            await using var check = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name=@name", master);
            check.Parameters.AddWithValue("@name", database);
            Assert.Equal(0, (int)(await check.ExecuteScalarAsync())!);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [Theory]
    [InlineData("missing")]
    [InlineData("unknown")]
    [InlineData("foreign")]
    [InlineData("lookalike")]
    public void SelectedCatalogRejectsInvalidTypeContractsBeforeConnecting(string kind)
    {
        var root = Path.Combine(Path.GetTempPath(), "MetaBiBulkCatalog", Guid.NewGuid().ToString("N"));
        try
        {
            var catalog = Catalog(true);
            var foreign = catalog.DataTypeList.Single(row => row.Id == "meta:type:String");
            if (kind == "lookalike") foreign.Id = "sqlserver:type:pretender";
            var id = kind switch { "missing" => null, "unknown" => "sqlserver:type:int", _ => foreign.Id };
            TypedWorkspaceModelMapper.Create(catalog, root, "xml");
            var error = Assert.Throws<MetaPipelineConfigurationException>(() => new SqlServerBulkCopyRowStreamWriter(
                "Server=DoesNotExist;Database=unused", "dbo.TargetRows", new([new PipelineColumn("Value", 0, id, id)]),
                dataTypeWorkspacePath: root));
            Assert.Contains("Value", error.Message);
            Assert.Contains(kind == "missing" ? "missing" : kind == "unknown" ? "selected MetaDataType catalog" : "does not belong", error.Message);
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    [Theory]
    [InlineData("execute")]
    [InlineData("execute-step")]
    [InlineData("execute-worker")]
    [InlineData("execute-sqlserver")]
    public void ExecutionCliExposesCatalogAndRejectsConversionWorkspace(string command)
    {
        var help = CliTestRunner.RunStandardCli("meta-pipeline", command + " --help");
        Assert.Equal(0, help.ExitCode);
        Assert.Contains("--data-type-workspace", help.Output);
        Assert.DoesNotContain("--data-type-conversion-workspace", help.Output);
        var retired = CliTestRunner.RunStandardCli("meta-pipeline", command + " --data-type-conversion-workspace unused");
        Assert.NotEqual(0, retired.ExitCode);
        Assert.Contains("data-type-conversion-workspace", retired.Output);
    }

    private static MetaDataTypeModel Catalog(bool arbitraryIds)
    {
        var catalog = MetaDataTypeInstance.CreateBuiltIn();
        if (arbitraryIds)
        {
            foreach (var type in catalog.DataTypeList.Where(type => type.DataTypeSystem.Name == "SqlServer")) type.Id = "user-" + type.Name;
            catalog.DataTypeSystemList.Single(system => system.Name == "SqlServer").Id = "custom-system";
        }
        return catalog;
    }
}
