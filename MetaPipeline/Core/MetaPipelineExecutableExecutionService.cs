using System.Diagnostics;
using System.Text;

namespace MetaPipeline;

public sealed class MetaPipelineExecutableExecutionService
{
    private const int OutputSnippetLength = 4000;

    public async Task<MetaPipelineExecutionResult> ExecuteAsync(
        MetaPipelineExecutableExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var startedAtUtc = DateTimeOffset.UtcNow;
        var taskStartedAtUtc = request.ExecutionContext?.TaskStartedAtUtc ?? startedAtUtc;
        var taskName = RequireValue(request.TaskName, "Executable task name is required.");
        var executablePath = RequireValue(request.ExecutablePath, $"Executable task '{taskName}' must name an executable path.");

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = request.Arguments ?? string.Empty,
                WorkingDirectory = NormalizeWorkingDirectory(request.WorkingDirectory),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
            EnableRaisingEvents = true,
        };

        try
        {
            process.Start();
        }
        catch (Exception ex)
        {
            return CreateFailedResult(
                request,
                taskName,
                startedAtUtc,
                taskStartedAtUtc,
                $"Could not start executable '{executablePath}'. {ex.Message}");
        }

        var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        var waitResult = await WaitForProcessExitAsync(
            process,
            request.TimeoutSeconds,
            cancellationToken).ConfigureAwait(false);
        var stdout = await CompleteReadAsync(stdoutTask).ConfigureAwait(false);
        var stderr = await CompleteReadAsync(stderrTask).ConfigureAwait(false);

        if (waitResult.TimedOut)
        {
            return CreateFailedResult(
                request,
                taskName,
                startedAtUtc,
                taskStartedAtUtc,
                $"Executable task '{taskName}' timed out after {request.TimeoutSeconds} second(s)."
                + RenderOutputEvidence(stdout, stderr));
        }

        var exitCode = process.ExitCode;
        if (exitCode != request.SuccessExitCode)
        {
            return CreateFailedResult(
                request,
                taskName,
                startedAtUtc,
                taskStartedAtUtc,
                $"Executable task '{taskName}' exited with code {exitCode}; expected {request.SuccessExitCode}."
                + RenderOutputEvidence(stdout, stderr),
                exitCode);
        }

        var completedAtUtc = DateTimeOffset.UtcNow;
        var taskResults = new[]
        {
            new MetaPipelineExecutionTaskResult(
                taskName,
                "Executable",
                MetaPipelineExecutionTaskStatus.Succeeded,
                taskStartedAtUtc,
                completedAtUtc,
                0,
                0,
                PipelineExecutionFailureStage.None,
                string.Empty,
                request.ExecutionContext?.TaskRunId,
                request.ExecutionContext?.AuditId,
                request.TimeoutSeconds,
                exitCode),
        };

        return new MetaPipelineExecutionResult(
            MetaPipelineExecutionStatus.Succeeded,
            taskName,
            string.Empty,
            "ProcessExecute",
            "Executable",
            0,
            0,
            0,
            startedAtUtc,
            completedAtUtc,
            PipelineExecutionFailureStage.None,
            string.Empty,
            string.Empty,
            taskResults);
    }

    private static async Task<ProcessWaitResult> WaitForProcessExitAsync(
        Process process,
        int? timeoutSeconds,
        CancellationToken cancellationToken)
    {
        if (timeoutSeconds is null or 0)
        {
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            return new ProcessWaitResult(false);
        }

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds.Value));
        try
        {
            await process.WaitForExitAsync(timeoutCts.Token).ConfigureAwait(false);
            return new ProcessWaitResult(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            TryKill(process);
            await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
            return new ProcessWaitResult(true);
        }
    }

    private static async Task<string> CompleteReadAsync(Task<string> readTask)
    {
        try
        {
            return await readTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return string.Empty;
        }
        catch (InvalidOperationException)
        {
            return string.Empty;
        }
    }

    private static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
        }
    }

    private static MetaPipelineExecutionResult CreateFailedResult(
        MetaPipelineExecutableExecutionRequest request,
        string taskName,
        DateTimeOffset startedAtUtc,
        DateTimeOffset taskStartedAtUtc,
        string failureMessage,
        int? exitCode = null)
    {
        var completedAtUtc = DateTimeOffset.UtcNow;
        var taskResults = new[]
        {
            new MetaPipelineExecutionTaskResult(
                taskName,
                "Executable",
                MetaPipelineExecutionTaskStatus.Failed,
                taskStartedAtUtc,
                completedAtUtc,
                0,
                0,
                PipelineExecutionFailureStage.Executable,
                failureMessage,
                request.ExecutionContext?.TaskRunId,
                request.ExecutionContext?.AuditId,
                request.TimeoutSeconds,
                exitCode),
        };

        return new MetaPipelineExecutionResult(
            MetaPipelineExecutionStatus.Failed,
            taskName,
            string.Empty,
            "ProcessExecute",
            "Executable",
            0,
            0,
            0,
            startedAtUtc,
            completedAtUtc,
            PipelineExecutionFailureStage.Executable,
            failureMessage,
            taskName,
            taskResults);
    }

    private static string RenderOutputEvidence(string stdout, string stderr)
    {
        var builder = new StringBuilder();
        AppendSnippet(builder, "stdout", stdout);
        AppendSnippet(builder, "stderr", stderr);
        return builder.ToString();
    }

    private static void AppendSnippet(StringBuilder builder, string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        var trimmed = value.Trim();
        if (trimmed.Length > OutputSnippetLength)
        {
            trimmed = trimmed[..OutputSnippetLength] + " ...";
        }

        builder.Append(' ');
        builder.Append(name);
        builder.Append(": ");
        builder.Append(trimmed.ReplaceLineEndings(" "));
    }

    private static string NormalizeWorkingDirectory(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim();

    private static string RequireValue(string? value, string errorMessage) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new MetaPipelineConfigurationException(errorMessage)
            : value.Trim();

    private sealed record ProcessWaitResult(bool TimedOut);
}
