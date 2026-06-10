using MetaBi.Tests.Common;

namespace MetaTabular.Tests;

public sealed class CliTests
{
    [Fact]
    public void Help_ShowsTargetSpecificCommands()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-tabular [--new-workspace <path> | <command> [options]]", result.Output);
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
        Assert.Contains("missing required option --server <server>.", result.Output);
    }

    [Fact]
    public void DeployHelp_DescribesObjectRealization()
    {
        var result = RunCli("deploy --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Creates tabular database objects", result.Output);
        Assert.Contains("calculation groups", result.Output);
        Assert.Contains("--drop-existing", result.Output);
        Assert.Contains("--no-process", result.Output);
        Assert.Contains("fails if processing fails", result.Output);
        Assert.Contains("drop, create, full-process", result.Output);
        Assert.DoesNotContain("--replace", result.Output);
    }

    [Fact]
    public void ProcessCommand_RequiresServer()
    {
        var result = RunCli("process --database-name Commerce");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("missing required option --server <server>.", result.Output);
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
        Assert.Contains("Processes an existing Analysis Services tabular database", result.Output);
        Assert.Contains("--refresh-type", result.Output);
        Assert.Contains("--table", result.Output);
        Assert.Contains("--partition", result.Output);
        Assert.Contains("deploy --no-process", result.Output);
    }

    [Fact]
    public void DropCommand_RequiresServer()
    {
        var result = RunCli("drop --database-name Commerce");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("missing required option --server <server>.", result.Output);
    }

    [Fact]
    public void DropHelp_DescribesDirectDrop()
    {
        var result = RunCli("drop --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Drops a tabular database", result.Output);
        Assert.Contains("--server", result.Output);
        Assert.Contains("--database-name", result.Output);
        Assert.Contains("no confirmation prompt", result.Output);
    }

    [Fact]
    public void RestoreCommand_RequiresSourceServer()
    {
        var result = RunCli("restore --target-server localhost\\TABULAR --target-database-name Prod --backup-file C:\\Temp\\prod.abf");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("missing required option --source-server <server>.", result.Output);
    }

    [Fact]
    public void RestoreHelp_DescribesBackupRestorePromotion()
    {
        var result = RunCli("restore --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("pre-prod-to-prod promotion", result.Output);
        Assert.Contains("--backup-file", result.Output);
        Assert.Contains("Restore does not process", result.Output);
        Assert.DoesNotContain("--replace", result.Output);
    }

    [Fact]
    public void NewWorkspaceAndAuthoringCommands_CreateTabularWorkspace()
    {
        var path = CreateTempPath();
        try
        {
            Assert.Equal(0, RunCli($"--new-workspace \"{path}\"").ExitCode);
            Assert.Equal(0, RunCli($"add-tabular-model --workspace \"{path}\" --id Commerce --name Commerce --compatibility-level 1500").ExitCode);
            Assert.Equal(0, RunCli($"add-tabular-table --workspace \"{path}\" --id Sales --tabular-model Commerce --name Sales").ExitCode);
            Assert.Equal(0, RunCli($"add-tabular-column --workspace \"{path}\" --id SalesAmountColumn --tabular-table Sales --name SalesAmount --data-type-id meta:type:Decimal").ExitCode);
            Assert.Equal(0, RunCli($"add-tabular-measure --workspace \"{path}\" --id SalesAmount --tabular-table Sales --name \"Sales Amount\" --expression \"SUM(Sales[SalesAmount])\"").ExitCode);
            Assert.Equal(0, RunCli($"add-tabular-calculation-group --workspace \"{path}\" --id TimeIntelligence --tabular-model Commerce --name TimeIntelligence --precedence 10").ExitCode);
            Assert.Equal(0, RunCli($"add-tabular-calculation-item --workspace \"{path}\" --id TimeYtd --tabular-calculation-group TimeIntelligence --name YTD --expression \"SELECTEDMEASURE()\"").ExitCode);

            var model = MetaTabularModel.LoadFromXmlWorkspace(path, searchUpward: false);
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

    private static (int ExitCode, string Output) RunCli(string arguments) =>
        CliTestRunner.RunStandardCli("MetaTabular", "meta-tabular.exe", arguments);

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
