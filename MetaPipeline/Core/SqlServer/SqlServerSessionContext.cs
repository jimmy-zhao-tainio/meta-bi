using System.Data;
using Microsoft.Data.SqlClient;

namespace MetaPipeline;

internal static class SqlServerSessionContext
{
    private static readonly string[] KnownKeys =
    [
        "MetaPipeline.PipelineRunId",
        "MetaPipeline.TaskRunId",
        "MetaPipeline.AuditId",
        "MetaPipeline.TaskStartedAtUtc",
        "MetaPipeline.PipelineName",
        "MetaPipeline.TaskName",
        "MetaPipeline.TaskKind",
    ];

    public static async Task ApplyAsync(
        SqlConnection connection,
        MetaPipelineExecutionContext? context,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(connection);

        foreach (var key in KnownKeys)
        {
            await SetValueAsync(connection, key, DBNull.Value, null, cancellationToken).ConfigureAwait(false);
        }

        if (context is null)
        {
            return;
        }

        await SetValueAsync(connection, "MetaPipeline.PipelineRunId", context.PipelineRunId, SqlDbType.UniqueIdentifier, cancellationToken).ConfigureAwait(false);
        await SetValueAsync(connection, "MetaPipeline.TaskRunId", context.TaskRunId, SqlDbType.UniqueIdentifier, cancellationToken).ConfigureAwait(false);
        await SetValueAsync(connection, "MetaPipeline.AuditId", context.AuditId, SqlDbType.BigInt, cancellationToken).ConfigureAwait(false);
        await SetValueAsync(connection, "MetaPipeline.TaskStartedAtUtc", context.TaskStartedAtUtc.UtcDateTime, SqlDbType.DateTime2, cancellationToken).ConfigureAwait(false);
        await SetValueAsync(connection, "MetaPipeline.PipelineName", Normalize(context.PipelineName), SqlDbType.NVarChar, cancellationToken).ConfigureAwait(false);
        await SetValueAsync(connection, "MetaPipeline.TaskName", Normalize(context.TaskName), SqlDbType.NVarChar, cancellationToken).ConfigureAwait(false);
        await SetValueAsync(connection, "MetaPipeline.TaskKind", Normalize(context.TaskKind), SqlDbType.NVarChar, cancellationToken).ConfigureAwait(false);
    }

    private static async Task SetValueAsync(
        SqlConnection connection,
        string key,
        object? value,
        SqlDbType? sqlDbType,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = "EXEC sys.sp_set_session_context @key = @Key, @value = @Value;";
        command.CommandTimeout = 0;
        command.Parameters.Add(new SqlParameter("@Key", SqlDbType.NVarChar, 128) { Value = key });

        var valueParameter = new SqlParameter("@Value", value ?? DBNull.Value);
        if (sqlDbType.HasValue)
        {
            valueParameter.SqlDbType = sqlDbType.Value;
        }

        command.Parameters.Add(valueParameter);
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
