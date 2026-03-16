using MetaDataVault.ConvertToMetaSql;

namespace MetaDataVault.Tests;

public sealed class ConvertToMetaSqlTests
{
    [Fact]
    public async Task ConvertAsync_LoadsRawWorkspaceAndCreatesEmptySqlWorkspaceInMemory()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var createResult = RunRawCli($"--new-workspace \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            var sqlWorkspace = await MetaDataVaultToMetaSqlConverter.ConvertAsync(workspacePath, targetPath);

            Assert.Equal(targetPath, sqlWorkspace.WorkspaceRootPath);
            Assert.Equal("SqlModel", sqlWorkspace.Model.Name);
            Assert.Empty(sqlWorkspace.Instance.GetOrCreateEntityRecords("Database"));
            Assert.Empty(sqlWorkspace.Instance.GetOrCreateEntityRecords("Table"));
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    private static (int ExitCode, string Output) RunRawCli(string arguments)
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var cliPath = CliTestSupport.ResolveCliPath(repoRoot, "MetaDataVault.Raw.Cli", "meta-datavault-raw.dll");
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{cliPath}\" {arguments}",
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        return CliTestSupport.RunProcess(startInfo, "Could not start DataVault CLI process.");
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
