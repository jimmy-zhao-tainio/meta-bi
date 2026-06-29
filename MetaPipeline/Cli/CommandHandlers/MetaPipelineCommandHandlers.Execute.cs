using MetaCli.Core;

internal sealed partial class MetaPipelineCommandHandlers
{
    private async Task<int> RunExecuteAsync(
        MetaCliInvocation invocation,
        MetaPipeline.MetaPipelineModel pipelineModel)
    {
        var parse = ReadExecutePipelineArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("execute"));
        }

        var outcome = await pipelineExecutionService.ExecutePipelineAsync(
            new MetaPipeline.MetaPipelineExecutionPipelineCommandRequest(
                pipelineModel,
                parse.PipelineWorkspacePath,
                parse.PipelineName,
                parse.TransformWorkspacePath,
                parse.BindingWorkspacePath,
                parse.DataTypeConversionWorkspacePath,
                parse.PipelineDbConnectionEnvironmentVariableName),
            CreateProgress)
            .ConfigureAwait(false);

        return WriteExecuteOutcome(outcome, parse.PipelineDbConnectionEnvironmentVariableName);
    }

    private async Task<int> RunExecuteWorkerAsync(
        MetaCliInvocation invocation,
        MetaPipeline.MetaPipelineModel pipelineModel)
    {
        var parse = ReadExecutePipelineArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("execute-worker"));
        }

        if (string.IsNullOrWhiteSpace(parse.WorkerControlPipeName))
        {
            return Fail(
                "Cannot execute pipeline worker.",
                "start execute-worker through meta-orchestration so a named pipe control channel is provided.",
                4,
                ["  Missing required option --control-pipe <name>."]);
        }

        var outcome = await pipelineExecutionService.ExecuteWorkerAsync(
            new MetaPipeline.MetaPipelineWorkerExecutionCommandRequest(
                pipelineModel,
                parse.PipelineWorkspacePath,
                parse.PipelineName,
                parse.TransformWorkspacePath,
                parse.BindingWorkspacePath,
                parse.DataTypeConversionWorkspacePath,
                parse.PipelineDbConnectionEnvironmentVariableName,
                parse.WorkerControlPipeName,
                parse.ControlPipeConnectTimeoutSeconds))
            .ConfigureAwait(false);

        return WriteExecuteWorkerOutcome(outcome, parse.PipelineDbConnectionEnvironmentVariableName);
    }

    private async Task<int> RunExecuteStepAsync(
        MetaCliInvocation invocation,
        MetaPipeline.MetaPipelineModel pipelineModel)
    {
        var parse = ReadExecuteStepArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("execute-step"));
        }

        var outcome = await pipelineExecutionService.ExecuteStepAsync(
            new MetaPipeline.MetaPipelineExecutionStepCommandRequest(
                pipelineModel,
                parse.PipelineWorkspacePath,
                parse.PipelineName,
                parse.StepName,
                parse.TransformWorkspacePath,
                parse.BindingWorkspacePath,
                parse.DataTypeConversionWorkspacePath,
                parse.PipelineDbConnectionEnvironmentVariableName),
            CreateProgress)
            .ConfigureAwait(false);

        return WriteExecuteStepOutcome(outcome, parse.PipelineDbConnectionEnvironmentVariableName);
    }

    private async Task<int> RunExecuteSqlServerAsync(MetaCliInvocation invocation)
    {
        var parse = ReadExecuteSqlServerArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("execute-sqlserver"));
        }

        var outcome = await pipelineExecutionService.ExecuteSqlServerAsync(
            new MetaPipeline.MetaPipelineDirectSqlServerExecutionCommandRequest(
                parse.TransformWorkspacePath,
                parse.BindingWorkspacePath,
                parse.ExecutionConnectionEnvironmentVariableName,
                parse.TargetConnectionEnvironmentVariableName,
                parse.Script,
                parse.Binding,
                parse.TargetSqlIdentifier,
                parse.BatchSize,
                parse.TimeoutSeconds,
                parse.TargetDataTypeSystemName,
                parse.DataTypeConversionWorkspacePath,
                parse.PipelineDbConnectionEnvironmentVariableName),
            CreateProgress)
            .ConfigureAwait(false);

        return WriteExecuteSqlServerOutcome(outcome, parse.PipelineDbConnectionEnvironmentVariableName);
    }

    private static MetaPipeline.IMetaPipelineExecutionProgress? CreateProgress(int totalSteps) =>
        PipelineConsoleProgressRenderer.TryCreate(totalSteps);

    private int WriteExecuteOutcome(
        MetaPipeline.MetaPipelineExecutionCommandOutcome outcome,
        string pipelineDbConnectionEnvironmentVariableName)
    {
        if (outcome.Succeeded)
        {
            WriteOkWhenNoLiveProgress(outcome);
            return 0;
        }

        return outcome.Status switch
        {
            MetaPipeline.MetaPipelineExecutionCommandStatus.ExecutionFailed => Fail(
                "Cannot execute pipeline.",
                "check the modeled task, selected script, target, and reachable databases, then retry.",
                4,
                BuildPipelineFailureDetails(outcome)),
            MetaPipeline.MetaPipelineExecutionCommandStatus.ValidationFailed => Fail(
                "Cannot validate pipeline model.",
                "fix the modeled pipeline task graph/details and retry execute.",
                4,
                outcome.Details),
            MetaPipeline.MetaPipelineExecutionCommandStatus.ConnectionFailure => Fail(
                "Cannot execute pipeline.",
                "set the named connection environment variable and retry.",
                details: outcome.Details),
            MetaPipeline.MetaPipelineExecutionCommandStatus.ConfigurationFailure => Fail(
                "Cannot configure pipeline execution.",
                "check the MetaPipeline workspace, transform/binding workspaces, and retry.",
                4,
                outcome.Details),
            MetaPipeline.MetaPipelineExecutionCommandStatus.OperationalDbUnavailable => WriteOperationalDbUnavailable(
                pipelineDbConnectionEnvironmentVariableName,
                outcome.Details),
            _ => Fail(
                "Cannot execute pipeline.",
                "check the MetaPipeline workspace, connection environment variables, and reachable databases, then retry.",
                4,
                outcome.Details),
        };
    }

    private int WriteExecuteStepOutcome(
        MetaPipeline.MetaPipelineExecutionCommandOutcome outcome,
        string pipelineDbConnectionEnvironmentVariableName)
    {
        if (outcome.Succeeded)
        {
            WriteOkWhenNoLiveProgress(outcome);
            return 0;
        }

        return outcome.Status switch
        {
            MetaPipeline.MetaPipelineExecutionCommandStatus.ExecutionFailed => Fail(
                "Cannot execute pipeline step.",
                "check the modeled step, selected script, target, and reachable databases, then retry.",
                4,
                BuildStepFailureDetails(outcome)),
            MetaPipeline.MetaPipelineExecutionCommandStatus.ConnectionFailure => Fail(
                "Cannot execute pipeline step.",
                "set the named connection environment variable and retry.",
                details: outcome.Details),
            MetaPipeline.MetaPipelineExecutionCommandStatus.ConfigurationFailure => Fail(
                "Cannot configure pipeline step.",
                "check the MetaPipeline workspace, transform/binding workspaces, and retry.",
                4,
                outcome.Details),
            MetaPipeline.MetaPipelineExecutionCommandStatus.OperationalDbUnavailable => WriteOperationalDbUnavailable(
                pipelineDbConnectionEnvironmentVariableName,
                outcome.Details),
            _ => Fail(
                "Cannot execute pipeline step.",
                "check the MetaPipeline workspace, connection environment variables, and reachable databases, then retry.",
                4,
                outcome.Details),
        };
    }

    private int WriteExecuteWorkerOutcome(
        MetaPipeline.MetaPipelineExecutionCommandOutcome outcome,
        string pipelineDbConnectionEnvironmentVariableName)
    {
        if (outcome.Succeeded)
        {
            return 0;
        }

        return outcome.Status switch
        {
            MetaPipeline.MetaPipelineExecutionCommandStatus.ExecutionFailed => 4,
            MetaPipeline.MetaPipelineExecutionCommandStatus.ValidationFailed => Fail(
                "Cannot validate pipeline model.",
                "fix the modeled pipeline task graph/details and retry execute-worker.",
                4,
                outcome.Details),
            MetaPipeline.MetaPipelineExecutionCommandStatus.ConnectionFailure => Fail(
                "Cannot execute pipeline worker.",
                "set the named connection environment variable and retry.",
                details: outcome.Details),
            MetaPipeline.MetaPipelineExecutionCommandStatus.ConfigurationFailure => Fail(
                "Cannot configure pipeline worker.",
                "check the MetaPipeline workspace, transform/binding workspaces, and orchestration worker protocol.",
                4,
                outcome.Details),
            MetaPipeline.MetaPipelineExecutionCommandStatus.OperationalDbUnavailable => WriteOperationalDbUnavailable(
                pipelineDbConnectionEnvironmentVariableName,
                outcome.Details),
            _ => Fail(
                "Cannot execute pipeline worker.",
                "check the MetaPipeline workspace, connection environment variables, worker protocol, and reachable databases.",
                4,
                outcome.Details),
        };
    }

    private int WriteExecuteSqlServerOutcome(
        MetaPipeline.MetaPipelineExecutionCommandOutcome outcome,
        string pipelineDbConnectionEnvironmentVariableName)
    {
        if (outcome.Succeeded)
        {
            WriteOkWhenNoLiveProgress(outcome);
            return 0;
        }

        return outcome.Status switch
        {
            MetaPipeline.MetaPipelineExecutionCommandStatus.ExecutionFailed => Fail(
                "Cannot execute SQL Server transform.",
                "check the selected script, target, and reachable databases, then retry.",
                4,
                BuildSqlServerFailureDetails(outcome)),
            MetaPipeline.MetaPipelineExecutionCommandStatus.ConnectionFailure => Fail(
                "Cannot execute SQL Server transform.",
                "set the named connection environment variable and retry.",
                details: outcome.Details),
            MetaPipeline.MetaPipelineExecutionCommandStatus.ConfigurationFailure => Fail(
                "Cannot configure SQL Server transform.",
                "check the transform/binding workspaces and retry.",
                4,
                outcome.Details),
            MetaPipeline.MetaPipelineExecutionCommandStatus.OperationalDbUnavailable => WriteOperationalDbUnavailable(
                pipelineDbConnectionEnvironmentVariableName,
                outcome.Details),
            _ => Fail(
                "Cannot execute SQL Server transform.",
                "check the workspaces, connection environment variables, and reachable databases, then retry.",
                4,
                outcome.Details),
        };
    }

    private void WriteOkWhenNoLiveProgress(MetaPipeline.MetaPipelineExecutionCommandOutcome outcome)
    {
        if (!outcome.ProgressRendered)
        {
            presenter.WriteOk();
        }
    }

    private int WriteOperationalDbUnavailable(
        string pipelineDbConnectionEnvironmentVariableName,
        IEnumerable<string> details) =>
        Fail(
            "MetaPipeline operational DB is not available.",
            $"create or choose the operational database, set {pipelineDbConnectionEnvironmentVariableName}, then run meta-pipeline create-pipeline-db --pipeline-db-connection-env <admin-env> --pipeline-db-name MetaPipeline.",
            4,
            details);

    private static IReadOnlyList<string> BuildPipelineFailureDetails(
        MetaPipeline.MetaPipelineExecutionCommandOutcome outcome)
    {
        if (outcome.Plan is null || outcome.Result is null)
        {
            return outcome.Details;
        }

        var details = new List<string>
        {
            $"  Pipeline: {outcome.Plan.PipelineName}",
            $"  Tasks: {outcome.Plan.Steps.Count}",
            $"  TargetWrite: {outcome.Result.TargetWriteModelName}",
            $"  Script: {outcome.Result.TransformScriptName}",
            $"  Target: {outcome.Result.TargetSqlIdentifier}",
            $"  TargetWriteRealization: {outcome.Result.TargetWriteOperationName}",
            $"  CompletedRows: {outcome.Result.RowCount}",
            $"  CompletedBatches: {outcome.Result.BatchCount}",
            $"  FailureStage: {outcome.Result.FailureStage}",
            $"  FailureTask: {outcome.Result.FailureTaskName}",
            $"  Failure: {outcome.Result.FailureMessage}",
        };
        if (outcome.OperationalRunId is Guid failedRunId)
        {
            details.Insert(0, $"  PipelineRunId: {failedRunId}");
        }

        return details;
    }

    private static IReadOnlyList<string> BuildStepFailureDetails(
        MetaPipeline.MetaPipelineExecutionCommandOutcome outcome)
    {
        if (outcome.Plan is null || outcome.Result is null)
        {
            return outcome.Details;
        }

        var details = new List<string>
        {
            $"  Pipeline: {outcome.Plan.PipelineName}",
            $"  Step: {outcome.Plan.TransformTaskName}",
            $"  TargetWrite: {outcome.Result.TargetWriteModelName}",
            $"  Script: {outcome.Result.TransformScriptName}",
            $"  Target: {outcome.Result.TargetSqlIdentifier}",
            $"  TargetWriteRealization: {outcome.Result.TargetWriteOperationName}",
            $"  CompletedRows: {outcome.Result.RowCount}",
            $"  CompletedBatches: {outcome.Result.BatchCount}",
            $"  FailureStage: {outcome.Result.FailureStage}",
            $"  FailureTask: {outcome.Result.FailureTaskName}",
            $"  Failure: {outcome.Result.FailureMessage}",
        };
        if (outcome.OperationalRunId is Guid failedRunId)
        {
            details.Insert(0, $"  PipelineRunId: {failedRunId}");
        }

        return details;
    }

    private static IReadOnlyList<string> BuildSqlServerFailureDetails(
        MetaPipeline.MetaPipelineExecutionCommandOutcome outcome)
    {
        if (outcome.Result is null)
        {
            return outcome.Details;
        }

        var details = new List<string>
        {
            $"  Script: {outcome.Result.TransformScriptName}",
            $"  Target: {outcome.Result.TargetSqlIdentifier}",
            $"  TargetWrite: {outcome.Result.TargetWriteOperationName}",
            $"  CompletedRows: {outcome.Result.RowCount}",
            $"  CompletedBatches: {outcome.Result.BatchCount}",
            $"  FailureStage: {outcome.Result.FailureStage}",
            $"  FailureTask: {outcome.Result.FailureTaskName}",
            $"  Failure: {outcome.Result.FailureMessage}",
        };
        if (outcome.OperationalRunId is Guid failedRunId)
        {
            details.Insert(0, $"  PipelineRunId: {failedRunId}");
        }

        return details;
    }
}
