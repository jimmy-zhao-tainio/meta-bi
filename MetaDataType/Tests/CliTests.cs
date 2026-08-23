using MetaBi.Tests.Common;

namespace MetaDataType.Tests;

public sealed class CliTests
{
    [Fact]
    public void Help_ShowsCreateCommand()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-data-type <command> [options]", result.Output);
        Assert.Contains("create", result.Output);
    }

    [Fact]
    public void Create_Help_ShowsOutputOptions()
    {
        var result = RunCli("create --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-data-type create", result.Output);
        Assert.Contains("--xml <path>", result.Output);
        Assert.Contains("--csharp <path>", result.Output);
        Assert.Contains("--sql <path>", result.Output);
    }

    [Fact]
    public void Create_FailsWhenOutputIsMissing()
    {
        var result = RunCli("create");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Parameter group 'output' requires one of:", result.Output);
    }

    [Fact]
    public void Create_CreatesWorkspace()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaDataType-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var result = RunCli($"create --xml \"{workspacePath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("MetaDataType workspace created:", result.Output);
            Assert.True(File.Exists(Path.Combine(workspacePath, "workspace.meta")));
            Assert.True(File.Exists(Path.Combine(workspacePath, "model.xml")));
            
            var typeXml = File.ReadAllText(Path.Combine(workspacePath, "instances", "DataType.xml"));
            Assert.Contains("sqlserver:type:nvarchar", typeXml);
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    private static (int ExitCode, string Output) RunCli(string arguments) =>
        CliTestRunner.RunStandardCli("meta-data-type", arguments);

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}


