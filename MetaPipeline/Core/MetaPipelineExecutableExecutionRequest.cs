namespace MetaPipeline;

public sealed record MetaPipelineExecutableExecutionRequest(
    string TaskName,
    string ExecutablePath,
    string? Arguments = null,
    string? WorkingDirectory = null,
    int SuccessExitCode = 0,
    int? TimeoutSeconds = null,
    MetaPipelineExecutionContext? ExecutionContext = null);
