using System.Diagnostics;
using MetaTransformBinding;
using MetaTransformScript;
using MetaTransformScript.Sql;

namespace MetaPipeline.Tests;

public sealed class MetaPipelineModelCliTests
{
    [Fact]
    public void Help_ShowsInstanceCommands()
    {
        var result = RunCli("--help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("init", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("add-pipeline", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("add-transform", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("inspect", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExecuteHelp_ShowsModeledExecutionOptions()
    {
        var result = RunCli("execute --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--task", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InitAddPipelineAndAddTransform_CreatesSanctionedWorkspace()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaPipeline.Tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(tempRoot, "pipeline");
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");
        var sourceEnv = "META_PIPELINE_TEST_SOURCE_" + Guid.NewGuid().ToString("N").ToUpperInvariant();
        var targetEnv = "META_PIPELINE_TEST_TARGET_" + Guid.NewGuid().ToString("N").ToUpperInvariant();

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                "select 1 as CustomerId, 'Acme' as CustomerName, cast(125.50 as decimal(18, 2)) as TotalAmount",
                "dbo.TargetCustomer",
                transformWorkspacePath,
                "dbo.v_customer_load");
            var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspacePath, searchUpward: false);
            var transformScript = Assert.Single(transformModel.TransformScriptList);
            BuildBindingWorkspace(
                bindingWorkspacePath,
                transformScript,
                "dbo.TargetCustomer",
                ["CustomerId", "CustomerName", "TotalAmount"],
                [0, 1, 2]);

            var init = RunCli($"init --new-workspace \"{workspacePath}\"");

            Assert.Equal(0, init.ExitCode);
            Assert.True(File.Exists(Path.Combine(workspacePath, "workspace.xml")));
            Assert.True(File.Exists(Path.Combine(workspacePath, "model.xml")));
            Assert.Contains("Pipelines: 0", init.Output, StringComparison.Ordinal);

            var addPipeline = RunCli($"add-pipeline --workspace \"{workspacePath}\" --name CustomerLoad --description \"Customer load\"");

            Assert.Equal(0, addPipeline.ExitCode);
            Assert.Contains("Pipeline: CustomerLoad", addPipeline.Output, StringComparison.Ordinal);

            var add = RunCli(
                $"add-transform --workspace \"{workspacePath}\" --pipeline CustomerLoad --task load-customers --transform-workspace \"{transformWorkspacePath}\" --binding-workspace \"{bindingWorkspacePath}\" --transform-script-id \"{transformScript.Id}\" --transform-binding-id binding:customer-load --source-connection-ref source --source-connection-env {sourceEnv} --target-connection-ref target --target-connection-env {targetEnv} --target dbo.TargetCustomer --batch-size 2");

            Assert.Equal(0, add.ExitCode);
            Assert.Contains("InsertRows", add.Output, StringComparison.Ordinal);
            Assert.Contains("Columns: 3", add.Output, StringComparison.Ordinal);

            var inspect = RunCli($"inspect --workspace \"{workspacePath}\"");

            Assert.Equal(0, inspect.ExitCode);
            Assert.Contains("Tasks: 2", inspect.Output, StringComparison.Ordinal);
            Assert.Contains("RowStreamColumns: 3", inspect.Output, StringComparison.Ordinal);
            Assert.Contains("load-customers [TransformExecution]", inspect.Output, StringComparison.Ordinal);
            Assert.Contains("load-customers.target-write [TargetWrite:InsertRows]", inspect.Output, StringComparison.Ordinal);

            var model = global::MetaPipeline.MetaPipelineModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);

            var pipeline = Assert.Single(model.PipelineList);
            Assert.Equal("CustomerLoad", pipeline.Id);
            Assert.Equal("CustomerLoad", pipeline.Name);
            Assert.Equal(2, model.PipelineTaskList.Count);
            Assert.Equal(2, model.ConnectionReferenceList.Count);
            Assert.Single(model.RowStreamList);
            Assert.Equal(3, model.RowStreamColumnList.Count);
            Assert.Single(model.RowStreamProducerList);
            Assert.Single(model.RowStreamConsumerList);
            Assert.Single(model.TaskDependencyList);

            Assert.Contains(model.PipelineTaskList, task => task.Id == "CustomerLoad.load-customers");
            Assert.Contains(model.PipelineTaskList, task => task.Id == "CustomerLoad.load-customers.target-write");

            var transformExecution = Assert.Single(model.TransformExecutionTaskList);
            Assert.Equal(transformScript.Id, transformExecution.TransformScriptId);
            Assert.Equal("binding:customer-load", transformExecution.TransformBindingId);

            var targetWrite = Assert.Single(model.TargetWriteTaskList);
            var insertRows = Assert.Single(model.InsertRowsTargetWriteTaskList);
            Assert.Equal(targetWrite.Id, insertRows.TargetWriteTaskId);
            Assert.Equal("dbo.TargetCustomer", insertRows.TargetSqlIdentifier);
            Assert.Equal("2", insertRows.BatchSize);
            Assert.Collection(
                model.RowStreamColumnList.OrderBy(static item => int.Parse(item.Ordinal)),
                column =>
                {
                    Assert.Equal("CustomerId", column.Name);
                    Assert.Equal("0", column.Ordinal);
                },
                column =>
                {
                    Assert.Equal("CustomerName", column.Name);
                    Assert.Equal("1", column.Ordinal);
                },
                column =>
                {
                    Assert.Equal("TotalAmount", column.Name);
                    Assert.Equal("2", column.Ordinal);
                });

            var execute = RunCli(
                $"execute --workspace \"{workspacePath}\" --pipeline CustomerLoad --task load-customers --transform-workspace \"{transformWorkspacePath}\" --binding-workspace \"{bindingWorkspacePath}\"");

            Assert.Equal(1, execute.ExitCode);
            Assert.Contains(sourceEnv, execute.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("dbo.TargetCustomer", execute.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    private static void BuildBindingWorkspace(
        string bindingWorkspacePath,
        TransformScript transformScript,
        string targetSqlIdentifier,
        IReadOnlyList<string> columns,
        IReadOnlyList<int> ordinals)
    {
        var model = MetaTransformBindingModel.CreateEmpty();
        model.TransformBindingList.Add(new TransformBinding
        {
            Id = "binding:customer-load",
            MetaTransformScriptTransformScriptId = transformScript.Id,
            TransformScriptName = transformScript.Name,
        });

        model.RowsetList.Add(new Rowset
        {
            Id = "rowset:customer-load",
            TransformBindingId = "binding:customer-load",
            DerivationKind = "Output",
            Name = transformScript.Name,
        });
        model.OutputRowsetList.Add(new OutputRowset
        {
            Id = "output:customer-load",
            TransformBindingId = "binding:customer-load",
            RowsetId = "rowset:customer-load",
        });
        model.TransformBindingTargetList.Add(new TransformBindingTarget
        {
            Id = "target:customer-load",
            TransformBindingId = "binding:customer-load",
            SqlIdentifier = targetSqlIdentifier,
        });

        for (var index = 0; index < columns.Count; index++)
        {
            model.ColumnList.Add(new Column
            {
                Id = $"column:{index + 1}",
                RowsetId = "rowset:customer-load",
                Name = columns[index],
                Ordinal = ordinals[index].ToString(),
            });
        }

        model.SaveToXmlWorkspace(bindingWorkspacePath);
    }

    private static (int ExitCode, string Output) RunCli(string arguments)
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot);
        var startInfo = new ProcessStartInfo
        {
            FileName = cliPath,
            Arguments = arguments,
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(startInfo)
                            ?? throw new InvalidOperationException("Could not start meta-pipeline CLI process.");
        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(2));
        try
        {
            process.WaitForExitAsync(timeout.Token).GetAwaiter().GetResult();
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

    private static string ResolveCliPath(string repoRoot)
    {
        var cliPath = Path.Combine(repoRoot, "MetaPipeline", "Cli", "bin", "Debug", "net8.0", "meta-pipeline.exe");
        if (!File.Exists(cliPath))
        {
            throw new FileNotFoundException($"Could not find compiled MetaPipeline CLI at '{cliPath}'. Build MetaPipeline.Cli before running tests.");
        }

        return cliPath;
    }

    private static string FindRepositoryRoot()
    {
        var directory = AppContext.BaseDirectory;
        while (!string.IsNullOrWhiteSpace(directory))
        {
            if (File.Exists(Path.Combine(directory, "README.md")) && Directory.Exists(Path.Combine(directory, "MetaPipeline", "Cli")))
            {
                return directory;
            }

            var parent = Directory.GetParent(directory);
            if (parent == null)
            {
                break;
            }

            directory = parent.FullName;
        }

        throw new InvalidOperationException("Could not locate meta-bi repository root from test base directory.");
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

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
