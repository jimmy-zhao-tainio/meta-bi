using System.Diagnostics;
using Meta.Integration;
using MetaConvert.TransformPatternToSqlScript;
using MTP = MetaTransformPattern;
using MTPI = MetaTransformPatternInstance;

namespace MetaTransformScript.Tests;

public sealed class MetaTransformPatternCliTests
{
    private const string PatternText =
        "INSERT INTO $(target) ($(target-fields)) SELECT $(source-expressions) FROM $(source);";

    [Fact]
    public void Help_ExposesSeparatePatternAndInstanceWorkflows()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("add-pattern", result.Output);
        Assert.Contains("update-pattern", result.Output);
        Assert.Contains("emit-pattern", result.Output);
        Assert.Contains("create-instance-workspace", result.Output);
        Assert.Contains("add-instance", result.Output);
        Assert.Contains("show-instances", result.Output);
        Assert.Contains("set-placeholder", result.Output);
        Assert.DoesNotContain("set-binding", result.Output);
        Assert.DoesNotContain("clear-binding", result.Output);
        Assert.DoesNotContain("add-binding-value", result.Output);
        Assert.DoesNotContain("add-application", result.Output);
    }

    [Fact]
    public void StandardInputWorkflow_KeepsDefinitionsAndInstancesInSeparateWorkspaces()
    {
        var root = CreateTempRoot();
        try
        {
            var patternWorkspace = Path.Combine(root, "Patterns");
            var instanceWorkspace = Path.Combine(root, "PatternInstances");
            Assert.Equal(0, RunCli($"create --xml \"{patternWorkspace}\"").ExitCode);
            Assert.Equal(0, RunCli($"create-instance-workspace --xml \"{instanceWorkspace}\"").ExitCode);
            Assert.Equal(0, RunCli(
                $"add-pattern --workspace \"{patternWorkspace}\" --id insert-select --name \"Insert select\"",
                PatternText).ExitCode);

            var emitted = RunCli(
                $"emit-pattern --workspace \"{patternWorkspace}\" --id insert-select");
            Assert.Equal(0, emitted.ExitCode);
            Assert.Equal(PatternText + Environment.NewLine, emitted.Output);

            Assert.Equal(0, RunCli(
                $"add-instance --workspace \"{instanceWorkspace}\" --pattern-workspace \"{patternWorkspace}\" --id load-customer --name LoadCustomer --pattern insert-select").ExitCode);
            SetPlaceholder(patternWorkspace, instanceWorkspace, "target", string.Empty);
            var emptyInstances = TypedWorkspaceModelMapper.Load<MTPI.MetaTransformPatternInstanceModel>(
                instanceWorkspace,
                searchUpward: false);
            Assert.Equal(
                string.Empty,
                Assert.Single(emptyInstances.TransformPatternInstancePlaceholderList).SqlText);

            SetPlaceholder(patternWorkspace, instanceWorkspace, "target", "[dbo].[Customer]");
            SetPlaceholder(patternWorkspace, instanceWorkspace, "target-fields", "[CustomerId], [Name]");
            SetPlaceholder(patternWorkspace, instanceWorkspace, "source-expressions", "[s].[CustomerId], [s].[Name]");
            SetPlaceholder(patternWorkspace, instanceWorkspace, "source", "[stage].[Customer] AS [s]");

            var showPatterns = RunCli($"show --workspace \"{patternWorkspace}\"");
            Assert.Equal(0, showPatterns.ExitCode);
            Assert.Contains("insert-select: Insert select (9 items, 4 placeholders)", showPatterns.Output);
            Assert.DoesNotContain("load-customer", showPatterns.Output);

            var showInstances = RunCli(
                $"show-instances --workspace \"{instanceWorkspace}\" --pattern-workspace \"{patternWorkspace}\"");
            Assert.Equal(0, showInstances.ExitCode);
            Assert.Contains(
                "load-customer: LoadCustomer <- insert-select (4/4 placeholders)",
                showInstances.Output);

            var patterns = TypedWorkspaceModelMapper.Load<MTP.MetaTransformPatternModel>(
                patternWorkspace,
                searchUpward: false);
            var instances = TypedWorkspaceModelMapper.Load<MTPI.MetaTransformPatternInstanceModel>(
                instanceWorkspace,
                searchUpward: false);
            var script = Assert.Single(
                TransformPatternToSqlScriptConverter.Convert(patterns, instances).SqlScriptList);
            Assert.Equal(
                "INSERT INTO [dbo].[Customer] ([CustomerId], [Name]) SELECT [s].[CustomerId], [s].[Name] FROM [stage].[Customer] AS [s];",
                script.SqlText);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public void InvalidUpdate_LeavesTheReusablePatternUnchanged()
    {
        var root = CreateTempRoot();
        try
        {
            var patternWorkspace = Path.Combine(root, "Patterns");
            Assert.Equal(0, RunCli($"create --xml \"{patternWorkspace}\"").ExitCode);
            Assert.Equal(0, RunCli(
                $"add-pattern --workspace \"{patternWorkspace}\" --id insert-select --name \"Insert select\"",
                PatternText).ExitCode);

            var invalid = RunCli(
                $"update-pattern --workspace \"{patternWorkspace}\" --id insert-select",
                "SELECT $(missing-close");
            Assert.NotEqual(0, invalid.ExitCode);

            var emitted = RunCli(
                $"emit-pattern --workspace \"{patternWorkspace}\" --id insert-select");
            Assert.Equal(PatternText + Environment.NewLine, emitted.Output);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    private static void SetPlaceholder(
        string patternWorkspace,
        string instanceWorkspace,
        string placeholder,
        string value)
    {
        var result = RunCli(
            $"set-placeholder --workspace \"{instanceWorkspace}\" --pattern-workspace \"{patternWorkspace}\" --instance load-customer --placeholder {placeholder}",
            value);
        Assert.True(result.ExitCode == 0, result.Output);
    }

    private static (int ExitCode, string Output) RunCli(
        string arguments,
        string? standardInput = null)
    {
        var assemblyPath = Path.Combine(AppContext.BaseDirectory, "meta-transform-pattern.dll");
        var startInfo = new ProcessStartInfo
        {
            FileName = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet",
            Arguments = $"\"{assemblyPath}\" {arguments}",
            WorkingDirectory = MetaBi.Tests.Common.CliTestRunner.FindRepositoryRoot(),
            RedirectStandardInput = standardInput is not null,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        using var process = Process.Start(startInfo)
                            ?? throw new InvalidOperationException("Could not start meta-transform-pattern.");
        if (standardInput is not null)
        {
            process.StandardInput.Write(standardInput);
            process.StandardInput.Close();
        }

        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(TimeSpan.FromSeconds(30)))
        {
            process.Kill(entireProcessTree: true);
            process.WaitForExit();
            throw new TimeoutException(
                $"Timed out waiting for meta-transform-pattern {arguments}.");
        }
        return (process.ExitCode, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
    }

    private static string CreateTempRoot()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "MetaTransformPattern.Cli.Tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    private static void DeleteTempRoot(string root)
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
