using System.Data;
using Microsoft.Data.SqlClient;

namespace MetaSql;

/// <summary>
/// Executes rendered SQL batches inside the deploy transaction boundary.
/// </summary>
internal sealed class SqlBatchExecutor
{
    public async Task ExecuteAsync(
        string connectionString,
        IReadOnlyList<string> statements,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentNullException.ThrowIfNull(statements);

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        using var transaction = connection.BeginTransaction();
        try
        {
            using (var init = connection.CreateCommand())
            {
                init.Transaction = transaction;
                init.CommandType = CommandType.Text;
                init.CommandText = "SET XACT_ABORT ON;";
                await init.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            for (var i = 0; i < statements.Count; i++)
            {
                var statement = statements[i];
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandType = CommandType.Text;
                command.CommandText = statement;
                try
                {
                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"SQL deploy failed at statement {i + 1}: {statement}",
                        ex);
                }
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
