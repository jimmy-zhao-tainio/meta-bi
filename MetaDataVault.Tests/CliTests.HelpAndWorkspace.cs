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
            var result = RunBusinessCli($"--new-workspace \"{workspacePath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("OK: metabusinessdatavault workspace created", result.Output, StringComparison.OrdinalIgnoreCase);

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
            var result = RunRawCli($"--new-workspace \"{workspacePath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("OK: metarawdatavault workspace created", result.Output, StringComparison.OrdinalIgnoreCase);

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            Assert.Equal("MetaRawDataVault", workspace.Model.Name);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public void Help_ShowsFromMetaSchemaCommand()
    {
        var result = RunRawCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-datavault-raw", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("from-metaschema", result.Output);
        Assert.DoesNotContain("check-business-materialization", result.Output);
        Assert.Contains("generate-metasql", result.Output);
        Assert.DoesNotContain("generate-sql", result.Output);
    }

    [Fact]
    public void FromMetaSchema_Help_ShowsRequiredOptions()
    {
        var result = RunRawCli("from-metaschema --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--source-workspace <path>", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--new-workspace <path>", result.Output);
        Assert.Contains("[--business-workspace <path>]", result.Output);
        Assert.Contains("[--ignore-field-name <name>]", result.Output);
        Assert.Contains("[--ignore-field-suffix <suffix>]", result.Output);
        Assert.Contains("[--include-views]", result.Output);
        Assert.Contains("[--verbose]", result.Output);
        Assert.Contains("schema-bootstrap", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("primary or unique keys", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("metadatavaultimplementation is required", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("views are excluded by default", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("print table and relationship materialization decisions to the console", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("schema-driven and agnostic to source field names", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CheckBusinessMaterialization_Help_ShowsRequiredOptions()
    {
        var result = RunBusinessCli("check-business-materialization --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--business-workspace <path>", result.Output);
        Assert.Contains("--bdv-workspace", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--weave-workspace <path>", result.Output);
        Assert.Contains("--fabric-workspace <path>", result.Output);
        Assert.Contains("MetaBusinessDataVault", result.Output);
        Assert.Contains("MetaFabric", result.Output);
    }

    [Fact]
    public void MaterializeBusiness_Help_ShowsRequiredOptions()
    {
        var result = RunBusinessCli("materialize-business --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--business-workspace <path>", result.Output);
        Assert.Contains("--bdv-workspace", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--weave-workspace <path>", result.Output);
        Assert.Contains("--fabric-workspace <path>", result.Output);
        Assert.Contains("--new-workspace <path>", result.Output);
        Assert.Contains("table name patterns", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BusinessGenerateMetaSql_Help_ShowsRequiredOptions()
    {
        var result = RunBusinessCli("generate-metasql --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--workspace <path>", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--data-type-conversion-workspace <path>", result.Output);
        Assert.Contains("--database-name <name>", result.Output);
        Assert.Contains("--out <path>", result.Output);
        Assert.Contains("Stub only", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("not currently implemented", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BusinessGenerateMetaSql_ReturnsStubFailure()
    {
        var result = RunBusinessCli("generate-metasql --workspace C:\\temp\\bdv --implementation-workspace C:\\temp\\impl --data-type-conversion-workspace C:\\temp\\conv --database-name Demo --out C:\\temp\\sql");

        Assert.Equal(4, result.ExitCode);
        Assert.Contains("generate-metasql is currently a stub", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("No SqlModel workspace is generated", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GenerateMetaSql_Help_ShowsRequiredOptions()
    {
        var result = RunRawCli("generate-metasql --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--workspace <path>", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--data-type-conversion-workspace <path>", result.Output);
        Assert.Contains("--database-name <name>", result.Output);
        Assert.Contains("--out <path>", result.Output);
        Assert.Contains("Stub only", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("not currently implemented", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GenerateMetaSql_ReturnsStubFailure()
    {
        var result = RunRawCli("generate-metasql --workspace C:\\temp\\rdv --implementation-workspace C:\\temp\\impl --data-type-conversion-workspace C:\\temp\\conv --database-name Demo --out C:\\temp\\sql");

        Assert.Equal(4, result.ExitCode);
        Assert.Contains("generate-metasql is currently a stub", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("No SqlModel workspace is generated", result.Output, StringComparison.OrdinalIgnoreCase);
    }
}
