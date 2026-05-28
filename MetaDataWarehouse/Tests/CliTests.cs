using MetaBi.Tests.Common;

namespace MetaDataWarehouse.Tests;

public sealed class CliTests
{
    [Fact]
    public void Help_ShowsWorkspaceCommands()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-data-warehouse [--new-workspace <path> | <command> [options]]", result.Output);
        Assert.Contains("--new-workspace", result.Output);
        Assert.Contains("add-dimension", result.Output);
        Assert.DoesNotContain("create-sample", result.Output);
        Assert.DoesNotContain("init-implementation", result.Output);
        Assert.DoesNotContain("--additivity", result.Output);
    }

    [Fact]
    public void AddCommandHelp_ShowsDescriptorOptions()
    {
        var result = RunCli("add-dimension --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Command: add-dimension", result.Output);
        Assert.Contains("Options:", result.Output);
        Assert.Contains("--workspace <path>", result.Output);
        Assert.Contains("--id <id>", result.Output);
        Assert.Contains("--warehouse <id>", result.Output);
        Assert.Contains("MetaDataWarehouse workspace", result.Output);
    }

    [Fact]
    public void NewWorkspaceAndAuthoringCommands_CreateLogicalWorkspace()
    {
        var path = CreateTempPath();
        try
        {
            var create = RunCli($"--new-workspace \"{path}\"");
            Assert.Equal(0, create.ExitCode);
            Assert.Contains("Ok", create.Output);
            Assert.True(File.Exists(Path.Combine(path, "workspace.xml")));
            Assert.True(File.Exists(Path.Combine(path, "model.xml")));

            Assert.Equal(0, RunCli($"add-warehouse --workspace \"{path}\" --id Commerce --name Commerce").ExitCode);
            Assert.Equal(0, RunCli($"add-dimension --workspace \"{path}\" --id Customer --warehouse Commerce --name Customer").ExitCode);
            Assert.Equal(0, RunCli($"add-dimension-attribute --workspace \"{path}\" --id CustomerNumber --dimension Customer --name CustomerNumber --data-type-id meta:type:String").ExitCode);
            Assert.Equal(0, RunCli($"add-dimension-business-key --workspace \"{path}\" --id CustomerBusinessKey --dimension Customer --name CustomerBusinessKey").ExitCode);
            Assert.Equal(0, RunCli($"add-dimension-business-key-part --workspace \"{path}\" --id CustomerBusinessKeyPart --business-key CustomerBusinessKey --attribute CustomerNumber").ExitCode);
            Assert.Equal(0, RunCli($"add-slowly-changing-dimension --workspace \"{path}\" --id CustomerHistory --dimension Customer --name CustomerHistory").ExitCode);
            Assert.Equal(0, RunCli($"add-fact --workspace \"{path}\" --id SalesOrder --warehouse Commerce --name SalesOrder").ExitCode);
            Assert.Equal(0, RunCli($"add-fact-measure --workspace \"{path}\" --id SalesAmount --fact SalesOrder --name SalesAmount --data-type-id meta:type:Decimal").ExitCode);

            var model = MetaDataWarehouseModel.LoadFromXmlWorkspace(path, searchUpward: false);
            var keyPart = Assert.Single(model.DimensionBusinessKeyPartList);
            Assert.Equal("CustomerNumber", keyPart.DimensionAttribute.Name);
            Assert.Single(model.SlowlyChangingDimensionList);
            var measure = Assert.Single(model.FactMeasureList);
            Assert.Equal("SalesAmount", measure.Name);
            Assert.Equal("meta:type:Decimal", measure.DataTypeId);
        }
        finally
        {
            DeleteDirectoryIfExists(path);
        }
    }

    private static (int ExitCode, string Output) RunCli(string arguments) =>
        CliTestRunner.RunStandardCli("MetaDataWarehouse", "meta-data-warehouse.exe", arguments);

    private static string CreateTempPath()
    {
        return Path.Combine(Path.GetTempPath(), "metadatawarehouse-cli-tests", Guid.NewGuid().ToString("N"));
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
