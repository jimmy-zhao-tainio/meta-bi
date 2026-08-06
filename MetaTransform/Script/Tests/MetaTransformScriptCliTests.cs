using MetaBi.Tests.Common;

namespace MetaTransformScript.Tests;

public sealed class MetaTransformScriptCliTests
{
    [Fact]
    public void Help_ShowsModeledCommandGroups()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-transform-script <command> [options]", result.Output);
        Assert.Contains("from", result.Output);
        Assert.Contains("stored-procedure", result.Output);
        Assert.Contains("target-identifiers", result.Output);
        Assert.Contains("to", result.Output);
    }

    [Fact]
    public void FromHelp_ShowsModeledChildCommands()
    {
        var result = RunCli("from --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-transform-script from <command> [options]", result.Output);
        Assert.Contains("sql-file", result.Output);
        Assert.Contains("sql-files", result.Output);
        Assert.Contains("sql-code", result.Output);
    }

    [Fact]
    public void FromSqlFileHelp_ShowsModeledOptions()
    {
        var result = RunCli("from sql-file --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--path <file.sql>", result.Output);
        Assert.Contains("--target <sql-identifier>", result.Output);
        Assert.Contains("--output-xml <path>", result.Output);
        Assert.Contains("--workspace <path>", result.Output);
    }

    [Fact]
    public void StoredProcedureAddContractHelp_ShowsModeledRepeatableOptions()
    {
        var result = RunCli("stored-procedure add-contract --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--workspace <path>", result.Output);
        Assert.Contains("--name <value>", result.Output);
        Assert.Contains("--operation <operation>", result.Output);
        Assert.Contains("--result-rowset <value>", result.Output);
        Assert.Contains("--result-column <rowset>=<column>", result.Output);
    }

    [Fact]
    public void FromSqlCode_CreatesWorkspace_AndToSqlCodeDefaultsWorkspaceToCurrentDirectory()
    {
        var root = CreateTempRoot();
        try
        {
            var workspacePath = Path.Combine(root, "TransformWS");
            var create = RunCli(
                $"from sql-code --code \"CREATE VIEW dbo.v_test AS SELECT 1 AS A\" --output-xml \"{workspacePath}\"");

            Assert.Equal(0, create.ExitCode);
            Assert.True(File.Exists(Path.Combine(workspacePath, "workspace.meta")));

            var export = RunCli("to sql-code", workingDirectory: workspacePath);

            Assert.Equal(0, export.ExitCode);
            Assert.Contains("SELECT", export.Output);
            Assert.Contains("1", export.Output);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    private static (int ExitCode, string Output) RunCli(string arguments, string? workingDirectory = null) =>
        CliTestRunner.RunStandardCli(
            Path.Combine("MetaTransform", "Script"),
            "meta-transform-script.exe",
            arguments,
            workingDirectory: workingDirectory);

    private static string CreateTempRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "MetaTransform.Script.Cli.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    private static void DeleteTempRoot(string root)
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
