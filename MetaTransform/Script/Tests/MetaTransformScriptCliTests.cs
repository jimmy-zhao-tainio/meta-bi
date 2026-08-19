using MetaBi.Tests.Common;
using Meta.Integration;
using MSS = MetaSqlScript;
using MTS = MetaTransformScript;

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
        Assert.Contains("sql-script-workspace", result.Output);
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

    [Fact]
    public void FromSqlScriptWorkspace_ImportsEveryModeledScript()
    {
        var root = CreateTempRoot();
        try
        {
            var sourcePath = Path.Combine(root, "SqlScripts");
            var outputPath = Path.Combine(root, "Transforms");
            var source = MSS.MetaSqlScriptModel.CreateEmpty();
            source.SqlScriptList.Add(new MSS.SqlScript
            {
                Id = "load-customer",
                Name = "LoadCustomer",
                SqlText = "INSERT INTO dbo.Customer (CustomerId) SELECT CustomerId FROM stage.Customer;",
            });
            source.SqlScriptList.Add(new MSS.SqlScript
            {
                Id = "clear-customer-stage",
                Name = "ClearCustomerStage",
                SqlText = "DELETE FROM stage.Customer WHERE IsReady = 0;",
            });
            TypedWorkspaceModelMapper.Create(source, sourcePath, "xml");

            var result = RunCli(
                $"from sql-script-workspace --source-workspace \"{sourcePath}\" --output-xml \"{outputPath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("Scripts: 2", result.Output);
            var transformed = TypedWorkspaceModelMapper.Load<MTS.MetaTransformScriptModel>(
                outputPath,
                searchUpward: false);
            Assert.Equal(2, transformed.TransformScriptList.Count);
            Assert.Contains(transformed.TransformScriptList, script => script.Name == "LoadCustomer");
            Assert.Contains(transformed.TransformScriptList, script => script.Name == "ClearCustomerStage");
            Assert.Single(transformed.InsertStatementList);
            Assert.Single(transformed.DeleteStatementList);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    private static (int ExitCode, string Output) RunCli(string arguments, string? workingDirectory = null) =>
        CliTestRunner.RunStandardCli(
            "meta-transform-script",
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
