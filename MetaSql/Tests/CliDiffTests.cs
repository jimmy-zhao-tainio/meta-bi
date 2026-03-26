using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using MetaSql;
using MetaSqlDeployManifest;
using MetaSql.Extractors.SqlServer;

namespace MetaSql.Tests;

public sealed partial class CliDiffTests
{
    [Fact]
    public void DeployPlanHelp_RendersExpectedUsage()
    {
        var repoRoot = FindRepositoryRoot();

        var startInfo = new ProcessStartInfo
        {
            FileName = "meta-sql",
            Arguments = $"deploy-plan --help",
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
        Assert.Contains("--approve-drop-table", result.Output, StringComparison.Ordinal);
        Assert.Contains("--approve-drop-column", result.Output, StringComparison.Ordinal);
        Assert.Contains("--approve-truncate-column", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("--schema", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("--table", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("[--with-data-drop]", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("[--with-data-truncate]", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("--with-drop", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("--allow-drop", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void DeployHelp_RendersExpectedUsage()
    {
        var repoRoot = FindRepositoryRoot();

        var startInfo = new ProcessStartInfo
        {
            FileName = "meta-sql",
            Arguments = $"deploy --help",
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
        Assert.DoesNotContain("--schema", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("--table", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeployPlanCommand_WhenDatabaseIsMissing_TreatsLiveAsEmpty()
    {
        var repoRoot = FindRepositoryRoot();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlMissingLivePlan_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            DropDatabase(masterConnectionString, databaseName);
            await CreateSourceWorkspaceWithExtraColumnAsync(sourcePath, databaseName);

            var startInfo = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("Verdict: deployable", result.Output, StringComparison.Ordinal);
            Assert.False(DatabaseExists(masterConnectionString, databaseName));

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            var root = Assert.Single(manifest.DeployManifestList);
            Assert.Equal("Missing", root.ExpectedLiveDatabasePresence);
            Assert.Single(manifest.AddSchemaList);
            Assert.NotEmpty(manifest.AddTableList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_WhenManifestExpectsMissingDatabase_CreatesDatabaseAndAppliesSchema()
    {
        var repoRoot = FindRepositoryRoot();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlMissingLiveDeploy_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            DropDatabase(masterConnectionString, databaseName);
            await CreateSourceWorkspaceWithExtraColumnAsync(sourcePath, databaseName);

            var planCommand = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            Assert.False(DatabaseExists(masterConnectionString, databaseName));

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.AddSchemaList);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");

            Assert.Equal(0, deployResult.ExitCode);
            Assert.True(DatabaseExists(masterConnectionString, databaseName));
            Assert.True(SchemaExists(databaseConnectionString, "raw"));
            Assert.True(TableExists(databaseConnectionString, "raw", "H_Customer"));
            Assert.True(ColumnExists(databaseConnectionString, "raw", "H_Customer", "CustomerName"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_WhenManifestExpectsMissingDatabase_RefusesIfDatabaseAlreadyExists()
    {
        var repoRoot = FindRepositoryRoot();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlMissingLiveRefusal_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            DropDatabase(masterConnectionString, databaseName);
            await CreateSourceWorkspaceWithExtraColumnAsync(sourcePath, databaseName);

            var planCommand = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);

            CreateDatabase(masterConnectionString, databaseName);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");

            Assert.NotEqual(0, deployResult.ExitCode);
            Assert.Contains("database already exists", deployResult.Output, StringComparison.OrdinalIgnoreCase);
            Assert.False(TableExists(databaseConnectionString, "raw", "H_Customer"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_WhenDatabaseExistsWithoutFilteredSchema_PlansAddSchema()
    {
        var repoRoot = FindRepositoryRoot();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlMissingSchemaPlan_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            DropDatabase(masterConnectionString, databaseName);
            CreateDatabase(masterConnectionString, databaseName);
            await CreateSourceWorkspaceWithExtraColumnAsync(sourcePath, databaseName);

            var startInfo = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(0, result.ExitCode);
            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            var root = Assert.Single(manifest.DeployManifestList);
            Assert.Equal("Present", root.ExpectedLiveDatabasePresence);
            Assert.Single(manifest.AddSchemaList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_WritesDeployableManifestForAddOnlyChanges()
    {
        var repoRoot = FindRepositoryRoot();
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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{outputPath}\"",
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
    public async Task DeployPlanCommand_WithoutDataDropApproval_BlocksLiveOnlyDataDrop()
    {
        var repoRoot = FindRepositoryRoot();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployNoDrop_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateParentChildWithForeignKey(databaseConnectionString);
            await CreateSourceWorkspaceWithChildOnlyNoForeignKeyAsync(sourcePath, databaseName);

            var startInfo = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("deploy-plan produced a non-deployable manifest.", result.Output, StringComparison.Ordinal);
            Assert.Contains("BlockTableDifference", result.Output, StringComparison.Ordinal);
            Assert.Contains("missing approval DataDropTable", result.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Empty(manifest.DropTableList);
            Assert.Single(manifest.DropForeignKeyList);
            Assert.Single(manifest.BlockTableDifferenceList);
            Assert.Empty(manifest.DropPrimaryKeyList);
            Assert.Empty(manifest.DropIndexList);
            Assert.Empty(manifest.DropTableColumnList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_RemovedGlobalFlagsAreRejected()
    {
        var repoRoot = FindRepositoryRoot();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployDeprecatedDropFlag_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateParentChildWithForeignKey(databaseConnectionString);
            await CreateSourceWorkspaceWithChildOnlyNoForeignKeyAsync(sourcePath, databaseName);

            var startInfo = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --with-data-drop --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(1, result.ExitCode);
            Assert.Contains("unknown option '--with-data-drop'.", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_StillEmitsAddAndAlterActions_WhenNoDestructiveApprovalRequired()
    {
        var repoRoot = FindRepositoryRoot();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployAddAlterNoDrop_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString, customerIdLength: 50);
            await CreateSourceWorkspaceWithExtraColumnAsync(sourcePath, databaseName);
            await MutateSourceWorkspaceAsync(
                sourcePath,
                model =>
                {
                    var customerIdColumn = RequireColumn(model, "raw", "H_Customer", "CustomerId");
                    SetOrReplaceColumnDetail(model, customerIdColumn, "Length", "100");
                });

            var startInfo = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("AddCount: 1", result.Output, StringComparison.Ordinal);
            Assert.Contains("AlterCount: 1", result.Output, StringComparison.Ordinal);
            Assert.Contains("DropCount: 0", result.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Single(manifest.AddTableColumnList);
            Assert.Single(manifest.AlterTableColumnList);
            Assert.Empty(manifest.DropTableList);
            Assert.Empty(manifest.DropForeignKeyList);
            Assert.Empty(manifest.DropPrimaryKeyList);
            Assert.Empty(manifest.DropIndexList);
            Assert.Empty(manifest.DropTableColumnList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_WithExactDataDropTableApproval_EmitsDropTableForLiveOnlyDrift()
    {
        var repoRoot = FindRepositoryRoot();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployWithDrop_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateParentChildWithForeignKey(databaseConnectionString);
            await CreateSourceWorkspaceWithChildOnlyNoForeignKeyAsync(sourcePath, databaseName);

            var startInfo = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --approve-drop-table raw.Parent --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("DropCount: 2", result.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Single(manifest.DropTableList);
            Assert.Single(manifest.DropForeignKeyList);
            Assert.Empty(manifest.BlockTableDifferenceList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_TableDropApproval_IsExactScopeOnly()
    {
        var repoRoot = FindRepositoryRoot();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployScopeDrop_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateParentChildWithForeignKey(databaseConnectionString);
            await CreateSourceWorkspaceWithChildOnlyNoForeignKeyAsync(sourcePath, databaseName);

            var startInfo = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --approve-drop-table raw.NotParent --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("missing approval DataDropTable(raw.Parent)", result.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Empty(manifest.DropTableList);
            Assert.Single(manifest.DropForeignKeyList);
            Assert.Single(manifest.BlockTableDifferenceList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_EmitsDropPrimaryKeyAndDropIndexByDefault()
    {
        var repoRoot = FindRepositoryRoot();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployNoDataDropPkIndex_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreatePkIndexOnlyDriftFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "PkIndexCase",
                model =>
                {
                    var schema = model.SchemaList.Single(row => string.Equals(row.Name, "raw", StringComparison.OrdinalIgnoreCase));
                    var table = model.TableList.Single(row => row.SchemaId == schema.Id && string.Equals(row.Name, "PkIndexCase", StringComparison.OrdinalIgnoreCase));

                    var primaryKeyIds = model.PrimaryKeyList
                        .Where(row => row.TableId == table.Id)
                        .Select(row => row.Id)
                        .ToHashSet(StringComparer.Ordinal);
                    model.PrimaryKeyColumnList.RemoveAll(row => primaryKeyIds.Contains(row.PrimaryKeyId));
                    model.PrimaryKeyList.RemoveAll(row => primaryKeyIds.Contains(row.Id));

                    var indexIds = model.IndexList
                        .Where(row => row.TableId == table.Id)
                        .Select(row => row.Id)
                        .ToHashSet(StringComparer.Ordinal);
                    model.IndexColumnList.RemoveAll(row => indexIds.Contains(row.IndexId));
                    model.IndexList.RemoveAll(row => indexIds.Contains(row.Id));
                });

            var startInfo = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("DropCount: 2", result.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Single(manifest.DropPrimaryKeyList);
            Assert.Single(manifest.DropIndexList);
            Assert.Empty(manifest.DropTableList);
            Assert.Empty(manifest.DropTableColumnList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_WithExactDataDropApprovals_EmitsDropTableAndDropTableColumn()
    {
        var repoRoot = FindRepositoryRoot();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployDataDropTableColumn_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateTableAndColumnOnlyDataDropFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                null,
                model =>
                {
                    var schema = model.SchemaList.Single(row => string.Equals(row.Name, "raw", StringComparison.OrdinalIgnoreCase));
                    var legacyTable = model.TableList.Single(row => row.SchemaId == schema.Id && string.Equals(row.Name, "LegacyOnly", StringComparison.OrdinalIgnoreCase));
                    var activeTable = model.TableList.Single(row => row.SchemaId == schema.Id && string.Equals(row.Name, "ActiveCase", StringComparison.OrdinalIgnoreCase));
                    var legacyColumn = model.TableColumnList.Single(row => row.TableId == activeTable.Id && string.Equals(row.Name, "LegacyCol", StringComparison.OrdinalIgnoreCase));

                    var legacyTableId = legacyTable.Id;
                    var legacyColumnId = legacyColumn.Id;

                    model.TableColumnDataTypeDetailList.RemoveAll(row => row.TableColumnId == legacyColumnId);
                    model.TableColumnList.RemoveAll(row => row.Id == legacyColumnId);

                    var legacyPrimaryKeyIds = model.PrimaryKeyList
                        .Where(row => row.TableId == legacyTableId)
                        .Select(row => row.Id)
                        .ToHashSet(StringComparer.Ordinal);
                    model.PrimaryKeyColumnList.RemoveAll(row => legacyPrimaryKeyIds.Contains(row.PrimaryKeyId));
                    model.PrimaryKeyList.RemoveAll(row => legacyPrimaryKeyIds.Contains(row.Id));

                    var legacyIndexIds = model.IndexList
                        .Where(row => row.TableId == legacyTableId)
                        .Select(row => row.Id)
                        .ToHashSet(StringComparer.Ordinal);
                    model.IndexColumnList.RemoveAll(row => legacyIndexIds.Contains(row.IndexId));
                    model.IndexList.RemoveAll(row => legacyIndexIds.Contains(row.Id));

                    var legacyForeignKeyIds = model.ForeignKeyList
                        .Where(row => row.SourceTableId == legacyTableId || row.TargetTableId == legacyTableId)
                        .Select(row => row.Id)
                        .ToHashSet(StringComparer.Ordinal);
                    model.ForeignKeyColumnList.RemoveAll(row => legacyForeignKeyIds.Contains(row.ForeignKeyId));
                    model.ForeignKeyList.RemoveAll(row => legacyForeignKeyIds.Contains(row.Id));

                    var legacyColumnIds = model.TableColumnList
                        .Where(row => row.TableId == legacyTableId)
                        .Select(row => row.Id)
                        .ToHashSet(StringComparer.Ordinal);
                    model.TableColumnDataTypeDetailList.RemoveAll(row => legacyColumnIds.Contains(row.TableColumnId));
                    model.TableColumnList.RemoveAll(row => legacyColumnIds.Contains(row.Id));
                    model.TableList.RemoveAll(row => row.Id == legacyTableId);
                });

            var startInfo = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --approve-drop-table raw.LegacyOnly --approve-drop-column raw.ActiveCase.LegacyCol --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(0, result.ExitCode);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Single(manifest.DropTableList);
            Assert.Single(manifest.DropTableColumnList);
            Assert.Empty(manifest.BlockTableDifferenceList);
            Assert.Empty(manifest.BlockTableColumnDifferenceList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

}
