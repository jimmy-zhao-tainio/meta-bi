using MetaBi.Tests.Common;

namespace MetaMultiDimensional.Tests;

public sealed class CliTests
{
    [Fact]
    public void Help_ShowsTargetSpecificCommands()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-multi-dimensional <command> [options]", result.Output);
        Assert.Contains("new-workspace", result.Output);
        Assert.Contains("deploy", result.Output);
        Assert.Contains("restore", result.Output);
        Assert.Contains("drop", result.Output);
        Assert.Contains("add-cube", result.Output);
        Assert.Contains("add-measure-group", result.Output);
        Assert.Contains("add-cell-permission", result.Output);
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
        Assert.Contains("Create modeled multidimensional database objects", result.Output);
        Assert.Contains("data source views", result.Output);
        Assert.Contains("--drop-existing", result.Output);
        Assert.Contains("--no-process", result.Output);
        Assert.Contains("processing failures fail the command", result.Output);
        Assert.DoesNotContain("--replace", result.Output);
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
        Assert.Contains("Drop a multidimensional database", result.Output);
        Assert.Contains("--server", result.Output);
        Assert.Contains("--database-name", result.Output);
        Assert.Contains("no confirmation prompt", result.Output);
    }

    [Fact]
    public void RestoreCommand_RequiresSourceServer()
    {
        var result = RunCli("restore --target-server localhost\\MULTI --target-database-name Prod --backup-file C:\\Temp\\prod.abf");

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
    public void NewWorkspaceAndAuthoringCommands_CreateMultidimensionalWorkspace()
    {
        var path = CreateTempPath();
        try
        {
            Assert.Equal(0, RunCli($"new-workspace \"{path}\"").ExitCode);
            Assert.Equal(0, RunCli($"add-multi-dimensional-database --workspace \"{path}\" --id CommerceDb --name Commerce").ExitCode);
            Assert.Equal(0, RunCli($"add-cube --workspace \"{path}\" --id CommerceCube --multi-dimensional-database CommerceDb --name Commerce").ExitCode);
            Assert.Equal(0, RunCli($"add-dimension --workspace \"{path}\" --id Date --multi-dimensional-database CommerceDb --name Date").ExitCode);
            Assert.Equal(0, RunCli($"add-dimension-attribute --workspace \"{path}\" --id DateKey --dimension Date --name DateKey --data-type-id meta:type:Int32 --is-key true").ExitCode);
            Assert.Equal(0, RunCli($"add-cube-dimension --workspace \"{path}\" --id DateCubeDimension --cube CommerceCube --dimension Date --name Date").ExitCode);
            Assert.Equal(0, RunCli($"add-measure-group --workspace \"{path}\" --id SalesMeasureGroup --cube CommerceCube --name Sales").ExitCode);
            Assert.Equal(0, RunCli($"add-measure --workspace \"{path}\" --id SalesAmount --measure-group SalesMeasureGroup --name \"Sales Amount\" --data-type-id meta:type:Decimal --aggregate-function Sum").ExitCode);
            Assert.Equal(0, RunCli($"add-dimension-usage --workspace \"{path}\" --id SalesDateUsage --measure-group SalesMeasureGroup --cube-dimension DateCubeDimension --usage-kind Regular --role-name OrderDate").ExitCode);
            Assert.Equal(0, RunCli($"add-named-set --workspace \"{path}\" --id TopDates --cube CommerceCube --name TopDates --expression \"TOPCOUNT([Date].[DateKey].MEMBERS, 10)\"").ExitCode);

            var model = MetaMultiDimensionalModel.LoadFromXmlWorkspace(path, searchUpward: false);
            Assert.Single(model.CubeList);
            Assert.Single(model.DimensionList);
            Assert.Single(model.MeasureGroupList);
            Assert.Single(model.DimensionUsageList);
            Assert.Single(model.NamedSetList);
            Assert.Equal("Molap", model.CubeList.Single().StorageMode);
            Assert.Equal("Regular", model.CubeList.Single().ProcessingMode);
            Assert.Equal("Molap", model.DimensionList.Single().StorageMode);
            Assert.Equal("Regular", model.DimensionList.Single().ProcessingMode);
            Assert.Equal("ByAttribute", model.DimensionList.Single().ProcessingGroup);
            Assert.Equal("Molap", model.MeasureGroupList.Single().StorageMode);
            Assert.Equal("Regular", model.MeasureGroupList.Single().ProcessingMode);
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
            Assert.Equal(0, RunCli($"new-workspace \"{path}\"").ExitCode);

            var add = RunCli(
                "--id CommerceDb --name Commerce",
                command: "add-multi-dimensional-database",
                workingDirectory: path);

            Assert.Equal(0, add.ExitCode);
            var model = MetaMultiDimensionalModel.LoadFromXmlWorkspace(path, searchUpward: false);
            var database = Assert.Single(model.MultiDimensionalDatabaseList);
            Assert.Equal("Commerce", database.Name);
        }
        finally
        {
            DeleteDirectoryIfExists(path);
        }
    }

    [Fact]
    public void AddDimensionHelp_ShowsModeDefaults()
    {
        var result = RunCli("add-dimension --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--storage-mode", result.Output);
        Assert.Contains("Default: Molap", result.Output);
        Assert.Contains("--processing-mode", result.Output);
        Assert.Contains("Default: Regular", result.Output);
        Assert.Contains("--processing-group", result.Output);
        Assert.Contains("Default: ByAttribute", result.Output);
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
            "MetaMultiDimensional",
            "meta-multi-dimensional.exe",
            cliArguments,
            workingDirectory: workingDirectory);
    }

    private static string CreateTempPath()
    {
        return Path.Combine(Path.GetTempPath(), "metamultidimensional-cli-tests", Guid.NewGuid().ToString("N"));
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
