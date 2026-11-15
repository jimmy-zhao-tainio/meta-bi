using System.Diagnostics;
using Meta.Integration;
using Meta.Operations;
using Meta.Operations.Domain;
using Meta.Surfaces;
using MetaBi.Tests.Common;
using MetaDataType;
using MetaDataType.Core;
using MetaSql.Extractors.SqlServer;
using Microsoft.Data.SqlClient;
using Xunit.Abstractions;

namespace MetaSql.Tests;

public sealed class SelectedCatalogSqlServerIntegrationTests(ITestOutputHelper output)
{
    private const string Master = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
    private const string IntegerId = "chosen-number-42";
    private const string TextId = "chosen-text-17";

    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task SelectedCatalogFlowsThroughSchemaExtractionPlanAndLiveDeploy(string representation)
    {
        var root = NewRoot();
        var database = "MetaSqlCatalog_" + Guid.NewGuid().ToString("N");
        var connection = new SqlConnectionStringBuilder(Master) { InitialCatalog = database }.ConnectionString;
        try
        {
            Execute(Master, $"CREATE DATABASE [{database}];");
            Execute(connection, """
                CREATE TABLE dbo.CopyValues (Id int NOT NULL, Value nvarchar(50) NULL);
                INSERT INTO dbo.CopyValues VALUES
                    (1, N'ordinary text'), (2, N'Åsa äter räksmörgås.'), (3, NULL), (4, N'');
                """);
            var catalogPath = await SaveCatalog(root, representation);
            var sourcePath = Path.Combine(root, "source");
            var schemaPath = Path.Combine(root, "schema");
            AssertOk(RunCli("meta-sql", $"extract sqlserver --connection-env CATALOG_TEST_SQL --schema dbo --table CopyValues --data-type-workspace \"{catalogPath}\" --output-{representation} \"{sourcePath}\"", connection));
            AssertOk(RunCli("meta-schema", $"extract sqlserver --connection-env CATALOG_TEST_SQL --system CatalogTest --schema dbo --table CopyValues --data-type-workspace \"{catalogPath}\" --output-{representation} \"{schemaPath}\"", connection));

            var selected = MetaDataTypeWorkspaceProvider.LoadOrDefault(catalogPath);
            var schema = await TypedWorkspaceModelMapper.LoadAsync(schemaPath, new MetaSchema.MetaSchemaWorkspaceBinding());
            new MetaSchema.SqlServerSchemaTypeValidator(selected).Validate(schema);
            Assert.Equal(IntegerId, schema.FieldList.Single(row => row.Name == "Id").MetaDataTypeId);
            var textField = schema.FieldList.Single(row => row.Name == "Value");
            Assert.Equal(TextId, textField.MetaDataTypeId);
            Assert.Equal("50", schema.FieldDataTypeDetailList.Single(row => row.Field == textField && row.Name == "Length").Value);

            var source = await TypedWorkspaceModelMapper.LoadAsync(sourcePath, new MetaSqlWorkspaceBinding());
            Assert.Equal(IntegerId, source.TableColumnList.Single(row => row.Name == "Id").MetaDataTypeId);
            var text = source.TableColumnList.Single(row => row.Name == "Value");
            Assert.Equal(TextId, text.MetaDataTypeId);
            var length = source.TableColumnDataTypeDetailList.Single(row => row.TableColumn == text && row.Name == "Length");
            Assert.Equal("50", length.Value);
            await using (var workspace = await WorkspaceSurface.OpenAsync(sourcePath))
                await workspace.ExecuteAsync([
                    new Operation.SetProperty("TableColumnDataTypeDetail", length.Id, "Value", "100"),
                    new Operation.InsertRecord("TableColumn", text.Table.Id + ".Extra",
                        new Dictionary<string, string> { ["Name"] = "Extra", ["Ordinal"] = "3", ["MetaDataTypeId"] = TextId, ["IsNullable"] = "true" },
                        new Dictionary<string, string> { ["TableId"] = text.Table.Id }),
                    new Operation.InsertRecord("TableColumnDataTypeDetail", text.Table.Id + ".Extra.Length",
                        new Dictionary<string, string> { ["Name"] = "Length", ["Value"] = "-1" },
                        new Dictionary<string, string> { ["TableColumnId"] = text.Table.Id + ".Extra" }),
                ]);

            var planPath = Path.Combine(root, "plan");
            AssertOk(RunCli("meta-sql", $"deploy-plan --connection-env CATALOG_TEST_SQL --source-workspace \"{sourcePath}\" --data-type-workspace \"{catalogPath}\" --output-xml \"{planPath}\"", connection));
            var manifest = await TypedWorkspaceModelMapper.LoadAsync(planPath, new MetaSqlDeployManifest.MetaSqlDeployManifestWorkspaceBinding());
            Assert.Single(manifest.AlterTableColumnList);
            Assert.Single(manifest.AddTableColumnList);
            AssertOk(RunCli("meta-sql", $"deploy --connection-env CATALOG_TEST_SQL --source-workspace \"{sourcePath}\" --manifest-workspace \"{planPath}\" --data-type-workspace \"{catalogPath}\"", connection));

            Assert.Equal(200, Scalar(connection, "SELECT max_length FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.CopyValues') AND name = N'Value';"));
            Assert.Equal(-1, Scalar(connection, "SELECT max_length FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.CopyValues') AND name = N'Extra';"));
            VerifyRows(connection);

            // A 25-character Unicode target permits these values. Feasibility must use
            // the selected nvarchar identity to compare 50 bytes, rather than 25 bytes.
            await using (var workspace = await WorkspaceSurface.OpenAsync(sourcePath))
                await workspace.ExecuteAsync([new Operation.SetProperty("TableColumnDataTypeDetail", length.Id, "Value", "25")]);
            var narrowPlan = Path.Combine(root, "narrow-plan");
            AssertOk(RunCli("meta-sql", $"deploy-plan --connection-env CATALOG_TEST_SQL --source-workspace \"{sourcePath}\" --data-type-workspace \"{catalogPath}\" --output-xml \"{narrowPlan}\"", connection));
            AssertOk(RunCli("meta-sql", $"deploy --connection-env CATALOG_TEST_SQL --source-workspace \"{sourcePath}\" --manifest-workspace \"{narrowPlan}\" --data-type-workspace \"{catalogPath}\"", connection));
            Assert.Equal(50, Scalar(connection, "SELECT max_length FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.CopyValues') AND name = N'Value';"));
            VerifyRows(connection);
            var reextracted = new SqlServerMetaSqlExtractor().ExtractMetaSqlWorkspace(new SqlServerExtractRequest
            {
                ConnectionString = connection, DataTypeWorkspacePath = catalogPath,
                SchemaName = "dbo", TableName = "CopyValues",
            });
            var persistedSource = await TypedWorkspaceModelMapper.LoadStateAsync(sourcePath);
            Assert.Empty(new MetaSqlDifferenceService().BuildDifferences(persistedSource, reextracted));
        }
        finally
        {
            Execute(Master, $"IF DB_ID(N'{database}') IS NOT NULL BEGIN ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{database}]; END;");
            Directory.Delete(root, recursive: true);
        }
    }

    [Theory]
    [InlineData("not-in-catalog", "resolves to 0")]
    [InlineData("sqlserver:type:int", "resolves to 0")]
    [InlineData("foreign-number", "does not resolve to a named SQL Server type")]
    public async Task PersistedMatchingInvalidTypesAreRejectedBeforeAnyConnection(string id, string diagnostic)
    {
        var root = NewRoot();
        try
        {
            var catalogPath = await SaveCatalog(root, "xml");
            var catalog = MetaDataTypeWorkspaceProvider.LoadOrDefault(catalogPath);
            var source = CreateSource(id);
            var sourcePath = Path.Combine(root, "source");
            await WorkspaceSurface.CreateAsync(new MetaSqlWorkspaceBinding().Write(source, MetaSqlModels.LoadContract()), sourcePath, "xml");
            var persisted = await TypedWorkspaceModelMapper.LoadStateAsync(sourcePath);
            var error = Assert.Throws<InvalidOperationException>(() => new MetaSqlDeployManifestService(catalog).BuildManifest(
                persisted, persisted, MetaSqlLiveDatabasePresence.Present, [], "Invalid", null));
            Assert.Contains("Source workspace column 'CatalogDb.dbo.CopyValues.Id'", error.Message);
            Assert.Contains(diagnostic, error.Message);

            var outputPath = Path.Combine(root, "should-not-exist");
            var cli = RunCli("meta-sql", $"deploy-plan --connection-env CATALOG_TEST_SQL --source-workspace \"{sourcePath}\" --data-type-workspace \"{catalogPath}\" --output-xml \"{outputPath}\"", "invalid connection string that must never be opened");
            Assert.Equal(4, cli.ExitCode);
            Assert.Contains(diagnostic, cli.Output);
            Assert.False(Directory.Exists(outputPath));
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task ChangedUsedCatalogMeaningRejectsMissingDatabasePlanBeforeCreation(string representation)
    {
        var root = NewRoot();
        var database = "MetaSqlCatalogGuard_" + Guid.NewGuid().ToString("N");
        var connection = new SqlConnectionStringBuilder(Master) { InitialCatalog = database }.ConnectionString;
        try
        {
            var catalogPath = await SaveCatalog(root, representation);
            var sourcePath = Path.Combine(root, "source");
            await WorkspaceSurface.CreateAsync(new MetaSqlWorkspaceBinding().Write(
                CreateSource(IntegerId, database), MetaSqlModels.LoadContract()), sourcePath, representation);
            var source = await TypedWorkspaceModelMapper.LoadStateAsync(sourcePath);
            var live = SqlServerMetaSqlWorkspaceFactory.CreateEmptyWorkspace(database);
            var catalog = MetaDataTypeWorkspaceProvider.LoadOrDefault(catalogPath);
            var plan = new MetaSqlDeployManifestService(catalog).BuildManifest(source, live,
                MetaSqlLiveDatabasePresence.Missing,
                new MetaSqlDifferenceService().BuildDifferences(source, live), "Guarded", null);
            Assert.True(plan.IsDeployable);
            Assert.StartsWith("sqlserver-v1:", Assert.Single(plan.ManifestModel.DeployManifestList).SourceInstanceFingerprint);
            var planPath = Path.Combine(root, "plan");
            await WorkspaceSurface.CreateAsync(new MetaSqlDeployManifest.MetaSqlDeployManifestWorkspaceBinding().Write(
                plan.ManifestModel, MetaSqlDeployManifest.MetaSqlDeployManifestModels.LoadContract()), planPath, representation);

            await using (var workspace = await WorkspaceSurface.OpenAsync(catalogPath))
                await workspace.ExecuteAsync([new Operation.SetProperty("DataType", IntegerId, "Name", "bigint")]);
            var request = new MetaSqlDeployRequest
            {
                ManifestWorkspacePath = planPath, SourceWorkspacePath = sourcePath,
                ConnectionString = connection, DataTypeWorkspacePath = catalogPath,
            };
            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => new MetaSqlDeployService().DeployAsync(request));
            Assert.Contains("Manifest source fingerprint mismatch.", error.Message);
            Assert.Contains("selected SQL Server type contract changed", error.Message);
            Assert.Equal(0, Scalar(Master, $"SELECT COUNT(*) FROM sys.databases WHERE name = N'{database}';"));

            // Restore the used meaning while changing an unused type: only the
            // interpretation on which this plan depends must remain unchanged.
            await using (var workspace = await WorkspaceSurface.OpenAsync(catalogPath))
                await workspace.ExecuteAsync([
                    new Operation.SetProperty("DataType", IntegerId, "Name", "int"),
                    new Operation.SetProperty("DataType", TextId, "Name", "varchar"),
                ]);
            var result = await new MetaSqlDeployService().DeployAsync(request);
            Assert.True(result.DatabaseCreated);
            Assert.Equal(1, Scalar(connection, """
                SELECT COUNT(*) FROM sys.columns c JOIN sys.types t ON t.user_type_id = c.user_type_id
                WHERE c.object_id = OBJECT_ID(N'dbo.CopyValues') AND c.name = N'Id' AND t.name = N'int';
                """));
        }
        finally
        {
            Execute(Master, $"IF DB_ID(N'{database}') IS NOT NULL BEGIN ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{database}]; END;");
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task AmbiguousSelectedSqlTypeNameFailsWithSelectedCatalogEvidence()
    {
        var root = NewRoot();
        try
        {
            var path = await SaveCatalog(root, "xml");
            await using (var workspace = await WorkspaceSurface.OpenAsync(path))
                await workspace.ExecuteAsync([new Operation.InsertRecord("DataType", "another-integer",
                    new Dictionary<string, string> { ["Name"] = "int" },
                    new Dictionary<string, string> { ["DataTypeSystemId"] = "chosen-system" })]);
            var catalog = MetaDataTypeWorkspaceProvider.LoadOrDefault(path);
            var sql = Assert.Throws<InvalidOperationException>(() => new SqlServerDataTypeCatalog(catalog).ResolveName("int", "SQL Server column 'dbo.CopyValues.Id'"));
            Assert.Contains("dbo.CopyValues.Id", sql.Message);
            Assert.Contains("resolves to 2", sql.Message);
            var schema = Assert.Throws<MetaSchema.SchemaTypeValidationException>(() => new MetaSchema.SqlServerSchemaTypeValidator(catalog).ResolveSqlTypeName("int"));
            Assert.Contains("resolves to 2", schema.Message);
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    private static async Task<string> SaveCatalog(string root, string representation)
    {
        var defaults = await MetaDataTypeWorkspaceProvider.LoadDefaultStateAsync();
        var catalog = new MetaDataTypeModel();
        var sql = new DataTypeSystem { Id = "chosen-system", Name = "SqlServer" };
        var foreign = new DataTypeSystem { Id = "SqlServer", Name = "Other" };
        catalog.DataTypeSystemList.AddRange([sql, foreign]);
        catalog.DataTypeList.AddRange([
            new DataType { Id = IntegerId, Name = "int", DataTypeSystem = sql },
            new DataType { Id = TextId, Name = "nvarchar", DataTypeSystem = sql },
            new DataType { Id = "foreign-number", Name = "int", DataTypeSystem = foreign },
        ]);
        var path = Path.Combine(root, "catalog");
        await WorkspaceSurface.CreateAsync(new MetaDataTypeWorkspaceBinding().Write(catalog, defaults.Model), path, representation);
        return path;
    }

    private static MetaSqlModel CreateSource(string typeId, string databaseName = "CatalogDb")
    {
        var model = MetaSqlModel.CreateEmpty();
        var database = new Database { Id = databaseName, Name = databaseName };
        var schema = new Schema { Id = databaseName + ".dbo", Name = "dbo", Database = database };
        var table = new Table { Id = schema.Id + ".CopyValues", Name = "CopyValues", Schema = schema };
        model.DatabaseList.Add(database);
        model.SchemaList.Add(schema);
        model.TableList.Add(table);
        model.TableColumnList.Add(new TableColumn { Id = table.Id + ".Id", Name = "Id", Ordinal = "1", MetaDataTypeId = typeId, IsNullable = "false", Table = table });
        return model;
    }

    private void VerifyRows(string connectionString)
    {
        string?[] expected = ["ordinary text", "Åsa äter räksmörgås.", null, ""];
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Value, Extra FROM dbo.CopyValues ORDER BY Id;";
        using var rows = command.ExecuteReader();
        for (var index = 0; index < expected.Length; index++)
        {
            Assert.True(rows.Read());
            Assert.Equal(index + 1, rows.GetInt32(0));
            var actual = rows.IsDBNull(1) ? null : rows.GetString(1);
            output.WriteLine($"Row {index + 1}: input={Format(expected[index])}; expected={Format(expected[index])}; actual={Format(actual)}");
            Assert.Equal(expected[index], actual);
            Assert.True(rows.IsDBNull(2));
        }
        Assert.False(rows.Read());
    }

    private static string Format(string? value) => value is null ? "SQL NULL" : "\"" + value + "\"";

    private static (int ExitCode, string Output) RunCli(string name, string arguments, string connection)
    {
        var start = CliTestRunner.CreateManagedCliStartInfo(name, arguments);
        start.Environment["CATALOG_TEST_SQL"] = connection;
        using var process = Process.Start(start) ?? throw new InvalidOperationException("Cannot start " + name);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(2));
        try { process.WaitForExitAsync(timeout.Token).GetAwaiter().GetResult(); }
        catch (OperationCanceledException) { process.Kill(entireProcessTree: true); throw; }
        return (process.ExitCode, stdout.GetAwaiter().GetResult() + stderr.GetAwaiter().GetResult());
    }

    private static void AssertOk((int ExitCode, string Output) result) => Assert.True(result.ExitCode == 0, result.Output);

    private static int Scalar(string connectionString, string sql)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        return Convert.ToInt32(command.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }

    private static void Execute(string connectionString, string sql)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static string NewRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "sql-selected-catalog", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
