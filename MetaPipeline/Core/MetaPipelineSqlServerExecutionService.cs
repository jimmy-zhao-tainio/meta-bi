using Microsoft.Data.SqlClient;

namespace MetaPipeline;

public sealed class MetaPipelineSqlServerExecutionService
{
    private readonly MetaPipelineExecutionWorkspaceResolver workspaceResolver;
    private readonly BufferedPipelineExecutionService bufferedExecutionService;

    public MetaPipelineSqlServerExecutionService(
        MetaPipelineExecutionWorkspaceResolver? workspaceResolver = null,
        BufferedPipelineExecutionService? bufferedExecutionService = null)
    {
        this.workspaceResolver = workspaceResolver ?? new MetaPipelineExecutionWorkspaceResolver();
        this.bufferedExecutionService = bufferedExecutionService ?? new BufferedPipelineExecutionService();
    }

    public async Task<MetaPipelineExecutionResult> ExecuteAsync(
        MetaPipelineSqlServerExecutionRequest request,
        IProgress<BufferedPipelineExecutionProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var startedAtUtc = DateTimeOffset.UtcNow;
        var taskResults = new List<MetaPipelineExecutionTaskResult>();

        if (string.IsNullOrWhiteSpace(request.ExecutionConnectionString))
        {
            throw new MetaPipelineConfigurationException("Execution connection string is required.");
        }

        var definition = workspaceResolver.ResolveByIds(
            request.TransformWorkspacePath,
            request.BindingWorkspacePath,
            request.TransformScriptId,
            request.TransformBindingId,
            request.TargetSqlIdentifier);

        var transformTaskName = ResolveTransformTaskName(request, definition);
        if (!definition.IsSelect)
        {
            var taskStartedAtUtc = request.ExecutionContext?.TaskStartedAtUtc ?? DateTimeOffset.UtcNow;
            try
            {
                var rowsAffected = await ExecuteStatementAsync(
                    request.ExecutionConnectionString,
                    definition.SourceSql,
                    request.ExecutionContext,
                    request.TimeoutSeconds,
                    cancellationToken).ConfigureAwait(false);
                var rowCount = NormalizeRowsAffected(rowsAffected);

                taskResults.Add(CreateSucceededTaskResult(
                    transformTaskName,
                    "TransformExecution",
                    taskStartedAtUtc,
                    DateTimeOffset.UtcNow,
                    rowCount,
                    0,
                    request.TimeoutSeconds,
                    executionContext: request.ExecutionContext) with
                {
                    TransformScriptId = definition.TransformScriptId,
                    TransformScriptName = definition.TransformScriptName,
                });

                return CreateResult(
                    definition,
                    MetaPipelineExecutionStatus.Succeeded,
                    "SqlServerExecuteNonQuery",
                    "None",
                    rowCount,
                    0,
                    startedAtUtc,
                    PipelineExecutionFailureStage.None,
                    string.Empty,
                    string.Empty,
                    taskResults);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                taskResults.Add(CreateFailedTaskResult(
                    transformTaskName,
                    "TransformExecution",
                    taskStartedAtUtc,
                    DateTimeOffset.UtcNow,
                    PipelineExecutionFailureStage.TransformExecution,
                    ex.Message,
                    timeoutSeconds: request.TimeoutSeconds,
                    executionContext: request.ExecutionContext) with
                {
                    TransformScriptId = definition.TransformScriptId,
                    TransformScriptName = definition.TransformScriptName,
                });

                return CreateResult(
                    definition,
                    MetaPipelineExecutionStatus.Failed,
                    "SqlServerExecuteNonQuery",
                    "None",
                    0,
                    0,
                    startedAtUtc,
                    PipelineExecutionFailureStage.TransformExecution,
                    ex.Message,
                    transformTaskName,
                    taskResults);
            }
        }

        if (string.IsNullOrWhiteSpace(request.TargetConnectionString))
        {
            throw new MetaPipelineConfigurationException("Target connection string is required for SELECT-kind InsertRows execution.");
        }

        if (request.BatchSize <= 0)
        {
            throw new MetaPipelineConfigurationException("Batch size must be greater than zero.");
        }

        var targetWriteModelName = NormalizeTargetWriteModelName(request.TargetWriteModelName);
        var targetWriteTaskName = ResolveTargetWriteTaskName(request, targetWriteModelName);
        var dataFlowStartedAtUtc = request.ExecutionContext?.TaskStartedAtUtc ?? DateTimeOffset.UtcNow;
        await using var targetWriteOperation = CreateTargetWriteOperation(
            request,
            definition,
            targetWriteModelName);

        var execution = await bufferedExecutionService.ExecuteAsync(
            new SqlServerTransformRowStreamSource(
                request.ExecutionConnectionString,
                definition.SourceSql,
                definition.RowStreamShape ?? throw new MetaPipelineConfigurationException("SELECT-kind execution requires a row-stream shape."),
                request.BatchSize,
                request.ExecutionContext,
                request.TimeoutSeconds),
            targetWriteOperation,
            progress,
            cancellationToken).ConfigureAwait(false);

        var dataFlowCompletedAtUtc = DateTimeOffset.UtcNow;
        if (!execution.Succeeded)
        {
            if (execution.FailureStage == PipelineExecutionFailureStage.TargetWrite)
            {
                taskResults.Add(CreateFailedTaskResult(
                    transformTaskName,
                    "TransformExecution",
                    dataFlowStartedAtUtc,
                    dataFlowCompletedAtUtc,
                    execution.FailureStage,
                    execution.FailureMessage,
                    execution.RowCount,
                    execution.BatchCount,
                    request.TimeoutSeconds,
                    request.ExecutionContext) with
                {
                    TransformScriptId = definition.TransformScriptId,
                    TransformScriptName = definition.TransformScriptName,
                });
                taskResults.Add(CreateFailedTaskResult(
                    targetWriteTaskName,
                    "TargetWrite",
                    dataFlowStartedAtUtc,
                    dataFlowCompletedAtUtc,
                    execution.FailureStage,
                    execution.FailureMessage,
                    execution.RowCount,
                    execution.BatchCount,
                    request.TimeoutSeconds));

                return CreateResult(
                    definition,
                    MetaPipelineExecutionStatus.Failed,
                    targetWriteOperation.Name,
                    targetWriteModelName,
                    execution.RowCount,
                    execution.BatchCount,
                    startedAtUtc,
                    execution.FailureStage,
                    execution.FailureMessage,
                    targetWriteTaskName,
                    taskResults);
            }

            taskResults.Add(CreateFailedTaskResult(
                transformTaskName,
                "TransformExecution",
                dataFlowStartedAtUtc,
                dataFlowCompletedAtUtc,
                execution.FailureStage,
                execution.FailureMessage,
                execution.RowCount,
                execution.BatchCount,
                request.TimeoutSeconds,
                request.ExecutionContext) with
            {
                TransformScriptId = definition.TransformScriptId,
                TransformScriptName = definition.TransformScriptName,
            });
            taskResults.Add(CreateSkippedTaskResult(targetWriteTaskName, "TargetWrite", request.TimeoutSeconds));

            return CreateResult(
                definition,
                MetaPipelineExecutionStatus.Failed,
                targetWriteOperation.Name,
                targetWriteModelName,
                execution.RowCount,
                execution.BatchCount,
                startedAtUtc,
                execution.FailureStage,
                execution.FailureMessage,
                transformTaskName,
                taskResults);
        }

        taskResults.Add(CreateSucceededTaskResult(
            transformTaskName,
            "TransformExecution",
            dataFlowStartedAtUtc,
            dataFlowCompletedAtUtc,
            execution.RowCount,
            execution.BatchCount,
            request.TimeoutSeconds,
            request.ExecutionContext) with
        {
            TransformScriptId = definition.TransformScriptId,
            TransformScriptName = definition.TransformScriptName,
        });
        taskResults.Add(CreateSucceededTaskResult(
            targetWriteTaskName,
            "TargetWrite",
            dataFlowStartedAtUtc,
            dataFlowCompletedAtUtc,
            execution.RowCount,
            execution.BatchCount,
            request.TimeoutSeconds));

        return CreateResult(
            definition,
            MetaPipelineExecutionStatus.Succeeded,
            targetWriteOperation.Name,
            targetWriteModelName,
            execution.RowCount,
            execution.BatchCount,
            startedAtUtc,
            PipelineExecutionFailureStage.None,
            string.Empty,
            string.Empty,
            taskResults);
    }

    private static MetaPipelineExecutionResult CreateResult(
        MetaPipelineExecutionDefinition definition,
        MetaPipelineExecutionStatus status,
        string targetWriteOperationName,
        string targetWriteModelName,
        long rowCount,
        int batchCount,
        DateTimeOffset startedAtUtc,
        PipelineExecutionFailureStage failureStage,
        string failureMessage,
        string failureTaskName,
        IReadOnlyList<MetaPipelineExecutionTaskResult> taskResults)
    {
        return new MetaPipelineExecutionResult(
            status,
            definition.TransformScriptName,
            definition.TargetSqlIdentifier ?? string.Empty,
            targetWriteOperationName,
            targetWriteModelName,
            definition.RowStreamShape?.ColumnCount ?? 0,
            rowCount,
            batchCount,
            startedAtUtc,
            DateTimeOffset.UtcNow,
            failureStage,
            failureMessage,
            failureTaskName,
            taskResults);
    }

    private static string ResolveTransformTaskName(
        MetaPipelineSqlServerExecutionRequest request,
        MetaPipelineExecutionDefinition definition)
    {
        return string.IsNullOrWhiteSpace(request.TransformTaskName)
            ? definition.TransformScriptName
            : request.TransformTaskName.Trim();
    }

    private static string ResolveTargetWriteTaskName(
        MetaPipelineSqlServerExecutionRequest request,
        string targetWriteModelName)
    {
        return string.IsNullOrWhiteSpace(request.TargetWriteTaskName)
            ? targetWriteModelName
            : request.TargetWriteTaskName.Trim();
    }

    private static string NormalizeTargetWriteModelName(string targetWriteModelName)
    {
        if (string.IsNullOrWhiteSpace(targetWriteModelName))
        {
            return "InsertRows";
        }

        if (string.Equals(targetWriteModelName, "InsertRows", StringComparison.OrdinalIgnoreCase))
        {
            return "InsertRows";
        }

        throw new MetaPipelineConfigurationException(
            $"Unsupported target write model '{targetWriteModelName}'. Supported values: InsertRows.");
    }

    private static string ResolveTargetWriteOperationName(string targetWriteModelName) => "SqlServerBulkInsert";

    private static IPipelineTargetWriteOperation CreateTargetWriteOperation(
        MetaPipelineSqlServerExecutionRequest request,
        MetaPipelineExecutionDefinition definition,
        string targetWriteModelName)
    {
        if (!string.Equals(targetWriteModelName, "InsertRows", StringComparison.Ordinal))
        {
            throw new MetaPipelineConfigurationException(
                $"Unsupported target write model '{targetWriteModelName}'. Supported values: InsertRows.");
        }

        return new SqlServerBulkInsertTargetWriteOperation(
            RequireValue(request.TargetConnectionString, "SELECT-kind execution requires a target connection string."),
            RequireValue(definition.TargetSqlIdentifier, "SELECT-kind execution requires a target SQL identifier."),
            definition.RowStreamShape ?? throw new MetaPipelineConfigurationException("SELECT-kind execution requires a row-stream shape."),
            request.ExecutionContext,
            request.TargetDataTypeSystemName,
            request.DataTypeConversionWorkspacePath,
            request.TimeoutSeconds);
    }

    private static async Task<int> ExecuteStatementAsync(
        string connectionString,
        string sql,
        MetaPipelineExecutionContext? executionContext,
        int? timeoutSeconds,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await SqlServerSessionContext.ApplyAsync(connection, executionContext, cancellationToken).ConfigureAwait(false);
        await using var command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = sql;
        command.CommandTimeout = ResolveCommandTimeoutSeconds(timeoutSeconds);
        return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static int ResolveCommandTimeoutSeconds(int? timeoutSeconds) =>
        timeoutSeconds is null ? 0 : timeoutSeconds.Value;

    private static long NormalizeRowsAffected(int rowsAffected) =>
        rowsAffected < 0 ? 0 : rowsAffected;

    private static string RequireValue(string? value, string errorMessage) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new MetaPipelineConfigurationException(errorMessage)
            : value.Trim();

    private static MetaPipelineExecutionTaskResult CreateSucceededTaskResult(
        string taskName,
        string taskKind,
        DateTimeOffset startedAtUtc,
        DateTimeOffset completedAtUtc,
        long rowCount = 0,
        int batchCount = 0,
        int? timeoutSeconds = null,
        MetaPipelineExecutionContext? executionContext = null)
    {
        return new MetaPipelineExecutionTaskResult(
            taskName,
            taskKind,
            MetaPipelineExecutionTaskStatus.Succeeded,
            startedAtUtc,
            completedAtUtc,
            rowCount,
            batchCount,
            PipelineExecutionFailureStage.None,
            string.Empty,
            executionContext?.TaskRunId,
            executionContext?.AuditId,
            timeoutSeconds);
    }

    private static MetaPipelineExecutionTaskResult CreateFailedTaskResult(
        string taskName,
        string taskKind,
        DateTimeOffset startedAtUtc,
        DateTimeOffset completedAtUtc,
        PipelineExecutionFailureStage failureStage,
        string failureMessage,
        long rowCount = 0,
        int batchCount = 0,
        int? timeoutSeconds = null,
        MetaPipelineExecutionContext? executionContext = null)
    {
        return new MetaPipelineExecutionTaskResult(
            taskName,
            taskKind,
            MetaPipelineExecutionTaskStatus.Failed,
            startedAtUtc,
            completedAtUtc,
            rowCount,
            batchCount,
            failureStage,
            failureMessage,
            executionContext?.TaskRunId,
            executionContext?.AuditId,
            timeoutSeconds);
    }

    private static MetaPipelineExecutionTaskResult CreateSkippedTaskResult(
        string taskName,
        string taskKind,
        int? timeoutSeconds = null)
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

}
