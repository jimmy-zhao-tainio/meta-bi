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
            return Fail(parse.ErrorMessage, HelpCommand("execute"));
        }

        MetaPipeline.MetaPipelineModeledSqlServerExecutionPlan? plan = null;
        MetaPipeline.MetaPipelineOperationalDbStore? operationalDb = null;
        Guid? operationalRunId = null;
        PipelineConsoleProgressRenderer? progress = null;
        try
        {
            var pipelineWorkspacePath = Path.GetFullPath(parse.PipelineWorkspacePath);
            operationalDb = CreateOperationalDbStore(parse.PipelineDbConnectionEnvironmentVariableName);
            if (operationalDb is not null)
            {
                operationalRunId = await operationalDb.StartRunAsync(
                    new MetaPipeline.MetaPipelineOperationalRunStart(
                        PipelineWorkspacePath: pipelineWorkspacePath,
                        PipelineName: parse.PipelineName,
                        TransformWorkspacePath: Path.GetFullPath(parse.TransformWorkspacePath),
                        BindingWorkspacePath: Path.GetFullPath(parse.BindingWorkspacePath)))
                    .ConfigureAwait(false);
            }

            var pipelineModel = MetaPipeline.MetaPipelineModel.LoadFromXmlWorkspace(pipelineWorkspacePath, searchUpward: false);
            var validation = new MetaPipeline.MetaPipelineModelValidationService()
                .ValidatePipeline(pipelineModel, parse.PipelineName);
            if (!validation.IsValid)
            {
                var details = validation.Errors.Select(static item => "  " + item).ToList();
                details.AddRange(await RecordOperationalFailureAsync(
                    operationalDb,
                    operationalRunId,
                    "Validation",
                    "Configuration",
                    string.Join(Environment.NewLine, validation.Errors),
                    new MetaPipeline.MetaPipelineConfigurationException("Cannot validate pipeline model."))
                    .ConfigureAwait(false));

                return Fail(
                    "Cannot validate pipeline model.",
                    "fix the modeled pipeline task graph/details and retry execute.",
                    4,
                    details);
            }

            plan = new MetaPipeline.MetaPipelineModeledSqlServerExecutionResolver().Resolve(
                new MetaPipeline.MetaPipelineModeledSqlServerExecutionRequest(
                    parse.PipelineWorkspacePath,
                    parse.PipelineName,
                    parse.TransformWorkspacePath,
                    parse.BindingWorkspacePath));

            if (operationalDb is not null && operationalRunId is Guid startedRunId)
            {
                await operationalDb.UpdateRunContextAsync(
                    startedRunId,
                    new MetaPipeline.MetaPipelineOperationalRunStart(
                        PipelineWorkspacePath: Path.GetFullPath(plan.PipelineWorkspacePath),
                        PipelineId: plan.PipelineId,
                        PipelineName: plan.PipelineName,
                        TransformTaskId: plan.TransformTaskId,
                        TransformTaskName: plan.TransformTaskName,
                        TargetWriteTaskId: plan.TargetWriteTaskId,
                        TargetWriteTaskName: plan.TargetWriteTaskName,
                        TransformWorkspacePath: Path.GetFullPath(plan.TransformWorkspacePath),
                        BindingWorkspacePath: Path.GetFullPath(plan.BindingWorkspacePath),
                        TransformScriptId: plan.TransformScriptId,
                        TransformBindingId: plan.TransformBindingId,
                        TransformScriptName: plan.TransformScriptName,
                        ExecutionConnectionReferenceName: plan.ExecutionConnectionReferenceName,
                        ExecutionConnectionEnvironmentVariableName: plan.ExecutionConnectionEnvironmentVariableName,
                        TargetConnectionReferenceName: plan.TargetConnectionReferenceName,
                        TargetConnectionEnvironmentVariableName: plan.TargetConnectionEnvironmentVariableName,
                        TargetSqlIdentifier: plan.TargetSqlIdentifier,
                        TargetWriteModelName: plan.TargetWriteModelName,
                        BatchSize: plan.BatchSize))
                    .ConfigureAwait(false);
            }

            MetaPipeline.MetaPipelineExecutionResult result;
            progress = PipelineConsoleProgressRenderer.TryCreate(plan.Steps.Count);
            result = await ExecuteModeledPlanAsync(
                plan,
                operationalDb,
                operationalRunId,
                parse.DataTypeConversionWorkspacePath,
                progress).ConfigureAwait(false);

            if (operationalDb is not null && operationalRunId is Guid completedRunId)
            {
                await operationalDb.CompleteRunAsync(
                    completedRunId,
                    result,
                    BuildModeledOperationalFingerprints(plan, parse.DataTypeConversionWorkspacePath))
                    .ConfigureAwait(false);
            }

            if (!result.Succeeded)
            {
                progress?.Complete(failed: true);
                progress?.Dispose();
                var failureDetails = new List<string>
                {
                    $"  Pipeline: {plan.PipelineName}",
                    $"  Tasks: {plan.Steps.Count}",
                    $"  TargetWrite: {result.TargetWriteModelName}",
                    $"  Script: {result.TransformScriptName}",
                    $"  Target: {result.TargetSqlIdentifier}",
                    $"  TargetWriteRealization: {result.TargetWriteOperationName}",
                    $"  CompletedRows: {result.RowCount}",
                    $"  CompletedBatches: {result.BatchCount}",
                    $"  FailureStage: {result.FailureStage}",
                    $"  FailureTask: {result.FailureTaskName}",
                    $"  Failure: {result.FailureMessage}",
                };
                if (operationalRunId is Guid failedRunId)
                {
                    failureDetails.Insert(0, $"  PipelineRunId: {failedRunId}");
                }

                return Fail(
                    "Cannot execute pipeline.",
                    "check the modeled task, selected script, target, and reachable databases, then retry.",
                    4,
                    failureDetails);
            }

            progress?.Complete(failed: false);
            progress?.Dispose();
            if (progress is null)
            {
                Presenter.WriteOk();
            }

            return 0;
        }
        catch (ConnectionEnvironmentVariableException ex)
        {
            progress?.Complete(failed: true);
            progress?.Dispose();
            var details = new List<string> { $"  {ex.Message}" };
            details.AddRange(await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Connection",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false));
            return Fail("Cannot execute pipeline.", "set the named connection environment variable and retry.", details: details);
        }
        catch (MetaPipeline.MetaPipelineConfigurationException ex)
        {
            progress?.Complete(failed: true);
            progress?.Dispose();
            var dbDetails = await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Configuration",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false);
            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(parse.PipelineWorkspacePath)}",
                $"  Pipeline: {parse.PipelineName}",
            };
            details.AddRange(dbDetails);

            return Fail(
                "Cannot configure pipeline execution.",
                "check the MetaPipeline workspace, transform/binding workspaces, and retry.",
                4,
                details.Append($"  {ex.Message}"));
        }
        catch (Exception ex)
        {
            progress?.Complete(failed: true);
            progress?.Dispose();
            if (IsOperationalDbStartupFailure(operationalDb, operationalRunId))
            {
                return Fail(
                    "MetaPipeline operational DB is not available.",
                    $"create or choose the operational database, set {parse.PipelineDbConnectionEnvironmentVariableName}, then run meta-pipeline create-pipeline-db --pipeline-db-connection-env <admin-env> --pipeline-db-name MetaPipeline.",
                    4,
                    new[]
                    {
                        $"  PipelineDbConnectionEnv: {parse.PipelineDbConnectionEnvironmentVariableName}",
                        $"  {ex.Message}",
                    });
            }

            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(parse.PipelineWorkspacePath)}",
                $"  Pipeline: {parse.PipelineName}",
                $"  {ex.Message}",
            };
            if (plan is not null)
            {
                details.Insert(3, $"  ExecutionConnectionEnv: {plan.ExecutionConnectionEnvironmentVariableName}");
                details.Insert(4, $"  TargetConnectionEnv: {plan.TargetConnectionEnvironmentVariableName}");
            }

            return Fail(
                "Cannot execute pipeline.",
                "check the MetaPipeline workspace, connection environment variables, and reachable databases, then retry.",
                4,
                details.Concat(await RecordOperationalFailureAsync(
                    operationalDb,
                    operationalRunId,
                    "Unexpected",
                    "Exception",
                    ex.Message,
                    ex).ConfigureAwait(false)));
        }
    }

    private static async Task<int> RunExecuteStepAsync(string[] args)
    {
        if (args.Length >= 2 && IsHelpToken(args[1]))
        {
            PrintExecuteStepHelp();
            return 0;
        }

        var parse = ParseExecuteStepArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("execute-step"));
        }

        MetaPipeline.MetaPipelineModeledSqlServerExecutionPlan? plan = null;
        MetaPipeline.MetaPipelineOperationalDbStore? operationalDb = null;
        Guid? operationalRunId = null;
        PipelineConsoleProgressRenderer? progress = null;
        try
        {
            var pipelineWorkspacePath = Path.GetFullPath(parse.PipelineWorkspacePath);
            operationalDb = CreateOperationalDbStore(parse.PipelineDbConnectionEnvironmentVariableName);
            if (operationalDb is not null)
            {
                operationalRunId = await operationalDb.StartRunAsync(
                    new MetaPipeline.MetaPipelineOperationalRunStart(
                        PipelineWorkspacePath: pipelineWorkspacePath,
                        PipelineName: parse.PipelineName,
                        TransformWorkspacePath: Path.GetFullPath(parse.TransformWorkspacePath),
                        BindingWorkspacePath: Path.GetFullPath(parse.BindingWorkspacePath)))
                    .ConfigureAwait(false);
            }

            plan = new MetaPipeline.MetaPipelineModeledSqlServerExecutionResolver().ResolveStep(
                new MetaPipeline.MetaPipelineModeledSqlServerExecutionStepRequest(
                    parse.PipelineWorkspacePath,
                    parse.PipelineName,
                    parse.StepName,
                    parse.TransformWorkspacePath,
                    parse.BindingWorkspacePath));

            if (operationalDb is not null && operationalRunId is Guid startedRunId)
            {
                await operationalDb.UpdateRunContextAsync(
                    startedRunId,
                    new MetaPipeline.MetaPipelineOperationalRunStart(
                        PipelineWorkspacePath: Path.GetFullPath(plan.PipelineWorkspacePath),
                        PipelineId: plan.PipelineId,
                        PipelineName: plan.PipelineName,
                        TransformTaskId: plan.TransformTaskId,
                        TransformTaskName: plan.TransformTaskName,
                        TargetWriteTaskId: plan.TargetWriteTaskId,
                        TargetWriteTaskName: plan.TargetWriteTaskName,
                        TransformWorkspacePath: Path.GetFullPath(plan.TransformWorkspacePath),
                        BindingWorkspacePath: Path.GetFullPath(plan.BindingWorkspacePath),
                        TransformScriptId: plan.TransformScriptId,
                        TransformBindingId: plan.TransformBindingId,
                        TransformScriptName: plan.TransformScriptName,
                        ExecutionConnectionReferenceName: plan.ExecutionConnectionReferenceName,
                        ExecutionConnectionEnvironmentVariableName: plan.ExecutionConnectionEnvironmentVariableName,
                        TargetConnectionReferenceName: plan.TargetConnectionReferenceName,
                        TargetConnectionEnvironmentVariableName: plan.TargetConnectionEnvironmentVariableName,
                        TargetSqlIdentifier: plan.TargetSqlIdentifier,
                        TargetWriteModelName: plan.TargetWriteModelName,
                        BatchSize: plan.BatchSize))
                    .ConfigureAwait(false);
            }

            MetaPipeline.MetaPipelineExecutionResult result;
            progress = PipelineConsoleProgressRenderer.TryCreate(1);
            result = await ExecuteModeledPlanAsync(
                plan,
                operationalDb,
                operationalRunId,
                parse.DataTypeConversionWorkspacePath,
                progress).ConfigureAwait(false);

            if (operationalDb is not null && operationalRunId is Guid completedRunId)
            {
                await operationalDb.CompleteRunAsync(
                    completedRunId,
                    result,
                    BuildModeledOperationalFingerprints(plan, parse.DataTypeConversionWorkspacePath))
                    .ConfigureAwait(false);
            }

            if (!result.Succeeded)
            {
                progress?.Complete(failed: true);
                progress?.Dispose();
                var failureDetails = new List<string>
                {
                    $"  Pipeline: {plan.PipelineName}",
                    $"  Step: {plan.TransformTaskName}",
                    $"  TargetWrite: {result.TargetWriteModelName}",
                    $"  Script: {result.TransformScriptName}",
                    $"  Target: {result.TargetSqlIdentifier}",
                    $"  TargetWriteRealization: {result.TargetWriteOperationName}",
                    $"  CompletedRows: {result.RowCount}",
                    $"  CompletedBatches: {result.BatchCount}",
                    $"  FailureStage: {result.FailureStage}",
                    $"  FailureTask: {result.FailureTaskName}",
                    $"  Failure: {result.FailureMessage}",
                };
                if (operationalRunId is Guid failedRunId)
                {
                    failureDetails.Insert(0, $"  PipelineRunId: {failedRunId}");
                }

                return Fail(
                    "Cannot execute pipeline step.",
                    "check the modeled step, selected script, target, and reachable databases, then retry.",
                    4,
                    failureDetails);
            }

            progress?.Complete(failed: false);
            progress?.Dispose();
            if (progress is null)
            {
                Presenter.WriteOk();
            }

            return 0;
        }
        catch (ConnectionEnvironmentVariableException ex)
        {
            progress?.Complete(failed: true);
            progress?.Dispose();
            var details = new List<string> { $"  {ex.Message}" };
            details.AddRange(await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Connection",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false));
            return Fail("Cannot execute pipeline step.", "set the named connection environment variable and retry.", details: details);
        }
        catch (MetaPipeline.MetaPipelineConfigurationException ex)
        {
            progress?.Complete(failed: true);
            progress?.Dispose();
            var dbDetails = await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Configuration",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false);
            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(parse.PipelineWorkspacePath)}",
                $"  Pipeline: {parse.PipelineName}",
                $"  Step: {parse.StepName}",
            };
            details.AddRange(dbDetails);

            return Fail(
                "Cannot configure pipeline step.",
                "check the MetaPipeline workspace, transform/binding workspaces, and retry.",
                4,
                details.Append($"  {ex.Message}"));
        }
        catch (Exception ex)
        {
            progress?.Complete(failed: true);
            progress?.Dispose();
            if (IsOperationalDbStartupFailure(operationalDb, operationalRunId))
            {
                return Fail(
                    "MetaPipeline operational DB is not available.",
                    $"create or choose the operational database, set {parse.PipelineDbConnectionEnvironmentVariableName}, then run meta-pipeline create-pipeline-db --pipeline-db-connection-env <admin-env> --pipeline-db-name MetaPipeline.",
                    4,
                    new[]
                    {
                        $"  PipelineDbConnectionEnv: {parse.PipelineDbConnectionEnvironmentVariableName}",
                        $"  {ex.Message}",
                    });
            }

            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(parse.PipelineWorkspacePath)}",
                $"  Pipeline: {parse.PipelineName}",
                $"  Step: {parse.StepName}",
                $"  {ex.Message}",
            };
            if (plan is not null)
            {
                details.Insert(4, $"  ExecutionConnectionEnv: {plan.ExecutionConnectionEnvironmentVariableName}");
                details.Insert(5, $"  TargetConnectionEnv: {plan.TargetConnectionEnvironmentVariableName}");
            }

            return Fail(
                "Cannot execute pipeline step.",
                "check the MetaPipeline workspace, connection environment variables, and reachable databases, then retry.",
                4,
                details.Concat(await RecordOperationalFailureAsync(
                    operationalDb,
                    operationalRunId,
                    "Unexpected",
                    "Exception",
                    ex.Message,
                    ex).ConfigureAwait(false)));
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
            return Fail(parse.ErrorMessage, HelpCommand("execute-sqlserver"));
        }

        MetaPipeline.MetaPipelineOperationalDbStore? operationalDb = null;
        Guid? operationalRunId = null;
        MetaPipeline.MetaPipelineTransformSelection? selection = null;
        PipelineConsoleProgressRenderer? progress = null;
        try
        {
            operationalDb = CreateOperationalDbStore(parse.PipelineDbConnectionEnvironmentVariableName);
            if (operationalDb is not null)
            {
                operationalRunId = await operationalDb.StartRunAsync(
                    new MetaPipeline.MetaPipelineOperationalRunStart(
                        TransformWorkspacePath: Path.GetFullPath(parse.TransformWorkspacePath),
                        BindingWorkspacePath: Path.GetFullPath(parse.BindingWorkspacePath),
                        ExecutionConnectionEnvironmentVariableName: parse.ExecutionConnectionEnvironmentVariableName,
                        TargetConnectionEnvironmentVariableName: parse.TargetConnectionEnvironmentVariableName,
                        TargetSqlIdentifier: parse.TargetSqlIdentifier,
                        BatchSize: parse.BatchSize))
                    .ConfigureAwait(false);
            }

            selection = new MetaPipeline.MetaPipelineTransformSelectionResolver().Resolve(
                parse.TransformWorkspacePath,
                parse.BindingWorkspacePath,
                parse.Script,
                parse.Binding);
            if (operationalDb is not null && operationalRunId is Guid startedRunId)
            {
                await operationalDb.UpdateRunContextAsync(
                    startedRunId,
                    new MetaPipeline.MetaPipelineOperationalRunStart(
                        TransformScriptId: selection.TransformScriptId,
                        TransformBindingId: selection.TransformBindingId,
                        TransformScriptName: selection.TransformScriptName))
                    .ConfigureAwait(false);
            }

            var executionConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(parse.ExecutionConnectionEnvironmentVariableName);
            var targetConnectionString = string.IsNullOrWhiteSpace(parse.TargetConnectionEnvironmentVariableName)
                ? null
                : ConnectionEnvironmentVariableResolver.ResolveRequired(parse.TargetConnectionEnvironmentVariableName);
            var executionContext = await CreateExecutionContextAsync(
                operationalDb,
                operationalRunId,
                null,
                selection.TransformScriptName,
                "TransformExecution").ConfigureAwait(false);

            MetaPipeline.MetaPipelineExecutionResult result;
            progress = PipelineConsoleProgressRenderer.TryCreate(1);
            try
            {
                progress?.StartStep(1, selection.TransformScriptName);
                result = await new MetaPipeline.MetaPipelineSqlServerExecutionService().ExecuteAsync(
                    new MetaPipeline.MetaPipelineSqlServerExecutionRequest(
                        parse.TransformWorkspacePath,
                        parse.BindingWorkspacePath,
                        executionConnectionString,
                        targetConnectionString,
                        selection.TransformScriptId,
                        selection.TransformBindingId,
                        parse.TargetSqlIdentifier,
                        parse.BatchSize,
                        parse.TimeoutSeconds,
                        ExecutionContext: executionContext,
                        TargetDataTypeSystemName: parse.TargetDataTypeSystemName,
                        DataTypeConversionWorkspacePath: parse.DataTypeConversionWorkspacePath),
                    progress).ConfigureAwait(false);
                progress?.CompleteStep(result.Succeeded, result.RowCount, result.BatchCount);
            }
            catch
            {
                progress?.CompleteStep(succeeded: false);
                throw;
            }
            if (operationalDb is not null && operationalRunId is Guid completedRunId)
            {
                await operationalDb.CompleteRunAsync(
                    completedRunId,
                    result,
                    BuildDirectOperationalFingerprints(parse, selection, result))
                    .ConfigureAwait(false);
            }

            if (!result.Succeeded)
            {
                progress?.Complete(failed: true);
                progress?.Dispose();
                var failureDetails = new List<string>
                {
                    $"  Script: {result.TransformScriptName}",
                    $"  Target: {result.TargetSqlIdentifier}",
                    $"  TargetWrite: {result.TargetWriteOperationName}",
                    $"  CompletedRows: {result.RowCount}",
                    $"  CompletedBatches: {result.BatchCount}",
                    $"  FailureStage: {result.FailureStage}",
                    $"  FailureTask: {result.FailureTaskName}",
                    $"  Failure: {result.FailureMessage}",
                };
                if (operationalRunId is Guid failedRunId)
                {
                    failureDetails.Insert(0, $"  PipelineRunId: {failedRunId}");
                }

                return Fail(
                    "Cannot execute SQL Server transform.",
                    "check the selected script, target, and reachable databases, then retry.",
                    4,
                    failureDetails);
            }

            progress?.Complete(failed: false);
            progress?.Dispose();
            if (progress is null)
            {
                Presenter.WriteOk();
            }

            return 0;
        }
        catch (ConnectionEnvironmentVariableException ex)
        {
            progress?.Complete(failed: true);
            progress?.Dispose();
            var details = new List<string> { $"  {ex.Message}" };
            details.AddRange(await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Connection",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false));
            return Fail("Cannot execute SQL Server transform.", "set the named connection environment variable and retry.", details: details);
        }
        catch (MetaPipeline.MetaPipelineConfigurationException ex)
        {
            progress?.Complete(failed: true);
            progress?.Dispose();
            var dbDetails = await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Configuration",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false);
            var details = new List<string>
            {
                $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
            };
            if (!string.IsNullOrWhiteSpace(parse.BindingWorkspacePath))
            {
                details.Add($"  BindingWorkspace: {Path.GetFullPath(parse.BindingWorkspacePath)}");
            }

            details.AddRange(dbDetails);
            return Fail(
                "Cannot configure SQL Server transform.",
                "check the transform/binding workspaces and retry.",
                4,
                details.Append($"  {ex.Message}"));
        }
        catch (Exception ex)
        {
            progress?.Complete(failed: true);
            progress?.Dispose();
            if (IsOperationalDbStartupFailure(operationalDb, operationalRunId))
            {
                return Fail(
                    "MetaPipeline operational DB is not available.",
                    $"create or choose the operational database, set {parse.PipelineDbConnectionEnvironmentVariableName}, then run meta-pipeline create-pipeline-db --pipeline-db-connection-env <admin-env> --pipeline-db-name MetaPipeline.",
                    4,
                    new[]
                    {
                        $"  PipelineDbConnectionEnv: {parse.PipelineDbConnectionEnvironmentVariableName}",
                        $"  {ex.Message}",
                    });
            }

            var details = new List<string>
            {
                $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                $"  ExecutionConnectionEnv: {parse.ExecutionConnectionEnvironmentVariableName}",
            };
            if (!string.IsNullOrWhiteSpace(parse.BindingWorkspacePath))
            {
                details.Add($"  BindingWorkspace: {Path.GetFullPath(parse.BindingWorkspacePath)}");
            }

            if (!string.IsNullOrWhiteSpace(parse.TargetConnectionEnvironmentVariableName))
            {
                details.Add($"  TargetConnectionEnv: {parse.TargetConnectionEnvironmentVariableName}");
            }

            details.Add($"  {ex.Message}");
            return Fail(
                "Cannot execute SQL Server transform.",
                "check the workspaces, connection environment variables, and reachable databases, then retry.",
                4,
                details.Concat(await RecordOperationalFailureAsync(
                    operationalDb,
                    operationalRunId,
                    "Unexpected",
                    "Exception",
                    ex.Message,
                    ex).ConfigureAwait(false)));
        }
    }

    private static async Task<MetaPipeline.MetaPipelineExecutionResult> ExecuteModeledPlanAsync(
        MetaPipeline.MetaPipelineModeledSqlServerExecutionPlan plan,
        MetaPipeline.MetaPipelineOperationalDbStore? operationalDb,
        Guid? operationalRunId,
        string? dataTypeConversionWorkspacePath,
        PipelineConsoleProgressRenderer? progress)
    {
        var startedAtUtc = DateTimeOffset.UtcNow;
        var taskResults = new List<MetaPipeline.MetaPipelineExecutionTaskResult>();
        long rowCount = 0;
        var batchCount = 0;
        var columnCount = 0;

        for (var index = 0; index < plan.Steps.Count; index++)
        {
            var step = plan.Steps[index];
            progress?.StartStep(index + 1, step.TransformTaskName);
            var executionConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
                step.ExecutionConnectionEnvironmentVariableName);
            var targetConnectionString = step.IsSelect
                ? ConnectionEnvironmentVariableResolver.ResolveRequired(
                    step.TargetConnectionEnvironmentVariableName
                    ?? throw new MetaPipeline.MetaPipelineConfigurationException("SELECT-kind pipeline execution requires a target connection reference."))
                : null;
            var executionContext = await CreateExecutionContextAsync(
                operationalDb,
                operationalRunId,
                plan.PipelineName,
                step.TransformTaskName,
                "TransformExecution").ConfigureAwait(false);

            MetaPipeline.MetaPipelineExecutionResult result;
            try
            {
                result = await new MetaPipeline.MetaPipelineSqlServerExecutionService().ExecuteAsync(
                    new MetaPipeline.MetaPipelineSqlServerExecutionRequest(
                        plan.TransformWorkspacePath,
                        plan.BindingWorkspacePath,
                        executionConnectionString,
                        targetConnectionString,
                        step.TransformScriptId,
                        step.TransformBindingId,
                        step.TargetSqlIdentifier,
                        step.BatchSize,
                        step.TimeoutSeconds,
                        step.TargetWriteModelName,
                        step.TransformTaskName,
                        step.TargetWriteTaskName,
                        executionContext,
                        step.TargetDataTypeSystemName,
                        dataTypeConversionWorkspacePath),
                    progress).ConfigureAwait(false);
            }
            catch
            {
                progress?.CompleteStep(succeeded: false);
                throw;
            }

            taskResults.AddRange(result.TaskResults);
            rowCount += result.RowCount;
            batchCount += result.BatchCount;
            columnCount += result.ColumnCount;
            progress?.CompleteStep(result.Succeeded, result.RowCount, result.BatchCount);

            if (!result.Succeeded)
            {
                AddSkippedFutureTasks(plan, index + 1, taskResults);
                return CreateModeledPlanResult(
                    plan,
                    MetaPipeline.MetaPipelineExecutionStatus.Failed,
                    startedAtUtc,
                    rowCount,
                    batchCount,
                    columnCount,
                    result.FailureStage,
                    result.FailureMessage,
                    result.FailureTaskName,
                    taskResults);
            }
        }

        return CreateModeledPlanResult(
            plan,
            MetaPipeline.MetaPipelineExecutionStatus.Succeeded,
            startedAtUtc,
            rowCount,
            batchCount,
            columnCount,
            MetaPipeline.PipelineExecutionFailureStage.None,
            string.Empty,
            string.Empty,
            taskResults);
    }

    private static MetaPipeline.MetaPipelineExecutionResult CreateModeledPlanResult(
        MetaPipeline.MetaPipelineModeledSqlServerExecutionPlan plan,
        MetaPipeline.MetaPipelineExecutionStatus status,
        DateTimeOffset startedAtUtc,
        long rowCount,
        int batchCount,
        int columnCount,
        MetaPipeline.PipelineExecutionFailureStage failureStage,
        string failureMessage,
        string failureTaskName,
        IReadOnlyList<MetaPipeline.MetaPipelineExecutionTaskResult> taskResults)
    {
        var scriptNames = string.Join(
            " -> ",
            plan.Steps.Select(static item => item.TransformScriptName));
        var targets = string.Join(
            " -> ",
            plan.Steps
                .Select(static item => item.TargetSqlIdentifier)
                .Where(static item => !string.IsNullOrWhiteSpace(item))
                .Cast<string>());
        var targetWriteModels = plan.Steps
            .Select(static item => item.TargetWriteModelName)
            .Where(static item => !string.IsNullOrWhiteSpace(item) && !string.Equals(item, "None", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var targetWriteModelName = targetWriteModels.Length == 0
            ? "None"
            : string.Join(",", targetWriteModels);

        return new MetaPipeline.MetaPipelineExecutionResult(
            status,
            scriptNames,
            targets,
            ResolveModeledPlanOperationName(plan),
            targetWriteModelName,
            columnCount,
            rowCount,
            batchCount,
            startedAtUtc,
            DateTimeOffset.UtcNow,
            failureStage,
            failureMessage,
            failureTaskName,
            taskResults);
    }

    private static string ResolveModeledPlanOperationName(
        MetaPipeline.MetaPipelineModeledSqlServerExecutionPlan plan)
    {
        if (plan.Steps.Count != 1)
        {
            return "SqlServerSerialPipeline";
        }

        return plan.Steps[0].IsSelect
            ? "SqlServerBulkInsert"
            : "SqlServerExecuteNonQuery";
    }

    private static void AddSkippedFutureTasks(
        MetaPipeline.MetaPipelineModeledSqlServerExecutionPlan plan,
        int startIndex,
        ICollection<MetaPipeline.MetaPipelineExecutionTaskResult> taskResults)
    {
        for (var index = startIndex; index < plan.Steps.Count; index++)
        {
            var step = plan.Steps[index];
            taskResults.Add(CreateSkippedTaskResult(step.TransformTaskName, "TransformExecution", step.TimeoutSeconds));
            if (step.IsSelect && !string.IsNullOrWhiteSpace(step.TargetWriteTaskName))
            {
                taskResults.Add(CreateSkippedTaskResult(step.TargetWriteTaskName, "TargetWrite", step.TimeoutSeconds));
            }
        }
    }

    private static MetaPipeline.MetaPipelineExecutionTaskResult CreateSkippedTaskResult(
        string taskName,
        string taskKind,
        int? timeoutSeconds)
    {
        var skippedAtUtc = DateTimeOffset.UtcNow;
        return new MetaPipeline.MetaPipelineExecutionTaskResult(
            taskName,
            taskKind,
            MetaPipeline.MetaPipelineExecutionTaskStatus.Skipped,
            skippedAtUtc,
            skippedAtUtc,
            0,
            0,
            MetaPipeline.PipelineExecutionFailureStage.None,
            string.Empty,
            TimeoutSeconds: timeoutSeconds);
    }

    private static IReadOnlyList<MetaPipeline.MetaPipelineOperationalFingerprint> BuildModeledOperationalFingerprints(
        MetaPipeline.MetaPipelineModeledSqlServerExecutionPlan plan,
        string? dataTypeConversionWorkspacePath)
    {
        var service = new MetaPipeline.MetaPipelineWorkspaceFingerprintService();
        var fingerprints = new List<MetaPipeline.MetaPipelineOperationalFingerprint>
        {
            service.CreateWorkspaceFingerprint(
                "PipelineWorkspace",
                plan.PipelineId,
                plan.PipelineWorkspacePath),
            service.CreateWorkspaceFingerprint(
                "TransformWorkspace",
                "all",
                plan.TransformWorkspacePath),
            service.CreateWorkspaceFingerprint(
                "BindingWorkspace",
                "all",
                plan.BindingWorkspacePath),
        };

        foreach (var step in plan.Steps)
        {
            fingerprints.Add(service.CreateWorkspaceFingerprint(
                "TransformScript",
                step.TransformScriptId,
                plan.TransformWorkspacePath,
                step.TransformTaskName,
                "TransformExecution"));
            fingerprints.Add(service.CreateWorkspaceFingerprint(
                "TransformBinding",
                step.TransformBindingId,
                plan.BindingWorkspacePath,
                step.TransformTaskName,
                "TransformExecution"));

            if (step.IsSelect
                && !string.IsNullOrWhiteSpace(step.TargetWriteTaskName)
                && !string.IsNullOrWhiteSpace(dataTypeConversionWorkspacePath))
            {
                fingerprints.Add(service.CreateWorkspaceFingerprint(
                    "DataTypeConversionWorkspace",
                    step.TargetDataTypeSystemName,
                    dataTypeConversionWorkspacePath,
                    step.TargetWriteTaskName,
                    "TargetWrite"));
            }
        }

        return fingerprints;
    }

    private static IReadOnlyList<MetaPipeline.MetaPipelineOperationalFingerprint> BuildDirectOperationalFingerprints(
        (bool Ok, string TransformWorkspacePath, string BindingWorkspacePath, string ExecutionConnectionEnvironmentVariableName, string TargetConnectionEnvironmentVariableName, string Script, string Binding, string? TargetSqlIdentifier, int BatchSize, int? TimeoutSeconds, string TargetDataTypeSystemName, string DataTypeConversionWorkspacePath, string PipelineDbConnectionEnvironmentVariableName, string ErrorMessage) parse,
        MetaPipeline.MetaPipelineTransformSelection selection,
        MetaPipeline.MetaPipelineExecutionResult result)
    {
        var service = new MetaPipeline.MetaPipelineWorkspaceFingerprintService();
        var fingerprints = new List<MetaPipeline.MetaPipelineOperationalFingerprint>
        {
            service.CreateWorkspaceFingerprint(
                "TransformWorkspace",
                "all",
                parse.TransformWorkspacePath),
            service.CreateWorkspaceFingerprint(
                "BindingWorkspace",
                "all",
                parse.BindingWorkspacePath),
        };

        var transformTask = result.TaskResults.FirstOrDefault(static task =>
            string.Equals(task.TaskKind, "TransformExecution", StringComparison.Ordinal));
        if (transformTask is not null)
        {
            fingerprints.Add(service.CreateWorkspaceFingerprint(
                "TransformScript",
                selection.TransformScriptId,
                parse.TransformWorkspacePath,
                transformTask.TaskName,
                transformTask.TaskKind));
            fingerprints.Add(service.CreateWorkspaceFingerprint(
                "TransformBinding",
                selection.TransformBindingId,
                parse.BindingWorkspacePath,
                transformTask.TaskName,
                transformTask.TaskKind));
        }

        var targetWriteTask = result.TaskResults.FirstOrDefault(static task =>
            string.Equals(task.TaskKind, "TargetWrite", StringComparison.Ordinal));
        if (targetWriteTask is not null && !string.IsNullOrWhiteSpace(parse.DataTypeConversionWorkspacePath))
        {
            fingerprints.Add(service.CreateWorkspaceFingerprint(
                "DataTypeConversionWorkspace",
                parse.TargetDataTypeSystemName,
                parse.DataTypeConversionWorkspacePath,
                targetWriteTask.TaskName,
                targetWriteTask.TaskKind));
        }

        return fingerprints;
    }

    private static async Task<MetaPipeline.MetaPipelineExecutionContext?> CreateExecutionContextAsync(
        MetaPipeline.MetaPipelineOperationalDbStore? operationalDb,
        Guid? operationalRunId,
        string? pipelineName,
        string? taskName,
        string taskKind)
    {
        if (operationalDb is null || operationalRunId is not Guid runId)
        {
            return null;
        }

        return new MetaPipeline.MetaPipelineExecutionContext(
            runId,
            Guid.NewGuid(),
            await operationalDb.ReserveAuditIdAsync().ConfigureAwait(false),
            DateTimeOffset.UtcNow,
            pipelineName,
            taskName,
            taskKind);
    }

    private static MetaPipeline.MetaPipelineOperationalDbStore? CreateOperationalDbStore(string connectionEnvironmentVariableName)
    {
        if (string.IsNullOrWhiteSpace(connectionEnvironmentVariableName))
        {
            return null;
        }

        var connectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(connectionEnvironmentVariableName);
        return new MetaPipeline.MetaPipelineOperationalDbStore(connectionString);
    }

    private static bool IsOperationalDbStartupFailure(
        MetaPipeline.MetaPipelineOperationalDbStore? operationalDb,
        Guid? operationalRunId) =>
        operationalDb is not null && operationalRunId is null;

    private static async Task<IReadOnlyList<string>> RecordOperationalFailureAsync(
        MetaPipeline.MetaPipelineOperationalDbStore? operationalDb,
        Guid? operationalRunId,
        string failureStage,
        string failureKind,
        string message,
        Exception exception)
    {
        if (operationalDb is null || operationalRunId is not Guid runId)
        {
            return Array.Empty<string>();
        }

        try
        {
            await operationalDb.FailRunAsync(runId, failureStage, failureKind, message, exception)
                .ConfigureAwait(false);
            return new[] { $"  PipelineRunId: {runId}" };
        }
        catch (Exception loggingException)
        {
            return new[]
            {
                $"  PipelineRunId: {runId}",
                $"  PipelineDbFailure: {loggingException.Message}",
            };
        }
    }
}
