using System.Globalization;
using MetaBi.Tests.Common;
using MetaOrchestration.Core;
using MetaOrchestration.WorkerProtocol;
using MetaPipeline;
using MetaTransform.Binding;
using MetaTransformBinding;
using MetaTransformScript;
using MetaTransformScript.Sql;

namespace MetaOrchestration.Tests;

public sealed class MetaOrchestrationAnalysisServiceTests
{
    [Fact]
    public void CliHelp_ShowsRunPlanExecutionCommand()
    {
        var result = RunCli("--help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("execute", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("add-dependency", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CliExecuteHelp_ShowsWorkspaceAndThrottle()
    {
        var result = RunCli("execute --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--schedule-workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--max-degree-of-parallelism", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--run-artifacts-root", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Options:", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Required. MetaOrchestration workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Refreshes run-plan rows", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("named pipe control channel", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("stops a worker at a blocked task", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("exclusive lease", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("execute-step", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CliExecute_FailsBeforeWorkspaceLoad_WhenPipelineDbConnectionEnvIsMissing()
    {
        var envName = "__META_ORCHESTRATION_TEST_MISSING_" + Guid.NewGuid().ToString("N");

        var result = RunCli(
            "execute " +
            "--workspace NoSuchOrchestration " +
            "--pipeline-workspace NoSuchPipeline " +
            "--transform-workspace NoSuchTransform " +
            "--binding-workspace NoSuchBinding " +
            $"--pipeline-db-connection-env {envName}");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("Cannot execute orchestration.", result.Output, StringComparison.Ordinal);
        Assert.Contains($"Connection environment variable '{envName}' was not found.", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("RunPlan:", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("FirstFailedTask", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("SkippedTask", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void ExecutionLease_PreventsConcurrentExecutionForSameWorkspace()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var workspacePath = Path.Combine(tempRoot, "Orchestration");
            var artifactRoot = Path.Combine(tempRoot, "Runs");
            Directory.CreateDirectory(workspacePath);

            using var lease = OrchestrationWorkspaceExecutionLease.Acquire(workspacePath, Guid.NewGuid(), artifactRoot);

            var ex = Assert.Throws<InvalidOperationException>(
                () => OrchestrationWorkspaceExecutionLease.Acquire(Path.Combine(workspacePath, "."), Guid.NewGuid(), artifactRoot));

            Assert.Contains("already using workspace", ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("RunId=", ex.Message, StringComparison.Ordinal);
            Assert.True(File.Exists(lease.LeaseRecordPath));
            Assert.Empty(Directory.GetFiles(workspacePath, "*", SearchOption.AllDirectories));
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void ExecutionLease_AllowsDifferentWorkspaces()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var firstWorkspacePath = Path.Combine(tempRoot, "First");
            var secondWorkspacePath = Path.Combine(tempRoot, "Second");
            var artifactRoot = Path.Combine(tempRoot, "Runs");
            Directory.CreateDirectory(firstWorkspacePath);
            Directory.CreateDirectory(secondWorkspacePath);

            using var first = OrchestrationWorkspaceExecutionLease.Acquire(firstWorkspacePath, Guid.NewGuid(), artifactRoot);
            using var second = OrchestrationWorkspaceExecutionLease.Acquire(secondWorkspacePath, Guid.NewGuid(), artifactRoot);

            Assert.NotEqual(first.LeaseRecordPath, second.LeaseRecordPath);
            Assert.True(File.Exists(first.LeaseRecordPath));
            Assert.True(File.Exists(second.LeaseRecordPath));
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task RuntimeRejectsRunArtifactsRootInsideOrchestrationWorkspace()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var workspacePath = Path.Combine(tempRoot, "Orchestration");
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => new MetaOrchestrationRuntimeService().ExecuteAsync(
                new OrchestrationRuntimeRequest(
                    workspacePath,
                    Path.Combine(tempRoot, "Pipeline"),
                    Path.Combine(tempRoot, "Transform"),
                    Path.Combine(tempRoot, "Binding"),
                    string.Empty,
                    string.Empty,
                    1,
                    RunArtifactsRootPath: Path.Combine(workspacePath, "Runs"))));

            Assert.Contains("RunArtifactsRootPath", ex.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void RuntimeLivenessRejectsReadyForNonPendingTask()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            OrchestrationRuntimeLiveness.ValidateWorkerEvent(
                "CustomerLoad",
                WorkerEventKinds.TaskReady,
                "CustomerLoad.load",
                new HashSet<string>(StringComparer.Ordinal),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)));

        Assert.Contains("not pending", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("must not send", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RuntimeLivenessRejectsTerminalEventWithoutGrant()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            OrchestrationRuntimeLiveness.ValidateWorkerEvent(
                "CustomerLoad",
                WorkerEventKinds.TaskSucceeded,
                "CustomerLoad.load",
                new HashSet<string>(["CustomerLoad.load"], StringComparer.Ordinal),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)));

        Assert.Contains("no active grant", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RuntimeLivenessRejectsWorkerThatMovesWhileAlreadyReady()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            OrchestrationRuntimeLiveness.ValidateWorkerEvent(
                "CustomerLoad",
                WorkerEventKinds.TaskReady,
                "CustomerLoad.write",
                new HashSet<string>(["CustomerLoad.write"], StringComparer.Ordinal),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["CustomerLoad"] = "CustomerLoad.load"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)));

        Assert.Contains("already waiting", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RuntimeLivenessRejectsReadyWhileGrantIsRunning()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            OrchestrationRuntimeLiveness.ValidateWorkerEvent(
                "CustomerLoad",
                WorkerEventKinds.TaskReady,
                "CustomerLoad.write",
                new HashSet<string>(["CustomerLoad.write"], StringComparer.Ordinal),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["CustomerLoad"] = "CustomerLoad.load"
                }));

        Assert.Contains("still running", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RuntimeLivenessRejectsTerminalEventForWrongGrant()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            OrchestrationRuntimeLiveness.ValidateWorkerEvent(
                "CustomerLoad",
                WorkerEventKinds.TaskFailed,
                "CustomerLoad.write",
                new HashSet<string>(StringComparer.Ordinal),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["CustomerLoad"] = "CustomerLoad.load"
                }));

        Assert.Contains("active grant", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CustomerLoad.load", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeLivenessRejectsUnsupportedWorkerEvent()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            OrchestrationRuntimeLiveness.ValidateWorkerEvent(
                "CustomerLoad",
                "PAUSE",
                "CustomerLoad.load",
                new HashSet<string>(["CustomerLoad.load"], StringComparer.Ordinal),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)));

        Assert.Contains("unsupported event", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RuntimeLivenessAcceptsStartedForMatchingGrant()
    {
        OrchestrationRuntimeLiveness.ValidateWorkerEvent(
            "CustomerLoad",
            WorkerEventKinds.TaskStarted,
            "CustomerLoad.load",
            new HashSet<string>(StringComparer.Ordinal),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CustomerLoad"] = "CustomerLoad.load"
            });
    }

    [Fact]
    public void WorkerProtocolRoundTripsGrantTaskCommandAndTaskStartedEvent()
    {
        var command = new WorkerProtocolCommand(
            WorkerCommandKinds.GrantTask,
            "cmd-1",
            "grant-1",
            "grant-0",
            2,
            "pipeline:CustomerLoad",
            "CustomerLoad",
            "pipeline:CustomerLoad:task:1",
            "retry");
        var commandLine = OrchestrationWorkerProtocol.EncodeCommand(command);

        Assert.True(OrchestrationWorkerProtocol.TryDecodeCommand(commandLine, out var decodedCommand));
        Assert.Equal(command, decodedCommand);

        var workerEvent = new WorkerProtocolEvent(
            WorkerEventKinds.TaskStarted,
            "worker-1",
            "pipeline:CustomerLoad",
            "CustomerLoad",
            "pipeline:CustomerLoad:task:1",
            "load",
            "grant-1",
            "cmd-1",
            2,
            0,
            "test-worker",
            "started");
        var eventLine = OrchestrationWorkerProtocol.EncodeEvent(workerEvent);

        Assert.True(OrchestrationWorkerProtocol.TryDecodeEvent(eventLine, out var decodedEvent));
        Assert.Equal(workerEvent, decodedEvent);
    }

    [Fact]
    public void RetryPolicyRetriesRetrySafeTransientFailuresOnlyWithinBudget()
    {
        var policy = new ResolvedOrchestrationRetryPolicy(
            "retry-policy:test",
            "TestRetryPolicy",
            MaxAttempts: 3,
            InitialDelayMilliseconds: 100,
            MaxDelayMilliseconds: 1000,
            BackoffMultiplier: 2,
            RetryReadOnlyTasksByDefault: true,
            RetryWriteTasksByDefault: false,
            RetryableFailureClasses: [OrchestrationRetryFailureClasses.TransientSql]);

        var first = policy.Evaluate(new OrchestrationRetryEvaluationContext(
            "task-1",
            AttemptNumber: 1,
            OrchestrationRetryFailureClasses.TransientSql,
            IsTaskRetrySafe: true,
            ExitCode: 4,
            FailureMessage: "deadlock"));
        Assert.True(first.ShouldRetry);
        Assert.Equal(2, first.NextAttemptNumber);
        Assert.Equal(TimeSpan.FromMilliseconds(100), first.Delay);

        var exhausted = policy.Evaluate(new OrchestrationRetryEvaluationContext(
            "task-1",
            AttemptNumber: 3,
            OrchestrationRetryFailureClasses.TransientSql,
            IsTaskRetrySafe: true,
            ExitCode: 4,
            FailureMessage: "deadlock"));
        Assert.False(exhausted.ShouldRetry);
        Assert.Contains("exhausted", exhausted.Reason, StringComparison.OrdinalIgnoreCase);

        var unsafeTask = policy.Evaluate(new OrchestrationRetryEvaluationContext(
            "task-1",
            AttemptNumber: 1,
            OrchestrationRetryFailureClasses.TransientSql,
            IsTaskRetrySafe: false,
            ExitCode: 4,
            FailureMessage: "deadlock"));
        Assert.False(unsafeTask.ShouldRetry);
        Assert.Contains("not retry-safe", unsafeTask.Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RunPlan_ModelsDefaultRetryPolicy()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "LoadA",
                    Task("LoadA", 1, "load-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

        var result = new MetaOrchestrationRunPlanningService().BuildRunPlan(model);

        Assert.Equal("Ready", result.Status);
        var retryPolicy = Assert.Single(model.RetryPolicyList);
        Assert.Equal("DefaultRetryPolicy", retryPolicy.Name);
        Assert.Equal("3", retryPolicy.MaxAttempts);
        Assert.Equal("true", retryPolicy.RetryReadOnlyTasksByDefault);
        Assert.Equal("false", retryPolicy.RetryWriteTasksByDefault);

        var assignment = Assert.Single(model.RunPlanRetryPolicyList);
        Assert.Same(retryPolicy, assignment.RetryPolicy);
        Assert.Same(Assert.Single(model.RunPlanList), assignment.RunPlan);
        Assert.Equal("Default", assignment.PolicyRole);

        Assert.Contains(
            model.RetryPolicyFailureClassList,
            item => ReferenceEquals(item.RetryPolicy, retryPolicy) &&
                    item.FailureClass == OrchestrationRetryFailureClasses.TransientSql &&
                    item.RetryBehavior == "Retry");

        var resolved = ResolvedOrchestrationRetryPolicy.FromRunPlan(model, assignment.RunPlan);
        Assert.Equal("DefaultRetryPolicy", resolved.Name);
        Assert.Contains(OrchestrationRetryFailureClasses.TransientSql, resolved.RetryableFailureClasses);

        var tempRoot = CreateTempRoot();
        try
        {
            var workspace = Path.Combine(tempRoot, "Orchestration");
            model.SaveToXmlWorkspace(workspace);

            var reloaded = MetaOrchestrationModel.LoadFromXmlWorkspace(workspace, searchUpward: false);
            var reloadedRunPlan = Assert.Single(reloaded.RunPlanList);
            var reloadedResolved = ResolvedOrchestrationRetryPolicy.FromRunPlan(reloaded, reloadedRunPlan);
            Assert.Equal("DefaultRetryPolicy", reloadedResolved.Name);
            Assert.Contains(OrchestrationRetryFailureClasses.TransientSql, reloadedResolved.RetryableFailureClasses);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void DiagnosticLogBufferEnforcesLineAndByteBudgets()
    {
        var buffer = new OrchestrationDiagnosticLogBuffer(new OrchestrationLogCapturePolicy(
            MaxLineLength: 8,
            MaxBytesPerWorkerStream: 80));

        buffer.AppendLine("12345678901234567890");
        buffer.AppendLine("second");
        buffer.AppendLine(new string('x', 200));

        var text = buffer.ToString();
        Assert.Contains("[line truncated]", text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("diagnostics truncated", text, StringComparison.OrdinalIgnoreCase);
        Assert.True(buffer.WasTruncated);
        Assert.True(buffer.DroppedBytes > 0);
    }

    [Fact]
    public void DiagnosticLogBufferWritesBoundedArtifact()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var artifactPath = Path.Combine(tempRoot, "logs", "worker.stdout.log");
            var buffer = new OrchestrationDiagnosticLogBuffer(
                new OrchestrationLogCapturePolicy(
                    MaxLineLength: 8,
                    MaxBytesPerWorkerStream: 48),
                artifactPath);

            buffer.AppendLine("abcdefghijk");
            buffer.AppendLine("second");
            buffer.AppendLine(new string('z', 200));

            Assert.True(File.Exists(artifactPath));
            var artifact = File.ReadAllText(artifactPath);
            Assert.Contains("abcdefgh", artifact, StringComparison.Ordinal);
            Assert.Contains("[line truncated]", artifact, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("second", artifact, StringComparison.Ordinal);
            Assert.DoesNotContain(new string('z', 8), artifact, StringComparison.Ordinal);
            Assert.True(buffer.WasTruncated);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task RuntimeFailsFastWhenWorkerExitsBeforeReady()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var model = CreateModel(
                AnalyzeProfiles(
                    Profile(
                        "CustomerLoad",
                        Task(
                            "CustomerLoad",
                            1,
                            "load",
                            "Select",
                            Access("dbo.Source", OrchestrationObjectAccessKind.Read, "Source"),
                            Access("dbo.Target", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                ExecuteWithFakePipelineWorkerAsync(
                    tempRoot,
                    model,
                    """
                    return
                    """));

            Assert.Contains("CustomerLoad", ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("exited before all", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task RuntimeFailsFastOnWorkerVersionMismatch()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var model = CreateModel(
                AnalyzeProfiles(
                    Profile(
                        "CustomerLoad",
                        Task(
                            "CustomerLoad",
                            1,
                            "load",
                            "Select",
                            Access("dbo.Source", OrchestrationObjectAccessKind.Read, "Source"),
                            Access("dbo.Target", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                ExecuteWithFakePipelineWorkerAsync(
                    tempRoot,
                    model,
                    """
                    Send-WorkerEvent -Kind 'WorkerOnline' -Version 'wrong-version' -Message 'online'
                    return
                    """));

            Assert.Contains("version mismatch", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task RuntimeRetriesRetrySafeTaskAfterTaskFailed()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var model = CreateModel(
                AnalyzeProfiles(
                    Profile(
                        "CustomerLoad",
                        Task(
                            "CustomerLoad",
                            1,
                            "load",
                            "Select",
                            Access("dbo.Source", OrchestrationObjectAccessKind.Read, "Source")))));
            AddTestRetryPolicy(model, maxAttempts: 2, retryWrites: false);

            var result = await ExecuteWithFakePipelineWorkerAsync(
                tempRoot,
                model,
                """
                $taskId = "pipeline:${pipeline}:task:1"
                Send-WorkerEvent -Kind 'WorkerOnline' -Message 'online'
                Send-WorkerEvent -Kind 'WorkerReady' -Message 'ready'
                $startCommand = Read-StartPipelineCommand
                Send-WorkerEvent -Kind 'PipelineStarted' -Message 'started'
                Send-WorkerEvent -Kind 'TaskReady' -TaskId $taskId -TaskName 'load' -GrantId '' -CommandId '' -Message 'ready'
                $command = Read-WorkerCommand
                Send-WorkerEvent -Kind 'GrantAccepted' -TaskId $taskId -TaskName 'load' -GrantId 'grant-1' -CommandId 'command-1' -Attempt 1 -Message 'accepted'
                Send-WorkerEvent -Kind 'TaskStarted' -TaskId $taskId -TaskName 'load' -GrantId 'grant-1' -CommandId 'command-1' -Attempt 1 -Message 'started'
                Send-WorkerEvent -Kind 'TaskFailed' -TaskId $taskId -TaskName 'load' -GrantId 'grant-1' -CommandId 'command-1' -Attempt 1 -ExitCode 4 -Message 'transient' -FailureClass 'WorkerReportedRetryable'
                $command = Read-WorkerCommand
                Send-WorkerEvent -Kind 'GrantAccepted' -TaskId $taskId -TaskName 'load' -GrantId 'grant-2' -CommandId 'command-2' -Attempt 2 -Message 'accepted'
                Send-WorkerEvent -Kind 'TaskStarted' -TaskId $taskId -TaskName 'load' -GrantId 'grant-2' -CommandId 'command-2' -Attempt 2 -Message 'started'
                Send-WorkerEvent -Kind 'TaskSucceeded' -TaskId $taskId -TaskName 'load' -GrantId 'grant-2' -CommandId 'command-2' -Attempt 2 -Message 'completed'
                return
                """);

            Assert.True(result.Succeeded);
            Assert.Equal(2, result.TaskResults.Count);
            Assert.Contains(result.TaskResults, item => item.ExitCode == 4 && item.AttemptNumber == 1);
            Assert.Contains(result.TaskResults, item => item.ExitCode == 0 && item.AttemptNumber == 2);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task RuntimeFailsFastWhenWorkerNeverEmitsOnline()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var model = CreateModel(
                AnalyzeProfiles(
                    Profile(
                        "CustomerLoad",
                        Task(
                            "CustomerLoad",
                            1,
                            "load",
                            "Select",
                            Access("dbo.Source", OrchestrationObjectAccessKind.Read, "Source"),
                            Access("dbo.Target", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                ExecuteWithFakePipelineWorkerAsync(
                    tempRoot,
                    model,
                    """
                    Start-Sleep -Seconds 5
                    return
                    """,
                    TimeSpan.FromMilliseconds(150)));

            Assert.Contains("stopped responding", ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(WorkerEventKinds.WorkerOnline, ex.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task RuntimeFailsFastWhenWorkerDoesNotStartPipelineAfterActivationCommand()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var model = CreateModel(
                AnalyzeProfiles(
                    Profile(
                        "CustomerLoad",
                        Task(
                            "CustomerLoad",
                            1,
                            "load",
                            "Select",
                            Access("dbo.Source", OrchestrationObjectAccessKind.Read, "Source"),
                            Access("dbo.Target", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                ExecuteWithFakePipelineWorkerAsync(
                    tempRoot,
                    model,
                    """
                    Send-WorkerEvent -Kind 'WorkerOnline' -Message 'online'
                    Send-WorkerEvent -Kind 'WorkerReady' -Message 'ready'
                    $startCommand = Read-StartPipelineCommand
                    Start-Sleep -Seconds 5
                    return
                    """,
                    TimeSpan.FromMilliseconds(150)));

            Assert.Contains("stopped responding", ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(WorkerEventKinds.PipelineStarted, ex.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task RuntimeMarksRunningTaskFailedWhenWorkerStopsRespondingDuringGrant()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var model = CreateModel(
                AnalyzeProfiles(
                    Profile(
                        "CustomerLoad",
                        Task(
                            "CustomerLoad",
                            1,
                            "load",
                            "Select",
                            Access("dbo.Source", OrchestrationObjectAccessKind.Read, "Source")))));

            var result = await ExecuteWithFakePipelineWorkerAsync(
                tempRoot,
                model,
                """
                $taskId = "pipeline:${pipeline}:task:1"
                Send-WorkerEvent -Kind 'WorkerOnline' -Message 'online'
                Send-WorkerEvent -Kind 'WorkerReady' -Message 'ready'
                $startCommand = Read-StartPipelineCommand
                Send-WorkerEvent -Kind 'PipelineStarted' -Message 'started'
                Send-WorkerEvent -Kind 'TaskReady' -TaskId $taskId -TaskName 'load' -GrantId '' -CommandId '' -Message 'ready'
                $command = Read-WorkerCommand
                Send-WorkerEvent -Kind 'GrantAccepted' -TaskId $taskId -TaskName 'load' -GrantId 'grant' -CommandId 'command' -Attempt 1 -Message 'accepted'
                Send-WorkerEvent -Kind 'TaskStarted' -TaskId $taskId -TaskName 'load' -GrantId 'grant' -CommandId 'command' -Attempt 1 -Message 'started'
                Start-Sleep -Seconds 5
                return
                """,
                TimeSpan.FromMilliseconds(150));

            Assert.False(result.Succeeded);
            var taskResult = Assert.Single(result.TaskResults);
            Assert.Equal(4, taskResult.ExitCode);
            Assert.Contains("active task outcome is unknown", taskResult.StandardError, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task RuntimeFailsFastWhenWorkerExitsAfterReadyBeforeGrant()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var model = CreateModel(
                AnalyzeProfiles(
                    Profile(
                        "Producer",
                        Task(
                            "Producer",
                            1,
                            "load",
                            "Select",
                            Access("dbo.Raw", OrchestrationObjectAccessKind.Read, "Source"),
                            Access("dbo.Stage", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                    Profile(
                        "Consumer",
                        Task(
                            "Consumer",
                            1,
                            "load",
                            "Select",
                            Access("dbo.Stage", OrchestrationObjectAccessKind.Read, "Source"),
                            Access("dbo.Mart", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                ExecuteWithFakePipelineWorkerAsync(
                    tempRoot,
                    model,
                    """
                    $taskId = "pipeline:${pipeline}:task:1"
                    Send-WorkerEvent -Kind 'WorkerOnline' -Message 'online'
                    Send-WorkerEvent -Kind 'WorkerReady' -Message 'ready'
                    $startCommand = Read-StartPipelineCommand
                    Send-WorkerEvent -Kind 'PipelineStarted' -Message 'started'
                    Send-WorkerEvent -Kind 'TaskReady' -TaskId $taskId -TaskName 'load' -GrantId '' -CommandId '' -Message 'ready'
                    if ($pipeline -eq 'Consumer') { return }
                    $command = Read-WorkerCommand
                    Send-WorkerEvent -Kind 'GrantAccepted' -TaskId $taskId -TaskName 'load' -GrantId 'grant' -CommandId 'command' -Attempt 1 -Message 'accepted'
                    Send-WorkerEvent -Kind 'TaskStarted' -TaskId $taskId -TaskName 'load' -GrantId 'grant' -CommandId 'command' -Attempt 1 -Message 'started'
                    Start-Sleep -Milliseconds 500
                    Send-WorkerEvent -Kind 'TaskSucceeded' -TaskId $taskId -TaskName 'load' -GrantId 'grant' -CommandId 'command' -Attempt 1 -Message 'completed'
                    return
                    """));

            Assert.Contains("Consumer", ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.True(
                ex.Message.Contains("exited before all", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("Cannot send GrantTask", StringComparison.OrdinalIgnoreCase),
                ex.Message);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task RuntimeFailsFastOnMalformedWorkerProtocolEvent()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var model = CreateModel(
                AnalyzeProfiles(
                    Profile(
                        "CustomerLoad",
                        Task(
                            "CustomerLoad",
                            1,
                            "load",
                            "Select",
                            Access("dbo.Source", OrchestrationObjectAccessKind.Read, "Source"),
                            Access("dbo.Target", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                ExecuteWithFakePipelineWorkerAsync(
                    tempRoot,
                    model,
                    """
                    Send-RawWorkerLine "META_PIPELINE_WORKER`tREADY`ttoo-few"
                    return
                    """));

            Assert.Contains("malformed protocol event", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void CliAddDependencyHelp_ShowsConditionalDependencyShape()
    {
        var result = RunCli("add-dependency --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--condition success|failure", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Options:", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Failure edges", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--from-task", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CliWorkspaceCreationHelp_UsesNewWorkspaceForInferenceOnly()
    {
        var help = RunCli("--help");
        var runPlan = RunCli("refresh-run-plan --help");

        Assert.Equal(0, help.ExitCode);
        Assert.Equal(0, runPlan.ExitCode);
        Assert.Contains("--new-workspace", help.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--new-workspace", runPlan.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--plan", help.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--schedule", runPlan.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--out", help.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--out", runPlan.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Analyze_InferProducerConsumerDependency_WhenTargetIsReadByAnotherPipeline()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("load-stage", "SELECT CustomerId FROM dbo.RawCustomer", "dbo.StageCustomer"),
                ("load-dim", "SELECT CustomerId FROM dbo.StageCustomer", "dbo.DimCustomer"));
            BuildBindingWorkspace(
                bindingWorkspace,
                (ResolveScript(transformModel, "load-stage"), ["dbo.RawCustomer"], "dbo.StageCustomer"),
                (ResolveScript(transformModel, "load-dim"), ["dbo.StageCustomer"], "dbo.DimCustomer"));
            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "StageCustomer", Script: ResolveScript(transformModel, "load-stage"), InsertRowsTarget: "dbo.StageCustomer"),
                (PipelineName: "DimCustomer", Script: ResolveScript(transformModel, "load-dim"), InsertRowsTarget: "dbo.DimCustomer"));

            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            Assert.True(result.IsCompleteDag);
            var edge = Assert.Single(result.Dependencies);
            Assert.Equal("pipeline:StageCustomer", edge.PredecessorPipelineId);
            Assert.Equal("pipeline:DimCustomer", edge.SuccessorPipelineId);
            Assert.Empty(result.Issues);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Analyze_AllowsScalarFunctionDefinitionsInTransformWorkspace_WhenPipelineUsesView()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("dbo.fnCustomerOrderCount", """
CREATE FUNCTION dbo.fnCustomerOrderCount
(
    @CustomerId INT
)
RETURNS BIGINT
AS
BEGIN
    RETURN
    (
        SELECT COUNT_BIG(*)
        FROM dbo.[Order] AS o
        WHERE o.CustomerId = @CustomerId
    );
END
""", null),
                ("load-orders", "SELECT CustomerId FROM dbo.RawOrder", "dbo.Order"),
                ("dbo.v_LoadStage", """
CREATE VIEW dbo.v_LoadStage AS
SELECT
    s.CustomerId,
    dbo.fnCustomerOrderCount(s.CustomerId) AS OrderCount
FROM dbo.Source AS s
""", "dbo.StageCustomer"));

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspace,
                bindingWorkspace);
            Assert.Equal(0, bindingResult.ErrorCount);
            Assert.Contains(
                bindingResult.Model.TransformBindingList,
                item => string.Equals(item.TransformScriptName, "dbo.fnCustomerOrderCount", StringComparison.OrdinalIgnoreCase));

            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "Orders", Script: ResolveScript(transformModel, "load-orders"), InsertRowsTarget: "dbo.Order"),
                (PipelineName: "StageCustomer", Script: ResolveScript(transformModel, "dbo.v_LoadStage"), InsertRowsTarget: "dbo.StageCustomer"));

            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            Assert.True(result.IsCompleteDag);
            Assert.Empty(result.Issues);
            var dependency = Assert.Single(result.Dependencies);
            Assert.Equal("pipeline:Orders", dependency.PredecessorPipelineId);
            Assert.Equal("pipeline:StageCustomer", dependency.SuccessorPipelineId);

            var pipeline = Assert.Single(result.Pipelines, item => string.Equals(item.PipelineName, "StageCustomer", StringComparison.OrdinalIgnoreCase));
            var transformTask = Assert.Single(pipeline.Tasks, item => string.Equals(item.TransformScriptName, "dbo.v_LoadStage", StringComparison.OrdinalIgnoreCase));
            Assert.Equal(BoundStatementKind.Select.ToString(), transformTask.StatementKind);
            Assert.Contains(transformTask.ObjectAccesses, item =>
                string.Equals(item.SqlIdentifier, "dbo.Source", StringComparison.OrdinalIgnoreCase) &&
                item.AccessKind == OrchestrationObjectAccessKind.Read);
            Assert.Contains(transformTask.ObjectAccesses, item =>
                string.Equals(item.SqlIdentifier, "dbo.Order", StringComparison.OrdinalIgnoreCase) &&
                item.AccessKind == OrchestrationObjectAccessKind.Read);
            Assert.Contains(transformTask.ObjectAccesses, item =>
                string.Equals(item.SqlIdentifier, "dbo.StageCustomer", StringComparison.OrdinalIgnoreCase) &&
                item.AccessKind == OrchestrationObjectAccessKind.Write);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Analyze_BlocksScalarFunctionDefinitionsAsPipelineTasks_WithSpecificIssue()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("dbo.fnAddOne", """
CREATE FUNCTION dbo.fnAddOne
(
    @value INT
)
RETURNS INT
AS
BEGIN
    RETURN @value + 1;
END
""", null));
            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspace,
                bindingWorkspace);
            Assert.Equal(0, bindingResult.ErrorCount);

            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "ScalarFunctionTask", Script: ResolveScript(transformModel, "dbo.fnAddOne"), InsertRowsTarget: null));

            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            Assert.False(result.IsCompleteDag);
            var issue = Assert.Single(result.Issues);
            Assert.Equal(OrchestrationIssueCode.NonExecutableTransformScript, issue.Code);
            Assert.True(issue.BlocksDag);
            Assert.True(issue.BlocksAutomaticRunPlanning);
            Assert.Contains("scalar function transform script 'dbo.fnAddOne'", issue.Message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("unsupported for binding-driven orchestration", issue.Message, StringComparison.OrdinalIgnoreCase);

            var task = Assert.Single(Assert.Single(result.Pipelines).Tasks);
            Assert.Equal(BoundStatementKind.ScalarFunction.ToString(), task.StatementKind);
            Assert.Empty(task.ObjectAccesses);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Theory]
    [InlineData("10:Reset,20:Append", OrchestrationWriteEffect.Replace)]
    [InlineData("10:Append,20:Reset", OrchestrationWriteEffect.ResetOnly)]
    public async Task Analyze_StoredProcedureContractOperations_PreserveInternalOrder(
        string operationOrder,
        OrchestrationWriteEffect expectedWriteEffect)
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("dq.RefreshStage", """
CREATE PROCEDURE dq.RefreshStage
AS
BEGIN
    SELECT 1 AS Marker;
END
""", null));
            AddStoredProcedureContractOperations(
                transformModel,
                "dq.RefreshStage",
                operationOrder.Split(',').Select(ParseOperationSeed).ToArray());
            transformModel.SaveToXmlWorkspace(transformWorkspace);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspace,
                bindingWorkspace);
            Assert.Equal(0, bindingResult.ErrorCount);

            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "RefreshStage", Script: ResolveScript(transformModel, "dq.RefreshStage"), InsertRowsTarget: null));

            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            var effect = Assert.Single(result.TaskObjectEffects, item =>
                string.Equals(item.SqlIdentifier, "dbo.Stage", StringComparison.OrdinalIgnoreCase));
            Assert.Equal(expectedWriteEffect, effect.WriteEffect);

            var task = Assert.Single(Assert.Single(result.Pipelines).Tasks);
            var accesses = task.ObjectAccesses
                .Where(item => string.Equals(item.SqlIdentifier, "dbo.Stage", StringComparison.OrdinalIgnoreCase))
                .OrderBy(static item => item.Ordinal)
                .ToArray();
            Assert.Equal(
                operationOrder.Split(',').Select(item => item.Split(':')[1]).ToArray(),
                accesses.Select(static item => item.OperationKind).ToArray());
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Analyze_StoredProcedureResultRowset_InsertRowsTargetContributesWriteDependency()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("dq.ExportCustomers", """
CREATE PROCEDURE dq.ExportCustomers
AS
BEGIN
    SELECT CustomerId FROM src.Customer;
END
""", null),
                ("read-export", "SELECT CustomerId FROM stg.CustomerExport", "mart.Customer"));
            AddStoredProcedureContractOperations(
                transformModel,
                "dq.ExportCustomers",
                [Operation(10, "Read", "src.Customer", "Source")]);
            AddStoredProcedureResultRowset(
                transformModel,
                "dq.ExportCustomers",
                ["CustomerId"]);
            transformModel.SaveToXmlWorkspace(transformWorkspace);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspace,
                bindingWorkspace);
            Assert.Equal(0, bindingResult.ErrorCount);

            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "ExportCustomers", Script: ResolveScript(transformModel, "dq.ExportCustomers"), InsertRowsTarget: "stg.CustomerExport"),
                (PipelineName: "ReadExport", Script: ResolveScript(transformModel, "read-export"), InsertRowsTarget: "mart.Customer"));

            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            Assert.True(result.IsCompleteDag);
            var dependency = Assert.Single(result.Dependencies);
            Assert.Equal("pipeline:ExportCustomers", dependency.PredecessorPipelineId);
            Assert.Equal("pipeline:ReadExport", dependency.SuccessorPipelineId);

            var exportTask = Assert.Single(
                Assert.Single(result.Pipelines, item => string.Equals(item.PipelineName, "ExportCustomers", StringComparison.OrdinalIgnoreCase)).Tasks);
            Assert.Contains(exportTask.ObjectAccesses, item =>
                string.Equals(item.SqlIdentifier, "stg.CustomerExport", StringComparison.OrdinalIgnoreCase) &&
                item.AccessKind == OrchestrationObjectAccessKind.Write &&
                string.Equals(item.AccessRole, "InsertRowsTarget", StringComparison.Ordinal));
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Analyze_ComplexLayeredStoredProcedureGraph_InferExpectedDagEffectsAndPersistedOperations()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");
            var orchestrationWorkspace = Path.Combine(tempRoot, "Orchestration");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("extract-customers", "SELECT CustomerId, CountryCode FROM src.Customer", "stg.Customer"),
                ("extract-orders", "SELECT OrderId, CustomerId FROM src.SalesOrder", "stg.SalesOrder"),
                ("dbo.RefreshCountry", """
CREATE PROCEDURE dbo.RefreshCountry
AS
BEGIN
    SELECT 1 AS Marker;
END
""", null),
                ("dbo.CurateCustomer", """
CREATE PROCEDURE dbo.CurateCustomer
AS
BEGIN
    SELECT 1 AS Marker;
END
""", null),
                ("build-dim-customer", "SELECT CustomerId, CountryCode FROM core.Customer", "dw.DimCustomer"),
                ("build-fact-order", """
SELECT
    o.OrderId,
    c.CustomerId
FROM stg.SalesOrder AS o
INNER JOIN dw.DimCustomer AS c
    ON c.CustomerId = o.CustomerId
""", "dw.FactOrder"),
                ("publish-sales", """
SELECT
    f.OrderId,
    d.CustomerId
FROM dw.FactOrder AS f
INNER JOIN dw.DimCustomer AS d
    ON d.CustomerId = f.CustomerId
""", "mart.Sales"));

            AddStoredProcedureContractOperations(
                transformModel,
                "dbo.RefreshCountry",
                [
                    Operation(10, "Read", "src.Country", "Source"),
                    Operation(20, "Reset", "ref.Country"),
                    Operation(30, "Append", "ref.Country"),
                    Operation(40, "Call", "audit.MarkCountryRefresh")
                ]);
            AddStoredProcedureContractOperations(
                transformModel,
                "dbo.CurateCustomer",
                [
                    Operation(10, "Read", "stg.Customer", "Source"),
                    Operation(20, "Read", "ref.Country", "Lookup"),
                    Operation(30, "Reset", "core.Customer"),
                    Operation(40, "Append", "core.Customer"),
                    Operation(50, "Mutation", "audit.CustomerLoadLog")
                ]);
            transformModel.SaveToXmlWorkspace(transformWorkspace);

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspace,
                bindingWorkspace);
            Assert.Equal(0, bindingResult.ErrorCount);

            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "ExtractCustomers", Script: ResolveScript(transformModel, "extract-customers"), InsertRowsTarget: "stg.Customer"),
                (PipelineName: "ExtractOrders", Script: ResolveScript(transformModel, "extract-orders"), InsertRowsTarget: "stg.SalesOrder"),
                (PipelineName: "RefreshCountry", Script: ResolveScript(transformModel, "dbo.RefreshCountry"), InsertRowsTarget: null),
                (PipelineName: "CurateCustomer", Script: ResolveScript(transformModel, "dbo.CurateCustomer"), InsertRowsTarget: null),
                (PipelineName: "BuildDimCustomer", Script: ResolveScript(transformModel, "build-dim-customer"), InsertRowsTarget: "dw.DimCustomer"),
                (PipelineName: "BuildFactOrder", Script: ResolveScript(transformModel, "build-fact-order"), InsertRowsTarget: "dw.FactOrder"),
                (PipelineName: "PublishSales", Script: ResolveScript(transformModel, "publish-sales"), InsertRowsTarget: "mart.Sales"));

            var service = new MetaOrchestrationAnalysisService();
            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            Assert.True(result.IsCompleteDag);
            Assert.Equal("Complete", result.DagStatus);
            Assert.Equal("Deterministic", result.DeterminismStatus);
            Assert.Equal("Complete", result.SynchronizationStatus);
            Assert.Empty(result.Issues);
            Assert.Equal(
                new[]
                {
                    "pipeline:BuildDimCustomer->pipeline:BuildFactOrder",
                    "pipeline:BuildDimCustomer->pipeline:PublishSales",
                    "pipeline:BuildFactOrder->pipeline:PublishSales",
                    "pipeline:CurateCustomer->pipeline:BuildDimCustomer",
                    "pipeline:ExtractCustomers->pipeline:CurateCustomer",
                    "pipeline:ExtractOrders->pipeline:BuildFactOrder",
                    "pipeline:RefreshCountry->pipeline:CurateCustomer"
                },
                result.Dependencies
                    .Select(static item => $"{item.PredecessorPipelineId}->{item.SuccessorPipelineId}")
                    .OrderBy(static item => item, StringComparer.Ordinal)
                    .ToArray());

            AssertTaskObjectEffect(
                result,
                "RefreshCountry",
                "ref.Country",
                OrchestrationAccessDirection.Write,
                OrchestrationWriteEffect.Replace,
                OrchestrationAccessPurpose.TargetLoad);
            AssertTaskObjectEffect(
                result,
                "CurateCustomer",
                "core.Customer",
                OrchestrationAccessDirection.Write,
                OrchestrationWriteEffect.Replace,
                OrchestrationAccessPurpose.TargetLoad);
            AssertTaskObjectEffect(
                result,
                "CurateCustomer",
                "ref.Country",
                OrchestrationAccessDirection.Read,
                OrchestrationWriteEffect.None,
                OrchestrationAccessPurpose.Lookup);
            AssertTaskObjectEffect(
                result,
                "CurateCustomer",
                "audit.CustomerLoadLog",
                OrchestrationAccessDirection.ReadWrite,
                OrchestrationWriteEffect.Mutation,
                OrchestrationAccessPurpose.TargetMutation);

            var curateTask = Assert.Single(Assert.Single(result.Pipelines, item =>
                string.Equals(item.PipelineName, "CurateCustomer", StringComparison.OrdinalIgnoreCase)).Tasks);
            Assert.Equal(
                new[]
                {
                    "10:Read:stg.Customer:Read",
                    "20:Read:ref.Country:Read",
                    "30:Reset:core.Customer:ResetWrite",
                    "40:Append:core.Customer:Write",
                    "50:Mutation:audit.CustomerLoadLog:ReadWrite"
                },
                curateTask.ObjectAccesses
                    .OrderBy(static item => item.Ordinal)
                    .Select(static item => $"{item.Ordinal}:{item.OperationKind}:{item.SqlIdentifier}:{item.AccessKind}")
                    .ToArray());

            var orchestrationModel = service.CreateModel(result, pipelineWorkspace);
            orchestrationModel.SaveToXmlWorkspace(orchestrationWorkspace);
            var reloaded = MetaOrchestrationModel.LoadFromXmlWorkspace(orchestrationWorkspace, searchUpward: false);
            var persistedCurateTask = Assert.Single(reloaded.TaskAccessProfileList, item =>
                string.Equals(item.TransformScriptName, "dbo.CurateCustomer", StringComparison.OrdinalIgnoreCase));
            Assert.Equal(
                new[]
                {
                    "10:Read:stg.Customer:Read",
                    "20:Read:ref.Country:Read",
                    "30:Reset:core.Customer:ResetWrite",
                    "40:Append:core.Customer:Write",
                    "50:Mutation:audit.CustomerLoadLog:ReadWrite"
                },
                reloaded.ObjectAccessList
                    .Where(item => string.Equals(item.TaskAccessProfile.Id, persistedCurateTask.Id, StringComparison.Ordinal))
                    .OrderBy(static item => int.Parse(item.Ordinal))
                    .Select(static item => $"{item.Ordinal}:{item.OperationKind}:{item.DataObject.SqlIdentifier}:{item.AccessKind}")
                    .ToArray());
            Assert.Equal("Complete", Assert.Single(reloaded.OrchestrationPlanList).DagStatus);
            Assert.Equal(7, reloaded.PipelineDependencyList.Count);
            Assert.Empty(reloaded.DependencyIssueList);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Analyze_DoesNotBlockPrivateResetWrite()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("truncate-private", "TRUNCATE TABLE dbo.PrivateScratch", null));
            BuildBindingWorkspace(
                bindingWorkspace,
                (ResolveScript(transformModel, "truncate-private"), [], "dbo.PrivateScratch"));
            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "CleanupScratch", Script: ResolveScript(transformModel, "truncate-private"), InsertRowsTarget: null));

            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            Assert.True(result.IsCompleteDag);
            Assert.Empty(result.Dependencies);
            Assert.Empty(result.Issues);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Analyze_BlocksResetWrite_WhenAnotherPipelineTouchesSameObject()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");
            var orchestrationWorkspace = Path.Combine(tempRoot, "Orchestration");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("truncate-stage", "TRUNCATE TABLE dbo.StageCustomer", null),
                ("read-stage", "SELECT CustomerId FROM dbo.StageCustomer", "dbo.DimCustomer"));
            BuildBindingWorkspace(
                bindingWorkspace,
                (ResolveScript(transformModel, "truncate-stage"), [], "dbo.StageCustomer"),
                (ResolveScript(transformModel, "read-stage"), ["dbo.StageCustomer"], "dbo.DimCustomer"));
            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "TruncateStage", Script: ResolveScript(transformModel, "truncate-stage"), InsertRowsTarget: null),
                (PipelineName: "ReadStage", Script: ResolveScript(transformModel, "read-stage"), InsertRowsTarget: "dbo.DimCustomer"));

            var service = new MetaOrchestrationAnalysisService();
            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);
            var model = service.CreateModel(result, pipelineWorkspace);
            model.SaveToXmlWorkspace(orchestrationWorkspace);
            var reloaded = MetaOrchestrationModel.LoadFromXmlWorkspace(orchestrationWorkspace, searchUpward: false);

            Assert.False(result.IsCompleteDag);
            var issue = Assert.Single(result.Issues);
            Assert.Equal(OrchestrationIssueCode.UnsafeSharedReset, issue.Code);
            Assert.True(issue.BlocksDag);
            Assert.Equal("Invalid", Assert.Single(reloaded.OrchestrationPlanList).DagStatus);
            Assert.Contains(reloaded.DependencyIssueList, item => item.Code == "UnsafeSharedReset");
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Analyze_SameTargetAppendWriters_KeepCompleteDagWithSynchronizationNote()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("load-stage-a", "SELECT CustomerId FROM dbo.RawCustomerA", "dbo.StageCustomer"),
                ("load-stage-b", "SELECT CustomerId FROM dbo.RawCustomerB", "dbo.StageCustomer"),
                ("read-stage", "SELECT CustomerId FROM dbo.StageCustomer", "dbo.Downstream"));
            BuildBindingWorkspace(
                bindingWorkspace,
                (ResolveScript(transformModel, "load-stage-a"), ["dbo.RawCustomerA"], "dbo.StageCustomer"),
                (ResolveScript(transformModel, "load-stage-b"), ["dbo.RawCustomerB"], "dbo.StageCustomer"),
                (ResolveScript(transformModel, "read-stage"), ["dbo.StageCustomer"], "dbo.Downstream"));
            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "StageA", Script: ResolveScript(transformModel, "load-stage-a"), InsertRowsTarget: "dbo.StageCustomer"),
                (PipelineName: "StageB", Script: ResolveScript(transformModel, "load-stage-b"), InsertRowsTarget: "dbo.StageCustomer"),
                (PipelineName: "ReadStage", Script: ResolveScript(transformModel, "read-stage"), InsertRowsTarget: "dbo.Downstream"));

            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            Assert.True(result.IsCompleteDag);
            Assert.Equal("Complete", result.DagStatus);
            Assert.Equal("HasConstraints", result.SynchronizationStatus);
            Assert.Contains(result.Dependencies, item =>
                item.PredecessorPipelineId == "pipeline:StageA" &&
                item.SuccessorPipelineId == "pipeline:ReadStage");
            Assert.Contains(result.Dependencies, item =>
                item.PredecessorPipelineId == "pipeline:StageB" &&
                item.SuccessorPipelineId == "pipeline:ReadStage");
            var issue = Assert.Single(result.Issues);
            Assert.Equal(OrchestrationIssueCode.SharedAppendWritersRequirePolicy, issue.Code);
            Assert.Equal(OrchestrationIssueDomain.Synchronization, issue.Domain);
            Assert.False(issue.BlocksDag);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void Analyze_InferIndependentTableChains_WithReplaceSequence()
    {
        var result = AnalyzeProfiles(
            Profile(
                "WriteA",
                Task("WriteA", 1, "write-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
            Profile(
                "ReadA",
                Task("ReadA", 1, "read-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Read, "Source"))),
            Profile(
                "RefreshB",
                Task("RefreshB", 1, "truncate-b", "Truncate", Access("dbo.B", OrchestrationObjectAccessKind.ResetWrite, "Target")),
                Task("RefreshB", 2, "write-b", "Select", Access("dbo.B", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
            Profile(
                "ReadB",
                Task("ReadB", 1, "read-b", "Select", Access("dbo.B", OrchestrationObjectAccessKind.Read, "Source"))));

        Assert.True(result.IsCompleteDag);
        Assert.Empty(result.Issues);
        Assert.Contains(result.Dependencies, item =>
            item.PredecessorPipelineId == "pipeline:WriteA" &&
            item.SuccessorPipelineId == "pipeline:ReadA");
        Assert.Contains(result.Dependencies, item =>
            item.PredecessorPipelineId == "pipeline:RefreshB" &&
            item.SuccessorPipelineId == "pipeline:ReadB");
    }

    [Fact]
    public void Analyze_ReplacementMixedWithAppend_RequiresExplicitOrderingWithoutInvalidDag()
    {
        var result = AnalyzeProfiles(
            Profile(
                "RefreshA",
                Task("RefreshA", 1, "truncate-a", "Truncate", Access("dbo.A", OrchestrationObjectAccessKind.ResetWrite, "Target")),
                Task("RefreshA", 2, "write-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
            Profile(
                "AppendA",
                Task("AppendA", 1, "append-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
            Profile(
                "ReadA",
                Task("ReadA", 1, "read-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Read, "Source"))));

        Assert.True(result.IsCompleteDag);
        Assert.Equal("Complete", result.DagStatus);
        Assert.Equal("RequiresExplicitOrdering", result.DeterminismStatus);
        Assert.Contains(result.Dependencies, item =>
            item.PredecessorPipelineId == "pipeline:RefreshA" &&
            item.SuccessorPipelineId == "pipeline:ReadA");
        Assert.Contains(result.Dependencies, item =>
            item.PredecessorPipelineId == "pipeline:AppendA" &&
            item.SuccessorPipelineId == "pipeline:ReadA");
        Assert.Contains(result.Issues, item =>
            item.Code == OrchestrationIssueCode.WriteOrderAmbiguity &&
            item.Domain == OrchestrationIssueDomain.Determinism &&
            !item.BlocksDag &&
            item.BlocksAutomaticRunPlanning);
    }

    [Fact]
    public void Analyze_TwoSameTableMutators_DoNotCreateArtificialDependencyCycle()
    {
        var result = AnalyzeProfiles(
            Profile(
                "UpdateA",
                Task("UpdateA", 1, "update-a", "Update", Access("dbo.A", OrchestrationObjectAccessKind.ReadWrite, "Target"))),
            Profile(
                "MergeA",
                Task("MergeA", 1, "merge-a", "Merge", Access("dbo.A", OrchestrationObjectAccessKind.ReadWrite, "Target"))));

        Assert.True(result.IsCompleteDag);
        Assert.Empty(result.Dependencies);
        Assert.Equal("RequiresExplicitOrdering", result.DeterminismStatus);
        Assert.Contains(result.Issues, item =>
            item.Code == OrchestrationIssueCode.WriteOrderAmbiguity &&
            item.Domain == OrchestrationIssueDomain.Determinism);
        Assert.Contains(result.Issues, item =>
            item.Code == OrchestrationIssueCode.SynchronizationRequired &&
            item.Domain == OrchestrationIssueDomain.Synchronization);
    }

    [Fact]
    public void Analyze_CrossTableProducerConsumerCycle_InvalidatesDag()
    {
        var result = AnalyzeProfiles(
            Profile(
                "P1",
                Task(
                    "P1",
                    1,
                    "load-a-from-b",
                    "Select",
                    Access("dbo.B", OrchestrationObjectAccessKind.Read, "Source"),
                    Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
            Profile(
                "P2",
                Task(
                    "P2",
                    1,
                    "load-b-from-a",
                    "Select",
                    Access("dbo.A", OrchestrationObjectAccessKind.Read, "Source"),
                    Access("dbo.B", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))));

        Assert.False(result.IsCompleteDag);
        Assert.Equal("Invalid", result.DagStatus);
        Assert.Contains(result.Issues, item =>
            item.Code == OrchestrationIssueCode.DependencyCycle &&
            item.Domain == OrchestrationIssueDomain.Dependency &&
            item.BlocksDag);
    }

    [Fact]
    public void Analyze_InferredMemberRepair_RequiresSynchronizationWithoutFalseFactPipelineDependency()
    {
        var result = AnalyzeProfiles(
            Profile(
                "LoadInternetSales",
                Task(
                    "LoadInternetSales",
                    1,
                    "load-internet-sales",
                    "Select",
                    Access("dbo.DimCustomer", OrchestrationObjectAccessKind.ReadWrite, "InferredMemberRepair"),
                    Access("dbo.FactInternetSales", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
            Profile(
                "LoadStoreSales",
                Task(
                    "LoadStoreSales",
                    1,
                    "load-store-sales",
                    "Select",
                    Access("dbo.DimCustomer", OrchestrationObjectAccessKind.ReadWrite, "InferredMemberRepair"),
                    Access("dbo.FactStoreSales", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))));

        Assert.True(result.IsCompleteDag);
        Assert.Empty(result.Dependencies);
        Assert.DoesNotContain(result.TaskDependencies, item =>
            item.PredecessorPipelineId != item.SuccessorPipelineId);
        Assert.Contains(result.TaskObjectEffects, item =>
            item.ObjectKey == "DBO.DIMCUSTOMER" &&
            item.WriteEffect == OrchestrationWriteEffect.ConditionalKeyedUpsert &&
            item.AccessPurpose == OrchestrationAccessPurpose.InferredMemberRepair &&
            !item.IsPublishedProducer &&
            item.RequiresSynchronization);
        Assert.Contains(result.Issues, item =>
            item.Code == OrchestrationIssueCode.SynchronizationRequired &&
            item.Domain == OrchestrationIssueDomain.Synchronization &&
            !item.BlocksDag);
    }

    [Fact]
    public void RunPlan_IndependentTasks_AreAllInitiallyReady()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "LoadA",
                    Task("LoadA", 1, "load-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "LoadB",
                    Task("LoadB", 1, "load-b", "Select", Access("dbo.RawB", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.B", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "LoadC",
                    Task("LoadC", 1, "load-c", "Select", Access("dbo.RawC", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.C", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

        var result = new MetaOrchestrationRunPlanningService().BuildRunPlan(model);

        Assert.Equal("Ready", result.Status);
        Assert.Equal(3, result.PlannedTasks);
        Assert.Equal(3, model.PlannedTaskList.Count);
        var dependencies = OrchestrationExecutionContinuity.BuildDependencyMap(model);
        var taskOutcomes = new Dictionary<string, string>(StringComparer.Ordinal);
        Assert.All(
            PlannedTaskRows(model),
            task => Assert.Equal(
                OrchestrationTaskReadiness.Ready,
                OrchestrationExecutionContinuity.EvaluateReadiness(task, dependencies, taskOutcomes, out _, out _, out _)));
    }

    [Fact]
    public void CliRefreshRunPlan_WritesRunPlanRowsIntoExistingWorkspace()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var orchestrationWorkspace = Path.Combine(tempRoot, "Orchestration");
            var model = CreateModel(
                AnalyzeProfiles(
                    Profile(
                        "LoadA",
                        Task("LoadA", 1, "load-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                    Profile(
                        "LoadB",
                        Task("LoadB", 1, "load-b", "Select", Access("dbo.RawB", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.B", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));
            model.SaveToXmlWorkspace(orchestrationWorkspace);

            var result = RunCli($"refresh-run-plan --workspace \"{orchestrationWorkspace}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("Ok", result.Output, StringComparison.Ordinal);
            var reloaded = MetaOrchestrationModel.LoadFromXmlWorkspace(orchestrationWorkspace, searchUpward: false);
            var runPlan = Assert.Single(reloaded.RunPlanList);
            Assert.Equal("DefaultRunPlan", runPlan.Name);
            Assert.Equal(2, reloaded.PlannedTaskList.Count);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void AddTaskDependency_RecordsFailureConditionAsDagEdge()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "LoadA",
                    Task("LoadA", 1, "load-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "FailureHandler",
                    Task("FailureHandler", 1, "record-failure", "Insert", Access("dbo.OrchestrationFailureLog", OrchestrationObjectAccessKind.Write, "Target")))));
        var service = new MetaOrchestrationRunPlanningService();

        var result = service.AddTaskOrderingResolution(
            model,
            "LoadA.load-a",
            "FailureHandler.record-failure",
            null,
            "Run the modeled failure branch.",
            "failure");

        Assert.Equal("Added", result.Action);
        var resolution = Assert.Single(model.TaskOrderingResolutionList);
        Assert.Equal("OnFailure", resolution.DependencyCondition);
        Assert.Equal("ExplicitTaskDependency", resolution.ResolutionKind);
        Assert.Equal("Active", resolution.Status);
        Assert.Equal("LoadA.load-a", $"{resolution.Predecessor.PipelineReference.Name}.{resolution.Predecessor.TaskName}");
        Assert.Equal("FailureHandler.record-failure", $"{resolution.Successor.PipelineReference.Name}.{resolution.Successor.TaskName}");

        service.BuildRunPlan(model);

        var dependencies = OrchestrationExecutionContinuity.BuildDependencyMap(model);
        var plannedTasks = PlannedTaskRows(model);
        var loadA = plannedTasks.Single(item => item.PipelineReference.Name == "LoadA");
        var failureHandler = plannedTasks.Single(item => item.PipelineReference.Name == "FailureHandler");
        Assert.Equal(
            OrchestrationTaskReadiness.Waiting,
            OrchestrationExecutionContinuity.EvaluateReadiness(failureHandler, dependencies, EmptyTaskOutcomes(), out var waitingDependency, out _, out _));
        Assert.Equal(loadA.TaskAccessProfile.Id, waitingDependency.PredecessorTaskProfileId);
        Assert.Equal("OnFailure", waitingDependency.Condition);

        var failedOutcomes = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [loadA.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Failed
        };
        Assert.Equal(
            OrchestrationTaskReadiness.Ready,
            OrchestrationExecutionContinuity.EvaluateReadiness(failureHandler, dependencies, failedOutcomes, out _, out _, out _));
    }

    [Fact]
    public void ExecuteContinuity_BlocksOnlyDownstreamDependents_WhenOneTaskFails()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "HRExtract",
                    Task("HRExtract", 1, "extract-hr", "Select", Access("dbo.RawHR", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.HRStage", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "HRDimension",
                    Task("HRDimension", 1, "load-hr-dim", "Select", Access("dbo.HRStage", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.HRDim", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "FinanceExtract",
                    Task("FinanceExtract", 1, "extract-finance", "Select", Access("dbo.RawFinance", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.FinanceStage", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "FinanceBudget",
                    Task("FinanceBudget", 1, "load-budget", "Select", Access("dbo.FinanceStage", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Budget", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "HRFailureHandler",
                    Task("HRFailureHandler", 1, "record-failure", "Insert", Access("dbo.HRFailureLog", OrchestrationObjectAccessKind.Write, "Target")))));
        var service = new MetaOrchestrationRunPlanningService();
        service.AddTaskOrderingResolution(model, "HRExtract.extract-hr", "HRFailureHandler.record-failure", null, "Run HR failure branch.", "failure");
        service.BuildRunPlan(model);
        var dependencies = OrchestrationExecutionContinuity.BuildDependencyMap(model);
        var taskOutcomes = new Dictionary<string, string>(StringComparer.Ordinal);
        var plannedTasks = PlannedTaskRows(model);
        var hrExtract = plannedTasks.Single(item => item.PipelineReference.Name == "HRExtract");
        var financeExtract = plannedTasks.Single(item => item.PipelineReference.Name == "FinanceExtract");
        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(hrExtract, dependencies, taskOutcomes, out _, out _, out _));
        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(financeExtract, dependencies, taskOutcomes, out _, out _, out _));

        var failedHrExtract = hrExtract;
        taskOutcomes[failedHrExtract.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Failed;
        taskOutcomes[financeExtract.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Succeeded;

        var hrDimension = plannedTasks.Single(item => item.PipelineReference.Name == "HRDimension");
        var financeBudget = plannedTasks.Single(item => item.PipelineReference.Name == "FinanceBudget");
        var hrFailureHandler = plannedTasks.Single(item => item.PipelineReference.Name == "HRFailureHandler");
        Assert.Equal(OrchestrationTaskReadiness.Skip, OrchestrationExecutionContinuity.EvaluateReadiness(hrDimension, dependencies, taskOutcomes, out var blockedDependency, out var blockedOutcome, out _));
        Assert.Equal(failedHrExtract.TaskAccessProfile.Id, blockedDependency.PredecessorTaskProfileId);
        Assert.Equal("OnSuccess", blockedDependency.Condition);
        Assert.Equal(OrchestrationExecutionContinuity.SkippedBlocked, blockedOutcome);
        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(financeBudget, dependencies, taskOutcomes, out _, out _, out _));
        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(hrFailureHandler, dependencies, taskOutcomes, out _, out _, out _));
    }

    [Fact]
    public void ExecuteContinuity_SkipsFailureBranch_WhenPredecessorSucceeds()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "LoadA",
                    Task("LoadA", 1, "load-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "FailureHandler",
                    Task("FailureHandler", 1, "record-failure", "Insert", Access("dbo.FailureLog", OrchestrationObjectAccessKind.Write, "Target")))));
        var service = new MetaOrchestrationRunPlanningService();
        service.AddTaskOrderingResolution(model, "LoadA.load-a", "FailureHandler.record-failure", null, "Run failure branch.", "failure");
        service.BuildRunPlan(model);
        var dependencies = OrchestrationExecutionContinuity.BuildDependencyMap(model);
        var plannedTasks = PlannedTaskRows(model);
        var loadA = plannedTasks.Single(item => item.PipelineReference.Name == "LoadA");
        var failureHandler = plannedTasks.Single(item => item.PipelineReference.Name == "FailureHandler");
        var taskOutcomes = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [loadA.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Succeeded
        };

        Assert.Equal(OrchestrationTaskReadiness.Skip, OrchestrationExecutionContinuity.EvaluateReadiness(failureHandler, dependencies, taskOutcomes, out var dependency, out var skipOutcome, out _));
        Assert.Equal("OnFailure", dependency.Condition);
        Assert.Equal(OrchestrationExecutionContinuity.SkippedConditionNotMet, skipOutcome);
    }


    [Fact]
    public void RunPlan_ProducerConsumer_KeepsDependencyAsRuntimeGraphEdge()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "WriteA",
                    Task("WriteA", 1, "write-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "ReadA",
                    Task("ReadA", 1, "read-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.B", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

        var result = new MetaOrchestrationRunPlanningService().BuildRunPlan(model);

        Assert.Equal(2, result.PlannedTasks);
        Assert.Equal(["ReadA.read-a", "WriteA.write-a"], PlannedTaskNamesInOrder(model));

        var dependencies = OrchestrationExecutionContinuity.BuildDependencyMap(model);
        var plannedTasks = PlannedTaskRows(model);
        var writeA = plannedTasks.Single(item => item.PipelineReference.Name == "WriteA");
        var readA = plannedTasks.Single(item => item.PipelineReference.Name == "ReadA");
        Assert.Equal(
            OrchestrationTaskReadiness.Waiting,
            OrchestrationExecutionContinuity.EvaluateReadiness(readA, dependencies, EmptyTaskOutcomes(), out var dependency, out _, out _));
        Assert.Equal(writeA.TaskAccessProfile.Id, dependency.PredecessorTaskProfileId);
        Assert.Equal("OnSuccess", dependency.Condition);

        var taskOutcomes = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [writeA.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Succeeded
        };
        Assert.Equal(
            OrchestrationTaskReadiness.Ready,
            OrchestrationExecutionContinuity.EvaluateReadiness(readA, dependencies, taskOutcomes, out _, out _, out _));
    }

    [Fact]
    public void RunPlan_DiamondGraph_KeepsEdgesInRuntimeDependencyGraph()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "Seed",
                    Task("Seed", 1, "seed", "Select", Access("dbo.Raw", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Stage", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "Dim",
                    Task("Dim", 1, "dim", "Select", Access("dbo.Stage", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Dim", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "Fact",
                    Task("Fact", 1, "fact", "Select", Access("dbo.Stage", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Fact", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "Mart",
                    Task("Mart", 1, "mart", "Select", Access("dbo.Dim", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Fact", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Mart", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

        var result = new MetaOrchestrationRunPlanningService().BuildRunPlan(model);

        Assert.Equal(4, result.PlannedTasks);
        Assert.Equal(["Dim.dim", "Fact.fact", "Mart.mart", "Seed.seed"], PlannedTaskNamesInOrder(model));

        var dependencies = OrchestrationExecutionContinuity.BuildDependencyMap(model);
        var plannedTasks = PlannedTaskRows(model);
        var seed = plannedTasks.Single(item => item.PipelineReference.Name == "Seed");
        var dim = plannedTasks.Single(item => item.PipelineReference.Name == "Dim");
        var fact = plannedTasks.Single(item => item.PipelineReference.Name == "Fact");
        var mart = plannedTasks.Single(item => item.PipelineReference.Name == "Mart");

        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(seed, dependencies, EmptyTaskOutcomes(), out _, out _, out _));
        Assert.Equal(OrchestrationTaskReadiness.Waiting, OrchestrationExecutionContinuity.EvaluateReadiness(dim, dependencies, EmptyTaskOutcomes(), out _, out _, out _));
        Assert.Equal(OrchestrationTaskReadiness.Waiting, OrchestrationExecutionContinuity.EvaluateReadiness(fact, dependencies, EmptyTaskOutcomes(), out _, out _, out _));
        Assert.Equal(OrchestrationTaskReadiness.Waiting, OrchestrationExecutionContinuity.EvaluateReadiness(mart, dependencies, EmptyTaskOutcomes(), out _, out _, out _));

        var afterSeed = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [seed.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Succeeded
        };
        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(dim, dependencies, afterSeed, out _, out _, out _));
        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(fact, dependencies, afterSeed, out _, out _, out _));
        Assert.Equal(OrchestrationTaskReadiness.Waiting, OrchestrationExecutionContinuity.EvaluateReadiness(mart, dependencies, afterSeed, out _, out _, out _));

        afterSeed[dim.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Succeeded;
        afterSeed[fact.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Succeeded;
        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(mart, dependencies, afterSeed, out _, out _, out _));
    }

    [Fact]
    public void CliInspectRunPlan_PrintsDependencyGraph_NotWavesOrOrder()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var orchestrationWorkspace = Path.Combine(tempRoot, "Orchestration");
            var model = CreateModel(
                AnalyzeProfiles(
                    Profile(
                        "Seed",
                        Task("Seed", 1, "seed", "Select", Access("dbo.Raw", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Stage", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                    Profile(
                        "Dim",
                        Task("Dim", 1, "dim", "Select", Access("dbo.Stage", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Dim", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                    Profile(
                        "Fact",
                        Task("Fact", 1, "fact", "Select", Access("dbo.Stage", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Fact", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                    Profile(
                        "Mart",
                        Task("Mart", 1, "mart", "Select", Access("dbo.Dim", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Fact", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Mart", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));
            new MetaOrchestrationRunPlanningService().BuildRunPlan(model);
            model.SaveToXmlWorkspace(orchestrationWorkspace);

            var result = RunCli($"inspect-run-plan --workspace \"{orchestrationWorkspace}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("DefaultRunPlan", result.Output, StringComparison.Ordinal);
            Assert.Contains("PlannedTasks: 4", result.Output, StringComparison.Ordinal);
            Assert.Contains("DependencyEdges: 4", result.Output, StringComparison.Ordinal);
            Assert.Contains("Graph:", result.Output, StringComparison.Ordinal);
            Assert.Contains("  Seed.seed", result.Output, StringComparison.Ordinal);
            Assert.Contains("    --> Dim.dim [OnSuccess/Data/dbo.Stage]", result.Output, StringComparison.Ordinal);
            Assert.Contains("    --> Fact.fact [OnSuccess/Data/dbo.Stage]", result.Output, StringComparison.Ordinal);
            Assert.Contains("  Dim.dim", result.Output, StringComparison.Ordinal);
            Assert.Contains("    --> Mart.mart [OnSuccess/Data/dbo.Dim]", result.Output, StringComparison.Ordinal);
            Assert.Contains("  Fact.fact", result.Output, StringComparison.Ordinal);
            Assert.Contains("    --> Mart.mart [OnSuccess/Data/dbo.Fact]", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("ParallelWaves", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("MaxWaveWidth", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("Wave ", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("+--", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("`--", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void RunPlan_ReplacementMixedWithAppend_RequiresOrderingResolution()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "RefreshA",
                    Task("RefreshA", 1, "truncate-a", "Truncate", Access("dbo.A", OrchestrationObjectAccessKind.ResetWrite, "Target")),
                    Task("RefreshA", 2, "write-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "AppendA",
                    Task("AppendA", 1, "append-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "ReadA",
                    Task("ReadA", 1, "read-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Read, "Source")))));
        var service = new MetaOrchestrationRunPlanningService();

        var failure = Assert.Throws<InvalidOperationException>(() => service.BuildRunPlan(model));
        Assert.Contains("require explicit run-planning policy", failure.Message, StringComparison.Ordinal);

        service.AddTaskOrderingResolution(model, "RefreshA.write-a", "AppendA.append-a", "dbo.A", "Refresh before append.");
        service.BuildRunPlan(model);

        var dependencies = OrchestrationExecutionContinuity.BuildDependencyMap(model);
        var plannedTasks = PlannedTaskRows(model);
        var refreshWrite = plannedTasks.Single(item => item.PipelineReference.Name == "RefreshA" && item.TaskAccessProfile.TaskName == "write-a");
        var appendA = plannedTasks.Single(item => item.PipelineReference.Name == "AppendA");
        Assert.Equal(
            OrchestrationTaskReadiness.Waiting,
            OrchestrationExecutionContinuity.EvaluateReadiness(appendA, dependencies, EmptyTaskOutcomes(), out var dependency, out _, out _));
        Assert.Equal(refreshWrite.TaskAccessProfile.Id, dependency.PredecessorTaskProfileId);

        var taskOutcomes = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [refreshWrite.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Succeeded
        };
        Assert.Equal(
            OrchestrationTaskReadiness.Ready,
            OrchestrationExecutionContinuity.EvaluateReadiness(appendA, dependencies, taskOutcomes, out _, out _, out _));
    }

    [Fact]
    public void RunPlan_SameTargetAppendWriters_RecordConcurrentLockPolicy()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "StageA",
                    Task("StageA", 1, "append-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Stage", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "StageB",
                    Task("StageB", 1, "append-b", "Select", Access("dbo.RawB", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Stage", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "ReadStage",
                    Task("ReadStage", 1, "read-stage", "Select", Access("dbo.Stage", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Downstream", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));
        var service = new MetaOrchestrationRunPlanningService();

        service.AddConcurrentAppendPolicy(model, "dbo.Stage", "Stage writes are append-only.");
        service.BuildRunPlan(model);

        var dependencies = OrchestrationExecutionContinuity.BuildDependencyMap(model);
        var plannedTasks = PlannedTaskRows(model);
        var stageA = plannedTasks.Single(item => item.PipelineReference.Name == "StageA");
        var stageB = plannedTasks.Single(item => item.PipelineReference.Name == "StageB");
        var readStage = plannedTasks.Single(item => item.PipelineReference.Name == "ReadStage");
        Assert.Equal(OrchestrationTaskReadiness.Waiting, OrchestrationExecutionContinuity.EvaluateReadiness(readStage, dependencies, EmptyTaskOutcomes(), out _, out _, out _));
        var taskOutcomes = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [stageA.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Succeeded
        };
        Assert.Equal(OrchestrationTaskReadiness.Waiting, OrchestrationExecutionContinuity.EvaluateReadiness(readStage, dependencies, taskOutcomes, out _, out _, out _));
        taskOutcomes[stageB.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Succeeded;
        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(readStage, dependencies, taskOutcomes, out _, out _, out _));
        Assert.Single(model.LockCompatibilityPolicyList);
        Assert.All(model.PlannedTaskLockList.Where(item => item.DataObject.SqlIdentifier == "dbo.Stage"), item => Assert.NotNull(item.Reason));
    }

    [Fact]
    public void RunPlan_ConditionalKeyedUpsert_RequiresScopedLockPolicy()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "LoadInternetSales",
                    Task(
                        "LoadInternetSales",
                        1,
                        "load-internet-sales",
                        "Select",
                        Access("dbo.DimCustomer", OrchestrationObjectAccessKind.ReadWrite, "InferredMemberRepair"),
                        Access("dbo.FactInternetSales", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "LoadStoreSales",
                    Task(
                        "LoadStoreSales",
                        1,
                        "load-store-sales",
                        "Select",
                        Access("dbo.DimCustomer", OrchestrationObjectAccessKind.ReadWrite, "InferredMemberRepair"),
                        Access("dbo.FactStoreSales", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));
        var service = new MetaOrchestrationRunPlanningService();

        var failure = Assert.Throws<InvalidOperationException>(() => service.BuildRunPlan(model));
        Assert.Contains("require explicit run-planning policy", failure.Message, StringComparison.Ordinal);
        Assert.Equal("Complete", Assert.Single(model.OrchestrationPlanList).DagStatus);

        service.AddLockCompatibilityPolicy(
            model,
            "dbo.DimCustomer",
            "ConditionalKeyedUpsert",
            "ConditionalKeyedUpsert",
            "serialize",
            "No modeled unique-key/atomic upsert proof yet.");
        service.BuildRunPlan(model);

        Assert.Equal(2, model.PlannedTaskList.Count);
        Assert.All(model.PlannedTaskLockList.Where(item => item.DataObject.SqlIdentifier == "dbo.DimCustomer"), item => Assert.NotNull(item.LockCompatibilityPolicy));
    }

    [Fact]
    public void RunPlan_InvalidDag_Throws()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "P1",
                    Task(
                        "P1",
                        1,
                        "load-a-from-b",
                        "Select",
                        Access("dbo.B", OrchestrationObjectAccessKind.Read, "Source"),
                        Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "P2",
                    Task(
                        "P2",
                        1,
                        "load-b-from-a",
                        "Select",
                        Access("dbo.A", OrchestrationObjectAccessKind.Read, "Source"),
                        Access("dbo.B", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

        var failure = Assert.Throws<InvalidOperationException>(() => new MetaOrchestrationRunPlanningService().BuildRunPlan(model));

        Assert.Contains("DagStatus is 'Invalid'", failure.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CliInfer_CreatesWorkspaceAndReturnsNonZero_WhenDagIsInvalid()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");
            var orchestrationWorkspace = Path.Combine(tempRoot, "Orchestration");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("truncate-stage", "TRUNCATE TABLE dbo.StageCustomer", null),
                ("read-stage", "SELECT CustomerId FROM dbo.StageCustomer", "dbo.DimCustomer"));
            BuildBindingWorkspace(
                bindingWorkspace,
                (ResolveScript(transformModel, "truncate-stage"), [], "dbo.StageCustomer"),
                (ResolveScript(transformModel, "read-stage"), ["dbo.StageCustomer"], "dbo.DimCustomer"));
            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "TruncateStage", Script: ResolveScript(transformModel, "truncate-stage"), InsertRowsTarget: null),
                (PipelineName: "ReadStage", Script: ResolveScript(transformModel, "read-stage"), InsertRowsTarget: "dbo.DimCustomer"));

            var result = RunCli($"--pipeline-workspace \"{pipelineWorkspace}\" --transform-workspace \"{transformWorkspace}\" --binding-workspace \"{bindingWorkspace}\" --new-workspace \"{orchestrationWorkspace}\"");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("Cannot continue", result.Output, StringComparison.Ordinal);
            var model = MetaOrchestrationModel.LoadFromXmlWorkspace(orchestrationWorkspace, searchUpward: false);
            Assert.Equal("Invalid", Assert.Single(model.OrchestrationPlanList).DagStatus);
            Assert.Single(model.DependencyIssueList);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    private static OrchestrationAnalysisResult Analyze(
        string pipelineWorkspace,
        string transformWorkspace,
        string bindingWorkspace)
    {
        return new MetaOrchestrationAnalysisService().Analyze(
            new OrchestrationAnalysisRequest(
                pipelineWorkspace,
                transformWorkspace,
                bindingWorkspace,
                "Default"));
    }

    private static OrchestrationAnalysisResult AnalyzeProfiles(params PipelineDependencyProfile[] profiles)
    {
        return new MetaOrchestrationAnalysisService().AnalyzeProfiles("Default", null, profiles);
    }

    private static Task<OrchestrationRuntimeResult> ExecuteWithFakePipelineWorkerAsync(
        string tempRoot,
        MetaOrchestrationModel model,
        string workerScript,
        TimeSpan? workerEventTimeout = null)
    {
        var orchestrationWorkspace = Path.Combine(tempRoot, "Orchestration");
        var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");
        var transformWorkspace = Path.Combine(tempRoot, "Transform");
        var bindingWorkspace = Path.Combine(tempRoot, "Binding");
        var runArtifactsRoot = Path.Combine(tempRoot, "Runs");
        Directory.CreateDirectory(pipelineWorkspace);
        Directory.CreateDirectory(transformWorkspace);
        Directory.CreateDirectory(bindingWorkspace);
        model.SaveToXmlWorkspace(orchestrationWorkspace);

        var workerPath = Path.Combine(tempRoot, "fake-meta-pipeline.cmd");
        var workerPowerShellPath = Path.Combine(tempRoot, "fake-meta-pipeline.ps1");
        File.WriteAllText(
            workerPath,
            """
            @echo off
            powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0fake-meta-pipeline.ps1" %*
            exit /b %errorlevel%
            """.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\n", Environment.NewLine, StringComparison.Ordinal));
        File.WriteAllText(
            workerPowerShellPath,
            FakeWorkerPowerShellPreamble
                + Environment.NewLine
                + workerScript.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\n", Environment.NewLine, StringComparison.Ordinal)
                + Environment.NewLine
                + FakeWorkerPowerShellEpilogue);

        return new MetaOrchestrationRuntimeService().ExecuteAsync(
            new OrchestrationRuntimeRequest(
                orchestrationWorkspace,
                pipelineWorkspace,
                transformWorkspace,
                bindingWorkspace,
                string.Empty,
                string.Empty,
                1,
                PipelineExecutableName: workerPath,
                RunArtifactsRootPath: runArtifactsRoot,
                ExpectedWorkerExecutableVersion: "test-worker",
                WorkerEventTimeout: workerEventTimeout));
    }

    private const string FakeWorkerPowerShellPreamble = """
        $ErrorActionPreference = 'Stop'
        $WorkerArgs = $args
        $pipeline = ''
        $pipeName = ''
        for ($index = 0; $index -lt $WorkerArgs.Count; $index++) {
            if ($WorkerArgs[$index] -eq '--pipeline') {
                $index++
                $pipeline = $WorkerArgs[$index]
                continue
            }
            if ($WorkerArgs[$index] -eq '--control-pipe') {
                $index++
                $pipeName = $WorkerArgs[$index]
                continue
            }
        }

        if ([string]::IsNullOrWhiteSpace($pipeName)) {
            throw 'missing --control-pipe'
        }

        $client = [System.IO.Pipes.NamedPipeClientStream]::new('.', $pipeName, [System.IO.Pipes.PipeDirection]::InOut, [System.IO.Pipes.PipeOptions]::Asynchronous)
        $client.Connect(10000)
        $utf8 = [System.Text.UTF8Encoding]::new($false)
        $reader = [System.IO.StreamReader]::new($client, $utf8, $false, 4096, $true)
        $writer = [System.IO.StreamWriter]::new($client, $utf8, 4096, $true)
        $writer.AutoFlush = $true

        function Send-WorkerEvent {
            param(
                [string] $Kind,
                [string] $TaskId = 'no-task',
                [string] $TaskName = 'no-task',
                [string] $GrantId = 'no-grant',
                [string] $CommandId = 'no-command',
                [int] $Attempt = 0,
                [int] $ExitCode = 0,
                [string] $Version = 'test-worker',
                [string] $Message = '',
                [string] $FailureClass = ''
            )
            $fields = @(
                'META_PIPELINE_WORKER',
                $Kind,
                'test-worker',
                "pipeline:$pipeline",
                $pipeline,
                $TaskId,
                $TaskName,
                $GrantId,
                $CommandId,
                [string] $Attempt,
                [string] $ExitCode,
                $Version,
                $Message,
                $FailureClass
            )
            $writer.WriteLine(($fields -join "`t"))
        }

        function Send-RawWorkerLine {
            param([string] $Line)
            $writer.WriteLine($Line)
        }

        function Read-WorkerCommand {
            $reader.ReadLine()
        }

        function Read-StartPipelineCommand {
            $line = Read-WorkerCommand
            if ([string]::IsNullOrWhiteSpace($line)) {
                throw 'missing StartPipeline command'
            }

            $fields = $line.Split("`t")
            if ($fields.Count -lt 10 -or $fields[0] -ne 'META_ORCHESTRATION' -or $fields[1] -ne 'StartPipeline') {
                throw "expected StartPipeline command, got '$line'"
            }

            $startedPipeline = [System.Uri]::UnescapeDataString($fields[7])
            if ([string]::IsNullOrWhiteSpace($startedPipeline)) {
                throw 'StartPipeline did not name a pipeline'
            }

            if (![string]::IsNullOrWhiteSpace($pipeline) -and $startedPipeline -ne $pipeline) {
                throw "StartPipeline named '$startedPipeline', expected '$pipeline'"
            }

            $script:pipeline = $startedPipeline
            $line
        }

        try {
        """;

    private const string FakeWorkerPowerShellEpilogue = """
        }
        finally {
            if ($null -ne $writer) { $writer.Dispose() }
            if ($null -ne $reader) { $reader.Dispose() }
            if ($null -ne $client) { $client.Dispose() }
        }
        """;

    private static void AddTestRetryPolicy(
        MetaOrchestrationModel model,
        int maxAttempts,
        bool retryWrites)
    {
        var plan = Assert.Single(model.OrchestrationPlanList);
        var policy = new RetryPolicy
        {
            Id = "retry-policy:test",
            OrchestrationPlan = plan,
            Name = "TestRetryPolicy",
            PolicyKind = "TestRetryPolicy",
            MaxAttempts = maxAttempts.ToString(CultureInfo.InvariantCulture),
            InitialDelayMilliseconds = "0",
            MaxDelayMilliseconds = "0",
            BackoffMultiplier = "1",
            RetryReadOnlyTasksByDefault = "true",
            RetryWriteTasksByDefault = retryWrites ? "true" : "false",
            Status = "Active",
            Reason = "Test retry policy."
        };
        model.RetryPolicyList.Add(policy);
        model.RetryPolicyFailureClassList.Add(new RetryPolicyFailureClass
        {
            Id = "retry-policy:test:failure-class:worker-reported-retryable",
            RetryPolicy = policy,
            FailureClass = WorkerFailureClasses.WorkerReportedRetryable,
            RetryBehavior = "Retry",
            Reason = "Test retry class."
        });
    }

    private static MetaOrchestrationModel CreateModel(OrchestrationAnalysisResult result)
    {
        return new MetaOrchestrationAnalysisService().CreateModel(result, Path.Combine(Path.GetTempPath(), "MetaOrchestration.Tests", "Pipeline"));
    }

    private static string[] PlannedTaskNamesInOrder(MetaOrchestrationModel model)
    {
        return PlannedTaskRows(model)
            .Select(static item => $"{item.PipelineReference.Name}.{item.TaskAccessProfile.TaskName}")
            .ToArray();
    }

    private static PlannedTask[] PlannedTaskRows(MetaOrchestrationModel model)
    {
        return model.PlannedTaskList
            .OrderBy(static item => int.Parse(item.Ordinal))
            .ToArray();
    }

    private static Dictionary<string, string> EmptyTaskOutcomes() =>
        new(StringComparer.Ordinal);

    private static PipelineDependencyProfile Profile(string pipelineName, params PipelineTaskAccessProfile[] tasks)
    {
        var objectAccesses = tasks
            .SelectMany(static item => item.ObjectAccesses)
            .GroupBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var first = group.OrderBy(static item => item.SqlIdentifier, StringComparer.OrdinalIgnoreCase).First();
                return new PipelineObjectAccessProfile(
                    first.SqlIdentifier,
                    first.ObjectKey,
                    AggregateAccessKind(group.Select(static item => item.AccessKind)),
                    "Pipeline",
                    0,
                    null,
                    string.Join("; ", group.Select(static item => $"{item.AccessRole}:{item.AccessKind}")));
            })
            .OrderBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new PipelineDependencyProfile(
            $"pipeline:{pipelineName}",
            pipelineName,
            tasks,
            objectAccesses,
            []);
    }

    private static PipelineTaskAccessProfile Task(
        string pipelineName,
        int ordinal,
        string taskName,
        string statementKind,
        params PipelineObjectAccessProfile[] accesses)
    {
        return new PipelineTaskAccessProfile(
            $"pipeline:{pipelineName}:task:{ordinal}",
            taskName,
            ordinal,
            $"script:{pipelineName}:{taskName}",
            taskName,
            $"binding:{pipelineName}:{taskName}",
            statementKind,
            accesses);
    }

    private static PipelineObjectAccessProfile Access(
        string sqlIdentifier,
        OrchestrationObjectAccessKind accessKind,
        string accessRole,
        int ordinal = 0,
        string? operationKind = null)
    {
        return new PipelineObjectAccessProfile(
            sqlIdentifier,
            sqlIdentifier.ToUpperInvariant(),
            accessKind,
            accessRole,
            ordinal,
            operationKind,
            accessRole);
    }

    private static OrchestrationObjectAccessKind AggregateAccessKind(IEnumerable<OrchestrationObjectAccessKind> kinds)
    {
        var set = kinds.ToHashSet();
        if (set.Contains(OrchestrationObjectAccessKind.ResetWrite)) return OrchestrationObjectAccessKind.ResetWrite;
        if (set.Contains(OrchestrationObjectAccessKind.ReadWrite)) return OrchestrationObjectAccessKind.ReadWrite;
        if (set.Contains(OrchestrationObjectAccessKind.Write)) return OrchestrationObjectAccessKind.Write;
        return OrchestrationObjectAccessKind.Read;
    }

    private static async Task<MetaTransformScriptModel> BuildTransformWorkspaceAsync(
        string transformWorkspace,
        params (string Name, string Sql, string? TargetSqlIdentifier)[] scripts)
    {
        var service = new MetaTransformScriptSqlService();
        for (var index = 0; index < scripts.Length; index++)
        {
            var script = scripts[index];
            if (index == 0)
            {
                await service.ImportFromSqlCodeToWorkspaceAsync(
                    script.Sql,
                    script.TargetSqlIdentifier,
                    transformWorkspace,
                    script.Name);
            }
            else
            {
                await service.AddSqlCodeToWorkspaceAsync(
                    script.Sql,
                    script.TargetSqlIdentifier,
                    transformWorkspace,
                    script.Name);
            }
        }

        return MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspace, searchUpward: false);
    }

    private static void AssertTaskObjectEffect(
        OrchestrationAnalysisResult result,
        string pipelineName,
        string sqlIdentifier,
        OrchestrationAccessDirection accessDirection,
        OrchestrationWriteEffect writeEffect,
        OrchestrationAccessPurpose accessPurpose)
    {
        var effect = Assert.Single(result.TaskObjectEffects, item =>
            string.Equals(item.PipelineName, pipelineName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(item.SqlIdentifier, sqlIdentifier, StringComparison.OrdinalIgnoreCase));
        Assert.Equal(accessDirection, effect.AccessDirection);
        Assert.Equal(writeEffect, effect.WriteEffect);
        Assert.Equal(accessPurpose, effect.AccessPurpose);
    }

    private static StoredProcedureOperationSeed ParseOperationSeed(string value)
    {
        var parts = value.Split(':', 3);
        return Operation(
            int.Parse(parts[0]),
            parts[1],
            parts.Length == 3 ? parts[2] : "dbo.Stage");
    }

    private static StoredProcedureOperationSeed Operation(
        int ordinal,
        string operationKind,
        string sqlIdentifier,
        string? accessRole = null) =>
        new(ordinal, operationKind, sqlIdentifier, accessRole);

    private static void AddStoredProcedureContractOperations(
        MetaTransformScriptModel model,
        string transformScriptName,
        IReadOnlyList<StoredProcedureOperationSeed> operations)
    {
        var script = ResolveScript(model, transformScriptName);
        var storedProcedure = Assert.Single(model.ScriptObjectStoredProcedureList, item =>
            string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
        var contract = new StoredProcedureContract
        {
            Id = $"{storedProcedure.Id}:contract",
            ScriptObjectStoredProcedure = storedProcedure
        };
        model.StoredProcedureContractList.Add(contract);

        foreach (var operation in operations)
        {
            model.StoredProcedureContractOperationList.Add(new StoredProcedureContractOperation
            {
                Id = $"{contract.Id}:operation:{operation.Ordinal}",
                StoredProcedureContract = contract,
                Ordinal = operation.Ordinal.ToString(),
                OperationKind = operation.OperationKind,
                SqlIdentifier = operation.SqlIdentifier,
                AccessRole = operation.AccessRole
            });
        }
    }

    private static void AddStoredProcedureResultRowset(
        MetaTransformScriptModel model,
        string transformScriptName,
        IReadOnlyList<string> columnNames)
    {
        var script = ResolveScript(model, transformScriptName);
        var storedProcedure = Assert.Single(model.ScriptObjectStoredProcedureList, item =>
            string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
        var contract = Assert.Single(model.StoredProcedureContractList, item =>
            string.Equals(item.ScriptObjectStoredProcedure.Id, storedProcedure.Id, StringComparison.Ordinal));
        var rowset = new StoredProcedureResultRowsetItem
        {
            Id = $"{contract.Id}:result-rowset:1",
            StoredProcedureContract = contract,
            Name = "Result",
            Ordinal = "0",
        };
        model.StoredProcedureResultRowsetItemList.Add(rowset);

        for (var index = 0; index < columnNames.Count; index++)
        {
            model.StoredProcedureResultColumnItemList.Add(new StoredProcedureResultColumnItem
            {
                Id = $"{rowset.Id}:column:{index + 1}",
                StoredProcedureResultRowsetItem = rowset,
                Name = columnNames[index],
                Ordinal = index.ToString(),
            });
        }
    }

    private sealed record StoredProcedureOperationSeed(
        int Ordinal,
        string OperationKind,
        string SqlIdentifier,
        string? AccessRole);

    private static void BuildBindingWorkspace(
        string bindingWorkspace,
        params (TransformScript Script, IReadOnlyList<string> Sources, string? Target)[] bindings)
    {
        var model = MetaTransformBindingModel.CreateEmpty();
        foreach (var bindingSeed in bindings)
        {
            var binding = new TransformBinding
            {
                Id = $"{bindingSeed.Script.Id}:binding",
                MetaTransformScriptTransformScriptId = bindingSeed.Script.Id,
                TransformScriptName = bindingSeed.Script.Name,
            };
            model.TransformBindingList.Add(binding);

            foreach (var source in bindingSeed.Sources.Select((value, index) => (value, index)))
            {
                model.RowsetList.Add(new Rowset
                {
                    Id = $"{binding.Id}:source:{source.index + 1}",
                    TransformBinding = binding,
                    Name = source.value,
                    DerivationKind = "Source",
                    SqlIdentifier = source.value,
                });
            }

            if (!string.IsNullOrWhiteSpace(bindingSeed.Target))
            {
                model.TransformBindingTargetList.Add(new TransformBindingTarget
                {
                    Id = $"{binding.Id}:target:1",
                    TransformBinding = binding,
                    SqlIdentifier = bindingSeed.Target,
                });
                var targetRowset = new Rowset
                {
                    Id = $"{binding.Id}:target-rowset",
                    TransformBinding = binding,
                    Name = bindingSeed.Target,
                    DerivationKind = "Target",
                    SqlIdentifier = bindingSeed.Target,
                };
                model.RowsetList.Add(targetRowset);
                model.OutputRowsetList.Add(new OutputRowset
                {
                    Id = $"{binding.Id}:output",
                    TransformBinding = binding,
                    Rowset = targetRowset,
                });
            }
        }

        model.SaveToXmlWorkspace(bindingWorkspace);
    }

    private static void BuildPipelineWorkspace(
        string pipelineWorkspace,
        params (string PipelineName, TransformScript Script, string? InsertRowsTarget)[] pipelines)
    {
        var model = MetaPipelineModel.CreateEmpty();
        foreach (var pipelineSeed in pipelines)
        {
            var pipeline = new Pipeline
            {
                Id = $"pipeline:{pipelineSeed.PipelineName}",
                Name = pipelineSeed.PipelineName,
            };
            model.PipelineList.Add(pipeline);
            var connection = new ConnectionReference
            {
                Id = $"{pipeline.Id}:connection:execution",
                Pipeline = pipeline,
                Name = "Execution",
                EnvironmentVariableName = "EXECUTION_SQL",
            };
            model.ConnectionReferenceList.Add(connection);
            var transformTask = new PipelineTask
            {
                Id = $"{pipeline.Id}:task:transform",
                Pipeline = pipeline,
                Name = "transform",
                Ordinal = "1",
            };
            model.PipelineTaskList.Add(transformTask);
            model.TransformExecutionTaskList.Add(new TransformExecutionTask
            {
                Id = $"{transformTask.Id}:execution",
                PipelineTask = transformTask,
                ExecutionConnectionReference = connection,
                TransformScriptId = pipelineSeed.Script.Id,
                TransformBindingId = $"{pipelineSeed.Script.Id}:binding",
            });

            if (string.IsNullOrWhiteSpace(pipelineSeed.InsertRowsTarget))
            {
                continue;
            }

            var rowStream = new RowStream
            {
                Id = $"{pipeline.Id}:rowstream:1",
                Pipeline = pipeline,
                Name = "transform.rows",
            };
            model.RowStreamList.Add(rowStream);
            model.RowStreamProducerList.Add(new RowStreamProducer
            {
                Id = $"{transformTask.Id}:producer",
                PipelineTask = transformTask,
                RowStream = rowStream,
            });

            var targetWriteTask = new PipelineTask
            {
                Id = $"{pipeline.Id}:task:target-write",
                Pipeline = pipeline,
                Name = "target-write",
                Ordinal = "2",
            };
            model.PipelineTaskList.Add(targetWriteTask);
            var targetWrite = new TargetWriteTask
            {
                Id = $"{targetWriteTask.Id}:target-write",
                PipelineTask = targetWriteTask,
                TargetConnectionReference = connection,
            };
            model.TargetWriteTaskList.Add(targetWrite);
            model.InsertRowsTargetWriteTaskList.Add(new InsertRowsTargetWriteTask
            {
                Id = $"{targetWrite.Id}:insert-rows",
                TargetWriteTask = targetWrite,
                TargetSqlIdentifier = pipelineSeed.InsertRowsTarget,
            });
            model.RowStreamConsumerList.Add(new RowStreamConsumer
            {
                Id = $"{targetWriteTask.Id}:consumer",
                PipelineTask = targetWriteTask,
                RowStream = rowStream,
            });
            model.TaskDependencyList.Add(new MetaPipeline.TaskDependency
            {
                Id = $"{transformTask.Id}:before:{targetWriteTask.Id}",
                Pipeline = pipeline,
                Predecessor = transformTask,
                Successor = targetWriteTask,
            });
        }

        model.SaveToXmlWorkspace(pipelineWorkspace);
    }

    private static TransformScript ResolveScript(MetaTransformScriptModel model, string name) =>
        model.TransformScriptList.Single(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));

    private static (int ExitCode, string Output) RunCli(string arguments, string? pathPrefix = null) =>
        CliTestRunner.RunStandardCli("MetaOrchestration", "meta-orchestration.exe", arguments, pathPrefix);

    private static string CreateTempRoot()
    {
        return Path.Combine(Path.GetTempPath(), "MetaOrchestration.Tests", Guid.NewGuid().ToString("N"));
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
