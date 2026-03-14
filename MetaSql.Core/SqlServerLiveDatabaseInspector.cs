using Microsoft.Data.SqlClient;

namespace MetaSql.Core;

public sealed class SqlServerLiveDatabaseInspector
{
    public LiveDatabaseSnapshot Inspect(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("preflight requires --connection <connectionString>.");
        }

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        var tables = LoadTables(connection);
        LoadColumns(connection, tables);
        LoadConstraints(connection, tables);
        LoadIndexes(connection, tables);
        return new LiveDatabaseSnapshot(tables);
    }

    private static Dictionary<string, LiveTable> LoadTables(SqlConnection connection)
    {
        const string sql = """
            SELECT
                schemas.name AS SchemaName,
                tables.name AS TableName,
                COALESCE(SUM(partitions.row_count), 0) AS [RowCount]
            FROM sys.tables AS tables
            INNER JOIN sys.schemas AS schemas ON schemas.schema_id = tables.schema_id
            LEFT JOIN sys.dm_db_partition_stats AS partitions
                ON partitions.object_id = tables.object_id
               AND partitions.index_id IN (0, 1)
            WHERE tables.is_ms_shipped = 0
            GROUP BY schemas.name, tables.name
            ORDER BY schemas.name, tables.name;
            """;

        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();
        var tables = new Dictionary<string, LiveTable>(StringComparer.OrdinalIgnoreCase);
        while (reader.Read())
        {
            var table = new LiveTable(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetInt64(2),
                new Dictionary<string, LiveColumn>(StringComparer.OrdinalIgnoreCase),
                new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                new HashSet<string>(StringComparer.OrdinalIgnoreCase));
            tables[table.ObjectKey] = table;
        }

        return tables;
    }

    private static void LoadColumns(SqlConnection connection, IDictionary<string, LiveTable> tables)
    {
        const string sql = """
            SELECT
                schemas.name AS SchemaName,
                tables.name AS TableName,
                columns.name AS ColumnName,
                types.name AS TypeName,
                columns.max_length,
                columns.precision,
                columns.scale,
                columns.is_nullable
            FROM sys.tables AS tables
            INNER JOIN sys.schemas AS schemas ON schemas.schema_id = tables.schema_id
            INNER JOIN sys.columns AS columns ON columns.object_id = tables.object_id
            INNER JOIN sys.types AS types ON types.user_type_id = columns.user_type_id
            WHERE tables.is_ms_shipped = 0
            ORDER BY schemas.name, tables.name, columns.column_id;
            """;

        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var tableKey = SqlObjectName.Format(reader.GetString(0), reader.GetString(1));
            if (!tables.TryGetValue(tableKey, out var table))
            {
                continue;
            }

            var columns = new Dictionary<string, LiveColumn>(table.Columns, StringComparer.OrdinalIgnoreCase)
            {
                [reader.GetString(2)] = new LiveColumn(
                    reader.GetString(2),
                    RenderTypeSql(
                        reader.GetString(3),
                        reader.GetInt16(4),
                        reader.GetByte(5),
                        reader.GetByte(6)),
                    reader.GetBoolean(7))
            };
            tables[tableKey] = table with { Columns = columns };
        }
    }

    private static void LoadConstraints(SqlConnection connection, IDictionary<string, LiveTable> tables)
    {
        const string keyConstraintSql = """
            SELECT
                schemas.name AS SchemaName,
                tables.name AS TableName,
                constraints.name AS ConstraintName
            FROM sys.key_constraints AS constraints
            INNER JOIN sys.tables AS tables ON tables.object_id = constraints.parent_object_id
            INNER JOIN sys.schemas AS schemas ON schemas.schema_id = tables.schema_id
            WHERE tables.is_ms_shipped = 0;
            """;

        LoadConstraintNames(connection, keyConstraintSql, tables);

        const string foreignKeySql = """
            SELECT
                schemas.name AS SchemaName,
                tables.name AS TableName,
                foreign_keys.name AS ConstraintName
            FROM sys.foreign_keys AS foreign_keys
            INNER JOIN sys.tables AS tables ON tables.object_id = foreign_keys.parent_object_id
            INNER JOIN sys.schemas AS schemas ON schemas.schema_id = tables.schema_id
            WHERE tables.is_ms_shipped = 0;
            """;

        LoadConstraintNames(connection, foreignKeySql, tables);
    }

    private static void LoadConstraintNames(SqlConnection connection, string sql, IDictionary<string, LiveTable> tables)
    {
        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var tableKey = SqlObjectName.Format(reader.GetString(0), reader.GetString(1));
            if (!tables.TryGetValue(tableKey, out var table))
            {
                continue;
            }

            var names = new HashSet<string>(table.ConstraintNames, StringComparer.OrdinalIgnoreCase)
            {
                reader.GetString(2)
            };
            tables[tableKey] = table with { ConstraintNames = names };
        }
    }

    private static void LoadIndexes(SqlConnection connection, IDictionary<string, LiveTable> tables)
    {
        const string sql = """
            SELECT
                schemas.name AS SchemaName,
                tables.name AS TableName,
                indexes.name AS IndexName
            FROM sys.indexes AS indexes
            INNER JOIN sys.tables AS tables ON tables.object_id = indexes.object_id
            INNER JOIN sys.schemas AS schemas ON schemas.schema_id = tables.schema_id
            WHERE tables.is_ms_shipped = 0
              AND indexes.is_hypothetical = 0
              AND indexes.is_primary_key = 0
              AND indexes.is_unique_constraint = 0
              AND indexes.name IS NOT NULL;
            """;

        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var tableKey = SqlObjectName.Format(reader.GetString(0), reader.GetString(1));
            if (!tables.TryGetValue(tableKey, out var table))
            {
                continue;
            }

            var names = new HashSet<string>(table.IndexNames, StringComparer.OrdinalIgnoreCase)
            {
                reader.GetString(2)
            };
            tables[tableKey] = table with { IndexNames = names };
        }
    }

    private static string RenderTypeSql(string typeName, short maxLength, byte precision, byte scale)
    {
        var normalizedName = typeName.ToLowerInvariant();
        return normalizedName switch
        {
            "nvarchar" or "nchar" => $"{normalizedName}({RenderLength(maxLength, true)})",
            "varchar" or "char" or "varbinary" or "binary" => $"{normalizedName}({RenderLength(maxLength, false)})",
            "decimal" or "numeric" => $"{normalizedName}({precision},{scale})",
            "datetime2" or "datetimeoffset" or "time" => $"{normalizedName}({scale})",
            _ => normalizedName
        };
    }

    private static string RenderLength(short maxLength, bool isUnicode)
    {
        if (maxLength == -1)
        {
            return "max";
        }

        return isUnicode ? (maxLength / 2).ToString() : maxLength.ToString();
    }
}
