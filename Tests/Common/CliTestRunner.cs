using System.Diagnostics;

namespace MetaBi.Tests.Common;

public static class CliTestRunner
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(2);

    public static (int ExitCode, string Output) RunStandardCli(
        string cliOwnerDirectory,
        string executableName,
        string arguments,
        string? pathPrefix = null,
        TimeSpan? timeout = null)
    {
        var cliRelativeDirectory = Path.Combine(cliOwnerDirectory, "Cli");
        var executableRelativePath = Path.Combine(
            cliRelativeDirectory,
            "bin",
            "Debug",
            "net8.0",
            executableName);

        return RunExecutable(
            executableRelativePath,
            cliRelativeDirectory,
            executableName,
            arguments,
            pathPrefix,
            timeout);
    }

    public static (int ExitCode, string Output) RunExecutable(
        string executableRelativePath,
        string requiredRelativePath,
        string displayName,
        string arguments,
        string? pathPrefix = null,
        TimeSpan? timeout = null)
    {
        var repoRoot = FindRepositoryRoot(requiredRelativePath);
        var executablePath = Path.Combine(repoRoot, executableRelativePath);
        if (!File.Exists(executablePath))
        {
            throw new FileNotFoundException(
                $"Could not find compiled {displayName} CLI at '{executablePath}'. Build {displayName} before running tests.");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            Arguments = arguments,
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        if (!string.IsNullOrWhiteSpace(pathPrefix))
        {
            startInfo.Environment["PATH"] = $"{pathPrefix};{startInfo.Environment["PATH"]}";
        }

        return RunProcess(startInfo, $"Could not start {displayName} CLI process.", timeout ?? DefaultTimeout);
    }

    public static string FindRepositoryRoot(string requiredRelativePath)
    {
        var directory = AppContext.BaseDirectory;
        while (!string.IsNullOrWhiteSpace(directory))
        {
            if (File.Exists(Path.Combine(directory, "README.md")) &&
                Directory.Exists(Path.Combine(directory, requiredRelativePath)))
            {
                return directory;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        throw new InvalidOperationException("Could not locate meta-bi repository root from test base directory.");
    }

    private static (int ExitCode, string Output) RunProcess(
        ProcessStartInfo startInfo,
        string errorMessage,
        TimeSpan timeout)
    {
        using var process = Process.Start(startInfo)
                            ?? throw new InvalidOperationException(errorMessage);
        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        try
        {
            using var timeoutSource = new CancellationTokenSource(timeout);
            try
            {
                process.WaitForExitAsync(timeoutSource.Token).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException exception)
            {
                TryKillProcessTree(process);
                process.WaitForExit();
                throw new TimeoutException($"Timed out waiting for process: {startInfo.FileName} {startInfo.Arguments}", exception);
            }

            var stdout = stdoutTask.GetAwaiter().GetResult();
            var stderr = stderrTask.GetAwaiter().GetResult();
            return (process.ExitCode, stdout + stderr);
        }
        finally
        {
            if (!process.HasExited)
            {
                TryKillProcessTree(process);
                process.WaitForExit();
            }
        }
    }

    private static void TryKillProcessTree(Process process)
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
        catch (NotSupportedException)
        {
        }
    }
}
