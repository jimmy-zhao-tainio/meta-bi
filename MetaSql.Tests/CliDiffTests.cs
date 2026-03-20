using System.Diagnostics;
using Microsoft.Data.SqlClient;
using MetaSqlDeployManifest;
using MetaSql.Extractors.SqlServer;

namespace MetaSql.Tests;

public sealed class CliDiffTests
{
    [Fact]
    public void DeployTestHelp_RendersExpectedUsage()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaSql.Cli", "meta-sql.dll");

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{cliPath}\" deploy-test --help",
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Command: deploy-test", result.Output, StringComparison.Ordinal);
        Assert.Contains("meta-sql deploy-test --source-workspace <path> --connection-string <value> --out <path>", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeployTestCommand_WritesDeployableManifestForAddOnlyChanges()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaSql.Cli", "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployTestSmoke_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString);
            await CreateSourceWorkspaceWithExtraColumnAsync(sourcePath, databaseName);

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-test --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("deploy-test complete", result.Output, StringComparison.Ordinal);
            Assert.Contains("Verdict: deployable", result.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Single(manifest.AddTableColumnList);
            Assert.Empty(manifest.BlockTableColumnDifferenceList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployTestCommand_WritesBlockingManifestForSharedObjectDifference()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaSql.Cli", "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployTestBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString);
            await CreateSourceWorkspaceWithChangedCustomerIdLengthAsync(sourcePath, databaseName);

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-test --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("deploy-test produced a non-deployable manifest.", result.Output, StringComparison.Ordinal);
            Assert.Contains("BlockTableColumnDifference", result.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Single(manifest.BlockTableColumnDifferenceList);
            Assert.Empty(manifest.AddTableColumnList);
            Assert.Empty(manifest.DropTableColumnList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    private static Task CreateSourceWorkspaceWithChangedCustomerIdLengthAsync(string sourcePath, string databaseName)
    {
        SqlServerMetaSqlProjector.Project(
            newWorkspacePath: sourcePath,
            databaseName: databaseName,
            tableRows:
            [
                new SqlServerMetaSqlProjector.TableRow("raw", "H_Customer")
            ],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.H_Customer"] =
                [
                    new("raw", "H_Customer", "HashKey", 1, false, "binary", 16, null, null),
                    new("raw", "H_Customer", "CustomerId", 2, false, "nvarchar", 20, null, null),
                    new("raw", "H_Customer", "LoadTimestamp", 3, false, "datetime2", null, 7, null),
                    new("raw", "H_Customer", "RecordSource", 4, false, "nvarchar", 256, null, null),
                    new("raw", "H_Customer", "AuditId", 5, false, "int", null, null, null),
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.H_Customer"] = [new("PK_H_Customer", true)],
            },
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.H_Customer"] = [new("PK_H_Customer", 1, "HashKey", false)],
            },
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
            indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));
        return Task.CompletedTask;
    }

    private static Task CreateSourceWorkspaceWithExtraColumnAsync(string sourcePath, string databaseName)
    {
        SqlServerMetaSqlProjector.Project(
            newWorkspacePath: sourcePath,
            databaseName: databaseName,
            tableRows:
            [
                new SqlServerMetaSqlProjector.TableRow("raw", "H_Customer")
            ],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.H_Customer"] =
                [
                    new("raw", "H_Customer", "HashKey", 1, false, "binary", 16, null, null),
                    new("raw", "H_Customer", "CustomerId", 2, false, "nvarchar", 50, null, null),
                    new("raw", "H_Customer", "CustomerName", 3, true, "nvarchar", 200, null, null),
                    new("raw", "H_Customer", "LoadTimestamp", 4, false, "datetime2", null, 7, null),
                    new("raw", "H_Customer", "RecordSource", 5, false, "nvarchar", 256, null, null),
                    new("raw", "H_Customer", "AuditId", 6, false, "int", null, null, null),
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.H_Customer"] = [new("PK_H_Customer", true)],
            },
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.H_Customer"] = [new("PK_H_Customer", 1, "HashKey", false)],
            },
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
            indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));
        return Task.CompletedTask;
    }

    private static void CreateDatabase(string masterConnectionString, string databaseName)
    {
        ExecuteSql(masterConnectionString, $"""
            IF DB_ID('{databaseName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{databaseName}];
            END;
            CREATE DATABASE [{databaseName}];
            """);
    }

    private static void CreateSimpleTable(string databaseConnectionString)
    {
        ExecuteSql(databaseConnectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.H_Customer (
                HashKey binary(16) NOT NULL,
                CustomerId nvarchar(50) NOT NULL,
                LoadTimestamp datetime2(7) NOT NULL,
                RecordSource nvarchar(256) NOT NULL,
                AuditId int NOT NULL,
                CONSTRAINT PK_H_Customer PRIMARY KEY (HashKey)
            );
            """);
    }

    private static void DropDatabase(string masterConnectionString, string databaseName)
    {
        try
        {
            ExecuteSql(masterConnectionString, $"""
                IF DB_ID('{databaseName}') IS NOT NULL
                BEGIN
                    ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{databaseName}];
                END;
                """);
        }
        catch
        {
        }
    }

    private static void ExecuteSql(string connectionString, string sql)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "MetaDataVault.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not find the repository root.");
    }

    private static string ResolveCliPath(string repoRoot, string projectDirectory, string assemblyName)
    {
        var cliPath = Path.Combine(repoRoot, projectDirectory, "bin", "Debug", "net8.0", assemblyName);
        if (!File.Exists(cliPath))
        {
            throw new FileNotFoundException($"Could not find CLI assembly '{assemblyName}'. Expected at '{cliPath}'.");
        }

        return cliPath;
    }

    private static (int ExitCode, string Output) RunProcess(ProcessStartInfo startInfo, string errorMessage)
    {
        using var process = new Process { StartInfo = startInfo };
        if (!process.Start())
        {
            throw new InvalidOperationException(errorMessage);
        }

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return (process.ExitCode, output + error);
    }

    private static void DeleteIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
