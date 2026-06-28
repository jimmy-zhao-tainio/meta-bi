using Meta.Core.Connections;
using MetaOrchestration.WorkerProtocol;

namespace MetaPipeline;

public interface IMetaPipelineExecutionProgress : IProgress<BufferedPipelineExecutionProgress>, IDisposable
{
    void StartStep(int stepIndex, string stepName);

    void CompleteStep(bool succeeded, long completedRowCount = 0, int completedBatchCount = 0);

    void Complete(bool failed);
}

public sealed record MetaPipelineExecutionPipelineCommandRequest(
    MetaPipelineModel PipelineModel,
    string PipelineWorkspacePath,
    string PipelineName,
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    string DataTypeConversionWorkspacePath,
    string PipelineDbConnectionEnvironmentVariableName);

public sealed record MetaPipelineExecutionStepCommandRequest(
    MetaPipelineModel PipelineModel,
    string PipelineWorkspacePath,
    string PipelineName,
    string StepName,
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    string DataTypeConversionWorkspacePath,
    string PipelineDbConnectionEnvironmentVariableName);

public sealed record MetaPipelineWorkerExecutionCommandRequest(
    MetaPipelineModel PipelineModel,
    string PipelineWorkspacePath,
    string PipelineName,
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    string DataTypeConversionWorkspacePath,
    string PipelineDbConnectionEnvironmentVariableName,
    string WorkerControlPipeName,
    int? ControlPipeConnectTimeoutSeconds);

public sealed record MetaPipelineDirectSqlServerExecutionCommandRequest(
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    string ExecutionConnectionEnvironmentVariableName,
    string TargetConnectionEnvironmentVariableName,
    string Script,
    string Binding,
    string? TargetSqlIdentifier,
    int BatchSize,
    int? TimeoutSeconds,
    string TargetDataTypeSystemName,
    string DataTypeConversionWorkspacePath,
    string PipelineDbConnectionEnvironmentVariableName);

public enum MetaPipelineExecutionCommandStatus
{
    Succeeded,
    ExecutionFailed,
    ValidationFailed,
    ConnectionFailure,
    ConfigurationFailure,
    OperationalDbUnavailable,
    UnexpectedFailure,
}

public sealed record MetaPipelineExecutionCommandOutcome(
    MetaPipelineExecutionCommandStatus Status,
    MetaPipelineExecutionResult? Result,
    MetaPipelineModeledExecutionPlan? Plan,
    MetaPipelineTransformSelection? TransformSelection,
    Guid? OperationalRunId,
    IReadOnlyList<string> Details,
    Exception? Exception,
    bool ProgressRendered)
{
    public bool Succeeded => Status == MetaPipelineExecutionCommandStatus.Succeeded;
}

public sealed class MetaPipelineExecutionCommandService
{
    public async Task<MetaPipelineExecutionCommandOutcome> ExecutePipelineAsync(
        MetaPipelineExecutionPipelineCommandRequest request,
        Func<int, IMetaPipelineExecutionProgress?>? createProgress = null)
    {
        MetaPipelineModeledExecutionPlan? plan = null;
        MetaPipelineOperationalDbStore? operationalDb = null;
        Guid? operationalRunId = null;
        IMetaPipelineExecutionProgress? progress = null;
        try
        {
            var pipelineWorkspacePath = Path.GetFullPath(request.PipelineWorkspacePath);
            operationalDb = CreateOperationalDbStore(request.PipelineDbConnectionEnvironmentVariableName);
            if (operationalDb is not null)
            {
                operationalRunId = await operationalDb.StartRunAsync(
                    new MetaPipelineOperationalRunStart(
                        PipelineWorkspacePath: pipelineWorkspacePath,
                        PipelineName: request.PipelineName,
                        TransformWorkspacePath: FullPathOrEmpty(request.TransformWorkspacePath),
                        BindingWorkspacePath: FullPathOrEmpty(request.BindingWorkspacePath)))
                    .ConfigureAwait(false);
            }

            var validation = new MetaPipelineModelValidationService()
                .ValidatePipeline(request.PipelineModel, request.PipelineName);
            if (!validation.IsValid)
            {
                var details = validation.Errors.Select(static item => "  " + item).ToList();
                details.AddRange(await RecordOperationalFailureAsync(
                    operationalDb,
                    operationalRunId,
                    "Validation",
                    "Configuration",
                    string.Join(Environment.NewLine, validation.Errors),
                    new MetaPipelineConfigurationException("Cannot validate pipeline model."))
                    .ConfigureAwait(false));

                return CreateOutcome(
                    MetaPipelineExecutionCommandStatus.ValidationFailed,
                    null,
                    null,
                    null,
                    operationalRunId,
                    details,
                    null,
                    progress);
            }

            plan = new MetaPipelineModeledExecutionResolver().Resolve(
                request.PipelineModel,
                pipelineWorkspacePath,
                request.PipelineName);

            if (operationalDb is not null && operationalRunId is Guid startedRunId)
            {
                await operationalDb.UpdateRunContextAsync(
                    startedRunId,
                    new MetaPipelineOperationalRunStart(
                        PipelineWorkspacePath: Path.GetFullPath(plan.PipelineWorkspacePath),
                        PipelineId: plan.PipelineId,
                        PipelineName: plan.PipelineName,
                        TransformTaskId: plan.TransformTaskId,
                        TransformTaskName: plan.TransformTaskName,
                        TargetWriteTaskId: plan.TargetWriteTaskId,
                        TargetWriteTaskName: plan.TargetWriteTaskName,
                        TransformWorkspacePath: FullPathOrEmpty(plan.TransformWorkspacePath),
                        BindingWorkspacePath: FullPathOrEmpty(plan.BindingWorkspacePath),
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

            progress = createProgress?.Invoke(plan.Steps.Count);
            var result = await ExecuteModeledPlanAsync(
                plan,
                operationalDb,
                operationalRunId,
                request.DataTypeConversionWorkspacePath,
                progress).ConfigureAwait(false);

            if (operationalDb is not null && operationalRunId is Guid completedRunId)
            {
                await operationalDb.CompleteRunAsync(
                    completedRunId,
                    result,
                    BuildModeledOperationalFingerprints(plan, request.DataTypeConversionWorkspacePath))
                    .ConfigureAwait(false);
            }

            if (!result.Succeeded)
            {
                CompleteProgress(progress, failed: true);
                return CreateOutcome(
                    MetaPipelineExecutionCommandStatus.ExecutionFailed,
                    result,
                    plan,
                    null,
                    operationalRunId,
                    Array.Empty<string>(),
                    null,
                    progress);
            }

            CompleteProgress(progress, failed: false);
            return CreateOutcome(
                MetaPipelineExecutionCommandStatus.Succeeded,
                result,
                plan,
                null,
                operationalRunId,
                Array.Empty<string>(),
                null,
                progress);
        }
        catch (ConnectionEnvironmentVariableException ex)
        {
            CompleteProgress(progress, failed: true);
            var details = new List<string> { $"  {ex.Message}" };
            details.AddRange(await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Connection",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false));
            return CreateOutcome(MetaPipelineExecutionCommandStatus.ConnectionFailure, null, plan, null, operationalRunId, details, ex, progress);
        }
        catch (MetaPipelineConfigurationException ex)
        {
            CompleteProgress(progress, failed: true);
            var dbDetails = await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Configuration",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false);
            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(request.PipelineWorkspacePath)}",
                $"  Pipeline: {request.PipelineName}",
            };
            details.AddRange(dbDetails);
            details.Add($"  {ex.Message}");

            return CreateOutcome(MetaPipelineExecutionCommandStatus.ConfigurationFailure, null, plan, null, operationalRunId, details, ex, progress);
        }
        catch (Exception ex)
        {
            CompleteProgress(progress, failed: true);
            if (IsOperationalDbStartupFailure(operationalDb, operationalRunId))
            {
                return CreateOutcome(
                    MetaPipelineExecutionCommandStatus.OperationalDbUnavailable,
                    null,
                    plan,
                    null,
                    operationalRunId,
                    new[]
                    {
                        $"  PipelineDbConnectionEnv: {request.PipelineDbConnectionEnvironmentVariableName}",
                        $"  {ex.Message}",
                    },
                    ex,
                    progress);
            }

            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(request.PipelineWorkspacePath)}",
                $"  Pipeline: {request.PipelineName}",
                $"  {ex.Message}",
            };
            if (plan is not null)
            {
                if (!string.IsNullOrWhiteSpace(plan.ExecutionConnectionEnvironmentVariableName))
                {
                    details.Insert(3, $"  ExecutionConnectionEnv: {plan.ExecutionConnectionEnvironmentVariableName}");
                }

                if (!string.IsNullOrWhiteSpace(plan.TargetConnectionEnvironmentVariableName))
                {
                    details.Insert(4, $"  TargetConnectionEnv: {plan.TargetConnectionEnvironmentVariableName}");
                }
            }

            details.AddRange(await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Unexpected",
                "Exception",
                ex.Message,
                ex).ConfigureAwait(false));

            return CreateOutcome(MetaPipelineExecutionCommandStatus.UnexpectedFailure, null, plan, null, operationalRunId, details, ex, progress);
        }
    }

    public async Task<MetaPipelineExecutionCommandOutcome> ExecuteStepAsync(
        MetaPipelineExecutionStepCommandRequest request,
        Func<int, IMetaPipelineExecutionProgress?>? createProgress = null)
    {
        MetaPipelineModeledExecutionPlan? plan = null;
        MetaPipelineOperationalDbStore? operationalDb = null;
        Guid? operationalRunId = null;
        IMetaPipelineExecutionProgress? progress = null;
        try
        {
            var pipelineWorkspacePath = Path.GetFullPath(request.PipelineWorkspacePath);
            operationalDb = CreateOperationalDbStore(request.PipelineDbConnectionEnvironmentVariableName);
            if (operationalDb is not null)
            {
                operationalRunId = await operationalDb.StartRunAsync(
                    new MetaPipelineOperationalRunStart(
                        PipelineWorkspacePath: pipelineWorkspacePath,
                        PipelineName: request.PipelineName,
                        TransformWorkspacePath: FullPathOrEmpty(request.TransformWorkspacePath),
                        BindingWorkspacePath: FullPathOrEmpty(request.BindingWorkspacePath)))
                    .ConfigureAwait(false);
            }

            plan = new MetaPipelineModeledExecutionResolver().ResolveStep(
                request.PipelineModel,
                pipelineWorkspacePath,
                request.PipelineName,
                request.StepName);

            if (operationalDb is not null && operationalRunId is Guid startedRunId)
            {
                await operationalDb.UpdateRunContextAsync(
                    startedRunId,
                    new MetaPipelineOperationalRunStart(
                        PipelineWorkspacePath: Path.GetFullPath(plan.PipelineWorkspacePath),
                        PipelineId: plan.PipelineId,
                        PipelineName: plan.PipelineName,
                        TransformTaskId: plan.TransformTaskId,
                        TransformTaskName: plan.TransformTaskName,
                        TargetWriteTaskId: plan.TargetWriteTaskId,
                        TargetWriteTaskName: plan.TargetWriteTaskName,
                        TransformWorkspacePath: FullPathOrEmpty(plan.TransformWorkspacePath),
                        BindingWorkspacePath: FullPathOrEmpty(plan.BindingWorkspacePath),
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

            progress = createProgress?.Invoke(1);
            var result = await ExecuteModeledPlanAsync(
                plan,
                operationalDb,
                operationalRunId,
                request.DataTypeConversionWorkspacePath,
                progress).ConfigureAwait(false);

            if (operationalDb is not null && operationalRunId is Guid completedRunId)
            {
                await operationalDb.CompleteRunAsync(
                    completedRunId,
                    result,
                    BuildModeledOperationalFingerprints(plan, request.DataTypeConversionWorkspacePath))
                    .ConfigureAwait(false);
            }

            if (!result.Succeeded)
            {
                CompleteProgress(progress, failed: true);
                return CreateOutcome(MetaPipelineExecutionCommandStatus.ExecutionFailed, result, plan, null, operationalRunId, Array.Empty<string>(), null, progress);
            }

            CompleteProgress(progress, failed: false);
            return CreateOutcome(MetaPipelineExecutionCommandStatus.Succeeded, result, plan, null, operationalRunId, Array.Empty<string>(), null, progress);
        }
        catch (ConnectionEnvironmentVariableException ex)
        {
            CompleteProgress(progress, failed: true);
            var details = new List<string> { $"  {ex.Message}" };
            details.AddRange(await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Connection",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false));
            return CreateOutcome(MetaPipelineExecutionCommandStatus.ConnectionFailure, null, plan, null, operationalRunId, details, ex, progress);
        }
        catch (MetaPipelineConfigurationException ex)
        {
            CompleteProgress(progress, failed: true);
            var dbDetails = await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Configuration",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false);
            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(request.PipelineWorkspacePath)}",
                $"  Pipeline: {request.PipelineName}",
                $"  Step: {request.StepName}",
            };
            details.AddRange(dbDetails);
            details.Add($"  {ex.Message}");

            return CreateOutcome(MetaPipelineExecutionCommandStatus.ConfigurationFailure, null, plan, null, operationalRunId, details, ex, progress);
        }
        catch (Exception ex)
        {
            CompleteProgress(progress, failed: true);
            if (IsOperationalDbStartupFailure(operationalDb, operationalRunId))
            {
                return CreateOutcome(
                    MetaPipelineExecutionCommandStatus.OperationalDbUnavailable,
                    null,
                    plan,
                    null,
                    operationalRunId,
                    new[]
                    {
                        $"  PipelineDbConnectionEnv: {request.PipelineDbConnectionEnvironmentVariableName}",
                        $"  {ex.Message}",
                    },
                    ex,
                    progress);
            }

            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(request.PipelineWorkspacePath)}",
                $"  Pipeline: {request.PipelineName}",
                $"  Step: {request.StepName}",
                $"  {ex.Message}",
            };
            if (plan is not null)
            {
                if (!string.IsNullOrWhiteSpace(plan.ExecutionConnectionEnvironmentVariableName))
                {
                    details.Insert(4, $"  ExecutionConnectionEnv: {plan.ExecutionConnectionEnvironmentVariableName}");
                }

                if (!string.IsNullOrWhiteSpace(plan.TargetConnectionEnvironmentVariableName))
                {
                    details.Insert(5, $"  TargetConnectionEnv: {plan.TargetConnectionEnvironmentVariableName}");
                }
            }

            details.AddRange(await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Unexpected",
                "Exception",
                ex.Message,
                ex).ConfigureAwait(false));

            return CreateOutcome(MetaPipelineExecutionCommandStatus.UnexpectedFailure, null, plan, null, operationalRunId, details, ex, progress);
        }
    }

    public async Task<MetaPipelineExecutionCommandOutcome> ExecuteWorkerAsync(
        MetaPipelineWorkerExecutionCommandRequest request)
    {
        MetaPipelineModeledExecutionPlan? plan = null;
        MetaPipelineOperationalDbStore? operationalDb = null;
        Guid? operationalRunId = null;
        try
        {
            await using var workerChannel = await OrchestrationWorkerProtocolChannel.ConnectClientAsync(
                request.WorkerControlPipeName,
                request.ControlPipeConnectTimeoutSeconds is null
                    ? null
                    : TimeSpan.FromSeconds(request.ControlPipeConnectTimeoutSeconds.Value)).ConfigureAwait(false);
            var executableVersion = OrchestrationWorkerProtocol.ResolveExecutableVersion();
            await WriteWorkerLifecycleEventAsync(
                workerChannel,
                WorkerEventKinds.WorkerOnline,
                executableVersion,
                "online").ConfigureAwait(false);
            await WriteWorkerLifecycleEventAsync(
                workerChannel,
                WorkerEventKinds.WorkerReady,
                executableVersion,
                "ready").ConfigureAwait(false);
            var startCommand = await ReadStartPipelineCommandAsync(
                workerChannel,
                request.PipelineName).ConfigureAwait(false);
            var pipelineWorkspacePath = Path.GetFullPath(request.PipelineWorkspacePath);
            operationalDb = CreateOperationalDbStore(request.PipelineDbConnectionEnvironmentVariableName);
            if (operationalDb is not null)
            {
                operationalRunId = await operationalDb.StartRunAsync(
                    new MetaPipelineOperationalRunStart(
                        PipelineWorkspacePath: pipelineWorkspacePath,
                        PipelineName: request.PipelineName,
                        TransformWorkspacePath: FullPathOrEmpty(request.TransformWorkspacePath),
                        BindingWorkspacePath: FullPathOrEmpty(request.BindingWorkspacePath)))
                    .ConfigureAwait(false);
            }

            var validation = new MetaPipelineModelValidationService()
                .ValidatePipeline(request.PipelineModel, request.PipelineName);
            if (!validation.IsValid)
            {
                var details = validation.Errors.Select(static item => "  " + item).ToList();
                details.AddRange(await RecordOperationalFailureAsync(
                    operationalDb,
                    operationalRunId,
                    "Validation",
                    "Configuration",
                    string.Join(Environment.NewLine, validation.Errors),
                    new MetaPipelineConfigurationException("Cannot validate pipeline model."))
                    .ConfigureAwait(false));

                return CreateOutcome(MetaPipelineExecutionCommandStatus.ValidationFailed, null, null, null, operationalRunId, details, null, null);
            }

            plan = new MetaPipelineModeledExecutionResolver().Resolve(
                request.PipelineModel,
                pipelineWorkspacePath,
                request.PipelineName);
            ValidateStartPipelineCommand(startCommand, plan);

            if (operationalDb is not null && operationalRunId is Guid startedRunId)
            {
                await operationalDb.UpdateRunContextAsync(
                    startedRunId,
                    new MetaPipelineOperationalRunStart(
                        PipelineWorkspacePath: Path.GetFullPath(plan.PipelineWorkspacePath),
                        PipelineId: plan.PipelineId,
                        PipelineName: plan.PipelineName,
                        TransformWorkspacePath: FullPathOrEmpty(plan.TransformWorkspacePath),
                        BindingWorkspacePath: FullPathOrEmpty(plan.BindingWorkspacePath)))
                    .ConfigureAwait(false);
            }

            var result = await ExecuteModeledPlanAsWorkerAsync(
                plan,
                operationalDb,
                operationalRunId,
                request.DataTypeConversionWorkspacePath,
                workerChannel,
                startCommand.TaskId,
                executableVersion)
                .ConfigureAwait(false);

            if (operationalDb is not null && operationalRunId is Guid completedRunId)
            {
                await operationalDb.CompleteRunAsync(
                    completedRunId,
                    result,
                    BuildModeledOperationalFingerprints(plan, request.DataTypeConversionWorkspacePath))
                    .ConfigureAwait(false);
            }

            return CreateOutcome(
                result.Succeeded ? MetaPipelineExecutionCommandStatus.Succeeded : MetaPipelineExecutionCommandStatus.ExecutionFailed,
                result,
                plan,
                null,
                operationalRunId,
                Array.Empty<string>(),
                null,
                null);
        }
        catch (ConnectionEnvironmentVariableException ex)
        {
            var details = new List<string> { $"  {ex.Message}" };
            details.AddRange(await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Connection",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false));
            return CreateOutcome(MetaPipelineExecutionCommandStatus.ConnectionFailure, null, plan, null, operationalRunId, details, ex, null);
        }
        catch (MetaPipelineConfigurationException ex)
        {
            var dbDetails = await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Configuration",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false);
            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(request.PipelineWorkspacePath)}",
                $"  Pipeline: {request.PipelineName}",
            };
            details.AddRange(dbDetails);
            details.Add($"  {ex.Message}");

            return CreateOutcome(MetaPipelineExecutionCommandStatus.ConfigurationFailure, null, plan, null, operationalRunId, details, ex, null);
        }
        catch (Exception ex)
        {
            if (IsOperationalDbStartupFailure(operationalDb, operationalRunId))
            {
                return CreateOutcome(
                    MetaPipelineExecutionCommandStatus.OperationalDbUnavailable,
                    null,
                    plan,
                    null,
                    operationalRunId,
                    new[]
                    {
                        $"  PipelineDbConnectionEnv: {request.PipelineDbConnectionEnvironmentVariableName}",
                        $"  {ex.Message}",
                    },
                    ex,
                    null);
            }

            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(request.PipelineWorkspacePath)}",
                $"  Pipeline: {request.PipelineName}",
                $"  {ex.Message}",
            };
            details.AddRange(await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Unexpected",
                "Exception",
                ex.Message,
                ex).ConfigureAwait(false));

            return CreateOutcome(MetaPipelineExecutionCommandStatus.UnexpectedFailure, null, plan, null, operationalRunId, details, ex, null);
        }
    }

    public async Task<MetaPipelineExecutionCommandOutcome> ExecuteSqlServerAsync(
        MetaPipelineDirectSqlServerExecutionCommandRequest request,
        Func<int, IMetaPipelineExecutionProgress?>? createProgress = null)
    {
        MetaPipelineOperationalDbStore? operationalDb = null;
        Guid? operationalRunId = null;
        MetaPipelineTransformSelection? selection = null;
        IMetaPipelineExecutionProgress? progress = null;
        try
        {
            operationalDb = CreateOperationalDbStore(request.PipelineDbConnectionEnvironmentVariableName);
            if (operationalDb is not null)
            {
                operationalRunId = await operationalDb.StartRunAsync(
                    new MetaPipelineOperationalRunStart(
                        TransformWorkspacePath: FullPathOrEmpty(request.TransformWorkspacePath),
                        BindingWorkspacePath: FullPathOrEmpty(request.BindingWorkspacePath),
                        ExecutionConnectionEnvironmentVariableName: request.ExecutionConnectionEnvironmentVariableName,
                        TargetConnectionEnvironmentVariableName: request.TargetConnectionEnvironmentVariableName,
                        TargetSqlIdentifier: request.TargetSqlIdentifier,
                        BatchSize: request.BatchSize))
                    .ConfigureAwait(false);
            }

            selection = new MetaPipelineTransformSelectionResolver().Resolve(
                request.TransformWorkspacePath,
                request.BindingWorkspacePath,
                request.Script,
                request.Binding);
            if (operationalDb is not null && operationalRunId is Guid startedRunId)
            {
                await operationalDb.UpdateRunContextAsync(
                    startedRunId,
                    new MetaPipelineOperationalRunStart(
                        TransformScriptId: selection.TransformScriptId,
                        TransformBindingId: selection.TransformBindingId,
                        TransformScriptName: selection.TransformScriptName))
                    .ConfigureAwait(false);
            }

            var executionConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(request.ExecutionConnectionEnvironmentVariableName);
            var targetConnectionString = string.IsNullOrWhiteSpace(request.TargetConnectionEnvironmentVariableName)
                ? null
                : ConnectionEnvironmentVariableResolver.ResolveRequired(request.TargetConnectionEnvironmentVariableName);
            var executionContext = await CreateExecutionContextAsync(
                operationalDb,
                operationalRunId,
                null,
                selection.TransformScriptName,
                "TransformExecution").ConfigureAwait(false);

            progress = createProgress?.Invoke(1);
            MetaPipelineExecutionResult result;
            try
            {
                progress?.StartStep(1, selection.TransformScriptName);
                result = await new MetaPipelineSqlServerExecutionService().ExecuteAsync(
                    new MetaPipelineSqlServerExecutionRequest(
                        request.TransformWorkspacePath,
                        request.BindingWorkspacePath,
                        executionConnectionString,
                        targetConnectionString,
                        selection.TransformScriptId,
                        selection.TransformBindingId,
                        request.TargetSqlIdentifier,
                        request.BatchSize,
                        request.TimeoutSeconds,
                        ExecutionContext: executionContext,
                        TargetDataTypeSystemName: request.TargetDataTypeSystemName,
                        DataTypeConversionWorkspacePath: request.DataTypeConversionWorkspacePath),
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
                    BuildDirectOperationalFingerprints(request, selection, result))
                    .ConfigureAwait(false);
            }

            if (!result.Succeeded)
            {
                CompleteProgress(progress, failed: true);
                return CreateOutcome(MetaPipelineExecutionCommandStatus.ExecutionFailed, result, null, selection, operationalRunId, Array.Empty<string>(), null, progress);
            }

            CompleteProgress(progress, failed: false);
            return CreateOutcome(MetaPipelineExecutionCommandStatus.Succeeded, result, null, selection, operationalRunId, Array.Empty<string>(), null, progress);
        }
        catch (ConnectionEnvironmentVariableException ex)
        {
            CompleteProgress(progress, failed: true);
            var details = new List<string> { $"  {ex.Message}" };
            details.AddRange(await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Connection",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false));
            return CreateOutcome(MetaPipelineExecutionCommandStatus.ConnectionFailure, null, null, selection, operationalRunId, details, ex, progress);
        }
        catch (MetaPipelineConfigurationException ex)
        {
            CompleteProgress(progress, failed: true);
            var dbDetails = await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Configuration",
                "Configuration",
                ex.Message,
                ex).ConfigureAwait(false);
            var details = new List<string>
            {
                $"  TransformWorkspace: {FullPathOrEmpty(request.TransformWorkspacePath)}",
            };
            if (!string.IsNullOrWhiteSpace(request.BindingWorkspacePath))
            {
                details.Add($"  BindingWorkspace: {FullPathOrEmpty(request.BindingWorkspacePath)}");
            }

            details.AddRange(dbDetails);
            details.Add($"  {ex.Message}");
            return CreateOutcome(MetaPipelineExecutionCommandStatus.ConfigurationFailure, null, null, selection, operationalRunId, details, ex, progress);
        }
        catch (Exception ex)
        {
            CompleteProgress(progress, failed: true);
            if (IsOperationalDbStartupFailure(operationalDb, operationalRunId))
            {
                return CreateOutcome(
                    MetaPipelineExecutionCommandStatus.OperationalDbUnavailable,
                    null,
                    null,
                    selection,
                    operationalRunId,
                    new[]
                    {
                        $"  PipelineDbConnectionEnv: {request.PipelineDbConnectionEnvironmentVariableName}",
                        $"  {ex.Message}",
                    },
                    ex,
                    progress);
            }

            var details = new List<string>
            {
                $"  TransformWorkspace: {FullPathOrEmpty(request.TransformWorkspacePath)}",
                $"  ExecutionConnectionEnv: {request.ExecutionConnectionEnvironmentVariableName}",
            };
            if (!string.IsNullOrWhiteSpace(request.BindingWorkspacePath))
            {
                details.Add($"  BindingWorkspace: {FullPathOrEmpty(request.BindingWorkspacePath)}");
            }

            if (!string.IsNullOrWhiteSpace(request.TargetConnectionEnvironmentVariableName))
            {
                details.Add($"  TargetConnectionEnv: {request.TargetConnectionEnvironmentVariableName}");
            }

            details.Add($"  {ex.Message}");
            details.AddRange(await RecordOperationalFailureAsync(
                operationalDb,
                operationalRunId,
                "Unexpected",
                "Exception",
                ex.Message,
                ex).ConfigureAwait(false));

            return CreateOutcome(MetaPipelineExecutionCommandStatus.UnexpectedFailure, null, null, selection, operationalRunId, details, ex, progress);
        }
    }

    private static async Task<MetaPipelineExecutionResult> ExecuteModeledPlanAsync(
        MetaPipelineModeledExecutionPlan plan,
        MetaPipelineOperationalDbStore? operationalDb,
        Guid? operationalRunId,
        string? dataTypeConversionWorkspacePath,
        IMetaPipelineExecutionProgress? progress)
    {
        var startedAtUtc = DateTimeOffset.UtcNow;
        var taskResults = new List<MetaPipelineExecutionTaskResult>();
        long rowCount = 0;
        var batchCount = 0;
        var columnCount = 0;

        for (var index = 0; index < plan.Steps.Count; index++)
        {
            var step = plan.Steps[index];
            progress?.StartStep(index + 1, step.TaskName);
            MetaPipelineExecutionResult result;
            try
            {
                result = await ExecuteModeledStepAsync(
                    plan,
                    step,
                    operationalDb,
                    operationalRunId,
                    dataTypeConversionWorkspacePath,
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
                    MetaPipelineExecutionStatus.Failed,
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
            MetaPipelineExecutionStatus.Succeeded,
            startedAtUtc,
            rowCount,
            batchCount,
            columnCount,
            PipelineExecutionFailureStage.None,
            string.Empty,
            string.Empty,
            taskResults);
    }

    private static async Task<MetaPipelineExecutionResult> ExecuteModeledPlanAsWorkerAsync(
        MetaPipelineModeledExecutionPlan plan,
        MetaPipelineOperationalDbStore? operationalDb,
        Guid? operationalRunId,
        string? dataTypeConversionWorkspacePath,
        OrchestrationWorkerProtocolChannel workerChannel,
        string resumeTaskId,
        string executableVersion)
    {
        var startedAtUtc = DateTimeOffset.UtcNow;
        var taskResults = new List<MetaPipelineExecutionTaskResult>();
        long rowCount = 0;
        var batchCount = 0;
        var columnCount = 0;
        var anyFailure = false;
        var firstFailureStage = PipelineExecutionFailureStage.None;
        var firstFailureMessage = string.Empty;
        var firstFailureTaskName = string.Empty;
        var pendingFailureStage = PipelineExecutionFailureStage.None;
        var pendingFailureMessage = string.Empty;
        var pendingFailureTaskName = string.Empty;
        await WritePipelineWorkerEventAsync(
            workerChannel,
            WorkerEventKinds.PipelineStarted,
            plan,
            null,
            string.Empty,
            string.Empty,
            0,
            0,
            executableVersion,
            "pipeline started").ConfigureAwait(false);

        var closePipelinePath = false;
        var startIndex = ResolveWorkerStartIndex(plan, resumeTaskId);
        for (var index = startIndex; index < plan.Steps.Count && !closePipelinePath; index++)
        {
            var step = plan.Steps[index];
            var command = await ReadWorkerCommandAtBoundaryAsync(
                workerChannel,
                plan,
                step,
                executableVersion,
                emitTaskReady: true).ConfigureAwait(false);
            while (true)
            {
                if (string.Equals(command.Kind, WorkerCommandKinds.StopPipeline, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(command.Kind, WorkerCommandKinds.FailPipeline, StringComparison.OrdinalIgnoreCase))
                {
                    if (pendingFailureStage != PipelineExecutionFailureStage.None)
                    {
                        anyFailure = true;
                        if (firstFailureStage == PipelineExecutionFailureStage.None)
                        {
                            firstFailureStage = pendingFailureStage;
                            firstFailureMessage = pendingFailureMessage;
                            firstFailureTaskName = pendingFailureTaskName;
                        }
                    }

                    closePipelinePath = true;
                    break;
                }

                if (!string.Equals(command.Kind, WorkerCommandKinds.GrantTask, StringComparison.OrdinalIgnoreCase))
                {
                    throw new MetaPipelineConfigurationException(
                        $"Worker command '{command.Kind}' is not supported. Expected GrantTask, StopPipeline, or FailPipeline.");
                }

                await WritePipelineWorkerEventAsync(
                    workerChannel,
                    WorkerEventKinds.GrantAccepted,
                    plan,
                    step,
                    command.GrantId,
                    command.CommandId,
                    command.AttemptNumber,
                    0,
                    executableVersion,
                    string.Empty).ConfigureAwait(false);
                await WritePipelineWorkerEventAsync(
                    workerChannel,
                    WorkerEventKinds.TaskStarted,
                    plan,
                    step,
                    command.GrantId,
                    command.CommandId,
                    command.AttemptNumber,
                    0,
                    executableVersion,
                    string.Empty).ConfigureAwait(false);
                MetaPipelineExecutionResult result;
                try
                {
                    result = await ExecuteModeledStepAsync(
                        plan,
                        step,
                        operationalDb,
                        operationalRunId,
                        dataTypeConversionWorkspacePath,
                        progress: null).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    pendingFailureStage = PipelineExecutionFailureStage.TransformExecution;
                    pendingFailureMessage = ex.Message;
                    pendingFailureTaskName = step.TaskName;
                    await WritePipelineWorkerEventAsync(
                        workerChannel,
                        WorkerEventKinds.TaskFailed,
                        plan,
                        step,
                        command.GrantId,
                        command.CommandId,
                        command.AttemptNumber,
                        4,
                        executableVersion,
                        ex.Message,
                        WorkerFailureClasses.WorkerReportedRetryable).ConfigureAwait(false);
                    command = await ReadWorkerCommandAtBoundaryAsync(
                        workerChannel,
                        plan,
                        step,
                        executableVersion,
                        emitTaskReady: false).ConfigureAwait(false);
                    continue;
                }

                taskResults.AddRange(result.TaskResults);
                rowCount += result.RowCount;
                batchCount += result.BatchCount;
                columnCount += result.ColumnCount;

                if (result.Succeeded)
                {
                    pendingFailureStage = PipelineExecutionFailureStage.None;
                    pendingFailureMessage = string.Empty;
                    pendingFailureTaskName = string.Empty;
                    await WritePipelineWorkerEventAsync(
                        workerChannel,
                        WorkerEventKinds.TaskSucceeded,
                        plan,
                        step,
                        command.GrantId,
                        command.CommandId,
                        command.AttemptNumber,
                        0,
                        executableVersion,
                        string.Empty).ConfigureAwait(false);
                    break;
                }

                pendingFailureStage = result.FailureStage;
                pendingFailureMessage = result.FailureMessage;
                pendingFailureTaskName = result.FailureTaskName;

                await WritePipelineWorkerEventAsync(
                    workerChannel,
                    WorkerEventKinds.TaskFailed,
                    plan,
                    step,
                    command.GrantId,
                    command.CommandId,
                    command.AttemptNumber,
                    ResolveFailureExitCode(result),
                    executableVersion,
                    result.FailureMessage,
                    ClassifyPipelineFailureClass(result.FailureStage)).ConfigureAwait(false);
                command = await ReadWorkerCommandAtBoundaryAsync(
                    workerChannel,
                    plan,
                    step,
                    executableVersion,
                    emitTaskReady: false).ConfigureAwait(false);
            }
        }

        return CreateModeledPlanResult(
            plan,
            anyFailure ? MetaPipelineExecutionStatus.Failed : MetaPipelineExecutionStatus.Succeeded,
            startedAtUtc,
            rowCount,
            batchCount,
            columnCount,
            firstFailureStage,
            firstFailureMessage,
            firstFailureTaskName,
            taskResults);
    }

    private static int ResolveWorkerStartIndex(
        MetaPipelineModeledExecutionPlan plan,
        string resumeTaskId)
    {
        if (string.IsNullOrWhiteSpace(resumeTaskId))
        {
            return 0;
        }

        for (var index = 0; index < plan.Steps.Count; index++)
        {
            if (string.Equals(plan.Steps[index].TaskId, resumeTaskId, StringComparison.Ordinal))
            {
                return index;
            }
        }

        throw new MetaPipelineConfigurationException(
            $"Worker command '{WorkerCommandKinds.StartPipeline}' requested resume task id '{resumeTaskId}', but pipeline '{plan.PipelineName}' has no matching task boundary.");
    }

    private static async Task<MetaPipelineExecutionResult> ExecuteModeledStepAsync(
        MetaPipelineModeledExecutionPlan plan,
        MetaPipelineModeledExecutionStep step,
        MetaPipelineOperationalDbStore? operationalDb,
        Guid? operationalRunId,
        string? dataTypeConversionWorkspacePath,
        IProgress<BufferedPipelineExecutionProgress>? progress)
    {
        if (step.StepKind == MetaPipelineModeledExecutionStepKind.Executable)
        {
            var executableContext = await CreateExecutionContextAsync(
                operationalDb,
                operationalRunId,
                plan.PipelineName,
                step.TaskName,
                "Executable").ConfigureAwait(false);

            return await new MetaPipelineExecutableExecutionService().ExecuteAsync(
                new MetaPipelineExecutableExecutionRequest(
                    step.TaskName,
                    step.ExecutablePath
                    ?? throw new MetaPipelineConfigurationException($"Executable task '{step.TaskName}' must name an executable path."),
                    step.Arguments,
                    step.WorkingDirectory,
                    step.SuccessExitCode ?? 0,
                    step.TimeoutSeconds,
                    executableContext)).ConfigureAwait(false);
        }

        var executionConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
            step.ExecutionConnectionEnvironmentVariableName
            ?? throw new MetaPipelineConfigurationException($"Transform task '{step.TaskName}' must name an execution connection environment variable."));
        var targetConnectionString = step.IsSelect
            ? ConnectionEnvironmentVariableResolver.ResolveRequired(
                step.TargetConnectionEnvironmentVariableName
                ?? throw new MetaPipelineConfigurationException("SELECT-kind pipeline execution requires a target connection reference."))
            : null;
        var executionContext = await CreateExecutionContextAsync(
            operationalDb,
            operationalRunId,
            plan.PipelineName,
            step.TaskName,
            "TransformExecution").ConfigureAwait(false);

        return await new MetaPipelineSqlServerExecutionService().ExecuteAsync(
            new MetaPipelineSqlServerExecutionRequest(
                step.TransformWorkspacePath
                ?? throw new MetaPipelineConfigurationException($"Transform task '{step.TaskName}' must name TransformWorkspacePath."),
                step.BindingWorkspacePath
                ?? throw new MetaPipelineConfigurationException($"Transform task '{step.TaskName}' must name BindingWorkspacePath."),
                executionConnectionString,
                targetConnectionString,
                step.TransformScriptId
                ?? throw new MetaPipelineConfigurationException($"Transform task '{step.TaskName}' must name a transform script id."),
                step.TransformBindingId
                ?? throw new MetaPipelineConfigurationException($"Transform task '{step.TaskName}' must name a transform binding id."),
                step.TargetSqlIdentifier,
                step.BatchSize,
                step.TimeoutSeconds,
                step.TargetWriteModelName ?? "None",
                step.TaskName,
                step.TargetWriteTaskName,
                executionContext,
                step.TargetDataTypeSystemName ?? "SqlServer",
                dataTypeConversionWorkspacePath),
            progress).ConfigureAwait(false);
    }

    private static async Task<WorkerProtocolCommand> ReadWorkerCommandAsync(
        OrchestrationWorkerProtocolChannel workerChannel)
    {
        while (await workerChannel.ReadLineAsync().ConfigureAwait(false) is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            try
            {
                if (!OrchestrationWorkerProtocol.TryDecodeCommand(line, out var command))
                {
                    throw new MetaPipelineConfigurationException(
                        $"Unsupported worker command line '{line}'.");
                }

                return command;
            }
            catch (InvalidOperationException ex)
            {
                throw new MetaPipelineConfigurationException(ex.Message, ex);
            }
        }

        throw new MetaPipelineConfigurationException(
            "The orchestration worker control channel closed before the pipeline worker received a command.");
    }

    private static async Task<WorkerProtocolCommand> ReadStartPipelineCommandAsync(
        OrchestrationWorkerProtocolChannel workerChannel,
        string expectedPipelineName)
    {
        var command = await ReadWorkerCommandAsync(workerChannel).ConfigureAwait(false);
        if (!string.Equals(command.Kind, WorkerCommandKinds.StartPipeline, StringComparison.OrdinalIgnoreCase))
        {
            throw new MetaPipelineConfigurationException(
                $"Worker command '{command.Kind}' is not supported before pipeline activation. Expected {WorkerCommandKinds.StartPipeline}.");
        }

        if (string.IsNullOrWhiteSpace(command.PipelineName))
        {
            throw new MetaPipelineConfigurationException(
                $"Worker command '{WorkerCommandKinds.StartPipeline}' must name the pipeline to activate.");
        }

        if (!string.Equals(command.PipelineName, expectedPipelineName, StringComparison.OrdinalIgnoreCase))
        {
            throw new MetaPipelineConfigurationException(
                $"Worker command '{WorkerCommandKinds.StartPipeline}' named pipeline '{command.PipelineName}', but this worker was started for pipeline '{expectedPipelineName}'.");
        }

        return command;
    }

    private static void ValidateStartPipelineCommand(
        WorkerProtocolCommand command,
        MetaPipelineModeledExecutionPlan plan)
    {
        if (!string.IsNullOrWhiteSpace(command.PipelineId) &&
            !string.Equals(command.PipelineId, plan.PipelineId, StringComparison.Ordinal))
        {
            throw new MetaPipelineConfigurationException(
                $"Worker command '{WorkerCommandKinds.StartPipeline}' named pipeline id '{command.PipelineId}', but resolved pipeline '{plan.PipelineName}' has id '{plan.PipelineId}'.");
        }
    }

    private static async Task<WorkerProtocolCommand> ReadWorkerCommandAtBoundaryAsync(
        OrchestrationWorkerProtocolChannel workerChannel,
        MetaPipelineModeledExecutionPlan plan,
        MetaPipelineModeledExecutionStep step,
        string executableVersion,
        bool emitTaskReady)
    {
        if (emitTaskReady)
        {
            await WritePipelineWorkerEventAsync(
                workerChannel,
                WorkerEventKinds.TaskReady,
                plan,
                step,
                string.Empty,
                string.Empty,
                0,
                0,
                executableVersion,
                string.Empty).ConfigureAwait(false);
        }

        var command = await ReadWorkerCommandAsync(workerChannel).ConfigureAwait(false);
        if (!string.Equals(command.TaskId, step.TaskId, StringComparison.Ordinal))
        {
            throw new MetaPipelineConfigurationException(
                $"Worker command '{command.Kind}' named task id '{command.TaskId}', but pipeline '{plan.PipelineName}' is waiting at task id '{step.TaskId}'.");
        }

        return command;
    }

    private static Task WritePipelineWorkerEventAsync(
        OrchestrationWorkerProtocolChannel workerChannel,
        string kind,
        MetaPipelineModeledExecutionPlan plan,
        MetaPipelineModeledExecutionStep? step,
        string grantId,
        string commandId,
        int attemptNumber,
        int exitCode,
        string executableVersion,
        string message,
        string failureClass = "")
    {
        return workerChannel.WriteEventAsync(new WorkerProtocolEvent(
            kind,
            Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture),
            plan.PipelineId,
            plan.PipelineName,
            step?.TaskId ?? string.Empty,
            step?.TaskName ?? string.Empty,
            grantId,
            commandId,
            attemptNumber,
            exitCode,
            executableVersion,
            message,
            failureClass));
    }

    private static Task WriteWorkerLifecycleEventAsync(
        OrchestrationWorkerProtocolChannel workerChannel,
        string kind,
        string executableVersion,
        string message)
    {
        return workerChannel.WriteEventAsync(new WorkerProtocolEvent(
            kind,
            Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture),
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            0,
            0,
            executableVersion,
            message,
            string.Empty));
    }

    private static string ClassifyPipelineFailureClass(PipelineExecutionFailureStage failureStage) =>
        failureStage switch
        {
            PipelineExecutionFailureStage.SourceRead => WorkerFailureClasses.TransientSql,
            PipelineExecutionFailureStage.TransformExecution => WorkerFailureClasses.TransientSql,
            PipelineExecutionFailureStage.TargetWrite => WorkerFailureClasses.TransientSql,
            PipelineExecutionFailureStage.ShapeValidation => WorkerFailureClasses.DeterministicModelError,
            _ => WorkerFailureClasses.WorkerReportedRetryable
        };

    private static MetaPipelineExecutionResult CreateModeledPlanResult(
        MetaPipelineModeledExecutionPlan plan,
        MetaPipelineExecutionStatus status,
        DateTimeOffset startedAtUtc,
        long rowCount,
        int batchCount,
        int columnCount,
        PipelineExecutionFailureStage failureStage,
        string failureMessage,
        string failureTaskName,
        IReadOnlyList<MetaPipelineExecutionTaskResult> taskResults)
    {
        var scriptNames = string.Join(
            " -> ",
            plan.Steps.Select(static item => RenderStepSubject(item)));
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

        return new MetaPipelineExecutionResult(
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
        MetaPipelineModeledExecutionPlan plan)
    {
        if (plan.Steps.Count != 1)
        {
            return "ModeledSerialPipeline";
        }

        if (plan.Steps[0].StepKind == MetaPipelineModeledExecutionStepKind.Executable)
        {
            return "ProcessExecute";
        }

        return plan.Steps[0].IsSelect
            ? "SqlServerBulkInsert"
            : "SqlServerExecuteNonQuery";
    }

    private static void AddSkippedFutureTasks(
        MetaPipelineModeledExecutionPlan plan,
        int startIndex,
        ICollection<MetaPipelineExecutionTaskResult> taskResults)
    {
        for (var index = startIndex; index < plan.Steps.Count; index++)
        {
            var step = plan.Steps[index];
            taskResults.Add(CreateSkippedTaskResult(step.TaskName, ResolveStepTaskKind(step), step.TimeoutSeconds));
            if (step.IsSelect && !string.IsNullOrWhiteSpace(step.TargetWriteTaskName))
            {
                taskResults.Add(CreateSkippedTaskResult(step.TargetWriteTaskName, "TargetWrite", step.TimeoutSeconds));
            }
        }
    }

    private static MetaPipelineExecutionTaskResult CreateSkippedTaskResult(
        string taskName,
        string taskKind,
        int? timeoutSeconds)
    {
        var skippedAtUtc = DateTimeOffset.UtcNow;
        return new MetaPipelineExecutionTaskResult(
            taskName,
            taskKind,
            MetaPipelineExecutionTaskStatus.Skipped,
            skippedAtUtc,
            skippedAtUtc,
            0,
            0,
            PipelineExecutionFailureStage.None,
            string.Empty,
            TimeoutSeconds: timeoutSeconds);
    }

    private static IReadOnlyList<MetaPipelineOperationalFingerprint> BuildModeledOperationalFingerprints(
        MetaPipelineModeledExecutionPlan plan,
        string? dataTypeConversionWorkspacePath)
    {
        var service = new MetaPipelineWorkspaceFingerprintService();
        var fingerprints = new List<MetaPipelineOperationalFingerprint>
        {
            service.CreateWorkspaceFingerprint(
                "PipelineWorkspace",
                plan.PipelineId,
                plan.PipelineWorkspacePath),
        };

        var transformWorkspacePaths = plan.Steps
            .Select(static step => step.TransformWorkspacePath)
            .Where(static path => !string.IsNullOrWhiteSpace(path))
            .Select(static path => path!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        for (var index = 0; index < transformWorkspacePaths.Length; index++)
        {
            fingerprints.Add(service.CreateWorkspaceFingerprint(
                "TransformWorkspace",
                index.ToString(System.Globalization.CultureInfo.InvariantCulture),
                transformWorkspacePaths[index]));
        }

        var bindingWorkspacePaths = plan.Steps
            .Select(static step => step.BindingWorkspacePath)
            .Where(static path => !string.IsNullOrWhiteSpace(path))
            .Select(static path => path!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        for (var index = 0; index < bindingWorkspacePaths.Length; index++)
        {
            fingerprints.Add(service.CreateWorkspaceFingerprint(
                "BindingWorkspace",
                index.ToString(System.Globalization.CultureInfo.InvariantCulture),
                bindingWorkspacePaths[index]));
        }

        foreach (var step in plan.Steps)
        {
            if (step.StepKind == MetaPipelineModeledExecutionStepKind.Executable)
            {
                continue;
            }

            fingerprints.Add(service.CreateWorkspaceFingerprint(
                "TransformScript",
                step.TransformScriptId ?? string.Empty,
                step.TransformWorkspacePath
                ?? throw new MetaPipelineConfigurationException($"Transform task '{step.TaskName}' must name TransformWorkspacePath."),
                step.TaskName,
                "TransformExecution"));
            fingerprints.Add(service.CreateWorkspaceFingerprint(
                "TransformBinding",
                step.TransformBindingId ?? string.Empty,
                step.BindingWorkspacePath
                ?? throw new MetaPipelineConfigurationException($"Transform task '{step.TaskName}' must name BindingWorkspacePath."),
                step.TaskName,
                "TransformExecution"));

            if (step.IsSelect
                && !string.IsNullOrWhiteSpace(step.TargetWriteTaskName)
                && !string.IsNullOrWhiteSpace(dataTypeConversionWorkspacePath))
            {
                fingerprints.Add(service.CreateWorkspaceFingerprint(
                    "DataTypeConversionWorkspace",
                    step.TargetDataTypeSystemName ?? "SqlServer",
                    dataTypeConversionWorkspacePath,
                    step.TargetWriteTaskName,
                    "TargetWrite"));
            }
        }

        return fingerprints;
    }

    private static IReadOnlyList<MetaPipelineOperationalFingerprint> BuildDirectOperationalFingerprints(
        MetaPipelineDirectSqlServerExecutionCommandRequest request,
        MetaPipelineTransformSelection selection,
        MetaPipelineExecutionResult result)
    {
        var service = new MetaPipelineWorkspaceFingerprintService();
        var fingerprints = new List<MetaPipelineOperationalFingerprint>
        {
            service.CreateWorkspaceFingerprint(
                "TransformWorkspace",
                "all",
                request.TransformWorkspacePath),
            service.CreateWorkspaceFingerprint(
                "BindingWorkspace",
                "all",
                request.BindingWorkspacePath),
        };

        var transformTask = result.TaskResults.FirstOrDefault(static task =>
            string.Equals(task.TaskKind, "TransformExecution", StringComparison.Ordinal));
        if (transformTask is not null)
        {
            fingerprints.Add(service.CreateWorkspaceFingerprint(
                "TransformScript",
                selection.TransformScriptId,
                request.TransformWorkspacePath,
                transformTask.TaskName,
                transformTask.TaskKind));
            fingerprints.Add(service.CreateWorkspaceFingerprint(
                "TransformBinding",
                selection.TransformBindingId,
                request.BindingWorkspacePath,
                transformTask.TaskName,
                transformTask.TaskKind));
        }

        var targetWriteTask = result.TaskResults.FirstOrDefault(static task =>
            string.Equals(task.TaskKind, "TargetWrite", StringComparison.Ordinal));
        if (targetWriteTask is not null && !string.IsNullOrWhiteSpace(request.DataTypeConversionWorkspacePath))
        {
            fingerprints.Add(service.CreateWorkspaceFingerprint(
                "DataTypeConversionWorkspace",
                request.TargetDataTypeSystemName,
                request.DataTypeConversionWorkspacePath,
                targetWriteTask.TaskName,
                targetWriteTask.TaskKind));
        }

        return fingerprints;
    }

    private static async Task<MetaPipelineExecutionContext?> CreateExecutionContextAsync(
        MetaPipelineOperationalDbStore? operationalDb,
        Guid? operationalRunId,
        string? pipelineName,
        string? taskName,
        string taskKind)
    {
        if (operationalDb is null || operationalRunId is not Guid runId)
        {
            return null;
        }

        return new MetaPipelineExecutionContext(
            runId,
            Guid.NewGuid(),
            await operationalDb.ReserveAuditIdAsync().ConfigureAwait(false),
            DateTimeOffset.UtcNow,
            pipelineName,
            taskName,
            taskKind);
    }

    private static string ResolveStepTaskKind(MetaPipelineModeledExecutionStep step) =>
        step.StepKind == MetaPipelineModeledExecutionStepKind.Executable
            ? "Executable"
            : "TransformExecution";

    private static int ResolveFailureExitCode(MetaPipelineExecutionResult result) =>
        result.TaskResults
            .FirstOrDefault(static task => task.Status == MetaPipelineExecutionTaskStatus.Failed)
            ?.ExitCode
        ?? 4;

    private static string RenderStepSubject(MetaPipelineModeledExecutionStep step) =>
        step.StepKind == MetaPipelineModeledExecutionStepKind.Executable
            ? step.TaskName
            : step.TransformScriptName ?? step.TaskName;

    private static MetaPipelineOperationalDbStore? CreateOperationalDbStore(string connectionEnvironmentVariableName)
    {
        if (string.IsNullOrWhiteSpace(connectionEnvironmentVariableName))
        {
            return null;
        }

        var connectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(connectionEnvironmentVariableName);
        return new MetaPipelineOperationalDbStore(connectionString);
    }

    private static string FullPathOrEmpty(string? path) =>
        string.IsNullOrWhiteSpace(path)
            ? string.Empty
            : Path.GetFullPath(path);

    private static bool IsOperationalDbStartupFailure(
        MetaPipelineOperationalDbStore? operationalDb,
        Guid? operationalRunId) =>
        operationalDb is not null && operationalRunId is null;

    private static async Task<IReadOnlyList<string>> RecordOperationalFailureAsync(
        MetaPipelineOperationalDbStore? operationalDb,
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

    private static MetaPipelineExecutionCommandOutcome CreateOutcome(
        MetaPipelineExecutionCommandStatus status,
        MetaPipelineExecutionResult? result,
        MetaPipelineModeledExecutionPlan? plan,
        MetaPipelineTransformSelection? selection,
        Guid? operationalRunId,
        IReadOnlyList<string> details,
        Exception? exception,
        IMetaPipelineExecutionProgress? progress) =>
        new(status, result, plan, selection, operationalRunId, details, exception, progress is not null);

    private static void CompleteProgress(IMetaPipelineExecutionProgress? progress, bool failed)
    {
        if (progress is null)
        {
            return;
        }

        progress.Complete(failed);
        progress.Dispose();
    }
}
