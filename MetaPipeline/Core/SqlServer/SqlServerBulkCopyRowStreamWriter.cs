using Microsoft.Data.SqlClient;
using System.Data;

namespace MetaPipeline;

public sealed class SqlServerBulkCopyRowStreamWriter : IPipelineRowStreamWriter, IAsyncDisposable
{
    private readonly string connectionString;
    private readonly string destinationTableName;
    private SqlConnection? connection;
    private bool disposed;

    public SqlServerBulkCopyRowStreamWriter(
        string connectionString,
        string targetSqlIdentifier,
        PipelineRowStreamShape shape)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetSqlIdentifier);
        ArgumentNullException.ThrowIfNull(shape);

        this.connectionString = connectionString;
        Shape = shape;
        destinationTableName = SqlServerMultipartIdentifier.Parse(targetSqlIdentifier).RenderBracketQuoted();
    }

    public PipelineRowStreamShape Shape { get; }

    public async Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        ArgumentNullException.ThrowIfNull(batch);
        Shape.EnsureCompatibleWith(batch.Shape, "batch shape");

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

        foreach (var column in Shape.Columns)
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
        foreach (var column in Shape.Columns)
        {
            table.Columns.Add(column.Name, typeof(object));
        }

        foreach (var row in batch.Rows)
        {
            var values = new object[Shape.ColumnCount];
            for (var ordinal = 0; ordinal < row.Length; ordinal++)
            {
                values[ordinal] = row[ordinal] ?? DBNull.Value;
            }

            table.Rows.Add(values);
        }

        return table;
    }
}
