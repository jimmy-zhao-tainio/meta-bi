using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using MetaSchema.Instance;
using MS = global::MetaSchema;

namespace MetaSchema.Extractors.SqlServer;

public sealed class MetaSchemaSqlServerExtractService
{
    private readonly SqlServerSchemaExtractor extractor;

    public MetaSchemaSqlServerExtractService()
        : this(new SqlServerSchemaExtractor())
    {
    }

    public MetaSchemaSqlServerExtractService(SqlServerSchemaExtractor extractor)
    {
        this.extractor = extractor;
    }

    public async Task<SqlServerExtractResult> ExtractToNewWorkspaceAsync(
        SqlServerExtractRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.NewWorkspacePath);
        cancellationToken.ThrowIfCancellationRequested();

        var workspacePath = Path.GetFullPath(request.NewWorkspacePath);
        var model = extractor.ExtractMetaSchemaModel(request);
        await MetaSchemaInstance.SaveToWorkspaceAsync(model, workspacePath, cancellationToken)
            .ConfigureAwait(false);

        return new SqlServerExtractResult(
            workspacePath,
            model.SystemList.Count,
            model.SchemaList.Count,
            model.TableList.Count,
            model.FieldList.Count,
            model.TableKeyList.Count,
            model.TableRelationshipList.Count);
    }
}

public sealed record SqlServerExtractResult(
    string WorkspacePath,
    int SystemCount,
    int SchemaCount,
    int TableCount,
    int FieldCount,
    int TableKeyCount,
    int TableRelationshipCount);

public sealed class SqlServerSchemaExtractor
{
    public MS.MetaSchemaModel ExtractMetaSchemaModel(SqlServerExtractRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.ConnectionString))
        {
            throw new InvalidOperationException("extract sqlserver requires a connection string.");
        }

        if (string.IsNullOrWhiteSpace(request.SystemName))
        {
            throw new InvalidOperationException("extract sqlserver requires --system <name>.");
        }

        if (string.IsNullOrWhiteSpace(request.SchemaName) && !request.AllSchemas)
        {
            throw new InvalidOperationException("extract sqlserver requires --schema <name> or --all-schemas.");
        }

        if (!string.IsNullOrWhiteSpace(request.SchemaName) && request.AllSchemas)
        {
            throw new InvalidOperationException("extract sqlserver does not allow --schema with --all-schemas.");
        }

        if (string.IsNullOrWhiteSpace(request.TableName) && !request.AllTables)
        {
            throw new InvalidOperationException("extract sqlserver requires --table <name> or --all-tables.");
        }

        if (!string.IsNullOrWhiteSpace(request.TableName) && request.AllTables)
        {
            throw new InvalidOperationException("extract sqlserver does not allow --table with --all-tables.");
        }

        using var connection = new SqlConnection(request.ConnectionString);
        connection.Open();

        var databaseName = string.IsNullOrWhiteSpace(connection.Database)
            ? "(default)"
            : connection.Database;
        var dataSource = connection.DataSource ?? string.Empty;
        var systemName = request.SystemName.Trim();
        var schemaFilter = request.AllSchemas ? null : request.SchemaName.Trim();
        var tableFilter = request.AllTables ? null : request.TableName.Trim();
        var tableRows = LoadTables(connection, schemaFilter, tableFilter)
            .OrderBy(static row => row.SchemaName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static row => row.SchemaName, StringComparer.Ordinal)
            .ThenBy(static row => row.TableName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static row => row.TableName, StringComparer.Ordinal)
            .ToList();
        if (tableRows.Count == 0)
        {
            var filterDescription = (schemaFilter, tableFilter) switch
            {
                (null, null) => "any schema/table filter",
                (not null, null) => $"schema '{schemaFilter}'",
                (null, not null) => $"table '{tableFilter}'",
                (not null, not null) => $"table '{schemaFilter}.{tableFilter}'",
            };
            throw new InvalidOperationException(
                $"No SQL Server tables matched {filterDescription} in database '{connection.Database}'.");
        }

        var columnsByTableKey = tableRows.ToDictionary(
            row => BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadColumns(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);
        var tableKeysByTableKey = tableRows.ToDictionary(
            row => BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadTableKeys(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);
        var tableKeyFieldsByTableKey = tableRows.ToDictionary(
            row => BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadTableKeyFields(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);
        var foreignKeysByTableKey = tableRows.ToDictionary(
            row => BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadForeignKeys(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);
        var foreignKeyColumnsByTableKey = tableRows.ToDictionary(
            row => BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadForeignKeyColumns(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);

        var model = MS.MetaSchemaModel.CreateEmpty();
        var system = new MS.System
        {
            Id = BuildSystemId(systemName),
            Name = systemName,
            Description = string.IsNullOrWhiteSpace(dataSource) ? null : databaseName + " @ " + dataSource
        };
        model.SystemList.Add(system);

        var schemasByName = new Dictionary<string, MS.Schema>(StringComparer.Ordinal);
        var schemaNames = tableRows
            .Select(static row => row.SchemaName)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal);
        foreach (var schemaName in schemaNames)
        {
            var schema = new MS.Schema
            {
                Id = BuildSchemaId(databaseName, schemaName),
                Name = schemaName,
                System = system
            };
            model.SchemaList.Add(schema);
            schemasByName.Add(schemaName, schema);
        }

        var tablesByKey = new Dictionary<string, MS.Table>(StringComparer.OrdinalIgnoreCase);
        foreach (var tableRow in tableRows)
        {
            var tableKey = BuildScopedObjectKey(tableRow.SchemaName, tableRow.TableName);
            var table = new MS.Table
            {
                Id = BuildTableId(databaseName, tableRow.SchemaName, tableRow.TableName),
                Name = tableRow.TableName,
                ObjectType = string.IsNullOrWhiteSpace(tableRow.ObjectType) ? null : tableRow.ObjectType,
                Schema = schemasByName[tableRow.SchemaName]
            };
            model.TableList.Add(table);
            tablesByKey.Add(tableKey, table);
        }

        var fieldsByColumnNameByTableKey = new Dictionary<string, Dictionary<string, MS.Field>>(StringComparer.OrdinalIgnoreCase);
        foreach (var tableRow in tableRows)
        {
            var tableKey = BuildScopedObjectKey(tableRow.SchemaName, tableRow.TableName);
            var table = tablesByKey[tableKey];
            var fieldsByColumnName = new Dictionary<string, MS.Field>(StringComparer.OrdinalIgnoreCase);
            fieldsByColumnNameByTableKey.Add(tableKey, fieldsByColumnName);

            foreach (var columnRow in columnsByTableKey[tableKey]
                         .OrderBy(static row => row.TableName, StringComparer.OrdinalIgnoreCase)
                         .ThenBy(static row => row.TableName, StringComparer.Ordinal)
                         .ThenBy(static row => row.OrdinalPosition))
            {
                var field = new MS.Field
                {
                    Id = BuildFieldId(databaseName, columnRow.SchemaName, columnRow.TableName, columnRow.ColumnName),
                    Name = columnRow.ColumnName,
                    MetaDataTypeId = BuildDataTypeId(columnRow.DataTypeName),
                    Ordinal = columnRow.OrdinalPosition.ToString(CultureInfo.InvariantCulture),
                    IsNullable = columnRow.IsNullable ? "true" : "false",
                    IsIdentity = columnRow.IsIdentity ? "true" : null,
                    IdentitySeed = string.IsNullOrWhiteSpace(columnRow.IdentitySeed) ? null : columnRow.IdentitySeed,
                    IdentityIncrement = string.IsNullOrWhiteSpace(columnRow.IdentityIncrement) ? null : columnRow.IdentityIncrement,
                    Table = table
                };

                model.FieldList.Add(field);
                fieldsByColumnName.Add(columnRow.ColumnName, field);
                AddFieldDataTypeDetails(model, field, columnRow);
            }
        }

        foreach (var tableRow in tableRows)
        {
            var tableKey = BuildScopedObjectKey(tableRow.SchemaName, tableRow.TableName);
            var table = tablesByKey[tableKey];
            var fieldsByColumnName = fieldsByColumnNameByTableKey[tableKey];
            var keyFieldsByName = tableKeyFieldsByTableKey[tableKey]
                .GroupBy(static row => row.KeyName, StringComparer.Ordinal)
                .ToDictionary(
                    static group => group.Key,
                    static group => group.OrderBy(static item => item.Ordinal).ToList(),
                    StringComparer.Ordinal);

            foreach (var keyRow in tableKeysByTableKey[tableKey]
                         .OrderBy(static row => row.KeyType, StringComparer.Ordinal)
                         .ThenBy(static row => row.Name, StringComparer.Ordinal))
            {
                var tableKeyRow = new MS.TableKey
                {
                    Id = BuildTableKeyId(databaseName, tableRow.SchemaName, tableRow.TableName, keyRow.Name),
                    Name = keyRow.Name,
                    KeyType = keyRow.KeyType,
                    Table = table
                };
                model.TableKeyList.Add(tableKeyRow);

                if (!keyFieldsByName.TryGetValue(keyRow.Name, out var keyFields))
                {
                    continue;
                }

                foreach (var keyField in keyFields)
                {
                    if (!fieldsByColumnName.TryGetValue(keyField.ColumnName, out var field))
                    {
                        throw new InvalidOperationException(
                            $"SQL Server key '{tableRow.SchemaName}.{tableRow.TableName}.{keyRow.Name}' referenced column '{keyField.ColumnName}' that was not extracted.");
                    }

                    model.TableKeyFieldList.Add(new MS.TableKeyField
                    {
                        Id = BuildTableKeyFieldId(
                            databaseName,
                            tableRow.SchemaName,
                            tableRow.TableName,
                            keyRow.Name,
                            keyField.Ordinal),
                        Ordinal = keyField.Ordinal.ToString(CultureInfo.InvariantCulture),
                        FieldName = keyField.ColumnName,
                        TableKey = tableKeyRow,
                        Field = field
                    });
                }
            }
        }

        foreach (var tableRow in tableRows)
        {
            var tableKey = BuildScopedObjectKey(tableRow.SchemaName, tableRow.TableName);
            var sourceTable = tablesByKey[tableKey];
            var sourceFieldsByColumnName = fieldsByColumnNameByTableKey[tableKey];
            var foreignKeyColumnsByName = foreignKeyColumnsByTableKey[tableKey]
                .GroupBy(static row => row.ForeignKeyName, StringComparer.Ordinal)
                .ToDictionary(
                    static group => group.Key,
                    static group => group.OrderBy(static item => item.Ordinal).ToList(),
                    StringComparer.Ordinal);

            foreach (var foreignKey in foreignKeysByTableKey[tableKey]
                         .OrderBy(static row => row.Name, StringComparer.Ordinal))
            {
                var targetTableKey = BuildScopedObjectKey(foreignKey.TargetSchemaName, foreignKey.TargetTableName);
                if (!tablesByKey.TryGetValue(targetTableKey, out var targetTable))
                {
                    continue;
                }

                var relationship = new MS.TableRelationship
                {
                    Id = BuildRelationshipId(databaseName, tableRow.SchemaName, tableRow.TableName, foreignKey.Name),
                    Name = foreignKey.Name,
                    SourceTable = sourceTable,
                    TargetTable = targetTable
                };
                model.TableRelationshipList.Add(relationship);

                if (!foreignKeyColumnsByName.TryGetValue(foreignKey.Name, out var fkColumns))
                {
                    continue;
                }

                var targetFieldsByColumnName = fieldsByColumnNameByTableKey[targetTableKey];
                foreach (var fkColumn in fkColumns)
                {
                    if (!sourceFieldsByColumnName.TryGetValue(fkColumn.SourceColumnName, out var sourceField))
                    {
                        throw new InvalidOperationException(
                            $"SQL Server foreign key '{tableRow.SchemaName}.{tableRow.TableName}.{foreignKey.Name}' referenced source column '{fkColumn.SourceColumnName}' that was not extracted.");
                    }

                    if (!targetFieldsByColumnName.TryGetValue(fkColumn.TargetColumnName, out var targetField))
                    {
                        throw new InvalidOperationException(
                            $"SQL Server foreign key '{tableRow.SchemaName}.{tableRow.TableName}.{foreignKey.Name}' referenced target column '{fkColumn.TargetColumnName}' that was not extracted.");
                    }

                    model.TableRelationshipFieldList.Add(new MS.TableRelationshipField
                    {
                        Id = BuildRelationshipFieldId(
                            databaseName,
                            tableRow.SchemaName,
                            tableRow.TableName,
                            foreignKey.Name,
                            fkColumn.Ordinal),
                        Ordinal = fkColumn.Ordinal.ToString(CultureInfo.InvariantCulture),
                        TableRelationship = relationship,
                        SourceField = sourceField,
                        TargetField = targetField
                    });
                }
            }
        }

        return model;
    }

    private static List<TableRow> LoadTables(SqlConnection connection, string? schemaName, string? tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                t.TABLE_SCHEMA,
                t.TABLE_NAME,
                t.TABLE_TYPE
            from INFORMATION_SCHEMA.TABLES t
            join sys.objects o on o.object_id = object_id(quotename(t.TABLE_SCHEMA) + '.' + quotename(t.TABLE_NAME))
            where t.TABLE_TYPE in ('BASE TABLE', 'VIEW')
              and t.TABLE_SCHEMA not in ('sys', 'INFORMATION_SCHEMA')
              and o.is_ms_shipped = 0
              and t.TABLE_NAME <> 'sysdiagrams'
              and (@schemaName is null or t.TABLE_SCHEMA = @schemaName)
              and (@tableName is null or t.TABLE_NAME = @tableName)
            order by t.TABLE_SCHEMA, t.TABLE_NAME
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = string.IsNullOrWhiteSpace(schemaName) ? DBNull.Value : schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = string.IsNullOrWhiteSpace(tableName) ? DBNull.Value : tableName });

        var rows = new List<TableRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new TableRow(
                SchemaName: reader.GetString(0),
                TableName: reader.GetString(1),
                ObjectType: NormalizeTableType(reader.GetString(2))));
        }

        return rows;
    }

    private static List<ColumnRow> LoadColumns(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                s.name as SchemaName,
                o.name as TableName,
                c.name as ColumnName,
                c.column_id as OrdinalPosition,
                c.is_nullable,
                ty.name as DataTypeName,
                COALESCE(TYPE_NAME(c.system_type_id), ty.name) as SystemDataTypeName,
                case
                    when c.max_length = -1 then -1
                    when COALESCE(TYPE_NAME(c.system_type_id), ty.name) in ('nchar', 'nvarchar') then c.max_length / 2
                    else c.max_length
                end as LengthValue,
                case
                    when COALESCE(TYPE_NAME(c.system_type_id), ty.name) in ('decimal', 'numeric') then convert(int, c.precision)
                    when COALESCE(TYPE_NAME(c.system_type_id), ty.name) in ('time', 'datetime2', 'datetimeoffset') then convert(int, c.scale)
                    else null
                end as PrecisionValue,
                case
                    when COALESCE(TYPE_NAME(c.system_type_id), ty.name) in ('decimal', 'numeric') then convert(int, c.scale)
                    else null
                end as ScaleValue,
                c.is_identity,
                convert(nvarchar(50), ic.seed_value) as IdentitySeed,
                convert(nvarchar(50), ic.increment_value) as IdentityIncrement
            from sys.objects o
            join sys.schemas s on s.schema_id = o.schema_id
            join sys.columns c on c.object_id = o.object_id
            join sys.types ty on ty.user_type_id = c.user_type_id
            left join sys.identity_columns ic on ic.object_id = c.object_id and ic.column_id = c.column_id
            where s.name = @schemaName
              and o.name = @tableName
              and o.type in ('U', 'V')
            order by o.name, c.column_id
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<ColumnRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new ColumnRow(
                SchemaName: reader.GetString(0),
                TableName: reader.GetString(1),
                ColumnName: reader.GetString(2),
                OrdinalPosition: ReadInt32(reader, 3),
                IsNullable: reader.GetBoolean(4),
                DataTypeName: reader.GetString(5),
                SystemDataTypeName: reader.GetString(6),
                Length: ReadNullableInt(reader, 7),
                Precision: ReadNullableInt(reader, 8),
                Scale: ReadNullableInt(reader, 9),
                IsIdentity: reader.GetBoolean(10),
                IdentitySeed: ReadNullableString(reader, 11),
                IdentityIncrement: ReadNullableString(reader, 12)));
        }

        return rows;
    }

    private static List<TableKeyRow> LoadTableKeys(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                kc.name as KeyName,
                kc.type as KeyConstraintType
            from sys.key_constraints kc
            join sys.tables srcTable on srcTable.object_id = kc.parent_object_id
            join sys.schemas srcSchema on srcSchema.schema_id = srcTable.schema_id
            where srcSchema.name = @schemaName
              and srcTable.name = @tableName
              and kc.type in ('PK', 'UQ')
            order by
                case kc.type when 'PK' then 0 else 1 end,
                kc.name
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<TableKeyRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new TableKeyRow(
                Name: reader.GetString(0),
                KeyType: NormalizeKeyType(reader.GetString(1))));
        }

        return rows;
    }

    private static List<TableKeyFieldRow> LoadTableKeyFields(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                kc.name as KeyName,
                ic.key_ordinal as Ordinal,
                srcColumn.name as ColumnName
            from sys.key_constraints kc
            join sys.tables srcTable on srcTable.object_id = kc.parent_object_id
            join sys.schemas srcSchema on srcSchema.schema_id = srcTable.schema_id
            join sys.index_columns ic on ic.object_id = kc.parent_object_id and ic.index_id = kc.unique_index_id
            join sys.columns srcColumn on srcColumn.object_id = ic.object_id and srcColumn.column_id = ic.column_id
            where srcSchema.name = @schemaName
              and srcTable.name = @tableName
              and kc.type in ('PK', 'UQ')
              and ic.key_ordinal > 0
            order by kc.name, ic.key_ordinal
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<TableKeyFieldRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new TableKeyFieldRow(
                KeyName: reader.GetString(0),
                Ordinal: ReadInt32(reader, 1),
                ColumnName: reader.GetString(2)));
        }

        return rows;
    }

    private static List<ForeignKeyRow> LoadForeignKeys(SqlConnection connection, string schemaName, string tableName)
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
              and fk.is_disabled = 0
              and fk.is_not_trusted = 0
            order by fk.name
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<ForeignKeyRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new ForeignKeyRow(
                Name: reader.GetString(0),
                TargetSchemaName: reader.GetString(1),
                TargetTableName: reader.GetString(2)));
        }

        return rows;
    }

    private static List<ForeignKeyColumnRow> LoadForeignKeyColumns(SqlConnection connection, string schemaName, string tableName)
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

        var rows = new List<ForeignKeyColumnRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new ForeignKeyColumnRow(
                ForeignKeyName: reader.GetString(0),
                Ordinal: ReadInt32(reader, 1),
                SourceColumnName: reader.GetString(2),
                TargetColumnName: reader.GetString(3)));
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
            _ => Convert.ToInt32(value, CultureInfo.InvariantCulture),
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

    private static string? ReadNullableString(SqlDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);

    private static string NormalizeTableType(string tableType) =>
        tableType switch
        {
            "BASE TABLE" => "Table",
            "VIEW" => "View",
            _ => tableType,
        };

    private static string NormalizeKeyType(string keyConstraintType) =>
        keyConstraintType switch
        {
            "PK" => "primary",
            "UQ" => "unique",
            _ => keyConstraintType,
        };

    private static string BuildSystemId(string databaseName) =>
        "sqlserver:system:" + databaseName;

    private static string BuildSchemaId(string databaseName, string schemaName) =>
        "sqlserver:" + databaseName + ":schema:" + schemaName;

    private static string BuildTableId(string databaseName, string schemaName, string tableName) =>
        "sqlserver:" + databaseName + ":schema:" + schemaName + ":table:" + tableName;

    private static string BuildDataTypeId(string dataTypeName) =>
        "sqlserver:type:" + dataTypeName;

    private static string BuildScopedObjectKey(string schemaName, string objectName) =>
        schemaName + "." + objectName;

    private static string BuildTableKeyId(string databaseName, string schemaName, string tableName, string keyName) =>
        "sqlserver:" + databaseName + ":schema:" + schemaName + ":table:" + tableName + ":key:" + keyName;

    private static string BuildTableKeyFieldId(string databaseName, string schemaName, string tableName, string keyName, int ordinal) =>
        BuildTableKeyId(databaseName, schemaName, tableName, keyName) +
        ":field:" +
        ordinal.ToString(CultureInfo.InvariantCulture);

    private static string BuildFieldId(string databaseName, string schemaName, string tableName, string columnName) =>
        "sqlserver:" + databaseName + ":schema:" + schemaName + ":table:" + tableName + ":field:" + columnName;

    private static void AddFieldDataTypeDetails(MS.MetaSchemaModel model, MS.Field field, ColumnRow columnRow)
    {
        switch (columnRow.SystemDataTypeName.ToLowerInvariant())
        {
            case "char":
            case "varchar":
            case "nchar":
            case "nvarchar":
            case "binary":
            case "varbinary":
                AddFieldDataTypeDetail(model, field, "Length", columnRow.Length);
                break;

            case "decimal":
            case "numeric":
                AddFieldDataTypeDetail(model, field, "Precision", columnRow.Precision);
                AddFieldDataTypeDetail(model, field, "Scale", columnRow.Scale);
                break;

            case "time":
            case "datetime2":
            case "datetimeoffset":
                AddFieldDataTypeDetail(model, field, "Precision", columnRow.Precision);
                break;
        }
    }

    private static void AddFieldDataTypeDetail(MS.MetaSchemaModel model, MS.Field field, string detailName, int? detailValue)
    {
        if (!detailValue.HasValue)
        {
            return;
        }

        model.FieldDataTypeDetailList.Add(new MS.FieldDataTypeDetail
        {
            Id = field.Id + ":detail:" + detailName,
            Name = detailName,
            Value = detailValue.Value.ToString(CultureInfo.InvariantCulture),
            Field = field
        });
    }

    private static string BuildRelationshipId(string databaseName, string schemaName, string tableName, string relationshipName) =>
        "sqlserver:" + databaseName + ":schema:" + schemaName + ":table:" + tableName + ":relationship:" + relationshipName;

    private static string BuildRelationshipFieldId(string databaseName, string schemaName, string tableName, string relationshipName, int ordinal) =>
        BuildRelationshipId(databaseName, schemaName, tableName, relationshipName) +
        ":field:" +
        ordinal.ToString(CultureInfo.InvariantCulture);

    private readonly record struct TableRow(
        string SchemaName,
        string TableName,
        string ObjectType);

    private readonly record struct ColumnRow(
        string SchemaName,
        string TableName,
        string ColumnName,
        int OrdinalPosition,
        bool IsNullable,
        string DataTypeName,
        string SystemDataTypeName,
        int? Length,
        int? Precision,
        int? Scale,
        bool IsIdentity,
        string? IdentitySeed,
        string? IdentityIncrement);

    private readonly record struct ForeignKeyRow(
        string Name,
        string TargetSchemaName,
        string TargetTableName);

    private readonly record struct TableKeyRow(
        string Name,
        string KeyType);

    private readonly record struct TableKeyFieldRow(
        string KeyName,
        int Ordinal,
        string ColumnName);

    private readonly record struct ForeignKeyColumnRow(
        string ForeignKeyName,
        int Ordinal,
        string SourceColumnName,
        string TargetColumnName);
}

public sealed class SqlServerExtractRequest
{
    public string NewWorkspacePath { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public string SchemaName { get; set; } = string.Empty;
    public bool AllSchemas { get; set; }
    public string TableName { get; set; } = string.Empty;
    public bool AllTables { get; set; }
}
