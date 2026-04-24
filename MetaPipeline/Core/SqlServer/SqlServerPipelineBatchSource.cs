using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;

namespace MetaPipeline;

public sealed class SqlServerPipelineBatchSource : IPipelineBatchSource
{
    private readonly string connectionString;
    private readonly string sql;
    private readonly int batchSize;

    public SqlServerPipelineBatchSource(
        string connectionString,
        string sql,
        IReadOnlyList<PipelineColumn> columns,
        int batchSize)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);
        ArgumentNullException.ThrowIfNull(columns);

        if (columns.Count == 0)
        {
            throw new MetaPipelineConfigurationException(
                "Source reader requires at least one bound output column.");
        }

        if (batchSize <= 0)
        {
            throw new MetaPipelineConfigurationException(
                "Batch size must be greater than zero.");
        }

        this.connectionString = connectionString;
        this.sql = sql;
        this.batchSize = batchSize;
        Columns = columns;
    }

    public IReadOnlyList<PipelineColumn> Columns { get; }

    public async IAsyncEnumerable<PipelineDataBatch> ReadBatchesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandTimeout = 0;

        await using var reader = await command.ExecuteReaderAsync(
            System.Data.CommandBehavior.SequentialAccess,
            cancellationToken).ConfigureAwait(false);

        ValidateReaderShape(reader);

        var rows = new List<object?[]>(batchSize);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var values = new object?[reader.FieldCount];
            reader.GetValues(values);
            for (var ordinal = 0; ordinal < values.Length; ordinal++)
            {
                if (values[ordinal] is DBNull)
                {
                    values[ordinal] = null;
                }
            }

            rows.Add(values);
            if (rows.Count >= batchSize)
            {
                yield return new PipelineDataBatch(rows.ToArray());
                rows = new List<object?[]>(batchSize);
            }
        }

        if (rows.Count > 0)
        {
            yield return new PipelineDataBatch(rows.ToArray());
        }
    }

    private void ValidateReaderShape(SqlDataReader reader)
    {
        if (reader.FieldCount != Columns.Count)
        {
            throw new MetaPipelineConfigurationException(
                $"Source result column count {reader.FieldCount} does not match bound target column count {Columns.Count}.");
        }

        for (var ordinal = 0; ordinal < Columns.Count; ordinal++)
        {
            var expected = Columns[ordinal];
            var actual = reader.GetName(ordinal);
            if (!string.Equals(actual, expected.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new MetaPipelineConfigurationException(
                    $"Source result column {ordinal + 1} is '{actual}' but binding expects '{expected.Name}'.");
            }
        }
    }
}
