using MetaBi.Tests.Common;

namespace MetaOrchestration.Tests;

[Trait("Category", "Integration")]
public sealed class ExecutionCatalogCliIntegrationTests
{
    [Fact]
    public void ExecuteExposesCatalogSelectionAndRejectsConversionWorkspace()
    {
        var help = CliTestRunner.RunStandardCli("meta-orchestration", "execute --help");
        Assert.Equal(0, help.ExitCode);
        Assert.Contains("--data-type-workspace", help.Output);
        Assert.DoesNotContain("--data-type-conversion-workspace", help.Output);
        var retired = CliTestRunner.RunStandardCli("meta-orchestration", "execute --data-type-conversion-workspace unused");
        Assert.NotEqual(0, retired.ExitCode);
        Assert.Contains("data-type-conversion-workspace", retired.Output);
    }
}
