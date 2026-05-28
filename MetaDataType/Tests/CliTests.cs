using MetaBi.Tests.Common;

namespace MetaDataType.Tests;

public sealed class CliTests
{
    [Fact]
    public void Help_ShowsNewWorkspaceSwitch()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-data-type [--new-workspace <path> | <command> [options]]", result.Output);
        Assert.Contains("--new-workspace", result.Output);
        Assert.DoesNotContain("init", result.Output);
    }

    [Fact]
    public void NewWorkspace_Help_ShowsRequiredOptions()
    {
        var result = RunCli("--new-workspace --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Options:", result.Output);
        Assert.Contains("--new-workspace <path>", result.Output);
    }

    [Fact]
    public void NewWorkspace_FailsWhenPathMissing()
    {
        var result = RunCli("--new-workspace");

        Assert.Equal(1, result.ExitCode);
        Assert.Contains("Cannot continue", result.Output);
        Assert.Contains("missing value for --new-workspace", result.Output);
    }

    [Fact]
    public void NewWorkspace_FailsWhenUnknownOptionProvided()
    {
        var result = RunCli("--bad nope");

        Assert.Equal(1, result.ExitCode);
        Assert.Contains("Cannot continue", result.Output);
        Assert.Contains("unknown option '--bad'", result.Output);
    }

    [Fact]
    public void Init_Command_IsRejected()
    {
        var result = RunCli("init --new-workspace nowhere");

        Assert.Equal(1, result.ExitCode);
        Assert.Contains("Cannot continue", result.Output);
        Assert.Contains("unknown command 'init'", result.Output);
    }

    [Fact]
    public void NewWorkspace_CreatesWorkspace()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaDataType-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var result = RunCli($"--new-workspace \"{workspacePath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("Ok", result.Output);
            Assert.True(File.Exists(Path.Combine(workspacePath, "workspace.xml")));
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
        CliTestRunner.RunStandardCli("MetaDataType", "meta-data-type.exe", arguments);

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}


