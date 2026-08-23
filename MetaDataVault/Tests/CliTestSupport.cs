using System.Diagnostics;
using MetaBi.Tests.Common;

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

    internal static ProcessStartInfo CreateManagedCliStartInfo(
        string cliAssemblyName,
        string arguments,
        string workingDirectory)
    {
        return CliTestRunner.CreateManagedCliStartInfo(cliAssemblyName, arguments, workingDirectory: workingDirectory);
    }
}

