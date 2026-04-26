using Meta.Core.Connections;

internal static partial class Program
{
    private static async Task<int> RunExecuteAsync(string[] args)
    {
        if (args.Length >= 2 && IsHelpToken(args[1]))
        {
            PrintExecuteHelp();
            return 0;
        }

        var parse = ParseExecutePipelineArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-pipeline execute --help");
        }

        MetaPipeline.MetaPipelineModeledSqlServerExecutionPlan? plan = null;
        try
        {
            plan = new MetaPipeline.MetaPipelineModeledSqlServerExecutionResolver().Resolve(
                new MetaPipeline.MetaPipelineModeledSqlServerExecutionRequest(
                    parse.PipelineWorkspacePath,
                    parse.PipelineName,
                    parse.TaskName,
                    parse.TransformWorkspacePath,
                    parse.BindingWorkspacePath));

            var sourceConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
                plan.SourceConnectionEnvironmentVariableName);
            var targetConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
                plan.TargetConnectionEnvironmentVariableName);

            var result = await new MetaPipeline.MetaPipelineSqlServerExecutionService().ExecuteAsync(
                new MetaPipeline.MetaPipelineSqlServerExecutionRequest(
                    plan.TransformWorkspacePath,
                    plan.BindingWorkspacePath,
                    sourceConnectionString,
                    targetConnectionString,
                    plan.TransformScriptId,
                    plan.TransformBindingId,
                    plan.TargetSqlIdentifier,
                    plan.BatchSize)).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return Fail(
                    "pipeline execution failed.",
                    "check the modeled task, selected script, target, and reachable databases, then retry.",
                    4,
                    new[]
                    {
                        $"  Pipeline: {plan.PipelineName}",
                        $"  Task: {plan.TransformTaskName}",
                        $"  TargetWriteTask: {plan.TargetWriteTaskName}",
                        $"  TargetWrite: {plan.TargetWriteModelName}",
                        $"  Script: {result.TransformScriptName}",
                        $"  Target: {result.TargetSqlIdentifier}",
                        $"  TargetWriteRealization: {result.TargetWriteOperationName}",
                        $"  CompletedRows: {result.RowCount}",
                        $"  CompletedBatches: {result.BatchCount}",
                        $"  FailureStage: {result.FailureStage}",
                        $"  Failure: {result.FailureMessage}",
                    });
            }

            Presenter.WriteOk($"Executed pipeline task {plan.TransformTaskName}");
            Presenter.WriteKeyValueBlock("Execution", new[]
            {
                ("Pipeline", plan.PipelineName),
                ("Task", plan.TransformTaskName),
                ("TargetWriteTask", plan.TargetWriteTaskName),
                ("TargetWrite", plan.TargetWriteModelName),
                ("TargetWriteRealization", result.TargetWriteOperationName),
                ("Status", result.Status.ToString()),
                ("Script", result.TransformScriptName),
                ("Target", result.TargetSqlIdentifier),
                ("Columns", result.ColumnCount.ToString()),
                ("Rows", result.RowCount.ToString()),
                ("Batches", result.BatchCount.ToString()),
            });
            return 0;
        }
        catch (ConnectionEnvironmentVariableException ex)
        {
            return Fail(ex.Message, "set the named connection environment variable and retry.");
        }
        catch (MetaPipeline.MetaPipelineConfigurationException ex)
        {
            return Fail(
                ex.Message,
                "check the MetaPipeline workspace, transform/binding workspaces, and retry.",
                4,
                new[]
                {
                    $"  Workspace: {Path.GetFullPath(parse.PipelineWorkspacePath)}",
                    $"  Pipeline: {parse.PipelineName}",
                    $"  Task: {parse.TaskName}",
                });
        }
        catch (Exception ex)
        {
            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(parse.PipelineWorkspacePath)}",
                $"  Pipeline: {parse.PipelineName}",
                $"  Task: {parse.TaskName}",
                $"  {ex.Message}",
            };
            if (plan is not null)
            {
                details.Insert(3, $"  SourceConnectionEnv: {plan.SourceConnectionEnvironmentVariableName}");
                details.Insert(4, $"  TargetConnectionEnv: {plan.TargetConnectionEnvironmentVariableName}");
            }

            return Fail(
                "pipeline execution failed.",
                "check the MetaPipeline workspace, connection environment variables, and reachable databases, then retry.",
                4,
                details);
        }
    }

    private static async Task<int> RunExecuteSqlServerAsync(string[] args)
    {
        if (args.Length >= 2 && IsHelpToken(args[1]))
        {
            PrintExecuteSqlServerHelp();
            return 0;
        }

        var parse = ParseExecuteSqlServerArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-pipeline execute-sqlserver --help");
        }

        try
        {
            var sourceConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(parse.SourceConnectionEnvironmentVariableName);
            var targetConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(parse.TargetConnectionEnvironmentVariableName);

            var result = await new MetaPipeline.MetaPipelineSqlServerExecutionService().ExecuteAsync(
                new MetaPipeline.MetaPipelineSqlServerExecutionRequest(
                    parse.TransformWorkspacePath,
                    parse.BindingWorkspacePath,
                    sourceConnectionString,
                    targetConnectionString,
                    parse.TransformScriptId,
                    parse.TransformBindingId,
                    parse.TargetSqlIdentifier,
                    parse.BatchSize)).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return Fail(
                    "sqlserver execution failed.",
                    "check the selected script, target, and reachable databases, then retry.",
                    4,
                    new[]
                    {
                        $"  Script: {result.TransformScriptName}",
                        $"  Target: {result.TargetSqlIdentifier}",
                        $"  TargetWrite: {result.TargetWriteOperationName}",
                        $"  CompletedRows: {result.RowCount}",
                        $"  CompletedBatches: {result.BatchCount}",
                        $"  FailureStage: {result.FailureStage}",
                        $"  Failure: {result.FailureMessage}",
                    });
            }

            Presenter.WriteOk($"Executed {result.RowCount} row{(result.RowCount == 1 ? string.Empty : "s")}");
            Presenter.WriteKeyValueBlock("Execution", new[]
            {
                ("Status", result.Status.ToString()),
                ("Script", result.TransformScriptName),
                ("Target", result.TargetSqlIdentifier),
                ("TargetWrite", result.TargetWriteOperationName),
                ("Columns", result.ColumnCount.ToString()),
                ("Rows", result.RowCount.ToString()),
                ("Batches", result.BatchCount.ToString()),
            });
            return 0;
        }
        catch (ConnectionEnvironmentVariableException ex)
        {
            return Fail(ex.Message, "set the named connection environment variable and retry.");
        }
        catch (MetaPipeline.MetaPipelineConfigurationException ex)
        {
            return Fail(
                ex.Message,
                "check the transform/binding workspaces and retry.",
                4,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  BindingWorkspace: {Path.GetFullPath(parse.BindingWorkspacePath)}",
                });
        }
        catch (Exception ex)
        {
            return Fail(
                "sqlserver execution failed.",
                "check the workspaces, connection environment variables, and reachable databases, then retry.",
                4,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  BindingWorkspace: {Path.GetFullPath(parse.BindingWorkspacePath)}",
                    $"  SourceConnectionEnv: {parse.SourceConnectionEnvironmentVariableName}",
                    $"  TargetConnectionEnv: {parse.TargetConnectionEnvironmentVariableName}",
                    $"  {ex.Message}",
                });
        }
    }
}
