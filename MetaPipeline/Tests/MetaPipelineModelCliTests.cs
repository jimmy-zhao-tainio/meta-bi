using MetaTransformBinding;
using MetaTransformScript;
using MetaBi.Tests.Common;
using MetaTransformScript.Sql;

namespace MetaPipeline.Tests;

public sealed class MetaPipelineModelCliTests
{
    [Fact]
    public void Help_ShowsInstanceCommands()
    {
        var result = RunCli("--help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("create", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("init", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("add-pipeline", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("add-step", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("add-executable-step", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("execute-worker", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("execute-step", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("add-transform", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create-pipeline-db", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("prune-pipeline-db", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("add-truncate", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("add-opaque-mutation", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("inspect", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExecuteHelp_ShowsModeledExecutionOptions()
    {
        var result = RunCli("execute --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--pipeline", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--pipeline-db-connection-env", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Options:", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("MetaPipeline workspace. Defaults", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Defaults to the current working", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("B/KB/MB/GB rate", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExecuteSqlServerHelp_UsesScriptSelectionOptions()
    {
        var result = RunCli("execute-sqlserver --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--script", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--binding", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Options:", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("B/KB/MB/GB rate", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--transform-script-id", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--transform-binding-id", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkspaceCommands_WhenWorkspaceIsOmitted_UseCurrentDirectory()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaPipeline.Tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(tempRoot, "pipeline");

        try
        {
            Assert.Equal(0, RunCli($"create --xml \"{workspacePath}\"").ExitCode);

            var add = RunCli("add-pipeline --name CwdPipeline", workingDirectory: workspacePath);

            Assert.Equal(0, add.ExitCode);

            var inspect = RunCli("inspect", workingDirectory: workspacePath);

            Assert.True(inspect.ExitCode == 0, inspect.Output);
            Assert.Contains("Pipelines: 1", inspect.Output, StringComparison.Ordinal);
            Assert.Contains("Pipeline: CwdPipeline", inspect.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void ExecuteStepHelp_ShowsTaskGrainExecutionOptions()
    {
        var result = RunCli("execute-step --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--pipeline", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--step-name", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Pipeline task name or id", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("exactly one", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("diagnostic", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("execute-worker", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExecuteWorkerHelp_ShowsPipelinePreservingProtocol()
    {
        var result = RunCli("execute-worker --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--pipeline", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--control-pipe", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("named pipe", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("stdout and stderr are diagnostics only", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("orchestration worker boundary", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("preserves that pipeline context", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("GrantTask", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("StopPipeline", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreatePipelineDbHelp_ShowsOperationalDbBoundary()
    {
        var result = RunCli("create-pipeline-db --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--pipeline-db-connection-env", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--pipeline-db-name", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("MetaPipeline", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("diagnostic logs", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("audit logs", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("metrics", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("failures", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("does not store model truth", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PrunePipelineDbHelp_ShowsRetentionOptions()
    {
        var result = RunCli("prune-pipeline-db --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--pipeline-db-connection-env", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--retention-days", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--dry-run", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("completed", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("RunDiagnosticsLog", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("audit lineage", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("RunLog", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Execute_WithUnavailablePipelineDb_FailsWithNextHelper()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaPipeline.Tests", Guid.NewGuid().ToString("N"));
        var pipelineDbEnv = "META_PIPELINE_TEST_OPERATIONAL_" + Guid.NewGuid().ToString("N").ToUpperInvariant();

        try
        {
            var missingDatabase = "MetaPipelineMissingOperational_" + Guid.NewGuid().ToString("N");
            Environment.SetEnvironmentVariable(
                pipelineDbEnv,
                $"Server=.;Database={missingDatabase};Integrated Security=true;TrustServerCertificate=true;Encrypt=false;Connect Timeout=1");
            Assert.Equal(0, RunCli($"create --xml \"{Path.Combine(tempRoot, "pipeline")}\"").ExitCode);

            var result = RunCli(
                $"execute --workspace \"{Path.Combine(tempRoot, "pipeline")}\" --pipeline CustomerLoad --transform-workspace \"{Path.Combine(tempRoot, "transform")}\" --binding-workspace \"{Path.Combine(tempRoot, "binding")}\" --pipeline-db-connection-env {pipelineDbEnv}");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("MetaPipeline operational DB is not available", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Next:", result.Output, StringComparison.Ordinal);
            Assert.Contains("create-pipeline-db", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(pipelineDbEnv, result.Output, StringComparison.Ordinal);
        }
        finally
        {
            Environment.SetEnvironmentVariable(pipelineDbEnv, null);
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task InitAddPipelineAndAddStep_CreatesSanctionedWorkspace()
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

            var created = RunCli($"create --xml \"{workspacePath}\"");

            Assert.Equal(0, created.ExitCode);
            Assert.Contains("Ok", created.Output, StringComparison.Ordinal);
            Assert.True(File.Exists(Path.Combine(workspacePath, "workspace.meta")));
            Assert.True(File.Exists(Path.Combine(workspacePath, "model.xml")));

            var addPipeline = RunCli($"add-pipeline --workspace \"{workspacePath}\" --name CustomerLoad --description \"Customer load\"");

            Assert.Equal(0, addPipeline.ExitCode);
            Assert.Contains("Ok", addPipeline.Output, StringComparison.Ordinal);

            var add = RunCli(
                $"add-step --workspace \"{workspacePath}\" --pipeline CustomerLoad --step-name load-customers --script \"{transformScript.Name}\" --transform-workspace \"{transformWorkspacePath}\" --binding-workspace \"{bindingWorkspacePath}\" --execution-connection-env {sourceEnv} --target-connection-env {targetEnv} --target dbo.TargetCustomer --batch-size 2 --timeout-seconds 30 --target-data-type-system SqlServer");

            Assert.Equal(0, add.ExitCode);
            Assert.Contains("Ok", add.Output, StringComparison.Ordinal);

            var addSecondTransform = RunCli(
                $"add-step --workspace \"{workspacePath}\" --pipeline CustomerLoad --step-name load-more-customers --script \"{transformScript.Name}\" --transform-workspace \"{transformWorkspacePath}\" --binding-workspace \"{bindingWorkspacePath}\" --execution-connection-env {sourceEnv} --target-connection-env {targetEnv} --target dbo.TargetCustomer");

            Assert.Equal(0, addSecondTransform.ExitCode);
            Assert.Contains("Ok", addSecondTransform.Output, StringComparison.Ordinal);

            var inspect = RunCli($"inspect --workspace \"{workspacePath}\"");

            Assert.True(inspect.ExitCode == 0, inspect.Output);
            Assert.Contains("Tasks: 4", inspect.Output, StringComparison.Ordinal);
            Assert.Contains("RowStreamColumns: 6", inspect.Output, StringComparison.Ordinal);
            Assert.Contains("load-customers [TransformExecution]", inspect.Output, StringComparison.Ordinal);
            Assert.Contains("load-customers.target-write [TargetWrite:InsertRows]", inspect.Output, StringComparison.Ordinal);
            Assert.Contains("load-more-customers [TransformExecution]", inspect.Output, StringComparison.Ordinal);
            Assert.Contains("load-more-customers.target-write [TargetWrite:InsertRows]", inspect.Output, StringComparison.Ordinal);

            var model = global::MetaPipeline.MetaPipelineModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);

            var pipeline = Assert.Single(model.PipelineList);
            Assert.Equal("CustomerLoad", pipeline.Id);
            Assert.Equal("CustomerLoad", pipeline.Name);
            Assert.Equal(4, model.PipelineTaskList.Count);
            Assert.Equal(2, model.ConnectionReferenceList.Count);
            Assert.Equal(2, model.RowStreamList.Count);
            Assert.Equal(6, model.RowStreamColumnList.Count);
            Assert.Equal(2, model.RowStreamProducerList.Count);
            Assert.Equal(2, model.RowStreamConsumerList.Count);
            Assert.Equal(3, model.TaskDependencyList.Count);

            Assert.Contains(model.PipelineTaskList, task => task.Id == "CustomerLoad.load-customers");
            Assert.Contains(model.PipelineTaskList, task => task.Id == "CustomerLoad.load-customers.target-write");
            Assert.Contains(model.PipelineTaskList, task => task.Id == "CustomerLoad.load-more-customers");
            Assert.Contains(model.PipelineTaskList, task => task.Id == "CustomerLoad.load-more-customers.target-write");

            Assert.Equal(2, model.TransformExecutionTaskList.Count);
            var transformExecution = Assert.Single(
                model.TransformExecutionTaskList,
                task => string.Equals(task.PipelineTask.Id, "CustomerLoad.load-customers", StringComparison.Ordinal));
            Assert.Equal(transformScript.Id, transformExecution.TransformScriptId);
            Assert.Equal("binding:customer-load", transformExecution.TransformBindingId);
            Assert.Equal("30", transformExecution.TimeoutSeconds);

            Assert.Equal(2, model.TargetWriteTaskList.Count);
            var targetWrite = Assert.Single(
                model.TargetWriteTaskList,
                task => string.Equals(task.PipelineTask.Id, "CustomerLoad.load-customers.target-write", StringComparison.Ordinal));
            var insertRows = Assert.Single(
                model.InsertRowsTargetWriteTaskList,
                task => string.Equals(task.TargetWriteTask.Id, targetWrite.Id, StringComparison.Ordinal));
            Assert.Equal(targetWrite.Id, insertRows.TargetWriteTask.Id);
            Assert.Equal("dbo.TargetCustomer", insertRows.TargetSqlIdentifier);
            Assert.Equal("2", insertRows.BatchSize);
            Assert.Equal("SqlServer", insertRows.TargetDataTypeSystemName);
            Assert.Collection(
                model.RowStreamColumnList
                    .Where(static item => string.Equals(item.RowStream.Id, "CustomerLoad.load-customers.rows", StringComparison.Ordinal))
                    .OrderBy(static item => int.Parse(item.Ordinal)),
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
                $"execute --workspace \"{workspacePath}\" --pipeline CustomerLoad --transform-workspace \"{transformWorkspacePath}\" --binding-workspace \"{bindingWorkspacePath}\"");

            Assert.Equal(1, execute.ExitCode);
            Assert.Contains(sourceEnv, execute.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("dbo.TargetCustomer", execute.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void AddExecutableStepAndExecute_UsesProcessExitCode()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaPipeline.Tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(tempRoot, "pipeline");
        var cmdExe = ResolveCmdExe();

        try
        {
            Assert.Equal(0, RunCli($"create --xml \"{workspacePath}\"").ExitCode);
            Assert.Equal(0, RunCli($"add-pipeline --workspace \"{workspacePath}\" --name ExecutableSuccess").ExitCode);
            Assert.Equal(0, RunCli(
                $"add-executable-step --workspace \"{workspacePath}\" --pipeline ExecutableSuccess --step-name run-success --executable \"{cmdExe}\" --arguments \"/c exit /b 0\"").ExitCode);

            var successInspect = RunCli($"inspect --workspace \"{workspacePath}\"");

            Assert.Equal(0, successInspect.ExitCode);
            Assert.Contains("run-success [Executable]", successInspect.Output, StringComparison.OrdinalIgnoreCase);

            var success = RunCli($"execute --workspace \"{workspacePath}\" --pipeline ExecutableSuccess");

            Assert.Equal(0, success.ExitCode);

            Assert.Equal(0, RunCli($"add-pipeline --workspace \"{workspacePath}\" --name ExecutableFailure").ExitCode);
            Assert.Equal(0, RunCli(
                $"add-executable-step --workspace \"{workspacePath}\" --pipeline ExecutableFailure --step-name run-failure --executable \"{cmdExe}\" --arguments \"/c echo failure-output & exit /b 7\"").ExitCode);

            var failure = RunCli($"execute --workspace \"{workspacePath}\" --pipeline ExecutableFailure");

            Assert.Equal(4, failure.ExitCode);
            Assert.Contains("exited with code 7", failure.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("failure-output", failure.Output, StringComparison.OrdinalIgnoreCase);

            var model = global::MetaPipeline.MetaPipelineModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            Assert.Equal(2, model.ExecutableTaskList.Count);
            Assert.Contains(model.ExecutableTaskList, task =>
                string.Equals(task.PipelineTask.Name, "run-success", StringComparison.Ordinal)
                && string.Equals(task.ExecutablePath, cmdExe, StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task AddStep_WhenScriptIsNotSelect_CreatesTransformExecutionOnly()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaPipeline.Tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(tempRoot, "pipeline");
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");
        var sourceEnv = "META_PIPELINE_TEST_SOURCE_" + Guid.NewGuid().ToString("N").ToUpperInvariant();

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                "UPDATE dbo.Customer SET Name = 'Acme' WHERE CustomerId = 1",
                null,
                transformWorkspacePath,
                "update-customer");
            var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspacePath, searchUpward: false);
            var transformScript = Assert.Single(transformModel.TransformScriptList);
            BuildBindingWorkspace(
                bindingWorkspacePath,
                transformScript,
                "dbo.Customer",
                [],
                []);

            Assert.Equal(0, RunCli($"create --xml \"{workspacePath}\"").ExitCode);
            Assert.Equal(0, RunCli($"add-pipeline --workspace \"{workspacePath}\" --name CustomerMutation").ExitCode);

            var add = RunCli(
                $"add-step --workspace \"{workspacePath}\" --pipeline CustomerMutation --step-name update-customer --script \"{transformScript.Name}\" --transform-workspace \"{transformWorkspacePath}\" --binding-workspace \"{bindingWorkspacePath}\" --execution-connection-env {sourceEnv}");

            Assert.Equal(0, add.ExitCode);
            Assert.Contains("Ok", add.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("TargetWriteTask", add.Output, StringComparison.Ordinal);

            var model = global::MetaPipeline.MetaPipelineModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);

            Assert.Single(model.PipelineTaskList);
            Assert.Single(model.ConnectionReferenceList);
            var transformExecution = Assert.Single(model.TransformExecutionTaskList);
            Assert.Equal(transformScript.Id, transformExecution.TransformScriptId);
            Assert.Equal("binding:customer-load", transformExecution.TransformBindingId);
            Assert.Empty(model.TargetWriteTaskList);
            Assert.Empty(model.InsertRowsTargetWriteTaskList);
            Assert.Empty(model.RowStreamList);
            Assert.Empty(model.RowStreamColumnList);
            Assert.Empty(model.RowStreamProducerList);
            Assert.Empty(model.RowStreamConsumerList);
            Assert.Empty(model.TaskDependencyList);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Execute_DoesNotPersistRunEvidenceInWorkspaceXml()
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
                "select 1 as CustomerId",
                "dbo.TargetCustomer",
                transformWorkspacePath,
                "dbo.v_customer_load");
            var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspacePath, searchUpward: false);
            var transformScript = Assert.Single(transformModel.TransformScriptList);
            BuildBindingWorkspace(
                bindingWorkspacePath,
                transformScript,
                "dbo.TargetCustomer",
                ["CustomerId"],
                [0]);

            Assert.Equal(0, RunCli($"create --xml \"{workspacePath}\"").ExitCode);
            Assert.Equal(0, RunCli($"add-pipeline --workspace \"{workspacePath}\" --name CustomerLoad").ExitCode);
            Assert.Equal(0, RunCli(
                $"add-step --workspace \"{workspacePath}\" --pipeline CustomerLoad --step-name load-customers --script \"{transformScript.Name}\" --transform-workspace \"{transformWorkspacePath}\" --binding-workspace \"{bindingWorkspacePath}\" --execution-connection-env {sourceEnv} --target-connection-env {targetEnv} --target dbo.TargetCustomer").ExitCode);

            var failingConnection = "Server=.;Database=MetaPipelineMissingDb_" + Guid.NewGuid().ToString("N") + ";Integrated Security=true;TrustServerCertificate=true;Encrypt=false;Connect Timeout=1";
            Environment.SetEnvironmentVariable(sourceEnv, failingConnection);
            Environment.SetEnvironmentVariable(targetEnv, failingConnection);

            var execute = RunCli(
                $"execute --workspace \"{workspacePath}\" --pipeline CustomerLoad --transform-workspace \"{transformWorkspacePath}\" --binding-workspace \"{bindingWorkspacePath}\"");

            Assert.Equal(4, execute.ExitCode);

            var allWorkspaceXml = Directory
                .GetFiles(workspacePath, "*.xml", SearchOption.AllDirectories)
                .Select(File.ReadAllText)
                .ToArray();
            Assert.DoesNotContain(allWorkspaceXml, text => text.Contains("PipelineRun", StringComparison.Ordinal));
            Assert.DoesNotContain(allWorkspaceXml, text => text.Contains("TaskRun", StringComparison.Ordinal));
            Assert.DoesNotContain(allWorkspaceXml, text => text.Contains("RunFingerprint", StringComparison.Ordinal));
        }
        finally
        {
            Environment.SetEnvironmentVariable(sourceEnv, null);
            Environment.SetEnvironmentVariable(targetEnv, null);
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Execute_WhenBindingShapeDriftsFromModeledRowStream_FailsValidationBeforeRuntime()
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
                "select 1 as CustomerId, 'Acme' as CustomerName",
                "dbo.TargetCustomer",
                transformWorkspacePath,
                "dbo.v_customer_load");
            var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspacePath, searchUpward: false);
            var transformScript = Assert.Single(transformModel.TransformScriptList);
            BuildBindingWorkspace(
                bindingWorkspacePath,
                transformScript,
                "dbo.TargetCustomer",
                ["CustomerId", "CustomerName"],
                [0, 1]);

            Assert.Equal(0, RunCli($"create --xml \"{workspacePath}\"").ExitCode);
            Assert.Equal(0, RunCli($"add-pipeline --workspace \"{workspacePath}\" --name CustomerLoad").ExitCode);
            Assert.Equal(0, RunCli(
                $"add-step --workspace \"{workspacePath}\" --pipeline CustomerLoad --step-name load-customers --script \"{transformScript.Name}\" --transform-workspace \"{transformWorkspacePath}\" --binding-workspace \"{bindingWorkspacePath}\" --execution-connection-env {sourceEnv} --target-connection-env {targetEnv} --target dbo.TargetCustomer").ExitCode);

            BuildBindingWorkspace(
                bindingWorkspacePath,
                transformScript,
                "dbo.TargetCustomer",
                ["CustomerId", "CustomerName", "CountryCode"],
                [0, 1, 2]);

            var execute = RunCli(
                $"execute --workspace \"{workspacePath}\" --pipeline CustomerLoad --transform-workspace \"{transformWorkspacePath}\" --binding-workspace \"{bindingWorkspacePath}\"");

            Assert.Equal(4, execute.ExitCode);
            Assert.Contains("no longer matches the resolved binding output shape", execute.Output, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(sourceEnv, execute.Output, StringComparison.Ordinal);
            Assert.DoesNotContain(targetEnv, execute.Output, StringComparison.Ordinal);
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
        var transformBinding = new TransformBinding
        {
            Id = "binding:customer-load",
            MetaTransformScriptTransformScriptId = transformScript.Id,
            TransformScriptName = transformScript.Name,
        };
        model.TransformBindingList.Add(transformBinding);

        var rowset = new Rowset
        {
            Id = "rowset:customer-load",
            TransformBinding = transformBinding,
            DerivationKind = "Output",
            Name = transformScript.Name,
        };
        model.RowsetList.Add(rowset);
        model.OutputRowsetList.Add(new OutputRowset
        {
            Id = "output:customer-load",
            TransformBinding = transformBinding,
            Rowset = rowset,
        });
        model.TransformBindingTargetList.Add(new TransformBindingTarget
        {
            Id = "target:customer-load",
            TransformBinding = transformBinding,
            SqlIdentifier = targetSqlIdentifier,
        });

        for (var index = 0; index < columns.Count; index++)
        {
            model.ColumnList.Add(new Column
            {
                Id = $"column:{index + 1}",
                Rowset = rowset,
                Name = columns[index],
                Ordinal = ordinals[index].ToString(),
            });
        }

        model.SaveToXmlWorkspace(bindingWorkspacePath);
    }

    private static (int ExitCode, string Output) RunCli(string arguments, string? workingDirectory = null) =>
        CliTestRunner.RunStandardCli("MetaPipeline", "meta-pipeline.exe", arguments, workingDirectory: workingDirectory);

    private static string ResolveCmdExe() =>
        Environment.GetEnvironmentVariable("ComSpec") ?? "cmd.exe";

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
