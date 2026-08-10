using MetaBi.Tests.Common;

namespace MetaTabular.Tests;

public sealed class CliTests
{
    [Fact]
    public void Help_ShowsTargetSpecificCommands()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-tabular <command> [options]", result.Output);
        Assert.Contains("create", result.Output);
        Assert.Contains("deploy", result.Output);
        Assert.Contains("process", result.Output);
        Assert.Contains("restore", result.Output);
        Assert.Contains("drop", result.Output);
        Assert.Contains("add-tabular-calculation-group", result.Output);
        Assert.Contains("add-tabular-partition", result.Output);
    }

    [Fact]
    public void DeployCommand_RequiresServer()
    {
        var result = RunCli("deploy --workspace .");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("Required parameter 'server' was not provided.", result.Output);
    }

    [Fact]
    public void DeployHelp_DescribesObjectRealization()
    {
        var result = RunCli("deploy --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Create modeled tabular database objects", result.Output);
        Assert.Contains("calculation groups", result.Output);
        Assert.Contains("--drop-existing", result.Output);
        Assert.Contains("--no-process", result.Output);
        Assert.Contains("processing failures fail the command", result.Output);
        Assert.DoesNotContain("--replace", result.Output);
    }

    [Fact]
    public void ProcessCommand_RequiresServer()
    {
        var result = RunCli("process --database-name Commerce");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("Required parameter 'server' was not provided.", result.Output);
    }

    [Fact]
    public void ProcessCommand_RequiresTableForPartition()
    {
        var result = RunCli("process --server localhost\\TABULAR --database-name Commerce --partition Current");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("--partition requires --table <name>.", result.Output);
    }

    [Fact]
    public void ProcessHelp_DescribesDatabaseTableAndPartitionProcessing()
    {
        var result = RunCli("process --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Process an existing tabular database", result.Output);
        Assert.Contains("--refresh-type", result.Output);
        Assert.Contains("--table", result.Output);
        Assert.Contains("--partition", result.Output);
    }

    [Fact]
    public void DropCommand_RequiresServer()
    {
        var result = RunCli("drop --database-name Commerce");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("Required parameter 'server' was not provided.", result.Output);
    }

    [Fact]
    public void DropHelp_DescribesDirectDrop()
    {
        var result = RunCli("drop --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Drop a tabular database", result.Output);
        Assert.Contains("--server", result.Output);
        Assert.Contains("--database-name", result.Output);
        Assert.Contains("no confirmation prompt", result.Output);
    }

    [Fact]
    public void RestoreCommand_RequiresSourceServer()
    {
        var result = RunCli("restore --source-database-name Source --target-server localhost\\TABULAR --target-database-name Prod --backup-file C:\\Temp\\prod.abf");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("Required parameter 'source-server' was not provided.", result.Output);
    }

    [Fact]
    public void RestoreHelp_DescribesBackupRestorePromotion()
    {
        var result = RunCli("restore --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("pre-prod-to-prod promotion", result.Output);
        Assert.Contains("--backup-file", result.Output);
        Assert.Contains("restore does not process", result.Output);
        Assert.DoesNotContain("--replace", result.Output);
    }

    [Fact]
    public void NewWorkspaceAndAuthoringCommands_CreateTabularWorkspace()
    {
        var path = CreateTempPath();
        try
        {
            Assert.Equal(0, RunCli($"create --xml \"{path}\"").ExitCode);
            Assert.Equal(0, RunCli($"add-tabular-model --workspace \"{path}\" --id Commerce --name Commerce --compatibility-level 1500").ExitCode);
            Assert.Equal(0, RunCli($"add-tabular-table --workspace \"{path}\" --id Sales --tabular-model Commerce --name Sales").ExitCode);
            Assert.Equal(0, RunCli($"add-tabular-column --workspace \"{path}\" --id SalesAmountColumn --tabular-table Sales --name SalesAmount --data-type-id meta:type:Decimal").ExitCode);
            Assert.Equal(0, RunCli($"add-tabular-measure --workspace \"{path}\" --id SalesAmount --tabular-table Sales --name \"Sales Amount\" --expression \"SUM(Sales[SalesAmount])\"").ExitCode);
            Assert.Equal(0, RunCli($"add-tabular-calculation-group --workspace \"{path}\" --id TimeIntelligence --tabular-model Commerce --name TimeIntelligence --precedence 10").ExitCode);
            Assert.Equal(0, RunCli($"add-tabular-calculation-item --workspace \"{path}\" --id TimeYtd --tabular-calculation-group TimeIntelligence --name YTD --expression \"SELECTEDMEASURE()\"").ExitCode);

            var model = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaTabularModel>(path, searchUpward: false);
            Assert.Single(model.TabularModelList);
            Assert.Single(model.TabularCalculationGroupList);
            Assert.Single(model.TabularCalculationItemList);
            Assert.Same(model.TabularTableList.Single(), model.TabularMeasureList.Single().TabularTable);
        }
        finally
        {
            DeleteDirectoryIfExists(path);
        }
    }

    [Fact]
    public void AuthoringCommandWithoutWorkspace_UsesCurrentDirectoryWorkspace()
    {
        var path = CreateTempPath();
        try
        {
            Assert.Equal(0, RunCli($"create --xml \"{path}\"").ExitCode);

            var add = RunCli(
                "--id Commerce --name Commerce --compatibility-level 1500",
                command: "add-tabular-model",
                workingDirectory: path);

            Assert.Equal(0, add.ExitCode);
            var model = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaTabularModel>(path, searchUpward: false);
            var tabularModel = Assert.Single(model.TabularModelList);
            Assert.Equal("Commerce", tabularModel.Name);
        }
        finally
        {
            DeleteDirectoryIfExists(path);
        }
    }

    private static (int ExitCode, string Output) RunCli(
        string arguments,
        string? command = null,
        string? workingDirectory = null)
    {
        var cliArguments = string.IsNullOrWhiteSpace(command)
            ? arguments
            : $"{command} {arguments}";
        return CliTestRunner.RunStandardCli(
            "meta-tabular",
            cliArguments,
            workingDirectory: workingDirectory);
    }

    private static string CreateTempPath()
    {
        return Path.Combine(Path.GetTempPath(), "metatabular-cli-tests", Guid.NewGuid().ToString("N"));
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
