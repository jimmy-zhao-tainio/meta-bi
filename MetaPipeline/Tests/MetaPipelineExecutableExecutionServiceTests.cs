namespace MetaPipeline.Tests;

public sealed class MetaPipelineExecutableExecutionServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WhenExitCodeMatches_ReturnsSucceeded()
    {
        var result = await new MetaPipelineExecutableExecutionService().ExecuteAsync(
            new MetaPipelineExecutableExecutionRequest(
                "cmd-success",
                ResolveCmdExe(),
                "/c exit /b 0"));

        Assert.True(result.Succeeded);
        Assert.Equal(MetaPipelineExecutionStatus.Succeeded, result.Status);
        var task = Assert.Single(result.TaskResults);
        Assert.Equal("Executable", task.TaskKind);
        Assert.Equal(MetaPipelineExecutionTaskStatus.Succeeded, task.Status);
        Assert.Equal(0, task.ExitCode);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExitCodeDiffers_ReturnsFailedWithRealExitCode()
    {
        var result = await new MetaPipelineExecutableExecutionService().ExecuteAsync(
            new MetaPipelineExecutableExecutionRequest(
                "cmd-failure",
                ResolveCmdExe(),
                "/c echo process failed & exit /b 7"));

        Assert.False(result.Succeeded);
        Assert.Equal(MetaPipelineExecutionStatus.Failed, result.Status);
        Assert.Equal(PipelineExecutionFailureStage.Executable, result.FailureStage);
        Assert.Contains("exited with code 7", result.FailureMessage, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("process failed", result.FailureMessage, StringComparison.OrdinalIgnoreCase);
        var task = Assert.Single(result.TaskResults);
        Assert.Equal("Executable", task.TaskKind);
        Assert.Equal(MetaPipelineExecutionTaskStatus.Failed, task.Status);
        Assert.Equal(7, task.ExitCode);
    }

    private static string ResolveCmdExe() =>
        Environment.GetEnvironmentVariable("ComSpec") ?? "cmd.exe";
}
