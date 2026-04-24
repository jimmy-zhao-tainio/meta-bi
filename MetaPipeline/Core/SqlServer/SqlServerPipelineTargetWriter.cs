using Microsoft.Data.SqlClient;
using System.Data;

namespace MetaPipeline;

public sealed class SqlServerPipelineTargetWriter : IPipelineBatchWriter, IAsyncDisposable
{
    private readonly string connectionString;
    private readonly string destinationTableName;
    private readonly IReadOnlyList<PipelineColumn> columns;
    private SqlConnection? connection;
    private bool disposed;

    public SqlServerPipelineTargetWriter(
        string connectionString,
        string targetSqlIdentifier,
        IReadOnlyList<PipelineColumn> columns)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetSqlIdentifier);
        ArgumentNullException.ThrowIfNull(columns);

        if (columns.Count == 0)
        {
            throw new MetaPipelineConfigurationException(
                "Target writer requires at least one column.");
        }

        var blankColumn = columns.FirstOrDefault(static item => string.IsNullOrWhiteSpace(item.Name));
        if (blankColumn is not null)
        {
            throw new MetaPipelineConfigurationException(
                "Target writer received a column with a blank name.");
        }

        var duplicateColumnNames = columns
            .GroupBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
            .Where(static group => group.Count() > 1)
            .Select(static group => group.Key)
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (duplicateColumnNames.Length > 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Target writer received duplicate column names: {string.Join(", ", duplicateColumnNames)}.");
        }

        this.connectionString = connectionString;
        this.columns = columns.OrderBy(static item => item.Ordinal).ToArray();
        destinationTableName = SqlServerMultipartIdentifier.Parse(targetSqlIdentifier).RenderBracketQuoted();
    }

    public async Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        ArgumentNullException.ThrowIfNull(batch);

        if (batch.RowCount == 0)
        {
            return;
        }

        var currentConnection = await EnsureConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = await currentConnection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        using var table = CreateDataTable(batch);
        using var bulkCopy = new SqlBulkCopy(
            currentConnection,
            SqlBulkCopyOptions.Default,
            (SqlTransaction)transaction)
        {
            DestinationTableName = destinationTableName,
            BatchSize = batch.RowCount,
            BulkCopyTimeout = 0,
        };

        foreach (var column in columns)
        {
            bulkCopy.ColumnMappings.Add(column.Name, column.Name);
        }

        await bulkCopy.WriteToServerAsync(table, cancellationToken).ConfigureAwait(false);
        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        if (connection is not null)
        {
            await connection.DisposeAsync().ConfigureAwait(false);
        }
    }

    private async Task<SqlConnection> EnsureConnectionAsync(CancellationToken cancellationToken)
    {
        if (connection is not null)
        {
            return connection;
        }

        connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return connection;
    }

    private DataTable CreateDataTable(PipelineDataBatch batch)
    {
        var table = new DataTable();
        foreach (var column in columns)
        {
            table.Columns.Add(column.Name, typeof(object));
        }

        foreach (var row in batch.Rows)
        {
            if (row.Length != columns.Count)
            {
                throw new MetaPipelineConfigurationException(
                    $"Buffered row contains {row.Length} values but target write expects {columns.Count}.");
            }

            var values = new object[columns.Count];
            for (var ordinal = 0; ordinal < row.Length; ordinal++)
            {
                values[ordinal] = row[ordinal] ?? DBNull.Value;
            }

            table.Rows.Add(values);
        }

        return table;
    }
}
