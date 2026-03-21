using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using MetaSqlDeployManifest;
using MetaSql.Extractors.SqlServer;

namespace MetaSql.Tests;

public sealed class CliDiffTests
{
    [Fact]
    public void DeployPlanHelp_RendersExpectedUsage()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaSql.Cli", "meta-sql.dll");

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{cliPath}\" deploy-plan --help",
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Command: deploy-plan", result.Output, StringComparison.Ordinal);
        Assert.Contains("meta-sql deploy-plan --source-workspace <path> --connection-string <value> --out <path>", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void DeployHelp_RendersExpectedUsage()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaSql.Cli", "meta-sql.dll");

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{cliPath}\" deploy --help",
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Command: deploy", result.Output, StringComparison.Ordinal);
        Assert.Contains("meta-sql deploy --manifest-workspace <path> --source-workspace <path> --connection-string <value>", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeployPlanCommand_WritesDeployableManifestForAddOnlyChanges()
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
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("deploy-plan complete", result.Output, StringComparison.Ordinal);
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
    public async Task DeployPlanCommand_WritesBlockingManifestForSharedObjectDifference()
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
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("deploy-plan produced a non-deployable manifest.", result.Output, StringComparison.Ordinal);
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

    [Fact]
    public async Task DeployCommand_AppliesAddOnlyManifest()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaSql.Cli", "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var verifyPath = Path.Combine(tempRoot, "post-deploy-manifest");
        var databaseName = $"MetaSqlDeployApply_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString);
            await CreateSourceWorkspaceWithExtraColumnAsync(sourcePath, databaseName);

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            Assert.Contains("Verdict: deployable", planResult.Output, StringComparison.Ordinal);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(0, deployResult.ExitCode);
            Assert.Contains("deploy complete", deployResult.Output, StringComparison.Ordinal);

            Assert.True(ColumnExists(databaseConnectionString, "raw", "H_Customer", "CustomerName"));

            var verifyCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --out \"{verifyPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var verifyResult = RunProcess(verifyCommand, "Could not start MetaSql CLI deploy-plan verification process.");
            Assert.Equal(0, verifyResult.ExitCode);
            Assert.Contains("AddCount: 0", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("DropCount: 0", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("BlockCount: 0", verifyResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_RefusesWhenManifestContainsBlocks()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaSql.Cli", "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployBlockRefusal_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString);
            await CreateSourceWorkspaceWithChangedCustomerIdLengthAsync(sourcePath, databaseName);

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(4, planResult.ExitCode);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(5, deployResult.ExitCode);
            Assert.Contains("Manifest 'DeployManifest' is non-deployable. BlockCount=1.", deployResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_RefusesWhenSourceFingerprintMismatches()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaSql.Cli", "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePlannedPath = Path.Combine(tempRoot, "source-planned");
        var sourceDifferentPath = Path.Combine(tempRoot, "source-different");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeploySourceStale_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString);
            await CreateSourceWorkspaceWithExtraColumnAsync(sourcePlannedPath, databaseName);
            await CreateSourceWorkspaceWithChangedCustomerIdLengthAsync(sourceDifferentPath, databaseName);

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePlannedPath}\" --connection-string \"{databaseConnectionString}\" --schema raw --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourceDifferentPath}\" --connection-string \"{databaseConnectionString}\" --schema raw",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(5, deployResult.ExitCode);
            Assert.Contains("Manifest source fingerprint mismatch.", deployResult.Output, StringComparison.Ordinal);
            Assert.False(ColumnExists(databaseConnectionString, "raw", "H_Customer", "CustomerName"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_RefusesWhenLiveFingerprintMismatches()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaSql.Cli", "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployLiveStale_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString);
            await CreateSourceWorkspaceWithExtraColumnAsync(sourcePath, databaseName);

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);

            ExecuteSql(databaseConnectionString, "ALTER TABLE raw.H_Customer ADD LiveOnlyAfterPlan int NULL;");

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(5, deployResult.ExitCode);
            Assert.Contains("Manifest live fingerprint mismatch.", deployResult.Output, StringComparison.Ordinal);
            Assert.False(ColumnExists(databaseConnectionString, "raw", "H_Customer", "CustomerName"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_RefusesUnsupportedActionKindBeforeExecution()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaSql.Cli", "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployUnsupportedAction_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString);
            await CreateSourceWorkspaceWithExtraColumnAsync(sourcePath, databaseName);

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);

            AddUnsupportedActionKindToManifestModel(planPath, "AddUnsupportedThing");

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(5, deployResult.ExitCode);
            Assert.Contains("Manifest contains unsupported action kind(s): AddUnsupportedThing.", deployResult.Output, StringComparison.Ordinal);

            // Prove nothing was applied after refusal.
            Assert.False(ColumnExists(databaseConnectionString, "raw", "H_Customer", "CustomerName"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_AppliesDependencyOrderedDropPath()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaSql.Cli", "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployDropOrdering_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateParentChildWithForeignKey(databaseConnectionString);
            await CreateSourceWorkspaceWithChildOnlyNoForeignKeyAsync(sourcePath, databaseName);

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            Assert.Contains("DropCount: 2", planResult.Output, StringComparison.Ordinal);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(0, deployResult.ExitCode);

            Assert.False(TableExists(databaseConnectionString, "raw", "Parent"));
            Assert.True(TableExists(databaseConnectionString, "raw", "Child"));
            Assert.False(ForeignKeyExists(databaseConnectionString, "FK_Child_Parent"));
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

    private static Task CreateSourceWorkspaceWithChildOnlyNoForeignKeyAsync(string sourcePath, string databaseName)
    {
        SqlServerMetaSqlProjector.Project(
            newWorkspacePath: sourcePath,
            databaseName: databaseName,
            tableRows:
            [
                new SqlServerMetaSqlProjector.TableRow("raw", "Child")
            ],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.Child"] =
                [
                    new("raw", "Child", "ChildId", 1, false, "int", null, null, null),
                    new("raw", "Child", "ParentId", 2, false, "int", null, null, null),
                    new("raw", "Child", "LoadTimestamp", 3, false, "datetime2", null, 7, null),
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.Child"] = [new("PK_Child", true)],
            },
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.Child"] = [new("PK_Child", 1, "ChildId", false)],
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

    private static void CreateParentChildWithForeignKey(string databaseConnectionString)
    {
        ExecuteSql(databaseConnectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.Parent (
                ParentId int NOT NULL,
                CONSTRAINT PK_Parent PRIMARY KEY (ParentId)
            );
            CREATE TABLE raw.Child (
                ChildId int NOT NULL,
                ParentId int NOT NULL,
                LoadTimestamp datetime2(7) NOT NULL,
                CONSTRAINT PK_Child PRIMARY KEY (ChildId)
            );
            ALTER TABLE raw.Child
                ADD CONSTRAINT FK_Child_Parent
                FOREIGN KEY (ParentId) REFERENCES raw.Parent(ParentId);
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

    private static bool ColumnExists(string connectionString, string schemaName, string tableName, string columnName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*)
            FROM sys.columns AS c
            INNER JOIN sys.tables AS t ON t.object_id = c.object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            WHERE s.name = @SchemaName
              AND t.name = @TableName
              AND c.name = @ColumnName;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        command.Parameters.AddWithValue("@ColumnName", columnName);
        var value = command.ExecuteScalar();
        return Convert.ToInt32(value) > 0;
    }

    private static bool TableExists(string connectionString, string schemaName, string tableName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*)
            FROM sys.tables AS t
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            WHERE s.name = @SchemaName
              AND t.name = @TableName;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        var value = command.ExecuteScalar();
        return Convert.ToInt32(value) > 0;
    }

    private static bool ForeignKeyExists(string connectionString, string foreignKeyName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*)
            FROM sys.foreign_keys
            WHERE name = @ForeignKeyName;
            """;
        command.Parameters.AddWithValue("@ForeignKeyName", foreignKeyName);
        var value = command.ExecuteScalar();
        return Convert.ToInt32(value) > 0;
    }

    private static void AddUnsupportedActionKindToManifestModel(string manifestWorkspacePath, string unsupportedEntityName)
    {
        var modelPath = Path.Combine(manifestWorkspacePath, "metadata", "model.xml");
        var document = XDocument.Load(modelPath);
        var model = document.Root ?? throw new InvalidOperationException("Manifest model.xml root is missing.");
        var entityList = model.Element("EntityList") ?? throw new InvalidOperationException("Manifest model.xml EntityList is missing.");
        entityList.Add(
            new XElement("Entity",
                new XAttribute("name", unsupportedEntityName),
                new XElement("PropertyList",
                    new XElement("Property", new XAttribute("name", "UnsupportedId"))),
                new XElement("RelationshipList",
                    new XElement("Relationship", new XAttribute("entity", "DeployManifest")))));
        document.Save(modelPath);
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
