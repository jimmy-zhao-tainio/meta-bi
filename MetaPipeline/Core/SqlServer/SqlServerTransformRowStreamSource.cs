using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;

namespace MetaPipeline;

public sealed class SqlServerTransformRowStreamSource : IPipelineRowStreamSource
{
    private readonly string connectionString;
    private readonly string sql;
    private readonly int batchSize;

    public SqlServerTransformRowStreamSource(
        string connectionString,
        string sql,
        PipelineRowStreamShape shape,
        int batchSize)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);
        ArgumentNullException.ThrowIfNull(shape);

        if (batchSize <= 0)
        {
            throw new MetaPipelineConfigurationException(
                "Batch size must be greater than zero.");
        }

        this.connectionString = connectionString;
        this.sql = sql;
        this.batchSize = batchSize;
        Shape = shape;
    }

    public PipelineRowStreamShape Shape { get; }

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
                yield return new PipelineDataBatch(Shape, rows.ToArray());
                rows = new List<object?[]>(batchSize);
            }
        }

        if (rows.Count > 0)
        {
            yield return new PipelineDataBatch(Shape, rows.ToArray());
        }
    }

    private void ValidateReaderShape(SqlDataReader reader)
    {
        if (reader.FieldCount != Shape.ColumnCount)
        {
            throw new MetaPipelineConfigurationException(
                $"Source result column count {reader.FieldCount} does not match bound target column count {Shape.ColumnCount}.");
        }

        for (var ordinal = 0; ordinal < Shape.ColumnCount; ordinal++)
        {
            var expected = Shape.Columns[ordinal];
            var actual = reader.GetName(ordinal);
            if (!string.Equals(actual, expected.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new MetaPipelineConfigurationException(
                    $"Source result column {ordinal + 1} is '{actual}' but binding expects '{expected.Name}'.");
            }
        }
    }
}
