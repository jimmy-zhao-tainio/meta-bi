using MetaSql;

namespace MetaDataVault.Tests;

public sealed partial class CliTests
{
    [Fact]
    public async Task RawGenerateMetaSql_GeneratesCurrentMetaSqlWorkspace()
    {
        var repoRoot = FindRepositoryRoot();
        var sourceWorkspacePath = Path.Combine(repoRoot, "Samples", "Demos", "RawDataVaultCliIntegration", "Workspace");
        var implementationWorkspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "MetaDataVaultImplementation");
        var tempRoot = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var outputWorkspacePath = Path.Combine(tempRoot, "current-metasql");

        try
        {
            var result = RunRawCli(
                $"generate-metasql --workspace \"{sourceWorkspacePath}\" --implementation-workspace \"{implementationWorkspacePath}\" --database-name \"RawGenerateMetaSqlTest\" --out \"{outputWorkspacePath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("raw metasql generated", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.True(Directory.Exists(outputWorkspacePath));

            var model = await MetaSqlModel.LoadFromXmlWorkspaceAsync(outputWorkspacePath, searchUpward: false);
            Assert.NotEmpty(model.DatabaseList);
            Assert.NotEmpty(model.TableList);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task BusinessGenerateMetaSql_GeneratesCurrentMetaSqlWorkspace()
    {
        var repoRoot = FindRepositoryRoot();
        var sourceWorkspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultCommerceHelpers");
        var implementationWorkspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "MetaDataVaultImplementation");
        var tempRoot = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var outputWorkspacePath = Path.Combine(tempRoot, "current-metasql");

        try
        {
            var result = RunBusinessCli(
                $"generate-metasql --workspace \"{sourceWorkspacePath}\" --implementation-workspace \"{implementationWorkspacePath}\" --database-name \"BusinessGenerateMetaSqlTest\" --out \"{outputWorkspacePath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("business metasql generated", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.True(Directory.Exists(outputWorkspacePath));

            var model = await MetaSqlModel.LoadFromXmlWorkspaceAsync(outputWorkspacePath, searchUpward: false);
            Assert.NotEmpty(model.DatabaseList);
            Assert.Contains(model.TableList, row => string.Equals(row.Name, "RSAT_Status_Current", StringComparison.Ordinal));
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }
}
