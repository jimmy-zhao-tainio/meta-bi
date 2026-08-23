using System.Diagnostics;

namespace MetaBi.Tests.Common;

public static class CliTestRunner
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(2);

    public static (int ExitCode, string Output) RunStandardCli(
        string cliAssemblyName,
        string arguments,
        string? pathPrefix = null,
        TimeSpan? timeout = null,
        string? workingDirectory = null)
    {
        var startInfo = CreateManagedCliStartInfo(
            cliAssemblyName,
            arguments,
            pathPrefix,
            workingDirectory);
        return RunProcess(startInfo, $"Could not start {cliAssemblyName} CLI process.", timeout ?? DefaultTimeout);
    }

    public static ProcessStartInfo CreateManagedCliStartInfo(
        string cliAssemblyName,
        string arguments,
        string? pathPrefix = null,
        string? workingDirectory = null)
    {
        var repoRoot = FindRepositoryRoot();
        var startInfo = new ProcessStartInfo
        {
            FileName = DotNetHost,
            Arguments = BuildArguments(cliAssemblyName, arguments),
            WorkingDirectory = workingDirectory ?? repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        if (!string.IsNullOrWhiteSpace(pathPrefix))
        {
            startInfo.Environment["PATH"] = pathPrefix + Path.PathSeparator + startInfo.Environment["PATH"];
        }

        return startInfo;
    }

    public static void ConfigureManagedCli(ProcessStartInfo startInfo, string cliAssemblyName)
    {
        startInfo.FileName = DotNetHost;
        startInfo.Arguments = BuildArguments(cliAssemblyName, startInfo.Arguments);
    }

    public static string FindRepositoryRoot() => FindRepositoryRoot(requiredRelativePath: null);

    public static string FindRepositoryRoot(string? requiredRelativePath)
    {
        var directory = AppContext.BaseDirectory;
        while (!string.IsNullOrWhiteSpace(directory))
        {
            if (File.Exists(Path.Combine(directory, "README.md")) &&
                (string.IsNullOrWhiteSpace(requiredRelativePath) ||
                 Directory.Exists(Path.Combine(directory, requiredRelativePath))))
            {
                return directory;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        throw new InvalidOperationException("Could not locate meta-bi repository root from test base directory.");
    }

    private static string DotNetHost =>
        Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";

    private static string BuildArguments(string cliAssemblyName, string arguments)
    {
        var assemblyPath = Path.Combine(AppContext.BaseDirectory, cliAssemblyName + ".dll");
        if (!File.Exists(assemblyPath))
        {
            throw new FileNotFoundException(
                $"The test build did not produce the '{cliAssemblyName}' CLI assembly at '{assemblyPath}'.",
                assemblyPath);
        }

        var quotedAssemblyPath = "\"" + assemblyPath.Replace("\"", "\\\"", StringComparison.Ordinal) + "\"";
        return string.IsNullOrWhiteSpace(arguments)
            ? quotedAssemblyPath
            : quotedAssemblyPath + " " + arguments;
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
