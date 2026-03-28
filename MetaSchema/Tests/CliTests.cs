using System.Diagnostics;
using MetaSchema.Core;

using Microsoft.Data.SqlClient;
using MetaSchema.Extractors.SqlServer;

namespace MetaSchema.Tests;

public sealed class CliTests
{
    [Fact]
    public void Help_ShowsExtractCommand()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-schema <command> [options]", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("extract", result.Output);
    }

    [Fact]
    public void ExtractSqlServer_Help_ShowsRequiredOptions()
    {
        var result = RunCli("extract sqlserver --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--new-workspace <path>", result.Output);
        Assert.Contains("--connection <connectionString>", result.Output);
        Assert.Contains("--system", result.Output);
        Assert.Contains("<name>", result.Output);
        Assert.Contains("--schema <name>", result.Output);
        Assert.Contains("--all-schemas", result.Output);
        Assert.Contains("--table <name>", result.Output);
        Assert.Contains("--all-tables", result.Output);
    }

    [Fact]
    public void ExtractSqlServer_FailsWhenExtractorUnknown()
    {
        var result = RunCli("extract nope");

        Assert.Equal(1, result.ExitCode);
        Assert.Contains("Error: unknown extractor 'nope'.", result.Output);
    }

    [Fact]
    public void ExtractSqlServer_FailsWhenConnectionMissing_AndDoesNotCreateTargetDirectory()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var result = RunCli($"extract sqlserver --new-workspace \"{workspacePath}\" --system TestSystem --schema dbo --table Cube");

            Assert.Equal(1, result.ExitCode);
            Assert.Contains("Error: missing required option --connection <connectionString>.", result.Output);
            Assert.False(Directory.Exists(workspacePath));
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    [Fact]
    public void ExtractSqlServer_FailsWhenSystemMissing_AndDoesNotCreateTargetDirectory()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var result = RunCli($"extract sqlserver --new-workspace \"{workspacePath}\" --connection \"Server=.;Database=master;Trusted_Connection=True;Encrypt=False\" --schema dbo --table Cube");

            Assert.Equal(1, result.ExitCode);
            Assert.Contains("Error: missing required option --system <name>.", result.Output);
            Assert.False(Directory.Exists(workspacePath));
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    [Fact]
    public void ExtractSqlServer_FailsWhenSchemaMissing_AndDoesNotCreateTargetDirectory()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var result = RunCli($"extract sqlserver --new-workspace \"{workspacePath}\" --connection \"Server=.;Database=master;Trusted_Connection=True;Encrypt=False\" --system TestSystem --table Cube");

            Assert.Equal(1, result.ExitCode);
            Assert.Contains("Error: missing required scope option --schema <name> or --all-schemas.", result.Output);
            Assert.False(Directory.Exists(workspacePath));
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    [Fact]
    public void ExtractSqlServer_FailsWhenTableMissing_AndDoesNotCreateTargetDirectory()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var result = RunCli($"extract sqlserver --new-workspace \"{workspacePath}\" --connection \"Server=.;Database=master;Trusted_Connection=True;Encrypt=False\" --system TestSystem --schema dbo");

            Assert.Equal(1, result.ExitCode);
            Assert.Contains("Error: missing required scope option --table <name> or --all-tables.", result.Output);
            Assert.False(Directory.Exists(workspacePath));
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    [Fact]
    public void ExtractSqlServer_FailsWhenSchemaAndAllSchemasProvided()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var result = RunCli($"extract sqlserver --new-workspace \"{workspacePath}\" --connection \"Server=.;Database=master;Trusted_Connection=True;Encrypt=False\" --system TestSystem --schema dbo --all-schemas --table Cube");

            Assert.Equal(1, result.ExitCode);
            Assert.Contains("Error: --schema and --all-schemas cannot be used together.", result.Output);
            Assert.False(Directory.Exists(workspacePath));
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    [Fact]
    public void ExtractSqlServer_FailsWhenTableAndAllTablesProvided()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var result = RunCli($"extract sqlserver --new-workspace \"{workspacePath}\" --connection \"Server=.;Database=master;Trusted_Connection=True;Encrypt=False\" --system TestSystem --schema dbo --table Cube --all-tables");

            Assert.Equal(1, result.ExitCode);
            Assert.Contains("Error: --table and --all-tables cannot be used together.", result.Output);
            Assert.False(Directory.Exists(workspacePath));
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    [Fact]
    public void MetaSchemaModel_UsesScalarMetaDataTypeId_AndIncludesStrongTableRelationships()
    {
        var model = MetaSchemaModels.CreateMetaSchemaModel();

        var field = Assert.Single(model.Entities, entity => entity.Name == "Field");
        var fieldDataTypeDetail = Assert.Single(model.Entities, entity => entity.Name == "FieldDataTypeDetail");
        var tableKey = Assert.Single(model.Entities, entity => entity.Name == "TableKey");
        var tableKeyField = Assert.Single(model.Entities, entity => entity.Name == "TableKeyField");
        var tableRelationship = Assert.Single(model.Entities, entity => entity.Name == "TableRelationship");
        var tableRelationshipField = Assert.Single(model.Entities, entity => entity.Name == "TableRelationshipField");
        Assert.DoesNotContain(model.Entities, entity => entity.Name == "FieldType");
        Assert.Contains(field.Properties, property => property.Name == "MetaDataTypeId");
        Assert.DoesNotContain(field.Properties, property => property.Name == "Length");
        Assert.DoesNotContain(field.Properties, property => property.Name == "NumericPrecision");
        Assert.DoesNotContain(field.Properties, property => property.Name == "Scale");
        Assert.DoesNotContain(field.Relationships, relationship => relationship.Entity == "FieldType");
        Assert.Contains(fieldDataTypeDetail.Relationships, relationship => relationship.Entity == "Field");
        Assert.Contains(tableKey.Properties, property => property.Name == "KeyType");
        Assert.Contains(tableKey.Relationships, relationship => relationship.Entity == "Table");
        Assert.Contains(tableKeyField.Properties, property => property.Name == "FieldName");
        Assert.Contains(tableKeyField.Relationships, relationship => relationship.Entity == "TableKey");
        Assert.Contains(tableKeyField.Relationships, relationship => relationship.Entity == "Field");
        Assert.Contains(tableRelationship.Relationships, relationship => relationship.Entity == "Table" && string.Equals(relationship.Role, "SourceTable", StringComparison.Ordinal));
        Assert.Contains(tableRelationship.Relationships, relationship => relationship.Entity == "Table" && string.Equals(relationship.Role, "TargetTable", StringComparison.Ordinal));
        Assert.Contains(tableRelationshipField.Relationships, relationship => relationship.Entity == "TableRelationship");
        Assert.Contains(tableRelationshipField.Relationships, relationship => relationship.Entity == "Field" && string.Equals(relationship.Role, "SourceField", StringComparison.Ordinal));
        Assert.Contains(tableRelationshipField.Relationships, relationship => relationship.Entity == "Field" && string.Equals(relationship.Role, "TargetField", StringComparison.Ordinal));
        Assert.DoesNotContain(tableRelationship.Properties, property => property.Name == "TargetSchemaName");
        Assert.DoesNotContain(tableRelationship.Properties, property => property.Name == "TargetTableName");
        Assert.DoesNotContain(tableRelationshipField.Properties, property => property.Name == "SourceFieldName");
        Assert.DoesNotContain(tableRelationshipField.Properties, property => property.Name == "TargetFieldName");
    }

    [Fact]
    public void SqlServerExtractor_ExtractsMetaSqlCompatibleFieldTypeDetails()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(tempRoot, "MetaSchemaWorkspace");
        var databaseName = $"MetaSchemaTypeDetails_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                CREATE TABLE dbo.TypeDetailCase
                (
                    Id int NOT NULL,
                    LoadTimestamp datetime2(7) NOT NULL,
                    Amount decimal(18,2) NOT NULL,
                    AuditId int NOT NULL,
                    CONSTRAINT PK_TypeDetailCase PRIMARY KEY (Id)
                );
                """);

            var extractor = new SqlServerSchemaExtractor();
            var workspace = extractor.ExtractMetaSchemaWorkspace(new SqlServerExtractRequest
            {
                NewWorkspacePath = workspacePath,
                ConnectionString = databaseConnectionString,
                SystemName = databaseName,
                SchemaName = "dbo",
                TableName = "TypeDetailCase",
            });

            var fieldsByName = workspace.Instance
                .GetOrCreateEntityRecords("Field")
                .ToDictionary(row => row.Values["Name"], StringComparer.Ordinal);
            var detailNamesByFieldId = workspace.Instance
                .GetOrCreateEntityRecords("FieldDataTypeDetail")
                .GroupBy(row => row.RelationshipIds["FieldId"], StringComparer.Ordinal)
                .ToDictionary(
                    group => group.Key,
                    group => group.ToDictionary(row => row.Values["Name"], row => row.Values["Value"], StringComparer.Ordinal),
                    StringComparer.Ordinal);

            var loadTimestampDetails = detailNamesByFieldId[fieldsByName["LoadTimestamp"].Id];
            Assert.Equal("7", loadTimestampDetails["Precision"]);

            var amountDetails = detailNamesByFieldId[fieldsByName["Amount"].Id];
            Assert.Equal("18", amountDetails["Precision"]);
            Assert.Equal("2", amountDetails["Scale"]);

            Assert.False(detailNamesByFieldId.ContainsKey(fieldsByName["AuditId"].Id));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    private static (int ExitCode, string Output) RunCli(string arguments)
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot);
        var startInfo = new ProcessStartInfo
        {
            FileName = cliPath,
            Arguments = arguments,
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        return RunProcess(startInfo, "Could not start meta-schema CLI process.");
    }

    private static (int ExitCode, string Output) RunProcess(ProcessStartInfo startInfo, string errorMessage)
    {
        using var process = Process.Start(startInfo)
                            ?? throw new InvalidOperationException(errorMessage);
        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            try
            {
                process.WaitForExitAsync(timeout.Token).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException exception)
            {
                TryKillProcessTree(process);
                process.WaitForExit();
                throw new TimeoutException($"Timed out waiting for process: {startInfo.FileName} {startInfo.Arguments}", exception);
            }

            var stdout = stdoutTask.GetAwaiter().GetResult();
            var stderr = stderrTask.GetAwaiter().GetResult();
            return (process.ExitCode, stdout + stderr);
        }
        finally
        {
            if (!process.HasExited)
            {
                TryKillProcessTree(process);
                process.WaitForExit();
            }
        }
    }

    private static string ResolveCliPath(string repoRoot)
    {
        var cliPath = Path.Combine(repoRoot, Path.Combine("MetaSchema", "Cli"), "bin", "Debug", "net8.0", "meta-schema.exe");
        if (!File.Exists(cliPath))
        {
            throw new FileNotFoundException($"Could not find compiled MetaSchema CLI at '{cliPath}'. Build MetaSchema.Cli before running tests.");
        }

        return cliPath;
    }

    private static void TryKillProcessTree(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory = AppContext.BaseDirectory;
        while (!string.IsNullOrWhiteSpace(directory))
        {
            if (File.Exists(Path.Combine(directory, "README.md")) && Directory.Exists(Path.Combine(directory, Path.Combine("MetaSchema", "Cli"))))
            {
                return directory;
            }

            var parent = Directory.GetParent(directory);
            if (parent == null)
            {
                break;
            }

            directory = parent.FullName;
        }

        throw new InvalidOperationException("Could not locate meta-bi repository root from test base directory.");
    }

    private static void CreateDatabase(string masterConnectionString, string databaseName)
    {
        using var connection = new SqlConnection(masterConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"IF DB_ID(N'{databaseName.Replace("'", "''", StringComparison.Ordinal)}') IS NULL CREATE DATABASE [{databaseName}]";
        command.ExecuteNonQuery();
    }

    private static void DropDatabase(string masterConnectionString, string databaseName)
    {
        using var connection = new SqlConnection(masterConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"""
            IF DB_ID(N'{databaseName.Replace("'", "''", StringComparison.Ordinal)}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{databaseName}];
            END
            """;
        command.ExecuteNonQuery();
    }

    private static void ExecuteSql(string connectionString, string sql)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}


