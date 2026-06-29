using Meta.Core.Services;

namespace MetaDataVault.Tests;

public sealed partial class CliTests
{
    [Fact]
    public async Task NewWorkspace_CreatesMetaBusinessDataVaultWorkspace()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");

        try
        {
            var result = RunBusinessCli($"new-workspace \"{workspacePath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("MetaBusinessDataVault workspace created", result.Output, StringComparison.Ordinal);

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            Assert.Equal("MetaBusinessDataVault", workspace.Model.Name);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task NewWorkspace_CreatesMetaRawDataVaultWorkspace()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");

        try
        {
            var result = RunRawCli($"new-workspace \"{workspacePath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("MetaRawDataVault workspace created", result.Output, StringComparison.Ordinal);

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            Assert.Equal("MetaRawDataVault", workspace.Model.Name);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public void Help_DoesNotShowFromMetaSchemaCommand()
    {
        var result = RunRawCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-datavault-raw", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("from-metaschema", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("check-business-materialization", result.Output);
        Assert.DoesNotContain("generate-metasql", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("raw-datavault-to-sql", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MetaConvert_Help_ShowsSchemaToRawDataVaultCommand()
    {
        var result = RunMetaConvertCli("schema-to-raw-datavault --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("schema-to-raw-datavault", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--source-workspace <path>", result.Output);
        Assert.Contains("--new-workspace <path>", result.Output);
        Assert.Contains("[--ignore-field-name <value>]", result.Output);
        Assert.Contains("[--ignore-field-suffix <value>]", result.Output);
        Assert.Contains("[--include-views]", result.Output);
        Assert.Contains("[--verbose]", result.Output);
    }

    [Fact]
    public void BusinessDataVaultToSql_Help_ShowsRequiredOptions()
    {
        var result = RunMetaConvertCli("business-datavault-to-sql --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("[--workspace <path>]", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--database-name <value>", result.Output);
        Assert.Contains("--out <path>", result.Output);
        Assert.Contains("Source workspace path. Defaults to the current directory.", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BusinessDataVaultToSql_RequiresDatabaseName()
    {
        var result = RunMetaConvertCli("business-datavault-to-sql --workspace C:\\temp\\bdv --implementation-workspace C:\\temp\\impl --out C:\\temp\\sql");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Required parameter 'database-name' was not provided.", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RawDataVaultToSql_Help_ShowsRequiredOptions()
    {
        var result = RunMetaConvertCli("raw-datavault-to-sql --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("[--workspace <path>]", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--database-name <value>", result.Output);
        Assert.Contains("--out <path>", result.Output);
        Assert.Contains("Source workspace path. Defaults to the current directory.", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RawDataVaultToSql_RequiresDatabaseName()
    {
        var result = RunMetaConvertCli("raw-datavault-to-sql --workspace C:\\temp\\rdv --implementation-workspace C:\\temp\\impl --out C:\\temp\\sql");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Required parameter 'database-name' was not provided.", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BusinessAddCommandHelp_ShowsOrdinalAsOptional()
    {
        var result = RunBusinessCli("add-link-hub --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("[--ordinal <value>]", result.Output);
    }

    [Fact]
    public void RawAddCommandHelp_ShowsOrdinalAsOptional()
    {
        var result = RunRawCli("add-link-hub --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("[--ordinal <value>]", result.Output);
    }
}
