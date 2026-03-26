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
    public async Task DeployPlanCommand_EmitsReplaceForeignKeyForExecutableSharedForeignKeyDifference()
    {
        var repoRoot = FindRepositoryRoot();
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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
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
                FileName = "meta-sql",
                Arguments = $"deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\"",
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
            Assert.Contains("ReplaceCount: 1", planResult.Output, StringComparison.Ordinal);

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
            Assert.Contains("ReplaceCount: 2", planResult.Output, StringComparison.Ordinal);

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
            Assert.Contains("ReplaceCount: 2", planResult.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.ReplacePrimaryKeyList);
            Assert.Single(manifest.ReplaceForeignKeyList);

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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
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
                FileName = "meta-sql",
                Arguments = $"deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\"",
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
            Assert.Contains("ReplaceCount: 1", planResult.Output, StringComparison.Ordinal);

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
            Assert.Contains("ReplaceCount: 2", planResult.Output, StringComparison.Ordinal);

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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
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
                FileName = "meta-sql",
                Arguments = $"deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\"",
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
            Assert.Contains("ReplaceCount: 1", planResult.Output, StringComparison.Ordinal);

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
            Assert.Contains("ReplaceCount: 2", planResult.Output, StringComparison.Ordinal);

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
            Assert.Contains("Verdict: deployable", planResult.Output, StringComparison.Ordinal);

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
            Assert.Contains("deploy complete", deployResult.Output, StringComparison.Ordinal);

            Assert.True(ColumnExists(databaseConnectionString, "raw", "H_Customer", "CustomerName"));

            var verifyCommand = new ProcessStartInfo
            {
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{verifyPath}\"",
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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
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
                FileName = "meta-sql",
                Arguments = $"deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\"",
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
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);

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
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);

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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --approve-truncate-column raw.VarcharCase.ValueCol --out \"{planPath}\"",
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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
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
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);

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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
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
            Assert.Contains("supports only MetaDataTypeId values owned by DataTypeSystem 'SqlServer'", block.DifferenceSummary, StringComparison.Ordinal);
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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
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
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);
            Assert.Contains("ReplaceCount: 1", planResult.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.AlterTableColumnList);
            Assert.Single(manifest.ReplacePrimaryKeyList);
            Assert.Empty(manifest.BlockTableColumnDifferenceList);
            Assert.Empty(manifest.BlockPrimaryKeyDifferenceList);

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
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);
            Assert.Contains("ReplaceCount: 1", planResult.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.AlterTableColumnList);
            Assert.Single(manifest.ReplaceForeignKeyList);
            Assert.Empty(manifest.BlockTableColumnDifferenceList);

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
            Assert.Contains("AlterCount: 1", planResult.Output, StringComparison.Ordinal);
            Assert.Contains("ReplaceCount: 1", planResult.Output, StringComparison.Ordinal);

            var manifest = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(planPath, searchUpward: false);
            Assert.Single(manifest.AlterTableColumnList);
            Assert.Single(manifest.ReplaceIndexList);
            Assert.Empty(manifest.BlockTableColumnDifferenceList);

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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
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
            Assert.Contains("AlterCount: 2", planResult.Output, StringComparison.Ordinal);

            var longRecordSource = new string('R', 150);
            ExecuteSql(databaseConnectionString, $"""
                INSERT INTO raw.H_Customer(HashKey, CustomerId, LoadTimestamp, RecordSource, AuditId)
                VALUES (CONVERT(binary(16), 0x00000000000000000000000000000002), N'CUST-2', SYSUTCDATETIME(), N'{longRecordSource}', 2);
                """);

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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePlannedPath}\" --connection-string \"{databaseConnectionString}\" --out \"{planPath}\"",
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
                FileName = "meta-sql",
                Arguments = $"deploy --manifest-workspace \"{planPath}\" --source-workspace \"{sourceDifferentPath}\" --connection-string \"{databaseConnectionString}\"",
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

            ExecuteSql(databaseConnectionString, "ALTER TABLE raw.H_Customer ADD LiveOnlyAfterPlan int NULL;");

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

            AddUnsupportedActionKindToManifestModel(planPath, "AddUnsupportedThing");

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
                FileName = "meta-sql",
                Arguments = $"deploy-plan --source-workspace \"{sourcePath}\" --connection-string \"{databaseConnectionString}\" --approve-drop-table raw.Parent --out \"{planPath}\"",
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

}
