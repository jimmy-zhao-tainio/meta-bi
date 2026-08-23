using MetaBi.Tests.Common;

namespace MetaTransformScript.Tests;

public sealed class MetaTransformBindingCliTests
{
    [Fact]
    public void Help_ShowsBindCommand()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-transform-binding <command> [options]", result.Output);
        Assert.Contains("bind", result.Output);
        Assert.Contains("help", result.Output);
    }

    [Fact]
    public void BindHelp_ShowsModeledOptions()
    {
        var result = RunCli("bind --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--transform-workspace <path>", result.Output);
        Assert.Contains("--source-schema <path>", result.Output);
        Assert.Contains("--target-schema <path>", result.Output);
        Assert.Contains("--execute-system <value>", result.Output);
        Assert.Contains("--output-xml <path>", result.Output);
        Assert.Contains("--ignore-target-columns <col[,col...]>", result.Output);
        Assert.Contains("--allow-partial", result.Output);
        Assert.Contains("--partial-report <path>", result.Output);
    }

    [Fact]
    public void Bind_WhenSourceSchemaIsMissing_FailsInMetaCliParser()
    {
        var result = RunCli("bind --transform-workspace transform --target-schema target --execute-system sys --output-xml binding");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Required parameter 'source-schema' was not provided.", result.Output);
    }

    [Fact]
    public void Bind_WhenPartialReportIsProvidedWithoutAllowPartial_FailsDomainValidation()
    {
        var result = RunCli("bind --transform-workspace transform --source-schema source --target-schema target --execute-system sys --output-xml binding --partial-report skipped.tsv");

        Assert.Equal(1, result.ExitCode);
        Assert.Contains("--partial-report requires --allow-partial.", result.Output);
        Assert.Contains("Next: meta-transform-binding help bind", result.Output);
    }

    private static (int ExitCode, string Output) RunCli(string arguments) =>
        CliTestRunner.RunStandardCli(
            "meta-transform-binding",
            arguments);
}
