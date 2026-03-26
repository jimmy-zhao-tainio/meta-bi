using System.Data;
using Microsoft.Data.SqlClient;

namespace MetaSql;

/// <summary>
/// Executes rendered SQL batches statement-by-statement using SQL Server autocommit behavior.
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

        for (var i = 0; i < statements.Count; i++)
        {
            var statement = statements[i];
            using var command = connection.CreateCommand();
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
    }
}
