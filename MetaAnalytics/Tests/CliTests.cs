using MetaBi.Tests.Common;

namespace MetaAnalytics.Tests;

public sealed class CliTests
{
    [Fact]
    public void Help_ShowsWorkspaceAndAuthoringCommands()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-analytics <command> [options]", result.Output);
        Assert.Contains("new-workspace", result.Output);
        Assert.Contains("add-data-source", result.Output);
        Assert.Contains("add-role-filter", result.Output);
        Assert.Contains("add-attribute-permission", result.Output);
        Assert.DoesNotContain("--new-workspace", result.Output);
        Assert.DoesNotContain("add-measure-expression", result.Output);
        Assert.DoesNotContain("add-kpi", result.Output);
        Assert.DoesNotContain("add-calculation-group", result.Output);
        Assert.DoesNotContain("add-cell-permission", result.Output);
        Assert.DoesNotContain("Semantic", result.Output);
    }

    [Fact]
    public void AddCommandHelp_ShowsDescriptorOptions()
    {
        var result = RunCli("add-model --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-analytics add-model", result.Output);
        Assert.Contains("Options:", result.Output);
        Assert.Contains("--workspace <path>", result.Output);
        Assert.Contains("--id <value>", result.Output);
        Assert.Contains("--name <value>", result.Output);
        Assert.Contains("MetaAnalytics workspace", result.Output);
    }

    [Fact]
    public void NewWorkspaceAndAuthoringCommands_CreateLogicalWorkspace()
    {
        var path = CreateTempPath();
        try
        {
            var create = RunCli($"new-workspace \"{path}\"");
            Assert.Equal(0, create.ExitCode);
            Assert.Contains("MetaAnalytics workspace created", create.Output);

            Assert.Equal(0, RunCli($"add-model --workspace \"{path}\" --id Commerce --name Commerce --default-culture en-US").ExitCode);
            Assert.Equal(0, RunCli($"add-data-source --workspace \"{path}\" --id WarehouseSource --model Commerce --name Warehouse --provider SqlServer --connection-reference COMMERCE_DW").ExitCode);
            Assert.Equal(0, RunCli($"add-table --workspace \"{path}\" --id Date --model Commerce --name Date --kind Dimension --data-category Time").ExitCode);
            Assert.Equal(0, RunCli($"add-table --workspace \"{path}\" --id Product --model Commerce --name Product --kind Dimension").ExitCode);
            Assert.Equal(0, RunCli($"add-table --workspace \"{path}\" --id Sales --model Commerce --name Sales --kind Fact").ExitCode);
            Assert.Equal(0, RunCli($"add-attribute --workspace \"{path}\" --id DateKey --table Date --name DateKey --data-type-id meta:type:Int32 --is-key true").ExitCode);
            Assert.Equal(0, RunCli($"add-attribute --workspace \"{path}\" --id CalendarYear --table Date --name CalendarYear --data-type-id meta:type:Int32").ExitCode);
            Assert.Equal(0, RunCli($"add-attribute --workspace \"{path}\" --id ProductName --table Product --name ProductName --data-type-id meta:type:String").ExitCode);
            Assert.Equal(0, RunCli($"add-attribute --workspace \"{path}\" --id SalesAmountColumn --table Sales --name SalesAmount --data-type-id meta:type:Decimal --is-hidden true").ExitCode);
            Assert.Equal(0, RunCli($"add-hierarchy --workspace \"{path}\" --id Calendar --table Date --name Calendar").ExitCode);
            Assert.Equal(0, RunCli($"add-hierarchy-level --workspace \"{path}\" --id CalendarYearLevel --hierarchy Calendar --attribute CalendarYear --name Year").ExitCode);
            Assert.Equal(0, RunCli($"add-measure --workspace \"{path}\" --id SalesAmount --table Sales --source-attribute SalesAmountColumn --name SalesAmount --data-type-id meta:type:Decimal").ExitCode);
            Assert.Equal(0, RunCli($"add-aggregation-behavior --workspace \"{path}\" --id SalesAmountAggregation --measure SalesAmount --function Sum").ExitCode);
            Assert.Equal(0, RunCli($"add-perspective --workspace \"{path}\" --id SalesPerspective --model Commerce --name Sales").ExitCode);
            Assert.Equal(0, RunCli($"add-perspective-measure --workspace \"{path}\" --id SalesPerspectiveSalesAmount --perspective SalesPerspective --measure SalesAmount").ExitCode);
            Assert.Equal(0, RunCli($"add-security-role --workspace \"{path}\" --id Reader --model Commerce --name Reader --permission Read").ExitCode);
            Assert.Equal(0, RunCli($"add-role-filter --workspace \"{path}\" --id ReaderSalesFilter --role Reader --table Sales --expression-language DAX --expression \"Sales[Region] = USERNAME()\"").ExitCode);

            var model = MetaAnalyticsModel.LoadFromXmlWorkspace(path, searchUpward: false);
            Assert.Single(model.AnalyticsModelList);
            Assert.Single(model.DataSourceList);
            Assert.Single(model.MeasureList);
            Assert.Single(model.AggregationBehaviorList);
            Assert.Single(model.RoleFilterList);
            Assert.Same(model.AttributeList.Single(row => row.Id == "SalesAmountColumn"), model.MeasureList.Single().SourceAttribute);
            Assert.Same(model.MeasureList.Single(), model.PerspectiveMeasureList.Single().Measure);
        }
        finally
        {
            DeleteDirectoryIfExists(path);
        }
    }

    [Fact]
    public void HierarchyLevelRejectsAttributeFromDifferentTable()
    {
        var path = CreateTempPath();
        try
        {
            Assert.Equal(0, RunCli($"new-workspace \"{path}\"").ExitCode);
            Assert.Equal(0, RunCli($"add-model --workspace \"{path}\" --id Commerce --name Commerce").ExitCode);
            Assert.Equal(0, RunCli($"add-table --workspace \"{path}\" --id Date --model Commerce --name Date --kind Dimension").ExitCode);
            Assert.Equal(0, RunCli($"add-table --workspace \"{path}\" --id Product --model Commerce --name Product --kind Dimension").ExitCode);
            Assert.Equal(0, RunCli($"add-attribute --workspace \"{path}\" --id ProductName --table Product --name ProductName --data-type-id meta:type:String").ExitCode);
            Assert.Equal(0, RunCli($"add-hierarchy --workspace \"{path}\" --id Calendar --table Date --name Calendar").ExitCode);

            var result = RunCli($"add-hierarchy-level --workspace \"{path}\" --id BadLevel --hierarchy Calendar --attribute ProductName --name Product");
            Assert.Equal(4, result.ExitCode);
            Assert.Contains("outside hierarchy table", result.Output);
        }
        finally
        {
            DeleteDirectoryIfExists(path);
        }
    }

    [Fact]
    public void AuthoringCommandWithoutWorkspaceDoesNotCreateWorkspaceInArbitraryDirectory()
    {
        var path = CreateTempPath();
        try
        {
            Directory.CreateDirectory(path);

            var result = RunCli("add-model --id Commerce --name Commerce", workingDirectory: path);

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("does not contain workspace.xml", result.Output);
            Assert.False(File.Exists(Path.Combine(path, "workspace.xml")));
            Assert.False(File.Exists(Path.Combine(path, "model.xml")));
            Assert.False(Directory.Exists(Path.Combine(path, "instances")));
        }
        finally
        {
            DeleteDirectoryIfExists(path);
        }
    }

    private static (int ExitCode, string Output) RunCli(string arguments, string? workingDirectory = null) =>
        CliTestRunner.RunStandardCli("MetaAnalytics", "meta-analytics.exe", arguments, workingDirectory: workingDirectory);

    private static string CreateTempPath()
    {
        return Path.Combine(Path.GetTempPath(), "metaanalytics-cli-tests", Guid.NewGuid().ToString("N"));
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
