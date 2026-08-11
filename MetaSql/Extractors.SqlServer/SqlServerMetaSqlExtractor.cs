using System.Data;
using Microsoft.Data.SqlClient;
using Meta.Operations.Domain;
using Meta.Integration;

namespace MetaSql.Extractors.SqlServer;

public sealed class SqlServerMetaSqlExtractor
{
    public InMemoryWorkspace ExtractMetaSqlWorkspace(SqlServerExtractRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var model = ExtractMetaSqlModel(request);
        return TypedWorkspaceModelMapper.ToInMemoryWorkspace(model);
    }

    public MetaSqlModel ExtractMetaSqlModel(SqlServerExtractRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.ConnectionString))
        {
            throw new InvalidOperationException("extract sqlserver requires a connection string.");
        }

        var schemaFilter = request.SchemaName?.Trim();
        var tableFilter = request.TableName?.Trim();
        var objectKinds = request.ObjectKinds == SqlServerExtractObjectKinds.None
            ? SqlServerExtractObjectKinds.All
            : request.ObjectKinds;
        if (tableFilter is not null && objectKinds == SqlServerExtractObjectKinds.All)
        {
            objectKinds = SqlServerExtractObjectKinds.Tables;
        }
        else if (tableFilter is not null && objectKinds != SqlServerExtractObjectKinds.Tables)
        {
            throw new InvalidOperationException(
                "extract sqlserver --table can only be used when extracting tables. Use --include-tables or omit object-kind switches.");
        }

        using var connection = new SqlConnection(request.ConnectionString);
        connection.Open();

        var databaseName = string.IsNullOrWhiteSpace(connection.Database)
            ? "(default)"
            : connection.Database;

        var tableRows = objectKinds.HasFlag(SqlServerExtractObjectKinds.Tables)
            ? LoadTables(connection, schemaFilter, tableFilter)
                .OrderBy(row => row.SchemaName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.SchemaName, StringComparer.Ordinal)
                .ThenBy(row => row.TableName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.TableName, StringComparer.Ordinal)
                .ToList()
            : new List<SqlServerMetaSqlProjector.TableRow>();
        var viewRows = objectKinds.HasFlag(SqlServerExtractObjectKinds.Views)
            ? LoadViews(connection, schemaFilter)
                .OrderBy(row => row.SchemaName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.SchemaName, StringComparer.Ordinal)
                .ThenBy(row => row.ViewName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.ViewName, StringComparer.Ordinal)
                .ToList()
            : new List<SqlServerMetaSqlProjector.ViewRow>();
        var functionRows = objectKinds.HasFlag(SqlServerExtractObjectKinds.Functions)
            ? LoadFunctions(connection, schemaFilter)
                .OrderBy(row => row.SchemaName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.SchemaName, StringComparer.Ordinal)
                .ThenBy(row => row.FunctionName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.FunctionName, StringComparer.Ordinal)
                .ToList()
            : new List<SqlServerMetaSqlProjector.FunctionRow>();
        var storedProcedureRows = objectKinds.HasFlag(SqlServerExtractObjectKinds.StoredProcedures)
            ? LoadStoredProcedures(connection, schemaFilter)
                .OrderBy(row => row.SchemaName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.SchemaName, StringComparer.Ordinal)
                .ThenBy(row => row.ProcedureName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.ProcedureName, StringComparer.Ordinal)
                .ToList()
            : new List<SqlServerMetaSqlProjector.StoredProcedureRow>();

        if (tableRows.Count == 0 && viewRows.Count == 0 && functionRows.Count == 0 && storedProcedureRows.Count == 0)
        {
            if (request.AllowEmpty)
            {
                var schemaRows = LoadSchemas(connection, schemaFilter)
                    .OrderBy(row => row, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(row => row, StringComparer.Ordinal)
                    .ToList();
                return SqlServerMetaSqlWorkspaceFactory.CreateEmptyModel(
                    databaseName,
                    schemaRows);
            }

            var filterDescription = (schemaFilter, tableFilter) switch
            {
                (null, null) => "any schema/table filter",
                (not null, null) => $"schema '{schemaFilter}'",
                (null, not null) => $"table '{tableFilter}'",
                (not null, not null) => $"table '{schemaFilter}.{tableFilter}'",
            };

            throw new InvalidOperationException(
                $"No SQL Server tables or SQL modules matched {filterDescription} in database '{databaseName}'.");
        }

        var columnsByTableKey = tableRows.ToDictionary(
            row => SqlServerMetaSqlProjector.BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadColumns(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);

        var primaryKeysByTableKey = tableRows.ToDictionary(
            row => SqlServerMetaSqlProjector.BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadPrimaryKeys(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);

        var primaryKeyColumnsByTableKey = tableRows.ToDictionary(
            row => SqlServerMetaSqlProjector.BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadPrimaryKeyColumns(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);

        var foreignKeysByTableKey = tableRows.ToDictionary(
            row => SqlServerMetaSqlProjector.BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadForeignKeys(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);

        var foreignKeyColumnsByTableKey = tableRows.ToDictionary(
            row => SqlServerMetaSqlProjector.BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadForeignKeyColumns(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);

        var indexesByTableKey = tableRows.ToDictionary(
            row => SqlServerMetaSqlProjector.BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadIndexes(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);

        var indexColumnsByTableKey = tableRows.ToDictionary(
            row => SqlServerMetaSqlProjector.BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadIndexColumns(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);

        return SqlServerMetaSqlProjector.Project(
            databaseName,
            tableRows,
            columnsByTableKey,
            primaryKeysByTableKey,
            primaryKeyColumnsByTableKey,
            foreignKeysByTableKey,
            foreignKeyColumnsByTableKey,
            indexesByTableKey,
            indexColumnsByTableKey,
            viewRows,
            functionRows,
            storedProcedureRows);
    }

    private static List<string> LoadSchemas(SqlConnection connection, string? schemaName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                s.name
            from sys.schemas s
            where (@schemaName is null or s.name = @schemaName)
            order by s.name
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = string.IsNullOrWhiteSpace(schemaName) ? DBNull.Value : schemaName });

        var rows = new List<string>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(reader.GetString(0));
        }

        return rows;
    }

    private static List<SqlServerMetaSqlProjector.TableRow> LoadTables(SqlConnection connection, string? schemaName, string? tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                s.name as SchemaName,
                t.name as TableName
            from sys.tables t
            join sys.schemas s on s.schema_id = t.schema_id
            where t.is_ms_shipped = 0
              and (@schemaName is null or s.name = @schemaName)
              and (@tableName is null or t.name = @tableName)
            order by s.name, t.name
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = string.IsNullOrWhiteSpace(schemaName) ? DBNull.Value : schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = string.IsNullOrWhiteSpace(tableName) ? DBNull.Value : tableName });

        var rows = new List<SqlServerMetaSqlProjector.TableRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new SqlServerMetaSqlProjector.TableRow(
                SchemaName: reader.GetString(0),
                TableName: reader.GetString(1)));
        }

        return rows;
    }

    private static List<SqlServerMetaSqlProjector.ColumnRow> LoadColumns(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                s.name as SchemaName,
                t.name as TableName,
                c.name as ColumnName,
                c.column_id as OrdinalPosition,
                c.is_nullable,
                ty.name as DataTypeName,
                case
                    when c.max_length = -1 then -1
                    when ty.name in ('nchar', 'nvarchar') then c.max_length / 2
                    else c.max_length
                end as LengthValue,
                case
                    when ty.name in ('decimal', 'numeric') then convert(int, c.precision)
                    when ty.name in ('time', 'datetime2', 'datetimeoffset') then convert(int, c.scale)
                    else null
                end as PrecisionValue,
                case
                    when ty.name in ('decimal', 'numeric') then convert(int, c.scale)
                    else null
                end as ScaleValue,
                c.is_identity,
                convert(nvarchar(50), ic.seed_value) as IdentitySeed,
                convert(nvarchar(50), ic.increment_value) as IdentityIncrement,
                cc.definition as ExpressionSql,
                dc.definition as DefaultExpressionSql
            from sys.tables t
            join sys.schemas s on s.schema_id = t.schema_id
            join sys.columns c on c.object_id = t.object_id
            join sys.types ty on ty.user_type_id = c.user_type_id
            left join sys.identity_columns ic on ic.object_id = c.object_id and ic.column_id = c.column_id
            left join sys.computed_columns cc on cc.object_id = c.object_id and cc.column_id = c.column_id
            left join sys.default_constraints dc on dc.parent_object_id = c.object_id and dc.parent_column_id = c.column_id
            where s.name = @schemaName
              and t.name = @tableName
            order by c.column_id
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<SqlServerMetaSqlProjector.ColumnRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new SqlServerMetaSqlProjector.ColumnRow(
                SchemaName: reader.GetString(0),
                TableName: reader.GetString(1),
                ColumnName: reader.GetString(2),
                OrdinalPosition: ReadInt32(reader, 3),
                IsNullable: ReadBool(reader, 4),
                DataTypeName: reader.GetString(5),
                Length: ReadNullableInt(reader, 6),
                Precision: ReadNullableInt(reader, 7),
                Scale: ReadNullableInt(reader, 8),
                IsIdentity: ReadBool(reader, 9),
                IdentitySeed: ReadNullableString(reader, 10),
                IdentityIncrement: ReadNullableString(reader, 11),
                ExpressionSql: ReadNullableString(reader, 12),
                DefaultExpressionSql: ReadNullableString(reader, 13)));
        }

        return rows;
    }

    private static List<SqlServerMetaSqlProjector.PrimaryKeyRow> LoadPrimaryKeys(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                kc.name as KeyName,
                case when i.type = 1 then 1 else 0 end as IsClustered
            from sys.key_constraints kc
            join sys.tables t on t.object_id = kc.parent_object_id
            join sys.schemas s on s.schema_id = t.schema_id
            join sys.indexes i on i.object_id = kc.parent_object_id and i.index_id = kc.unique_index_id
            where s.name = @schemaName
              and t.name = @tableName
              and kc.type = 'PK'
            order by kc.name
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<SqlServerMetaSqlProjector.PrimaryKeyRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new SqlServerMetaSqlProjector.PrimaryKeyRow(
                Name: reader.GetString(0),
                IsClustered: ReadBool(reader, 1)));
        }

        return rows;
    }

    private static List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow> LoadPrimaryKeyColumns(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                kc.name as KeyName,
                ic.key_ordinal as Ordinal,
                c.name as ColumnName,
                ic.is_descending_key
            from sys.key_constraints kc
            join sys.tables t on t.object_id = kc.parent_object_id
            join sys.schemas s on s.schema_id = t.schema_id
            join sys.index_columns ic on ic.object_id = kc.parent_object_id and ic.index_id = kc.unique_index_id
            join sys.columns c on c.object_id = ic.object_id and c.column_id = ic.column_id
            where s.name = @schemaName
              and t.name = @tableName
              and kc.type = 'PK'
              and ic.key_ordinal > 0
            order by kc.name, ic.key_ordinal
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new SqlServerMetaSqlProjector.PrimaryKeyColumnRow(
                KeyName: reader.GetString(0),
                Ordinal: ReadInt32(reader, 1),
                ColumnName: reader.GetString(2),
                IsDescending: ReadBool(reader, 3)));
        }

        return rows;
    }

    private static List<SqlServerMetaSqlProjector.ForeignKeyRow> LoadForeignKeys(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                fk.name as ForeignKeyName,
                refSchema.name as TargetSchemaName,
                refTable.name as TargetTableName
            from sys.foreign_keys fk
            join sys.tables srcTable on srcTable.object_id = fk.parent_object_id
            join sys.schemas srcSchema on srcSchema.schema_id = srcTable.schema_id
            join sys.tables refTable on refTable.object_id = fk.referenced_object_id
            join sys.schemas refSchema on refSchema.schema_id = refTable.schema_id
            where srcSchema.name = @schemaName
              and srcTable.name = @tableName
            order by fk.name
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<SqlServerMetaSqlProjector.ForeignKeyRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new SqlServerMetaSqlProjector.ForeignKeyRow(
                Name: reader.GetString(0),
                TargetSchemaName: reader.GetString(1),
                TargetTableName: reader.GetString(2)));
        }

        return rows;
    }

    private static List<SqlServerMetaSqlProjector.ForeignKeyColumnRow> LoadForeignKeyColumns(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                fk.name as ForeignKeyName,
                fkc.constraint_column_id as Ordinal,
                srcColumn.name as SourceColumnName,
                refColumn.name as TargetColumnName
            from sys.foreign_keys fk
            join sys.tables srcTable on srcTable.object_id = fk.parent_object_id
            join sys.schemas srcSchema on srcSchema.schema_id = srcTable.schema_id
            join sys.foreign_key_columns fkc on fkc.constraint_object_id = fk.object_id
            join sys.columns srcColumn on srcColumn.object_id = fkc.parent_object_id and srcColumn.column_id = fkc.parent_column_id
            join sys.columns refColumn on refColumn.object_id = fkc.referenced_object_id and refColumn.column_id = fkc.referenced_column_id
            where srcSchema.name = @schemaName
              and srcTable.name = @tableName
            order by fk.name, fkc.constraint_column_id
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new SqlServerMetaSqlProjector.ForeignKeyColumnRow(
                ForeignKeyName: reader.GetString(0),
                Ordinal: ReadInt32(reader, 1),
                SourceColumnName: reader.GetString(2),
                TargetColumnName: reader.GetString(3)));
        }

        return rows;
    }

    private static List<SqlServerMetaSqlProjector.IndexRow> LoadIndexes(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                i.name as IndexName,
                i.is_unique,
                case when i.type = 1 then 1 else 0 end as IsClustered,
                i.filter_definition
            from sys.indexes i
            join sys.tables t on t.object_id = i.object_id
            join sys.schemas s on s.schema_id = t.schema_id
            where s.name = @schemaName
              and t.name = @tableName
              and i.index_id > 0
              and i.is_primary_key = 0
              and i.is_unique_constraint = 0
              and i.is_hypothetical = 0
              and i.type in (1, 2)
            order by i.name
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<SqlServerMetaSqlProjector.IndexRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new SqlServerMetaSqlProjector.IndexRow(
                Name: reader.GetString(0),
                IsUnique: ReadBool(reader, 1),
                IsClustered: ReadBool(reader, 2),
                FilterSql: NormalizeFilterSql(ReadNullableString(reader, 3))));
        }

        return rows;
    }

    private static List<SqlServerMetaSqlProjector.IndexColumnRow> LoadIndexColumns(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                i.name as IndexName,
                ic.index_column_id as Ordinal,
                c.name as ColumnName,
                ic.is_descending_key,
                ic.is_included_column
            from sys.indexes i
            join sys.tables t on t.object_id = i.object_id
            join sys.schemas s on s.schema_id = t.schema_id
            join sys.index_columns ic on ic.object_id = i.object_id and ic.index_id = i.index_id
            join sys.columns c on c.object_id = ic.object_id and c.column_id = ic.column_id
            where s.name = @schemaName
              and t.name = @tableName
              and i.index_id > 0
              and i.is_primary_key = 0
              and i.is_unique_constraint = 0
              and i.is_hypothetical = 0
              and i.type in (1, 2)
            order by i.name, ic.index_column_id
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<SqlServerMetaSqlProjector.IndexColumnRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new SqlServerMetaSqlProjector.IndexColumnRow(
                IndexName: reader.GetString(0),
                Ordinal: ReadInt32(reader, 1),
                ColumnName: reader.GetString(2),
                IsDescending: ReadBool(reader, 3),
                IsIncluded: ReadBool(reader, 4)));
        }

        return rows;
    }

    private static List<SqlServerMetaSqlProjector.ViewRow> LoadViews(SqlConnection connection, string? schemaName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                s.name as SchemaName,
                v.name as ViewName,
                sm.definition as DefinitionSql
            from sys.views v
            join sys.schemas s on s.schema_id = v.schema_id
            join sys.sql_modules sm on sm.object_id = v.object_id
            where v.is_ms_shipped = 0
              and (@schemaName is null or s.name = @schemaName)
            order by s.name, v.name
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = string.IsNullOrWhiteSpace(schemaName) ? DBNull.Value : schemaName });

        var rows = new List<SqlServerMetaSqlProjector.ViewRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new SqlServerMetaSqlProjector.ViewRow(
                SchemaName: reader.GetString(0),
                ViewName: reader.GetString(1),
                DefinitionSql: reader.GetString(2)));
        }

        return rows;
    }

    private static List<SqlServerMetaSqlProjector.StoredProcedureRow> LoadStoredProcedures(SqlConnection connection, string? schemaName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                s.name as SchemaName,
                p.name as ProcedureName,
                sm.definition as DefinitionSql
            from sys.procedures p
            join sys.schemas s on s.schema_id = p.schema_id
            join sys.sql_modules sm on sm.object_id = p.object_id
            where p.is_ms_shipped = 0
              and (@schemaName is null or s.name = @schemaName)
            order by s.name, p.name
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = string.IsNullOrWhiteSpace(schemaName) ? DBNull.Value : schemaName });

        var rows = new List<SqlServerMetaSqlProjector.StoredProcedureRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new SqlServerMetaSqlProjector.StoredProcedureRow(
                SchemaName: reader.GetString(0),
                ProcedureName: reader.GetString(1),
                DefinitionSql: reader.GetString(2)));
        }

        return rows;
    }

    private static List<SqlServerMetaSqlProjector.FunctionRow> LoadFunctions(SqlConnection connection, string? schemaName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                s.name as SchemaName,
                o.name as FunctionName,
                o.type as FunctionType,
                sm.definition as DefinitionSql
            from sys.objects o
            join sys.schemas s on s.schema_id = o.schema_id
            join sys.sql_modules sm on sm.object_id = o.object_id
            where o.is_ms_shipped = 0
              and o.type in ('FN', 'IF', 'TF')
              and (@schemaName is null or s.name = @schemaName)
            order by s.name, o.name
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = string.IsNullOrWhiteSpace(schemaName) ? DBNull.Value : schemaName });

        var rows = new List<SqlServerMetaSqlProjector.FunctionRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new SqlServerMetaSqlProjector.FunctionRow(
                SchemaName: reader.GetString(0),
                FunctionName: reader.GetString(1),
                FunctionKind: NormalizeFunctionKind(reader.GetString(2)),
                DefinitionSql: reader.GetString(3)));
        }

        return rows;
    }

    private static int? ReadNullableInt(SqlDataReader reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal))
        {
            return null;
        }

        var value = reader.GetValue(ordinal);
        return value switch
        {
            byte byteValue => byteValue,
            short shortValue => shortValue,
            int intValue => intValue,
            long longValue => checked((int)longValue),
            decimal decimalValue => decimal.ToInt32(decimalValue),
            _ => Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture),
        };
    }

    private static int ReadInt32(SqlDataReader reader, int ordinal)
    {
        var value = ReadNullableInt(reader, ordinal);
        if (!value.HasValue)
        {
            throw new InvalidOperationException($"Expected non-null integer at ordinal {ordinal}.");
        }

        return value.Value;
    }

    private static bool ReadBool(SqlDataReader reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal))
        {
            return false;
        }

        var value = reader.GetValue(ordinal);
        return value switch
        {
            bool boolValue => boolValue,
            byte byteValue => byteValue != 0,
            short shortValue => shortValue != 0,
            int intValue => intValue != 0,
            _ => Convert.ToBoolean(value, System.Globalization.CultureInfo.InvariantCulture),
        };
    }

    private static string ReadNullableString(SqlDataReader reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal))
        {
            return string.Empty;
        }

        return reader.GetString(ordinal);
    }

    private static string NormalizeFilterSql(string value)
    {
        var normalized = value.Trim();
        while (IsWrappedInSingleOuterParentheses(normalized))
        {
            normalized = normalized[1..^1].Trim();
        }

        return normalized;
    }

    private static bool IsWrappedInSingleOuterParentheses(string value)
    {
        if (value.Length < 2 || value[0] != '(' || value[^1] != ')')
        {
            return false;
        }

        var depth = 0;
        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];
            if (ch == '(')
            {
                depth++;
            }
            else if (ch == ')')
            {
                depth--;
                if (depth == 0 && i < value.Length - 1)
                {
                    return false;
                }
            }
        }

        return depth == 0;
    }

    private static string NormalizeFunctionKind(string objectType)
    {
        return objectType switch
        {
            "FN" => "ScalarFunction",
            "IF" => "InlineTableValuedFunction",
            "TF" => "TableValuedFunction",
            _ => objectType,
        };
    }
}

public sealed class SqlServerExtractRequest
{
    public string ConnectionString { get; set; } = string.Empty;
    public string? SchemaName { get; set; }
    public string? TableName { get; set; }
    public SqlServerExtractObjectKinds ObjectKinds { get; set; } = SqlServerExtractObjectKinds.All;
    public bool AllowEmpty { get; set; }
}

[Flags]
public enum SqlServerExtractObjectKinds
{
    None = 0,
    Tables = 1,
    Views = 2,
    Functions = 4,
    StoredProcedures = 8,
    All = Tables | Views | Functions | StoredProcedures
}
