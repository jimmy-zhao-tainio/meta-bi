using Microsoft.Data.SqlClient;
using MetaDataType;
using Meta.Integration;
using MetaSchema;
using System.Data;
using System.Globalization;

namespace MetaPipeline;

public sealed class SqlServerBulkCopyRowStreamWriter : IPipelineRowStreamWriter, IAsyncDisposable
{
    private readonly string connectionString;
    private readonly string destinationTableName;
    private readonly MetaPipelineExecutionContext? executionContext;
    private readonly Type[] columnTypes;
    private readonly int? timeoutSeconds;
    private SqlConnection? connection;
    private bool disposed;

    public SqlServerBulkCopyRowStreamWriter(
        string connectionString,
        string targetSqlIdentifier,
        PipelineRowStreamShape shape,
        MetaPipelineExecutionContext? executionContext = null,
        string targetDataTypeSystemName = "SqlServer",
        string? dataTypeWorkspacePath = null,
        int? timeoutSeconds = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetSqlIdentifier);
        ArgumentNullException.ThrowIfNull(shape);

        this.connectionString = connectionString;
        this.executionContext = executionContext;
        if (!string.Equals(targetDataTypeSystemName, "SqlServer", StringComparison.OrdinalIgnoreCase))
            throw new MetaPipelineConfigurationException("SQL Server bulk copy requires the SqlServer type system.");
        this.timeoutSeconds = timeoutSeconds;
        Shape = shape;
        var catalog = string.IsNullOrWhiteSpace(dataTypeWorkspacePath) ? MetaDataTypeInstance.BuiltIn
            : TypedWorkspaceModelMapper.Load<MetaDataTypeModel>(Path.GetFullPath(dataTypeWorkspacePath), searchUpward: false);
        var validator = new SqlServerSchemaTypeValidator(catalog);
        columnTypes = Shape.Columns.Select(column => ResolveColumnClrType(column, validator)).ToArray();
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

        using var table = CreateDataTable(batch);
        var currentConnection = await EnsureConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = await currentConnection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        using var bulkCopy = new SqlBulkCopy(
            currentConnection,
            SqlBulkCopyOptions.Default,
            (SqlTransaction)transaction)
        {
            DestinationTableName = destinationTableName,
            BatchSize = batch.RowCount,
            BulkCopyTimeout = timeoutSeconds is null ? 0 : timeoutSeconds.Value,
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
        await SqlServerSessionContext.ApplyAsync(connection, executionContext, cancellationToken).ConfigureAwait(false);
        return connection;
    }

    private DataTable CreateDataTable(PipelineDataBatch batch)
    {
        // Validate the entire batch before allocating the transport table or opening a connection.
        foreach (var row in batch.Rows)
            for (var ordinal = 0; ordinal < row.Length; ordinal++)
                if (row[ordinal] is { } value && value is not DBNull && !columnTypes[ordinal].IsInstanceOfType(value))
                    throw new MetaPipelineConfigurationException(
                        $"Column '{Shape.Columns[ordinal].Name}' supplies CLR type '{value.GetType().FullName}', but its validated SQL Server contract requires '{columnTypes[ordinal].FullName}'. Apply value conversions in the TransformScript before execution.");

        var table = new DataTable { Locale = CultureInfo.InvariantCulture };
        for (var index = 0; index < Shape.Columns.Count; index++)
            table.Columns.Add(Shape.Columns[index].Name, columnTypes[index]);
        foreach (var row in batch.Rows)
            table.Rows.Add(row.Select(value => value ?? DBNull.Value).ToArray());
        return table;
    }

    private static Type ResolveColumnClrType(PipelineColumn column, SqlServerSchemaTypeValidator validator)
    {
        try
        {
            var target = validator.Resolve(column.TargetMetaDataTypeId, $"Pipeline column '{column.Name}'");
            if (column.SourceMetaDataTypeId is { } sourceId && !string.Equals(sourceId, target.Id, StringComparison.Ordinal))
                throw new MetaPipelineConfigurationException($"Column '{column.Name}' must carry matching resulting expression and target type IDs.");
            return ResolveSqlServerClrType(target.Name, column);
        }
        catch (SchemaTypeValidationException ex)
        {
            throw new MetaPipelineConfigurationException(ex.Message);
        }
    }

    // SQL Server driver representation, never a value-conversion policy.
    private static Type ResolveSqlServerClrType(string typeName, PipelineColumn column)
    {
        return typeName.ToLowerInvariant() switch
        {
            "bigint" => typeof(long),
            "binary" => typeof(byte[]),
            "bit" => typeof(bool),
            "char" => typeof(string),
            "date" => typeof(DateTime),
            "datetime" => typeof(DateTime),
            "datetime2" => typeof(DateTime),
            "datetimeoffset" => typeof(DateTimeOffset),
            "decimal" => typeof(decimal),
            "float" => typeof(double),
            "geography" => typeof(object),
            "geometry" => typeof(object),
            "hierarchyid" => typeof(object),
            "int" => typeof(int),
            "money" => typeof(decimal),
            "nchar" => typeof(string),
            "numeric" => typeof(decimal),
            "nvarchar" => typeof(string),
            "real" => typeof(float),
            "rowversion" => typeof(byte[]),
            "smallint" => typeof(short),
            "smallmoney" => typeof(decimal),
            "sql_variant" => typeof(object),
            "time" => typeof(TimeSpan),
            "timestamp" => typeof(byte[]),
            "tinyint" => typeof(byte),
            "uniqueidentifier" => typeof(Guid),
            "varbinary" => typeof(byte[]),
            "varchar" => typeof(string),
            "xml" => typeof(string),
            _ => throw new MetaPipelineConfigurationException(
                $"Column '{column.Name}' resolved to unsupported SQL Server data type '{typeName}'."),
        };
    }

}
