using Microsoft.Data.SqlClient;
using MetaDataTypeConversion;
using MetaDataTypeConversion.Core;
using System.Data;
using System.Globalization;

namespace MetaPipeline;

public sealed class SqlServerBulkCopyRowStreamWriter : IPipelineRowStreamWriter, IAsyncDisposable
{
    private readonly string connectionString;
    private readonly string destinationTableName;
    private readonly MetaPipelineExecutionContext? executionContext;
    private readonly string targetDataTypeSystemName;
    private readonly MetaDataTypeConversionModel dataTypeConversionWorkspace;
    private readonly IMetaDataTypeConversionService dataTypeConversionService = new MetaDataTypeConversionService();
    private readonly int? timeoutSeconds;
    private SqlConnection? connection;
    private bool disposed;

    public SqlServerBulkCopyRowStreamWriter(
        string connectionString,
        string targetSqlIdentifier,
        PipelineRowStreamShape shape,
        MetaPipelineExecutionContext? executionContext = null,
        string targetDataTypeSystemName = "SqlServer",
        string? dataTypeConversionWorkspacePath = null,
        int? timeoutSeconds = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetSqlIdentifier);
        ArgumentNullException.ThrowIfNull(shape);

        this.connectionString = connectionString;
        this.executionContext = executionContext;
        this.targetDataTypeSystemName = string.IsNullOrWhiteSpace(targetDataTypeSystemName)
            ? "SqlServer"
            : targetDataTypeSystemName.Trim();
        this.timeoutSeconds = timeoutSeconds;
        dataTypeConversionWorkspace = MetaDataTypeConversionWorkspaceProvider.LoadOrDefault(dataTypeConversionWorkspacePath);
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
        var columnTypes = Shape.Columns
            .Select(ResolveColumnClrType)
            .ToArray();
        var table = new DataTable
        {
            Locale = CultureInfo.InvariantCulture,
        };
        for (var index = 0; index < Shape.Columns.Count; index++)
        {
            table.Columns.Add(Shape.Columns[index].Name, columnTypes[index]);
        }

        foreach (var row in batch.Rows)
        {
            var values = new object[Shape.ColumnCount];
            for (var ordinal = 0; ordinal < row.Length; ordinal++)
            {
                values[ordinal] = CoerceValue(row[ordinal], columnTypes[ordinal], Shape.Columns[ordinal]);
            }

            table.Rows.Add(values);
        }

        return table;
    }

    private Type ResolveColumnClrType(PipelineColumn column)
    {
        if (string.IsNullOrWhiteSpace(column.TargetMetaDataTypeId))
        {
            return typeof(object);
        }

        var targetDataTypeId = ResolveRuntimeTargetDataTypeId(column);
        return ResolveSqlServerClrType(targetDataTypeId, column);
    }

    private string ResolveRuntimeTargetDataTypeId(PipelineColumn column)
    {
        var modeledTargetDataTypeId = column.TargetMetaDataTypeId?.Trim() ?? string.Empty;
        if (MetaDataTypeConversionService.BelongsToDataTypeSystem(modeledTargetDataTypeId, targetDataTypeSystemName))
        {
            return modeledTargetDataTypeId;
        }

        try
        {
            return dataTypeConversionService
                .Resolve(dataTypeConversionWorkspace, modeledTargetDataTypeId, targetDataTypeSystemName)
                .TargetDataTypeId;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            throw new MetaPipelineConfigurationException(
                $"Column '{column.Name}' declares target MetaDataTypeId '{modeledTargetDataTypeId}', but no sanctioned conversion resolves it to target data type system '{targetDataTypeSystemName}'. {ex.Message}");
        }
    }

    private static Type ResolveSqlServerClrType(string targetDataTypeId, PipelineColumn column)
    {
        if (!MetaDataTypeConversionService.BelongsToDataTypeSystem(targetDataTypeId, "SqlServer"))
        {
            throw new MetaPipelineConfigurationException(
                $"Column '{column.Name}' resolved to target data type '{targetDataTypeId}', but SQL Server InsertRows requires a SqlServer data type.");
        }

        var typeName = ExtractTypeName(targetDataTypeId);
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
                $"Column '{column.Name}' resolved to unsupported SQL Server data type '{targetDataTypeId}'."),
        };
    }

    private static object CoerceValue(object? value, Type targetType, PipelineColumn column)
    {
        if (value is null || value is DBNull)
        {
            return DBNull.Value;
        }

        if (targetType == typeof(object) || targetType.IsInstanceOfType(value))
        {
            return value;
        }

        try
        {
            if (targetType == typeof(string))
            {
                var text = Convert.ToString(value, CultureInfo.InvariantCulture);
                return text ?? (object)DBNull.Value;
            }

            if (targetType == typeof(Guid))
            {
                return value is string text
                    ? Guid.Parse(text)
                    : (Guid)value;
            }

            if (targetType == typeof(DateTime))
            {
                return value is string text
                    ? DateTime.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                    : Convert.ToDateTime(value, CultureInfo.InvariantCulture);
            }

            if (targetType == typeof(DateTimeOffset))
            {
                return value switch
                {
                    string text => DateTimeOffset.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    DateTime dateTime => new DateTimeOffset(dateTime),
                    _ => (DateTimeOffset)value,
                };
            }

            if (targetType == typeof(TimeSpan))
            {
                return value is string text
                    ? TimeSpan.Parse(text, CultureInfo.InvariantCulture)
                    : (TimeSpan)value;
            }

            if (targetType == typeof(byte[]))
            {
                return value is byte[] bytes
                    ? bytes
                    : throw new InvalidCastException($"Value type '{value.GetType().FullName}' cannot be coerced to byte[].");
            }

            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
        {
            throw new MetaPipelineConfigurationException(
                $"Column '{column.Name}' value of CLR type '{value.GetType().FullName}' could not be coerced to '{targetType.FullName}' for SQL Server bulk copy. {ex.Message}");
        }
    }

    private static string ExtractTypeName(string dataTypeId)
    {
        const string marker = ":type:";
        var markerIndex = dataTypeId.IndexOf(marker, StringComparison.Ordinal);
        if (markerIndex < 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Data type id '{dataTypeId}' does not use the expected '<system>:type:<name>' shape.");
        }

        return dataTypeId[(markerIndex + marker.Length)..];
    }
}
