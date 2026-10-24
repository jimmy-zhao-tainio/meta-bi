using Meta.Integration;
using MetaDataType;
using MetaDataTypeConversion.Core;
using MetaTransformScript.Sql;
using Microsoft.Data.SqlClient;

namespace MetaDataTypeConversion.Tests;

// Explicit integration coverage; the repository's integration suite uses local SQL Server.
public sealed class SqlValueConversionSqlServerTests
{
    [Fact]
    public async Task DisposableSqlWorkspace_RoundTripsSnippetsAndBindings_AndExecutesStrictParameterizedSql()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var databaseName = "MetaBiValueConversionTest_" + suffix;
        var environmentName = "META_BI_VALUE_CONVERSION_TEST_" + suffix;
        var directory = Path.Combine(Path.GetTempPath(), "MetaDataTypeConversion-sql-tests", suffix);
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var connectionString = new SqlConnectionStringBuilder(masterConnectionString) { InitialCatalog = databaseName }.ConnectionString;
        using var master = new SqlConnection(masterConnectionString);
        await master.OpenAsync();
        var created = false;
        try
        {
            Environment.SetEnvironmentVariable(environmentName, connectionString);
            var model = MetaDataTypeConversionInstance.CreateBuiltIn();
            // The SQL surface requires model name == database name and creates the database itself.
            // Rename only this disposable test materialization, preserving all entity contracts.
            var workspace = TypedWorkspaceModelMapper.ToInMemoryWorkspace(model);
            workspace.Model.Name = databaseName;
            workspace.Instance.ModelName = databaseName;
            await WorkspaceSurface.CreateAsync(workspace, directory, "sql", environmentName);
            created = true;
            var stored = await TypedWorkspaceModelMapper.LoadStateAsync(directory);
            stored.Model.Name = "MetaDataTypeConversion";
            stored.Instance.ModelName = "MetaDataTypeConversion";
            var loaded = TypedWorkspaceModelMapper.FromInMemoryWorkspace(stored, MetaDataTypeConversionModel.CreateEmpty);
            var implementation = loaded.ConversionImplementationList.Single(row => row.Name == "SqlExpression");
            var snippet = loaded.SqlConversionSnippetList.Single(row => row.Id == "sqlserver:snippet:cast");
            Assert.Same(snippet, implementation.SqlConversionSnippet);
            Assert.Equal("CAST($(value) AS $(type))", snippet.SqlText);
            var binding = loaded.SqlConversionParameterBindingList.Single(row => row.SqlConversionSnippet == snippet && row.Name == "value");
            Assert.Same(loaded.SqlConversionParameterList.Single(row => row.Name == "SourceExpression"), binding.SqlConversionParameter);
            var service = new MetaDataTypeConversionService();
            var syntax = new MetaTransformScriptSqlService();
            var value = service.GenerateSqlExpression(loaded, MetaDataTypeInstance.BuiltIn,
                new("sqlserver:type:int", "SqlServer", "(40 + 2) * 3", new(Length: 24)));
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using (var command = new SqlCommand("SELECT " + syntax.ExportScalarExpression(value.Expression), connection))
                Assert.Equal("126", await command.ExecuteScalarAsync());

            // A data-only snippet change adds arithmetic. The bound compound operand must stay grouped.
            snippet.SqlText = "CAST($(value) * 2 AS $(type))";
            var composed = service.GenerateSqlExpression(loaded, MetaDataTypeInstance.BuiltIn,
                new("sqlserver:type:int", "SqlServer", "1 + 2", new(Length: 24)));
            using (var command = new SqlCommand("SELECT " + syntax.ExportScalarExpression(composed.Expression), connection))
                Assert.Equal("6", await command.ExecuteScalarAsync());
            snippet.SqlText = "CAST($(value) AS $(type))";

            loaded.DataTypeMappingList.Single(row => row.SourceDataTypeId == "meta:type:Decimal").ConversionImplementation = implementation;
            var decimalValue = service.GenerateSqlExpression(loaded, MetaDataTypeInstance.BuiltIn,
                new("meta:type:Decimal", "SqlServer", "1.2345", new(Precision: 19, Scale: 4)));
            var decimalSql = syntax.ExportScalarExpression(decimalValue.Expression);
            using (var command = new SqlCommand($"SELECT {decimalSql}, SQL_VARIANT_PROPERTY({decimalSql}, 'Precision'), SQL_VARIANT_PROPERTY({decimalSql}, 'Scale')", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                Assert.True(await reader.ReadAsync());
                Assert.Equal(1.2345m, reader.GetDecimal(0));
                Assert.Equal(19, Convert.ToInt32(reader.GetValue(1)));
                Assert.Equal(4, Convert.ToInt32(reader.GetValue(2)));
            }
            var invalidValue = service.GenerateSqlExpression(loaded, MetaDataTypeInstance.BuiltIn,
                new("meta:type:Decimal", "SqlServer", "'not numeric'", new(Precision: 19, Scale: 4)));
            using (var command = new SqlCommand("SELECT " + syntax.ExportScalarExpression(invalidValue.Expression), connection))
                await Assert.ThrowsAsync<SqlException>(async () => await command.ExecuteScalarAsync());
        }
        finally
        {
            Environment.SetEnvironmentVariable(environmentName, null);
            SqlConnection.ClearAllPools();
            if (created)
            {
                using var drop = new SqlCommand($"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}]", master);
                await drop.ExecuteNonQueryAsync();
            }
            if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
        }
    }
}
