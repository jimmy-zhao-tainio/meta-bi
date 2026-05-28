using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;

namespace MetaPipeline;

public sealed class MetaPipelineOperationalDbStore
{
    private readonly string connectionString;

    public MetaPipelineOperationalDbStore(string connectionString)
    {
        this.connectionString = string.IsNullOrWhiteSpace(connectionString)
            ? throw new ArgumentException("Connection string is required.", nameof(connectionString))
            : connectionString;
    }

    public async Task BootstrapAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = MetaPipelineOperationalDbSchema.BootstrapSql;
        command.CommandTimeout = 0;
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task CreateDatabaseAndBootstrapAsync(
        string pipelineDatabaseName,
        CancellationToken cancellationToken = default)
    {
        var normalizedDatabaseName = NormalizeDatabaseName(pipelineDatabaseName);
        var serverConnectionBuilder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = "master",
        };

        await using (var connection = new SqlConnection(serverConnectionBuilder.ConnectionString))
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await ExecuteNonQueryAsync(
                connection,
                null,
                $"""
IF DB_ID(@PipelineDatabaseName) IS NULL
BEGIN
    CREATE DATABASE {QuoteSqlIdentifier(normalizedDatabaseName)};
END;
""",
                cancellationToken,
                Parameter("@PipelineDatabaseName", normalizedDatabaseName));
        }

        var pipelineConnectionBuilder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = normalizedDatabaseName,
        };
        await new MetaPipelineOperationalDbStore(pipelineConnectionBuilder.ConnectionString)
            .BootstrapAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<MetaPipelineOperationalPruneResult> PruneAsync(
        int retentionDays,
        bool dryRun,
        CancellationToken cancellationToken = default)
    {
        if (retentionDays <= 0)
        {
            throw new MetaPipelineConfigurationException("Retention days must be greater than zero.");
        }

        var cutoffUtc = DateTimeOffset.UtcNow.AddDays(-retentionDays);
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 0;
            command.CommandText = """
DECLARE @PipelineRunIds TABLE
(
    [PipelineRunId] uniqueidentifier NOT NULL PRIMARY KEY
);

INSERT INTO @PipelineRunIds ([PipelineRunId])
SELECT [PipelineRunId]
FROM [MetaPipeline].[PipelineRun]
WHERE [CompletedAtUtc] IS NOT NULL
  AND [CompletedAtUtc] < @CutoffUtc;

DECLARE @EligibleCompletedRuns bigint = (SELECT COUNT_BIG(*) FROM @PipelineRunIds);
DECLARE @RunDiagnosticsLogs bigint = (
    SELECT COUNT_BIG(*)
    FROM [MetaPipeline].[RunDiagnosticsLog] [diagnostic]
    INNER JOIN @PipelineRunIds [run] ON [run].[PipelineRunId] = [diagnostic].[PipelineRunId]);

IF @DryRun = 0
BEGIN
    DELETE [diagnostic]
    FROM [MetaPipeline].[RunDiagnosticsLog] [diagnostic]
    INNER JOIN @PipelineRunIds [run] ON [run].[PipelineRunId] = [diagnostic].[PipelineRunId];
END;

SELECT
    @EligibleCompletedRuns AS [EligibleCompletedRuns],
    @RunDiagnosticsLogs AS [RunDiagnosticsLogs];
""";
            command.Parameters.Add(Parameter("@CutoffUtc", cutoffUtc));
            command.Parameters.Add(Parameter("@DryRun", dryRun ? 1 : 0));

            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                throw new MetaPipelineConfigurationException("Pipeline DB prune did not return row counts.");
            }

            var result = new MetaPipelineOperationalPruneResult(
                cutoffUtc,
                dryRun,
                reader.GetInt64(0),
                reader.GetInt64(1));
            await reader.CloseAsync().ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<Guid> StartRunAsync(
        MetaPipelineOperationalRunStart run,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(run);

        var runId = Guid.NewGuid();
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await ExecuteNonQueryAsync(
                connection,
                transaction,
                """
INSERT INTO [MetaPipeline].[PipelineRun]
(
    [PipelineRunId],
    [StartedAtUtc],
    [Status],
    [PipelineWorkspacePath],
    [PipelineId],
    [PipelineName],
    [TransformTaskId],
    [TransformTaskName],
    [TargetWriteTaskId],
    [TargetWriteTaskName],
    [TransformWorkspacePath],
    [BindingWorkspacePath],
    [TransformScriptId],
    [TransformBindingId],
    [TransformScriptName],
    [ExecutionConnectionReferenceName],
    [ExecutionConnectionEnvironmentVariableName],
    [TargetConnectionReferenceName],
    [TargetConnectionEnvironmentVariableName],
    [TargetSqlIdentifier],
    [TargetWriteModelName],
    [BatchSize]
)
VALUES
(
    @PipelineRunId,
    @StartedAtUtc,
    N'Running',
    @PipelineWorkspacePath,
    @PipelineId,
    @PipelineName,
    @TransformTaskId,
    @TransformTaskName,
    @TargetWriteTaskId,
    @TargetWriteTaskName,
    @TransformWorkspacePath,
    @BindingWorkspacePath,
    @TransformScriptId,
    @TransformBindingId,
    @TransformScriptName,
    @ExecutionConnectionReferenceName,
    @ExecutionConnectionEnvironmentVariableName,
    @TargetConnectionReferenceName,
    @TargetConnectionEnvironmentVariableName,
    @TargetSqlIdentifier,
    @TargetWriteModelName,
    @BatchSize
);
""",
                cancellationToken,
                Parameter("@PipelineRunId", runId),
                Parameter("@StartedAtUtc", DateTimeOffset.UtcNow),
                Parameter("@PipelineWorkspacePath", Normalize(run.PipelineWorkspacePath)),
                Parameter("@PipelineId", Normalize(run.PipelineId)),
                Parameter("@PipelineName", Normalize(run.PipelineName)),
                Parameter("@TransformTaskId", Normalize(run.TransformTaskId)),
                Parameter("@TransformTaskName", Normalize(run.TransformTaskName)),
                Parameter("@TargetWriteTaskId", Normalize(run.TargetWriteTaskId)),
                Parameter("@TargetWriteTaskName", Normalize(run.TargetWriteTaskName)),
                Parameter("@TransformWorkspacePath", Normalize(run.TransformWorkspacePath)),
                Parameter("@BindingWorkspacePath", Normalize(run.BindingWorkspacePath)),
                Parameter("@TransformScriptId", Normalize(run.TransformScriptId)),
                Parameter("@TransformBindingId", Normalize(run.TransformBindingId)),
                Parameter("@TransformScriptName", Normalize(run.TransformScriptName)),
                Parameter("@ExecutionConnectionReferenceName", Normalize(run.ExecutionConnectionReferenceName)),
                Parameter("@ExecutionConnectionEnvironmentVariableName", Normalize(run.ExecutionConnectionEnvironmentVariableName)),
                Parameter("@TargetConnectionReferenceName", Normalize(run.TargetConnectionReferenceName)),
                Parameter("@TargetConnectionEnvironmentVariableName", Normalize(run.TargetConnectionEnvironmentVariableName)),
                Parameter("@TargetSqlIdentifier", Normalize(run.TargetSqlIdentifier)),
                Parameter("@TargetWriteModelName", Normalize(run.TargetWriteModelName)),
                Parameter("@BatchSize", run.BatchSize));

            await InsertLogAsync(
                connection,
                transaction,
                runId,
                null,
                "Information",
                "PipelineRun",
                "Pipeline run started.",
                cancellationToken).ConfigureAwait(false);

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return runId;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    public async Task UpdateRunContextAsync(
        Guid runId,
        MetaPipelineOperationalRunStart run,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(run);

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await ExecuteNonQueryAsync(
            connection,
            null,
            """
UPDATE [MetaPipeline].[PipelineRun]
SET
    [PipelineWorkspacePath] = COALESCE(@PipelineWorkspacePath, [PipelineWorkspacePath]),
    [PipelineId] = COALESCE(@PipelineId, [PipelineId]),
    [PipelineName] = COALESCE(@PipelineName, [PipelineName]),
    [TransformTaskId] = COALESCE(@TransformTaskId, [TransformTaskId]),
    [TransformTaskName] = COALESCE(@TransformTaskName, [TransformTaskName]),
    [TargetWriteTaskId] = COALESCE(@TargetWriteTaskId, [TargetWriteTaskId]),
    [TargetWriteTaskName] = COALESCE(@TargetWriteTaskName, [TargetWriteTaskName]),
    [TransformWorkspacePath] = COALESCE(@TransformWorkspacePath, [TransformWorkspacePath]),
    [BindingWorkspacePath] = COALESCE(@BindingWorkspacePath, [BindingWorkspacePath]),
    [TransformScriptId] = COALESCE(@TransformScriptId, [TransformScriptId]),
    [TransformBindingId] = COALESCE(@TransformBindingId, [TransformBindingId]),
    [TransformScriptName] = COALESCE(@TransformScriptName, [TransformScriptName]),
    [ExecutionConnectionReferenceName] = COALESCE(@ExecutionConnectionReferenceName, [ExecutionConnectionReferenceName]),
    [ExecutionConnectionEnvironmentVariableName] = COALESCE(@ExecutionConnectionEnvironmentVariableName, [ExecutionConnectionEnvironmentVariableName]),
    [TargetConnectionReferenceName] = COALESCE(@TargetConnectionReferenceName, [TargetConnectionReferenceName]),
    [TargetConnectionEnvironmentVariableName] = COALESCE(@TargetConnectionEnvironmentVariableName, [TargetConnectionEnvironmentVariableName]),
    [TargetSqlIdentifier] = COALESCE(@TargetSqlIdentifier, [TargetSqlIdentifier]),
    [TargetWriteModelName] = COALESCE(@TargetWriteModelName, [TargetWriteModelName]),
    [BatchSize] = COALESCE(@BatchSize, [BatchSize])
WHERE [PipelineRunId] = @PipelineRunId;
""",
            cancellationToken,
            Parameter("@PipelineRunId", runId),
            Parameter("@PipelineWorkspacePath", Normalize(run.PipelineWorkspacePath)),
            Parameter("@PipelineId", Normalize(run.PipelineId)),
            Parameter("@PipelineName", Normalize(run.PipelineName)),
            Parameter("@TransformTaskId", Normalize(run.TransformTaskId)),
            Parameter("@TransformTaskName", Normalize(run.TransformTaskName)),
            Parameter("@TargetWriteTaskId", Normalize(run.TargetWriteTaskId)),
            Parameter("@TargetWriteTaskName", Normalize(run.TargetWriteTaskName)),
            Parameter("@TransformWorkspacePath", Normalize(run.TransformWorkspacePath)),
            Parameter("@BindingWorkspacePath", Normalize(run.BindingWorkspacePath)),
            Parameter("@TransformScriptId", Normalize(run.TransformScriptId)),
            Parameter("@TransformBindingId", Normalize(run.TransformBindingId)),
            Parameter("@TransformScriptName", Normalize(run.TransformScriptName)),
            Parameter("@ExecutionConnectionReferenceName", Normalize(run.ExecutionConnectionReferenceName)),
            Parameter("@ExecutionConnectionEnvironmentVariableName", Normalize(run.ExecutionConnectionEnvironmentVariableName)),
            Parameter("@TargetConnectionReferenceName", Normalize(run.TargetConnectionReferenceName)),
            Parameter("@TargetConnectionEnvironmentVariableName", Normalize(run.TargetConnectionEnvironmentVariableName)),
            Parameter("@TargetSqlIdentifier", Normalize(run.TargetSqlIdentifier)),
            Parameter("@TargetWriteModelName", Normalize(run.TargetWriteModelName)),
            Parameter("@BatchSize", run.BatchSize));
    }

    public async Task<long> ReserveAuditIdAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = "SELECT NEXT VALUE FOR [MetaPipeline].[AuditIdSequence];";
        command.CommandTimeout = 0;
        var value = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToInt64(value, CultureInfo.InvariantCulture);
    }

    public async Task CompleteRunAsync(
        Guid runId,
        MetaPipelineExecutionResult result,
        IReadOnlyList<MetaPipelineOperationalFingerprint>? fingerprints = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await ExecuteNonQueryAsync(
                connection,
                transaction,
                """
UPDATE [MetaPipeline].[PipelineRun]
SET
    [CompletedAtUtc] = @CompletedAtUtc,
    [Status] = @Status,
    [TransformScriptName] = COALESCE(NULLIF(@TransformScriptName, N''), [TransformScriptName]),
    [TargetSqlIdentifier] = COALESCE(NULLIF(@TargetSqlIdentifier, N''), [TargetSqlIdentifier]),
    [TargetWriteModelName] = COALESCE(NULLIF(@TargetWriteModelName, N''), [TargetWriteModelName]),
    [FailureStage] = NULLIF(@FailureStage, N'None'),
    [FailureKind] = CASE WHEN @Status = N'Failed' THEN N'Runtime' ELSE NULL END,
    [FailureMessage] = NULLIF(@FailureMessage, N'')
WHERE [PipelineRunId] = @PipelineRunId;
""",
                cancellationToken,
                Parameter("@PipelineRunId", runId),
                Parameter("@CompletedAtUtc", result.CompletedAtUtc),
                Parameter("@Status", result.Status.ToString()),
                Parameter("@TransformScriptName", result.TransformScriptName),
                Parameter("@TargetSqlIdentifier", result.TargetSqlIdentifier),
                Parameter("@TargetWriteModelName", result.TargetWriteModelName),
                Parameter("@FailureStage", result.FailureStage.ToString()),
                Parameter("@FailureMessage", result.FailureMessage));

            await InsertRunMetricAsync(connection, transaction, runId, null, "ColumnCount", result.ColumnCount, "count", cancellationToken).ConfigureAwait(false);
            await InsertRunMetricAsync(connection, transaction, runId, null, "RowCount", result.RowCount, "rows", cancellationToken).ConfigureAwait(false);
            await InsertRunMetricAsync(connection, transaction, runId, null, "BatchCount", result.BatchCount, "batches", cancellationToken).ConfigureAwait(false);
            await InsertRunMetricAsync(connection, transaction, runId, null, "TaskCount", result.TaskResults.Count, "count", cancellationToken).ConfigureAwait(false);
            await InsertRunMetricAsync(connection, transaction, runId, null, "DurationMilliseconds", DurationMilliseconds(result.StartedAtUtc, result.CompletedAtUtc), "ms", cancellationToken).ConfigureAwait(false);

            Guid? failedTaskRunId = null;
            var fingerprintList = fingerprints ?? Array.Empty<MetaPipelineOperationalFingerprint>();
            var taskRunIdsByKey = fingerprintList.Count == 0
                ? null
                : new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
            foreach (var task in result.TaskResults)
            {
                var taskRunId = task.TaskRunId ?? Guid.NewGuid();
                if (taskRunIdsByKey is not null)
                {
                    var taskKey = BuildTaskKey(task.TaskName, task.TaskKind);
                    if (!taskRunIdsByKey.TryAdd(taskKey, taskRunId))
                    {
                        throw new MetaPipelineConfigurationException(
                            $"Task fingerprint evidence is ambiguous because task '{task.TaskName}' kind '{task.TaskKind}' appears more than once.");
                    }
                }

                await ExecuteNonQueryAsync(
                    connection,
                    transaction,
                    """
INSERT INTO [MetaPipeline].[TaskRun]
(
    [TaskRunId],
    [PipelineRunId],
    [AuditId],
    [TaskName],
    [TaskKind],
    [StartedAtUtc],
    [CompletedAtUtc],
    [Status],
    [TimeoutSeconds],
    [FailureStage],
    [FailureMessage]
)
VALUES
(
    @TaskRunId,
    @PipelineRunId,
    @AuditId,
    @TaskName,
    @TaskKind,
    @StartedAtUtc,
    @CompletedAtUtc,
    @Status,
    @TimeoutSeconds,
    @FailureStage,
    @FailureMessage
);
""",
                    cancellationToken,
                    Parameter("@TaskRunId", taskRunId),
                    Parameter("@PipelineRunId", runId),
                    Parameter("@AuditId", task.AuditId),
                    Parameter("@TaskName", task.TaskName),
                    Parameter("@TaskKind", task.TaskKind),
                    Parameter("@StartedAtUtc", task.StartedAtUtc),
                    Parameter("@CompletedAtUtc", task.CompletedAtUtc),
                    Parameter("@Status", task.Status.ToString()),
                    Parameter("@TimeoutSeconds", task.TimeoutSeconds),
                    Parameter("@FailureStage", task.FailureStage == PipelineExecutionFailureStage.None ? null : task.FailureStage.ToString()),
                    Parameter("@FailureMessage", Normalize(task.FailureMessage)));

                await InsertRunMetricAsync(connection, transaction, runId, taskRunId, "RowCount", task.RowCount, "rows", cancellationToken).ConfigureAwait(false);
                await InsertRunMetricAsync(connection, transaction, runId, taskRunId, "BatchCount", task.BatchCount, "batches", cancellationToken).ConfigureAwait(false);
                await InsertRunMetricAsync(connection, transaction, runId, taskRunId, "DurationMilliseconds", DurationMilliseconds(task.StartedAtUtc, task.CompletedAtUtc), "ms", cancellationToken).ConfigureAwait(false);

                if (task.Status == MetaPipelineExecutionTaskStatus.Failed && failedTaskRunId is null)
                {
                    failedTaskRunId = taskRunId;
                }
            }

            if (fingerprintList.Count > 0)
            {
                await InsertFingerprintsAsync(
                    connection,
                    transaction,
                    runId,
                    taskRunIdsByKey ?? throw new InvalidOperationException("Task run id index was not initialized."),
                    fingerprintList,
                    cancellationToken).ConfigureAwait(false);
            }

            if (result.Status == MetaPipelineExecutionStatus.Failed)
            {
                await InsertFailureAsync(
                    connection,
                    transaction,
                    runId,
                    failedTaskRunId,
                    "Runtime",
                    result.FailureStage.ToString(),
                    null,
                    result.FailureMessage,
                    cancellationToken).ConfigureAwait(false);
                await InsertLogAsync(
                    connection,
                    transaction,
                    runId,
                    failedTaskRunId,
                    "Error",
                    "PipelineRun",
                    result.FailureMessage,
                    cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await InsertLogAsync(
                    connection,
                    transaction,
                    runId,
                    null,
                    "Information",
                    "PipelineRun",
                    "Pipeline run completed.",
                    cancellationToken).ConfigureAwait(false);
            }

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    public async Task FailRunAsync(
        Guid runId,
        string failureStage,
        string failureKind,
        string message,
        Exception? exception = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var completedAtUtc = DateTimeOffset.UtcNow;
            await ExecuteNonQueryAsync(
                connection,
                transaction,
                """
UPDATE [MetaPipeline].[PipelineRun]
SET
    [CompletedAtUtc] = @CompletedAtUtc,
    [Status] = N'Failed',
    [FailureStage] = @FailureStage,
    [FailureKind] = @FailureKind,
    [FailureMessage] = @FailureMessage
WHERE [PipelineRunId] = @PipelineRunId;
""",
                cancellationToken,
                Parameter("@PipelineRunId", runId),
                Parameter("@CompletedAtUtc", completedAtUtc),
                Parameter("@FailureStage", failureStage),
                Parameter("@FailureKind", failureKind),
                Parameter("@FailureMessage", message));

            await InsertFailureAsync(
                connection,
                transaction,
                runId,
                null,
                failureKind,
                failureStage,
                exception?.GetType().FullName,
                message,
                cancellationToken).ConfigureAwait(false);
            await InsertLogAsync(
                connection,
                transaction,
                runId,
                null,
                "Error",
                failureKind,
                message,
                cancellationToken).ConfigureAwait(false);

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    private static async Task InsertRunMetricAsync(
        SqlConnection connection,
        SqlTransaction? transaction,
        Guid runId,
        Guid? taskRunId,
        string metricName,
        decimal metricValue,
        string metricUnit,
        CancellationToken cancellationToken)
    {
        await ExecuteNonQueryAsync(
            connection,
            transaction,
            """
INSERT INTO [MetaPipeline].[RunMetric]
(
    [PipelineRunId],
    [TaskRunId],
    [MetricName],
    [MetricValue],
    [MetricUnit]
)
VALUES
(
    @PipelineRunId,
    @TaskRunId,
    @MetricName,
    @MetricValue,
    @MetricUnit
);
""",
            cancellationToken,
            Parameter("@PipelineRunId", runId),
            Parameter("@TaskRunId", taskRunId),
            Parameter("@MetricName", metricName),
            DecimalParameter("@MetricValue", metricValue),
            Parameter("@MetricUnit", metricUnit));
    }

    private static async Task InsertFingerprintsAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        Guid runId,
        IReadOnlyDictionary<string, Guid> taskRunIdsByKey,
        IReadOnlyList<MetaPipelineOperationalFingerprint> fingerprints,
        CancellationToken cancellationToken)
    {
        if (fingerprints.Count == 0)
        {
            return;
        }

        foreach (var fingerprint in fingerprints)
        {
            Guid? taskRunId = null;
            if (!string.IsNullOrWhiteSpace(fingerprint.TaskName)
                || !string.IsNullOrWhiteSpace(fingerprint.TaskKind))
            {
                if (string.IsNullOrWhiteSpace(fingerprint.TaskName) || string.IsNullOrWhiteSpace(fingerprint.TaskKind))
                {
                    throw new MetaPipelineConfigurationException(
                        $"Fingerprint '{fingerprint.FingerprintKind}' must provide both task name and task kind, or neither.");
                }

                var key = BuildTaskKey(fingerprint.TaskName, fingerprint.TaskKind);
                if (!taskRunIdsByKey.TryGetValue(key, out var resolvedTaskRunId))
                {
                    throw new MetaPipelineConfigurationException(
                        $"Fingerprint '{fingerprint.FingerprintKind}' references task '{fingerprint.TaskName}' kind '{fingerprint.TaskKind}', but that task was not recorded.");
                }

                taskRunId = resolvedTaskRunId;
            }

            await ExecuteNonQueryAsync(
                connection,
                transaction,
                """
INSERT INTO [MetaPipeline].[RunFingerprint]
(
    [PipelineRunId],
    [TaskRunId],
    [FingerprintKind],
    [SubjectId],
    [SubjectPath],
    [Algorithm],
    [FingerprintValue]
)
VALUES
(
    @PipelineRunId,
    @TaskRunId,
    @FingerprintKind,
    @SubjectId,
    @SubjectPath,
    @Algorithm,
    @FingerprintValue
);
""",
                cancellationToken,
                Parameter("@PipelineRunId", runId),
                Parameter("@TaskRunId", taskRunId),
                Parameter("@FingerprintKind", fingerprint.FingerprintKind),
                Parameter("@SubjectId", Normalize(fingerprint.SubjectId)),
                Parameter("@SubjectPath", Normalize(fingerprint.SubjectPath)),
                Parameter("@Algorithm", fingerprint.Algorithm),
                Parameter("@FingerprintValue", fingerprint.FingerprintValue));
        }
    }

    private static string BuildTaskKey(string taskName, string taskKind) =>
        taskName.Trim() + "\u001f" + taskKind.Trim();

    private static async Task InsertFailureAsync(
        SqlConnection connection,
        SqlTransaction? transaction,
        Guid runId,
        Guid? taskRunId,
        string failureKind,
        string failureStage,
        string? exceptionType,
        string message,
        CancellationToken cancellationToken)
    {
        await ExecuteNonQueryAsync(
            connection,
            transaction,
            """
INSERT INTO [MetaPipeline].[RunFailure]
(
    [PipelineRunId],
    [TaskRunId],
    [FailureKind],
    [FailureStage],
    [OccurredAtUtc],
    [ExceptionType],
    [Message]
)
VALUES
(
    @PipelineRunId,
    @TaskRunId,
    @FailureKind,
    @FailureStage,
    @OccurredAtUtc,
    @ExceptionType,
    @Message
);
""",
            cancellationToken,
            Parameter("@PipelineRunId", runId),
            Parameter("@TaskRunId", taskRunId),
            Parameter("@FailureKind", failureKind),
            Parameter("@FailureStage", failureStage),
            Parameter("@OccurredAtUtc", DateTimeOffset.UtcNow),
            Parameter("@ExceptionType", Normalize(exceptionType)),
            Parameter("@Message", string.IsNullOrWhiteSpace(message) ? "Pipeline run failed." : message));
    }

    private static async Task InsertLogAsync(
        SqlConnection connection,
        SqlTransaction? transaction,
        Guid runId,
        Guid? taskRunId,
        string level,
        string category,
        string message,
        CancellationToken cancellationToken)
    {
        if (IsDiagnosticsLevel(level))
        {
            await InsertDiagnosticsLogAsync(
                connection,
                transaction,
                runId,
                taskRunId,
                level,
                category,
                message,
                cancellationToken).ConfigureAwait(false);
            return;
        }

        await ExecuteNonQueryAsync(
            connection,
            transaction,
            """
INSERT INTO [MetaPipeline].[RunLog]
(
    [PipelineRunId],
    [TaskRunId],
    [LoggedAtUtc],
    [Level],
    [Category],
    [Message]
)
VALUES
(
    @PipelineRunId,
    @TaskRunId,
    @LoggedAtUtc,
    @Level,
    @Category,
    @Message
);
""",
            cancellationToken,
            Parameter("@PipelineRunId", runId),
            Parameter("@TaskRunId", taskRunId),
            Parameter("@LoggedAtUtc", DateTimeOffset.UtcNow),
            Parameter("@Level", level),
            Parameter("@Category", category),
            Parameter("@Message", string.IsNullOrWhiteSpace(message) ? "Pipeline run event." : message));
    }

    private static async Task InsertDiagnosticsLogAsync(
        SqlConnection connection,
        SqlTransaction? transaction,
        Guid runId,
        Guid? taskRunId,
        string level,
        string category,
        string message,
        CancellationToken cancellationToken)
    {
        await ExecuteNonQueryAsync(
            connection,
            transaction,
            """
INSERT INTO [MetaPipeline].[RunDiagnosticsLog]
(
    [PipelineRunId],
    [TaskRunId],
    [LoggedAtUtc],
    [Level],
    [Category],
    [Message]
)
VALUES
(
    @PipelineRunId,
    @TaskRunId,
    @LoggedAtUtc,
    @Level,
    @Category,
    @Message
);
""",
            cancellationToken,
            Parameter("@PipelineRunId", runId),
            Parameter("@TaskRunId", taskRunId),
            Parameter("@LoggedAtUtc", DateTimeOffset.UtcNow),
            Parameter("@Level", level),
            Parameter("@Category", category),
            Parameter("@Message", string.IsNullOrWhiteSpace(message) ? "Pipeline run event." : message));
    }

    private static bool IsDiagnosticsLevel(string level) =>
        string.Equals(level, "Information", StringComparison.OrdinalIgnoreCase)
        || string.Equals(level, "Debug", StringComparison.OrdinalIgnoreCase)
        || string.Equals(level, "Trace", StringComparison.OrdinalIgnoreCase);

    private static async Task ExecuteNonQueryAsync(
        SqlConnection connection,
        SqlTransaction? transaction,
        string commandText,
        CancellationToken cancellationToken,
        params SqlParameter[] parameters)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandType = CommandType.Text;
        command.CommandText = commandText;
        command.CommandTimeout = 0;
        command.Parameters.AddRange(parameters);
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static SqlParameter Parameter(string name, object? value) =>
        new(name, value ?? DBNull.Value);

    private static SqlParameter DecimalParameter(string name, decimal value)
    {
        var parameter = new SqlParameter(name, SqlDbType.Decimal)
        {
            Precision = 38,
            Scale = 6,
            Value = value,
        };
        return parameter;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string NormalizeDatabaseName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return MetaPipelineOperationalDbSchema.DefaultDatabaseName;
        }

        var normalized = value.Trim();
        if (normalized.Length > 128)
        {
            throw new MetaPipelineConfigurationException("Pipeline DB name must be 128 characters or fewer.");
        }

        if (normalized.IndexOf('\0') >= 0)
        {
            throw new MetaPipelineConfigurationException("Pipeline DB name contains an invalid null character.");
        }

        return normalized;
    }

    private static string QuoteSqlIdentifier(string value) =>
        "[" + value.Replace("]", "]]", StringComparison.Ordinal) + "]";

    private static decimal DurationMilliseconds(DateTimeOffset startedAtUtc, DateTimeOffset completedAtUtc)
    {
        var duration = completedAtUtc - startedAtUtc;
        return duration.Ticks <= 0 ? 0 : (decimal)duration.TotalMilliseconds;
    }
}
