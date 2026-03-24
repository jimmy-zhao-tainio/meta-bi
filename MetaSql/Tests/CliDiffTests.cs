using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using MetaSql;
using MetaSqlDeployManifest;
using MetaSql.Extractors.SqlServer;

namespace MetaSql.Tests;

public sealed class CliDiffTests
{
    [Fact]
    public void DeployPlanHelp_RendersExpectedUsage()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");

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
        Assert.Contains("--approve-drop-table", result.Output, StringComparison.Ordinal);
        Assert.Contains("--approve-drop-column", result.Output, StringComparison.Ordinal);
        Assert.Contains("--approve-truncate-column", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("[--with-data-drop]", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("[--with-data-truncate]", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("--with-drop", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("--allow-drop", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void DeployHelp_RendersExpectedUsage()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");

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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
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
    public async Task DeployPlanCommand_WithoutDataDropApproval_BlocksLiveOnlyDataDrop()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
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
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --with-data-drop --out \"{outputPath}\"",
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
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
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --approve-drop-table raw.Parent --out \"{outputPath}\"",
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
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
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --approve-drop-table raw.NotParent --out \"{outputPath}\"",
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
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
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table PkIndexCase --out \"{outputPath}\"",
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
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
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --approve-drop-table raw.LegacyOnly --approve-drop-column raw.ActiveCase.LegacyCol --out \"{outputPath}\"",
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

    [Fact]
    public async Task DeployPlanCommand_EmitsReplaceForeignKeyForExecutableSharedForeignKeyDifference()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplaceFkPlan_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateForeignKeyReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceWithSingleForeignKeyTargetAsync(sourcePath, databaseName, targetTableName: "ParentB", includeForeignKeyMember: true);

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
            Assert.Contains("ReplaceCount: 1", result.Output, StringComparison.Ordinal);
            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Single(manifest.ReplaceForeignKeyList);
            Assert.Empty(manifest.BlockForeignKeyDifferenceList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_EmitsBlockForeignKeyDifferenceForNonExecutableSharedForeignKeyDifference()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplaceFkBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateForeignKeyReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceWithSingleForeignKeyTargetAsync(sourcePath, databaseName, targetTableName: "ParentB", includeForeignKeyMember: false);

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

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Empty(manifest.ReplaceForeignKeyList);
            var block = Assert.Single(manifest.BlockForeignKeyDifferenceList);
            Assert.Contains("no member rows", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_RefusesWhenForeignKeyBlockEntryIsPresent()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplaceFkRefuse_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateForeignKeyReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceWithSingleForeignKeyTargetAsync(sourcePath, databaseName, targetTableName: "ParentB", includeForeignKeyMember: false);

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
            Assert.Equal("ParentA", GetForeignKeyTargetTableName(databaseConnectionString, "FK_Child_Parent"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_AppliesReplaceForeignKeyAction()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplaceFkApply_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateForeignKeyReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceWithSingleForeignKeyTargetAsync(sourcePath, databaseName, targetTableName: "ParentB", includeForeignKeyMember: true);

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
            Assert.Contains("ReplaceCount: 1", planResult.Output, StringComparison.Ordinal);

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
            Assert.Contains("AppliedReplaceCount: 1", deployResult.Output, StringComparison.Ordinal);
            Assert.Equal("ParentB", GetForeignKeyTargetTableName(databaseConnectionString, "FK_Child_Parent"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_RollsBackWhenSecondForeignKeyReplacementFails()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplaceFkRollback_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateForeignKeyReplaceRollbackFixture(databaseConnectionString);
            await CreateSourceWorkspaceWithTwoForeignKeysTargetingParentBAsync(sourcePath, databaseName);

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
            Assert.Contains("ReplaceCount: 2", planResult.Output, StringComparison.Ordinal);

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
            Assert.Contains("SQL deploy failed at statement 4:", deployResult.Output, StringComparison.Ordinal);

            Assert.Equal("ParentA", GetForeignKeyTargetTableName(databaseConnectionString, "FK_ChildA_Parent"));
            Assert.Equal("ParentA", GetForeignKeyTargetTableName(databaseConnectionString, "FK_ChildB_Parent"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_EmitsReplacePrimaryKeyForExecutableSharedNonClusteredPrimaryKeyDifference()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplacePkPlan_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreatePrimaryKeyReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "PkReplaceCase",
                model =>
                {
                    var primaryKey = RequirePrimaryKey(model, "raw", "PkReplaceCase", "PK_PkReplaceCase");
                    var keyA = RequireColumn(model, "raw", "PkReplaceCase", "KeyA");
                    var keyB = RequireColumn(model, "raw", "PkReplaceCase", "KeyB");
                    SetPrimaryKeyMembers(model, primaryKey, [keyA, keyB]);
                });

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table PkReplaceCase --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("ReplaceCount: 1", result.Output, StringComparison.Ordinal);
            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Single(manifest.ReplacePrimaryKeyList);
            Assert.Empty(manifest.BlockPrimaryKeyDifferenceList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanAndDeploy_EmitAndApplyExplicitDependentForeignKeyReplacementForPrimaryKeyReplacement()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplacePkDependentFk_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreatePrimaryKeyReplaceWithDependentForeignKeyFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                tableName: null,
                model =>
                {
                    var primaryKey = RequirePrimaryKey(model, "raw", "ParentPkCase", "PK_ParentPkCase");
                    var primaryKeyMember = model.PrimaryKeyColumnList.Single(row =>
                        row.PrimaryKeyId == primaryKey.Id &&
                        string.Equals(row.Ordinal, "1", StringComparison.Ordinal));
                    primaryKeyMember.IsDescending = "true";
                });

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
            Assert.Contains("ReplaceCount: 2", planResult.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.ReplacePrimaryKeyList);
            Assert.Single(manifest.ReplaceForeignKeyList);

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
            Assert.Contains("AppliedReplaceCount: 2", deployResult.Output, StringComparison.Ordinal);
            Assert.True(ForeignKeyExists(databaseConnectionString, "FK_ChildPkCase_ParentPkCase"));
            Assert.Equal("ParentPkCase", GetForeignKeyTargetTableName(databaseConnectionString, "FK_ChildPkCase_ParentPkCase"));
            Assert.True(GetPrimaryKeyKeyIsDescending(databaseConnectionString, "raw", "ParentPkCase", "KeyA"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_BlocksReplacePrimaryKeyForClusteredPrimaryKeyChange()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplacePkClustered_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateClusteredPrimaryKeyReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "PkClusteredCase",
                model =>
                {
                    var primaryKey = RequirePrimaryKey(model, "raw", "PkClusteredCase", "PK_PkClusteredCase");
                    var keyA = RequireColumn(model, "raw", "PkClusteredCase", "KeyA");
                    var keyB = RequireColumn(model, "raw", "PkClusteredCase", "KeyB");
                    SetPrimaryKeyMembers(model, primaryKey, [keyA, keyB]);
                });

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table PkClusteredCase --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("deploy-plan produced a non-deployable manifest.", result.Output, StringComparison.Ordinal);
            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Empty(manifest.ReplacePrimaryKeyList);
            var block = Assert.Single(manifest.BlockPrimaryKeyDifferenceList);
            Assert.Contains("clustered primary key replacement is blocked", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_BlocksReplacePrimaryKeyWhenMemberRowsAreMissing()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplacePkMembers_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreatePrimaryKeyReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "PkReplaceCase",
                model =>
                {
                    var primaryKey = RequirePrimaryKey(model, "raw", "PkReplaceCase", "PK_PkReplaceCase");
                    model.PrimaryKeyColumnList.RemoveAll(row => row.PrimaryKeyId == primaryKey.Id);
                });

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table PkReplaceCase --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("deploy-plan produced a non-deployable manifest.", result.Output, StringComparison.Ordinal);
            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Empty(manifest.ReplacePrimaryKeyList);
            var block = Assert.Single(manifest.BlockPrimaryKeyDifferenceList);
            Assert.Contains("no member rows", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_BlocksReplacePrimaryKeyWhenDependentForeignKeyChoreographyIsUnsupported()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplacePkFkBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreatePrimaryKeyReplaceWithDependentForeignKeyFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                tableName: null,
                model =>
                {
                    var primaryKey = RequirePrimaryKey(model, "raw", "ParentPkCase", "PK_ParentPkCase");
                    var keyA = RequireColumn(model, "raw", "ParentPkCase", "KeyA");
                    var keyB = RequireColumn(model, "raw", "ParentPkCase", "KeyB");
                    SetPrimaryKeyMembers(model, primaryKey, [keyA, keyB]);
                });

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
            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Empty(manifest.ReplacePrimaryKeyList);
            var block = Assert.Single(manifest.BlockPrimaryKeyDifferenceList);
            Assert.Contains("unsupported target-column shape", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_RefusesWhenPrimaryKeyBlockEntryIsPresent()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplacePkRefuse_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateClusteredPrimaryKeyReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "PkClusteredCase",
                model =>
                {
                    var primaryKey = RequirePrimaryKey(model, "raw", "PkClusteredCase", "PK_PkClusteredCase");
                    var keyA = RequireColumn(model, "raw", "PkClusteredCase", "KeyA");
                    var keyB = RequireColumn(model, "raw", "PkClusteredCase", "KeyB");
                    SetPrimaryKeyMembers(model, primaryKey, [keyA, keyB]);
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table PkClusteredCase --out \"{planPath}\"",
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
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table PkClusteredCase",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(5, deployResult.ExitCode);
            Assert.Contains("Manifest 'DeployManifest' is non-deployable. BlockCount=1.", deployResult.Output, StringComparison.Ordinal);
            Assert.Equal(["KeyA"], GetPrimaryKeyKeyColumns(databaseConnectionString, "raw", "PkClusteredCase"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_AppliesReplacePrimaryKeyAction()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplacePkApply_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreatePrimaryKeyReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "PkReplaceCase",
                model =>
                {
                    var primaryKey = RequirePrimaryKey(model, "raw", "PkReplaceCase", "PK_PkReplaceCase");
                    var keyA = RequireColumn(model, "raw", "PkReplaceCase", "KeyA");
                    var keyB = RequireColumn(model, "raw", "PkReplaceCase", "KeyB");
                    SetPrimaryKeyMembers(model, primaryKey, [keyA, keyB]);
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table PkReplaceCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            Assert.Contains("ReplaceCount: 1", planResult.Output, StringComparison.Ordinal);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table PkReplaceCase",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(0, deployResult.ExitCode);
            Assert.Contains("AppliedReplaceCount: 1", deployResult.Output, StringComparison.Ordinal);
            Assert.Equal(["KeyA", "KeyB"], GetPrimaryKeyKeyColumns(databaseConnectionString, "raw", "PkReplaceCase"));
            Assert.False(GetPrimaryKeyIsClustered(databaseConnectionString, "raw", "PkReplaceCase"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_RollsBackWhenSecondPrimaryKeyReplacementFails()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplacePkRollback_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreatePrimaryKeyReplaceRollbackFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                tableName: null,
                model =>
                {
                    var primaryKeyA = RequirePrimaryKey(model, "raw", "PkRollbackA", "PK_PkRollbackA");
                    var keyAA = RequireColumn(model, "raw", "PkRollbackA", "KeyA");
                    var keyAB = RequireColumn(model, "raw", "PkRollbackA", "KeyB");
                    SetPrimaryKeyMembers(model, primaryKeyA, [keyAA, keyAB]);

                    var primaryKeyB = RequirePrimaryKey(model, "raw", "PkRollbackB", "PK_PkRollbackB");
                    var keyBB = RequireColumn(model, "raw", "PkRollbackB", "KeyB");
                    SetPrimaryKeyMembers(model, primaryKeyB, [keyBB]);
                });

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
            Assert.Contains("ReplaceCount: 2", planResult.Output, StringComparison.Ordinal);

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
            Assert.Contains("SQL deploy failed at statement", deployResult.Output, StringComparison.Ordinal);

            Assert.Equal(["KeyA"], GetPrimaryKeyKeyColumns(databaseConnectionString, "raw", "PkRollbackA"));
            Assert.Equal(["KeyA"], GetPrimaryKeyKeyColumns(databaseConnectionString, "raw", "PkRollbackB"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_EmitsReplaceIndexForExecutableSharedNonClusteredIndexDifference()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplaceIndexPlan_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateIndexReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "IndexReplaceCase",
                model =>
                {
                    var member = RequireIndexMember(model, "raw", "IndexReplaceCase", "IX_IndexReplaceCase_Payload", "Payload");
                    member.IsDescending = "true";
                });

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table IndexReplaceCase --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("ReplaceCount: 1", result.Output, StringComparison.Ordinal);
            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Single(manifest.ReplaceIndexList);
            Assert.Empty(manifest.BlockIndexDifferenceList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_BlocksReplaceIndexForClusteredIndexChange()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplaceIndexClustered_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateClusteredIndexReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "IndexClusteredCase",
                model =>
                {
                    var member = RequireIndexMember(model, "raw", "IndexClusteredCase", "IX_IndexClusteredCase_Payload", "Payload");
                    member.IsDescending = "true";
                });

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table IndexClusteredCase --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("deploy-plan produced a non-deployable manifest.", result.Output, StringComparison.Ordinal);
            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Empty(manifest.ReplaceIndexList);
            var block = Assert.Single(manifest.BlockIndexDifferenceList);
            Assert.Contains("clustered index replacement is blocked", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_BlocksReplaceIndexForNonExecutableSharedIndexDifference()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplaceIndexBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateIndexReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "IndexReplaceCase",
                model =>
                {
                    var index = RequireIndex(model, "raw", "IndexReplaceCase", "IX_IndexReplaceCase_Payload");
                    model.IndexColumnList.RemoveAll(row => row.IndexId == index.Id);
                });

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table IndexReplaceCase --out \"{outputPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var result = RunProcess(startInfo, "Could not start MetaSql CLI process.");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("deploy-plan produced a non-deployable manifest.", result.Output, StringComparison.Ordinal);
            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Empty(manifest.ReplaceIndexList);
            var block = Assert.Single(manifest.BlockIndexDifferenceList);
            Assert.Contains("no member rows", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_RefusesWhenIndexBlockEntryIsPresent()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplaceIndexRefuse_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateClusteredIndexReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "IndexClusteredCase",
                model =>
                {
                    var member = RequireIndexMember(model, "raw", "IndexClusteredCase", "IX_IndexClusteredCase_Payload", "Payload");
                    member.IsDescending = "true";
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table IndexClusteredCase --out \"{planPath}\"",
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
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table IndexClusteredCase",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(5, deployResult.ExitCode);
            Assert.Contains("Manifest 'DeployManifest' is non-deployable. BlockCount=1.", deployResult.Output, StringComparison.Ordinal);
            Assert.False(GetIndexIsDescending(databaseConnectionString, "IX_IndexClusteredCase_Payload", "Payload"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_AppliesReplaceIndexAction()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplaceIndexApply_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateIndexReplaceFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "IndexReplaceCase",
                model =>
                {
                    var index = RequireIndex(model, "raw", "IndexReplaceCase", "IX_IndexReplaceCase_Payload");
                    index.IsUnique = "true";
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table IndexReplaceCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            Assert.Contains("ReplaceCount: 1", planResult.Output, StringComparison.Ordinal);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table IndexReplaceCase",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(0, deployResult.ExitCode);
            Assert.Contains("AppliedReplaceCount: 1", deployResult.Output, StringComparison.Ordinal);
            Assert.True(GetIndexIsUnique(databaseConnectionString, "IX_IndexReplaceCase_Payload"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_RollsBackWhenSecondIndexReplacementFails()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployReplaceIndexRollback_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateIndexReplaceRollbackFixture(databaseConnectionString);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "IndexRollbackCase",
                model =>
                {
                    RequireIndex(model, "raw", "IndexRollbackCase", "IX_IndexRollback_A").IsUnique = "true";
                    RequireIndex(model, "raw", "IndexRollbackCase", "IX_IndexRollback_B").IsUnique = "true";
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table IndexRollbackCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            Assert.Contains("ReplaceCount: 2", planResult.Output, StringComparison.Ordinal);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table IndexRollbackCase",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(5, deployResult.ExitCode);
            Assert.Contains("SQL deploy failed at statement", deployResult.Output, StringComparison.Ordinal);

            Assert.False(GetIndexIsUnique(databaseConnectionString, "IX_IndexRollback_A"));
            Assert.False(GetIndexIsUnique(databaseConnectionString, "IX_IndexRollback_B"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_EmitsAlterTableColumnForExecutableSharedObjectDifference()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployTestBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString, customerIdLength: 50);
            await CreateSourceWorkspaceWithCustomerIdLengthAsync(sourcePath, databaseName, customerIdLength: 100);

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
            Assert.Contains("AlterCount: 1", result.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Single(manifest.AlterTableColumnList);
            Assert.Empty(manifest.BlockTableColumnDifferenceList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_EmitsBlockInsteadOfAlterForInfeasibleSharedColumnDifference()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var outputPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployPlanBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString, customerIdLength: 200, customerIdValue: new string('X', 150));
            await CreateSourceWorkspaceWithCustomerIdLengthAsync(sourcePath, databaseName, customerIdLength: 100);

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

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(outputPath, searchUpward: false);
            Assert.Empty(manifest.AlterTableColumnList);
            var block = Assert.Single(manifest.BlockTableColumnDifferenceList);
            Assert.Contains("smaller than live data currently stored", block.DifferenceSummary, StringComparison.Ordinal);
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployBlockRefusal_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString, customerIdLength: 200, customerIdValue: new string('X', 150));
            await CreateSourceWorkspaceWithCustomerIdLengthAsync(sourcePath, databaseName, customerIdLength: 100);

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
            Assert.Equal(400, GetColumnMaxLengthBytes(databaseConnectionString, "raw", "H_Customer", "CustomerId"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_AppliesAlterTableColumnAction()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployAlterApply_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString, customerIdLength: 50);
            await CreateSourceWorkspaceWithCustomerIdLengthAsync(sourcePath, databaseName, customerIdLength: 100);

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
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);

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
            Assert.Contains("AppliedAlterCount: 1", deployResult.Output, StringComparison.Ordinal);

            Assert.Equal(200, GetColumnMaxLengthBytes(databaseConnectionString, "raw", "H_Customer", "CustomerId"));
            Assert.False(GetColumnNullable(databaseConnectionString, "raw", "H_Customer", "CustomerId"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_AppliesVarcharLengthWidening()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterVarcharWide_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateVarcharCaseTable(databaseConnectionString, length: 50, seedValue: "short");
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "VarcharCase",
                model =>
                {
                    var column = RequireColumn(model, "raw", "VarcharCase", "ValueCol");
                    SetOrReplaceColumnDetail(model, column, "Length", "100");
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table VarcharCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table VarcharCase",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(0, deployResult.ExitCode);
            Assert.Contains("AppliedAlterCount: 1", deployResult.Output, StringComparison.Ordinal);

            Assert.Equal(100, GetColumnMaxLengthBytes(databaseConnectionString, "raw", "VarcharCase", "ValueCol"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_BlocksVarcharLengthNarrowingWhenDataWouldTruncate()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterVarcharNarrow_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateVarcharCaseTable(databaseConnectionString, length: 100, seedValue: new string('X', 80));
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "VarcharCase",
                model =>
                {
                    var column = RequireColumn(model, "raw", "VarcharCase", "ValueCol");
                    SetOrReplaceColumnDetail(model, column, "Length", "50");
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table VarcharCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(4, planResult.ExitCode);
            Assert.Contains("deploy-plan produced a non-deployable manifest.", planResult.Output, StringComparison.Ordinal);
            Assert.Contains("Missing approval DataTruncationColumn(raw.VarcharCase.ValueCol)", planResult.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Empty(manifest.AlterTableColumnList);
            var block = Assert.Single(manifest.BlockTableColumnDifferenceList);
            Assert.Contains("smaller than live data currently stored", block.DifferenceSummary, StringComparison.Ordinal);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanAndDeploy_WithExactDataTruncationApproval_NarrowsLengthAndTruncatesData()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterVarcharNarrowTruncate_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateVarcharCaseTable(databaseConnectionString, length: 100, seedValue: new string('X', 80));
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "VarcharCase",
                model =>
                {
                    var column = RequireColumn(model, "raw", "VarcharCase", "ValueCol");
                    SetOrReplaceColumnDetail(model, column, "Length", "50");
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table VarcharCase --approve-truncate-column raw.VarcharCase.ValueCol --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);
            Assert.Contains("TruncateCount: 1", planResult.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.AlterTableColumnList);
            Assert.Single(manifest.TruncateTableColumnDataList);
            Assert.Empty(manifest.BlockTableColumnDifferenceList);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table VarcharCase",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(0, deployResult.ExitCode);
            Assert.Contains("AppliedAlterCount: 1", deployResult.Output, StringComparison.Ordinal);
            Assert.Contains("AppliedTruncateCount: 1", deployResult.Output, StringComparison.Ordinal);

            Assert.Equal(50, GetColumnMaxLengthBytes(databaseConnectionString, "raw", "VarcharCase", "ValueCol"));
            Assert.Equal(50, GetValueLength(databaseConnectionString, "raw", "VarcharCase", "ValueCol", 1));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_BlocksNullableToNotNullWhenLiveContainsNulls()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterNullBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateNullableCaseTable(databaseConnectionString, includeNullRow: true);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "NullableCase",
                model =>
                {
                    var column = RequireColumn(model, "raw", "NullableCase", "ValueCol");
                    column.IsNullable = "false";
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table NullableCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(4, planResult.ExitCode);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Empty(manifest.AlterTableColumnList);
            var block = Assert.Single(manifest.BlockTableColumnDifferenceList);
            Assert.Contains("contains NULL values", block.DifferenceSummary, StringComparison.Ordinal);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_AppliesNullableToNotNullWhenLiveContainsNoNulls()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterNullSuccess_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateNullableCaseTable(databaseConnectionString, includeNullRow: false);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "NullableCase",
                model =>
                {
                    var column = RequireColumn(model, "raw", "NullableCase", "ValueCol");
                    column.IsNullable = "false";
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table NullableCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table NullableCase",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(0, deployResult.ExitCode);

            Assert.False(GetColumnNullable(databaseConnectionString, "raw", "NullableCase", "ValueCol"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_BlocksDecimalPrecisionScaleChangeInCurrentSlice()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterDecimalBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateDecimalCaseTable(databaseConnectionString, precision: 18, scale: 2);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "DecimalCase",
                model =>
                {
                    var column = RequireColumn(model, "raw", "DecimalCase", "Amount");
                    SetOrReplaceColumnDetail(model, column, "Precision", "20");
                    SetOrReplaceColumnDetail(model, column, "Scale", "2");
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table DecimalCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(4, planResult.ExitCode);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Empty(manifest.AlterTableColumnList);
            var block = Assert.Single(manifest.BlockTableColumnDifferenceList);
            Assert.Contains("only length-based sqlserver types are executable", block.DifferenceSummary, StringComparison.Ordinal);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_BlocksNonSqlServerMetaDataTypeForAlter()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterNonSqlType_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateVarcharCaseTable(databaseConnectionString, length: 50, seedValue: "abc");
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "VarcharCase",
                model =>
                {
                    var column = RequireColumn(model, "raw", "VarcharCase", "ValueCol");
                    column.MetaDataTypeId = "meta:type:String";
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table VarcharCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(4, planResult.ExitCode);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Empty(manifest.AlterTableColumnList);
            var block = Assert.Single(manifest.BlockTableColumnDifferenceList);
            Assert.Contains("supports only sqlserver:type:*", block.DifferenceSummary, StringComparison.Ordinal);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_BlocksComputedExpressionChange()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterExpressionBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
                CREATE TABLE raw.ExprCase (
                    ValueCol int NOT NULL
                );
                """);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "ExprCase",
                model =>
                {
                    var column = RequireColumn(model, "raw", "ExprCase", "ValueCol");
                    column.ExpressionSql = "([ValueCol] + 1)";
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table ExprCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(4, planResult.ExitCode);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Empty(manifest.AlterTableColumnList);
            var block = Assert.Single(manifest.BlockTableColumnDifferenceList);
            Assert.Contains("unsupported column aspect change(s): ExpressionSql", block.DifferenceSummary, StringComparison.Ordinal);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_BlocksAlterWhenColumnParticipatesInPrimaryKey()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterPkBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
                CREATE TABLE raw.PkCase (
                    KeyCol int NOT NULL,
                    CONSTRAINT PK_PkCase PRIMARY KEY (KeyCol)
                );
                """);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "PkCase",
                model =>
                {
                    var column = RequireColumn(model, "raw", "PkCase", "KeyCol");
                    column.IsNullable = "true";
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table PkCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(4, planResult.ExitCode);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.AlterTableColumnList);
            var block = Assert.Single(manifest.BlockTableColumnDifferenceList);
            Assert.Contains("dependent primary key", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanAndDeploy_AppliesAlterWhenColumnParticipatesInNonClusteredPrimaryKey()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterPkApply_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
                CREATE TABLE raw.PkAlterCase (
                    KeyCol varchar(50) NOT NULL,
                    CONSTRAINT PK_PkAlterCase PRIMARY KEY NONCLUSTERED (KeyCol)
                );
                """);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "PkAlterCase",
                model =>
                {
                    var column = RequireColumn(model, "raw", "PkAlterCase", "KeyCol");
                    SetOrReplaceColumnDetail(model, column, "Length", "100");
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table PkAlterCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);
            Assert.Contains("ReplaceCount: 1", planResult.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.AlterTableColumnList);
            Assert.Single(manifest.ReplacePrimaryKeyList);
            Assert.Empty(manifest.BlockTableColumnDifferenceList);
            Assert.Empty(manifest.BlockPrimaryKeyDifferenceList);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table PkAlterCase",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(0, deployResult.ExitCode);
            Assert.Contains("AppliedAlterCount: 1", deployResult.Output, StringComparison.Ordinal);
            Assert.Contains("AppliedReplaceCount: 1", deployResult.Output, StringComparison.Ordinal);
            Assert.Equal(100, GetColumnMaxLengthBytes(databaseConnectionString, "raw", "PkAlterCase", "KeyCol"));
            Assert.Equal(["KeyCol"], GetPrimaryKeyKeyColumns(databaseConnectionString, "raw", "PkAlterCase"));
            Assert.False(GetPrimaryKeyIsClustered(databaseConnectionString, "raw", "PkAlterCase"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanAndDeploy_AppliesAlterWhenColumnParticipatesInForeignKey()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterFkBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
                CREATE TABLE raw.ParentCase (
                    ParentCode varchar(50) NOT NULL,
                    CONSTRAINT PK_ParentCase PRIMARY KEY (ParentCode)
                );
                CREATE TABLE raw.FkCase (
                    ChildId int NOT NULL,
                    ParentCode varchar(50) NOT NULL,
                    CONSTRAINT PK_FkCase PRIMARY KEY (ChildId)
                );
                ALTER TABLE raw.FkCase
                    ADD CONSTRAINT FK_FkCase_ParentCase
                    FOREIGN KEY (ParentCode) REFERENCES raw.ParentCase(ParentCode);
                """);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                tableName: null,
                model =>
                {
                    var column = RequireColumn(model, "raw", "FkCase", "ParentCode");
                    column.IsNullable = "true";
                });

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
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);
            Assert.Contains("ReplaceCount: 1", planResult.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.AlterTableColumnList);
            Assert.Single(manifest.ReplaceForeignKeyList);
            Assert.Empty(manifest.BlockTableColumnDifferenceList);

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
            Assert.Contains("AppliedAlterCount: 1", deployResult.Output, StringComparison.Ordinal);
            Assert.Contains("AppliedReplaceCount: 1", deployResult.Output, StringComparison.Ordinal);
            Assert.True(GetColumnNullable(databaseConnectionString, "raw", "FkCase", "ParentCode"));
            Assert.True(ForeignKeyExists(databaseConnectionString, "FK_FkCase_ParentCase"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanAndDeploy_AppliesAlterWhenColumnParticipatesInIndexIncludedMember()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterIndexBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
                CREATE TABLE raw.IndexCase (
                    Id int NOT NULL,
                    Payload varchar(50) NOT NULL,
                    Note varchar(50) NULL,
                    CONSTRAINT PK_IndexCase PRIMARY KEY (Id)
                );
                CREATE INDEX IX_IndexCase_Id
                    ON raw.IndexCase (Id)
                    INCLUDE (Payload);
                """);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "IndexCase",
                model =>
                {
                    var column = RequireColumn(model, "raw", "IndexCase", "Payload");
                    SetOrReplaceColumnDetail(model, column, "Length", "100");
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table IndexCase --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);
            Assert.Contains("ReplaceCount: 1", planResult.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.AlterTableColumnList);
            Assert.Single(manifest.ReplaceIndexList);
            Assert.Empty(manifest.BlockTableColumnDifferenceList);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table IndexCase",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(0, deployResult.ExitCode);
            Assert.Contains("AppliedAlterCount: 1", deployResult.Output, StringComparison.Ordinal);
            Assert.Contains("AppliedReplaceCount: 1", deployResult.Output, StringComparison.Ordinal);
            Assert.Equal(100, GetColumnMaxLengthBytes(databaseConnectionString, "raw", "IndexCase", "Payload"));
            Assert.True(IndexExists(databaseConnectionString, "IX_IndexCase_Id"));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployPlanCommand_BlocksAlterWhenColumnDependsOnClusteredIndex()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterClusteredIndexBlock_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
                CREATE TABLE raw.IndexCaseClustered (
                    Id int NOT NULL,
                    Payload varchar(50) NOT NULL,
                    CONSTRAINT PK_IndexCaseClustered PRIMARY KEY NONCLUSTERED (Id)
                );
                CREATE CLUSTERED INDEX IX_IndexCaseClustered_Payload
                    ON raw.IndexCaseClustered (Payload);
                """);
            await CreateSourceWorkspaceFromLiveAndMutateAsync(
                sourcePath,
                databaseConnectionString,
                "raw",
                "IndexCaseClustered",
                model =>
                {
                    var column = RequireColumn(model, "raw", "IndexCaseClustered", "Payload");
                    SetOrReplaceColumnDetail(model, column, "Length", "100");
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table IndexCaseClustered --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(4, planResult.ExitCode);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.AlterTableColumnList);
            Assert.Empty(manifest.ReplaceIndexList);
            var block = Assert.Single(manifest.BlockTableColumnDifferenceList);
            Assert.Contains("clustered index replacement is blocked", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task DeployCommand_RollsBackWhenSecondAlterFailsAtApplyTime()
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlAlterRollback_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString, customerIdLength: 50, customerIdValue: "CUST-1");
            await CreateSourceWorkspaceWithCustomerIdLengthAsync(sourcePath, databaseName, customerIdLength: 100);
            await MutateSourceWorkspaceAsync(
                sourcePath,
                model =>
                {
                    var recordSourceColumn = RequireColumn(model, "raw", "H_Customer", "RecordSource");
                    SetOrReplaceColumnDetail(model, recordSourceColumn, "Length", "100");
                });

            var planCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table H_Customer --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            Assert.Contains("AlterCount: 2", planResult.Output, StringComparison.Ordinal);

            var longRecordSource = new string('R', 150);
            ExecuteSql(databaseConnectionString, $"""
                INSERT INTO raw.H_Customer(HashKey, CustomerId, LoadTimestamp, RecordSource, AuditId)
                VALUES (CONVERT(binary(16), 0x00000000000000000000000000000002), N'CUST-2', SYSUTCDATETIME(), N'{longRecordSource}', 2);
                """);

            var deployCommand = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{cliPath}\" deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --table H_Customer",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var deployResult = RunProcess(deployCommand, "Could not start MetaSql CLI deploy process.");
            Assert.Equal(5, deployResult.ExitCode);
            Assert.Contains("SQL deploy failed at statement 2:", deployResult.Output, StringComparison.Ordinal);

            Assert.Equal(100, GetColumnMaxLengthBytes(databaseConnectionString, "raw", "H_Customer", "CustomerId"));
            Assert.Equal(512, GetColumnMaxLengthBytes(databaseConnectionString, "raw", "H_Customer", "RecordSource"));
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(tempRoot, "source-metasql");
        var planPath = Path.Combine(tempRoot, "deploy-manifest");
        var databaseName = $"MetaSqlDeployUnsupportedAction_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            CreateSimpleTable(databaseConnectionString, customerIdLength: 50);
            await CreateSourceWorkspaceWithCustomerIdLengthAsync(sourcePath, databaseName, customerIdLength: 100);

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
            Assert.Equal(100, GetColumnMaxLengthBytes(databaseConnectionString, "raw", "H_Customer", "CustomerId"));
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaSql", "Cli"), "meta-sql.dll");
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
                Arguments = $"\"{cliPath}\" deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --schema raw --approve-drop-table raw.Parent --out \"{planPath}\"",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var planResult = RunProcess(planCommand, "Could not start MetaSql CLI deploy-plan process.");
            Assert.Equal(0, planResult.ExitCode);
            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.DropTableList);
            Assert.Single(manifest.DropForeignKeyList);

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
        return CreateSourceWorkspaceWithCustomerIdLengthAsync(sourcePath, databaseName, customerIdLength: 20);
    }

    private static Task CreateSourceWorkspaceWithCustomerIdLengthAsync(
        string sourcePath,
        string databaseName,
        int customerIdLength)
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
                    new("raw", "H_Customer", "CustomerId", 2, false, "nvarchar", customerIdLength, null, null),
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

    private static Task CreateSourceWorkspaceWithSingleForeignKeyTargetAsync(
        string sourcePath,
        string databaseName,
        string targetTableName,
        bool includeForeignKeyMember)
    {
        SqlServerMetaSqlProjector.Project(
            newWorkspacePath: sourcePath,
            databaseName: databaseName,
            tableRows:
            [
                new SqlServerMetaSqlProjector.TableRow("raw", "ParentA"),
                new SqlServerMetaSqlProjector.TableRow("raw", "ParentB"),
                new SqlServerMetaSqlProjector.TableRow("raw", "Child")
            ],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ParentA"] =
                [
                    new("raw", "ParentA", "ParentId", 1, false, "int", null, null, null),
                ],
                ["raw.ParentB"] =
                [
                    new("raw", "ParentB", "ParentId", 1, false, "int", null, null, null),
                ],
                ["raw.Child"] =
                [
                    new("raw", "Child", "ChildId", 1, false, "int", null, null, null),
                    new("raw", "Child", "ParentId", 2, false, "int", null, null, null),
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ParentA"] = [new("PK_ParentA", true)],
                ["raw.ParentB"] = [new("PK_ParentB", true)],
                ["raw.Child"] = [new("PK_Child", true)],
            },
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ParentA"] = [new("PK_ParentA", 1, "ParentId", false)],
                ["raw.ParentB"] = [new("PK_ParentB", 1, "ParentId", false)],
                ["raw.Child"] = [new("PK_Child", 1, "ChildId", false)],
            },
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.Child"] = [new("FK_Child_Parent", "raw", targetTableName)],
            },
            foreignKeyColumnsByTableKey: includeForeignKeyMember
                ? new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
                {
                    ["raw.Child"] = [new("FK_Child_Parent", 1, "ParentId", "ParentId")],
                }
                : new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
            indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));
        return Task.CompletedTask;
    }

    private static Task CreateSourceWorkspaceWithTwoForeignKeysTargetingParentBAsync(string sourcePath, string databaseName)
    {
        SqlServerMetaSqlProjector.Project(
            newWorkspacePath: sourcePath,
            databaseName: databaseName,
            tableRows:
            [
                new SqlServerMetaSqlProjector.TableRow("raw", "ParentA"),
                new SqlServerMetaSqlProjector.TableRow("raw", "ParentB"),
                new SqlServerMetaSqlProjector.TableRow("raw", "ChildA"),
                new SqlServerMetaSqlProjector.TableRow("raw", "ChildB")
            ],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ParentA"] =
                [
                    new("raw", "ParentA", "ParentId", 1, false, "int", null, null, null),
                ],
                ["raw.ParentB"] =
                [
                    new("raw", "ParentB", "ParentId", 1, false, "int", null, null, null),
                ],
                ["raw.ChildA"] =
                [
                    new("raw", "ChildA", "ChildAId", 1, false, "int", null, null, null),
                    new("raw", "ChildA", "ParentId", 2, false, "int", null, null, null),
                ],
                ["raw.ChildB"] =
                [
                    new("raw", "ChildB", "ChildBId", 1, false, "int", null, null, null),
                    new("raw", "ChildB", "ParentId", 2, false, "int", null, null, null),
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ParentA"] = [new("PK_ParentA", true)],
                ["raw.ParentB"] = [new("PK_ParentB", true)],
                ["raw.ChildA"] = [new("PK_ChildA", true)],
                ["raw.ChildB"] = [new("PK_ChildB", true)],
            },
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ParentA"] = [new("PK_ParentA", 1, "ParentId", false)],
                ["raw.ParentB"] = [new("PK_ParentB", 1, "ParentId", false)],
                ["raw.ChildA"] = [new("PK_ChildA", 1, "ChildAId", false)],
                ["raw.ChildB"] = [new("PK_ChildB", 1, "ChildBId", false)],
            },
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ChildA"] = [new("FK_ChildA_Parent", "raw", "ParentB")],
                ["raw.ChildB"] = [new("FK_ChildB_Parent", "raw", "ParentB")],
            },
            foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ChildA"] = [new("FK_ChildA_Parent", 1, "ParentId", "ParentId")],
                ["raw.ChildB"] = [new("FK_ChildB_Parent", 1, "ParentId", "ParentId")],
            },
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
                    new("raw", "H_Customer", "LoadTimestamp", 3, false, "datetime2", null, 7, null),
                    new("raw", "H_Customer", "RecordSource", 4, false, "nvarchar", 256, null, null),
                    new("raw", "H_Customer", "AuditId", 5, false, "int", null, null, null),
                    new("raw", "H_Customer", "CustomerName", 6, true, "nvarchar", 200, null, null),
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

    private static async Task CreateSourceWorkspaceFromLiveAndMutateAsync(
        string sourcePath,
        string connectionString,
        string schemaName,
        string? tableName,
        Action<MetaSqlModel> mutate)
    {
        var extractor = new SqlServerMetaSqlExtractor();
        extractor.ExtractMetaSqlWorkspace(new SqlServerExtractRequest
        {
            NewWorkspacePath = sourcePath,
            ConnectionString = connectionString,
            SchemaName = schemaName,
            TableName = tableName,
        });

        await MutateSourceWorkspaceAsync(sourcePath, mutate);
    }

    private static async Task MutateSourceWorkspaceAsync(
        string sourcePath,
        Action<MetaSqlModel> mutate)
    {
        var model = await MetaSqlModel.LoadFromXmlWorkspaceAsync(sourcePath, searchUpward: false);
        mutate(model);
        await model.SaveToXmlWorkspaceAsync(sourcePath);
    }

    private static TableColumn RequireColumn(
        MetaSqlModel model,
        string schemaName,
        string tableName,
        string columnName)
    {
        var schema = model.SchemaList
            .Single(row => string.Equals(row.Name, schemaName, StringComparison.OrdinalIgnoreCase));
        var table = model.TableList
            .Single(row =>
                row.SchemaId == schema.Id &&
                string.Equals(row.Name, tableName, StringComparison.OrdinalIgnoreCase));
        return model.TableColumnList
            .Single(row =>
                row.TableId == table.Id &&
                string.Equals(row.Name, columnName, StringComparison.OrdinalIgnoreCase));
    }

    private static PrimaryKey RequirePrimaryKey(
        MetaSqlModel model,
        string schemaName,
        string tableName,
        string primaryKeyName)
    {
        var schema = model.SchemaList
            .Single(row => string.Equals(row.Name, schemaName, StringComparison.OrdinalIgnoreCase));
        var table = model.TableList
            .Single(row =>
                row.SchemaId == schema.Id &&
                string.Equals(row.Name, tableName, StringComparison.OrdinalIgnoreCase));
        return model.PrimaryKeyList
            .Single(row =>
                row.TableId == table.Id &&
                string.Equals(row.Name, primaryKeyName, StringComparison.OrdinalIgnoreCase));
    }

    private static void SetPrimaryKeyMembers(
        MetaSqlModel model,
        PrimaryKey primaryKey,
        IReadOnlyList<TableColumn> columns)
    {
        model.PrimaryKeyColumnList.RemoveAll(row => row.PrimaryKeyId == primaryKey.Id);
        for (var i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            model.PrimaryKeyColumnList.Add(new PrimaryKeyColumn
            {
                Id = $"{primaryKey.Id}.column.{i + 1}",
                PrimaryKeyId = primaryKey.Id,
                PrimaryKey = primaryKey,
                TableColumnId = column.Id,
                TableColumn = column,
                Ordinal = (i + 1).ToString(),
            });
        }
    }

    private static Index RequireIndex(
        MetaSqlModel model,
        string schemaName,
        string tableName,
        string indexName)
    {
        var schema = model.SchemaList
            .Single(row => string.Equals(row.Name, schemaName, StringComparison.OrdinalIgnoreCase));
        var table = model.TableList
            .Single(row =>
                row.SchemaId == schema.Id &&
                string.Equals(row.Name, tableName, StringComparison.OrdinalIgnoreCase));
        return model.IndexList
            .Single(row =>
                row.TableId == table.Id &&
                string.Equals(row.Name, indexName, StringComparison.OrdinalIgnoreCase));
    }

    private static IndexColumn RequireIndexMember(
        MetaSqlModel model,
        string schemaName,
        string tableName,
        string indexName,
        string columnName)
    {
        var index = RequireIndex(model, schemaName, tableName, indexName);
        var column = RequireColumn(model, schemaName, tableName, columnName);
        return model.IndexColumnList
            .Single(row =>
                row.IndexId == index.Id &&
                row.TableColumnId == column.Id);
    }

    private static void SetOrReplaceColumnDetail(
        MetaSqlModel model,
        TableColumn column,
        string detailName,
        string detailValue)
    {
        var existing = model.TableColumnDataTypeDetailList.FirstOrDefault(row =>
            row.TableColumnId == column.Id &&
            string.Equals(row.Name, detailName, StringComparison.OrdinalIgnoreCase));
        if (existing is null)
        {
            model.TableColumnDataTypeDetailList.Add(new TableColumnDataTypeDetail
            {
                Id = $"{column.Id}.detail.{detailName}",
                Name = detailName,
                Value = detailValue,
                TableColumnId = column.Id,
                TableColumn = column,
            });
            return;
        }

        existing.Value = detailValue;
    }

    private static void CreateVarcharCaseTable(
        string connectionString,
        int length,
        string? seedValue)
    {
        var script = $"""
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.VarcharCase (
                Id int NOT NULL,
                ValueCol varchar({length}) NOT NULL,
                CONSTRAINT PK_VarcharCase PRIMARY KEY (Id)
            );
            """;
        if (!string.IsNullOrEmpty(seedValue))
        {
            var escapedSeedValue = seedValue.Replace("'", "''", StringComparison.Ordinal);
            script += $"""

                INSERT INTO raw.VarcharCase(Id, ValueCol)
                VALUES (1, '{escapedSeedValue}');
                """;
        }

        ExecuteSql(connectionString, script);
    }

    private static void CreateNullableCaseTable(string connectionString, bool includeNullRow)
    {
        var script = """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.NullableCase (
                Id int NOT NULL,
                ValueCol varchar(50) NULL,
                CONSTRAINT PK_NullableCase PRIMARY KEY (Id)
            );
            INSERT INTO raw.NullableCase(Id, ValueCol)
            VALUES (1, 'not-null');
            """;
        if (includeNullRow)
        {
            script += """

                INSERT INTO raw.NullableCase(Id, ValueCol)
                VALUES (2, NULL);
                """;
        }

        ExecuteSql(connectionString, script);
    }

    private static void CreateDecimalCaseTable(
        string connectionString,
        int precision,
        int scale)
    {
        ExecuteSql(connectionString, $"""
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.DecimalCase (
                Id int NOT NULL,
                Amount decimal({precision},{scale}) NOT NULL,
                CONSTRAINT PK_DecimalCase PRIMARY KEY (Id)
            );
            INSERT INTO raw.DecimalCase(Id, Amount)
            VALUES (1, 123.45);
            """);
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

    private static void CreateSimpleTable(
        string databaseConnectionString,
        int customerIdLength = 50,
        string? customerIdValue = null)
    {
        var setupSql = $"""
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.H_Customer (
                HashKey binary(16) NOT NULL,
                CustomerId nvarchar({customerIdLength}) NOT NULL,
                LoadTimestamp datetime2(7) NOT NULL,
                RecordSource nvarchar(256) NOT NULL,
                AuditId int NOT NULL,
                CONSTRAINT PK_H_Customer PRIMARY KEY (HashKey)
            );
            """;

        if (!string.IsNullOrEmpty(customerIdValue))
        {
            var escapedValue = customerIdValue.Replace("'", "''", StringComparison.Ordinal);
            setupSql += $"""
                
                INSERT INTO raw.H_Customer(HashKey, CustomerId, LoadTimestamp, RecordSource, AuditId)
                VALUES (CONVERT(binary(16), 0x00000000000000000000000000000001), N'{escapedValue}', SYSUTCDATETIME(), N'SRC', 1);
                """;
        }

        ExecuteSql(databaseConnectionString, setupSql);
    }

    private static void CreatePkIndexOnlyDriftFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.PkIndexCase (
                Id int NOT NULL,
                Payload nvarchar(100) NOT NULL,
                CONSTRAINT PK_PkIndexCase PRIMARY KEY (Id)
            );
            CREATE INDEX IX_PkIndexCase_Payload
                ON raw.PkIndexCase (Payload);
            """);
    }

    private static void CreateTableAndColumnOnlyDataDropFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.ActiveCase (
                Id int NOT NULL,
                KeepCol nvarchar(50) NOT NULL,
                LegacyCol nvarchar(50) NULL,
                CONSTRAINT PK_ActiveCase PRIMARY KEY (Id)
            );
            CREATE TABLE raw.LegacyOnly (
                Id int NOT NULL,
                Payload nvarchar(50) NOT NULL,
                CONSTRAINT PK_LegacyOnly PRIMARY KEY (Id)
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

    private static void CreateForeignKeyReplaceFixture(string databaseConnectionString)
    {
        ExecuteSql(databaseConnectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.ParentA (
                ParentId int NOT NULL,
                CONSTRAINT PK_ParentA PRIMARY KEY (ParentId)
            );
            CREATE TABLE raw.ParentB (
                ParentId int NOT NULL,
                CONSTRAINT PK_ParentB PRIMARY KEY (ParentId)
            );
            CREATE TABLE raw.Child (
                ChildId int NOT NULL,
                ParentId int NOT NULL,
                CONSTRAINT PK_Child PRIMARY KEY (ChildId)
            );
            ALTER TABLE raw.Child
                ADD CONSTRAINT FK_Child_Parent
                FOREIGN KEY (ParentId) REFERENCES raw.ParentA(ParentId);
            """);
    }

    private static void CreateForeignKeyReplaceRollbackFixture(string databaseConnectionString)
    {
        ExecuteSql(databaseConnectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.ParentA (
                ParentId int NOT NULL,
                CONSTRAINT PK_ParentA PRIMARY KEY (ParentId)
            );
            CREATE TABLE raw.ParentB (
                ParentId int NOT NULL,
                CONSTRAINT PK_ParentB PRIMARY KEY (ParentId)
            );
            CREATE TABLE raw.ChildA (
                ChildAId int NOT NULL,
                ParentId int NOT NULL,
                CONSTRAINT PK_ChildA PRIMARY KEY (ChildAId)
            );
            CREATE TABLE raw.ChildB (
                ChildBId int NOT NULL,
                ParentId int NOT NULL,
                CONSTRAINT PK_ChildB PRIMARY KEY (ChildBId)
            );
            INSERT INTO raw.ParentA(ParentId) VALUES (1), (2);
            INSERT INTO raw.ParentB(ParentId) VALUES (1);
            INSERT INTO raw.ChildA(ChildAId, ParentId) VALUES (1, 1);
            INSERT INTO raw.ChildB(ChildBId, ParentId) VALUES (1, 2);
            ALTER TABLE raw.ChildA
                ADD CONSTRAINT FK_ChildA_Parent
                FOREIGN KEY (ParentId) REFERENCES raw.ParentA(ParentId);
            ALTER TABLE raw.ChildB
                ADD CONSTRAINT FK_ChildB_Parent
                FOREIGN KEY (ParentId) REFERENCES raw.ParentA(ParentId);
            """);
    }

    private static void CreatePrimaryKeyReplaceFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.PkReplaceCase (
                KeyA int NOT NULL,
                KeyB int NOT NULL,
                Payload int NULL,
                CONSTRAINT PK_PkReplaceCase PRIMARY KEY NONCLUSTERED (KeyA)
            );
            INSERT INTO raw.PkReplaceCase(KeyA, KeyB, Payload)
            VALUES (1, 10, 100), (2, 20, 200), (3, 30, 300);
            """);
    }

    private static void CreateClusteredPrimaryKeyReplaceFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.PkClusteredCase (
                KeyA int NOT NULL,
                KeyB int NOT NULL,
                Payload int NULL,
                CONSTRAINT PK_PkClusteredCase PRIMARY KEY CLUSTERED (KeyA)
            );
            INSERT INTO raw.PkClusteredCase(KeyA, KeyB, Payload)
            VALUES (1, 10, 100), (2, 20, 200), (3, 30, 300);
            """);
    }

    private static void CreatePrimaryKeyReplaceWithDependentForeignKeyFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.ParentPkCase (
                KeyA int NOT NULL,
                KeyB int NOT NULL,
                Payload int NULL,
                CONSTRAINT PK_ParentPkCase PRIMARY KEY NONCLUSTERED (KeyA)
            );
            CREATE TABLE raw.ChildPkCase (
                ChildId int NOT NULL,
                ParentKeyA int NOT NULL,
                ParentKeyB int NOT NULL,
                CONSTRAINT PK_ChildPkCase PRIMARY KEY (ChildId)
            );
            ALTER TABLE raw.ChildPkCase
                ADD CONSTRAINT FK_ChildPkCase_ParentPkCase
                FOREIGN KEY (ParentKeyA) REFERENCES raw.ParentPkCase(KeyA);
            INSERT INTO raw.ParentPkCase(KeyA, KeyB, Payload)
            VALUES (1, 10, 100), (2, 20, 200), (3, 30, 300);
            INSERT INTO raw.ChildPkCase(ChildId, ParentKeyA, ParentKeyB)
            VALUES (1, 1, 10), (2, 2, 20), (3, 3, 30);
            """);
    }

    private static void CreatePrimaryKeyReplaceRollbackFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.PkRollbackA (
                KeyA int NOT NULL,
                KeyB int NOT NULL,
                Payload int NULL,
                CONSTRAINT PK_PkRollbackA PRIMARY KEY NONCLUSTERED (KeyA)
            );
            CREATE TABLE raw.PkRollbackB (
                KeyA int NOT NULL,
                KeyB int NOT NULL,
                Payload int NULL,
                CONSTRAINT PK_PkRollbackB PRIMARY KEY NONCLUSTERED (KeyA)
            );
            INSERT INTO raw.PkRollbackA(KeyA, KeyB, Payload)
            VALUES (1, 10, 100), (2, 20, 200), (3, 30, 300);
            INSERT INTO raw.PkRollbackB(KeyA, KeyB, Payload)
            VALUES (1, 7, 100), (2, 7, 200), (3, 8, 300);
            """);
    }

    private static void CreateIndexReplaceFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.IndexReplaceCase (
                Id int NOT NULL,
                Payload int NOT NULL,
                Note int NULL,
                CONSTRAINT PK_IndexReplaceCase PRIMARY KEY (Id)
            );
            INSERT INTO raw.IndexReplaceCase(Id, Payload, Note)
            VALUES (1, 10, 1), (2, 20, 2), (3, 30, 3);
            CREATE NONCLUSTERED INDEX IX_IndexReplaceCase_Payload
                ON raw.IndexReplaceCase (Payload ASC);
            """);
    }

    private static void CreateClusteredIndexReplaceFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.IndexClusteredCase (
                Id int NOT NULL,
                Payload int NOT NULL
            );
            INSERT INTO raw.IndexClusteredCase(Id, Payload)
            VALUES (1, 10), (2, 20), (3, 30);
            CREATE CLUSTERED INDEX IX_IndexClusteredCase_Payload
                ON raw.IndexClusteredCase (Payload ASC);
            """);
    }

    private static void CreateIndexReplaceRollbackFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.IndexRollbackCase (
                Id int NOT NULL,
                A int NOT NULL,
                B int NOT NULL,
                CONSTRAINT PK_IndexRollbackCase PRIMARY KEY (Id)
            );
            INSERT INTO raw.IndexRollbackCase(Id, A, B)
            VALUES (1, 1, 1), (2, 2, 1), (3, 3, 2);
            CREATE NONCLUSTERED INDEX IX_IndexRollback_A
                ON raw.IndexRollbackCase (A ASC);
            CREATE NONCLUSTERED INDEX IX_IndexRollback_B
                ON raw.IndexRollbackCase (B ASC);
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

    private static short GetColumnMaxLengthBytes(string connectionString, string schemaName, string tableName, string columnName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT c.max_length
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
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Column '{schemaName}.{tableName}.{columnName}' was not found.");
        }

        return Convert.ToInt16(value);
    }

    private static int GetValueLength(
        string connectionString,
        string schemaName,
        string tableName,
        string columnName,
        int id)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"""
            SELECT LEN([{columnName}])
            FROM [{schemaName}].[{tableName}]
            WHERE [Id] = @Id;
            """;
        command.Parameters.AddWithValue("@Id", id);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Row '{schemaName}.{tableName}.Id={id}' was not found.");
        }

        return Convert.ToInt32(value);
    }

    private static bool GetColumnNullable(string connectionString, string schemaName, string tableName, string columnName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT c.is_nullable
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
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Column '{schemaName}.{tableName}.{columnName}' was not found.");
        }

        return Convert.ToInt32(value) == 1;
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

    private static bool IndexExists(string connectionString, string indexName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(1)
            FROM sys.indexes
            WHERE name = @IndexName;
            """;
        command.Parameters.AddWithValue("@IndexName", indexName);
        var value = command.ExecuteScalar();
        return Convert.ToInt32(value) > 0;
    }

    private static string GetForeignKeyTargetTableName(string connectionString, string foreignKeyName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT t.name
            FROM sys.foreign_keys AS fk
            INNER JOIN sys.tables AS t ON t.object_id = fk.referenced_object_id
            WHERE fk.name = @ForeignKeyName;
            """;
        command.Parameters.AddWithValue("@ForeignKeyName", foreignKeyName);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Foreign key '{foreignKeyName}' was not found.");
        }

        return Convert.ToString(value)!;
    }

    private static List<string> GetPrimaryKeyKeyColumns(string connectionString, string schemaName, string tableName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT c.name
            FROM sys.key_constraints AS kc
            INNER JOIN sys.tables AS t ON t.object_id = kc.parent_object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            INNER JOIN sys.index_columns AS ic
                ON ic.object_id = kc.parent_object_id
               AND ic.index_id = kc.unique_index_id
            INNER JOIN sys.columns AS c
                ON c.object_id = ic.object_id
               AND c.column_id = ic.column_id
            WHERE kc.type = 'PK'
              AND s.name = @SchemaName
              AND t.name = @TableName
              AND ic.key_ordinal > 0
            ORDER BY ic.key_ordinal;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        using var reader = command.ExecuteReader();
        var result = new List<string>();
        while (reader.Read())
        {
            result.Add(reader.GetString(0));
        }

        if (result.Count == 0)
        {
            throw new InvalidOperationException($"Primary key for '{schemaName}.{tableName}' was not found.");
        }

        return result;
    }

    private static bool GetPrimaryKeyIsClustered(string connectionString, string schemaName, string tableName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT i.type_desc
            FROM sys.key_constraints AS kc
            INNER JOIN sys.tables AS t ON t.object_id = kc.parent_object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            INNER JOIN sys.indexes AS i
                ON i.object_id = kc.parent_object_id
               AND i.index_id = kc.unique_index_id
            WHERE kc.type = 'PK'
              AND s.name = @SchemaName
              AND t.name = @TableName;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Primary key for '{schemaName}.{tableName}' was not found.");
        }

        return string.Equals(Convert.ToString(value), "CLUSTERED", StringComparison.OrdinalIgnoreCase);
    }

    private static bool GetPrimaryKeyKeyIsDescending(string connectionString, string schemaName, string tableName, string columnName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT ic.is_descending_key
            FROM sys.key_constraints AS kc
            INNER JOIN sys.tables AS t ON t.object_id = kc.parent_object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            INNER JOIN sys.index_columns AS ic
                ON ic.object_id = kc.parent_object_id
               AND ic.index_id = kc.unique_index_id
            INNER JOIN sys.columns AS c
                ON c.object_id = ic.object_id
               AND c.column_id = ic.column_id
            WHERE kc.type = 'PK'
              AND s.name = @SchemaName
              AND t.name = @TableName
              AND c.name = @ColumnName
              AND ic.key_ordinal > 0;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        command.Parameters.AddWithValue("@ColumnName", columnName);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Primary key key column '{schemaName}.{tableName}.{columnName}' was not found.");
        }

        return Convert.ToBoolean(value);
    }

    private static bool GetIndexIsUnique(string connectionString, string indexName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT i.is_unique
            FROM sys.indexes AS i
            WHERE i.name = @IndexName;
            """;
        command.Parameters.AddWithValue("@IndexName", indexName);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Index '{indexName}' was not found.");
        }

        return Convert.ToBoolean(value);
    }

    private static bool GetIndexIsDescending(string connectionString, string indexName, string columnName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT ic.is_descending_key
            FROM sys.indexes AS i
            INNER JOIN sys.index_columns AS ic
                ON ic.object_id = i.object_id
               AND ic.index_id = i.index_id
            INNER JOIN sys.columns AS c
                ON c.object_id = i.object_id
               AND c.column_id = ic.column_id
            WHERE i.name = @IndexName
              AND c.name = @ColumnName
              AND ic.key_ordinal > 0;
            """;
        command.Parameters.AddWithValue("@IndexName", indexName);
        command.Parameters.AddWithValue("@ColumnName", columnName);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Index key '{indexName}.{columnName}' was not found.");
        }

        return Convert.ToBoolean(value);
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

