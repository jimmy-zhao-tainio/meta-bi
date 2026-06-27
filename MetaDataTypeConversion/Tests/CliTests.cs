using MetaBi.Tests.Common;

namespace MetaDataTypeConversion.Tests;

public sealed class CliTests
{
    [Fact]
    public void Help_ShowsNewWorkspaceCommand()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-data-type-conversion <command> [options]", result.Output);
        Assert.Contains("new-workspace", result.Output);
        Assert.DoesNotContain("--new-workspace", result.Output);
        Assert.DoesNotContain("init", result.Output);
        Assert.Contains("check", result.Output);
        Assert.Contains("resolve", result.Output);
    }

    [Fact]
    public void NewWorkspace_Help_ShowsRequiredOptions()
    {
        var result = RunCli("new-workspace --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("new-workspace <path>", result.Output);
        Assert.Contains("Arguments:", result.Output);
        Assert.Contains("Directory where the sanctioned workspace will be created.", result.Output);
    }

    [Fact]
    public void Resolve_Help_ShowsDescribedOptions()
    {
        var result = RunCli("resolve --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--workspace <path>", result.Output);
        Assert.Contains("--source-data-type <value>", result.Output);
        Assert.Contains("--target-data-type-system <value>", result.Output);
        Assert.Contains("Target data type system", result.Output);
    }

    [Fact]
    public void Init_Command_IsRejected()
    {
        var result = RunCli("init --new-workspace nowhere");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Unknown command 'init'.", result.Output);
    }

    [Fact]
    public void RootNewWorkspaceSwitch_IsRejected()
    {
        var result = RunCli("--new-workspace nowhere");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Option '--new-workspace' is not recognized.", result.Output);
    }

    [Fact]
    public void NewWorkspace_CreatesWorkspace()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaDataTypeConversion-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var result = RunCli($"new-workspace \"{workspacePath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("Ok", result.Output);
            Assert.True(File.Exists(Path.Combine(workspacePath, "workspace.xml")));
            Assert.True(File.Exists(Path.Combine(workspacePath, "model.xml")));
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    [Fact]
    public void Check_ValidatesSeededWorkspace()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaDataTypeConversion-check-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var create = RunCli($"new-workspace \"{workspacePath}\"");
            Assert.Equal(0, create.ExitCode);

            var check = RunCli($"check --workspace \"{workspacePath}\"");
            Assert.Equal(0, check.ExitCode);
            Assert.Contains("Ok", check.Output);
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    [Fact]
    public void Check_DefaultsWorkspaceToCurrentDirectory()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaDataTypeConversion-cwd-check-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var create = RunCli($"new-workspace \"{workspacePath}\"");
            Assert.Equal(0, create.ExitCode);

            var check = RunCli("check", workingDirectory: workspacePath);
            Assert.Equal(0, check.ExitCode);
            Assert.Contains("Ok", check.Output);
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    [Fact]
    public void Resolve_ReturnsTargetTypeAndImplementation()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaDataTypeConversion-resolve-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var create = RunCli($"new-workspace \"{workspacePath}\"");
            Assert.Equal(0, create.ExitCode);

            var resolve = RunCli($"resolve --workspace \"{workspacePath}\" --source-data-type sqlserver:type:nvarchar");
            Assert.Equal(0, resolve.ExitCode);
            Assert.Contains("Ok", resolve.Output);
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    [Fact]
    public void Resolve_DefaultsWorkspaceToCurrentDirectory()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaDataTypeConversion-cwd-resolve-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var create = RunCli($"new-workspace \"{workspacePath}\"");
            Assert.Equal(0, create.ExitCode);

            var resolve = RunCli("resolve --source-data-type sqlserver:type:nvarchar", workingDirectory: workspacePath);
            Assert.Equal(0, resolve.ExitCode);
            Assert.Contains("Ok", resolve.Output);
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    [Fact]
    public void Resolve_WithTargetDataTypeSystem_ReturnsSelectedTarget()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaDataTypeConversion-resolve-target-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var create = RunCli($"new-workspace \"{workspacePath}\"");
            Assert.Equal(0, create.ExitCode);

            var resolve = RunCli($"resolve --workspace \"{workspacePath}\" --source-data-type meta:type:String --target-data-type-system SqlServer");
            Assert.Equal(0, resolve.ExitCode);
            Assert.Contains("Ok", resolve.Output);
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    private static (int ExitCode, string Output) RunCli(string arguments, string? workingDirectory = null) =>
        CliTestRunner.RunStandardCli("MetaDataTypeConversion", "meta-data-type-conversion.exe", arguments, workingDirectory: workingDirectory);

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}


