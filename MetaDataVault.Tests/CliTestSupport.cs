using System.Diagnostics;

namespace MetaDataVault.Tests;

internal static class CliTestSupport
{
    internal static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "MetaDataVault.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not locate repository root.");
    }

    internal static string ResolveCliPath(string repoRoot, string projectDirectory, string assemblyName)
    {
        var path = Path.Combine(repoRoot, projectDirectory, "bin", "Debug", "net8.0", assemblyName);
        if (File.Exists(path))
        {
            return path;
        }

        path = Path.Combine(repoRoot, projectDirectory, "bin", "Debug", "net8.0", "win-x64", assemblyName);
        if (File.Exists(path))
        {
            return path;
        }

        throw new FileNotFoundException($"Could not locate '{assemblyName}' under '{projectDirectory}'.");
    }

    internal static (int ExitCode, string Output) RunProcess(ProcessStartInfo startInfo, string errorMessage)
    {
        using var process = new Process { StartInfo = startInfo };
        if (!process.Start())
        {
            throw new InvalidOperationException(errorMessage);
        }

        var stdOut = process.StandardOutput.ReadToEnd();
        var stdErr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return (process.ExitCode, stdOut + stdErr);
    }
}
